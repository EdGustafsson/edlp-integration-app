using System;


namespace EduDev.ClientLibrary.ApiModels
{
    public class CourseSource
    {
        public Guid Id { get; private set; }
        public Guid CourseId { get; set; }
        public int ExternalId { get; set; }
        public string ExternalSource { get; set; }
    }
}
