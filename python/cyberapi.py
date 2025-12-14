"""
CyberAPI Python SDK
Official Python client for the CyberAPI Threat Intelligence API
"""

import requests
from typing import Optional, Dict, Any

class CyberAPI:
    """
    CyberAPI Client for Python
    
    Usage:
        >>> from cyberapi import CyberAPI
        >>> client = CyberAPI(api_key="your_api_key_here")
        >>> result = client.check(domain="example.com")
        >>> print(result['risk_score'])
    """
    
    def __init__(self, api_key: str, base_url: str = "https://threats.cyberapi.io/api/v1"):
        """
        Initialize CyberAPI client
        
        Args:
            api_key: Your CyberAPI key (get one at https://threats.cyberapi.io)
            base_url: API base URL (default: production)
        """
        self.api_key = api_key
        self.base_url = base_url.rstrip('/')
        self.session = requests.Session()
        self.session.headers.update({
            'X-API-Key': api_key,
            'User-Agent': 'CyberAPI-Python-SDK/1.0.0'
        })
    
    def check(self, domain: Optional[str] = None, ip: Optional[str] = None) -> Dict[str, Any]:
        """
        Check a domain or IP for threats
        
        Args:
            domain: Domain name to check (e.g., "example.com")
            ip: IP address to check (e.g., "1.2.3.4")
            
        Returns:
            dict: Threat intelligence report
            
        Raises:
            ValueError: If neither domain nor ip is provided
            requests.HTTPError: If API request fails
        """
        if not domain and not ip:
            raise ValueError("Either 'domain' or 'ip' must be provided")
        
        params = {}
        if domain:
            params['domain'] = domain
        if ip:
            params['ip'] = ip
        
        response = self.session.get(f"{self.base_url}/check", params=params)
        response.raise_for_status()
        
        return response.json()
    
    def is_malicious(self, domain: Optional[str] = None, ip: Optional[str] = None) -> bool:
        """
        Quick check if domain/IP is malicious
        
        Returns:
            bool: True if risk_score >= 80
        """
        result = self.check(domain=domain, ip=ip)
        return result.get('risk_score', 0) >= 80
    
    def get_signals(self, domain: Optional[str] = None, ip: Optional[str] = None) -> list:
        """
        Get list of threat signals detected
        
        Returns:
            list: List of signal strings
        """
        result = self.check(domain=domain, ip=ip)
        return result.get('signals', [])
