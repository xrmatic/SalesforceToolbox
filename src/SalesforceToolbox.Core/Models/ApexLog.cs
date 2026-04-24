using Newtonsoft.Json;

namespace SalesforceToolbox.Core.Models
{
    public class ApexLog
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("LogUser")]
        public LogUserRef LogUser { get; set; }

        [JsonProperty("Application")]
        public string Application { get; set; }

        [JsonProperty("Operation")]
        public string Operation { get; set; }

        [JsonProperty("Request")]
        public string Request { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("LogLength")]
        public long LogLength { get; set; }

        [JsonProperty("LastModifiedDate")]
        public System.DateTime? LastModifiedDate { get; set; }

        [JsonProperty("StartTime")]
        public System.DateTime? StartTime { get; set; }
    }

    public class LogUserRef
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
