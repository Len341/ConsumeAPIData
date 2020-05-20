
namespace ConsumeAPIData
{
    public static class domain
    {
        public static string Url { get; set; }
        public static string createDate { get; set; }
    }
    public class regInfo
    {
        public string regName { get; set; }
        public string regState { get; set; }
        public string regEmail { get; set; }
        public string regTelephone { get; set; }
        public string regStreet { get; set; }
        public string regCity { get; set; }
    }

    public class techInfo
    {
        public string techName { get; set; }
        public string techState { get; set; }
        public string techEmail { get; set; }
        public string techTelephone { get; set; }
        public string techStreet { get; set; }
        public string techCity { get; set; }
    }

    public class adminInfo
    {
        public string adminName { get; set; }
        public string adminState { get; set; }
        public string adminEmail { get; set; }
        public string adminTelephone { get; set; }
        public string adminStreet { get; set; }
        public string adminCity { get; set; }
    }
}
