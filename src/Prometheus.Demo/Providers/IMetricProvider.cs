namespace Prometheus.Demo.Providers {
    enum MetricType { counter, gauge, histogram, summary }

    interface IMetricProvider {
        void WriteAll(MetricWriter writer);
    }
}