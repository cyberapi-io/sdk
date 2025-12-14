/**
 * CyberAPI JavaScript/Node.js SDK
 * Official client for CyberAPI Threat Intelligence API
 */

class CyberAPI {
    /**
     * Initialize CyberAPI client
     * @param {string} apiKey - Your CyberAPI key
     * @param {string} baseUrl - API base URL (default: production)
     */
    constructor(apiKey, baseUrl = 'https://threats.cyberapi.io/api/v1') {
        this.apiKey = apiKey;
        this.baseUrl = baseUrl.replace(/\/$/, '');
    }

    /**
     * Check a domain or IP for threats
     * @param {Object} options - Check options
     * @param {string} options.domain - Domain to check
     * @param {string} options.ip - IP address to check
     * @returns {Promise<Object>} Threat intelligence report
     */
    async check({ domain = null, ip = null } = {}) {
        if (!domain && !ip) {
            throw new Error("Either 'domain' or 'ip' must be provided");
        }

        const params = new URLSearchParams();
        if (domain) params.append('domain', domain);
        if (ip) params.append('ip', ip);

        const response = await fetch(`${this.baseUrl}/check?${params}`, {
            headers: {
                'X-API-Key': this.apiKey,
                'User-Agent': 'CyberAPI-JS-SDK/1.0.0'
            }
        });

        if (!response.ok) {
            throw new Error(`API Error: ${response.status} ${response.statusText}`);
        }

        return await response.json();
    }

    /**
     * Quick check if domain/IP is malicious
     * @param {Object} options - Check options
     * @returns {Promise<boolean>} True if risk_score >= 80
     */
    async isMalicious({ domain = null, ip = null } = {}) {
        const result = await this.check({ domain, ip });
        return result.risk_score >= 80;
    }

    /**
     * Get list of threat signals detected
     * @param {Object} options - Check options
     * @returns {Promise<Array<string>>} List of signals
     */
    async getSignals({ domain = null, ip = null } = {}) {
        const result = await this.check({ domain, ip });
        return result.signals || [];
    }
}

// Export for Node.js and browsers
if (typeof module !== 'undefined' && module.exports) {
    module.exports = CyberAPI;
}
