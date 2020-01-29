using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using GrpcTerminal;

namespace TerminalServer
{
    class TerminalImpl : Terminal.TerminalBase
    {
        private ProcessStartInfo CreateStartInfo(string name, IEnumerable<string> args)
        {
            var info = new ProcessStartInfo
            {
                CreateNoWindow = true,
                FileName = name,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true
            };

            foreach (var arg in args)
            {
                info.ArgumentList.Add(arg);
            }

            return info;
        }

        public override Task<Response> Execute(Request request, ServerCallContext context)
        {
            Response response = new Response();

            using var process = new Process
            {
                StartInfo = CreateStartInfo(request.Name, request.Args),
            };

            try
            {
                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                process.OutputDataReceived += (sender, args) => output.AppendLine(args.Data);
                process.ErrorDataReceived += (sender, args) => error.AppendLine(args.Data);

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.StandardInput.Write(request.Stdin);
                process.StandardInput.Flush();
                process.StandardInput.Close();

                process.WaitForExit();

                response.Stderr = error.ToString();
                response.Stdout = output.ToString();

                response.Code = process.ExitCode;
            }
            catch (Exception ex)
            {
                response.Code = ex.HResult;
                response.Stderr = ex.ToString();
            }

            return Task.FromResult(response);
        }
    }
}