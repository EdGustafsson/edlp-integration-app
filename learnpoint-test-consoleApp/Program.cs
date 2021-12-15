using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using EduDev.ClientLibrary.ApiModels;
using MvLpApi.ClientLibrary;
using MvLpApi.ClientLibrary.ApiModels;
using Newtonsoft.Json.Linq;
using learnpoint_test_consoleApp.Entities;
using learnpoint_test_consoleApp.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace learnpoint_test_consoleApp
{
    class Program
    {
        private static JObject parameters = JObject.Parse(File.ReadAllText(@"Parameters.json"));

        private static string apiBaseAddress = parameters.SelectToken("parameters.apiBaseAddress.value").Value<string>();
        private static string tokenEndpointUri = parameters.SelectToken("parameters.tokenEndpointUri.value").Value<string>();
        private static string clientId = parameters.SelectToken("parameters.clientId.value").Value<string>();
        private static string clientSecret = parameters.SelectToken("parameters.clientSecret.value").Value<string>();
        private static string requestedScopes = parameters.SelectToken("parameters.requestedScopes.value").Value<string>();
        private static string tenantIdentifier = parameters.SelectToken("parameters.tenantIdentifier.value").Value<string>();




        private static HttpClient client = new HttpClient()
        { BaseAddress = new Uri("https://edu-development-rest.azurewebsites.net") };
        private static string val = "application/json";
        private static MediaTypeWithQualityHeaderValue media = new MediaTypeWithQualityHeaderValue(val);
        static void Main(string[] args)
        {


            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(media);
            var accessToken = DataRetrieval.GetAccessToken(tokenEndpointUri, clientId, clientSecret, requestedScopes);

            var a = FetchStudentsData(accessToken);

           

            FillUsers(accessToken);


        }
        private static  StudentsData FetchStudentsData(string accessToken)
        {
            var studentsData = DataRetrieval.GetStudentsData(
                                    DataRetrieval.GetApiJSONDataString(apiBaseAddress, tenantIdentifier, DataRetrieval.Endpoint.Students, accessToken));

            return studentsData;
        }

        private static GroupsData FetchGroupsData(string accessToken)
        {
            var groupsData = DataRetrieval.GetGroupsData(
                                    DataRetrieval.GetApiJSONDataString(apiBaseAddress, tenantIdentifier, DataRetrieval.Endpoint.Groups, accessToken));

            return groupsData;
        }

        private static StaffMembersData FetchStaffMembersData(string accessToken)
        {
            var staffMembersData = DataRetrieval.GetStaffMembersData(
                                     DataRetrieval.GetApiJSONDataString(apiBaseAddress, tenantIdentifier, DataRetrieval.Endpoint.StaffMembers, accessToken));

            return staffMembersData;
        }

        private static void FillUsers(string accessToken)
        {

            using (var context = new DataContext())
            {

            try
            {
                
                var studentsData = FetchStudentsData(accessToken);
                var message = string.Empty;
                var message2 = string.Empty;

                foreach (var student in studentsData.Students)
                {

                        var findUser = context.Resources.Where(u => u.Type == "User"
                                                                        && u.SourceId.IntId == student.Id
                                                                        && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);




                        if (findUser.OfType<Resource>().Any())
                        {


                            var localUser = context.Resources.FirstOrDefault(u => u.Type == "User"
                                                                       && u.SourceId.IntId == student.Id
                                                                       && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                            var user = new User()
                            {
                                FirstName = student.FirstName,
                                LastName = student.LastName,
                                Email = student.Email
                            };

                            if (localUser != null)
                            {
                                User targetUser = GetUser(localUser.TargetId.GuidId);

                                if (targetUser != null)
                                {
                                    message = UpdateUser(targetUser.Id, user);


                                    Console.WriteLine($"Created: {message}");
                                }
                                else
                                {

                                    message = AddUser(user);


                                    var container = JToken.Parse(message);
                                    Guid sourceId;
                                    Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                                    var item = context.Resources.Single(r => r.Id == localUser.Id);

                                    item.LastUpdated = DateTime.Now;
                                    item.SourceId.IntId = student.Id;
                                    item.SourceId.IType = Resource.ExternalId.IdType.Int;
                                    item.SourceId.SType = Resource.ExternalId.SystemType.Learnpoint;

                                    item.TargetId.GuidId = sourceId;
                                    item.TargetId.IType = Resource.ExternalId.IdType.Guid;
                                    item.TargetId.SType = Resource.ExternalId.SystemType.EduApi;

       
                                    context.Entry(item).State = EntityState.Modified;
                                    context.SaveChanges();


                                    Console.WriteLine($"Created: {message}");


                                }
                            }

                        }
                        else
                        {


                            var user = new User()
                            {
                                FirstName = student.FirstName,
                                LastName = student.LastName,
                                Email = student.Email
                            };

                            message = AddUser(user);



                            var container = JToken.Parse(message);
                            Guid sourceId;
                            Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                            var newItem = new Resource()
                            {
                                Id = Guid.NewGuid(),
                                Type = "User",
                                LastUpdated = DateTime.Now,
                                SourceId = new Resource.ExternalId()
                                {
                                    IntId = student.Id,
                                    IType = Resource.ExternalId.IdType.Int,
                                    SType = Resource.ExternalId.SystemType.Learnpoint
                                },
                                TargetId = new Resource.ExternalId()
                                {
                                    GuidId = user.Id,
                                    IType = Resource.ExternalId.IdType.Guid,
                                    SType = Resource.ExternalId.SystemType.EduApi
                                },
                            };


                            context.Set<Resource>().Add(newItem);
                            context.SaveChanges();



                            Console.WriteLine($"Created: {message}");


                        }
                    }
                    Console.ReadLine();
                }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }

            }
        }


        private static void FillGroups(string accessToken)
        {

            using (var context = new DataContext())
            {

                try
                {

                    var groupsData = FetchGroupsData(accessToken);
                    var message = string.Empty;
                    var message2 = string.Empty;

                    foreach (var group in groupsData.Groups)
                    {

                        if (group.Category.Code == "CourseInstance")
                        {

                            var findCourses = context.Resources.Where(u => u.Type == "Course"
                                                                        && u.SourceId.IntId == group.Id
                                                                        && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                            if (findCourses.OfType<Resource>().Any())
                            {

                                var localGroup = context.Resources.FirstOrDefault(u => u.Type == "Course"
                                                                       && u.SourceId.IntId == group.Id
                                                                       && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);


                                var course = new Course()
                                {
                                    Name = group.Name,
                                    CourseCode = group.Code,
                                    StartDate = group.LifespanFrom ?? DateTime.Now,
                                    EndDate = group.LifespanUntil ?? DateTime.Now
                                };

                                

                                if (localGroup != null)
                                {
                                    Course targetCourse = GetCourse(localGroup.TargetId.GuidId);

                                    if (targetCourse != null)
                                    {
                                        message = UpdateCourse(targetCourse.Id, course);

                                        Console.WriteLine($"Created: {message}");
                                    }
                                    else
                                    {
                                        message = AddCourse(course);


                                        var container = JToken.Parse(message);
                                        Guid sourceId;
                                        Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                                        var newItem = new Resource()
                                        {
                                            Id = localGroup.Id,
                                            Type = "Course",
                                            LastUpdated = DateTime.Now,
                                            SourceId = new Resource.ExternalId()
                                            {
                                                IntId = group.Id,
                                                IType = Resource.ExternalId.IdType.Int,
                                                SType = Resource.ExternalId.SystemType.Learnpoint
                                            },
                                            TargetId = new Resource.ExternalId()
                                            {
                                                GuidId = sourceId,
                                                IType = Resource.ExternalId.IdType.Guid,
                                                SType = Resource.ExternalId.SystemType.EduApi
                                            },
                                        };


                                        context.Set<Resource>().Add(newItem);
                                        context.SaveChanges();


                                        Console.WriteLine($"Created: {message}");
                                    }
                                    


                                    
                                }
                                else
                                {

                                    message = AddCourse(course);


                                    var container = JToken.Parse(message);
                                    Guid sourceId;
                                    Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                                    var item = context.Resources.Single(r => r.Id == localGroup.Id);

                                    item.LastUpdated = DateTime.Now;
                                    item.SourceId.IntId = group.Id;
                                    item.SourceId.IType = Resource.ExternalId.IdType.Int;
                                    item.SourceId.SType = Resource.ExternalId.SystemType.Learnpoint;

                                    item.TargetId.GuidId = sourceId;
                                    item.TargetId.IType = Resource.ExternalId.IdType.Guid;
                                    item.TargetId.SType = Resource.ExternalId.SystemType.EduApi;


                                    context.Entry(item).State = EntityState.Modified;
                                    context.SaveChanges();




                                    Console.WriteLine($"Created: {message}");


                                }

                            }
                            else
                            {


                                var course = new Course()
                                {
                                    Name = group.Name,
                                    CourseCode = group.Code,
                                    StartDate = group.LifespanFrom ?? DateTime.Now,
                                    EndDate = group.LifespanUntil ?? DateTime.Now
                                };

                                message = AddCourse(course);


                                var container = JToken.Parse(message);
                                Guid sourceId;
                                Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                                var newItem = new Resource()
                                {
                                    Type = "Course",
                                    LastUpdated = DateTime.Now,
                                    SourceId = new Resource.ExternalId()
                                    {
                                        IntId = group.Id,
                                        IType = Resource.ExternalId.IdType.Int,
                                        SType = Resource.ExternalId.SystemType.Learnpoint
                                    },
                                    TargetId = new Resource.ExternalId()
                                    {
                                        GuidId = sourceId,
                                        IType = Resource.ExternalId.IdType.Guid,
                                        SType = Resource.ExternalId.SystemType.EduApi
                                    },
                                };


                                context.Set<Resource>().Add(newItem);
                                context.SaveChanges();


                                Console.WriteLine($"Created: {message}");



                            }
                        }
                    }
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }

            }

            
        }

        private static void FillStaffMembers(string accessToken)
        {
            using (var context = new DataContext())
            {

                try
                {

                    var staffMembersData = FetchStaffMembersData(accessToken);
                    var message = string.Empty;
                    var message2 = string.Empty;

                    foreach (var staffMember in staffMembersData.StaffMembers)
                    {


                        var localUser = context.Resources.FirstOrDefault(u => u.Type == "User"
                                                                        && u.SourceId.IntId == staffMember.Id
                                                                        && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                        if (localUser == null)
                        {

                            var user = new User()
                            {
                                FirstName = staffMember.FirstName,
                                LastName = staffMember.LastName,
                                Email = staffMember.Email
                            };

                            message = AddUser(user);


                            var container = JToken.Parse(message);
                            Guid sourceId;
                            Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                            var newItem = new Resource()
                            {
                                Type = "User",
                                LastUpdated = DateTime.Now,
                                SourceId = new Resource.ExternalId()
                                {
                                    IntId = staffMember.Id,
                                    IType = Resource.ExternalId.IdType.Int,
                                    SType = Resource.ExternalId.SystemType.Learnpoint
                                },
                                TargetId = new Resource.ExternalId()
                                {
                                    GuidId = sourceId,
                                    IType = Resource.ExternalId.IdType.Guid,
                                    SType = Resource.ExternalId.SystemType.EduApi
                                },
                            };


                            context.Set<Resource>().Add(newItem);
                            context.SaveChanges();


                            Console.WriteLine($"Created: {message}");

                        }
                        else
                        {


                            var user = new User()
                            {
                                FirstName = staffMember.FirstName,
                                LastName = staffMember.LastName,
                                Email = staffMember.Email
                            };

                            User targetUser = GetUser(localUser.TargetId.GuidId);

                            if (targetUser != null)
                            {
                                message = UpdateUser(targetUser.Id, user);


                                Console.WriteLine($"Created: {message}");
                            }
                            else
                            {

                                message = AddUser(user);


                                var container = JToken.Parse(message);
                                Guid sourceId;
                                Guid.TryParse(container["guid"]?.ToString(), out sourceId);

                                var item = context.Resources.Single(r => r.Id == localUser.Id);

                                item.LastUpdated = DateTime.Now;
                                item.SourceId.IntId = staffMember.Id;
                                item.SourceId.IType = Resource.ExternalId.IdType.Int;
                                item.SourceId.SType = Resource.ExternalId.SystemType.Learnpoint;

                                item.TargetId.GuidId = sourceId;
                                item.TargetId.IType = Resource.ExternalId.IdType.Guid;
                                item.TargetId.SType = Resource.ExternalId.SystemType.EduApi;


                                context.Entry(item).State = EntityState.Modified;
                                context.SaveChanges();


                                Console.WriteLine($"Created: {message}");


                            }
                        }
                    }
                    Console.ReadLine();
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }

            }

           
        }

        private static void FillCourseMembership(string accessToken)
        {
            using (var context = new DataContext())
            {
                try
                {
                    var studentsData = FetchStudentsData(accessToken);
                    var groupsData = FetchGroupsData(accessToken);


                    foreach (var student in studentsData.Students)
                    {

                        var localUser = context.Resources.FirstOrDefault(u => u.Type == "User"
                                                                            && u.SourceId.IntId == student.Id 
                                                                            && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);


                        User targetUser = GetUser(localUser.TargetId.GuidId);

                        foreach(var group in student.Groups)
                        {
                            

                            var localCourse = context.Resources.FirstOrDefault(u => u.Type == "Course"
                                                                                    && u.SourceId.IntId == group.Group.Id
                                                                                    && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                            Course targetCourse = GetCourse(localCourse.TargetId.GuidId);

                            CourseMembership newCourseMembership = new CourseMembership()
                            {
                                UserId = targetUser.Id,
                                CourseId = targetCourse.Id,
                                EnrolledDate = targetCourse.StartDate

                            };
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error: {e.Message}");
                }
            }
        }

        private static User GetUser(Guid id)
        {
            var action = $"api/user/findnumber/{id}";
            var request =
                client.GetAsync(action);

            var response =
                request.Result.Content.
                ReadAsAsync<User>();

            return response.Result;
        }

        private static string AddUser(User user)
        {
            var action = "api/user";
            var request =
                client.PostAsJsonAsync(action, user);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }
        private static string UpdateUser(Guid id, User user)
        {
            var action = $"api/user/{id}";
            var request =
                client.PutAsJsonAsync(action, user);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }

        private static Course GetCourse(Guid id)
        {
            var action = $"api/course/findnumber/{id}";
            var request =
                client.GetAsync(action);

            var response =
                request.Result.Content.
                ReadAsAsync<Course>();

            return response.Result;
        }

        private static string AddCourse(Course course)
        {
            var action = "api/course";
            var request =
                client.PostAsJsonAsync(action, course);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }

        private static string UpdateCourse(Guid id, Course course)
        {
            var action = $"api/course/{id}";
            var request =
                client.PutAsJsonAsync(action, course);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }
    }
}
