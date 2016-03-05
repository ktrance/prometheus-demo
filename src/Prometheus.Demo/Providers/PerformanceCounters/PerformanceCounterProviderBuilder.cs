using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Prometheus.Demo.Providers.PerformanceCounters {
    class PerformanceCounterProviderBuilder {
        private List<CounterBase> _counters = new List<CounterBase>();
        public PerformanceCounterProviderBuilder AddFromXml(string xml) {
            if (string.IsNullOrEmpty(xml)) {
                throw new ArgumentException("cannot be null or empty", nameof(xml));
            }
            XmlDocument document = new XmlDocument();
            document.Load(xml);
            foreach (XmlNode node in document.SelectNodes("//counter")) {
                var counter = new PerformanceCounter {
                    CategoryName = node.Attributes["category"].Value,
                    CounterName = node.Attributes["countername"].Value,
                    InstanceName = node.Attributes["instance"].Value
                };

                _counters.Add(Create(
                    node.Attributes["name"].Value,
                    node.Attributes["help"].Value,
                    node.Attributes["type"].Value,
                    counter,
                    node.Attributes["perftype"].Value)
                );
            }
            return this;
        }

        private static CounterBase Create(string name, string help, string type, PerformanceCounter counter, string perftype) {
            return Create(name,
                help,
                (MetricType)Enum.Parse(typeof(MetricType), type),
                counter,
                (PerformanceCounterType)Enum.Parse(typeof(PerformanceCounterType), perftype));
        }

        private static CounterBase Create(string name, string help, MetricType type, PerformanceCounter counter, PerformanceCounterType perftype) {
            switch (perftype) {
                case PerformanceCounterType.raw:
                    return new RawCounter(name, help, type, counter);
                case PerformanceCounterType.next:
                    return new NextCounter(name, help, type, counter);
                case PerformanceCounterType.sample:
                    return new SampleCounter(name, help, type, counter);
                default:
                    throw new InvalidOperationException();
            }
        }

        public PerformanceCounterProviderBuilder Add(CounterBase counter) {
            _counters.Add(counter);
            return this;
        }

        public PerformanceCounterProviderBuilder Add(string name, string help, MetricType type, PerformanceCounter counter, PerformanceCounterType perftype) {
            _counters.Add(Create(name, help, type, counter, perftype));
            return this;
        }

        public PerformanceCounterProvider Build() {
            return new PerformanceCounterProvider(_counters.ToArray());
        }
    }
}
