Gem::Specification.new do |spec|
  spec.name          = "cyberapi"
  spec.version       = "1.0.0"
  spec.authors       = ["CyberAPI"]
  spec.email         = ["inbox@cyberapi.io"]

  spec.summary       = "Official Ruby client for CyberAPI Threat Intelligence API"
  spec.description   = "Ruby SDK for CyberAPI - Check domains and IPs for security threats, malware, phishing, and more."
  spec.homepage      = "https://github.com/cyberapi-io/sdk"
  spec.license       = "MIT"
  spec.required_ruby_version = ">= 2.5.0"

  spec.metadata["homepage_uri"] = spec.homepage
  spec.metadata["source_code_uri"] = "https://github.com/cyberapi-io/sdk/tree/main/ruby"
  spec.metadata["documentation_uri"] = "https://threats.cyberapi.io/docs"

  spec.files = Dir["lib/**/*", "README.md", "LICENSE"]
  spec.require_paths = ["lib"]

  # No external dependencies - uses Ruby standard library only
end
