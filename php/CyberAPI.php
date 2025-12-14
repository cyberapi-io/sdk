<?php

namespace CyberAPI;

class CyberAPI {
    private $apiKey;
    private $baseUrl;
    private $timeout;

    public function __construct($apiKey, $baseUrl = 'https://threats.cyberapi.io/api/v1', $timeout = 10) {
        $this->apiKey = $apiKey;
        $this->baseUrl = rtrim($baseUrl, '/');
        $this->timeout = $timeout;
    }

    /**
     * Check a domain or IP for threats
     *
     * @param string|null $domain Domain name to check
     * @param string|null $ip IP address to check
     * @return array Response from the API
     * @throws \Exception If the request fails
     */
    public function check($domain = null, $ip = null) {
        if (!$domain && !$ip) {
            throw new \InvalidArgumentException("Either domain or ip must be provided");
        }

        $params = [];
        if ($domain) $params['domain'] = $domain;
        if ($ip) $params['ip'] = $ip;

        $url = $this->baseUrl . '/check?' . http_build_query($params);

        $ch = curl_init();
        curl_setopt($ch, CURLOPT_URL, $url);
        curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
        curl_setopt($ch, CURLOPT_HTTPHEADER, [
            'X-API-Key: ' . $this->apiKey,
            'Accept: application/json',
            'User-Agent: CyberAPI-PHP/1.0.0'
        ]);
        curl_setopt($ch, CURLOPT_TIMEOUT, $this->timeout);

        $response = curl_exec($ch);
        $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
        $error = curl_error($ch);
        
        curl_close($ch);

        if ($error) {
            throw new \Exception("Connection error: " . $error);
        }

        $data = json_decode($response, true);

        if ($httpCode >= 400) {
            $message = isset($data['detail']) ? $data['detail'] : "HTTP Error $httpCode";
            throw new \Exception($message, $httpCode);
        }

        return $data;
    }
}
