using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CyberAPI.Models
{
    /// <summary>
    /// Threat intelligence response from CyberAPI
    /// </summary>
    public class ThreatResponse
    {
        [JsonPropertyName("risk_score")]
        public int RiskScore { get; set; }
        
        [JsonPropertyName("verdict")]
        public string Verdict { get; set; }
        
        [JsonPropertyName("domain")]
        public string Domain { get; set; }
        
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        
        [JsonPropertyName("signals")]
        public List<string> Signals { get; set; }
        
        [JsonPropertyName("geo_location")]
        public GeoLocation GeoLocation { get; set; }
        
        [JsonPropertyName("dns_security")]
        public Dictionary<string, object> DnsSecurity { get; set; }
        
        [JsonPropertyName("ssl_info")]
        public Dictionary<string, object> SslInfo { get; set; }
        
        [JsonPropertyName("tech_stack")]
        public Dictionary<string, object> TechStack { get; set; }
        
        [JsonPropertyName("security_headers")]
        public Dictionary<string, object> SecurityHeaders { get; set; }
        
        [JsonPropertyName("cached")]
        public bool Cached { get; set; }
        
        [JsonPropertyName("process_time_ms")]
        public int ProcessTimeMs { get; set; }
    }
    
    /// <summary>
    /// Geolocation information for an IP address
    /// </summary>
    public class GeoLocation
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; }
        
        [JsonPropertyName("country")]
        public string Country { get; set; }
        
        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [JsonPropertyName("isp")]
        public string Isp { get; set; }
        
        [JsonPropertyName("is_hosting")]
        public bool IsHosting { get; set; }
        
        [JsonPropertyName("risk_score")]
        public int RiskScore { get; set; }
    }
}
