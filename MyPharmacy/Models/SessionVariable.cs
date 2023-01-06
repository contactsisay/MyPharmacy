namespace MyPharmacy.Models
{
    public class SessionVariable
    {
        public const string SessionKeyUserId = "SessionKeyUserId";
        public const string SessionKeyUserEmail = "SessionKeyUserEmail";
        public const string SessionKeyUserRoleId = "SessionKeyUserRoleId";
        public const string SessionKeySessionId = "SessionKeySessionId";        

        public enum SessionKeyName
        {
            SessionKeyUserId = 0,
            SessionKeyUserEmail = 1,
            SessionKeyUserRoleId = 2,
            SessionKeySessionId = 3,
        }
    }
}
