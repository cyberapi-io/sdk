using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using CyberAPI.Models;

namespace CyberAPI
{
    /// <summary>
    /// CyberAPI .NET Client
    /// Official .NET client for the CyberAPI Threat Intelligence API
    /// </summary>
    public class CyberAPIClient : IDisposable
    {
        private const string DefaultBaseUrl = "https://threats.cyberapi.io/api/v1";
        private const string UserAgent = "CyberAPI-DotNet/1.0.0";
        
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly string _baseUrl;
        
        /// <summary>
        /// Initialize a new CyberAPI client
        /// </summary>
        /// <param name="apiKey">Your CyberAPI key (get one at https://threats.cyberapi.io)</param>
        /// <param name="baseUrl">API base URL (default: production)</param>
        /// <param name="timeout">Request timeout in seconds (default: 10)</param>
        public CyberAPIClient(string apiKey, string baseUrl = DefaultBaseUrl, int timeout = 10)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key is required", nameof(apiKey));
            
            _apiKey = apiKey;
            _baseUrl = baseUrl.TrimEnd('/');
            
            _httpClient = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(timeout)
            };
            
            _httpClient.DefaultRequestHeaders.Add("X-API-Key", _apiKey);
            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
        
        /// <summary>
        /// Check a domain or IP for threats
        /// </summary>
        /// <param name="domain">Domain name to check (e.g., "example.com")</param>
        /// <param name="ip">IP address to check (e.g., "1.2.3.4")</param>
        /// <returns>Threat intelligence report</returns>
        /// <exception cref="ArgumentException">If neither domain nor ip is provided</exception>
        /// <exception cref="CyberAPIException">If API request fails</exception>
        public async Task<ThreatResponse> CheckAsync(string domain = null, string ip = null)
        {
            if (string.IsNullOrWhiteSpace(domain) && string.IsNullOrWhiteSpace(ip))
                throw new ArgumentException("Either domain or ip must be provided");
            
            var queryParams = new List<string>();
            if (!string.IsNullOrWhiteSpace(domain))
                queryParams.Add($"domain={Uri.EscapeDataString(domain)}");
            if (!string.IsNullOrWhiteSpace(ip))
                queryParams.Add($"ip={Uri.EscapeDataString(ip)}");
            
            var url = $"{_baseUrl}/check?{string.Join("&", queryParams)}";
            
            try
            {
                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorData = TryParseJson(content);
                    var message = errorData?.GetProperty("detail").GetString() ?? $"HTTP {(int)response.StatusCode}";
                    
                    throw new CyberAPIException(message, (int)response.StatusCode);
                }
                
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                return JsonSerializer.Deserialize<ThreatResponse>(content, options);
            }
            catch (HttpRequestException ex)
            {
                throw new CyberAPIException($"Network error: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new CyberAPIException("Request timeout", ex);
            }
            catch (JsonException ex)
            {
                throw new CyberAPIException($"Invalid JSON response: {ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// Quick check if domain/IP is malicious
        /// </summary>
        /// <param name="domain">Domain name to check</param>
        /// <param name="ip">IP address to check</param>
        /// <returns>True if risk_score >= 80</returns>
        public async Task<bool> IsMaliciousAsync(string domain = null, string ip = null)
        {
            var result = await CheckAsync(domain, ip);
            return result.RiskScore >= 80;
        }
        
        /// <summary>
        /// Get list of threat signals detected
        /// </summary>
        /// <param name="domain">Domain name to check</param>
        /// <param name="ip">IP address to check</param>
        /// <returns>List of signal strings</returns>
        public async Task<List<string>> GetSignalsAsync(string domain = null, string ip = null)
        {
            var result = await CheckAsync(domain, ip);
            return result.Signals ?? new List<string>();
        }
        
        private JsonDocument TryParseJson(string content)
        {
            try
            {
                return JsonDocument.Parse(content);
            }
            catch
            {
                return null;
            }
        }
        
        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
    
    /// <summary>
    /// Exception thrown by CyberAPI client
    /// </summary>
    public class CyberAPIException : Exception
    {
        public int? StatusCode { get; }
        
        public CyberAPIException(string message) : base(message) { }
        
        public CyberAPIException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        
        public CyberAPIException(string message, Exception innerException) : base(message, innerException) { }
    }
}
