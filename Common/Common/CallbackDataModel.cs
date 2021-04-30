using System;

namespace Common.Common
{
    public class CallbackDataModel
    {
        public string Command { get; set; }
        public Guid NotifyId { get; set; }

        public CallbackDataModel(string command, Guid notifyId)
        {
            Command = command;
            NotifyId = notifyId;
        }

        public static CallbackDataModel FromCallbackData(string callbackData)
        {
            var props = callbackData.Split(CommonResource.Separator);
            var id = Guid.TryParse(props[1], out var guid) ? guid : Guid.Empty;

            return new CallbackDataModel(props[0], id);
        }

        public static string ToCallbackData(CallbackDataModel model) =>
            string.Join(CommonResource.Separator, model.Command, model.NotifyId);
    }
}
