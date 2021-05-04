namespace Services.Services.IoServices.FileServices
{
    public class IoRepository
    {
        public INotifyRemover NotifyRemover { get; }
        public INotifyWriter NotifyWriter { get; }
        public INotifyEditor NotifyEditor { get; }
        public INotifyReader NotifyReader { get; }

        public IoRepository(INotifyRemover notifyRemover, INotifyWriter  notifyWriter, INotifyEditor notifyEditor, INotifyReader notifyReader)
        {
            NotifyRemover = notifyRemover;
            NotifyWriter = notifyWriter;
            NotifyEditor = notifyEditor;
            NotifyReader = notifyReader;
        }
    }
}
