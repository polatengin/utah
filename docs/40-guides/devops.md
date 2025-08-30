---
layout: default
title: DevOps Integration
parent: Guides
nav_order: 5
---

Learn how to integrate Utah scripts into your DevOps workflows, CI/CD pipelines, and automation processes.

## CI/CD Integration

### GitHub Actions

```yaml
# .github/workflows/deploy.yml
name: Deploy with Utah

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest

    steps:
    - uses: actions/checkout@v3

    - name: Install Utah
      run: |
        curl -sL https://utahshx.com/install.sh | sudo bash

    - name: Run Deployment Script
      run: |
        utah scripts/deploy.shx
      env:
        DEPLOY_ENV: production
        API_KEY: ${{ secrets.API_KEY }}

    - name: Health Check
      run: |
        utah scripts/server-health-check.shx
```

### GitLab CI/CD

```yaml
# .gitlab-ci.yml
stages:
  - test
  - deploy

variables:
  UTAH_VERSION: "latest"

before_script:
  - curl -sL https://utahshx.com/install.sh | sudo bash

test:
  stage: test
  script:
    - utah test/run-tests.shx
  artifacts:
    reports:
      junit: test-results.xml

deploy:
  stage: deploy
  script:
    - utah scripts/deploy.shx
  only:
    - main
  environment:
    name: production
    url: https://example.com
```

### Jenkins Pipeline

```groovy
// Jenkinsfile
pipeline {
    agent any

    environment {
        UTAH_HOME = '/usr/local/bin'
    }

    stages {
        stage('Setup') {
            steps {
                sh '''
                    curl -sL https://utahshx.com/install.sh | sudo bash
                    utah version
                '''
            }
        }

        stage('Test') {
            steps {
                sh 'utah test/run-tests.shx'
            }
            post {
                always {
                    publishTestResults testResultsPattern: 'test-results.xml'
                }
            }
        }

        stage('Deploy') {
            when {
                branch 'main'
            }
            steps {
                sh 'utah scripts/deploy.shx'
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}
```

## Infrastructure as Code

### Terraform Integration

```typescript
// infrastructure/terraform.shx
import "utils/terraform.shx";

function deployInfrastructure(): void {
  console.log("Deploying infrastructure...");

  // Initialize Terraform
  terraform.init();

  // Plan changes
  let plan: string = terraform.plan();
  console.log(plan);

  // Apply changes
  if (console.prompt("Apply changes? (y/n)") == "y") {
    terraform.apply();
    console.log("Infrastructure deployed successfully!");
  }
}

function destroyInfrastructure(): void {
  console.log("Destroying infrastructure...");

  if (console.prompt("Are you sure? (y/n)") == "y") {
    terraform.destroy();
    console.log("Infrastructure destroyed!");
  }
}

// Main execution
let action: string = args.get("--action");

switch (action) {
  case "deploy":
    deployInfrastructure();
    break;
  case "destroy":
    destroyInfrastructure();
    break;
  default:
    console.log("Usage: utah run terraform.shx --action [deploy|destroy]");
    break;
}
```

### Ansible Integration

```typescript
// infrastructure/ansible.shx
import "utils/ansible.shx";

function runPlaybook(playbook: string, inventory: string): void {
  console.log("Running playbook: ${playbook}");

  let result: string = ansible.playbook(playbook, inventory);

  if (string.contains(result, "failed=0")) {
    console.log("✅ Playbook executed successfully!");
  } else {
    console.log("❌ Playbook execution failed!");
    exit(1);
  }
}

function manageServices(): void {
  let services: string[] = ["nginx", "postgresql", "redis"];

  for (let service: string in services) {
    console.log("Managing service: ${service}");
    ansible.service(service, "restarted");
  }
}

// Execute playbooks
runPlaybook("site.yml", "production");
manageServices();
```

## Container Integration

### Docker Operations

```typescript
// docker/docker-ops.shx
function buildAndPush(image: string, tag: string): void {
  console.log("Building ${image}:${tag}...");

  // Build image
  let buildResult: string = `$(docker build -t ${image}:${tag} .)`;

  if (string.contains(buildResult, "Successfully built")) {
    console.log("✅ Image built successfully!");

    // Push to registry
    `$(docker push ${image}:${tag})`;
    console.log("✅ Image pushed to registry!");
  } else {
    console.log("❌ Image build failed!");
    exit(1);
  }
}

function deployContainer(image: string, port: number): void {
  console.log("Deploying container: ${image}");

  // Stop existing container
  `$(docker stop ${image} || true)`;
  `$(docker rm ${image} || true)`;

  // Run new container
  let runCmd: string = "docker run -d --name ${image} -p ${port}:80 ${image}:latest";
  `$(${runCmd})`;

  console.log("✅ Container deployed on port ${port}");
}

// Main deployment
buildAndPush("my-app", "latest");
deployContainer("my-app", 8080);
```

### Kubernetes Deployment

```typescript
// kubernetes/k8s-deploy.shx
function deployToKubernetes(namespace: string, manifest: string): void {
  console.log("Deploying to namespace: ${namespace}");

  // Create namespace if it doesn't exist
  `$(kubectl create namespace ${namespace} || true)`;

  // Apply manifest
  let result: string = `$(kubectl apply -f ${manifest} -n ${namespace})`;

  if (string.contains(result, "created") || string.contains(result, "configured")) {
    console.log("✅ Deployment successful!");
  } else {
    console.log("❌ Deployment failed!");
    exit(1);
  }
}

function waitForDeployment(namespace: string, deployment: string): void {
  console.log("Waiting for deployment: ${deployment}");

  `$(kubectl rollout status deployment/${deployment} -n ${namespace})`;
  console.log("✅ Deployment ready!");
}

function checkHealth(namespace: string, service: string): void {
  console.log("Checking health of service: ${service}");

  let pods: string = `$(kubectl get pods -n ${namespace} -l app=${service})`;

  if (string.contains(pods, "Running")) {
    console.log("✅ Service is healthy!");
  } else {
    console.log("❌ Service is not healthy!");
    exit(1);
  }
}

// Deploy application
deployToKubernetes("production", "manifests/app.yaml");
waitForDeployment("production", "my-app");
checkHealth("production", "my-app");
```

## Monitoring and Alerting

### Health Checks

```typescript
// monitoring/server-health-check.shx
function checkWebservice(url: string): boolean {
  try {
    let response: string = web.get(url);
    return string.contains(response, "healthy");
  }
  catch {
    return false;
  }
}

function checkDatabase(connectionString: string): boolean {
  try {
    let result: string = `$(psql "${connectionString}" -c "SELECT 1;")`;
    return string.contains(result, "1 row");
  }
  catch {
    return false;
  }
}

function checkServices(): void {
  let services: object[] = [
    json.parse("{\"name\": \"Web API\", \"url\": \"https://api.example.com/health\"}"),
    json.parse("{\"name\": \"Database\", \"connection\": \"postgresql://user:pass@localhost:5432/db\"}")
  ];

  for (let service: object in services) {
    let serviceName: string = json.get(service, ".name");
    let serviceUrl: string = json.get(service, ".url") || "";
    let serviceConnection: string = json.get(service, ".connection") || "";
    
    let healthy: boolean = false;
    if (serviceUrl != "") {
      healthy = checkWebservice(serviceUrl);
    } else if (serviceConnection != "") {
      healthy = checkDatabase(serviceConnection);
    }

    if (healthy) {
      console.log("✅ ${serviceName} is healthy");
    } else {
      console.log("❌ ${serviceName} is unhealthy");
      sendAlert(serviceName);
    }
  }
}

function sendAlert(serviceName: string): void {
  let message: string = "Alert: ${serviceName} is unhealthy";

  // Send to Slack
  web.post("https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK", json.parse("{\"text\": \"${message}\"}"));

  // Send email
  `$(echo "${message}" | mail -s "Service Alert" admin@example.com)`;
}

checkServices();
```

### Log Analysis

```typescript
// monitoring/log-analysis.shx
function analyzeAccessLogs(logFile: string): void {
  console.log("Analyzing access logs: ${logFile}");

  let logs: string = fs.readFile(logFile);
  let lines: string[] = string.split(logs, "\n");

  let errorCount: number = 0;
  let warningCount: number = 0;

  for (let line: string in lines) {
    if (string.contains(line, "ERROR")) {
      errorCount++;
    } else if (string.contains(line, "WARN")) {
      warningCount++;
    }
  }

  console.log("Errors: ${errorCount}, Warnings: ${warningCount}");

  if (errorCount > 10) {
    sendAlert("High error count: ${errorCount}");
  }
}

function rotateLogFiles(logDir: string): void {
  console.log("Rotating logs in: ${logDir}");

  let filesList: string = `$(ls ${logDir}/*.log 2>/dev/null || true)`;
  let files: string[] = string.split(filesList, "\n");

  for (let file: string in files) {
    if (string.length(file) > 0) {
      let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;
      let archivedName: string = "${file}.${timestamp}";

      fs.move(file, archivedName);
      `$(gzip ${archivedName})`;
    }
  }
}

// Analyze and rotate logs
analyzeAccessLogs("/var/log/nginx/access.log");
rotateLogFiles("/var/log/myapp");
```

## Security and Compliance

### Security Scanning

```typescript
// security/security-scan.shx
function scanForVulnerabilities(): void {
  console.log("Running security scan...");

  // Run dependency check
  let depCheck: string = `$(dependency-check --project myapp --scan .)`;

  if (string.contains(depCheck, "High") || string.contains(depCheck, "Critical")) {
    console.log("❌ Critical vulnerabilities found!");
    exit(1);
  }

  // Run container scan
  let containerScan: string = `$(trivy image myapp:latest)`;

  if (string.contains(containerScan, "HIGH") || string.contains(containerScan, "CRITICAL")) {
    console.log("❌ Container vulnerabilities found!");
    exit(1);
  }

  console.log("✅ Security scan passed!");
}

function auditCompliance(): void {
  console.log("Running compliance audit...");

  // Check file permissions
  let permissions: string = `$(find /app -type f -perm 777)`;

  if (string.trim(permissions) != "") {
    console.log("❌ Files with 777 permissions found!");
    exit(1);
  }

  // Check for secrets in code
  let secrets: string = `$(grep -r 'password\\|secret\\|key' src/ || true)`;

  if (string.contains(secrets, "password") || string.contains(secrets, "secret")) {
    console.log("❌ Potential secrets found in code!");
    exit(1);
  }

  console.log("✅ Compliance audit passed!");
}

scanForVulnerabilities();
auditCompliance();
```

## Best Practices

### Error Handling

```typescript
// Always handle errors in DevOps scripts
function deployWithErrorHandling(): void {
  try {
    // Deployment logic
    `$(kubectl apply -f deployment.yaml)`;
  }
  catch {
    console.log("❌ Deployment failed");

    // Rollback on failure
    `$(kubectl rollout undo deployment/myapp)`;

    // Send notification
    sendAlert("Deployment failed, rolled back");

    exit(1);
  }
}
```

### Environment Management

```typescript
// Use environment variables for configuration
let environment: string = env.get("DEPLOY_ENV") || "development";
let apiKey: string = env.get("API_KEY") || "";

if (apiKey == "") {
  console.log("❌ API_KEY environment variable is required");
  exit(1);
}
```

### Logging and Monitoring

```typescript
// Log all operations with timestamps
function logOperation(operation: string): void {
  let timestamp: string = `$(date)`;
  console.log("[${timestamp}] ${operation}");
}

// Monitor resource usage
function checkResourceUsage(): void {
  let cpuUsage: string = `$(top -bn1 | grep 'Cpu(s)' | awk '{print $2}')`;
  let memUsage: string = `$(free -m | grep Mem | awk '{print ($3/$2)*100}')`;

  console.log("CPU Usage: ${cpuUsage}, Memory Usage: ${memUsage}%");
}
```

### Configuration Management

```typescript
// Use configuration files
import "config/production.shx";

function loadConfig(environment: string): object {
  let configFile: string = "config/${environment}.json";

  if (fs.exists(configFile)) {
    let configData: string = fs.readFile(configFile);
    return json.parse(configData);
  }

  console.log("❌ Configuration file not found: ${configFile}");
  exit(1);
}
```
