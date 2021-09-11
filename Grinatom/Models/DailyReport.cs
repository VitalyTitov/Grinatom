using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grinatom.Models
{
    public class DailyReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Timesheet { get; set; }
        public string Date { get; set; }
        public string Hours { get; set; }
    }
}
