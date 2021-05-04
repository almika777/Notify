namespace Common
{
    public static class BotCommands
    {
        public static class OnMessage
        {
            public static string Start => "/start";
            public static string ShowNotification => "/show";
        }

        public static class OnCallback
        {
            public static string Show => "show";
            public static string Remove => "remove";
            public static string EditEntry => "edit";
            public static string EditNotifyName => "edit_name";
            public static string EditNotifyDate => "edit_date";
            public static string SetFrequency => "frequency";
        }
    }
}
