using Newtonsoft.Json;
using ZonneConnect.Zonneplan.Models;

namespace ZonneConnect.Zonneplan;

public static class TokenFile
{
    public static Token? Read(string file = "./data/token.json")
    {
        if (!File.Exists(file))
        {
            return null;
        }

        var content = File.ReadAllText(file);
        return JsonConvert.DeserializeObject<Token>(content);
    }

    public static void Write(Token token, string file = "./data/token.json")
    {
        if(!String.IsNullOrEmpty(token.AccessToken))
        {
            return;
        }

        var content = JsonConvert.SerializeObject(token);
        File.WriteAllText(file, content);
    }
}

