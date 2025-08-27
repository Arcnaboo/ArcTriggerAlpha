using ArcTrigger.Domain.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ArcTrigger.Domain.Services
{
    public class MarketDataService : IMarketDataService
    {
        private readonly CookieContainer _cookies = new();
        private ClientWebSocket? _webSocket;
        private Timer? _heartbeatTimer;

        public Uri BaseUri { get; set; } = new Uri("https://192.168.1.112:5000");
        public Uri WebSocketUri { get; set; } = new Uri("wss://192.168.1.112:5000/v1/api/ws");

        public event EventHandler<string>? MessageReceived;
        public event EventHandler? Connected;
        public event EventHandler? Disconnected;

        private HttpClient CreateHttpClient()
        {
            var handler = new HttpClientHandler
            {
                CookieContainer = _cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            };

            var http = new HttpClient(handler)
            {
                BaseAddress = BaseUri
            };
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
            http.DefaultRequestHeaders.Add("Accept", "application/json");
            http.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
            return http;
        }

        public async Task WarmupAndSnapshotAsync(string[] conids, string[] fields, CancellationToken cancellationToken)
        {
            using var http = CreateHttpClient();

            var status = await http.GetAsync("/v1/api/iserver/auth/status", cancellationToken);

            var conidsCsv = string.Join(",", conids);
            var fieldsCsv = string.Join(",", fields);
            var snapUrl = $"/v1/api/iserver/marketdata/snapshot?conids={conidsCsv}&fields={fieldsCsv}";
            await http.GetAsync(snapUrl, cancellationToken);
        }

        public async Task StartStreamingAsync(string[] conids, string[] fields, CancellationToken cancellationToken)
        {
            var socketsHandler = new SocketsHttpHandler
            {
                CookieContainer = _cookies,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                SslOptions = new System.Net.Security.SslClientAuthenticationOptions
                {
                    RemoteCertificateValidationCallback = (_, __, ___, ____) => true
                }
            };
            using var invoker = new HttpMessageInvoker(socketsHandler);

            _webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(20);
            _webSocket.Options.SetRequestHeader("User-Agent", "Mozilla/5.0");
            _webSocket.Options.SetRequestHeader("X-Requested-With", "XMLHttpRequest");
            _webSocket.Options.SetRequestHeader("Origin", "https://192.168.1.112:5000");

            await _webSocket.ConnectAsync(WebSocketUri, invoker, cancellationToken);
            Connected?.Invoke(this, EventArgs.Empty);

            _heartbeatTimer = new Timer(async _ =>
            {
                try
                {
                    if (_webSocket?.State == WebSocketState.Open)
                    {
                        var hbBytes = Encoding.UTF8.GetBytes("ech+hb");
                        await _webSocket.SendAsync(new ArraySegment<byte>(hbBytes), WebSocketMessageType.Text, true, CancellationToken.None);
                    }
                }
                catch { }
            }, null, 5000, 5000);

            await Task.Delay(1500, cancellationToken);
            var fieldsJson = JsonSerializer.Serialize(new { fields });
            foreach (var conid in conids)
            {
                var subMsg = $"smd+{conid}+{fieldsJson}";
                var data = Encoding.UTF8.GetBytes(subMsg);
                await _webSocket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, cancellationToken);
            }

            _ = ReceiveLoopAsync(cancellationToken);
        }

        private async Task ReceiveLoopAsync(CancellationToken cancellationToken)
        {
            if (_webSocket == null)
                return;

            var buffer = new byte[64 * 1024];
            try
            {
                while (!cancellationToken.IsCancellationRequested && _webSocket.State == WebSocketState.Open)
                {
                    var result = await _webSocket.ReceiveAsync(buffer, cancellationToken);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        try
                        {
                            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "bye", cancellationToken);
                        }
                        catch { }
                        Disconnected?.Invoke(this, EventArgs.Empty);
                        break;
                    }

                    var text = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    MessageReceived?.Invoke(this, text);
                }
            }
            catch (WebSocketException)
            {
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async ValueTask DisposeAsync()
        {
            try { _heartbeatTimer?.Dispose(); } catch { }
            if (_webSocket != null)
            {
                try
                {
                    if (_webSocket.State == WebSocketState.Open)
                    {
                        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "dispose", CancellationToken.None);
                    }
                }
                catch { }
                finally
                {
                    _webSocket.Dispose();
                }
            }
        }
    }
}
