using System;
using System.Collections.Generic;
using System.Text;

namespace learnpoint_test_consoleApp.Models
{
    class Resource
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public ExternalId SourceId { get; set; }
        public ExternalId TargetId { get; set; }

        public class ExternalId
        {
            public int IntId { get; set; }
            public Guid GuidId { get; set; }
            public IdType Type { get; set; }
            public string ExternalSystem { get; set; }

            public enum IdType
            {
                Int,
                Guid
            }
        }
    }
}
