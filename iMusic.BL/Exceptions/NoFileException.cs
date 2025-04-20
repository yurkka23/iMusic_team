namespace iMusic.BL.Exceptions;

public class NoFileException : Exception
{
    public NoFileException(string message) : base($"{message} (no file)") { }
}
