﻿using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Konamiman.NestorPreTer
{
    class Program
    {
        const string ApplicationFile = "npr.com.z80";

        static int Main(string[] args)
        {
            //This allows embeding the Z80DotNet.dll dependency within the
            //executable file itself, so the whole program is one single .exe
            AppDomain.CurrentDomain.AssemblyResolve += (sender, a) => {
                var resourceName = new AssemblyName(a.Name).Name + ".dll";
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program), resourceName))
                {
                    var assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };

            var commandLineArgs = string.Join(" ", args);
            var commandLineLengthInBytes = Encoding.ASCII.GetBytes(commandLineArgs).Length;
            if (commandLineLengthInBytes > 127)
            {
                Console.WriteLine($"*** Command line must be at most 127 characters (was {commandLineLengthInBytes})");
                return 1;
            }

            byte[] application;
            try
            {
                var appStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(typeof(Program), ApplicationFile);
                var memStream = new MemoryStream();
                appStream.CopyTo(memStream);
                application = memStream.ToArray();
                memStream.Dispose();
                appStream.Dispose();
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
