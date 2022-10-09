using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Models
{
    public class RunEntry
    {

        public RunEntry()
        {

        }

        public RunEntry(int distance, int time)
        {
            Distance = distance;
            Time = time;
        }

        public int Distance { get; set; }

        public int Time { get; set; }

        
    }
}
