using DbAccessApplication.Models;
using LoginHandling.Models;
using Microsoft.AspNetCore.Identity;

namespace LoginHandling.Services
{
    public class ApiProxyService
    {
        private readonly ILogger<ApiProxyService> _logger;
        private readonly HttpClient _client;

        public ApiProxyService(ILogger<ApiProxyService> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _client = httpClientFactory.CreateClient("api");
        }

        public async Task<OpenDoorRequest> GetDoorOpenRequestAsync(string deviceGeneratedCode)
        {
            try
            {
                string path = $"api/DoorOpenRequest/deviceGeneratedCode/{deviceGeneratedCode}";

                OpenDoorRequest openDoorRequest = null;
                HttpResponseMessage response = await _client.GetAsync(path);

                response.EnsureSuccessStatusCode();

                openDoorRequest = await response.Content.ReadAsAsync<OpenDoorRequest>();
                return openDoorRequest;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on GetDoorOpenRequestAsync");
                throw;
            }
        }

        public async Task<UserPermissions> GetUserPermissionsAsync(string currentUserId, string deviceId)
        {
            try
            {
                string path = $"api/DoorOpenRequest/user/{currentUserId}/deviceId/{deviceId}";

                UserPermissions userPermissions = null;
                HttpResponseMessage response = await _client.GetAsync(path);

                response.EnsureSuccessStatusCode();

                userPermissions = await response.Content.ReadAsAsync<UserPermissions>();
                return userPermissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on GetUserPermissionsAsync");
                return null;
            }
        }

        public async Task DeleteOpenDoorRequestAsync(int id)
        {
            try
            {
                string path = $"api/DoorOpenRequest/{id}";

                HttpResponseMessage response = await _client.DeleteAsync(path);

                response.EnsureSuccessStatusCode();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on DeleteOpenDoorRequestAsync");
            }
        }

        public async Task UpdateOpenDoorRequestAsync(int id, OpenDoorRequest content)
        {
            try
            {
                string path = $"api/DoorOpenRequest/{id}";

                HttpResponseMessage response = await _client.PutAsJsonAsync(path, content);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on UpdateOpenDoorRequestAsync");
            }
        }

        public async Task<IEnumerable<AccessExtended>> GetAccessesAsync()
        {
            try
            {
                string path = $"api/DoorOpenRequest/accesses";

                IEnumerable<AccessExtended> accesses = null;
                HttpResponseMessage response = await _client.GetAsync(path);

                response.EnsureSuccessStatusCode();

                accesses = await response.Content.ReadAsAsync<IEnumerable<AccessExtended>>();
                return accesses;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on GetAccessesAsync");
                return null;
            }
        }


    }
}
