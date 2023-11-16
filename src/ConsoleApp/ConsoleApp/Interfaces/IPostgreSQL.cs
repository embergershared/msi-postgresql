using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp.Interfaces
{
    internal interface IPostgreSql
    {
        Task<bool> ConnectAsync();
        Task<string> GetDataAsync();
    }
}
