# Psi

## Benchmarks

``` ini

BenchmarkDotNet=v0.13.3, OS=Windows 11 (10.0.25193.1000)
Intel Core i7-8550U CPU 1.80GHz (Kaby Lake R), 1 CPU, 8 logical and 4 physical cores
.NET SDK=7.0.101
  [Host]     : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2
  DefaultJob : .NET 6.0.12 (6.0.1222.56807), X64 RyuJIT AVX2


```
|                      Method |     Mean |    Error |   StdDev |    Gen0 |    Gen1 | Allocated |
|---------------------------- |---------:|---------:|---------:|--------:|--------:|----------:|
| CreateContractAndCallMethod | 113.2 ms | 18.02 ms | 53.13 ms | 93.7500 | 31.2500 | 491.28 KB |
