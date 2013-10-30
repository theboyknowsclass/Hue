using System.Net.Http;
using System.Threading.Tasks;

namespace TheBoyKnowsClass.Hue.Common.Operations
{
    public static class HTTPOperations
    {
        public static async Task<string> GetAsync(HttpClient httpClient, string uri)
        {
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PostAsync(HttpClient httpClient, string uri, string postContent)
        {
            HttpResponseMessage response = await httpClient.PostAsync(uri, new StringContent(postContent));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> PutAsync(HttpClient httpClient, string uri, string postContent)
        {
            HttpResponseMessage response = await httpClient.PutAsync(uri, new StringContent(postContent));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> DeleteAsync(HttpClient httpClient, string uri)
        {
            HttpResponseMessage response = await httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
