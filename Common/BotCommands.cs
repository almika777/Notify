namespace Common
{
    public static class BotCommands
    {
        public static class OnMessage
        {
            public static string Start => "/start";
            public static string ShowNotification => "/show";
            public static class Admin
            {
                public static string Members => "/members";
            }
        }

        public static class OnCallback
        {
            public static string Show => "show";
            public static string Remove => "remove";
            public static string EditEntry => "edit";
            public static string SetFrequency => "frequency";

            public class Edit
            {
                public static string NotifyName => "edit_name";
                public static string NotifyDate => "edit_date";
                public static string NotifyFrequency => "edit_frequency";
            }
        }
    }
}
