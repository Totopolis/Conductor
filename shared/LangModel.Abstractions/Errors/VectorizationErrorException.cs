namespace LangModel.Abstractions.Errors;

[Serializable]
public class VectorizationErrorException : Exception
{
	public VectorizationErrorException() { }
	
	public VectorizationErrorException(string message) : base(message) { }
	
	public VectorizationErrorException(string message, Exception inner) : base(message, inner) { }
}
