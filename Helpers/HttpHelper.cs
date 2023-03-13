using System.Net.Http.Headers;
using System.Text.Json;

namespace PostCode.Helpers;

public static class HttpHelper
{
    public static async Task<T?> Get<T>(string url, string referer)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("Referer", referer);
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseStream = await response.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<T>(responseStream);
            }

            return default;
        }
    }
}