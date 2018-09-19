using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class History
    {
        public int id { get; set; }
        public int action { get; set; }
        public string field { get; set; }
        public string old { get; set; }
        public string New { get; set; }
    }
}
