using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoogleApi_Reto.DataStore
{
    public class SessionDataStore : IDataStore
    {
        private readonly ISession _session;
        public SessionDataStore(HttpContext context)
        {
            _session = context.Session;
        }

        public Task ClearAsync()
        {
            _session.Clear();
            return Task.CompletedTask;
        }

        public Task DeleteAsync<T>(string key)
        {
            _session.Remove(GetKey<T>(key));
            return Task.CompletedTask;
        }

        public Task<T> GetAsync<T>(string key)
        {
            var data = _session.GetString(GetKey<T>(key));
            if (data == null)
                return Task.FromResult(default(T));
            return Task.FromResult(JsonSerializer.Deserialize<T>(data));
        }

        public Task StoreAsync<T>(string key, T value)
        {
            var data = JsonSerializer.Serialize(value);
            _session.SetString(GetKey<T>(key), data);
            return Task.CompletedTask;
        }

        private string GetKey<T>(string key)
        {
            return $"{typeof(T).FullName}:{key}";
        }
    }
}
