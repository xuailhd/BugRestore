using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class ZenTaoAction
    {
        public int Id { get; set; }

        public string ObjectType { get; set; }

        public int ObjectId { get; set; }

        public string Product { get; set; }

        public int Project { get; set; }

        public string Actor { get; set; }

        public string Action { get; set; }

        public DateTime Date { get; set; }

        public string comment { get; set; }

        public string Extra { get; set; }

        public string Read { get; set; }
    }
}
