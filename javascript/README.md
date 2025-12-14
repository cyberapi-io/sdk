# CyberAPI JavaScript SDK

Official JavaScript/Node.js client for CyberAPI Threat Intelligence API.

## Installation

### Node.js
```bash
# Copy cyberapi.js to your project
const CyberAPI = require('./cyberapi');
```

### Browser
```html
<script src="cyberapi.js"></script>
```

## Quick Start

### Node.js
```javascript
const CyberAPI = require('./cyberapi');

const client = new CyberAPI('your_api_key_here');

// Check a domain
const result = await client.check({ domain: 'suspicious-site.com' });
console.log(`Risk Score: ${result.risk_score}`);
console.log(`Verdict: ${result.verdict}`);

// Quick malicious check
if (await client.isMalicious({ domain: 'phishing-site.xyz' })) {
  console.log('⚠️ Malicious domain detected!');
}

// Get threat signals
const signals = await client.getSignals({ domain: 'example.com' });
console.log('Detected signals:', signals);
```

### Browser
```javascript
const client = new CyberAPI('your_api_key_here');

client.check({ domain: 'example.com' })
  .then(result => {
    console.log('Risk Score:', result.risk_score);
  })
  .catch(error => {
    console.error('Error:', error);
  });
```

## API Reference

### `new CyberAPI(apiKey, baseUrl?)`
Initialize the client with your API key.

### `check({ domain?, ip? })`
Get full threat intelligence report for a domain or IP.

**Returns**: `Promise<Object>` with risk_score, verdict, signals, and detailed analysis

### `isMalicious({ domain?, ip? })`
Quick boolean check if target is malicious (risk_score >= 80).

**Returns**: `Promise<boolean>`

### `getSignals({ domain?, ip? })`
Get list of detected threat signals.

**Returns**: `Promise<Array<string>>`

## Get Your API Key

Visit [https://www.cyberapi.io](https://www.cyberapi.io) to get your API key.

## Support

- Email: inbox@cyberapi.io
- Docs: https://www.cyberapi.io/docs
