using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeperEditor.Models
{
    public class Employee
    {
        public string PersonId { get; set; }
        public string BioId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ScheduledTimeIn { get; set; }
        public string ScheduledTimeOut { get; set; }

        public override string ToString()
        {
            return $"{LastName}, {FirstName} {MiddleName}".Trim();
        }
    }
}
