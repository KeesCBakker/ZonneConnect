using Microsoft.Extensions.Options;

namespace ZonneConnect.PvOutput;

public class PvOutputClient
{
    private readonly HttpClient _client;
    public PvOutputSettings Settings { get; }

    public PvOutputClient(HttpClient client, IOptions<PvOutputSettings> settings)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        Settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
    }

    public async Task Log(DateTime timestamp, int powerGenerationInWatts, int energyGenerationInWattHours)
    {
        var url = $"https://pvoutput.org/service/r2/addstatus.jsp?key={Settings.ApiKey}&sid={Settings.SystemId}&d={timestamp:yyyyMMdd}&t={timestamp:HH:mm}&v1={energyGenerationInWattHours}&v2={powerGenerationInWatts}";

        using var response = await _client.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var responseContent = await response.Content.ReadAsStringAsync();
    }
}
