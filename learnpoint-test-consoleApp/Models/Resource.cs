using System;
using System.Collections.Generic;
using System.Text;

namespace learnpoint_test_consoleApp.Models
{
    public class Resource
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public ExternalId SourceId { get; set; }
        public ExternalId TargetId { get; set; }
        public DateTime LastUpdated { get; set; }

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
