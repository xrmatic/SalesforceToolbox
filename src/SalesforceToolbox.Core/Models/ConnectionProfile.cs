using Newtonsoft.Json;
using System;

namespace SalesforceToolbox.Core.Models
{
    public class ConnectionProfile
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string InstanceUrl { get; set; }
        public string Username { get; set; }
        public string ClientId { get; set; }
        public OrgType OrgType { get; set; }

        [JsonIgnore]
        public string AccessToken { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        [JsonIgnore]
        public string ClientSecret { get; set; }

        public string EncryptedAccessToken { get; set; }
        public string EncryptedRefreshToken { get; set; }
        public string EncryptedClientSecret { get; set; }

        public DateTime LastConnected { get; set; }
        public bool IsDefault { get; set; }
    }

    public enum OrgType
    {
        Production,
        Sandbox,
        Custom
    }
}
