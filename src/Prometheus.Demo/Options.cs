using CommandLine;
using CommandLine.Text;

namespace Prometheus.Demo {

    class Options {
        [Option('p', "port", Required = false, DefaultValue = 9999, HelpText = "port to expose")]
        public int Port { get; set; }

        [Option('f', "file", Required = false, DefaultValue = "perf_counters.xml", HelpText = "counter file")]
        public string Counterfile { get; set; }

        [Option('v', "verbose", Required = false, DefaultValue = false)]
        public bool Verbose { get; set; }

        [HelpOption]
        public string GetUsage() {
            var help = new HelpText {
                Heading = new HeadingInfo("Prometheus.Demo", "1.0.0.0"),
                AdditionalNewLineAfterOption = false,
                AddDashesToOption = true
            };
            help.AddOptions(this);
            return help;
        }
    }
}
