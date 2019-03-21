using System;
using System.Threading.Tasks;

namespace async_workshop.Controllers
{
    public class MyService
    {
        public async Task<string> GetNameAsync()
        {
            // simulate db
            await Task.Delay(100);
            return "Anders";
        }


















        public async Task CrashAsync()
        {
            // simulate db
            await Task.Delay(100);
            throw new Exception("I Crashed 😒");
        }

        public async void VoidCrash()
        {
            // simulate db
            await Task.Delay(100);
            throw new Exception("I crashed 🔥");
        }











        public async Task CrashAsyncWithNumber(int number)
        {
            await Task.Delay(100);
            throw new Exception("I Crashed 😒. Number: " + number);
        }
    }
}
