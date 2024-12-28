using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageSystemPC
{
    public class UserSessionTable{
        [PrimaryKey]
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    public class AdminAccountTable{
        [PrimaryKey,AutoIncrement]
        public int AdminId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
        public bool AdminType { get; set; } //Moderator = True, Inserter = False
    }
    public class UsersAccountTable{
        [PrimaryKey]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; } //1 = Teacher, 2 = Student
        public bool IsActive {  get; set; }
    }
    
    public class SubTable{
        [PrimaryKey, AutoIncrement]
        public int SubId { get; set; }
        public string SubName { get; set; }
        public bool ShowDeg {  get; set; }
        public int UserId { get; set; }
        public string SubTeacherName { get; set; }

    }
    /*public string SubTeacher { get; set; }*/
}
