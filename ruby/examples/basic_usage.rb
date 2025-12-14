#!/usr/bin/env ruby

require_relative '../cyberapi'

# Example: Basic usage of CyberAPI Ruby SDK

# Get API key from environment or use demo
api_key = ENV['CYBERAPI_KEY'] || 'demo'

# Initialize client
client = CyberAPI::Client.new(api_key: api_key)

puts "🔍 CyberAPI Ruby SDK - Basic Usage Example\n\n"

# Example 1: Check a domain
puts "=" * 50
puts "Example 1: Domain Check"
puts "=" * 50

begin
  result = client.check(domain: "example.com")
  
  puts "Domain: #{result['domain']}"
  puts "Risk Score: #{result['risk_score']}/100"
  puts "Verdict: #{result['verdict']}"
  puts "Cached: #{result['cached'] ? 'Yes' : 'No'}"
  puts "Process Time: #{result['process_time_ms']}ms"
  
  if result['signals'].any?
    puts "\nThreat Signals:"
    result['signals'].each { |signal| puts "  ⚠️  #{signal}" }
  else
    puts "\n✅ No threat signals detected"
  end
rescue CyberAPI::Error => e
  puts "❌ Error: #{e.message}"
end

# Example 2: Quick malicious check
puts "\n" + "=" * 50
puts "Example 2: Quick Malicious Check"
puts "=" * 50

begin
  domain = "example.com"
  is_malicious = client.malicious?(domain: domain)
  
  if is_malicious
    puts "⚠️  #{domain} is MALICIOUS!"
  else
    puts "✅ #{domain} appears safe"
  end
rescue CyberAPI::Error => e
  puts "❌ Error: #{e.message}"
end

# Example 3: Get signals only
puts "\n" + "=" * 50
puts "Example 3: Extract Signals"
puts "=" * 50

begin
  signals = client.signals(domain: "example.com")
  
  puts "Detected #{signals.length} signals:"
  if signals.any?
    signals.each { |signal| puts "  - #{signal}" }
  else
    puts "  (none)"
  end
rescue CyberAPI::Error => e
  puts "❌ Error: #{e.message}"
end

# Example 4: IP check with geolocation
puts "\n" + "=" * 50
puts "Example 4: IP Geolocation"
puts "=" * 50

begin
  result = client.check(ip: "8.8.8.8")
  geo = result['geo_location']
  
  puts "IP: #{geo['ip']}"
  puts "Country: #{geo['country'] || 'Unknown'}"
  puts "City: #{geo['city'] || 'Unknown'}"
  puts "ISP: #{geo['isp'] || 'Unknown'}"
  puts "Risk Score: #{result['risk_score']}/100"
rescue CyberAPI::Error => e
  puts "❌ Error: #{e.message}"
end

puts "\n" + "=" * 50
puts "✨ Examples completed!"
puts "=" * 50
