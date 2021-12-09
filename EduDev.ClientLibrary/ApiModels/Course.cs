using System;

namespace EduDev.ClientLibrary.ApiModels
{
    public class Course
    {
        public Guid Id { get; set; }
        public string CourseCode { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get;  set; }
    }
}
