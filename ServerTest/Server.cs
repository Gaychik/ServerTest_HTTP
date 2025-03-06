using ServerTest;
using System.Net;
using System.Runtime.CompilerServices;

class Server
{
    private HttpListener _listener;
    private RouteHandler _routeHandler;
    public Server(string[] prefixes)
    {
        _listener = new HttpListener();
        _routeHandler = new RouteHandler();
        foreach (string prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix);
        }   
    }
    public async Task Start()
    { 
         _listener.Start();
        _routeHandler.RegisterRoute("/register",RegisterGet);
        _routeHandler.RegisterRoute("/login", LoginGet);
        _routeHandler.RegisterRoute("/", Index);
        _routeHandler.RegisterRoute("/Error", Error);
        _routeHandler.RegisterRoute("/css", Get_File_CSS);
        await  Main();

    }

    public async Task Main()
    {
        Console.WriteLine("Run server...");
        while (true)
        {
          var context = await _listener.GetContextAsync();

          await  ControllerHandlers(context);
        }
    }

    public async Task ControllerHandlers(HttpListenerContext context)
    {
        var content_type = "text/html";

        var response = await _routeHandler.HandleRequest(context.Request);
        if (context.Request.Url.AbsolutePath.Contains("css"))
        {
            content_type = "text/css";
        }
        context.Response.ContentType = content_type;
          await SetDataToStream(context.Response.OutputStream, response);
        context.Response.Close();
    }

    private async Task<string> GetDataFromStream(Stream input)
    { 
         using (var reader = new StreamReader(input))
        {

           return  await reader.ReadToEndAsync();
        }
    }

    private async Task SetDataToStream(Stream output,string response)
    {

        
        using (var writer = new StreamWriter(output, System.Text.Encoding.UTF8 ))
        {
            Console.WriteLine("Запись данных в выходной поток...");
             await writer.WriteAsync(response);
            await writer.FlushAsync();
        }
 
    }

    private async Task<string> RegisterGet(HttpListenerRequest req)
    {
      
       return  await File.ReadAllTextAsync("templates/register.html");

    }
    private async Task<string> Index(HttpListenerRequest req)
    {
        return await File.ReadAllTextAsync("templates/index.html");

    }
    private async Task<string> LoginGet(HttpListenerRequest req)
    {
        return await File.ReadAllTextAsync("templates/login.html");

    }
    private async Task<string> Error(HttpListenerRequest req)
    {
        return "Мы сожалеем, что страница не найдена...";

    }

    private async Task<string> Get_File_CSS(HttpListenerRequest req)
    {
        var route = req.Url.AbsolutePath[1..];
        return await File.ReadAllTextAsync(route);

    }
    




}