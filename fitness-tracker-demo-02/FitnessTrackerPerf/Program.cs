// See https://aka.ms/new-console-template for more information
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using FitnessTrackerPerf;
using Microsoft.Extensions.Logging;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Toolchains.InProcess.NoEmit;

// There appears to be a bug in Benchmark.NET with this configuation option
var config = ManualConfig.Create(DefaultConfig.Instance)
.WithOptions(ConfigOptions.DisableOptimizationsValidator)
           .AddJob(Job.MediumRun
                .WithToolchain(InProcessNoEmitToolchain.Instance)
                .WithId("InProcess"));

// var summary = BenchmarkRunner.Run<EncryptBenchmark>();
//Console.WriteLine("Starting");

BenchmarkRunner.Run<EncryptBenchmark>();
BenchmarkRunner.Run<MultiplyBenchmark>();