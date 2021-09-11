using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grinatom.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Timesheet { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Date { get; set; }

        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
