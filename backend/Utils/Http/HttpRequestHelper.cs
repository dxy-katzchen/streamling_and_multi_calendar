using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Streamling.Model;
using Streamling.Utils;

namespace API.Utils.Http
{
    public static class HttpRequestHelper
    {
        private static readonly HttpClient _httpClient = new HttpClient();


        public static async Task<JsonObject> SendRequestAsync<T>(HttpMethod httpMethod, RequestObj requestObj, string apiUrl, T requestBody = default)
        {
            var jsonContent = requestBody != null
                ? new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json")
                : new StringContent(string.Empty, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(httpMethod, $"{requestObj.BaseURL}{apiUrl}")
            {
                Content = jsonContent
            };

            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.Add($"{requestObj.UserCredential.Key}", $"{requestObj.UserCredential.Value}");

            if (requestBody != null)
            { request.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json"); }

            var response = await _httpClient.SendAsync(request);

            var responseBody = await response.Content.ReadAsStringAsync();

            // Check for successful response
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    throw new HttpRequestException($"Rate limit exceeded (429). Response: {responseBody}", null, HttpStatusCode.TooManyRequests);
                }
                throw new HttpRequestException($"Request failed with status {response.StatusCode}. Response: {responseBody}", null, response.StatusCode);
            }

            // Validate that the response is valid JSON before parsing
            if (string.IsNullOrWhiteSpace(responseBody) ||
                (responseBody.StartsWith("<") && responseBody.Contains("html")))
            {
                throw new InvalidOperationException($"Response is not valid JSON: {responseBody.Substring(0, Math.Min(100, responseBody.Length))}...");
            }

            var jsonObject = ProcessStringToJsonObject(responseBody);
            return jsonObject;
        }

        private static JsonObject ProcessStringToJsonObject(string jsonString)
        {
            try
            {
                JsonObject? jsonObject = JsonNode.Parse(jsonString) as JsonObject;
                if (jsonObject == null)
                {
                    throw new InvalidOperationException("Invalid JSON format.");
                }
                return jsonObject;
            }
            catch (JsonException ex)
            {
                throw new InvalidOperationException("Failed to parse JSON string.", ex);
            }
        }
        public static async Task<JsonObject> SendRequestWithRetryAsync<T>(HttpMethod httpMethod, RequestObj requestObj, string apiUrl, int maxRetries = 5, int delayMilliseconds = 1000)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await HttpRequestHelper.SendRequestAsync<T>(httpMethod, requestObj, apiUrl);
                }
                catch (HttpRequestException ex) when (ex.StatusCode == (HttpStatusCode)429)
                {
                    retryCount++;
                    if (retryCount > maxRetries)
                    {
                        throw;
                    }
                    await Task.Delay(delayMilliseconds * retryCount); // Exponential backoff
                }
            }
        }
    }
}