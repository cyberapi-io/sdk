require 'net/http'
require 'json'
require 'uri'

module CyberAPI
  # CyberAPI Ruby Client
  # 
  # Official Ruby client for the CyberAPI Threat Intelligence API
  #
  # @example Basic usage
  #   client = CyberAPI::Client.new(api_key: "your_api_key_here")
  #   result = client.check(domain: "example.com")
  #   puts result["risk_score"]
  class Client
    BASE_URL = "https://threats.cyberapi.io/api/v1"
    USER_AGENT = "CyberAPI-Ruby/1.0.0"
    
    attr_reader :api_key, :base_url, :timeout
    
    # Initialize a new CyberAPI client
    #
    # @param api_key [String] Your CyberAPI key (get one at https://threats.cyberapi.io)
    # @param base_url [String] API base URL (default: production)
    # @param timeout [Integer] Request timeout in seconds (default: 10)
    def initialize(api_key:, base_url: BASE_URL, timeout: 10)
      raise ArgumentError, "API key is required" if api_key.nil? || api_key.empty?
      
      @api_key = api_key
      @base_url = base_url.chomp('/')
      @timeout = timeout
    end
    
    # Check a domain or IP for threats
    #
    # @param domain [String, nil] Domain name to check (e.g., "example.com")
    # @param ip [String, nil] IP address to check (e.g., "1.2.3.4")
    # @return [Hash] Threat intelligence report
    # @raise [ArgumentError] If neither domain nor ip is provided
    # @raise [CyberAPI::Error] If API request fails
    #
    # @example Check a domain
    #   result = client.check(domain: "example.com")
    #   puts result["verdict"]
    #
    # @example Check an IP
    #   result = client.check(ip: "1.2.3.4")
    #   puts result["geo_location"]
    def check(domain: nil, ip: nil)
      raise ArgumentError, "Either domain or ip must be provided" if domain.nil? && ip.nil?
      
      params = {}
      params[:domain] = domain if domain
      params[:ip] = ip if ip
      
      uri = URI("#{@base_url}/check")
      uri.query = URI.encode_www_form(params)
      
      request = Net::HTTP::Get.new(uri)
      request['X-API-Key'] = @api_key
      request['Accept'] = 'application/json'
      request['User-Agent'] = USER_AGENT
      
      response = Net::HTTP.start(uri.hostname, uri.port, use_ssl: uri.scheme == 'https', read_timeout: @timeout) do |http|
        http.request(request)
      end
      
      handle_response(response)
    end
    
    # Quick check if domain/IP is malicious
    #
    # @param domain [String, nil] Domain name to check
    # @param ip [String, nil] IP address to check
    # @return [Boolean] True if risk_score >= 80
    #
    # @example
    #   if client.malicious?(domain: "suspicious-site.com")
    #     puts "Warning: Malicious domain detected!"
    #   end
    def malicious?(domain: nil, ip: nil)
      result = check(domain: domain, ip: ip)
      result['risk_score'].to_i >= 80
    end
    
    # Get list of threat signals detected
    #
    # @param domain [String, nil] Domain name to check
    # @param ip [String, nil] IP address to check
    # @return [Array<String>] List of signal strings
    #
    # @example
    #   signals = client.signals(domain: "example.com")
    #   puts "Detected signals: #{signals.join(', ')}"
    def signals(domain: nil, ip: nil)
      result = check(domain: domain, ip: ip)
      result['signals'] || []
    end
    
    private
    
    def handle_response(response)
      case response.code.to_i
      when 200..299
        JSON.parse(response.body)
      when 400..499
        error_data = JSON.parse(response.body) rescue {}
        message = error_data['detail'] || "HTTP #{response.code}"
        raise CyberAPI::ClientError.new(message, response.code.to_i)
      when 500..599
        error_data = JSON.parse(response.body) rescue {}
        message = error_data['detail'] || "HTTP #{response.code}"
        raise CyberAPI::ServerError.new(message, response.code.to_i)
      else
        raise CyberAPI::Error.new("Unexpected HTTP status: #{response.code}", response.code.to_i)
      end
    rescue JSON::ParserError => e
      raise CyberAPI::Error.new("Invalid JSON response: #{e.message}")
    end
  end
  
  # Base error class for CyberAPI
  class Error < StandardError
    attr_reader :status_code
    
    def initialize(message, status_code = nil)
      super(message)
      @status_code = status_code
    end
  end
  
  # Client error (4xx responses)
  class ClientError < Error; end
  
  # Server error (5xx responses)
  class ServerError < Error; end
end
