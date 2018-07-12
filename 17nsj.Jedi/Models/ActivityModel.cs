using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _17nsj.Jedi.Models
{
    public class ActivityModel
    {
        public string Category { get; set; }
        public string CategoryName { get; set; }
        public string CategoryColor { get; set; }
        public int Id { get; set; }
        public string ThumbnailURL { get; set; }
        public string Title { get; set; }
        public string MediaURL { get; set; }
        public string Outline { get; set; }
        public string Term { get; set; }
        public string Location { get; set; }
        public string MapURL { get; set; }
        public string RelationalURL { get; set; }
        public bool CanWaitable { get; set; }
        public Nullable<bool> IsClosed { get; set; }
        public Nullable<int> WaitingTime { get; set; }
        public Nullable<System.DateTime> WaitingInfoUpdatedAt { get; set; }
        public bool IsAvailable { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
    }
}
