using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartHome.Models
{
    public class Log
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string? Text { get; set; }
        public DateTime Timestamp { get; set; }
        public bool IsSeen { get; set; }
    }
}
