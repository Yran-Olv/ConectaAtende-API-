using BenchmarkDotNet.Running;
using ConectaAtende.Benchmarks.Benchmarks;

Console.WriteLine("Executando benchmarks de Contatos...");
var summary1 = BenchmarkRunner.Run<ContactBenchmarks>();

Console.WriteLine("\nExecutando benchmarks de HashTable...");
var summary2 = BenchmarkRunner.Run<HashTableBenchmarks>();

Console.WriteLine("\nBenchmarks concluídos!");
