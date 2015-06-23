namespace HashTag.Logging.Client.Configuration
{
    public class ClientConfig
    {
        public const bool IGNORECASE_FLAG = false;

        public static string ApplicationName { get; set; }

        public static string ActiveEnvironment { get; set; }

        public static string ActiveUserName { get; set; }
    }
}
