using Nancy;
using Nancy.Hosting.Self;
using Prometheus.Demo.Providers;
using Prometheus.Demo.Providers.PerformanceCounters;
using Prometheus.Demo.Providers.Processes;
using System;

namespace Prometheus.Demo {
    public class Program {
        static void Main(string[] args) {
            using (var host = new NancyHost(new HostConfiguration { UrlReservations = new UrlReservations { CreateAutomatically = true } }, new Uri("http://localhost:9999"))) {
                host.Start();
                Console.ReadLine();
            }
        }
    }

    public class PrometheusModule : NancyModule {
        private readonly MetricWriter _writer = new MetricWriter();
        private  CompositeMetricProvider _providers;
        public PrometheusModule() {
            SetupProviders();
            Get["/metrics"] = _ => { return ServeStats(); };
        }

        private void SetupProviders() {
            _providers = new CompositeMetricProvider(
                new PerformanceCounterProviderBuilder().AddFromXml("counters.xml").Build(),
                new ProcessMetricProvider(
                    new ProcessCounter { Name = "process_total_cpu_user", Help = "Total milliseconds of user time.", DataFunc = p => p.UserProcessorTime.TotalMilliseconds, Type = MetricType.counter },
                    new ProcessCounter { Name = "process_total_cpu_kernel", Help = "Total milliseconds of kernel time.", DataFunc = p => p.PrivilegedProcessorTime.TotalMilliseconds, Type = MetricType.counter },
                    new ProcessCounter { Name = "process_total_virtual", Help = "Total virtual footprint in kb.", DataFunc = p => p.VirtualMemorySize64, Type = MetricType.gauge },
                    new ProcessCounter { Name = "process_total_private", Help = "Total private footprint in kb.", DataFunc = p => p.PrivateMemorySize64, Type = MetricType.gauge }
                    ));
        }

        private string ServeStats() {
            Console.WriteLine("{0}: request recieved", DateTime.Now);
            _providers.WriteAll(_writer);
            return _writer.Flush();
        }
    }
}
