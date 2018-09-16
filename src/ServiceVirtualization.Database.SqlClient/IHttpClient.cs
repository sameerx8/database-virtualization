using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceVirtualization.Database.SqlClient {
    public interface IHttpClient
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken);
    }
}