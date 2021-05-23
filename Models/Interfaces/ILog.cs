namespace Models.Interfaces
{
    public interface ILog
    {
        void Debug(string TAG, string message);
        void Info(string TAG, string message);
        void Warning(string TAG, string message);
        void Error(string TAG, string message);
    }
}
