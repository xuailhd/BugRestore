using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class Action
    {
        public Action()
        {
            Histories = new List<History>();
        }
        public List<History> Histories { get; set; }
        public int id { get; set; }
        public string objectType { get; set; } = "bug";
        public int objectID { get; set; }

        public string product { get; set; } = ",1,";

        public int project { get; set; } = 1;

        public string actor { get; set; }

        public string action { get; set; }
        public DateTime? date { get; set; }

        public string comment { get; set; }

        public string extra { get; set; }
        public int read { get; set; } = 1;

    }
}
