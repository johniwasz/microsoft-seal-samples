// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Running;
using FitnessTrackerPerf;



var summary = BenchmarkRunner.Run<EncryptBenchmark>();