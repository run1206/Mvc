using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;

namespace SimpleWebSite.Controllers
{
    public class HomeController
    {
        public string Index()
        {
            return "Hi from MVC";
        }

        public string GetUser(int id)
        {
            return $"User: {id}";
        }

        public IDictionary<string, string> Dict()
        {
            return new Dictionary<string, string> {
                {"first", "wall" },
                {"second", "floor" }
            };
        }
    }
}
