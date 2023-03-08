using Newtonsoft.Json;

namespace ZonneConnect.Zonneplan.Models;

public class ApiResponse
{
    [JsonProperty("error")]
    public string Error { get; set; } = String.Empty;

    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; } = string.Empty;

    [JsonProperty("hint")]
    public string Hint { get; set; } = string.Empty;

    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    public void ThrowException()
    {
        if (!String.IsNullOrEmpty(Error) || !String.IsNullOrEmpty(Message))
        {
            throw new ZonneplanApiException(this);
        }
    }
}

public class ApiResponse<T> : ApiResponse
{
    [JsonProperty("data")]
    public T? Data { get; set; }

    
}

public class ZonneplanApiException :Exception
{

    public ZonneplanApiException(ApiResponse error) : base(error.Message)
    {
        this.Error = error;
    }

    public ApiResponse Error { get; }
}
