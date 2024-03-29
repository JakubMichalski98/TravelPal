﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TravelPal.Interfaces;

namespace TravelPal.Models
{
    public class TravelDocument : IPackingListItem
    {
        public string Name { get; set; }
        public bool Required { get; set; }

        public TravelDocument(string name, bool required)
        {
            Name = name;
            Required = required;
        }

        // Returns interpolated string containing the name of the traveldocument as well as whether it is required or not
        public string GetInfo()
        {
            string requiredMessage;
            if (Required)
            {
                requiredMessage = "Required";
            }
            else
            {
                requiredMessage = "Not Required";
            }

            return $"{Name} | {requiredMessage}";
        }
    }
}
