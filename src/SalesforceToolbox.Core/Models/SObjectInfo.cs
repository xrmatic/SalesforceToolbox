using Newtonsoft.Json;
using System.Collections.Generic;

namespace SalesforceToolbox.Core.Models
{
    public class SObjectInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("labelPlural")]
        public string LabelPlural { get; set; }

        [JsonProperty("keyPrefix")]
        public string KeyPrefix { get; set; }

        [JsonProperty("custom")]
        public bool IsCustom { get; set; }

        [JsonProperty("queryable")]
        public bool IsQueryable { get; set; }
    }

    public class SObjectDetail
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("labelPlural")]
        public string LabelPlural { get; set; }

        [JsonProperty("fields")]
        public List<SObjectField> Fields { get; set; } = new List<SObjectField>();

        [JsonProperty("recordCount")]
        public long RecordCount { get; set; }
    }

    public class SObjectField
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("length")]
        public int Length { get; set; }

        [JsonProperty("nillable")]
        public bool IsNillable { get; set; }

        [JsonProperty("custom")]
        public bool IsCustom { get; set; }

        [JsonProperty("externalId")]
        public bool IsExternalId { get; set; }

        [JsonProperty("unique")]
        public bool IsUnique { get; set; }
    }
}
