using System;
using Grpc.Core;
using GrpcTerminal;

namespace TerminalServer
{
    class ServerMain
    {
        static void Main(string[] args)
        {
            var (host, port) = ParseArgs(args);

            Server server = new Server
            {
                Services = { Terminal.BindService(new TerminalImpl()) },
                Ports = { new ServerPort(host, port, ServerCredentials.Insecure) }
            };

            server.Start();

            Console.WriteLine("Terminal server listening on port " + port);
            Console.WriteLine("Press any key to stop the server...");
            Console.ReadKey(true);

            server.ShutdownAsync().Wait();
        }

        static (string host, int port) ParseArgs(string[] args)
        {
            if (args.Length == 2 && int.TryParse(args[1], out int port))
            {
                return (args[0], port);
            }

            Console.WriteLine("Invalid arguments.");
            Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} HOST PORT");

            Environment.Exit(0);
            return (null, -1);
        }
    }
}
