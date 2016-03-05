using System;
using System.Diagnostics;

namespace Prometheus.Demo.Providers.PerformanceCounters {
    enum PerformanceCounterType { raw, next, sample };

    /// <summary>
    /// https://msdn.microsoft.com/en-ca/library/xb29hack(v=vs.100).aspx
    /// https://msdn.microsoft.com/en-ca/library/f77ezfb7(v=vs.100).aspx
    /// </summary>
    abstract class CounterBase {
        public CounterBase(string name, string help, MetricType type, PerformanceCounter counter) {
            if (string.IsNullOrEmpty(name)) {
                throw new ArgumentException("canot be null or empty", nameof(name));
            }
            if (string.IsNullOrEmpty(help)) {
                throw new ArgumentException("canot be null or empty", nameof(help));
            }
            if (counter == null) {
                throw new ArgumentNullException(nameof(counter));
            }

            Name = name;
            Help = help;
            Type = type;
            _counter = counter;
        }
        public string Name { get; private set; }
        public string Help { get; private set; }
        public MetricType Type { get; private set; }
        protected PerformanceCounter _counter;
        public abstract float GetValue();
    }

    class NextCounter : CounterBase {
        public NextCounter(string name, string help, MetricType type, PerformanceCounter counter) :
            base(name, help, type, counter) {
            _counter.NextValue();
        }

        public override float GetValue() {
            return _counter.NextValue();
        }
    }

    class RawCounter : CounterBase {
        public RawCounter(string name, string help, MetricType type, PerformanceCounter counter) :
            base(name, help, type, counter) { }

        public override float GetValue() {
            return _counter.RawValue;
        }
    }

    class SampleCounter : CounterBase {
        private CounterSample _previous;
        public SampleCounter(string name, string help, MetricType type, PerformanceCounter counter) :
            base(name, help, type, counter) {
            _previous = _counter.NextSample();
        }

        public override float GetValue() {
            var current = _counter.NextSample();
            var result = CounterSample.Calculate(_previous, current);
            _previous = current;
            return result;
        }
    }
}
