using System;
using System.Threading.Tasks;
using CyberAPI;

namespace CyberAPI.Examples
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Get API key from environment or use demo
            var apiKey = Environment.GetEnvironmentVariable("CYBERAPI_KEY") ?? "demo";
            
            Console.WriteLine("🔍 CyberAPI .NET SDK - Basic Usage Examples\n");
            
            using var client = new CyberAPIClient(apiKey);
            
            // Example 1: Domain Check
            await Example1_DomainCheck(client);
            
            // Example 2: Quick Malicious Check
            await Example2_MaliciousCheck(client);
            
            // Example 3: Get Signals
            await Example3_GetSignals(client);
            
            // Example 4: IP Geolocation
            await Example4_IpGeolocation(client);
            
            // Example 5: Error Handling
            await Example5_ErrorHandling(client);
            
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("✨ Examples completed!");
            Console.WriteLine(new string('=', 50));
        }
        
        static async Task Example1_DomainCheck(CyberAPIClient client)
        {
            Console.WriteLine(new string('=', 50));
            Console.WriteLine("Example 1: Domain Check");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                var result = await client.CheckAsync(domain: "example.com");
                
                Console.WriteLine($"Domain: {result.Domain}");
                Console.WriteLine($"Risk Score: {result.RiskScore}/100");
                Console.WriteLine($"Verdict: {result.Verdict}");
                Console.WriteLine($"Cached: {result.Cached}");
                Console.WriteLine($"Process Time: {result.ProcessTimeMs}ms");
                
                if (result.Signals != null && result.Signals.Count > 0)
                {
                    Console.WriteLine("\nThreat Signals:");
                    foreach (var signal in result.Signals)
                    {
                        Console.WriteLine($"  ⚠️  {signal}");
                    }
                }
                else
                {
                    Console.WriteLine("\n✅ No threat signals detected");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        
        static async Task Example2_MaliciousCheck(CyberAPIClient client)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Example 2: Quick Malicious Check");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                var domain = "example.com";
                var isMalicious = await client.IsMaliciousAsync(domain: domain);
                
                if (isMalicious)
                {
                    Console.WriteLine($"⚠️  {domain} is MALICIOUS!");
                }
                else
                {
                    Console.WriteLine($"✅ {domain} appears safe");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        
        static async Task Example3_GetSignals(CyberAPIClient client)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Example 3: Extract Signals");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                var signals = await client.GetSignalsAsync(domain: "example.com");
                
                Console.WriteLine($"Detected {signals.Count} signals:");
                if (signals.Count > 0)
                {
                    foreach (var signal in signals)
                    {
                        Console.WriteLine($"  - {signal}");
                    }
                }
                else
                {
                    Console.WriteLine("  (none)");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        
        static async Task Example4_IpGeolocation(CyberAPIClient client)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Example 4: IP Geolocation");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                var result = await client.CheckAsync(ip: "8.8.8.8");
                var geo = result.GeoLocation;
                
                Console.WriteLine($"IP: {geo?.Ip ?? "N/A"}");
                Console.WriteLine($"Country: {geo?.Country ?? "Unknown"}");
                Console.WriteLine($"City: {geo?.City ?? "Unknown"}");
                Console.WriteLine($"ISP: {geo?.Isp ?? "Unknown"}");
                Console.WriteLine($"Risk Score: {result.RiskScore}/100");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
            }
        }
        
        static async Task Example5_ErrorHandling(CyberAPIClient client)
        {
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("Example 5: Error Handling");
            Console.WriteLine(new string('=', 50));
            
            try
            {
                // This will throw an exception - missing both domain and ip
                await client.CheckAsync();
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"✅ Caught expected error: {ex.Message}");
            }
            catch (CyberAPIException ex)
            {
                Console.WriteLine($"API Error: {ex.Message} (HTTP {ex.StatusCode})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }
    }
}
