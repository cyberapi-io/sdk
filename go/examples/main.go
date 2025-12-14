package main

import (
	"fmt"
	"log"
	"os"

	"github.com/fabriziosalmi/cyber-api/sdk/go"
)

func main() {
	apiKey := os.Getenv("CYBERAPI_KEY")
	if apiKey == "" {
		apiKey = "demo"
	}

	client := cyberapi.NewClient(apiKey)

	fmt.Println("Checking example.com...")
	result, err := client.Check("example.com", "")
	if err != nil {
		log.Fatal(err)
	}

	fmt.Printf("Risk Score: %d\n", result.RiskScore)
	fmt.Printf("Verdict: %s\n", result.Verdict)
	
	if result.Geo != nil {
		fmt.Printf("Location: %v\n", result.Geo["country"])
	}
}
