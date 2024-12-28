using SQLite;

namespace CollageSystemPC.Methods.actions
{
    internal class MineSQLite
    {
        public string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "YourDatabaseName.db");

        private readonly SQLiteAsyncConnection _database;
 

        public MineSQLite()
        {
            _database = new SQLiteAsyncConnection(dbPath);

        }
        public async Task<int> deleteSession()
        {
            var session = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            if (session != null)
            {
                return await _database.DeleteAsync(session);
            }
            return 0;
        }
        public async Task<AdminAccountTable> UserSessionChecker()
        {
            // Check if a user session exists
            var IfUserExist = await _database.Table<UserSessionTable>().FirstOrDefaultAsync();
            if (IfUserExist == null)
                return null;

            // Find and return the matching admin user
            var user = await _database.Table<AdminAccountTable>()
                                       .FirstOrDefaultAsync(u => u.AdminId == IfUserExist.UserId && u.Password == IfUserExist.Password);
            return user;
        }

        public async Task<AdminAccountTable > UserLoginChecker(string username, string password)
        {
            var IfUserExist = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.Username == username && d.Password == password);
            return IfUserExist;
        }

        public async Task<int> insertSession(UserSessionTable session)
        {
            int rows = await _database.InsertAsync(session);
            return rows;
        }

        public async Task<List<UsersAccountTable>> GetUserData(int type)
        {
            var STD = await _database.Table<UsersAccountTable>().Where(b => b.UserType == type).ToListAsync();
            return STD;
        }
        public async Task<List<AdminAccountTable>> GetAdminData()
        {
            var admins = await _database.Table<AdminAccountTable>().ToListAsync();
            return admins;
        }
        public async Task<List<SubTable>> GetSubData()
        {
            var sub = await _database.Table<SubTable>().ToListAsync();
            return sub;
        }

        public async Task<List<UsersAccountTable>> GetUserDataByName(string SearchName , int type)
        {
            var filteredUsers = await _database.Table<UsersAccountTable>()
            .Where(s => s.Name.Contains(SearchName) && s.UserType == type)
            .ToListAsync();
            return filteredUsers;
        }
        public async Task<List<AdminAccountTable>> GetAdminDataByName(string SearchName)
        {
            var filteredAdmins = await _database.Table<AdminAccountTable>()
            .Where(s => s.Name.Contains(SearchName))
            .ToListAsync();
            return filteredAdmins;
        }
        public async Task<List<SubTable>> GetSubDataByName(string SearchName)
        {
            var filteredSub = await _database.Table<SubTable>()
            .Where(s => s.SubName.Contains(SearchName))
            .ToListAsync();
            return filteredSub;
        }

        public async Task <UsersAccountTable> CheckIfIdExist(int UserId)
        {
            var existingUser = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == UserId);
            return existingUser;
        }
        public async Task <UsersAccountTable> CheckIfUsernameExist(string Stdusername)
        {
            var existingUser = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.Username == Stdusername);
            return existingUser;
        }
        public async Task <AdminAccountTable> CheckIfAdminUsernameExist(string Adminusername)
        {
            var existingUser = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.Username == Adminusername);
            return existingUser;
        }

        public async Task <int> InsertUser(int Id,string Username , string name , string password , int UserType)
        {
            var newUser = new UsersAccountTable
            {
                UserId = Id,
                Name = name,
                Username = Username,
                Password = password,
                UserType = UserType,
                IsActive = true,
            };
            int rows = await _database.InsertAsync(newUser);
            return rows;
        } 
        public async Task <int> InsertAdmin(string Username , string name , string password , bool AdminType){
            var newUser = new AdminAccountTable{
                Username = Username,
                Name = name,
                Password = password,
                AdminType = AdminType,
            };
            int rows = await _database.InsertAsync(newUser);
            return rows;
        } 
        
        public async Task <int> UpdateUser(int Id,string Username , string name , string password , int UserType , bool isActive)
        {
            var Updateduser = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == Id);

            // Update UserType based on radio button selection
            Updateduser.Name = name;
            Updateduser.Username = Username;
            Updateduser.IsActive = isActive;

                // Handle password updates if provided
            if (!string.IsNullOrEmpty(password)){
               Updateduser.Password = password;
            }

                // Update the database with the new user information
                int rows = await _database.UpdateAsync(Updateduser);
                return rows;
        }
        public async Task <int> UpdateAdmin(int Id,string Username , string name , string password , bool AdminType)
        {
            var UpdatedAdmin = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.AdminId == Id);

            // Update UserType based on radio button selection
            UpdatedAdmin.Name = name;
            UpdatedAdmin.Username = Username;
            UpdatedAdmin.AdminType = AdminType;

                // Handle password updates if provided
            if (!string.IsNullOrEmpty(password)){
                UpdatedAdmin.Password = password;
            }

                // Update the database with the new user information
                int rows = await _database.UpdateAsync(UpdatedAdmin);
                return rows;
        }
        
        public async Task <int> DeleteUser(int Id)
        {
            var user = await _database.Table<UsersAccountTable>().FirstOrDefaultAsync(d => d.UserId == Id);
            int rows = await _database.DeleteAsync(user);
            return rows;
        }
        public async Task <int> DeleteSub(int Id)
        {
            var sub = await _database.Table<SubTable>().FirstOrDefaultAsync(d => d.SubId == Id);
            int rows = await _database.DeleteAsync(sub);
            return rows;
        }
        
        public async Task <int> DeleteAdmin(int Id)
        {
            var admin = await _database.Table<AdminAccountTable>().FirstOrDefaultAsync(d => d.AdminId == Id);
            int rows = await _database.DeleteAsync(admin);
            return rows;
        }

        
        public async Task <int> DeActiveAllSTD()
        {
            var stdda = await _database.Table<UsersAccountTable>().Where(s => s.UserType == 2).ToListAsync();
            foreach (var std in stdda)
            {
                std.IsActive = false;
            }
            int rows = await _database.UpdateAllAsync(stdda);
            return rows;
        }
        

    }
}
