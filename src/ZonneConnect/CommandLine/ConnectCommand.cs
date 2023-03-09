using Newtonsoft.Json;
using System.CommandLine;
using ZonneConnect.Zonneplan;

namespace ZonneConnect.CommandLine;

class ConnectCommand : Command
{
    private readonly ZonneplanApi _api;

    public ConnectCommand(ZonneplanApi api) : base("connect", "Connects to Zonneplan One with an email address. This will generate a token that needs to be approved by email.")
    {
        _api = api ?? throw new ArgumentNullException(nameof(api));

        var emailArg = new Argument<string>("email", "The email address you want to connect to Zonneplan One.")
        {
            Arity = ArgumentArity.ExactlyOne
        };

        AddArgument(emailArg);
        this.SetHandler(Execute, emailArg);
    }

    private async Task Execute(string email)
    {
        Header.WriteHeader();

        Console.WriteLine($"Requesting temp password for: {email}...");
        var temp = await _api.RequestTemporaryPassword(email);

        Console.WriteLine($"Got temporary password:       {temp!.Uuid}");
        Console.WriteLine();
        Console.Write("Please confirm the password through your email... ");

        while (true)
        {
            Thread.Sleep(5000);

            var token = await _api.GetTemporaryPassword(email, temp.Uuid);
            if (token != null)
            {
                Console.WriteLine("OK!");
                Console.WriteLine();

                TokenFile.Write(token);

                break;
            }
        }

        await Task.CompletedTask;
    }
}
