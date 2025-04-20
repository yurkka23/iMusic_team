namespace iMusic.BL.Exceptions;

public class NoRightsException : Exception
{
    public NoRightsException(string message) : base($"{message} (no rights)") { }
}