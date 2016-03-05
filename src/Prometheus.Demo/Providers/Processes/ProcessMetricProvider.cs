using System;
using System.Diagnostics;

namespace Prometheus.Demo.Providers.Processes {
    class ProcessMetricProvider : IMetricProvider {
        private readonly ProcessCounter[] _counters;

        public ProcessMetricProvider(params ProcessCounter[] counters) {
            if (counters == null || counters.Length == 0) {
                throw new ArgumentException("cannot be null or zero length", nameof(counters));
            }
            _counters = counters;
        }

        public void WriteAll(MetricWriter writer) {
            var processes = Process.GetProcesses();

            foreach (var counter in _counters) {
                WriteProcessStats(processes, writer, counter);
            }
            foreach (var process in processes) {
                process.Dispose();
            }
        }

        private void WriteProcessStats(Process[] processes, MetricWriter writer, ProcessCounter counter) {
            writer.WriteHeader(counter.Name, counter.Help, counter.Type);
            foreach (var process in processes) {
                try {
                    writer.WriteCounter(counter.Name, new[] { "process=\"" + process.ProcessName + "\"", "id=\"" + process.Id.ToString() + "\"" }, counter.DataFunc(process));
                } catch { }
            }
        }

        private void WriteProcessStats(Process[] processes, MetricWriter writer, string name, string help, Func<Process, double> datafunc, MetricType type) {
            writer.WriteHeader(name, help, type);
            foreach (var process in processes) {
                try {
                    writer.WriteCounter(name, new[] { "process=\"" + process.ProcessName + "\"", "id=\"" + process.Id.ToString() + "\"" }, datafunc(process));
                } catch { }
            }
        }
    }

    class ProcessCounter {
        public string Name { get; set; }
        public string Help { get; set; }
        public Func<Process, double> DataFunc { get; set; }
        public MetricType Type { get; set; }
    }
}
