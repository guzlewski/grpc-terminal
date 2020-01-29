using System;
using System.Text;
using Grpc.Core;
using GrpcTerminal;
using Pastel;

namespace TerminalClient
{
    class ClientMain
    {
        static void Main(string[] args)
        {
            var (host, port, request) = ParseArgs(args);

            var channel = new Channel(host, port, ChannelCredentials.Insecure, new ChannelOption[] { new ChannelOption(ChannelOptions.MaxReceiveMessageLength, int.MaxValue) });

            var client = new Terminal.TerminalClient(channel);
            var reply = client.Execute(request);

            ProcessReply(reply);

            channel.ShutdownAsync().Wait();
        }

        static (string host, int port, Request request) ParseArgs(string[] args)
        {
            if (args.Length >= 3 && int.TryParse(args[1], out int port))
            {
                return (args[0], port, CreateRequest(args));
            }

            Console.WriteLine("Invalid arguments.");
            Console.WriteLine($"Usage: {AppDomain.CurrentDomain.FriendlyName} HOST PORT PROGRAM_NAME [--i] [ARG1] ...");
            Console.WriteLine($"Arguments are optional.");
            Console.WriteLine($"Flag --i to input data to stdin, end with EOF.");

            Environment.Exit(0);
            return (null, -1, null);
        }

        static Request CreateRequest(string[] args)
        {
            bool input = false;

            var request = new Request
            {
                Name = args[2],
                Stdin = ""
            };

            for (int i = 3; i < args.Length; i++)
            {
                if (args[i] != "--i")
                {
                    request.Args.Add(args[i]);
                }
                else
                {
                    input = true;
                }
            }

            if (input)
            {
                var builder = new StringBuilder();
                string line;

                Console.WriteLine("Stdin:".Pastel("FFFFFF"));

                while ((line = Console.ReadLine()) != null)
                {
                    builder.AppendLine(line);
                }

                request.Stdin = builder.ToString();
            }

            return request;
        }

        static void ProcessReply(Response response)
        {
            Console.WriteLine("Exit code: ".Pastel("00FF00") + response.Code.ToString().Pastel("FFFFFF"));
            Console.WriteLine("Stdout:".Pastel("FFFF00"));
            Console.WriteLine(response.Stdout);
            Console.WriteLine("Stderr:".Pastel("FF0000"));
            Console.WriteLine(response.Stderr);
        }
    }
}
