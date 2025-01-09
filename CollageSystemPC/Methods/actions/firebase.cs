using FireSharp.Config;
using FireSharp.Interfaces;
using Newtonsoft.Json;


namespace CollageSystemPC.Methods.actions
{
    internal class Firebase : DataBase
    {
        public IFirebaseConfig fc = new FirebaseConfig
        {
            AuthSecret = "7gviqqKuDYSHOM6kjHznZjS5u1VTgjAx3D1uq7X9",
            BasePath = "https://ctsapp-9de50-default-rtdb.europe-west1.firebasedatabase.app/"
        };
        public IFirebaseClient client;
        private void connection()
        {
            try
            {
                client = new FireSharp.FirebaseClient(fc);
            }
            catch (Exception)
            {
                Console.WriteLine("some ");
            }
        }
        public Firebase() {
            connection();
        }
        public override  async Task<AdminAccountTable> CheckIfAdminUsernameExist(string username)
        {
            var SetData = await client.GetAsync("User/AccountAdmin");
            if (SetData == null || SetData.Body == "null")
            {
                return null;
            }
            Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(SetData.Body);
            AdminAccountTable adminAccount = data.FirstOrDefault(x => x.Value.Username == username).Value;
            if (adminAccount == null)
            {
                return null;
            }
            return adminAccount;
        }
        public override async Task<UsersAccountTable> CheckIfIdExist(int UserId)
        {
            var SetData = await client.GetAsync("User/Account/" + UserId);
            if (SetData == null || SetData.Body == "null")
            {
                return null;
            }
             UsersAccountTable account = JsonConvert.DeserializeObject<UsersAccountTable>(SetData.Body);
            if (account == null)
            {
                return null;
            }
            return account;
        }
        public override async  Task<UsersAccountTable> CheckIfUsernameExist(string username)
        {
            var SetData = await client.GetAsync("User/Account");
            if (SetData == null || SetData.Body == "null")
            {
                return null;
            }
            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(SetData.Body);
            UsersAccountTable adminAccount = data.FirstOrDefault(x => x.Value.Username == username).Value;
            if (adminAccount == null)
            {
                return null;
            }
            return adminAccount;
        }
        public override async Task<int> DeActiveAllSTD()
        {
            var SetData = await client.GetAsync("User/Account");
            if (SetData == null || SetData.Body == "null")
            {
                return 0;
            }
            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(SetData.Body);
            data = data.Where(x => x.Value.UserType == 2).ToDictionary(x => x.Key, x => x.Value);
            foreach (var item in data)
            {
                item.Value.IsActive = false;
                var SetData2 = await client.SetAsync("User/Account/" + item.Value.UserId, item.Value);
            }
            return 1;
        }
        public override async Task<int> DeleteAdmin(int Id)
        {
            var set = await client.DeleteAsync("User/AccountAdmin/" + Id);
            if (set == null)
            {
                return 0;
            }
            return 1;
        }
        public override async Task<int> deleteSession()
        {
            SecureStorage.Remove("userid");
            var SetData = await client.DeleteAsync("User/AdminUserSession/" + UserSession.UserId);
            return 1;
        }
        public override async Task<int> DeleteSub(int Id)
        {
            var set = await client.DeleteAsync("User/sub/" + Id);

            if (set == null)
            {
                return 0;
            }
            return 1;

        }
        public override async Task<int> DeleteUser(int Id)
        {
            var set = await client.DeleteAsync("User/Account/" + Id);
            if (set == null)
            {
                return 0;
            }
            return 1;
        }

        public override async Task<List<AdminAccountTable>> GetAdminData()
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }
            Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
            List<AdminAccountTable> list = data.Select(x => x.Value).ToList();
            return list;

        }
        public override async Task<List<AdminAccountTable>> GetAdminDataByName(string name)
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }
            Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
            List<AdminAccountTable> list = data.Where(x => x.Value.Name == name).Select(x => x.Value).ToList();
            return list;
        }
        public override async Task<List<SubTable>> GetSubData()
        {
            var set = await client.GetAsync("User/sub");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }
            Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
            List<SubTable> list = data.Select(x => x.Value).ToList();
            return list;
        }
        public override async Task<List<SubTable>> GetSubDataByName(string name)
        {
            var set = await client.GetAsync("User/sub");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }
            Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
            List<SubTable> list = data.Where(x => x.Value.SubTeacherName == name).Select(x => x.Value).ToList();
            return list;
        }
        public override async Task<List<UsersAccountTable>> GetUserData(int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }
            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
            List<UsersAccountTable> list = data.Where(x => x.Value.UserType == type).Select(x => x.Value).ToList();
            return list;
        }
        public override async Task<List<UsersAccountTable>> GetUserDataByName(string name, int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }
            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
            List<UsersAccountTable> list = data.Where(x => x.Value.Name == name && x.Value.UserType == type).Select(x => x.Value).ToList();
            return list;
        }
        public override async Task<int> InsertAdmin(string Username, string name, string password, bool AdminType)
        {
            AdminAccountTable adminAccount = new AdminAccountTable
            {
                Name = name,
                Username = Username,
                Password = password,
                AdminType = AdminType
            };
            var SetData = client.Push("User/AccountAdmin", adminAccount);
            if (SetData == null)
            {
                return 0;
            }
            return 1;
        }
        public override async Task<int> insertSession(UserSessionTable session)
        {
            var setData = await client.SetAsync("User/AdminUserSession/" + session.UserId, session);
            SecureStorage.SetAsync("userid/", session.UserId.ToString());

            if (setData == null)
            {
                return 0;
            }
            return 1;
        }
        public override async Task<int> InsertUser(int Id, string Username, string name, string password, int UserType)
        {
            UsersAccountTable usersAccount = new UsersAccountTable
            {
                UserId = Id,
                Name = name,
                Username = Username,
                Password = password,
                UserType = UserType,
                IsActive = true
            };
            var setData = await client.SetAsync("User/Account/" + Id, usersAccount);
            if (setData == null)
            {
                return 0;
            }
            return 1;

        }
        public override async Task<int> UpdateAdmin(int Id, string Username, string name, string password, bool AdminType)
        {
            AdminAccountTable admin= new AdminAccountTable
            {
                AdminId = Id,
                Name = name,
                Username = Username,
                Password = password,
                AdminType = AdminType
            };
            var updateData = await client.UpdateAsync("User/AccountAdmin/" + Id, admin);
            if (updateData == null)
            {
                return 0;
            }
            return 1;
        }
     
        public override async Task<int> UpdateUser(int Id, string Username, string name, string password, int UserType, bool isActive)
        {
            UsersAccountTable user = new UsersAccountTable
            {
                UserId = Id,
                Name = name,
                Username = Username,
                Password = password,
                UserType = UserType,
                IsActive = isActive
            };
            var updateData = await client.UpdateAsync("User/Account/" + Id, user);
            if (updateData == null)
            {
                return 0;
            }
            return 1;
        }
        public override async Task<AdminAccountTable> UserLoginChecker(string username, string password)
        {
            var getData = await client.GetAsync("User/AccountAdmin");
            if (getData == null || getData.Body == "null")
            {
                return null;
            }
            Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(getData.Body);
            AdminAccountTable adminAccount = data.FirstOrDefault(x => x.Value.Username == username && x.Value.Password == password).Value;
            if (adminAccount == null)
            {
                return null;
            }
            return adminAccount;
        }
        public override async Task<AdminAccountTable> UserSessionChecker()
        {
            var userid = await SecureStorage.GetAsync("userid");
            if( userid == null)
            {
                return null;
            }
            var getData = await client.GetAsync("User/AdminUserSession/" + userid);
            
            if(getData == null || getData.Body == "null")
            {
                return null;
            }
            AdminAccountTable adminAccount = JsonConvert.DeserializeObject<AdminAccountTable>(getData.Body);
            if (adminAccount == null)
            {
                return null;
            }
            return adminAccount;
        }
     
    }
}
