namespace Common
{
    public static class CommonResource
    {
        public static string Separator => "|";

        public static string[] DateFormats => new[]
        {
            "dd.MM.yyyy",
            "dd/MM/yyyy",
            "dd.MM.yyyy HH:mm",
            "dd/MM/yyyy HH:mm",
        };
    }
}
