using System;

namespace EduDev.ClientLibrary.ApiModels
{
    public class CourseMembership
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid CourseId { get; set; }
        public DateTime EnrolledDate { get; set; }
    }
}
