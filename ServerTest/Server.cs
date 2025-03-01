using System.Net;

class Server
{
    private HttpListener _listener;

    public Server(string[] prefixes)
    {
        _listener = new HttpListener();
        foreach (string prefix in prefixes)
        {
            _listener.Prefixes.Add(prefix);
        }   
    }
    public async Task Start()
    { 
         _listener.Start();
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
        var route = context.Request.Url.AbsolutePath;
        Console.WriteLine($"Пришел {context.Request.HttpMethod} запрос по маршруту: {route}");
        var response = "";
        var type_content = "text/html";
        if (route == "/register")
            if (context.Request.HttpMethod == "POST")
            {

                await GetDataFromStream(context.Request.InputStream);
            }
            else
            {
                response = File.ReadAllText("templates/register.html");

            }
        if (route == "/login")
        {
            if (context.Request.HttpMethod == "POST")
            {

            }
            else
            {
                response = File.ReadAllText("templates/login.html");

            }
        }
        if (route.Contains("css"))
        {
         
            response = File.ReadAllText(route[1..]);
            type_content="text/css";
        }


        context.Response.ContentType = type_content;
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

        
        using (var writer = new StreamWriter(output))
        {
            Console.WriteLine("Запись данных в выходной поток...");
             await writer.WriteAsync(response);
            await writer.FlushAsync();
        }
 
    }
    private async Task SaveMessage(HttpListenerRequest req)
    {

       var message_obj = await  GetDataFromStream(req.InputStream);
        Console.WriteLine (message_obj);
 
    }



}