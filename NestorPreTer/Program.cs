using System;
using System.IO;

namespace Konamiman.NestorPreTer
{
    class Program
    {
        const string ApplicationFile = "npr.com.z80";

        static int Main(string[] args)
        {
            var commandLineArgs = string.Join(" ", args);
            if (commandLineArgs.Length > 127)
            {
                Console.WriteLine($"*** Command line must be at most 127 characters (was {commandLineArgs.Length})");
                return 1;
            }

            byte[] application;
            try
            {
                application = File.ReadAllBytes(ApplicationFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** Error loading the application file {ApplicationFile} : {ex.Message}");
                return 2;
            }

            var runner = new MsxDosAppRunner(application);
            try
            {
                return runner.Run(commandLineArgs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"*** Error running the application: {ex.Message}");
                return 3;
            }
        }
    }
}
