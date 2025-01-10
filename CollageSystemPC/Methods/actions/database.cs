using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageSystemPC.Methods.actions
{
    internal class DataBase
    {
        public static DataBase selectedDatabase;
        public virtual async Task<int> deleteSession()
        {
            return new int();
        }
        public virtual async Task InitializeDatabaseAsync()
        {
            await Task.CompletedTask;
        }
        public virtual async Task<UserSessionTable> UserSessionChecker()
        {
            return new UserSessionTable();
        }
        public virtual async Task<AdminAccountTable> UserLoginChecker(string username, string password , string id)
        {
            return new AdminAccountTable();
        }
        public virtual async Task<int> insertSession(UserSessionTable session)
        {
            return new int();
        }
        public virtual async Task<List<UsersAccountTable>> GetUserData(int type)
        {
            return new List<UsersAccountTable>();
        }
        public virtual async Task<List<AdminAccountTable>> GetAdminData()
        {
            return new List<AdminAccountTable>();
        }
        public virtual async Task<List<SubTable>> GetSubData()
        {
            return new List<SubTable>();
        }
        public virtual async Task<int> InsertUser(int Id, string Username, string name, string password, int UserType)
        {
            return new int();
        }
        public virtual async Task<int> UpdateUser(int Id, string Username, string name, string password, int UserType, bool isActive)
        {
            return new int();
        }

        public virtual async Task<List<UsersAccountTable>> GetUserDataByName(string name, int type)
        {
            return new List<UsersAccountTable>();
        }
        public virtual async Task<List<AdminAccountTable>> GetAdminDataByName(string name)
        {
            return new List<AdminAccountTable>();
        }
        public virtual async Task<List<SubTable>> GetSubDataByName(string name)
        {
            return new List<SubTable>();
        }
        public virtual async Task<UsersAccountTable> CheckIfIdExist(int UserId)
        {
            return new UsersAccountTable();
        }
        public virtual async Task<UsersAccountTable> CheckIfUsernameExist(string username)
        {
            return new UsersAccountTable();
        }
        public virtual async Task<AdminAccountTable> CheckIfAdminUsernameExist(string username)
        {
            return new AdminAccountTable();
        }

        public virtual async Task<int> InsertAdmin(string Username, string name, string password,bool AdminType)
        {
            return new int();
        }

        public virtual async Task<int> UpdateAdmin(int Id, string Username, string name, string password, bool AdminType)
        {
            return new int();
        }
        public virtual async Task<int> DeleteUser(int Id , int type)
        {
            return new int();
        }
        public virtual async Task<int> DeleteAdmin(int Id)
        {
            return new int();
        }
        public virtual async Task<int> DeleteSub(int Id)
        {
            return new int();
        }
        public virtual async Task<int> DeActiveAllSTD()
        {
            return new int();
        }

        public virtual async Task<int> DeleteAllSub()
        {
            return new int();
        }

        public virtual async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            return new List<SubjectPosts>();
        }

        public virtual async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            return new int();
        }
        public virtual async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            return new int();
        }
        public virtual async Task<SubjectPosts> getSubjectPost(int postId)
        {
            return new SubjectPosts();
        }
        public virtual async Task<int> deleteSubjectPost(int postId)
        {
            return new int();
        }
    }
}
