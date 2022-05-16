namespace MultivendorWebViewer.Configuration
{
    public interface ICopyable
    {
        object Copy(bool clone = false);
    }
    public interface ICopyable<T>
    {
        T Copy(bool clone = false);
    }
}