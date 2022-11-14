using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Models
{
    public class RunEntry<T>
    {

        public RunEntry()
        {

        }

        public RunEntry(T distance, T time)
        {
            Distance = distance;
            Time = time;
        }

        public T Distance { get; set; }

        public T Time { get; set; }

        
    }
}
