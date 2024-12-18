﻿using System;
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
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }
        public string TeacherUsername { get; set; }
        public bool IsActive { get; set; }
    }
    public class SubViewModel{
        public int SubId { get; set; }       // From SubTable
        public string SubName { get; set; }  // From SubTable
        public string SubTeacher { get; set; } // From UsersAccountTable (UserId == SubTable.UserId)
    }

}
