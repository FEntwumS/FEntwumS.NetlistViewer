namespace FEntwumS.NetlistReaderFrontend.Services;

public interface ICustomLogger
{
    public void Log(string message, bool showOutput = false);
    
    public void Error(string message, bool showOutput = true);
}