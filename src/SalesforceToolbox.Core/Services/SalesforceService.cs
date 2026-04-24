using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Salesforce.Common;
using Salesforce.Common.Models.Json;
using Salesforce.Force;
using SalesforceToolbox.Core.Interfaces;
using SalesforceToolbox.Core.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SalesforceToolbox.Core.Services
{
    public class SalesforceService : ISalesforceService
    {
        private ForceClient _forceClient;
        private AuthenticationClient _authClient;

        public bool IsConnected => _forceClient != null;
        public string InstanceUrl { get; private set; }
        public string Username { get; private set; }

        public async Task<bool> ConnectAsync(ConnectionProfile profile)
        {
            try
            {
                _authClient = new AuthenticationClient();
                string tokenUrl = GetTokenUrl(profile);

                if (!string.IsNullOrEmpty(profile.AccessToken))
                {
                    _forceClient = new ForceClient(profile.InstanceUrl, profile.AccessToken, "v56.0");
                    InstanceUrl = profile.InstanceUrl;
                    Username = profile.Username;
                    return true;
                }

                // Username-password flow requires password + security token concatenated
                throw new InvalidOperationException(
                    "No access token available. Please authenticate via the connection wizard.");
            }
            catch (Exception ex)
            {
                _forceClient = null;
                throw new Exception($"Connection failed: {ex.Message}", ex);
            }
        }

        public async Task<bool> ConnectWithCredentialsAsync(ConnectionProfile profile, string password)
        {
            try
            {
                _authClient = new AuthenticationClient();
                string tokenUrl = GetTokenUrl(profile);

                await _authClient.UsernamePasswordAsync(
                    profile.ClientId,
                    profile.ClientSecret,
                    profile.Username,
                    password,
                    tokenUrl);

                _forceClient = new ForceClient(
                    _authClient.InstanceUrl,
                    _authClient.AccessToken,
                    _authClient.ApiVersion);

                InstanceUrl = _authClient.InstanceUrl;
                Username = profile.Username;

                profile.AccessToken = _authClient.AccessToken;
                profile.InstanceUrl = _authClient.InstanceUrl;
                profile.LastConnected = DateTime.UtcNow;

                return true;
            }
            catch (Exception ex)
            {
                _forceClient = null;
                throw new Exception($"Authentication failed: {ex.Message}", ex);
            }
        }

        public async Task DisconnectAsync()
        {
            _forceClient = null;
            _authClient = null;
            InstanceUrl = null;
            Username = null;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string soql)
        {
            EnsureConnected();
            var result = await _forceClient.QueryAsync<T>(soql);
            return result.Records;
        }

        public async Task<T> GetRecordAsync<T>(string objectType, string id)
        {
            EnsureConnected();
            return await _forceClient.QueryByIdAsync<T>(objectType, id);
        }

        public async Task<IEnumerable<SObjectInfo>> GetSObjectsAsync()
        {
            EnsureConnected();
            var result = await _forceClient.GetObjectsAsync<JObject>();
            if (result?.SObjects == null) return Enumerable.Empty<SObjectInfo>();

            return result.SObjects.Select(t => new SObjectInfo
            {
                Name = t["name"]?.ToString(),
                Label = t["label"]?.ToString(),
                LabelPlural = t["labelPlural"]?.ToString(),
                KeyPrefix = t["keyPrefix"]?.ToString(),
                IsCustom = t["custom"]?.ToObject<bool>() ?? false,
                IsQueryable = t["queryable"]?.ToObject<bool>() ?? false
            }).ToList();
        }

        public async Task<SObjectDetail> DescribeObjectAsync(string objectName)
        {
            EnsureConnected();
            var result = await _forceClient.DescribeAsync<JObject>(objectName);

            var detail = new SObjectDetail
            {
                Name = result["name"]?.ToString(),
                Label = result["label"]?.ToString(),
                LabelPlural = result["labelPlural"]?.ToString()
            };

            var fields = result["fields"] as JArray;
            if (fields != null)
            {
                detail.Fields = fields.Select(f => new SObjectField
                {
                    Name = f["name"]?.ToString(),
                    Label = f["label"]?.ToString(),
                    Type = f["type"]?.ToString(),
                    Length = f["length"]?.ToObject<int>() ?? 0,
                    IsNillable = f["nillable"]?.ToObject<bool>() ?? false,
                    IsCustom = f["custom"]?.ToObject<bool>() ?? false,
                    IsExternalId = f["externalId"]?.ToObject<bool>() ?? false,
                    IsUnique = f["unique"]?.ToObject<bool>() ?? false
                }).ToList();
            }

            return detail;
        }

        public async Task<IEnumerable<UserInfo>> GetUsersAsync(string searchTerm = null)
        {
            EnsureConnected();
            string soql = "SELECT Id, Name, Username, Email, IsActive, ProfileId, Profile.Name, UserType, LastLoginDate, CreatedDate FROM User";
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string escaped = searchTerm.Replace("'", "\\'");
                soql += $" WHERE Name LIKE '%{escaped}%' OR Username LIKE '%{escaped}%' OR Email LIKE '%{escaped}%'";
            }
            soql += " ORDER BY Name LIMIT 200";

            var result = await _forceClient.QueryAsync<UserInfo>(soql);
            return result.Records;
        }

        public async Task<IEnumerable<ApexLog>> GetApexLogsAsync()
        {
            EnsureConnected();
            const string soql = "SELECT Id, LogUser.Name, Application, Operation, Request, Status, LogLength, LastModifiedDate, StartTime FROM ApexLog ORDER BY LastModifiedDate DESC LIMIT 100";
            var result = await _forceClient.QueryAsync<ApexLog>(soql);
            return result.Records;
        }

        public async Task<string> GetApexLogBodyAsync(string logId)
        {
            EnsureConnected();
            return await _forceClient.ExecuteRestApiAsync<string>($"sobjects/ApexLog/{logId}/Body");
        }

        public async Task DeleteApexLogAsync(string logId)
        {
            EnsureConnected();
            await _forceClient.DeleteAsync("ApexLog", logId);
        }

        private void EnsureConnected()
        {
            if (_forceClient == null)
                throw new InvalidOperationException("Not connected to Salesforce. Please connect first.");
        }

        private string GetTokenUrl(ConnectionProfile profile)
        {
            switch (profile.OrgType)
            {
                case OrgType.Sandbox:
                    return "https://test.salesforce.com";
                case OrgType.Custom:
                    return profile.InstanceUrl ?? "https://login.salesforce.com";
                default:
                    return "https://login.salesforce.com";
            }
        }
    }
}
