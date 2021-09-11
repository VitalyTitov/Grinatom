using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Grinatom.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Position { get; set; }
        public int Timesheet { get; set; }

        public int? RoleId { get; set; }
        public Role Role { get; set; }

        public List<Report> Reports { get; set; }
        public User()
        {
            Reports = new List<Report>();
        }

    }
}
