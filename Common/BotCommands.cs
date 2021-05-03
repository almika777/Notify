namespace Common
{
    public static class BotCommands
    {
        public static class OnMessage
        {
            public static string StartCommand => "/start";
            public static string ShowNotificationCommand => "/show";
        }

        public static class OnCallback
        {
            public static string ShowCallbackDataCommand => "show";
            public static string RemoveCallbackCommand => "remove";
            public static string EditCallbackCommand => "edit";
            public static string EditNameCallbackCommand => "edit_name";
            public static string EditDateCallbackCommand => "edit_date";
        }
    }
}
