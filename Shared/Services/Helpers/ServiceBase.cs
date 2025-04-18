using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Shared.Helpers;
using Shared.Models.Common;
using Shared.Services.Interfaces;

namespace Shared.Services.Helpers
{
    public abstract class ServiceBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly ISettingsService _settingsService;
        protected readonly JsonSerializerOptions _serializerOptions;

        protected ServiceBase(HttpClient httpClient, ISettingsService settingsService)
        {
            _httpClient = httpClient;
            _settingsService = settingsService;
            
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                WriteIndented = true
            };
            
            // Only set the BaseAddress if it's not already set
            if (_httpClient.BaseAddress == null)
            {
                var baseUrl = _settingsService.ApiBaseUrl;
                _httpClient.BaseAddress = new Uri(baseUrl);
            }
        
            // Set up headers
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
            // Set authorization if token exists
            var token = _settingsService.AuthToken;
            if (!string.IsNullOrEmpty(token))
            {
                // Only set Authorization if it's not already set with the same token
                var existingAuth = _httpClient.DefaultRequestHeaders.Authorization;
                if (existingAuth == null || existingAuth.Parameter != token)
                {
                    _httpClient.DefaultRequestHeaders.Authorization = 
                        new AuthenticationHeaderValue("Bearer", token);
                }
            }
            
            //SetupHttpClient();
        }

        private void SetupHttpClient()
        {
            var baseUrl = _settingsService.ApiBaseUrl;
            _httpClient.BaseAddress = new Uri(baseUrl);
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            
            var token = _settingsService.AuthToken;
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected async Task<T> GetAsync<T>(string endpoint)
        {
            try
            {
                Console.WriteLine("getting upcoming reservations");
                var response = await _httpClient.GetAsync(endpoint);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in GET request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PostAsync<T>(string endpoint, object data)
        {
            Console.WriteLine($"post request to {endpoint}");
            try
            {
                var jsonContent = JsonHelper.Serialize(data);
                Console.WriteLine($"to order api: {jsonContent}");
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine($"to api content: {content}");
                var response = await _httpClient.PostAsync(endpoint, content);
                Console.WriteLine($"final response: {response}");
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in POST request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PutAsync<T>(string endpoint, object data)
        {
            try
            {
                var jsonContent = JsonHelper.Serialize(data);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync(endpoint, content);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in PUT request: {ex.Message}", ex);
            }
        }

        protected async Task<T> DeleteAsync<T>(string endpoint)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(endpoint);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in DELETE request: {ex.Message}", ex);
            }
        }

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Unauthorized access. Please login again.");
                }
                
                ApiResponse<object> errorResponse = null;
                try
                {
                    errorResponse = JsonHelper.Deserialize<ApiResponse<object>>(content);
                }
                catch
                {
                    // If can't deserialize as ApiResponse, throw generic exception
                    throw new HttpRequestException($"HTTP request error: {response.StatusCode} - {content}");
                }
                
                throw new HttpRequestException(errorResponse?.Message ?? "Unknown error occurred");
            }
            
            try
            {
                // Try to deserialize directly to T first
                return JsonHelper.Deserialize<T>(content);
            }
            catch
            {
                // If that fails, try to deserialize as ApiResponse<T>
                var apiResponse = JsonHelper.Deserialize<ApiResponse<T>>(content);
                if (apiResponse != null && apiResponse.Success)
                {
                    return apiResponse.Data;
                }
                
                throw new JsonException($"Failed to deserialize response: {content}");
            }
        }

        protected async Task<byte[]> GetFileAsync(string endpoint)
        {
            try
            {
                var response = await _httpClient.GetAsync(endpoint);
                
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to download file: {response.StatusCode}");
                }
                
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error downloading file: {ex.Message}", ex);
            }
        }

        protected async Task<T> PostFormDataAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PostAsync(endpoint, formData);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in POST form data request: {ex.Message}", ex);
            }
        }

        protected async Task<T> PutFormDataAsync<T>(string endpoint, MultipartFormDataContent formData)
        {
            try
            {
                var response = await _httpClient.PutAsync(endpoint, formData);
                return await HandleResponse<T>(response);
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Error in PUT form data request: {ex.Message}", ex);
            }
        }
    }
}