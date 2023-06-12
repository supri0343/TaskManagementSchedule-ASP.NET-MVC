using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestAplication.Models;
using TestAplication.Util;

namespace TestAplication.Data
{
    public class DbInitializer
    {
        public static void Initializer(ScheduleContext scheduleContext)
        {
            scheduleContext.Database.EnsureCreated();

            // Look for any Schedule.
            if (scheduleContext.Users.Any())
            {
                return;   // DB has been seeded
            }
            else
            {
                var schedules = new Schedule[]
                {
                new Schedule{Detail="WASH CLOTHES",Priority=3,DueDate=DateTime.Now.AddMinutes(30),IsDone=false},
                new Schedule{Detail="WASH SHOES",Priority=2,DueDate=DateTime.Now.AddMinutes(45),IsDone=false},
                new Schedule{Detail="STUDY",Priority=4,DueDate=DateTime.Now.AddMinutes(50),IsDone=false},
                };

                foreach (var schedule in schedules)
                {
                    scheduleContext.Add(schedule);
                }
                scheduleContext.SaveChanges();

                var Users = new UserModel[]
                {
                new UserModel{UserName="USER",UserPassword=SHA1Encrypt.Hash("USER")},
                 new UserModel{UserName="ADMIN",UserPassword=SHA1Encrypt.Hash("ADMIN")}
                };
                foreach (var User in Users)
                {
                    scheduleContext.Users.Add(User);
                }
                scheduleContext.SaveChanges();

                var Roles = new RoleModel[]
                {
                new RoleModel{UserId=1,RoleName="USER"},
                new RoleModel{UserId=2,RoleName="ADMIN"},
                };
                foreach (var Role in Roles)
                {
                    scheduleContext.Roles.Add(Role);
                }

                scheduleContext.SaveChanges();
            }

           

        }
    }
}
