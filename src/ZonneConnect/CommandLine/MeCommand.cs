using System.CommandLine;
using System.Reflection;
using ZonneConnect.Zonneplan;
using ZonneConnect.Zonneplan.Models;

namespace ZonneConnect.CommandLine;

class MeCommand : ApiCommand
{
    public MeCommand(ZonneplanApi api) : base("me", "Returns debug information about who's logged on.", api)
    {
        this.SetHandler(Execute);
    }

    private async Task Execute()
    {
        Header.WriteHeader();
        var token = await GetToken();
        
        var data = await Api.Me(token);
        data?.AddressGroups.ForEach(a =>
        {
            a.Connections.ForEach(c =>
            {
                c.Contracts.ForEach(co =>
                {
                    var padding = 25;
                    Console.WriteLine("UUID:".PadRight(padding) + c.Uuid);
                    co.Meta.GetType().GetProperties(BindingFlags.Public | BindingFlags.GetField | BindingFlags.Instance).OrderBy(x => x.Name).ToList().ForEach(p =>
                    {
                        var name = p.Name + ":";
                        Console.Write(name.PadRight(padding));
                        Console.WriteLine(p.GetValue(co.Meta));
                    });
                    Console.WriteLine();
                });
            });
        });

        await Task.CompletedTask;
    }
}
