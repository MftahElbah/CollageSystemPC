using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageSystemPC.Methods
{
    public class StdViewModel
    {
        public int StdId { get; set; }
        public string StdName { get; set; }
        public string StdUsername { get; set; }
        public bool IsActive { get; set; }
    }
    public class SubTableView
    {
        public int SubId { get; set; }
        public string SubName { get; set; }
        public string TeacherName { get; set; }
    }
}
