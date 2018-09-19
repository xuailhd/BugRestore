using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore.Model
{
    public class ZenTaoFile
    {
        public int Id { get; set; }

        public string Pathname { get; set; }

        public string Title { get; set; }

        public string Extension { get; set; }

        public int Size { get; set; }

        public string ObjectType { get; set; }

        public int ObjectID { get; set; }

        public string AddBy { get; set; }

        public DateTime AddedDate { get; set; }

        public int Downloads { get; set; }

        public string Extra { get; set; }

        public string Deleted { get; set; }
    }
}
