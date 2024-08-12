using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DominionWars.Common.Logger;

public struct RemoteLogData
{
    public string Msg;
    public DateTime Date;
    public string Scope;
    public LogPumpType Source;
}

public struct GetData
{
    public string ServerUrl;
    public int Heartbeat;
    public int? Timeout;
    public CookieCollection Cookies;
}

public class RemotePumpNode : PumpNode
{
    private readonly HttpClient _client;
    private readonly GetData _getData;


    public readonly bool AutoPump;
    private string _dataCache = "";

    public RemotePumpNode(GetData getData, bool autoPump = true)
    {
        _getData = getData;
        AutoPump = autoPump;
        HttpClientHandler handler = new HttpClientHandler();
        if (_getData.Cookies != null)
        {
            handler.CookieContainer = new CookieContainer();
            handler.CookieContainer.Add(_getData.Cookies);
        }

        _client = new HttpClient(handler);
        if (_getData.Timeout != null)
        {
            _client.Timeout = new TimeSpan(_getData.Timeout.Value);
        }

        if (AutoPump)
        {
            Start();
        }
    }

    public void Start() =>
        Task.Run(async () =>
        {
            for (;;)
            {
                try
                {
                    await Pumping();
                }
                catch (Exception e)
                {
                    Logger.Error($"RemotePump request error\n{e}", "Logger");
                }
            }
        });

    private async Task Pumping()
    {
        string reqData = "";
        using (HttpResponseMessage response = await _client.GetAsync(_getData.ServerUrl))
        {
            reqData = await response.Content.ReadAsStringAsync();
        }

        if (_dataCache != reqData || _dataCache != "")
        {
            try
            {
                LogData data = JsonSerializer.Deserialize<LogData>(reqData);
                On(data with { Source = data.Source ?? LogPumpType.Remote, Value = data.Msg });
            }
            catch (Exception e)
            {
                Logger.Error($"RemotePump data unpack error(${e.Message})", "Logger");
            }

            _dataCache = reqData;
        }
    }
}
