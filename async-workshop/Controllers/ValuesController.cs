using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace async_workshop.Controllers
{
    //[Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {

        private MyService service = new MyService();


        /**
         * 
         * Calling a async method will return a task. If you do not await it you will not get it's value.
         * 
         */
        [HttpGet("/name")]
        public ActionResult<string> Name()
        {
            var name = service.GetNameAsync();

            return $"My name is: {name}";

            // returns  My name is: task.ToString();
            // My name is: System.Runtime.CompilerServices.AsyncTaskMethodBuilder`1+AsyncStateMachineBox`1[System.String,async_workshop.Controllers.MyService+<GetNameAsync>d__0]
        }


        [HttpGet("/name-correct")]
        public async Task<ActionResult<string>> NameCorrect()
        {
            var name = await service.GetNameAsync();

            return $"My name is: {name}";
            // returns the awaited value of the task
            // my name is: Anders
        }




































        /*
         * 
         * EXCEPTIONS
         * 
         * lessons:
         * exceptions not caught when not awaiting
         * exceptions caugh where awaiting
         * where does uncaught exceptions go?
         * async void crashes process
         * try/catch await?
         * 
         * 
         */


        [HttpGet("/throw")]
        public async Task<ActionResult<string>> Throw()
        {
            Task nameTask;
            try
            {
                nameTask = service.CrashAsync();
            }
            catch (Exception ex)
            {
                return $"Exception message: {ex.Message}";
            }

            return "did not crash";
            // returns did not crash
        }

        [HttpGet("/throw-correct")]
        public async Task<ActionResult<string>> ThrowCorrect()
        {
            Task nameTask;
            try
            {
                // renamed variable to xTask
                nameTask = service.CrashAsync();

                // awaits the result of the task
                await nameTask;
            }
            catch (Exception ex)
            {
                return $"Exception message: {ex.Message}";
                // returns the exception message
            }

            return "did not crash";
        }




        /*
         * 
         * async void method that does not await a task that crashes. What will happen?
         * This will call the method (possibly on a different thread)
         * Since it does not await service.Crash it will not trigger the Try/Catch
         * Write did not crash to the body stream (because asp.net still has the request open)
         */
        [HttpGet("/void")]
        public async void Void()
        {
            Response.ContentType = "text/plain; charset=utf-8";

            Task nameTask;
            try
            {
                nameTask = service.CrashAsync();

                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("did not crash"));

            }
            catch (Exception ex)
            {
                var message = ex.Message;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"Exception message: {message}"));
            }

        }

        /*
         * 
         * async void that awaits a task that crashes.
         * This will call the method (possibly on a different thread)
         * return the call to it's caller (asp.net) since asp.net can't await a void method
         * trigger the Try/Catch and do nothing
         * 
         */
        [HttpGet("/void2")]
        public async void VoidAwait()
        {
            Response.ContentType = "text/plain; charset=utf-8";

            Task nameTask;
            try
            {
                nameTask = service.CrashAsync();

                await nameTask;

                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("did not crash"));

            }
            catch (Exception ex)
            {
                var message = ex.Message;
                //await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"Exception message: {message}"));


            }
        }

        /*
         * 
         * async void that awaits a task that crashes.
         * This will call the method (possibly on a different thread)
         * return the call to it's caller (asp.net) since asp.net can't await a void method
         * await the exception
         * When there is no-one catching our exception (it can't bubble up to the caller (asp.net)) it crashes the proccess. Thank god it's not ON ERROR RESUME NEXT.
         * 
         * 
         */
        [HttpGet("/void2.1")]
        public async void VoidAwaitNoCatch()
        {
            Response.ContentType = "text/plain; charset=utf-8";

            Task nameTask;
            nameTask = service.CrashAsync();

            await nameTask;
        }



        /**
         * 
         * async task: that awaits a task that crashes.
         * This will call the method service.crash (possibly on a different thread)
         * await the task
         * trigger the Try/Catch
         * Write to the body stream (which is open because asp.net await our task)
         * 
         */
        [HttpGet("/void3")]
        public async Task NameVoidAwait2()
        {
            Response.ContentType = "text/plain; charset=utf-8";

            Task nameTask;
            try
            {
                nameTask = service.CrashAsync();

                await nameTask;

                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("did not crash"));
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"Exception message: {message}"));
            }
        }

        /*
         * 
         * async task: That calls a async void method (that it can't await, because void you know...)
         * This will call the method service.VoidCrash() (possibly on a different thread)
         * it will not trigger the Try/catcg since it could not await VoidCrash.
         * Write to the body stream "did not crash"
         * When there is no-one catching our exception (it can't bubble up to the caller, asp.net) it crashes the proccess. Thank god it's not ON ERROR RESUME NEXT.
         * 
         */
        [HttpGet("/void4")]
        public async Task voidCrash()
        {
            Response.ContentType = "text/plain; charset=utf-8";

            try
            {
                service.AsyncVoidCrash();

                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("did not crash"));
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"Exception message: {message}"));
            }
        }


        /*
         * 
         * async task: That calls a async void method (that it can't await, because void you know...)
         * This will call the method service.VoidCrash() (possibly on a different thread)
         * it will not trigger the Try/catcg since it could not await VoidCrash.
         * Write to the body stream "did not crash"
         * When there is no-one catching our exception (it can't bubble up to the caller, asp.net) it crashes the proccess. Thank god it's not ON ERROR RESUME NEXT.
         * 
         */
        [HttpGet("/throw3/{number}")]
        public async Task Throw3(int number)
        {
            Response.ContentType = "text/plain; charset=utf-8";

            try
            {
                var task = service.CrashAsyncWithNumber(number);

                //// wait for CrashAsync to throw and then force GC pressure.
                //await Task.Delay(200);
                //GC.Collect();

                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("did not crash"));
            }
            catch (Exception ex)
            {
                var message = ex.Message;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes($"Exception message: {message}"));
            }
        }

        [HttpGet("/gc")]
        public string TriggerGC()
        {
            GC.Collect();
            return "Force GC";
        }

        [HttpGet("/hardwork")]
        public string Hardwork()
        {
            var task = service.DoHardWorkAsync();

            return "Done with hardwork";
        }

        [HttpGet("/hardwork-correct")]
        public async Task<string> Hardwork(CancellationToken cancellationToken)
        {
            var task = service.DoHardWorkAsync(cancellationToken);

            await task;

            return "Done with hardwork";
        }

        [HttpGet("/hardwork-correct2")]
        public async Task<string> Hardwork2()
        {
            using (var cts = new CancellationTokenSource())
            {
                cts.CancelAfter(5000);

                await service.DoHardWorkAsync(cts.Token);
            }
            return "Done with hardwork";
        }
    }
}
