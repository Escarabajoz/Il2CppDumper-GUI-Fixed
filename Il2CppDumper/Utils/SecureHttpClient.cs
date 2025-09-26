using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Il2CppDumper.Utils
{
    public static class SecureHttpClient
    {
        private static readonly HttpClient _httpClient;
        private static readonly Regex _versionRegex = new Regex(@"^[\d\.]+$", RegexOptions.Compiled);
        
        static SecureHttpClient()
        {
            var handler = new HttpClientHandler()
            {
                // Enable certificate validation
                ServerCertificateCustomValidationCallback = null // Use default validation
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(10), // 10 second timeout
                DefaultRequestHeaders = 
                {
                    {"User-Agent", "Il2CppDumper/1.0"}
                }
            };
        }

        public static async Task<string> GetVersionSafelyAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                throw new ArgumentException("URL cannot be null or empty", nameof(url));

            // Validate URL format
            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri) || 
                (uri.Scheme != "https" && uri.Scheme != "http"))
            {
                throw new ArgumentException("Invalid URL format or non-HTTP(S) scheme", nameof(url));
            }

            // Only allow HTTPS for security
            if (uri.Scheme != "https")
            {
                throw new ArgumentException("Only HTTPS URLs are allowed", nameof(url));
            }

            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                
                var response = await _httpClient.GetAsync(url, cts.Token);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                
                // Validate response content
                if (string.IsNullOrWhiteSpace(content))
                {
                    throw new InvalidOperationException("Empty response received");
                }

                // Trim and validate version format
                content = content.Trim();
                if (content.Length > 20) // Reasonable version string length
                {
                    throw new InvalidOperationException("Version string too long");
                }

                // Validate version format (only digits and dots)
                if (!_versionRegex.IsMatch(content))
                {
                    throw new InvalidOperationException("Invalid version format");
                }

                return content;
            }
            catch (HttpRequestException ex)
            {
                throw new InvalidOperationException($"Network error: {ex.Message}", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new InvalidOperationException("Request timed out", ex);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error: {ex.Message}", ex);
            }
        }

        public static void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}