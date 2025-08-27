using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArcTrigger.Domain.IServices
{
   public interface IMarketDataService :IAsyncDisposable
    {
     
        /// Temel ayarlar.
  
        Uri BaseUri { get; set; }
        Uri WebSocketUri { get; set; }

       
        /// HTTP ısınma ve anlık görüntü (snapshot) isteği yapar.
        /// Auth durumunu tetikler ve belirtilen conid/field parametreleriyle snapshot çeker.
      
        Task WarmupAndSnapshotAsync(string[] conids, string[] fields, CancellationToken cancellationToken);


        /// WebSocket bağlantısını açar, heartbeat gönderir ve verilen enstrümanlara abone olur.
   
        Task StartStreamingAsync(string[] conids, string[] fields, CancellationToken cancellationToken);

  
        /// WS üzerinden gelen ham metin mesajları yayını.
     
        event EventHandler<string>? MessageReceived;

   
        /// Bağlantı açıldığında tetiklenir.
       
        event EventHandler? Connected;

        
        /// Bağlantı kapandığında tetiklenir.
      
        event EventHandler? Disconnected;
    }
}
