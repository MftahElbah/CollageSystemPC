using CollageSystemPC.Methods;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CollageSystemPC.Methods
{
    public class DatabaseHelper
    {
        public readonly SQLiteAsyncConnection _database;
        // Defines the path to the SQLite database in the local application data directory.

        public DatabaseHelper(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            // Initializes the SQLiteAsyncConnection with the provided database path.
        }

        public async Task InitializeDatabaseAsync()
        {
            await _database.CreateTableAsync<UserSessionTable>();
            await _database.CreateTableAsync<UsersAccountTable>();
            await _database.CreateTableAsync<AdminAccountTable>();
            await _database.CreateTableAsync<SubTable>();

            await SeedDatabase(); // Calls the method to seed the database with initial data if needed.

        }
        private async Task SeedDatabase()
        {
            var teacher = await _database.Table<UsersAccountTable>().ToListAsync();
            if (teacher.Count == 0)
            {
                var initialTeacher = new List<UsersAccountTable>
                {
                        new UsersAccountTable {UserId=111111121, Name="\u0637\u0627\u0644\u0628 1", Username="s1", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111122, Name="\u0637\u0627\u0644\u0628 2", Username="s2", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111123, Name="\u0637\u0627\u0644\u0628 3", Username="s3", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111124, Name="\u0637\u0627\u0644\u0628 4", Username="s4", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111125, Name="\u0637\u0627\u0644\u0628 5", Username="s5", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111126, Name="\u0637\u0627\u0644\u0628 6", Username="s6", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111127, Name="\u0637\u0627\u0644\u0628 7", Username="s7", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111128, Name="\u0637\u0627\u0644\u0628 8", Username="s8", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111129, Name="\u0637\u0627\u0644\u0628 9", Username="s9", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111130, Name="\u0637\u0627\u0644\u0628 10", Username="s10", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111131, Name="\u0637\u0627\u0644\u0628 11", Username="s11", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111132, Name="\u0637\u0627\u0644\u0628 12", Username="s12", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111133, Name="\u0637\u0627\u0644\u0628 13", Username="s13", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111134, Name="\u0637\u0627\u0644\u0628 14", Username="s14", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111135, Name="\u0637\u0627\u0644\u0628 15", Username="s15", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111136, Name="\u0637\u0627\u0644\u0628 16", Username="s16", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111137, Name="\u0637\u0627\u0644\u0628 17", Username="s17", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111138, Name="\u0637\u0627\u0644\u0628 18", Username="s18", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111139, Name="\u0637\u0627\u0644\u0628 19", Username="s19", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111140, Name="\u0637\u0627\u0644\u0628 20", Username="s20", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111141, Name="\u0637\u0627\u0644\u0628 21", Username="s21", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111142, Name="\u0637\u0627\u0644\u0628 22", Username="s22", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111143, Name="\u0637\u0627\u0644\u0628 23", Username="s23", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111144, Name="\u0637\u0627\u0644\u0628 24", Username="s24", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111145, Name="\u0637\u0627\u0644\u0628 25", Username="s25", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111146, Name="\u0637\u0627\u0644\u0628 26", Username="s26", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111147, Name="\u0637\u0627\u0644\u0628 27", Username="s27", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111148, Name="\u0637\u0627\u0644\u0628 28", Username="s28", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111149, Name="\u0637\u0627\u0644\u0628 29", Username="s29", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111150, Name="\u0637\u0627\u0644\u0628 30", Username="s30", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111151, Name="\u0637\u0627\u0644\u0628 31", Username="s31", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111152, Name="\u0637\u0627\u0644\u0628 32", Username="s32", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111153, Name="\u0637\u0627\u0644\u0628 33", Username="s33", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111154, Name="\u0637\u0627\u0644\u0628 34", Username="s34", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111155, Name="\u0637\u0627\u0644\u0628 35", Username="s35", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111156, Name="\u0637\u0627\u0644\u0628 36", Username="s36", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111157, Name="\u0637\u0627\u0644\u0628 37", Username="s37", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111158, Name="\u0637\u0627\u0644\u0628 38", Username="s38", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111159, Name="\u0637\u0627\u0644\u0628 39", Username="s39", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111160, Name="\u0637\u0627\u0644\u0628 40", Username="s40", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111161, Name="\u0637\u0627\u0644\u0628 41", Username="s41", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111162, Name="\u0637\u0627\u0644\u0628 42", Username="s42", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111163, Name="\u0637\u0627\u0644\u0628 43", Username="s43", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111164, Name="\u0637\u0627\u0644\u0628 44", Username="s44", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111165, Name="\u0637\u0627\u0644\u0628 45", Username="s45", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111166, Name="\u0637\u0627\u0644\u0628 46", Username="s46", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111167, Name="\u0637\u0627\u0644\u0628 47", Username="s48", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111168, Name="\u0637\u0627\u0644\u0628 47", Username="s49", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111169, Name="\u0637\u0627\u0644\u0628 47", Username="s50", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111170, Name="\u0637\u0627\u0644\u0628 47", Username="s51", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111171, Name="طالب 48", Username="s52", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111172, Name="طالب 49", Username="s53", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111173, Name="طالب 50", Username="s54", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111174, Name="طالب 51", Username="s55", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111175, Name="طالب 52", Username="s56", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111176, Name="طالب 53", Username="s57", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111177, Name="طالب 54", Username="s58", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111178, Name="طالب 55", Username="s59", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111179, Name="طالب 56", Username="s60", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111180, Name="طالب 57", Username="s61", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111181, Name="طالب 58", Username="s62", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111182, Name="طالب 59", Username="s63", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111183, Name="طالب 60", Username="s64", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111184, Name="طالب 61", Username="s65", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111185, Name="طالب 62", Username="s66", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111186, Name="طالب 63", Username="s67", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111187, Name="طالب 64", Username="s68", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111188, Name="طالب 65", Username="s69", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111189, Name="طالب 66", Username="s70", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111190, Name="طالب 67", Username="s71", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111191, Name="طالب 68", Username="s72", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111192, Name="طالب 69", Username="s73", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111193, Name="طالب 70", Username="s74", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111194, Name="طالب 71", Username="s75", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111195, Name="طالب 72", Username="s76", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111196, Name="طالب 73", Username="s77", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111197, Name="طالب 74", Username="s78", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111198, Name="طالب 75", Username="s79", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111199, Name="طالب 76", Username="s80", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111200, Name="طالب 77", Username="s81", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111201, Name="طالب 78", Username="s82", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111202, Name="طالب 79", Username="s83", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111203, Name="طالب 80", Username="s84", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111204, Name="طالب 81", Username="s85", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111205, Name="طالب 82", Username="s86", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111206, Name="طالب 83", Username="s87", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111207, Name="طالب 84", Username="s88", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111208, Name="طالب 85", Username="s89", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111209, Name="طالب 86", Username="s90", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111210, Name="طالب 87", Username="s91", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111211, Name="طالب 88", Username="s92", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111212, Name="طالب 89", Username="s93", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111213, Name="طالب 90", Username="s94", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111214, Name="طالب 91", Username="s95", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111215, Name="طالب 92", Username="s96", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111216, Name="طالب 93", Username="s97", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111217, Name="طالب 94", Username="s98", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111218, Name="طالب 95", Username="s99", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111219, Name="طالب 96", Username="s100", Password="1", UserType=2, IsActive=true},
                        new UsersAccountTable {UserId=111111220, Name="طالب 97", Username="s101", Password="1", UserType=2, IsActive=true},

                        new UsersAccountTable {UserId=111111000, Name="استاذ 1", Username="t1", Password="1", UserType=1, IsActive=true},
                        new UsersAccountTable {UserId=111111001, Name="استاذ 2", Username="t2", Password="1", UserType=1, IsActive=true},
                        new UsersAccountTable {UserId=111111002, Name="استاذ 3", Username="t3", Password="1", UserType=1, IsActive=true},
                        new UsersAccountTable {UserId=111111003, Name="استاذ 4", Username="t4", Password="1", UserType=1, IsActive=true},
                        new UsersAccountTable {UserId=111111004, Name="استاذ 5", Username="t5", Password="1", UserType=1, IsActive=true},
                        new UsersAccountTable {UserId=111111005, Name="استاذ 6", Username="t6", Password="1", UserType=1, IsActive=true},
                };
                await _database.InsertAllAsync(initialTeacher); // Inserts the initial Teacher Account into the database.
            }
            var sub = await _database.Table<SubTable>().ToListAsync();
            if (sub.Count == 0)
            {
                var initialSub = new List<SubTable>
                {
                    new SubTable {ShowDeg=true,SubId=1,SubName="KKK",UserId=111111002 , SubTeacherName = "kkk"},
                    new SubTable {ShowDeg=true,SubId=2,SubName="LLL",UserId=111111000 , SubTeacherName = "kkk"},
                    new SubTable {ShowDeg=true,SubId=3,SubName="MMM",UserId=111111004 , SubTeacherName = "kkk"},
                };
                await _database.InsertAllAsync(initialSub); // Inserts the initial Teacher Account into the database.
            }
            var admin = await _database.Table<AdminAccountTable>().ToListAsync();
            if (sub.Count == 0)
            {
                var initialSub = new List<AdminAccountTable>
                {
                    new AdminAccountTable {AdminId=000 , AdminType = true , Name = "a" , Username = "a" , Password = "a"},
                };
                await _database.InsertAllAsync(initialSub); // Inserts the initial Teacher Account into the database.
            }
        }
    }
}
