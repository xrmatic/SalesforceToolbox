using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using SalesforceToolbox.Core.Models;

namespace SalesforceToolbox.Core.Services
{
    public class CredentialStore
    {
        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "SalesforceToolbox");

        private static readonly string ProfilesFilePath = Path.Combine(AppDataPath, "profiles.json");

        public List<ConnectionProfile> LoadProfiles()
        {
            try
            {
                if (!File.Exists(ProfilesFilePath))
                    return new List<ConnectionProfile>();

                var json = File.ReadAllText(ProfilesFilePath);
                var profiles = JsonConvert.DeserializeObject<List<ConnectionProfile>>(json)
                               ?? new List<ConnectionProfile>();

                foreach (var profile in profiles)
                    DecryptProfile(profile);

                return profiles;
            }
            catch
            {
                return new List<ConnectionProfile>();
            }
        }

        public void SaveProfiles(List<ConnectionProfile> profiles)
        {
            Directory.CreateDirectory(AppDataPath);

            var toSave = new List<ConnectionProfile>();
            foreach (var profile in profiles)
            {
                var copy = CloneProfile(profile);
                EncryptProfile(copy);
                toSave.Add(copy);
            }

            var json = JsonConvert.SerializeObject(toSave, Formatting.Indented);
            File.WriteAllText(ProfilesFilePath, json);
        }

        public void SaveProfile(ConnectionProfile profile)
        {
            var profiles = LoadProfiles();
            var existing = profiles.FindIndex(p => p.Id == profile.Id);
            if (existing >= 0)
                profiles[existing] = profile;
            else
                profiles.Add(profile);
            SaveProfiles(profiles);
        }

        public void DeleteProfile(string profileId)
        {
            var profiles = LoadProfiles();
            profiles.RemoveAll(p => p.Id == profileId);
            SaveProfiles(profiles);
        }

        private void EncryptProfile(ConnectionProfile profile)
        {
            if (!string.IsNullOrEmpty(profile.AccessToken))
            {
                profile.EncryptedAccessToken = Encrypt(profile.AccessToken);
                profile.AccessToken = null;
            }
            if (!string.IsNullOrEmpty(profile.RefreshToken))
            {
                profile.EncryptedRefreshToken = Encrypt(profile.RefreshToken);
                profile.RefreshToken = null;
            }
            if (!string.IsNullOrEmpty(profile.ClientSecret))
            {
                profile.EncryptedClientSecret = Encrypt(profile.ClientSecret);
                profile.ClientSecret = null;
            }
        }

        private void DecryptProfile(ConnectionProfile profile)
        {
            if (!string.IsNullOrEmpty(profile.EncryptedAccessToken))
                profile.AccessToken = Decrypt(profile.EncryptedAccessToken);
            if (!string.IsNullOrEmpty(profile.EncryptedRefreshToken))
                profile.RefreshToken = Decrypt(profile.EncryptedRefreshToken);
            if (!string.IsNullOrEmpty(profile.EncryptedClientSecret))
                profile.ClientSecret = Decrypt(profile.EncryptedClientSecret);
        }

        private string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText)) return plainText;
            var data = Encoding.UTF8.GetBytes(plainText);
            var encrypted = ProtectedData.Protect(data, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encrypted);
        }

        private string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText)) return cipherText;
            try
            {
                var data = Convert.FromBase64String(cipherText);
                var decrypted = ProtectedData.Unprotect(data, null, DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(decrypted);
            }
            catch
            {
                return string.Empty;
            }
        }

        private ConnectionProfile CloneProfile(ConnectionProfile source)
        {
            return new ConnectionProfile
            {
                Id = source.Id,
                Name = source.Name,
                InstanceUrl = source.InstanceUrl,
                Username = source.Username,
                ClientId = source.ClientId,
                OrgType = source.OrgType,
                AccessToken = source.AccessToken,
                RefreshToken = source.RefreshToken,
                ClientSecret = source.ClientSecret,
                EncryptedAccessToken = source.EncryptedAccessToken,
                EncryptedRefreshToken = source.EncryptedRefreshToken,
                EncryptedClientSecret = source.EncryptedClientSecret,
                LastConnected = source.LastConnected,
                IsDefault = source.IsDefault
            };
        }
    }
}
