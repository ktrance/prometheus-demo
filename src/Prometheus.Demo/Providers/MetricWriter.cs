using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Prometheus.Demo.Providers {
    class MetricWriter {
        private static readonly NumberFormatInfo _formatInfo = new NumberFormatInfo() { NumberDecimalSeparator = "." };
        private static string[] _fixedLabels = new[] { string.Format("machine=\"{0}\"", Environment.MachineName) };
        private StringBuilder _buffer = new StringBuilder();

        public void WriteHeader(string name, string help, MetricType type) {
            _buffer.Append(string.Format("#HELP {0} {1}\n", name, help));
            _buffer.Append(string.Format("#TYPE {0} {1}\n", name, type.ToString()));
        }

        public void WriteCounter(string name, IEnumerable<string> labels, double data) {
            _buffer.Append(string.Format("{0} {{{1}}} {2}\n", name, string.Join(",", labels.Concat(_fixedLabels)), data));
        }

        public string Flush() {
            var content = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(_buffer.ToString()));
            _buffer.Clear();
            return content;
        }
    }
}
