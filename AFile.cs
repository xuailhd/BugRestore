using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class AFile
    {
        public int id { get; set; }
        public string pathname { get; set; }
        public string title { get; set; }
        public string extension { get; set; }

        public int size { get; set; }
        public string objectType { get; set; } = "bug";
        public int objectID { get; set; }
        public string addedBy { get; set; } = "admin";

        public DateTime? addedDate { get; set; } = DateTime.Now;

        public int downloads { get; set; }

        public int deleted { get; set; }
    }
}
