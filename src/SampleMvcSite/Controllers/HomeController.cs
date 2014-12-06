using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using SampleMvcSite.Models;
using TagCache.Redis;
using TagCache.Redis.Interfaces;

namespace SampleMvcSite.Controllers
{
    public class ListLogger : IRedisCacheLogger
    {
        public List<string> Logs { get; set; }

        public ListLogger()
        {
            Logs = new List<string>();
        }

        public void Log(string method, string arg, string message)
        {
            Logs.Insert(0,string.Format("{3} RedisLog> {0}({1}) : {2}", method, arg, message,DateTime.Now));
        }
    }

    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public static ListLogger Logger { get; set; }


        public ActionResult Index()
        {
            if (Logger == null)
            {
                Logger = new ListLogger();
            }

            var stopwatch = new Stopwatch();

            var result = new MyViewModel();

            stopwatch.Start();
            var cache = new RedisCacheProvider();
            cache.Logger = Logger;
            stopwatch.Stop();
            result.SetupTimeMs = stopwatch.ElapsedMilliseconds;

            stopwatch.Reset();
            stopwatch.Start();
            var item = cache.Get<Models.SampleModel>("models:sample1");
            if (item == null)
            {
                item = new SampleModel()
                       {
                           DateOfBirth = DateTime.Now,
                           Name = "Hello",
                           Surname = "World",
                           Other = new List<OtherModels>()
                                   {
                                       new OtherModels(){Foo = 1, Bar = 20.928},
                                       new OtherModels(){Foo = 31, Bar = 30.3328},
                                   } 
                       };
                cache.Set("models:sample1", item, DateTime.Now.AddSeconds(5), new List<string>(){"Test1", "Test2"});
            }
            stopwatch.Stop();
            result.LoadTimeMs = stopwatch.ElapsedMilliseconds;

            result.Data = item;

            return View(result);
        }


        public ActionResult ViewLog()
        {
            return View(Logger.Logs);
        }
    }
}
