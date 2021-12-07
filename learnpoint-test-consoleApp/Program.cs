using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using EduDev.ClientLibrary.ApiModels;
using MvLpApi.ClientLibrary;
using MvLpApi.ClientLibrary.ApiModels;
using Newtonsoft.Json.Linq;
using learnpoint_test_consoleApp.Models;

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
                    var user = new User()
                    {  
                        FirstName = student.FirstName, 
                        LastName = student.LastName, 
                        Email = student.Email 
                    };

                    message = AddUser(user);

                    var externalId = JObject.Parse(message)["Id"].First()["Id"].Value<string>();

                    

                    

                        var newItem = new SavedItem()
                        {
                            Type = "User",
                            ExternalId = Convert.ToInt32(externalId),
                            ExternalSource = "LearnPoint",
                            LastUpdated = DateTime.Now
                        };

                        context.SavedItems.Add(newItem);
                        context.SaveChanges();
                   

                    Console.WriteLine($"Created: {message}");
                    Console.WriteLine($"Created: {message2}");
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
            try
            {

                var groupsData = FetchGroupsData(accessToken);
                var message = string.Empty;
                var message2 = string.Empty;
                foreach (var group in groupsData.Groups)
                {
                    if(group.Category.Name == "Skola")
                    {
                        var course = new Course()
                        {
                            Name = group.Name, 
                            CourseCode = group.Code, 
                            StartDate = group.LifespanFrom ?? DateTime.Now, 
                            EndDate = group.LifespanUntil ?? DateTime.Now 
                        };




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

        private static void FillStaffMembers(string accessToken)
        {
            try
            {

                var staffMembersData = FetchStaffMembersData(accessToken);
                var message = string.Empty;
                var message2 = string.Empty;
                foreach (var staffMember in staffMembersData.StaffMembers)
                {
                    var user = new User()
                    { 
                        FirstName = staffMember.FirstName,
                        LastName = staffMember.LastName, 
                        Email = staffMember.Email 
                    };

                    message = AddUser(user);



                    Console.WriteLine($"Created: {message}");

                }
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
            }
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

        private static string AddCourse(Course course)
        {
            var action = "api/course";
            var request =
                client.PostAsJsonAsync(action, course);

            var response =
                request.Result.Content.ReadAsStringAsync();

            return response.Result;
        }

    }
}
