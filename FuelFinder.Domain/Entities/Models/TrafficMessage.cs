using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuelFinder.Domain.Entities;
using System.Threading.Tasks;

namespace FuelFinder.Domain.Entities.Models
{
    public class TrafficMessage
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Priority { get; set; }
        public string FormattedTime { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string PriorityText => Priority switch
        {
            1 => "Mycket Allvarlig",
            2 => "Allvarlig",
            3 => "Störning",
            4 => "Information",
            _ => "Meddelande"
        };

        // public string BackGroundColor => Priority <= 2 ? "DimGray" : "White";
        //public string TextColor => Priority <= 2 ? "DarkRed" : "Red";

        public string BackgroundColor => Priority switch
        {
            1 or 2 => "LightCoral",   // röd
            3 => "Moccasin",     // orange/gul
            _ => "Honeydew"      // grön
        };

        public string TextColor => Priority switch
        {
            1 or 2 => "DarkRed",      
            3 => "DarkOrange",   
            _ => "DarkGreen"     
        };


    }
}
