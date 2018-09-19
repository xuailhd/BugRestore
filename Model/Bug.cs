using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BugRestore
{
    public class ZenTaoBug
    {
        public int Id { get; set; }

        public int Product { get; set; }

        public int Branch { get; set; }

        public int Module { get; set; }

        public int Project { get; set; }

        public int Plan { get; set; }

        public int Story { get; set; }

        public int StoryVersion { get; set; }

        public int Task { get; set; }

        public int ToTask { get; set; }

        public int ToStory { get; set; }

        public int Severity { get; set; }

        public int Pri { get; set; }

        public int Confirmed { get; set; }

        public int ActivatedCount { get; set; }

        public int DuplicateBug { get; set; }

        public int Case { get; set; }

        public int CaseVersion { get; set; }

        public int Result { get; set; }

        public int Testtask { get; set; }

        public int Deleted { get; set; }

        public string Title { get; set; }

        public string Keywords { get; set; }

        public string Os { get; set; }

        public string Browser { get; set; }

        public string Hardware { get; set; }

        public string Found { get; set; }

        public string Type { get; set; }

        public string Steps { get; set; }

        public string Status { get; set; }

        public string Color { get; set; }

        public string Mailto { get; set; }

        public string OpenedBy { get; set; }

        public string OpenedBuild { get; set; }

        public string AssignedTo { get; set; }

        public string ResolvedBy { get; set; }

        public string Resolution { get; set; }

        public string ResolvedBuild { get; set; }

        public string ClosedBy { get; set; }

        public string LinkBug { get; set; }

        public string LastEditedBy { get; set; }

        public DateTime OpenedDate { get; set; }

        public DateTime AssignedDate { get; set; }

        public DateTime ResolvedDate { get; set; }

        public DateTime ClosedDate { get; set; }

        public DateTime LastEditedDate { get; set; }
    }
}
