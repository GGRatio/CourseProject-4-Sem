using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Helpers
{
    public class SessionManager
    {
        private static string filePath = "session.json";

        public static void SaveUser(int  userId, string login)
        {
            var savedUser = new SavedUser
            {
                UserId = userId,
                Login = login,
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
