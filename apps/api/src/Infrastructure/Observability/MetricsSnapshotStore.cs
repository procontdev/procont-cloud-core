using System.Collections.Concurrent;
using System.Globalization;

namespace Infrastructure.Observability;

public sealed class MetricsSnapshotStore
{
    private readonly ConcurrentDictionary<string, double> values = new(StringComparer.OrdinalIgnoreCase);

    public void Increment(string name, double amount = 1)
        => values.AddOrUpdate(name, amount, (_, current) => current + amount);

    public void Set(string name, double value)
        => values.AddOrUpdate(name, value, (_, _) => value);

    public string ExportPrometheus()
    {
        var lines = values
            .OrderBy(pair => pair.Key, StringComparer.Ordinal)
            .Select(pair => $"{pair.Key} {pair.Value.ToString(CultureInfo.InvariantCulture)}");

        return string.Join(Environment.NewLine, lines) + Environment.NewLine;
    }
}
