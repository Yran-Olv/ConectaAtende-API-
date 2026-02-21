using BenchmarkDotNet.Attributes;
using ConectaAtende.Infrastructure.DataStructures;

namespace ConectaAtende.Benchmarks.Benchmarks;

[MemoryDiagnoser]
[SimpleJob]
public class HashTableBenchmarks
{
    private CustomHashTable<string, int> _customHashTable = null!;
    private Dictionary<string, int> _standardDictionary = null!;
    private List<string> _keys = new();

    [GlobalSetup]
    public void Setup()
    {
        _customHashTable = new CustomHashTable<string, int>();
        _standardDictionary = new Dictionary<string, int>();
        
        var random = new Random(42);
        for (int i = 0; i < 1000; i++)
        {
            _keys.Add($"Key_{i}_{random.Next(10000)}");
        }
    }

    [Benchmark(Baseline = true)]
    public void CustomHashTable_Insert()
    {
        for (int i = 0; i < 1000; i++)
        {
            _customHashTable.Insert(_keys[i], i);
        }
    }

    [Benchmark]
    public void StandardDictionary_Insert()
    {
        for (int i = 0; i < 1000; i++)
        {
            _standardDictionary[_keys[i]] = i;
        }
    }

    [Benchmark]
    public void CustomHashTable_Search()
    {
        for (int i = 0; i < 100; i++)
        {
            _customHashTable.TryGetValue(_keys[i], out _);
        }
    }

    [Benchmark]
    public void StandardDictionary_Search()
    {
        for (int i = 0; i < 100; i++)
        {
            _standardDictionary.TryGetValue(_keys[i], out _);
        }
    }

    [Benchmark]
    public void CustomHashTable_Remove()
    {
        for (int i = 0; i < 100; i++)
        {
            _customHashTable.Remove(_keys[i]);
        }
    }

    [Benchmark]
    public void StandardDictionary_Remove()
    {
        for (int i = 0; i < 100; i++)
        {
            _standardDictionary.Remove(_keys[i]);
        }
    }
}
