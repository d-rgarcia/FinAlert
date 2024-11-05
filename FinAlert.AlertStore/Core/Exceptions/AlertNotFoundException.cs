namespace FinAlert.AlertStore.Core.Exceptions;

[Serializable]
internal class AlertNotFoundException : Exception
{
    public AlertNotFoundException() : base()
    {
    }
    
    public AlertNotFoundException(string? message) : base(message)
    {
    }

    public AlertNotFoundException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}