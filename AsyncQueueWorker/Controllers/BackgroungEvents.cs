using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AsyncQueueWorker.Controllers
{
    public class MyFirstEventArgs : EventArgs
    {
        public string Message { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class BackgroungEventsController : ControllerBase
    {
        public static event EventHandler<MyFirstEventArgs> MyFirstEvent;

        static BackgroungEventsController()
        {
            MyFirstEvent += MyCallbackFunction1;
            MyFirstEvent += MyCallbackFunction2;
            
            MyFirstEvent += (object sender, MyFirstEventArgs e) =>
            {
                Console.WriteLine(
                    $"(object sender, MyFirstEventArgs e) -> {e.Message}");
            };
        }

        [HttpGet]
        public IActionResult Get([FromQuery] string message)
        {
            MyFirstEvent?.Invoke(
                this, new MyFirstEventArgs { Message = message });

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            MyFirstEvent -= MyCallbackFunction2;
            
            while (MyFirstEvent.GetInvocationList()
                .Any(e => e.Method.Name == nameof(MyCallbackFunction3)))
            {
                MyFirstEvent -= MyCallbackFunction3;
            }

            return Ok();
        }

        [HttpPost]
        public IActionResult Post()
        {
            MyFirstEvent += MyCallbackFunction3;

            return Ok();
        }

        public static void MyCallbackFunction1(
            object sender, MyFirstEventArgs e)
        {
            Console.WriteLine(
                $"{nameof(MyCallbackFunction1)} -> {e.Message}");
        }

        public static void MyCallbackFunction2(
            object sender, MyFirstEventArgs e)
        {
            Console.WriteLine(
                $"{nameof(MyCallbackFunction2)} -> {e.Message}");
        }

        public static void MyCallbackFunction3(
            object sender, MyFirstEventArgs e)
        {
            Console.WriteLine(
                $"{nameof(MyCallbackFunction3)} -> {e.Message}");
        }

    }
}
