using System;

namespace Energy.Helpers
{
    [Serializable]
    public class SavedUser
    {
        public int UserId { get; set; }
        public string Login { get; set; }
        public string Role { get; set; }
        public DateTime SavedAt { get; set; }
    }
}