using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class Bug
    {
        public Bug()
        {
            Actions = new List<Action>();
            AFiles = new List<AFile>();
        }

        public List<Action> Actions { get; set; }
        public List<AFile> AFiles { get; set; }

        public int id { get; set; }
        public int product { get; set; } = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["ProductID"]);
        public int branch { get; set; } = 0;

        public int module { get; set; } = 0;

        public int project { get; set; } = 0;

        public int plan { get; set; } = 0;
        public int story { get; set; } = 0;
        public int storyVersion { get; set; } = 1;
        public int task { get; set; } = 0;
        public int toTask { get; set; } = 0;
        public int toStory { get; set; } = 0;

        public string title { get; set; }

        public string keywords { get; set; } = "";

        public int severity { get; set; }

        public int pri { get; set; }

        public string type { get; set; }
        public string os { get; set; } = "";

        public string browser { get; set; } = "";
        public string hardware { get; set; } = "";
        public string found { get; set; } = "";
        public string steps { get; set; }
        public string status { get; set; }

        public string color { get; set; } = "";
        public int confirmed { get; set; }

        public int activatedCount { get; set; }
        public string mailto { get; set; } = "";
        public string openedBy { get; set; }

        public DateTime? openedDate { get; set; }

        public string openedBuild { get; set; } = "trunk";
        public string assignedTo { get; set; }
        public DateTime? assignedDate { get; set; }
        public string resolvedBy { get; set; }
        public string resolution { get; set; }
        public string resolvedBuild { get; set; } = "trunk";

        public DateTime? resolvedDate { get; set; }

        public string closedBy { get; set; }
        public DateTime? closedDate { get; set; }
        public int duplicateBug { get; set; } = 0;

        public string linkBug { get; set; } = "";
        public int Case { get; set; } = 0;

        public int caseVersion { get; set; } = 0;

        public int result { get; set; } = 0;
        public int testtask { get; set; } = 0;

        public string lastEditedBy { get; set; }
        public DateTime? lastEditedDate { get; set; }
    }
}
