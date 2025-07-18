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
        curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

    - name: Run Deployment Script
      run: |
        utah run scripts/deploy.shx
      env:
        DEPLOY_ENV: production
        API_KEY: ${{ secrets.API_KEY }}

    - name: Health Check
      run: |
        utah run scripts/health-check.shx
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
  - curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash

test:
  stage: test
  script:
    - utah run test/run-tests.shx
  artifacts:
    reports:
      junit: test-results.xml

deploy:
  stage: deploy
  script:
    - utah run scripts/deploy.shx
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
                    curl -sL https://raw.githubusercontent.com/polatengin/utah/refs/heads/main/scripts/install.sh | sudo bash
                    utah version
                '''
            }
        }

        stage('Test') {
            steps {
                sh 'utah run test/run-tests.shx'
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
                sh 'utah run scripts/deploy.shx'
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
let action: string = args.getString("--action");

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
  console.log(`Running playbook: ${playbook}`);

  let result: string = ansible.playbook(playbook, inventory);

  if (result.includes("failed=0")) {
    console.log("✅ Playbook executed successfully!");
  } else {
    console.log("❌ Playbook execution failed!");
    script.exit(1);
  }
}

function manageServices(): void {
  let services: string[] = ["nginx", "postgresql", "redis"];

  for (let service of services) {
    console.log(`Managing service: ${service}`);
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
  console.log(`Building ${image}:${tag}...`);

  // Build image
  let buildResult: string = system.execute(`docker build -t ${image}:${tag} .`);

  if (buildResult.includes("Successfully built")) {
    console.log("✅ Image built successfully!");

    // Push to registry
    system.execute(`docker push ${image}:${tag}`);
    console.log("✅ Image pushed to registry!");
  } else {
    console.log("❌ Image build failed!");
    script.exit(1);
  }
}

function deployContainer(image: string, port: number): void {
  console.log(`Deploying container: ${image}`);

  // Stop existing container
  system.execute(`docker stop ${image} || true`);
  system.execute(`docker rm ${image} || true`);

  // Run new container
  let runCmd: string = `docker run -d --name ${image} -p ${port}:80 ${image}:latest`;
  system.execute(runCmd);

  console.log(`✅ Container deployed on port ${port}`);
}

// Main deployment
buildAndPush("my-app", "latest");
deployContainer("my-app", 8080);
```

### Kubernetes Deployment

```typescript
// kubernetes/k8s-deploy.shx
function deployToKubernetes(namespace: string, manifest: string): void {
  console.log(`Deploying to namespace: ${namespace}`);

  // Create namespace if it doesn't exist
  system.execute(`kubectl create namespace ${namespace} || true`);

  // Apply manifest
  let result: string = system.execute(`kubectl apply -f ${manifest} -n ${namespace}`);

  if (result.includes("created") || result.includes("configured")) {
    console.log("✅ Deployment successful!");
  } else {
    console.log("❌ Deployment failed!");
    script.exit(1);
  }
}

function waitForDeployment(namespace: string, deployment: string): void {
  console.log(`Waiting for deployment: ${deployment}`);

  system.execute(`kubectl rollout status deployment/${deployment} -n ${namespace}`);
  console.log("✅ Deployment ready!");
}

function checkHealth(namespace: string, service: string): void {
  console.log(`Checking health of service: ${service}`);

  let pods: string = system.execute(`kubectl get pods -n ${namespace} -l app=${service}`);

  if (pods.includes("Running")) {
    console.log("✅ Service is healthy!");
  } else {
    console.log("❌ Service is not healthy!");
    script.exit(1);
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
// monitoring/health-check.shx
function checkWebservice(url: string): boolean {
  try {
    let response: string = web.get(url);
    return response.includes("healthy");
  } catch (error) {
    return false;
  }
}

function checkDatabase(connectionString: string): boolean {
  try {
    let result: string = system.execute(`psql "${connectionString}" -c "SELECT 1;"`);
    return result.includes("1 row");
  } catch (error) {
    return false;
  }
}

function checkServices(): void {
  let services: object[] = [
    { name: "Web API", url: "https://api.example.com/health" },
    { name: "Database", connection: "postgresql://user:pass@localhost:5432/db" }
  ];

  for (let service of services) {
    let healthy: boolean = service.url
      ? checkWebservice(service.url)
      : checkDatabase(service.connection);

    if (healthy) {
      console.log(`✅ ${service.name} is healthy`);
    } else {
      console.log(`❌ ${service.name} is unhealthy`);
      sendAlert(service.name);
    }
  }
}

function sendAlert(serviceName: string): void {
  let message: string = `Alert: ${serviceName} is unhealthy`;

  // Send to Slack
  web.post("https://hooks.slack.com/services/YOUR/SLACK/WEBHOOK", {
    text: message
  });

  // Send email
  system.execute(`echo "${message}" | mail -s "Service Alert" admin@example.com`);
}

checkServices();
```

### Log Analysis

```typescript
// monitoring/log-analysis.shx
function analyzeAccessLogs(logFile: string): void {
  console.log(`Analyzing access logs: ${logFile}`);

  let logs: string = filesystem.readFile(logFile);
  let lines: string[] = logs.split("\n");

  let errorCount: number = 0;
  let warningCount: number = 0;

  for (let line of lines) {
    if (line.includes("ERROR")) {
      errorCount++;
    } else if (line.includes("WARN")) {
      warningCount++;
    }
  }

  console.log(`Errors: ${errorCount}, Warnings: ${warningCount}`);

  if (errorCount > 10) {
    sendAlert(`High error count: ${errorCount}`);
  }
}

function rotateLogFiles(logDir: string): void {
  console.log(`Rotating logs in: ${logDir}`);

  let files: string[] = filesystem.listFiles(logDir);

  for (let file of files) {
    if (file.endsWith(".log")) {
      let timestamp: string = utility.dateString();
      let archivedName: string = `${file}.${timestamp}`;

      filesystem.move(file, archivedName);
      system.execute(`gzip ${archivedName}`);
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
  let depCheck: string = system.execute("dependency-check --project myapp --scan .");

  if (depCheck.includes("High") || depCheck.includes("Critical")) {
    console.log("❌ Critical vulnerabilities found!");
    script.exit(1);
  }

  // Run container scan
  let containerScan: string = system.execute("trivy image myapp:latest");

  if (containerScan.includes("HIGH") || containerScan.includes("CRITICAL")) {
    console.log("❌ Container vulnerabilities found!");
    script.exit(1);
  }

  console.log("✅ Security scan passed!");
}

function auditCompliance(): void {
  console.log("Running compliance audit...");

  // Check file permissions
  let permissions: string = system.execute("find /app -type f -perm 777");

  if (permissions.trim() != "") {
    console.log("❌ Files with 777 permissions found!");
    script.exit(1);
  }

  // Check for secrets in code
  let secrets: string = system.execute("grep -r 'password\\|secret\\|key' src/ || true");

  if (secrets.includes("password") || secrets.includes("secret")) {
    console.log("❌ Potential secrets found in code!");
    script.exit(1);
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
    system.execute("kubectl apply -f deployment.yaml");
  } catch (error) {
    console.log(`❌ Deployment failed: ${error}`);

    // Rollback on failure
    system.execute("kubectl rollout undo deployment/myapp");

    // Send notification
    sendAlert("Deployment failed, rolled back");

    script.exit(1);
  }
}
```

### Environment Management

```typescript
// Use environment variables for configuration
let environment: string = system.env("DEPLOY_ENV") || "development";
let apiKey: string = system.env("API_KEY") || "";

if (apiKey == "") {
  console.log("❌ API_KEY environment variable is required");
  script.exit(1);
}
```

### Logging and Monitoring

```typescript
// Log all operations with timestamps
function logOperation(operation: string): void {
  let timestamp: string = utility.dateString();
  console.log(`[${timestamp}] ${operation}`);
}

// Monitor resource usage
function checkResourceUsage(): void {
  let cpuUsage: string = system.execute("top -bn1 | grep 'Cpu(s)' | awk '{print $2}'");
  let memUsage: string = system.execute("free -m | grep Mem | awk '{print ($3/$2)*100}'");

  console.log(`CPU Usage: ${cpuUsage}, Memory Usage: ${memUsage}%`);
}
```

### Configuration Management

```typescript
// Use configuration files
import "config/production.shx";

function loadConfig(environment: string): object {
  let configFile: string = `config/${environment}.json`;

  if (filesystem.exists(configFile)) {
    let configData: string = filesystem.readFile(configFile);
    return json.parse(configData);
  }

  throw new Error(`Configuration file not found: ${configFile}`);
}
```
