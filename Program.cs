using Microsoft.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SpotifyAuthenticator
{
    class Program
    {
        static void Main(string[] args)
        {
            var app = new CommandLineApplication { Name = "Spotify Authenticator" };
            app.HelpOption("-?|-h|--help");

            app.Command("authorize", Authorize);

            app.OnExecute(() => 0);

            app.Execute(args);
        }

        private static void Authorize(CommandLineApplication command)
        {
            var clientIdOption = command.Option("-c|--clientId <clientId>", "The application client id", CommandOptionType.SingleValue);

            var clientSecretOption = command.Option("-k|--clientSecret <clientSecret>", "The application client secret", CommandOptionType.SingleValue);

            var scopeOption = command.Option("-s|--scope <scope>", "The application client secret", CommandOptionType.MultipleValue);

            var outputOption = command.Option("-o|--output <path>", "Path to output token as json", CommandOptionType.SingleValue);

            var browserPathOption = command.Option("-b|--browser <path>", "Path to the browser to start", CommandOptionType.SingleValue);

            command.OnExecute(
                () =>
                {
                    TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();

                    Scope scope = scopeOption.Values
                        .Select(v => Enum.Parse<Scope>(v))
                        .Aggregate(new Scope(), (a, s) => a | s);

                    var auth = new AutorizationCodeAuth()
                    {
                        ClientId = clientIdOption.Value(),
                        RedirectUri = "http://localhost",
                        Scope = scope,
                        ShowDialog = true,
                        OpenUriAction = uri => Process.Start(new ProcessStartInfo { FileName = browserPathOption.Value(), Arguments = uri })
                    };

                    auth.OnResponseReceivedEvent += response =>
                    {
                        Token token = auth.ExchangeAuthCode(response.Code, clientSecretOption.Value());
                        string json = JsonConvert.SerializeObject(token);
                        tcs.SetResult(json);
                    };

                    try
                    {
                        try
                        {
                            auth.StartHttpServer();
                            auth.DoAuth();

                            Task.WaitAll(new[] { tcs.Task }, TimeSpan.FromSeconds(30));

                            using (var stream = File.OpenWrite(outputOption.Value()))
                            {
                                using (var writer = new StreamWriter(stream))
                                {
                                    writer.Write(tcs.Task.Result);
                                }
                            }

                            Console.WriteLine($"Successfully wrote token to '{outputOption.Value()}'");

                            return 0;
                        }
                        finally
                        {
                            auth.StopHttpServer();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"Failed to retrieve token: '{e.Message}'");

                        return -1;
                    }
                }
            );
        }
    }
}
