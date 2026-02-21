using System.Diagnostics;

namespace ConectaAtende.Infrastructure.DataStructures;

/// <summary>
/// Classe para comparar a estrutura associativa didática com Dictionary padrão do .NET.
/// </summary>
public class HashTableComparison
{
    /// <summary>
    /// Compara o desempenho entre CustomHashTable e Dictionary padrão.
    /// </summary>
    public static ComparisonResult ComparePerformance(int itemCount)
    {
        var random = new Random(42);
        var keys = new List<string>();
        var values = new List<int>();

        // Gera dados de teste
        for (int i = 0; i < itemCount; i++)
        {
            keys.Add($"Key_{i}_{random.Next(10000)}");
            values.Add(random.Next(1000000));
        }

        // Testa CustomHashTable
        var customTable = new CustomHashTable<string, int>();
        var customInsertTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                customTable.Insert(keys[i], values[i]);
            }
        });

        var customSearchTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                customTable.TryGetValue(keys[i], out _);
            }
        });

        var customRemoveTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount / 2; i++)
            {
                customTable.Remove(keys[i]);
            }
        });

        // Testa Dictionary padrão
        var standardDict = new Dictionary<string, int>();
        var standardInsertTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                standardDict[keys[i]] = values[i];
            }
        });

        var standardSearchTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount; i++)
            {
                standardDict.TryGetValue(keys[i], out _);
            }
        });

        var standardRemoveTime = MeasureTime(() =>
        {
            for (int i = 0; i < itemCount / 2; i++)
            {
                standardDict.Remove(keys[i]);
            }
        });

        return new ComparisonResult
        {
            ItemCount = itemCount,
            CustomHashTable = new PerformanceMetrics
            {
                InsertTime = customInsertTime,
                SearchTime = customSearchTime,
                RemoveTime = customRemoveTime,
                FinalCount = customTable.Count,
                Capacity = customTable.Capacity
            },
            StandardDictionary = new PerformanceMetrics
            {
                InsertTime = standardInsertTime,
                SearchTime = standardSearchTime,
                RemoveTime = standardRemoveTime,
                FinalCount = standardDict.Count,
                Capacity = standardDict.Count
            }
        };
    }

    private static long MeasureTime(Action action)
    {
        var stopwatch = Stopwatch.StartNew();
        action();
        stopwatch.Stop();
        return stopwatch.ElapsedMilliseconds;
    }
}

public class ComparisonResult
{
    public int ItemCount { get; set; }
    public PerformanceMetrics CustomHashTable { get; set; } = new();
    public PerformanceMetrics StandardDictionary { get; set; } = new();
}

public class PerformanceMetrics
{
    public long InsertTime { get; set; }
    public long SearchTime { get; set; }
    public long RemoveTime { get; set; }
    public int FinalCount { get; set; }
    public int Capacity { get; set; }
}
