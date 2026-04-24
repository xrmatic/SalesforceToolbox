using System.Collections.Generic;
using System.Threading.Tasks;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.Core.Interfaces
{
    public interface ISalesforceService
    {
        bool IsConnected { get; }
        string InstanceUrl { get; }
        string Username { get; }
        Task<bool> ConnectAsync(ConnectionProfile profile);
        Task DisconnectAsync();
        Task<IEnumerable<T>> QueryAsync<T>(string soql);
        Task<T> GetRecordAsync<T>(string objectType, string id);
        Task<IEnumerable<SObjectInfo>> GetSObjectsAsync();
        Task<SObjectDetail> DescribeObjectAsync(string objectName);
        Task<IEnumerable<UserInfo>> GetUsersAsync(string searchTerm = null);
        Task<IEnumerable<ApexLog>> GetApexLogsAsync();
        Task<string> GetApexLogBodyAsync(string logId);
        Task DeleteApexLogAsync(string logId);
    }
}
