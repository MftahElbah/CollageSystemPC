using CollageSystemPC.Pages;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using Newtonsoft.Json;
using Syncfusion.Maui.Core;
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
        public override async Task<AdminAccountTable> CheckIfAdminUsernameExist(string username)
        {
            var SetData = await client.GetAsync("User/AccountAdmin");
            if (SetData == null || SetData.Body == "null")
            {
                return null;
            }

            // Declare the result variable
            AdminAccountTable adminAccount = null;

            // Check if the response body starts with a "{" indicating a dictionary format
            if (SetData.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary (key-value format)
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(SetData.Body);
                if (dataDict != null)
                {
                    // Find the admin account by username in the dictionary
                    adminAccount = dataDict.Values.FirstOrDefault(x => x != null && x.Username == username);
                }
            }
            // Check if the response body starts with a "[" indicating a list format
            else if (SetData.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List (array format)
                var dataList = JsonConvert.DeserializeObject<List<AdminAccountTable>>(SetData.Body);
                if (dataList != null)
                {
                    // Find the admin account by username in the list
                    adminAccount = dataList.FirstOrDefault(x => x != null && x.Username == username);
                }
            }

            // If no admin account is found, return null
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

            UsersAccountTable userAccount = null;

            // Check if the response body starts with a "{" indicating a dictionary format
            if (SetData.Body.Trim().StartsWith("{"))
            {
                // Deserialize as Dictionary<string, UsersAccountTable>
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(SetData.Body);
                userAccount = dataDict?.FirstOrDefault(x => x.Value != null && x.Value.Username == username).Value;
            }
            // Check if the response body starts with a "[" indicating a list format
            else if (SetData.Body.Trim().StartsWith("["))
            {
                // Deserialize as List<UsersAccountTable>
                var dataList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(SetData.Body);
                userAccount = dataList?.FirstOrDefault(x => x != null && x.Username == username);
            }

            // Return the user account if found, otherwise return null
            return userAccount;
        }

        public override async Task<int> DeActiveAllSTD()
        {
            var SetData = await client.GetAsync("User/Account");
            if (SetData == null || SetData.Body == "null")
            {
                return 0;
            }

            // Declare the data collection variable
            Dictionary<string, UsersAccountTable> dataDict = null;
            List<UsersAccountTable> dataList = null;

            // Check if the response body starts with a "{" indicating a dictionary format
            if (SetData.Body.Trim().StartsWith("{"))
            {
                dataDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(SetData.Body);
            }
            // Check if the response body starts with a "[" indicating a list format
            else if (SetData.Body.Trim().StartsWith("["))
            {
                dataList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(SetData.Body);
            }

            // If data is a dictionary, filter and deactivate users
            if (dataDict != null)
            {
                // Filter users with UserType 2 and deactivate them
                var filteredData = dataDict.Where(x => x.Value != null && x.Value.UserType == 2).ToDictionary(x => x.Key, x => x.Value);
                foreach (var item in filteredData)
                {
                    item.Value.IsActive = false;
                    await client.SetAsync("User/Account/" + item.Value.UserId, item.Value);
                }
            }
            // If data is a list, filter and deactivate users
            else if (dataList != null)
            {
                var filteredData = dataList.Where(x => x != null && x.UserType == 2).ToList();
                foreach (var item in filteredData)
                {
                    item.IsActive = false;
                    await client.SetAsync("User/Account/" + item.UserId, item);
                }
            }

            return 1;
        }



        public override async Task<int> DeleteAllSub()
        {
            try
            {
                int count = -1;
                // 1️⃣ Fetch all subjects
                var subData = await client.GetAsync("sub/");
                if (subData != null && subData.Body != "null")
                {
                    // Declare the variables for handling both formats
                    List<SubTable> subjectsList = null;
                    Dictionary<string, SubTable> subjectsDict = null;

                    // Check if the response body starts with "{" (indicating dictionary format)
                    if (subData.Body.Trim().StartsWith("{"))
                    {
                        subjectsDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(subData.Body);
                    }
                    // Check if the response body starts with "[" (indicating list format)
                    else if (subData.Body.Trim().StartsWith("["))
                    {
                        subjectsList = JsonConvert.DeserializeObject<List<SubTable>>(subData.Body);
                    }

                    // If subjects are in Dictionary format
                    if (subjectsDict != null)
                    {
                        count = subjectsDict.Count;
                        foreach (var item in subjectsDict)
                        {
                            if (item.Value != null)
                            {
                                await DeleteSub(item.Value.SubId); // Call existing DeleteSub method
                                count--;
                            }
                        }
                    }
                    // If subjects are in List format
                    else if (subjectsList != null)
                    {
                        count = subjectsList.Count;
                        foreach (var item in subjectsList)
                        {
                            if (item != null)
                            {
                                await DeleteSub(item.SubId); // Call existing DeleteSub method
                                count--;
                            }
                        }
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

                // Declare the variables for handling both formats
                Dictionary<string, SubTable> dataDict = null;
                List<SubTable> dataList = null;

                // Check if the response body starts with "{" (indicating dictionary format)
                if (getData.Body.Trim().StartsWith("{"))
                {
                    dataDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(getData.Body);
                }
                // Check if the response body starts with "[" (indicating list format)
                else if (getData.Body.Trim().StartsWith("["))
                {
                    dataList = JsonConvert.DeserializeObject<List<SubTable>>(getData.Body);
                }

                int deletedCount = 0;

                // If the data is in Dictionary format
                if (dataDict != null)
                {
                    foreach (var item in dataDict)
                    {
                        if (item.Value != null && item.Value.SubId > -1)
                        {
                            FirebaseResponse deleteResponse = await client.DeleteAsync($"post/{item.Key}");
                            if (deleteResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
                        }
                    }
                }
                // If the data is in List format
                else if (dataList != null)
                {
                    foreach (var item in dataList)
                    {
                        if (item != null && item.SubId > -1)
                        {
                            // Delete based on the index of the item in the list (you can adapt as needed for your structure)
                            string itemKey = dataList.IndexOf(item).ToString();
                            FirebaseResponse deleteResponse = await client.DeleteAsync($"post/{itemKey}");
                            if (deleteResponse.StatusCode == System.Net.HttpStatusCode.OK)
                            {
                                deletedCount++;
                            }
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
                deletedCount += await DeleteBookssBySubId(subId);

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

            try
            {
                var degreeData = await client.GetAsync("degree/");
                if (degreeData != null && degreeData.Body != "null")
                {
                    // Declare variables to handle both formats
                    Dictionary<string, DegreeTable> degreeDict = null;
                    List<DegreeTable> degreeList = null;

                    // Check if the response body starts with "{" (indicating dictionary format)
                    if (degreeData.Body.Trim().StartsWith("{"))
                    {
                        degreeDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(degreeData.Body);
                    }
                    // Check if the response body starts with "[" (indicating list format)
                    else if (degreeData.Body.Trim().StartsWith("["))
                    {
                        degreeList = JsonConvert.DeserializeObject<List<DegreeTable>>(degreeData.Body);
                    }

                    // If data is in Dictionary format
                    if (degreeDict != null)
                    {
                        foreach (var item in degreeDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var degreeDelete = await client.DeleteAsync($"degree/{item.Key}");
                                if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                    // If data is in List format
                    else if (degreeList != null)
                    {
                        foreach (var item in degreeList)
                        {
                            if (item != null && item.SubId == subId)
                            {
                                var degreeDelete = await client.DeleteAsync($"degree/{item.DegId}");
                                if (degreeDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting degrees: {ex.Message}");
            }

            return deletedCount;
        }
        private async Task<int> DeletePostsBySubId(int subId)
        {
            int deletedCount = 0;

            try
            {
                var postData = await client.GetAsync("post/");
                if (postData != null && postData.Body != "null")
                {
                    // Handle both List and Dictionary formats
                    List<SubjectPosts> postsList = null;
                    Dictionary<string, SubjectPosts> postsDict = null;

                    if (postData.Body.Trim().StartsWith("{"))
                    {
                        postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(postData.Body);
                    }
                    else if (postData.Body.Trim().StartsWith("["))
                    {
                        postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(postData.Body);
                    }

                    // Process List format
                    if (postsList != null)
                    {
                        foreach (var item in postsList)
                        {
                            if (item != null && item.SubId == subId)
                            {
                                var deletedAssignmentCount = await DeleteAssignmentByPostId(item.PostId);
                                var postDelete = await client.DeleteAsync($"post/{item.PostId}");
                                if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }

                    // Process Dictionary format
                    else if (postsDict != null)
                    {
                        foreach (var item in postsDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var deletedAssignmentCount = await DeleteAssignmentByPostId(item.Value.PostId);
                                var postDelete = await client.DeleteAsync($"post/{item.Key}");
                                if (postDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting posts: {ex.Message}");
            }

            return deletedCount;
        }

        private async Task<int> DeleteAssignmentByPostId(int postId)
        {
            int deletedCount = 0;

            try
            {
                // Fetch assignments from Firebase
                var assignmentData = await client.GetAsync("assignment/");
                if (assignmentData != null && assignmentData.Body != "null")
                {
                    // Handle both List and Dictionary formats
                    Dictionary<string, SubjectAssignments> assignmentsDict = null;
                    List<SubjectAssignments> assignmentsList = null;

                    if (assignmentData.Body.Trim().StartsWith("{"))
                    {
                        assignmentsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectAssignments>>(assignmentData.Body);
                    }
                    else if (assignmentData.Body.Trim().StartsWith("["))
                    {
                        assignmentsList = JsonConvert.DeserializeObject<List<SubjectAssignments>>(assignmentData.Body);
                    }

                    // Process List format
                    if (assignmentsList != null)
                    {
                        foreach (var item in assignmentsList)
                        {
                            if (item != null && item.PostId == postId)
                            {
                                int key = int.Parse($"{item.StdId}{item.PostId}");
                                var requestDelete = await client.DeleteAsync($"assignment/{key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }

                    // Process Dictionary format
                    else if (assignmentsDict != null)
                    {
                        foreach (var item in assignmentsDict)
                        {
                            if (item.Value != null && item.Value.PostId == postId)
                            {
                                var requestDelete = await client.DeleteAsync($"assignment/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting assignments: {ex.Message}");
            }

            return deletedCount;
        }

        private async Task<int> DeleteRequestsBySubId(int subId)
        {
            int deletedCount = 0;

            try
            {
                var requestData = await client.GetAsync("request/");
                if (requestData != null && requestData.Body != "null")
                {
                    // Handle both List and Dictionary formats
                    List<RequestJoinSubject> requestsList = null;
                    Dictionary<string, RequestJoinSubject> requestsDict = null;

                    if (requestData.Body.Trim().StartsWith("{"))
                    {
                        requestsDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(requestData.Body);
                    }
                    else if (requestData.Body.Trim().StartsWith("["))
                    {
                        requestsList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(requestData.Body);
                    }

                    // Process List format
                    if (requestsList != null)
                    {
                        foreach (var item in requestsList)
                        {
                            if (item != null && item.SubId == subId)
                            {
                                var requestDelete = await client.DeleteAsync($"request/{item.ReqId}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }

                    // Process Dictionary format
                    else if (requestsDict != null)
                    {
                        foreach (var item in requestsDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var requestDelete = await client.DeleteAsync($"request/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting requests: {ex.Message}");
            }

            return deletedCount;
        }

        private async Task<int> DeleteBookssBySubId(int subId)
        {
            int deletedCount = 0;

            try
            {
                var requestData = await client.GetAsync("book/");
                if (requestData != null && requestData.Body != "null")
                {
                    // Handle both List and Dictionary formats
                    List<SubjectBooks> booksList = null;
                    Dictionary<string, SubjectBooks> booksDict = null;

                    if (requestData.Body.Trim().StartsWith("{"))
                    {
                        booksDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectBooks>>(requestData.Body);
                    }
                    else if (requestData.Body.Trim().StartsWith("["))
                    {
                        booksList = JsonConvert.DeserializeObject<List<SubjectBooks>>(requestData.Body);
                    }

                    // Process List format
                    if (booksList != null)
                    {
                        foreach (var item in booksList)
                        {
                            if (item != null && item.SubId == subId)
                            {
                                var requestDelete = await client.DeleteAsync($"book/{item.BookId}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }

                    // Process Dictionary format
                    else if (booksDict != null)
                    {
                        foreach (var item in booksDict)
                        {
                            if (item.Value != null && item.Value.SubId == subId)
                            {
                                var requestDelete = await client.DeleteAsync($"book/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting books: {ex.Message}");
            }

            return deletedCount;
        }

        public override async Task<int> DeleteUser(int userId, int type , string name)
        {
            try
            {
                var userDeleteResponse = await client.DeleteAsync($"User/Account/{userId}");
                if (userDeleteResponse == null || userDeleteResponse.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return 0; // Failed to delete user
                }
                switch (type){
                    case 1:
                    await DeleteAllUserSubs(userId);
                    await DeleteTasks(userId);

                    break;

                    case 2:
                    await DeleteAllStdSubs(userId , name);
                    break;
                }
                    // 2️⃣ Delete the user account

                return 1; // Only user deleted
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user and related data: {ex.Message}");
                return 0;
            }
        }

        private async Task DeleteAllStdSubs(int userId, string name)
        {
            try
            {
                // Fetch the request data
                var reqdata = await client.GetAsync("request/");
                if (reqdata != null && reqdata.Body != "null")
                {
                    // Handle both List and Dictionary formats for requests
                    List<RequestJoinSubject> requestsList = null;
                    Dictionary<string, RequestJoinSubject> requestsDict = null;

                    if (reqdata.Body.Trim().StartsWith("{"))
                    {
                        requestsDict = JsonConvert.DeserializeObject<Dictionary<string, RequestJoinSubject>>(reqdata.Body);
                    }
                    else if (reqdata.Body.Trim().StartsWith("["))
                    {
                        requestsList = JsonConvert.DeserializeObject<List<RequestJoinSubject>>(reqdata.Body);
                    }

                    // Process List format
                    if (requestsList != null)
                    {
                        foreach (var item in requestsList)
                        {
                            if (item != null && item.UserId == userId)
                            {
                                await client.DeleteAsync($"request/{item.ReqId}");
                            }
                        }
                    }
                    // Process Dictionary format
                    else if (requestsDict != null)
                    {
                        foreach (var item in requestsDict)
                        {
                            if (item.Value != null && item.Value.UserId == userId)
                            {
                                await client.DeleteAsync($"request/{item.Key}");
                            }
                        }
                    }
                }

                // Fetch degree data
                var degdata = await client.GetAsync("degree/");
                if (degdata != null && degdata.Body != "null")
                {
                    // Handle both List and Dictionary formats for degrees
                    List<DegreeTable> degreesList = null;
                    Dictionary<string, DegreeTable> degreesDict = null;

                    if (degdata.Body.Trim().StartsWith("{"))
                    {
                        degreesDict = JsonConvert.DeserializeObject<Dictionary<string, DegreeTable>>(degdata.Body);
                    }
                    else if (degdata.Body.Trim().StartsWith("["))
                    {
                        degreesList = JsonConvert.DeserializeObject<List<DegreeTable>>(degdata.Body);
                    }

                    // Process List format
                    if (degreesList != null)
                    {
                        foreach (var item in degreesList)
                        {
                            if (item != null && item.StdName == name)
                            {
                                await client.DeleteAsync($"degree/{item.DegId}");
                            }
                        }
                    }
                    // Process Dictionary format
                    else if (degreesDict != null)
                    {
                        foreach (var item in degreesDict)
                        {
                            if (item.Value != null && item.Value.StdName == name)
                            {
                                await client.DeleteAsync($"degree/{item.Key}");
                            }
                        }
                    }
                }

                // Fetch assignment data
                var assigndata = await client.GetAsync("assignment/");
                if (assigndata != null && assigndata.Body != "null")
                {
                    // Check if the response is in dictionary format (starts with '{')
                    if (assigndata.Body.Trim().StartsWith("{"))
                    {
                        // Deserialize as a Dictionary (key-value format)
                        var assignmentsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectAssignments>>(assigndata.Body);

                        // Ensure the dictionary is not null
                        if (assignmentsDict != null)
                        {
                            // Iterate through the dictionary items
                            foreach (var item in assignmentsDict)
                            {
                                // Ensure the value is not null and matches the user ID
                                if (item.Value != null && item.Value.StdId == userId)
                                {
                                    // Delete the assignment using the key
                                    var keyToDelete = item.Key;
                                    await client.DeleteAsync($"assignment/{keyToDelete}");
                                }
                            }
                        }
                    }
                    // Check if the response is in list format (starts with '[')
                    else if (assigndata.Body.Trim().StartsWith("["))
                    {
                        // Deserialize as a List (array format)
                        var assignmentsList = JsonConvert.DeserializeObject<List<SubjectAssignments>>(assigndata.Body);

                        // Ensure the list is not null
                        if (assignmentsList != null)
                        {
                            // Iterate through the list
                            foreach (var item in assignmentsList)
                            {
                                // Ensure the item is not null and matches the user ID
                                if (item != null && item.StdId == userId)
                                {
                                    // Create a key using StdId and PostId
                                    int keyToDelete = int.Parse($"{item.StdId}{item.PostId}"); // or combine numerically if needed

                                    // Delete the assignment using the generated key
                                    await client.DeleteAsync($"assignment/{keyToDelete}");
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting student's subjects: {ex.Message}");
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
                    // Check if the response body starts with '{' (indicating a Dictionary format)
                    if (subData.Body.Trim().StartsWith("{"))
                    {
                        // Deserialize as a Dictionary (key-value format)
                        var subjectsDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(subData.Body);

                        // Ensure the dictionary is not null
                        if (subjectsDict != null)
                        {
                            // Iterate through the dictionary items
                            foreach (var item in subjectsDict)
                            {
                                if (item.Value != null && item.Value.UserId == userId)
                                {
                                    await DeleteSub(item.Value.SubId); // Call existing DeleteSub method
                                }
                            }
                        }
                    }
                    // Check if the response body starts with '[' (indicating a List format)
                    else if (subData.Body.Trim().StartsWith("["))
                    {
                        // Deserialize as a List (array format)
                        var subjectsList = JsonConvert.DeserializeObject<List<SubTable>>(subData.Body);

                        // Ensure the list is not null
                        if (subjectsList != null)
                        {
                            // Iterate through the list
                            foreach (var item in subjectsList)
                            {
                                if (item != null && item.UserId == userId)
                                {
                                    await DeleteSub(item.SubId); // Call existing DeleteSub method
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting user's subjects: {ex.Message}");
            }
        }

        public async Task<int> DeleteTasks(int id)
        {
            try
            {
                int deletedCount = 0; // Count the deleted subject

                var tasksdata = await client.GetAsync($"tasks/");
                if (tasksdata != null && tasksdata.Body != "null")
                {
                    // Handle both List and Dictionary formats
                    Dictionary<string, SchedulerTask> tasksDict = null;
                    List<SchedulerTask> tasksList = null;

                    if (tasksdata.Body.Trim().StartsWith("{"))
                    {
                        tasksDict = JsonConvert.DeserializeObject<Dictionary<string, SchedulerTask>>(tasksdata.Body);
                    }
                    else if (tasksdata.Body.Trim().StartsWith("["))
                    {
                        tasksList = JsonConvert.DeserializeObject<List<SchedulerTask>>(tasksdata.Body);
                    }

                    // Process List format
                    if (tasksList != null)
                    {
                        foreach (var item in tasksList)
                        {
                            if (item != null && item.UserId == id)
                            {
                                var requestDelete = await client.DeleteAsync($"tasks/{item.TaskId}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }

                    // Process Dictionary format
                    else if (tasksDict != null)
                    {
                        foreach (var item in tasksDict)
                        {
                            if (item.Value != null && item.Value.UserId == id)
                            {
                                var requestDelete = await client.DeleteAsync($"tasks/{item.Key}");
                                if (requestDelete.StatusCode == System.Net.HttpStatusCode.OK)
                                {
                                    deletedCount++;
                                }
                            }
                        }
                    }
                }
                return deletedCount; // Return total number of deleted records
            }
            
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting subject and related data: {ex.Message}");
                return 0;
            }
        }

        


        public override async Task<List<AdminAccountTable>> GetAdminData()
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }

            List<AdminAccountTable> result = new List<AdminAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.AdminId != UserSession.UserId).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.AdminId != UserSession.UserId).ToList();
                }
            }

            return result;
        }

        public override async Task<List<AdminAccountTable>> GetAdminDataByName(string name)
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }

            List<AdminAccountTable> result = new List<AdminAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.Name.Contains(name)).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.Name.Contains(name)).ToList();
                }
            }

            return result;
        }
        public async Task<List<AdminAccountTable>> GetAdminDataById(int id)
        {
            var set = await client.GetAsync("User/AccountAdmin");
            if (set == null || set.Body == "null")
            {
                return new List<AdminAccountTable>();
            }

            List<AdminAccountTable> result = new List<AdminAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.AdminId == id).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.AdminId == id).ToList();
                }
            }

            return result;
        }

        public override async Task<List<SubTable>> GetSubData()
        {
            var set = await client.GetAsync("sub/");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }

            List<SubTable> result = new List<SubTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<SubTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null).ToList();
                }
            }

            return result;
        }

        public override async Task<List<SubTable>> GetSubDataByName(string name)
        {
            var set = await client.GetAsync("sub/");
            if (set == null || set.Body == "null")
            {
                return new List<SubTable>();
            }

            List<SubTable> result = new List<SubTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, SubTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.SubName.Contains(name)).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<SubTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.SubName.Contains(name)).ToList();
                }
            }

            return result;
        }

        public override async Task<List<UsersAccountTable>> GetUserData(int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }

            List<UsersAccountTable> result = new List<UsersAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.UserType == type).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.UserType == type).ToList();
                }
            }

            return result;
        }

        public override async Task<List<UsersAccountTable>> GetUserDataByName(string name, int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }

            List<UsersAccountTable> result = new List<UsersAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.Name.Contains(name) && x.UserType == type).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.Name.Contains(name) && x.UserType == type).ToList();
                }
            }

            return result;
        }
        public async Task<List<UsersAccountTable>> GetUserDataById(int Id, int type)
        {
            var set = await client.GetAsync("User/Account");
            if (set == null || set.Body == "null")
            {
                return new List<UsersAccountTable>();
            }

            List<UsersAccountTable> result = new List<UsersAccountTable>();

            if (set.Body.Trim().StartsWith("{"))
            {
                // Deserialize as a Dictionary
                var dataDict = JsonConvert.DeserializeObject<Dictionary<string, UsersAccountTable>>(set.Body);
                if (dataDict != null)
                {
                    result = dataDict.Values.Where(x => x != null && x.UserId == Id && x.UserType == type).ToList();
                }
            }
            else if (set.Body.Trim().StartsWith("["))
            {
                // Deserialize as a List
                var dataList = JsonConvert.DeserializeObject<List<UsersAccountTable>>(set.Body);
                if (dataList != null)
                {
                    result = dataList.Where(x => x != null && x.UserId == Id && x.UserType == type).ToList();
                }
            }

            return result;
        }


        public override async Task<int> InsertAdmin(string Username, string name, string password, bool AdminType)
        {
            // Fetch the existing data
            var set = await client.GetAsync("User/AccountAdmin");
            int newId = 1;

            if (set != null && set.Body != "null")
            {
                // Check if the response is a dictionary or a list
                if (set.Body.Trim().StartsWith("{"))
                {
                    // Deserialize as a Dictionary
                    var dataDict = JsonConvert.DeserializeObject<Dictionary<string, AdminAccountTable>>(set.Body);
                    if (dataDict != null && dataDict.Any())
                    {
                        // Get the highest existing ID from the dictionary and increment it
                        newId = dataDict.Values.Where(x => x != null).Max(x => x.AdminId) + 1;
                    }
                }
                else if (set.Body.Trim().StartsWith("["))
                {
                    // Deserialize as a List
                    List<AdminAccountTable> dataList = JsonConvert.DeserializeObject<List<AdminAccountTable>>(set.Body);
                    if (dataList != null && dataList.Any())
                    {
                        // Get the highest existing ID from the list and increment it
                        newId = dataList.Where(x => x != null).Max(x => x.AdminId) + 1;
                    }
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
            var SetData = await client.SetAsync($"User/AccountAdmin/{newId}", adminAccount);

            if (SetData == null)
            {
                return 0;
            }

            return newId; // Return the new ID
        }

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
                List<AdminAccountTable> usercheck = await GetAdminDataById(Id);
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
            List<UsersAccountTable> usercheck = await GetUserDataById(Id, UserType);
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

        public override async Task<List<SubjectPosts>> getSubjectPostsBySubId(int subId)
        {
            try
            {
                // Fetch the data from Firebase
                FirebaseResponse response = await client.GetAsync("post");

                // Check if the response is null or contains "null" as body
                if (response == null || string.IsNullOrEmpty(response.Body) || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> result = new List<SubjectPosts>();

                // Check if the response is a List or a Dictionary
                if (response.Body.TrimStart().StartsWith("["))
                {
                    // Deserialize as List
                    var postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body);
                    if (postsList != null)
                    {
                        // Filter posts by SubId
                        result = postsList.Where(e => e != null && e.SubId == subId).ToList();
                    }
                }
                else if (response.Body.TrimStart().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body);
                    if (postsDict != null)
                    {
                        // Filter dictionary values by SubId
                        result = postsDict.Values.Where(e => e != null && e.SubId == subId).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception (optional)
                Console.WriteLine($"Error fetching posts by SubId: {ex.Message}");

                // Return an empty list in case of an error
                return new List<SubjectPosts>();
            }
        }


        public override async Task<int> insertSubjectPost(SubjectPosts subjectPosts)
        {
            try
            {
                int id;
                List<SubjectPosts> sb = await getSubjectPosts();
                if (sb.Count == 0)
                    id = 1;
                else
                {
                    id = sb.Max(s => s?.PostId ?? 0) + 1;
                }
                subjectPosts.PostId = id;
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
                // Fetch the data from Firebase
                FirebaseResponse response = await client.GetAsync("post");

                // Check if the response is null or contains "null" as body
                if (response == null || string.IsNullOrEmpty(response.Body) || response.Body == "null")
                {
                    return new List<SubjectPosts>();
                }

                List<SubjectPosts> result = new List<SubjectPosts>();

                // Check if the response is a List or a Dictionary
                if (response.Body.TrimStart().StartsWith("["))
                {
                    // Deserialize as List
                    var postsList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body);
                    if (postsList != null)
                    {
                        result = postsList.Where(x => x != null).ToList();
                    }
                }
                else if (response.Body.TrimStart().StartsWith("{"))
                {
                    // Deserialize as Dictionary
                    var postsDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body);
                    if (postsDict != null)
                    {
                        result = postsDict.Values.Where(x => x != null).ToList();
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error fetching posts: {ex.Message}");
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
                // Fetch all posts from Firebase
                FirebaseResponse response = await client.GetAsync("post/");
                if (response == null || string.IsNullOrEmpty(response.Body) || response.Body == "null")
                {
                    return null;
                }

                // Check whether the response is a list or a dictionary
                if (response.Body.TrimStart().StartsWith("["))
                {
                    // If it's a list
                    var dataList = JsonConvert.DeserializeObject<List<SubjectPosts>>(response.Body);
                    if (dataList == null || !dataList.Any())
                    {
                        return null;
                    }

                    // Find the post with the matching postId
                    var post = dataList.FirstOrDefault(x => x != null && x.PostId == postId);
                    return post;
                }
                else if (response.Body.TrimStart().StartsWith("{"))
                {
                    // If it's a dictionary
                    var dataDict = JsonConvert.DeserializeObject<Dictionary<string, SubjectPosts>>(response.Body);
                    if (dataDict == null || !dataDict.Any())
                    {
                        return null;
                    }

                    // Find the post with the matching postId
                    var post = dataDict.Values.FirstOrDefault(x => x != null && x.PostId == postId);
                    return post;
                }

                // If neither list nor dictionary, return null
                return null;
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                Console.WriteLine($"Error fetching post with ID {postId}: {ex.Message}");
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
