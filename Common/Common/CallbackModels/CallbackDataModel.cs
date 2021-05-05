using System;

namespace Common.Common
{
    public class CallbackDataModel
    {
        public string Command { get; }
        public Guid NotifyId { get; }

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

        public static string ToCallbackData(CallbackDataModel model)
            => ToCallbackData(model.Command, model.NotifyId);

        public static string ToCallbackData(string command, Guid notifyId) =>
            string.Join(CommonResource.Separator, command, notifyId);
    }
}
