using System.CommandLine;
using System.Runtime;
using TimeZoneConverter;
using ZonneConnect.PvOutput;
using ZonneConnect.Zonneplan;

namespace ZonneConnect.CommandLine;

class CurrentCommand : ApiCommand
{
    private readonly PvOutputClient _pvOutputClient;

    public CurrentCommand(ZonneplanApi api, PvOutputClient pvOutputClient) : base("current", "Gets the current status of the solar panel.", api)
    {
        _pvOutputClient = pvOutputClient ?? throw new ArgumentNullException(nameof(pvOutputClient));

        var uuid = new Option<string>("--uuid", "The identifier.");
        var poll = new Option<bool>("--poll", ()=>false, "Will keep polling every 5 minutes.");

        AddOption(uuid);
        AddOption(poll);
        this.SetHandler(Execute, uuid, poll);
    }

    private async Task Execute(string? uuid, bool poll)
    {
        Header.WriteHeader();

        if (String.IsNullOrEmpty(_pvOutputClient.Settings.ApiKey))
        {
            ExitWith("Please set environment setting PvOutput__ApiKey in the .env file.", 3);
        }

        if (String.IsNullOrEmpty(_pvOutputClient.Settings.SystemId))
        {
            ExitWith("Please set environment setting PvOutput__SystemId in the .env file.", 3);
        }

        if (String.IsNullOrEmpty(uuid))
        {
            var token = await GetToken();
            var me = await Api.Me(token);
            uuid = me!.AddressGroups.First().Uuid;
        }

        DateTime lastProcessed = DateTime.MinValue;

        do
        {
            var token = await GetToken();
            var data = await Api.Current(token, uuid);
            var last = lastProcessed;
            var watt = 0;

            if (data.Length > 0 && data.First().Measurements.Count > 0)
            {
                last = Convert(data.First().Measurements.Last().MeasuredAt);
                watt = data.First().Measurements.Last().Value;
                var wattHours = data.First().Total;

                if (last > lastProcessed)
                {
                    Console.WriteLine();
                    Console.WriteLine("TimeStamp:        " + last);
                    Console.WriteLine("Watt:             " + watt);
                    Console.WriteLine("Total Watt Today: " + wattHours);

                    lastProcessed = last;

                    await _pvOutputClient.Log(last, watt, wattHours);
                }
            }
            else
            {
                Console.WriteLine("No measurements.");
            }

            if (poll)
            {
                var togo = last.AddMinutes(5.5) - DateTime.Now;

                if (togo.TotalMilliseconds < 5000)
                {
                    if (watt == 0)
                    {
                        togo = TimeSpan.FromMinutes(5);
                    }
                    else
                    {
                        togo = TimeSpan.FromSeconds(30);
                    }
                }


                Console.WriteLine("Sleeping for:     " + ((int)togo.TotalSeconds) + "s");
                Thread.Sleep((int)togo.TotalMilliseconds);
            }
        }
        while (poll);

        Console.WriteLine();

        await Task.CompletedTask;
    }

    private DateTime Convert(DateTime measuredAt)
    {
        var tzVar = Environment.GetEnvironmentVariable("TZ");
        var tz = !String.IsNullOrWhiteSpace(tzVar) ?
            TZConvert.GetTimeZoneInfo(tzVar) :
            TimeZoneInfo.Local;


        var  utcDateTime = new DateTimeOffset(measuredAt, TimeSpan.Zero);
        return TimeZoneInfo.ConvertTime(utcDateTime, tz).LocalDateTime;
    }
}
