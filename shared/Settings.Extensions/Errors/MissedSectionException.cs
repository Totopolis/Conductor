namespace Settings.Extensions.Errors;

[Serializable]
public sealed class MissedSectionException : Exception
{
    private MissedSectionException()
    {
    }

    public MissedSectionException(string message) : base(message)
    {
    }
}
