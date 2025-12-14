<?php

require_once __DIR__ . '/../CyberAPI.php';

use CyberAPI\CyberAPI;

// Replace with your actual API key
$apiKey = getenv('CYBERAPI_KEY') ?: 'demo';

$client = new CyberAPI($apiKey);

echo "Checking example.com...\n";

try {
    $result = $client->check('example.com');
    
    echo "Risk Score: " . $result['risk_score'] . "\n";
    echo "Verdict: " . $result['verdict'] . "\n";
    
    if (isset($result['geo'])) {
        echo "Location: " . $result['geo']['country'] . "\n";
    }
    
} catch (Exception $e) {
    echo "Error: " . $e->getMessage() . "\n";
}
