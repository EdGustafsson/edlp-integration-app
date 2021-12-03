using System;

namespace EduDev.ClientLibrary.ApiModels
{
    public class UserSource
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; set; }
        public int ExternalId { get; set; }
        public string ExternalSource { get; set; }
    }
}
