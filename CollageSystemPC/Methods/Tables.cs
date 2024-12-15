using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TP
{
    public class UserSessionTable
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string Password { get; set; }
    }

    public class AdminAccountTable
    {
        public int AdminId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class UsersAccountTable
    {
        [PrimaryKey]
        public int UserId { get; set; }
        public string Name { get; set; }
        [Unique]
        public string Username { get; set; }
        public string Password { get; set; }
        public int UserType { get; set; } //1 = Teacher, 2 = Student
        public bool IsActive {  get; set; }

    }
    public class DepTable
    {
        [PrimaryKey, AutoIncrement]
        public int DepId { get; set; }
        public string DepName { get; set; }
    }

    public class BranchTable
    {
        [PrimaryKey, AutoIncrement]
        public int BranchId { get; set; }
        public string BranchName { get; set; }
        public string DepName { get; set; }
    }


    public class SubTable
    {
        [PrimaryKey, AutoIncrement]
        public int SubId { get; set; }
        public string SubName { get; set; }
        public int SubDep { get; set; } //foreign key to DepTable
        public int SubBranch { get; set; }//foreign key to BranchTable
        public int SubClass { get; set; }
        public bool ShowDeg {  get; set; }
        public int UserId { get; set; } //Foreign key to user
        public string SubTeacher { get; set; }
    }
}
