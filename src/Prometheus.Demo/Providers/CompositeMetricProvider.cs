using System;

namespace Prometheus.Demo.Providers {
    class CompositeMetricProvider : IMetricProvider {
        private IMetricProvider[] _providers;
        public CompositeMetricProvider(params IMetricProvider[] providers) {
            if (providers == null || providers.Length == 0) {
                throw new ArgumentException("cannot be null or zero length", nameof(providers));
            }
            _providers = providers;
        }

        public void WriteAll(MetricWriter writer) {
            foreach (var provider in _providers) {
                provider.WriteAll(writer);
            }
        }
    }
}
