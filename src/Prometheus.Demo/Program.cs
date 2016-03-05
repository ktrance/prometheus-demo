using CommandLine;
using Nancy;
using Nancy.Hosting.Self;
using Prometheus.Demo.Providers;
using Prometheus.Demo.Providers.PerformanceCounters;
using Prometheus.Demo.Providers.Processes;
using System;

namespace Prometheus.Demo {
    class Program {
        public static Options Options { get; } = new Options();

        static void Main(string[] args) {
            if (!Parser.Default.ParseArguments(args, Options)) {
                return;
            }
            var url = string.Format("http://localhost:{0}", Options.Port);
            if (Options.Verbose) {
                Console.WriteLine("opening listener on {0}", url);
            }
            using (var host = new NancyHost(new HostConfiguration { UrlReservations = new UrlReservations { CreateAutomatically = true } }, new Uri(url))) {
                host.Start();
                Console.ReadLine();
            }
        }
    }

    public class PrometheusModule : NancyModule {
        private readonly MetricWriter _writer = new MetricWriter();
        private  CompositeMetricProvider _providers;
        public PrometheusModule() {
            BuildProviders();
            Get["/metrics"] = _ => { return ServeStats(); };
        }

        private void BuildProviders() {
            _providers = new CompositeMetricProvider(
                new PerformanceCounterProviderBuilder().AddFromXml(Program.Options.Counterfile).Build(),
                new ProcessMetricProvider(
                    new ProcessCounter { Name = "process_total_cpu_user", Help = "Total milliseconds of user time.", DataFunc = p => p.UserProcessorTime.TotalMilliseconds, Type = MetricType.counter },
                    new ProcessCounter { Name = "process_total_cpu_kernel", Help = "Total milliseconds of kernel time.", DataFunc = p => p.PrivilegedProcessorTime.TotalMilliseconds, Type = MetricType.counter },
                    new ProcessCounter { Name = "process_total_virtual", Help = "Total virtual footprint in kb.", DataFunc = p => p.VirtualMemorySize64, Type = MetricType.gauge },
                    new ProcessCounter { Name = "process_total_private", Help = "Total private footprint in kb.", DataFunc = p => p.PrivateMemorySize64, Type = MetricType.gauge }
                    ));
        }

        private string ServeStats() {
            if (Program.Options.Verbose) {
                Console.WriteLine("{0}: request recieved", DateTime.Now);
            }
            _providers.WriteAll(_writer);
            return _writer.Flush();
        }
    }
}
