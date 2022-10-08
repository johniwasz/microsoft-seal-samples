using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerTests
{
    internal class LoggerUtilties
    {
        internal static readonly ILoggerFactory _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
            .AddDebug();
        });


    }
}
