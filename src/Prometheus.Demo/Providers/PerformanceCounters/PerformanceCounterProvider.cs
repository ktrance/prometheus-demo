using System;
using System.Linq;

namespace Prometheus.Demo.Providers.PerformanceCounters {
    class PerformanceCounterProvider : IMetricProvider {
        private CounterBase[] _counters;

        public PerformanceCounterProvider(params CounterBase[] counters) {
            if (counters == null || counters.Length == 0) {
                throw new ArgumentException("cannot be null or zero length", nameof(counters));
            }
            _counters = counters;
        }

        public void WriteAll(MetricWriter writer) {
            foreach (var counter in _counters) {
                writer.WriteHeader(counter.Name, counter.Help, counter.Type);
                writer.WriteCounter(counter.Name, Enumerable.Empty<string>(), counter.GetValue());
            }
        }   
    }
}
