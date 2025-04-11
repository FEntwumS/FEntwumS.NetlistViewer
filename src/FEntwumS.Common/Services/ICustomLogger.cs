namespace FEntwumS.Common.Services;

public interface ICustomLogger
{
    public void Log(string message, bool showOutput = false);
    
    public void Error(string message, Exception? ex = null, bool showOutput = true);
}