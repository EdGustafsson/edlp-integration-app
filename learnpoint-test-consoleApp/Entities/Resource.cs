using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace learnpoint_test_consoleApp.Entities
{
    [Table("Resource")]
    public class Resource
    {
        [Required]
        public Guid Id { get; set; }
        public string Type { get; set; }
        public ExternalId SourceId { get; set; }
        public ExternalId TargetId { get; set; }
        public DateTime LastUpdated { get; set; }

        [Owned]
        public class ExternalId
        {
            public int IntId { get; set; }
            public Guid GuidId { get; set; }
            public IdType IType { get; set; }
            public SystemType SType { get; set; }

            public enum SystemType
            {
                Learnpoint,
                EduApi
            }
            public enum IdType
            {
                Int,
                Guid
            }
        }
    }
}
