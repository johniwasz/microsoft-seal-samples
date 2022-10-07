using Microsoft.Research.SEAL;
using System;
using System.Collections.Generic;
using System.Text;

namespace FitnessTracker.Common.Models
{
    public class ClientData
    {
        public Ciphertext Hours { get; set; }

        public Ciphertext Distance { get; set; }
    }
}
