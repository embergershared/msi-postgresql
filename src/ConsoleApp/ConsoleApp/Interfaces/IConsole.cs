using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    internal interface IConsole
    {
        Task<bool> RunAsync();
    }
}