using Newtonsoft.Json;
using System;
using System.IO;

namespace Energy.Helpers
{
    public class SessionManager
    {
        private static string filePath = "session.json";

        public static void SaveUser(int userId, string login, string role)
        {
            var savedUser = new SavedUser
            {
                UserId = userId,
                Login = login,
                Role = role,
                SavedAt = DateTime.Now
            };

            string json = JsonConvert.SerializeObject(savedUser, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }

        public static SavedUser LoadUser()
        {
            if (File.Exists(filePath))
            {
                try
                {
                    string json = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<SavedUser>(json);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        public static void ClearSession()
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        public static bool HasSavedSession()
        {
            return File.Exists(filePath);
        }
    }
}