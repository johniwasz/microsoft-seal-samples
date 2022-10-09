using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace FitnessTrackerTests.Logging
{
    internal class LoggerUtilties
    {

        internal static ILoggerFactory GetXUnitLoggerFactory(ITestOutputHelper testOutputHelper)
        {
            ILoggerFactory factory = new LoggerFactory(new List<ILoggerProvider>() { new XUnitLoggerProvider(testOutputHelper) });
            return factory;
        }

        internal static ILogger<T> CreateXUnitLogger<T>(ITestOutputHelper testOutputHelper)
        {
            ILoggerFactory loggerFactory = GetXUnitLoggerFactory(testOutputHelper);

            return loggerFactory.CreateLogger<T>();
        }


    }
}
