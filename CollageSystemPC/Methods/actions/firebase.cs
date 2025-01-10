using CollageSystemPC.Pages;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Windows.System;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
            
            List<AdminAccountTable> data = JsonConvert.DeserializeObject<List<AdminAccountTable>>(SetData.Body);
            AdminAccountTable adminAccount = data.FirstOrDefault(x => x != null && x.Username == username);
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
        public override async Task<UsersAccountTable> CheckIfUsernameExist(string username)
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

        /*public async Task<int> DeleteAllSub()
        {
            try
            {
                // Fetch all subjects from Firebase
                FirebaseResponse getData = await client.GetAsync("sub/");
                if (getData == null || getData.Body == "null")
                {
                    return 0;
                }

                // Deserialize the subjects into a dictionary
                Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(getData.Body);

                int deletedCount = 0;

                // Iterate and delete each subject individually
                foreach (var item in data)
                {
                    DeleteSub($"User/sub/{item.Key}");
                    if (deleteResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        deletedCount++;
                    }
                }
                return deletedCount; // Return the number of deleted records
            }

            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subjects: {ex.Message}");
                return 0;
            }
        }*/
        public override async Task<int> DeleteAllSub()
        {
            try
            {
                int count = -1;
                // 1️⃣ Fetch all subjects
                var subData = await client.GetAsync("sub/");
                if (subData != null && subData.Body != "null")
                {
                    var subjects = JsonConvert.DeserializeObject<List<SubTable>>(subData.Body);
                    count = subjects.Count ;

                    // 2️⃣ Find and delete each subject linked to the user
                    foreach (var item in subjects)
                    {
                        
                            await DeleteSub(item.SubId); // Call existing DeleteSub method
                            count--;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user's subjects: {ex.Message}");
                return -1;
            }
        }
        public async Task<int> DeleteAllPostsWithSubIdGreaterThanMinusOne()
        {
            try
            {
                // Fetch all posts from Firebase
                FirebaseResponse getData = await client.GetAsync("post/");
                if (getData == null || getData.Body == "null")
                {
                    return 0;
                }

                // Deserialize the posts into a dictionary
                Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(getData.Body);

                int deletedCount = 0;

                // Iterate and delete posts where SubId > -1
                foreach (var item in data)
                {
                    if (item.Value.SubId > -1)
                    {
                        FirebaseResponse deleteResponse = await client.DeleteAsync($"post/{item.Key}");
                        if (deleteResponse.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            deletedCount++;
                        }
                    }
                }

                return deletedCount; // Return the number of deleted records
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting posts: {ex.Message}");
                return 0;
            }
        }
        public async Task<bool> DeleteAllRequests()
        {
            try
            {
                // Delete the entire "degree/" node
                FirebaseResponse deleteResponse = await client.DeleteAsync("request/");

                return deleteResponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all degrees: {ex.Message}");
                return false;
            }
            
        }
        public async Task<bool> DeleteAllDegrees()
        {
            try
            {
                // Delete the entire "degree/" node
                FirebaseResponse deleteResponse = await client.DeleteAsync("degree/");

                return deleteResponse.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all degrees: {ex.Message}");
                return false;
            }
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
            //SecureStorage.Remove("userid");
            //var SetData =
                await client.DeleteAsync("User/AdminUserSession/" + UserSession.UserId);
            return 1;
        }
        public override async Task<int> DeleteSub(int subId)
        {
            try
            {
                // Delete the subject from "sub/"
                var subDeleteResponse = await client.DeleteAsync($"sub/{subId}");
                if (subDeleteResponse == null || subDeleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return 0; // Failed to delete the subject
                }

                int deletedCount = 1; // Count the deleted subject

                // Call separate methods to delete related data
                deletedCount += await DeleteDegreesBySubId(subId);
                deletedCount += await DeletePostsBySubId(subId);
                deletedCount += await DeleteRequestsBySubId(subId);

                return deletedCount; // Return total number of deleted records
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subject and related data: {ex.Message}");
                return 0;
            }
        }

        private async Task<int> DeleteDegreesBySubId(int subId)
        {
            int deletedCount = 0;

            var degreeData = await client.GetAsync("degree/");
            if (degreeData != null && degreeData.Body != "null")
            {
                var degrees = JsonConvert.DeserializeObject<List<DegreeTable>>(degreeData.Body);
                foreach (var item in degrees)
                {
                    if (item.SubId == subId)
                    {
                        var degreeDelete = await client.DeleteAsync($"degree/{item.DegId}");
                        if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            deletedCount++;
                        }
                    }
                }
            }

            return deletedCount;
        }

        private async Task<int> DeletePostsBySubId(int subId)
        {
            int deletedCount = 0;

            var postData = await client.GetAsync("post/");
            if (postData != null && postData.Body != "null")
            {
                var posts = JsonConvert.DeserializeObject<List<SubjectPosts>>(postData.Body);
                foreach (var item in posts)
                {
                    if (item.SubId == subId)
                    {
                        var postDelete = await client.DeleteAsync($"post/{item.PostId}");
                        if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                        {
                            deletedCount++;
                        }
                    }
                }
            }

            return deletedCount;
        }

        private async Task<int> DeleteRequestsBySubId(int subId)
        {
            int deletedCount = 0;

            var requestData = await client.GetAsync("request/");
            if (requestData != null && requestData.Body != "null")
            {
                var requests = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(requestData.Body);
                foreach (var item in requests)
                {
                    if(item != null)
                    {
                        if (item.SubId == subId)
                        {
                            var requestDelete = await client.DeleteAsync($"request/{item.ReqId}");
                            if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                    }
                }
            }

            return deletedCount;
        }

        public override async Task<int> DeleteUser(int userId, int type)
        {
            try
            {
                if (type == 1) // For users with subjects (e.g., teachers)
                {
                    // 1️⃣ Delete all subjects related to the user
                    await DeleteAllUserSubs(userId);

                    // 2️⃣ Delete the user account
                    var userDeleteResponse = await client.DeleteAsync($"User/Account/{userId}");
                    if (userDeleteResponse == null || userDeleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return 0; // Failed to delete user
                    }

                    return 1; // User deleted
                }
                else if (type == 2) // For users without subjects (e.g., admins)
                {
                    var userDeleteResponse = await client.DeleteAsync($"User/Account/{userId}");
                    if (userDeleteResponse == null || userDeleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        return 0;
                    }

                    return 1; // Only user deleted
                }

                return 0; // Invalid type
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user and related data: {ex.Message}");
                return 0;
            }
        }

        private async Task DeleteAllUserSubs(int userId)
        {
            try
            {
                // 1️⃣ Fetch all subjects
                var subData = await client.GetAsync("sub/");
                if (subData != null && subData.Body != "null")
                {
                    var subjects = JsonConvert.DeserializeObject<List<SubTable>>(subData.Body);

                    // 2️⃣ Find and delete each subject linked to the user
                    foreach (var item in subjects)
                    {
                        if (item.UserId == userId) // Assuming SubTeacherId links subject to user
                        {
                            await DeleteSub(item.SubId); // Call existing DeleteSub method
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user's subjects: {ex.Message}");
            }
        }


        public override async Task<List<AdminAccountTable>> GetAdminData()
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }
            List<AdminAccountTable> data = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
            List<AdminAccountTable> result = data.Where(x=> x != null && x.AdminId != UserSession.UserId).ToList();
            //Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
            //List<AdminAccountTable> list = data.Select(x => x.Value).ToList();
            //return list;
            return result;

        }
        public override async Task<List<AdminAccountTable>> GetAdminDataByName(string name)
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }

            //Dictionary<string, AdminAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
            List<AdminAccountTable> data = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
            // Filter admins where Name contains the given name (case-insensitive)
            List<AdminAccountTable> list = data
                .Where(x => x != null &&( x.Name.Contains(name)))
                .ToList();

            return list;
        }

        public override async Task<List<SubTable>> GetSubData()
        {
            var set = await client.GetAsync("sub/");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }

            // Deserialize into a Dictionary
            //Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
            List<SubTable> data = JsonConvert.DeserializeObject<List<SubTable>>(set.Body);
            // Handle null values and convert to a List<SubTable>
            List<SubTable> result = data
                .Where(s => s != null)
                .ToList();

            return result;
        }


        public override async Task<List<SubTable>> GetSubDataByName(string name)
        {
            var set = await client.GetAsync("sub/");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }

            //Dictionary<string, SubTable> data = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
            List<SubTable> data = JsonConvert.DeserializeObject<List<SubTable>>(set.Body);
            //List<AdminAccountTable> result = data.Where(x => x != null).ToList();
            // Filter subjects where SubTeacherName contains the given name (case-insensitive)
            List<SubTable> result = data
                    .Where(s => s != null && s.SubName.Contains(name))
                    .ToList();

            return result;
        }

        public override async Task<List<UsersAccountTable>> GetUserData(int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }
            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
            //List<UsersAccountTable> data = JsonConvert.DeserializeObject<List<UsersAccountTable>>(set.Body);
            //List<AdminAccountTable> result = data.Where(x => x != null).ToList();
            List<UsersAccountTable> list = data.Where(x => x.Value.UserType == type).Select(x => x.Value).ToList(); return list;
        }
        public override async Task<List<UsersAccountTable>> GetUserDataByName(string name, int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }

            Dictionary<string, UsersAccountTable> data = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
            //List<UsersAccountTable> data = JsonConvert.DeserializeObject<List<UsersAccountTable>>(set.Body);

            // Filter users by name containing the given string (case-insensitive)
            List<UsersAccountTable> list = data.Where(x => x.Value.Name.Contains( name )&& x.Value.UserType == type).Select(x => x.Value).ToList();

            return list;
        }

        public override async Task<int> InsertAdmin(string Username, string name, string password, bool AdminType)
        {
            // Fetch the existing data
            var set = await client.GetAsync("User/AccountAdmin");
            int newId = 1;

            if (set != null && set.Body != "null")
            {
                // Deserialize the data into a dictionary
                List<AdminAccountTable> data = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
                List<AdminAccountTable> result = data.Where(x => x != null).ToList();
                // Get the highest existing ID
                if (data.Any())
                {
                    newId = result.Max(x => x.AdminId) + 1;
                }
            }

            // Create a new admin account with the auto-incremented ID
            AdminAccountTable adminAccount = new AdminAccountTable
            {
                AdminId = newId,
                Name = name,
                Username = Username,
                Password = password,
                AdminType = AdminType
            };

            // Push the new data to Firebase
            var SetData = client.Set($"User/AccountAdmin/{newId}", adminAccount);

            if (SetData == null)
            {
                return 0;
            }

            return newId; // Return the new ID
        }

        /*public override async Task<int> insertSession(UserSessionTable session)
        {
            var setData = await client.SetAsync("User/AdminUserSession/" + session.UserId, session);
            SecureStorage.SetAsync("userid/", session.UserId.ToString());

            if (setData == null)
            {
                return 0;
            }
            return 1;
        }*/
        public override async Task<int> insertSession(UserSessionTable session)
        {
            try
            {
                UserSessionTable set = new UserSessionTable()
                {
                    UserId = session.UserId,
                    Password = session.Password
                };
                var SetData = await client.SetAsync("User/AdminUserSession/" + set.UserId, set);
                await SecureStorage.SetAsync("userid", set.UserId.ToString());
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
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
            if (string.IsNullOrEmpty(password))
            {
                List<AdminAccountTable> usercheck = await GetAdminDataByName(name);
                password = usercheck.First().Password;
            }
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
            if (string.IsNullOrEmpty(password))
            {
            List<UsersAccountTable> usercheck = await GetUserDataByName(name, UserType);
            password = usercheck.First().Password;
            }

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
        public override async Task<AdminAccountTable> UserLoginChecker(string username, string password,string id)
        {
            try
            {
                // Fetch the data from Firebase
                FirebaseResponse getData = await client.GetAsync("User/AccountAdmin");

                if (getData == null || getData.Body == "null")
                {
                    return null;
                }

                // Attempt to deserialize the data as a List of AdminAccountTable
                List<AdminAccountTable> data = JsonConvert.DeserializeObject<List<AdminAccountTable>>(getData.Body);

                // Check if the data is null or empty
                if (data == null || !data.Any())
                {
                    return null;
                }
                AdminAccountTable adminAccount = null; 
                if (!string.IsNullOrEmpty(username)) { 
                    adminAccount = data.FirstOrDefault(x => x != null && (x.Username == username && x.Password == password));
                }
                if (!string.IsNullOrEmpty(id)) {
                    int ids = int.Parse(id);
                    adminAccount = data.FirstOrDefault(x => x != null && (x.AdminId == ids && x.Password == password));
                }

                // Find the admin account by matching the username and password

                if (adminAccount == null)
                {
                    return null;
                }

                return adminAccount;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        /*public override async Task<AdminAccountTable> UserSessionChecker()
        {
            try
            {
                FirebaseResponse response;
                string userid = await SecureStorage.GetAsync("userid");
                if (userid == null)
                {
                    return null;
                }
                
                response = await client.GetAsync("User/AdminUserSession/" + userid);
                
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                AdminAccountTable LUS = JsonConvert.DeserializeObject<AdminAccountTable>(response.Body.ToString());

                AdminAccountTable result = LUS;
                return result;

            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return null;
            }
        }*/
        /*public override async Task<AdminAccountTable> UserSessionChecker()
        {
            var userid = await SecureStorage.GetAsync("userid");
            if (userid == null)
            {
                return null;
            }
            var getData = await client.GetAsync("User/AdminUserSession/" + userid);

            if (getData == null || getData.Body == "null")
            {
                return null;
            }
            AdminAccountTable adminAccount = JsonConvert.DeserializeObject<AdminAccountTable>(getData.Body);
            if (adminAccount == null)
            {
                return null;
            }
            return adminAccount;
        }*/

        public override async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.Where(e => e!= null && e.SubId == subId).ToList();
                return result;
            }
            catch
            {
                return new List<SubjectPosts>();
           
            }
        }
        public override async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
                List<SubjectPosts> sb = await getSubjectPosts();
                subjectPosts.PostId = sb.Count;
                var setData = await client.SetAsync("post/" + subjectPosts.PostId, subjectPosts);
                return 1;
            }
            catch
            {
                return 0;
            }
        }
        public async Task<List<SubjectPosts>> getSubjectPosts()
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post");
                if (response == null || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> LUS = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body.ToString());
                if (LUS == null) return new List<SubjectPosts>();


                List<SubjectPosts> result = LUS.ToList();
                return result;
            }
            catch
            {
                return new List<SubjectPosts>();
            }
        }
        public override async Task<int> updateSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
                var SetData = await client.UpdateAsync("post/" + subjectPosts.PostId, subjectPosts);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

        public override async Task<SubjectPosts> getSubjectPost(int postId)
        {
            try
            {
                FirebaseResponse response = await client.GetAsync("post/" + postId);
                if (response == null || response.Body == "null")
                {
                    return null;
                }

                SubjectPosts LUS = JsonConvert.DeserializeObject<SubjectPosts>(response.Body.ToString());
                SubjectPosts result = LUS;
                return result;
            }
            catch
            {
                return null;
            }
        }
        public override async Task<int> deleteSubjectPost(int postId)
        {
            try
            {
                var SetData = await client.DeleteAsync("post/" + postId);
                return 1;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return 0;
            }
        }

    }
}
