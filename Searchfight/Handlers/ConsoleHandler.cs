using System;
using System.Net;
using System.Threading.Tasks;

namespace Searchfight.Handlers
{
    public class ConsoleHandler : IConsoleHandler
    {
        public void WriteLine(string text)
        {
            Console.WriteLine(text);
        }
    }
}
