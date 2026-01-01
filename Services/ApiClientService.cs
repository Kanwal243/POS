using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;
using EyeHospitalPOS.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EyeHospitalPOS.Services
{
    /// <summary>
    /// Base API client service that handles JWT authentication and HTTP requests
    /// Uses IHttpContextAccessor for accessing HTTP context information
    /// </summary>
    public class ApiClientService
    {
        protected readonly HttpClient _httpClient;
        protected readonly IJSRuntime _jsRuntime;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Helper.LoginManager _loginManager;
        protected readonly JsonSerializerOptions _jsonOptions;

        public ApiClientService(HttpClient httpClient, IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor, Helper.LoginManager loginManager)
        {
            _httpClient = httpClient;
            _jsRuntime = jsRuntime;
            _httpContextAccessor = httpContextAccessor;
            _loginManager = loginManager;
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };
        }

        /// <summary>
        /// Sets the JWT authorization header from sessionStorage, LoginManager, or HTTP context
        /// </summary>
        protected async Task SetAuthHeaderAsync()
        {
            string? token = null;
            
            // Try to get token from sessionStorage (client-side)
            try
            {
                token = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "authToken");
            }
            catch
            {
                // JS interop not available (server-side), try other sources
            }
            
            // If not found in sessionStorage, try to get from LoginManager
            token ??= _loginManager.AuthToken;
            
            // If still not found, try to get from HTTP context (server-side)
            if (_httpContextAccessor.HttpContext?.Request.Headers.TryGetValue("Authorization", out var authHeader) == true)
            {
                token ??= authHeader.ToString().Replace("Bearer ", "");
            }
            
            // Remove existing authorization header
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            
            // Set authorization header if token exists
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            
            // Add correlation ID for request tracking
            var correlationId = _httpContextAccessor.HttpContext?.Request.Headers["X-Correlation-Id"].ToString() 
                                ?? _httpContextAccessor.HttpContext?.TraceIdentifier;
                                
            if (!string.IsNullOrEmpty(correlationId))
            {
                _httpClient.DefaultRequestHeaders.Remove("X-Correlation-Id");
                _httpClient.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            }
        }

        /// <summary>
        /// Makes a GET request to the API
        /// </summary>
        protected async Task<T?> GetAsync<T>(string endpoint)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
        }

        /// <summary>
        /// Makes a POST request to the API
        /// </summary>
        protected async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync(endpoint, data, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        /// <summary>
        /// Makes a PUT request to the API
        /// </summary>
        protected async Task<TResponse?> PutAsync<TRequest, TResponse>(string endpoint, TRequest data)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PutAsJsonAsync(endpoint, data, _jsonOptions);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<TResponse>(_jsonOptions);
        }

        /// <summary>
        /// Makes a DELETE request to the API
        /// </summary>
        protected async Task DeleteAsync(string endpoint)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.DeleteAsync(endpoint);
            response.EnsureSuccessStatusCode();
        }
    }
}

