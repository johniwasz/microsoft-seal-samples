using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitnessTracker.Common.Models
{
    public class EncryptedRunInfoCKKS
    {

        public Ciphertext Time { get; set; }

        public Ciphertext Distance { get; set; }

        public Ciphertext Speed { get; set; }
    }
}
