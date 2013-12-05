using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ApplicasaRESTClientnet;
using System.Dynamic;
using Newtonsoft.Json;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            RESTClient client = new RESTClient();
            client.ApplicationID = "xxxxxx";
            client.ApplicationVersion = 1.0f;
            client.IsSandbox = true;
            client.Platform = Platforms.iOS;
            
            var t = client.Initialize();
            t.Wait();
            var user = t.Result;
            t = client.RegisterWithUserNameAndPassword("yyyyyy", "");
            t.Wait();
            user = t.Result;
            t = client.LoginWithUserNameAndPassword("yyyyyy","");
            t.Wait();
            user = t.Result;


        }
    }
}
