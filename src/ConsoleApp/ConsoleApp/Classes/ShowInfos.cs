using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using ConsoleApp.Interfaces;
using static System.Console;


namespace ConsoleApp.Classes
{
    internal class ShowInfos : IShowInfos
    {
        // Code inspired from: https://github.com/dotnet/dotnet-docker/blob/main/samples/dotnetapp/Program.cs
        // Variant of https://github.com/dotnet/core/tree/main/samples/dotnet-runtimeinfo
        // Ascii text: https://ascii.co.uk/text (Universe font)

        private const double Mega = 1024 * 1024;
        private const double Giga = Mega * 1024;

        private readonly string _nl = Environment.NewLine;
        
        public void Show()
        {

            WriteLine(
                $"         42{_nl}" +
                $"         42              ,d                             ,d{_nl}" +
                $"         42              42                             42{_nl}" +
                $" ,adPPYb,42  ,adPPYba, MM42MMM 8b,dPPYba,   ,adPPYba, MM42MMM{_nl}" +
                $"a8\"    `Y42 a8\"     \"8a  42    42P\'   `\"8a a8P_____42   42{_nl}" +
                $"8b       42 8b       d8  42    42       42 8PP\"\"\"\"\"\"\"   42{_nl}" +
                $"\"8a,   ,d42 \"8a,   ,a8\"  42,   42       42 \"8b,   ,aa   42,{_nl}" +
                $" `\"8bbdP\"Y8  `\"YbbdP\"\'   \"Y428 42       42  `\"Ybbd8\"\'   \"Y428{_nl}");

            var assemblyInformation = ((AssemblyInformationalVersionAttribute[])typeof(object).Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false))[0];
            var informationalVersionSplit = assemblyInformation.InformationalVersion.Split('+');

            WriteLine("=== .NET information ===");
            WriteLine("{0,-20} {1}", "Environment.Version", Environment.Version);
            WriteLine("{0,-20} {1}", "FrameworkDescription", RuntimeInformation.FrameworkDescription);
            WriteLine("{0,-20} {1}", "Libraries version", informationalVersionSplit[0]);
            WriteLine("{0,-20} {1}", "Libraries hash", informationalVersionSplit[1]);
            WriteLine("{0,-20} {1}", "Runtime identifier", RuntimeInformation.RuntimeIdentifier);
            WriteLine("{0,-20} {1}", "Runtime location", Path.GetDirectoryName(typeof(object).Assembly.Location));
            WriteLine();

            WriteLine("===Environment information ===");
            WriteLine($"{nameof(Environment.ProcessorCount),-20} {Environment.ProcessorCount}");
            WriteLine($"{nameof(RuntimeInformation.OSArchitecture),-20} {RuntimeInformation.OSArchitecture}");
            WriteLine($"{nameof(RuntimeInformation.OSDescription),-20} {RuntimeInformation.OSDescription}");
            WriteLine($"{nameof(Environment.OSVersion),-20} {Environment.OSVersion}");
            WriteLine();

            WriteLine("===Host information ===");
            WriteLine($"{nameof(Environment.UserName),-20} {Environment.UserName}");
            var hostName = Dns.GetHostName();
            WriteLine("{0,-20} {1}", "HostName", hostName);
            var ipAddresses = Dns.GetHostAddresses(hostName);
            for (var i = 0; i < ipAddresses.Length; i++)
            {
                WriteLine("{0,-20} {1}", $"IP address {i}", ipAddresses[i]);
            }
            WriteLine();

            WriteLine("===Memory information ===");
            // Gets the amount of the virtual memory, in bytes, allocated for the associated process.
            WriteLine("{0,-20} {1}", "VirtualMemorySize64", GetInBestUnit(Process.GetCurrentProcess().VirtualMemorySize64));
            // Gets the amount of physical memory, in bytes, allocated for the associated process.
            WriteLine("{0,-20} {1}", "WorkingSet64", GetInBestUnit(Process.GetCurrentProcess().WorkingSet64));
            // Gets the amount of private memory, in bytes, allocated for the associated process.
            WriteLine("{0,-20} {1}", "PrivateMemorySize64", GetInBestUnit(Process.GetCurrentProcess().PrivateMemorySize64));
            WriteLine();

            WriteLine("===Processor information ===");
            WriteLine("{0,-20} {1}", "Priority class", Process.GetCurrentProcess().PriorityClass);
            WriteLine("{0,-20} {1}", "User processor time",Process.GetCurrentProcess().UserProcessorTime);
            WriteLine("{0,-20} {1}", "Privileged processor time", Process.GetCurrentProcess().PrivilegedProcessorTime);
            WriteLine("{0,-20} {1}", "Total processor time", Process.GetCurrentProcess().TotalProcessorTime);
            WriteLine();
        }

        string GetInBestUnit(long size)
        {
            if (size < Mega)
            {
                return $"{size} bytes";
            }
            else if (size < Giga)
            {
                var megabytes = size / Mega;
                return $"{megabytes:F} MiB";
            }
            else
            {
                double gigabytes = size / Giga;
                return $"{gigabytes:F} GiB";
            }
        }
    }
}
