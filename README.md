# PR Review Agent

AI-powered Pull Request Review Agent that automatically analyzes Pull Requests using Large Language Models (LLMs) and posts actionable review comments back to GitHub or Azure DevOps.

Built with:

* .NET 8 Web API
* Semantic Kernel
* Azure OpenAI / OpenAI
* GitHub Integration
* Azure DevOps Integration
* Dependency Injection
* Clean Architecture Principles

---

## Features

### Pull Request Review Automation

* Analyze Pull Request diffs automatically
* Detect bugs and code smells
* Identify security vulnerabilities
* Review architecture and design decisions
* Suggest performance improvements
* Recommend better exception handling
* Identify missing logging
* Detect scalability concerns
* Highlight maintainability issues
* Suggest missing unit tests

### AI-Powered Analysis

Supports:

* GPT-4o
* GPT-4.1
* GPT-4 Turbo
* GPT-35 Turbo

Providers:

* Azure OpenAI
* OpenAI

### Integrations

* GitHub Pull Requests
* GitHub Webhooks
* Azure DevOps Pull Requests
* Azure DevOps Service Hooks

---

## Table of Contents

* Overview
* Features
* Architecture
* Solution Structure
* Review Categories
* Prerequisites
* Configuration
* Running Locally
* API Endpoints
* Health Check
* Webhook Setup
* How It Works
* Sample Review Output
* Security
* Cost Considerations
* Deployment
* CI/CD
* Testing
* Troubleshooting
* Roadmap
* FAQ
* Contributing
* License

---

# Overview

PR Review Agent receives Pull Request events through webhooks, analyzes code changes using AI, and posts structured review comments back to the Pull Request.

The goal is to provide fast, consistent, and scalable code reviews while reducing manual review effort.

---

# Architecture

High-Level Flow:

GitHub Pull Request
│
▼
Webhook Event
│
▼
GitHubController
│
▼
ReviewService
│
▼
Semantic Kernel
│
▼
Azure OpenAI / OpenAI
│
▼
Review Issues Generated
│
▼
GitHub Review Comments

---

# Solution Structure

PRReviewAgent

├── Controllers

│   └── GitHubController.cs

├── Services

│   ├── GitHubService.cs

│   └── ReviewService.cs

├── Models

│   ├── ReviewIssue.cs

│   └── Webhooks

├── Prompts

│   └── PRReviewPrompt.txt

├── appsettings.json

├── Program.cs

└── README.md

---

# Review Categories

The AI reviews code for:

## Bugs

Examples:

* Null reference risks
* Incorrect logic
* Race conditions

## Security

Examples:

* Hardcoded secrets
* Sensitive logging
* Injection vulnerabilities

## Performance

Examples:

* N+1 queries
* Unnecessary allocations
* Blocking calls

## Maintainability

Examples:

* Duplicate code
* Complex methods
* Poor naming

## SOLID Principles

Examples:

* SRP violations
* Tight coupling
* Dependency issues

## Scalability

Examples:

* Synchronous bottlenecks
* Missing caching
* Poor API design

## Testing

Examples:

* Missing unit tests
* Missing edge-case validation

---

# Prerequisites

Before running the application ensure:

* .NET 8 SDK installed
* GitHub Repository
* GitHub Personal Access Token
* Azure OpenAI Resource or OpenAI API Key
* Visual Studio 2022 / VS Code (Optional)

---

# Configuration

Configuration can be supplied through:

* appsettings.json
* Environment Variables
* Azure Key Vault
* User Secrets

## Environment Variables

ASPNETCORE_ENVIRONMENT

OPENAI_PROVIDER

OPENAI_API_KEY

AZURE_OPENAI_ENDPOINT

AZURE_OPENAI_API_KEY

AZURE_OPENAI_DEPLOYMENT

GITHUB_TOKEN

GITHUB_WEBHOOK_SECRET

AZDO_ORG

AZDO_PROJECT

AZDO_PAT

---

## User Secrets

Initialize:

dotnet user-secrets init

Store secrets:

dotnet user-secrets set "AzureOpenAI:ApiKey" "<your-key>"

Never commit secrets to source control.

---

# Running Locally

## Clone Repository

git clone https://github.com/SandyCode1/pr-review-agent

cd pr-review-agent

## Restore Packages

dotnet restore

## Build

dotnet build

## Run

dotnet run

---

# API Endpoints

| Method | Endpoint            | Description             |
| ------ | ------------------- | ----------------------- |
| POST   | /api/github/webhook | GitHub Webhook Receiver |
| GET    | /api/health         | Health Check            |
| GET    | /swagger            | Swagger UI              |

---

# Health Check

Request:

GET /api/health

Response:

{
"status": "Healthy"
}

---

# GitHub Webhook Setup

Navigate to:

Repository → Settings → Webhooks

Create Webhook:

Payload URL:

https://your-domain/api/github/webhook

Content Type:

application/json

Secret:

Use GITHUB_WEBHOOK_SECRET value.

Events:

* Pull Requests
* Pull Request Reviews (Optional)

---

# Azure DevOps Setup

Create a Service Hook:

* Pull Request Created
* Pull Request Updated

Target:

https://your-domain/api/github/webhook

---

# How It Works

Step 1

Pull Request is created or updated.

Step 2

GitHub sends a webhook event.

Step 3

GitHubController receives the event.

Step 4

ReviewService fetches changed files and diffs.

Step 5

Prompt is generated.

Step 6

Prompt is sent to Azure OpenAI.

Step 7

AI analyzes code.

Step 8

Review comments are generated.

Step 9

Comments are posted back to GitHub.

---

# Sample Review Output

Category: Security

Severity: High

Issue:

Sensitive token value is logged.

Recommendation:

Avoid logging secrets and mask sensitive information before writing logs.

File:

AuthenticationService.cs

Line:

42

---

Category: Performance

Severity: Medium

Issue:

Database query executed inside a loop.

Recommendation:

Move query outside loop or use batch retrieval.

---

# Prompt Customization

Prompt Location:

Prompts/PRReviewPrompt.txt

You can customize:

* Review tone
* Security focus
* Coding standards
* Review categories
* Severity definitions

---

# Security

Recommended Practices:

* Validate GitHub webhook signatures
* Store secrets in Azure Key Vault
* Use User Secrets locally
* Rotate PAT tokens regularly
* Enable HTTPS only
* Restrict webhook endpoints

Never commit:

* PAT tokens
* OpenAI keys
* Azure OpenAI keys
* Webhook secrets

---

# Cost Considerations

## Azure OpenAI

Cost depends on:

* Model selected
* Input tokens
* Output tokens

Typical Pull Request review:

* 5K–20K tokens

Estimated cost:

* A few cents per review

## Hosting

Development:

* Azure Free Tier

Production:

* Azure App Service Basic or higher

---

# Deployment

## Azure App Service

1. Create App Service
2. Configure Environment Variables
3. Deploy Application
4. Configure Webhook URL
5. Test Health Endpoint

## Docker

Build:

docker build -t prreviewagent .

Run:

docker run -p 8080:80 prreviewagent

---

# CI/CD

Recommended Pipeline:

1. Restore
2. Build
3. Test
4. Publish
5. Deploy

GitHub Actions Example:

* Build on Pull Request
* Deploy on Main Branch Merge

---

# Testing

Recommended Unit Tests:

## ReviewService

* Prompt generation
* Review parsing
* Severity mapping

## GitHubService

* API communication
* Error handling
* Authentication

## Controller

* Webhook validation
* Request handling

Integration Testing:

* Postman
* Curl
* GitHub Webhook Delivery Tests

---

# Troubleshooting

## 401 Unauthorized

Verify:

* GitHub PAT
* Repository Access
* Token Scopes

## Webhook Failing

Verify:

* URL
* Secret
* Event Type

## Azure OpenAI Errors

Verify:

* Endpoint
* API Key
* Deployment Name

## Rate Limiting

Implement:

* Retry Policy
* Exponential Backoff

---

# Production Recommendations

For enterprise deployments:

* Azure Key Vault
* Azure Application Insights
* Distributed Cache
* Background Queue Processing
* Retry Policies
* Structured Logging
* Health Monitoring
* Rate Limiting
* Audit Logging

---

# Future Roadmap

## Current

* GitHub PR Reviews
* Azure OpenAI Integration
* Semantic Kernel

## Planned

* Azure DevOps Review Threads
* Reviewer Assignment Suggestions
* Historical PR Learning
* Team-Specific Rules
* RAG-Based Reviews
* Azure AI Search Integration
* Redis Caching
* Service Bus Processing
* Multi-Tenant Support

---

# FAQ

## Can I use OpenAI instead of Azure OpenAI?

Yes.

Set:

OPENAI_PROVIDER=openai

and configure OPENAI_API_KEY.

## Can I customize review rules?

Yes.

Update:

Prompts/PRReviewPrompt.txt

## Can I deploy on-premises?

Yes.

Deploy using:

* IIS
* Docker
* Kubernetes
* Linux VM

---

# Contributing

Contributions are welcome.

Steps:

1. Fork repository
2. Create feature branch
3. Commit changes
4. Open Pull Request

Please include:

* Clear description
* Unit tests
* Documentation updates

---

# License

This project currently does not include a license.

For open-source usage, consider adding:

MIT License

or

Apache 2.0 License

---

# Author

PR Review Agent

Built using:

* .NET 8
* Semantic Kernel
* Azure OpenAI
* GitHub APIs

Designed to provide scalable, automated, AI-assisted Pull Request Reviews.





-------

A couple more sections I'd add later (once the project matures) are:

Swagger screenshots
End-to-end architecture diagram (Mermaid)
GitHub Actions workflow YAML
Docker Compose setup
Kubernetes deployment manifests
Azure deployment guide with screenshots
Application Insights monitoring guide

Those additions can make the repository look like a Solution Architect–level showcase project rather than a simple demo.