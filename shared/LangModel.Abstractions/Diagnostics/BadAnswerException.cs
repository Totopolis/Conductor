namespace LangModel.Abstractions.Errors;

[Serializable]
public class BadAnswerException : Exception
{
    private BadAnswerException()
    {
    }

    public BadAnswerException(string message) : base(message)
    {
    }

    public BadAnswerException(string message, Exception inner) : base(message, inner)
    {
    }
}
