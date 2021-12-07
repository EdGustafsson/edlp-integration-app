using System;

namespace learnpoint_test_consoleApp.Models
{
    public class SavedItem
    {
        public Guid Id { get; private set; }
        public string Type { get; set; }
        public int ExternalId { get; set; }
        public string ExternalSource { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
