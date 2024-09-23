namespace LangModel.Abstractions.Errors;

[Serializable]
public class BadQuestionException : Exception
{
    public BadQuestionException()
    {
    }

    public BadQuestionException(string message) : base(message)
    {
    }

    public BadQuestionException(string message, Exception inner) : base(message, inner)
    {
    }
}
