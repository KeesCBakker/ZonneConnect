using Microsoft.Extensions.Options;
using System.CommandLine;
using ZonneConnect.PvOutput;
using ZonneConnect.Zonneplan;
using ZonneConnect.Zonneplan.Models;

namespace ZonneConnect.CommandLine;

abstract class ApiCommand : Command
{
    protected ZonneplanApi Api { get; }

    public ApiCommand(
        string name,
        string description,
        ZonneplanApi api) : base(name, description)
    {
        Api = api ?? throw new ArgumentNullException(nameof(api));
    }

    protected async Task<Token> GetToken()
    {
        var token = TokenFile.Read();

        if (token == null)
        {
            ExitWith("No token found, please connect first.", 1);
        }

        var newToken = await Api.RefreshToken(token);
        if (newToken == null)
        {
            ExitWith("Token could not be refreshed.", 2);
        }

        TokenFile.Write(newToken);
        return newToken;
    }

    protected static void ExitWith(string str, int state)
    {
        Console.WriteLine(str);
        Environment.Exit(state);
        throw new Exception(str);
    }
}
