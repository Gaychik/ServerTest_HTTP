using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ServerTest
{
    public class RouteHandler
    {
        private Dictionary<KeyValuePair<string,string>, Func<HttpListenerRequest, Task<string>>> _routes = new();

        public void RegisterRoute(string route_name, Func<HttpListenerRequest, Task<string>> func, string method="GET")
        {
            var  key_route = new KeyValuePair<string,string>(route_name, method);
            _routes[key_route] = func;
        }

        public async Task<string> HandleRequest(HttpListenerRequest req)
        {

            var route_name = req.Url.AbsolutePath;
            var method = req.HttpMethod;


            if (_routes.TryGetValue(KeyValuePair.Create(route_name, method), out var handler))
            {

                return await handler(req);
            }
            else if(route_name.Contains("css"))
            {
               return  await _routes[KeyValuePair.Create("/css", "GET")](req);
            }
            else
            {
               return await _routes[KeyValuePair.Create("/Error", "GET")](req);
            }

        
        }
    }
}
