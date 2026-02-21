using System.Collections;

namespace ConectaAtende.Infrastructure.DataStructures;

/// <summary>
/// Estrutura associativa didática implementada manualmente.
/// Implementa uma hash table com buckets e tratamento de colisões por encadeamento.
/// </summary>
public class CustomHashTable<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private const int DefaultCapacity = 16;
    private const double LoadFactorThreshold = 0.75;

    private Bucket<TKey, TValue>[] _buckets;
    private int _count;
    private int _capacity;

    public int Count => _count;
    public int Capacity => _capacity;

    public CustomHashTable(int initialCapacity = DefaultCapacity)
    {
        _capacity = initialCapacity;
        _buckets = new Bucket<TKey, TValue>[_capacity];
        _count = 0;
    }

    /// <summary>
    /// Insere ou atualiza um par chave-valor na tabela.
    /// </summary>
    public void Insert(TKey key, TValue value)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var index = GetBucketIndex(key);
        
        if (_buckets[index] == null)
        {
            _buckets[index] = new Bucket<TKey, TValue>();
        }

        var bucket = _buckets[index];
        var existing = bucket.Find(key);

        if (existing != null)
        {
            // Atualiza valor existente
            existing.Value = value;
        }
        else
        {
            // Insere novo elemento
            bucket.Add(key, value);
            _count++;

            // Verifica se precisa fazer rehash
            if ((double)_count / _capacity >= LoadFactorThreshold)
            {
                Rehash();
            }
        }
    }

    /// <summary>
    /// Busca um valor pela chave.
    /// </summary>
    public bool TryGetValue(TKey key, out TValue? value)
    {
        value = default(TValue);

        if (key == null)
            return false;

        var index = GetBucketIndex(key);
        var bucket = _buckets[index];

        if (bucket == null)
            return false;

        var entry = bucket.Find(key);
        if (entry != null)
        {
            value = entry.Value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Remove um elemento pela chave.
    /// </summary>
    public bool Remove(TKey key)
    {
        if (key == null)
            return false;

        var index = GetBucketIndex(key);
        var bucket = _buckets[index];

        if (bucket == null)
            return false;

        if (bucket.Remove(key))
        {
            _count--;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Verifica se a chave existe na tabela.
    /// </summary>
    public bool ContainsKey(TKey key)
    {
        if (key == null)
            return false;

        var index = GetBucketIndex(key);
        var bucket = _buckets[index];

        return bucket != null && bucket.Find(key) != null;
    }

    /// <summary>
    /// Calcula o índice do bucket usando função hash.
    /// </summary>
    private int GetBucketIndex(TKey key)
    {
        var hashCode = key.GetHashCode();
        // Garante que o índice seja positivo usando módulo
        return Math.Abs(hashCode % _capacity);
    }

    /// <summary>
    /// Realiza rehash quando o fator de carga é excedido.
    /// </summary>
    private void Rehash()
    {
        var oldBuckets = _buckets;
        _capacity *= 2;
        _buckets = new Bucket<TKey, TValue>[_capacity];
        _count = 0;

        foreach (var oldBucket in oldBuckets)
        {
            if (oldBucket != null)
            {
                foreach (var entry in oldBucket)
                {
                    Insert(entry.Key, entry.Value);
                }
            }
        }
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var bucket in _buckets)
        {
            if (bucket != null)
            {
                foreach (var entry in bucket)
                {
                    yield return entry;
                }
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Bucket que armazena entradas e trata colisões por encadeamento.
/// </summary>
internal class Bucket<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    where TKey : notnull
{
    private readonly List<HashEntry<TKey, TValue>> _entries = new();

    public void Add(TKey key, TValue value)
    {
        _entries.Add(new HashEntry<TKey, TValue>(key, value));
    }

    public HashEntry<TKey, TValue>? Find(TKey key)
    {
        return _entries.FirstOrDefault(e => e.Key.Equals(key));
    }

    public bool Remove(TKey key)
    {
        var entry = _entries.FirstOrDefault(e => e.Key.Equals(key));
        if (entry != null)
        {
            return _entries.Remove(entry);
        }
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        foreach (var entry in _entries)
        {
            yield return new KeyValuePair<TKey, TValue>(entry.Key, entry.Value);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

/// <summary>
/// Entrada na hash table (par chave-valor).
/// </summary>
internal class HashEntry<TKey, TValue>
    where TKey : notnull
{
    public TKey Key { get; }
    public TValue Value { get; set; }

    public HashEntry(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
