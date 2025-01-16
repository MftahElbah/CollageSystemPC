﻿using SQLite;
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

    public class SubjectPosts
    {
        [PrimaryKey, AutoIncrement]
        public int PostId { get; set; }
        public int SubId { get; set; }
        public string PostTitle { get; set; }
        public string PostDes { get; set; }
        public DateTime PostDate { get; set; }
        //public DateTime? DeadLineTime { get; set; }
        //public Byte[] PostDesFile { get; set; }
        public string PostFileLink { get; set; }

    }

    public class RequestJoinSubject
    {
        [PrimaryKey, AutoIncrement]
        public int ReqId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; }
        public int SubId { get; set; }
        public DateTime RequestDate { get; set; }
    }

    public class DegreeTable
    {
        [PrimaryKey, AutoIncrement]
        public int DegId { get; set; } //foreignkey to Subject
        public int SubId { get; set; } //foreignkey to Subject
        public string StdName { get; set; } //foreignkey to UsertsAccount UserId type 3
        public float Deg { get; set; }
        public float MiddelDeg { get; set; }
        public float Total
        {
            get { return Deg + MiddelDeg; }
        }
    }

    public class SubjectBooks
    {
        [PrimaryKey, AutoIncrement]
        public int BookId { get; set; }
        public string BookName { get; set; }
        public int SubId { get; set; }
        public string BookFile { get; set; }
        public DateTime UploadDate { get; set; }
    }

    public class SubjectAssignments
    {
        public int PostId { get; set; }
        public int StdId { get; set; }
        public string StdName { get; set; }
        public string AssignmentFile { get; set; }
        //public string FileType { get; set; }

    }

    /*public string SubTeacher { get; set; }*/
}
