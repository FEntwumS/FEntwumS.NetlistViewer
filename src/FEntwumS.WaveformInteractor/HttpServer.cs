namespace FEntwumS.WaveformInteractor;

using System.Net;
using System.Text;

public class HttpServer
{
    private readonly HttpListener _listener;

    private string[] signalNames;

    public HttpServer(string[] prefixes)
    {
        if (!HttpListener.IsSupported)
        {
            throw new NotSupportedException("HttpListener is not supported.");
        }

        if (prefixes == null || prefixes.Length == 0)
        {
            throw new ArgumentException("At least one prefix must be specified.", nameof(prefixes));
        }

        _listener = new HttpListener();

        foreach (string prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix);
        }
    }

    public async Task StartAsync()
    {
        _listener.Start();
        Console.WriteLine("Listening... for FEEEENTWUUMS");

        // Handle one request at a time for simplicity
        while (true)
        {
            try
            {
                var context = await _listener.GetContextAsync();
                ProcessRequest(context);
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine($"Listener exception: {ex.Message}");
                break; // Exit loop if the listener is stopped
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }

    private void ProcessRequest(HttpListenerContext context)
    {
        var request = context.Request;

        string signalName;
        if (request.HttpMethod == "GET")
        {
            // Extract signal name from the query string (e.g., ?signalname=MySignal)
            signalName = request.QueryString["signalname"] ?? "No signal name provided";
        }
        else
        {
            signalName = "Unsupported HTTP method";
        }
        
        // check if signalname is in signal list

        // Construct the response
        string responseString = signalName;
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);

        var response = context.Response;
        response.ContentType = "text/plain"; // Set response content type
        response.ContentLength64 = buffer.Length;

        using var output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);

        Console.WriteLine($"Processed signal: {signalName}");
    }
    
    public void Stop()
    {
        _listener.Stop();
        Console.WriteLine("Server stopped.");
    }

    public void SetSignalNames(string[] signalNames)
    {
        this.signalNames = signalNames;
    }
}