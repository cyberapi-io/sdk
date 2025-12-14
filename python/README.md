# CyberAPI Python SDK

Official Python client for CyberAPI Threat Intelligence API.

## Installation

```bash
pip install requests
# Copy cyberapi.py to your project
```

## Quick Start

```python
from cyberapi import CyberAPI

# Initialize client
client = CyberAPI(api_key="your_api_key_here")

# Check a domain
result = client.check(domain="suspicious-site.com")
print(f"Risk Score: {result['risk_score']}")
print(f"Verdict: {result['verdict']}")

# Quick malicious check
if client.is_malicious(domain="phishing-site.xyz"):
    print("⚠️ Malicious domain detected!")

# Get threat signals
signals = client.get_signals(domain="example.com")
print(f"Detected signals: {signals}")
```

## API Reference

### `CyberAPI(api_key, base_url=...)`
Initialize the client with your API key.

### `check(domain=None, ip=None)`
Get full threat intelligence report for a domain or IP.

**Returns**: `dict` with risk_score, verdict, signals, and detailed analysis

### `is_malicious(domain=None, ip=None)`
Quick boolean check if target is malicious (risk_score >= 80).

**Returns**: `bool`

### `get_signals(domain=None, ip=None)`
Get list of detected threat signals.

**Returns**: `list[str]`

## Get Your API Key

Visit [https://threats.cyberapi.io](https://threats.cyberapi.io) to get your API key.

## Support

- Email: inbox@cyberapi.io
- Docs: https://threats.cyberapi.io/docs
