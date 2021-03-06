using EduDev.ClientLibrary.ApiModels;
using learnpoint_test_consoleApp.Data;
using learnpoint_test_consoleApp.Entities;
using Microsoft.EntityFrameworkCore;
using MvLpApi.ClientLibrary;
using MvLpApi.ClientLibrary.ApiModels;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace edlp-integration-app
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
        private static StudentsData FetchStudentsData(string accessToken)
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

                    foreach (var student in studentsData.Students)
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

                        if (localUser == null)
                        {
                            message = AddUser(user);

                            Guid targetId = ParseGuid(message);

                            CreateResource(context, "User", targetId, student.Id);

                            Console.WriteLine($"Created: {message}");
                        }
                        else
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

                                Guid targetId = ParseGuid(message);

                                UpdateResource(context, student.Id, targetId, localUser.Id);

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



        private static void FillGroups(string accessToken)
        {

            using (var context = new DataContext())
            {

                try
                {

                    var groupsData = FetchGroupsData(accessToken);
                    var message = string.Empty;

                    foreach (var group in groupsData.Groups)
                    {

                        if (group.Category.Code == "CourseInstance")
                        {

                            var localCourse = context.Resources.FirstOrDefault(u => u.Type == "Course"
                                                                   && u.SourceId.IntId == group.Id
                                                                   && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                            var course = new Course()
                            {
                                Name = group.Name,
                                CourseCode = group.Code,
                                StartDate = group.LifespanFrom ?? DateTime.Now,
                                EndDate = group.LifespanUntil ?? DateTime.Now,

                            };

                            if (localCourse == null)
                            {
                                message = AddCourse(course);

                                Guid targetId = ParseGuid(message);

                                CreateResource(context, "Course", targetId, group.Id);

                                Console.WriteLine($"Created: {message}");
                            }
                            else
                            {

                                User targetCourse = GetUser(localCourse.TargetId.GuidId);

                                if (targetCourse != null)
                                {


                                    message = UpdateCourse(targetCourse.Id, course);

                                    Console.WriteLine($"Created: {message}");

                                }
                                else
                                {
                                    message = AddCourse(course);

                                    Guid targetId = ParseGuid(message);

                                    UpdateResource(context, group.Id, targetId, localCourse.Id);

                                    Console.WriteLine($"Created: {message}");

                                }
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

                    foreach (var staffMember in staffMembersData.StaffMembers)
                    {


                        var localUser = context.Resources.FirstOrDefault(u => u.Type == "User"
                                                                        && u.SourceId.IntId == staffMember.Id
                                                                        && u.SourceId.SType == Resource.ExternalId.SystemType.Learnpoint);

                        var user = new User()
                        {
                            FirstName = staffMember.FirstName,
                            LastName = staffMember.LastName,
                            Email = staffMember.Email
                        };

                        if (localUser == null)
                        {

                            message = AddUser(user);

                            Guid targetId = ParseGuid(message);

                            CreateResource(context, "User", targetId, staffMember.Id);

                            Console.WriteLine($"Created: {message}");

                        }
                        else
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

                                Guid targetId = ParseGuid(message);

                                UpdateResource(context, staffMember.Id, targetId, localUser.Id);

                                Console.WriteLine($"Created: {message}");

                            }
                        }
                    }
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

                        foreach (var group in student.Groups)
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

                            context.Entry(newCourseMembership).State = EntityState.Added;
                            context.SaveChanges();

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

        private static User GetCourseMembership(Guid id)
        {
            var action = $"api/coursemembersihp/findnumber/{id}";
            var request =
                client.GetAsync(action);

            var response =
                request.Result.Content.
                ReadAsAsync<User>();

            return response.Result;
        }

        private static string AddCourseMembership(CourseMembership courseMembership)
        {
            var action = "api/coursemembersihp";
            var request =
                client.PostAsJsonAsync(action, courseMembership);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }

        private static void CreateResource(DataContext context, string type, Guid targetId, int sourceId)
        {

            var newItem = new Resource()
            {
                Id = Guid.NewGuid(),
                Type = type,
                LastUpdated = DateTime.Now,
                SourceId = new Resource.ExternalId()
                {
                    IntId = sourceId,
                    IType = Resource.ExternalId.IdType.Int,
                    SType = Resource.ExternalId.SystemType.Learnpoint
                },
                TargetId = new Resource.ExternalId()
                {
                    GuidId = targetId,
                    IType = Resource.ExternalId.IdType.Guid,
                    SType = Resource.ExternalId.SystemType.EduApi
                },
            };

            context.Set<Resource>().Add(newItem);
            context.SaveChanges();


        }

        private static void UpdateResource(DataContext context, int sourceId, Guid targetId, Guid resourceId)
        {
            var item = context.Resources.Single(r => r.Id == resourceId);

            item.LastUpdated = DateTime.Now;
            item.SourceId.IntId = sourceId;
            item.SourceId.IType = Resource.ExternalId.IdType.Int;
            item.SourceId.SType = Resource.ExternalId.SystemType.Learnpoint;

            item.TargetId.GuidId = targetId;
            item.TargetId.IType = Resource.ExternalId.IdType.Guid;
            item.TargetId.SType = Resource.ExternalId.SystemType.EduApi;


            context.Entry(item).State = EntityState.Modified;
            context.SaveChanges();
        }

        private static Guid ParseGuid(string message)
        {
            var container = JToken.Parse(message);
            Guid sourceId;
            Guid.TryParse(container["guid"]?.ToString(), out sourceId);
            return sourceId;
        }


    }
}
