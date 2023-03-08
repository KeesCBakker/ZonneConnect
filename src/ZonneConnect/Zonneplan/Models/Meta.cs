using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class Meta
{
    [JsonProperty("identifier")]
    public string Identifier { get; set; } = String.Empty;

    [JsonProperty("name")]
    public string Name { get; set; } = String.Empty;

    [JsonProperty("panel_count")]
    public int PanelCount { get; set; }

    //[JsonProperty("panel_type")]
    //public object PanelType { get; set; }

    [JsonProperty("installation_wp")]
    public int InstallationWp { get; set; }

    [JsonProperty("panel_wp")]
    public int PanelWp { get; set; }

    [JsonProperty("first_measured_at")]
    public DateTime FirstMeasuredAt { get; set; }

    [JsonProperty("last_measured_at")]
    public DateTime LastMeasuredAt { get; set; }

    [JsonProperty("last_measured_power_value")]
    public int LastMeasuredPowerValue { get; set; }

    [JsonProperty("total_power_measured")]
    public int TotalPowerMeasured { get; set; }

    [JsonProperty("sgn_serial_number")]
    public string SgnSerialNumber { get; set; } = String.Empty;

    [JsonProperty("module_firmware_version")]
    public string ModuleFirmwareVersion { get; set; } = String.Empty;

    [JsonProperty("inverter_firmware_version")]
    public string InverterFirmwareVersion { get; set; } = String.Empty;

    [JsonProperty("show_in_contract_screen")]
    public bool ShowInContractScreen { get; set; }

    [JsonProperty("enable_pv_analysis")]
    public bool EnablePvAnalysis { get; set; }
}

