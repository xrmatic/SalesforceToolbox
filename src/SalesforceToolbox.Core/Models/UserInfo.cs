using Newtonsoft.Json;

namespace SalesforceToolbox.Core.Models
{
    public class UserInfo
    {
        [JsonProperty("Id")]
        public string Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Username")]
        public string Username { get; set; }

        [JsonProperty("Email")]
        public string Email { get; set; }

        [JsonProperty("IsActive")]
        public bool IsActive { get; set; }

        [JsonProperty("ProfileId")]
        public string ProfileId { get; set; }

        [JsonProperty("Profile")]
        public ProfileRef Profile { get; set; }

        [JsonProperty("UserType")]
        public string UserType { get; set; }

        [JsonProperty("LastLoginDate")]
        public System.DateTime? LastLoginDate { get; set; }

        [JsonProperty("CreatedDate")]
        public System.DateTime? CreatedDate { get; set; }
    }

    public class ProfileRef
    {
        [JsonProperty("Name")]
        public string Name { get; set; }
    }
}
