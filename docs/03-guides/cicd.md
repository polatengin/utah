---
layout: default
title: CI/CD Integration
parent: Guides
nav_order: 4
---

Continuous integration and deployment automation with Utah. Learn how to integrate Utah scripts into CI/CD pipelines, automate deployments, and manage build processes.

## Prerequisites

- Understanding of CI/CD concepts
- Experience with build pipelines
- Knowledge of containerization (Docker)
- Familiarity with version control (Git)

## Pipeline Integration Patterns

### Build Pipeline Script

```typescript
script.description("Build and test application in CI pipeline");

args.define("--environment", "-e", "Target environment", "string", false, "development");
args.define("--build-number", "-b", "Build number", "string", true);
args.define("--skip-tests", "-s", "Skip test execution", "boolean", false, false);

let environment: string = args.getString("--environment");
let buildNumber: string = args.getString("--build-number");
let skipTests: boolean = args.getBoolean("--skip-tests");

// Set up build environment
script.exitOnError(true);
script.enableDebug(false);

console.log("Starting build ${buildNumber} for ${environment}");

// Validate environment
let validEnvironments: string[] = ["development", "staging", "production"];
if (!validEnvironments.contains(environment)) {
  console.log("❌ Invalid environment: ${environment}");
  console.log("Valid environments: ${validEnvironments.join(", ")}");
  exit(1);
}

// Check prerequisites
function validateBuildEnvironment(): void {
  console.log("Validating build environment...");

  let requiredTools: string[] = ["node", "npm", "docker", "git"];
  for (let tool: string in requiredTools) {
    if (!os.isInstalled(tool)) {
      console.log("❌ Required tool missing: ${tool}");
      exit(1);
    } else {
      let version: string = "$(${tool} --version | head -1)";
      console.log("✅ ${tool}: ${version}");
    }
  }
}

// Install dependencies
function installDependencies(): void {
  console.log("Installing dependencies...");

  if (fs.exists("package.json")) {
    `$(npm ci)`;
    console.log("✅ NPM dependencies installed");
  }

  if (fs.exists("requirements.txt")) {
    `$(pip install -r requirements.txt)`;
    console.log("✅ Python dependencies installed");
  }

  if (fs.exists("Gemfile")) {
    `$(bundle install)`;
    console.log("✅ Ruby dependencies installed");
  }
}

// Run tests
function runTests(): void {
  if (skipTests) {
    console.log("⏭️  Skipping tests (--skip-tests flag)");
    return;
  }

  console.log("Running tests...");

  if (fs.exists("package.json")) {
    let packageContent: string = fs.readFile("package.json");
    let packageData: object = json.parse(packageContent);

    if (json.has(packageData, ".scripts.test")) {
      `$(npm test)`;
      console.log("✅ JavaScript tests passed");
    }
  }

  if (fs.exists("pytest.ini") || fs.exists("setup.py")) {
    `$(python -m pytest)`;
    console.log("✅ Python tests passed");
  }

  if (fs.exists("Rakefile")) {
    `$(rake test)`;
    console.log("✅ Ruby tests passed");
  }
}

// Build application
function buildApplication(): void {
  console.log("Building application...");

  // Frontend build
  if (fs.exists("package.json")) {
    let packageContent: string = fs.readFile("package.json");
    let packageData: object = json.parse(packageContent);

    if (json.has(packageData, ".scripts.build")) {
      `$(npm run build)`;
      console.log("✅ Frontend build completed");
    }
  }

  // Docker build
  if (fs.exists("Dockerfile")) {
    let imageName: string = "myapp:${buildNumber}";
    "$(docker build -t ${imageName} .)";
    console.log("✅ Docker image built: ${imageName}");

    // Tag for environment
    let envTag: string = "myapp:${environment}-latest";
    "$(docker tag ${imageName} ${envTag})";
    console.log("✅ Tagged for environment: ${envTag}");
  }
}

// Generate build artifacts
function generateArtifacts(): void {
  console.log("Generating build artifacts...");

  let artifactsDir: string = "artifacts";
  fs.createDirectory(artifactsDir);

  // Build metadata
  let buildInfo: object = {
    "build_number": buildNumber,
    "environment": environment,
    "timestamp": `$(date -Iseconds)`,
    "git_commit": `$(git rev-parse HEAD)`,
    "git_branch": `$(git rev-parse --abbrev-ref HEAD)`,
    "node_version": `$(node --version 2>/dev/null || echo "N/A")`,
    "docker_version": `$(docker --version 2>/dev/null || echo "N/A")`
  };

  fs.writeFile("${artifactsDir}/build-info.json", json.stringify(buildInfo, true));

  // Copy important files
  if (fs.exists("dist")) {
    "$(cp -r dist ${artifactsDir}/")`;
  }

  if (fs.exists("build")) {
    "$(cp -r build ${artifactsDir}/")`;
  }

  console.log("✅ Artifacts generated in ${artifactsDir}/");
}

// Main build process
validateBuildEnvironment();
installDependencies();
runTests();
buildApplication();
generateArtifacts();

console.log("🎉 Build ${buildNumber} completed successfully for ${environment}");
```

### Deployment Script

```typescript
script.description("Deploy application to target environment");

args.define("--environment", "-e", "Target environment", "string", true);
args.define("--version", "-v", "Version to deploy", "string", true);
args.define("--rollback", "-r", "Rollback to previous version", "boolean", false, false);
args.define("--dry-run", "-d", "Dry run mode", "boolean", false, false);

let environment: string = args.getString("--environment");
let version: string = args.getString("--version");
let isRollback: boolean = args.getBoolean("--rollback");
let isDryRun: boolean = args.getBoolean("--dry-run");

script.exitOnError(true);

if (isDryRun) {
  console.log("🧪 Running in DRY RUN mode - no actual changes will be made");
}

// Environment configuration
let envConfig: object = {
  "staging": {
    "replicas": 2,
    "namespace": "staging",
    "domain": "staging.myapp.com"
  },
  "production": {
    "replicas": 5,
    "namespace": "production",
    "domain": "myapp.com"
  }
};

if (!json.has(envConfig, ".${environment}")) {
  console.log("❌ Unknown environment: ${environment}");
  exit(1);
}

let config: object = json.get(envConfig, ".${environment}");
let replicas: number = json.getNumber(config, ".replicas");
let namespace: string = json.getString(config, ".namespace");
let domain: string = json.getString(config, ".domain");

console.log("Deploying to ${environment} environment");
console.log("Version: ${version}");
console.log("Replicas: ${replicas}");
console.log("Namespace: ${namespace}");
console.log("Domain: ${domain}");

// Pre-deployment checks
function preDeploymentChecks(): void {
  console.log("Running pre-deployment checks...");

  // Check if kubectl is available and configured
  if (!os.isInstalled("kubectl")) {
    console.log("❌ kubectl not found");
    exit(1);
  }

  // Check cluster connectivity
  let clusterInfo: string = `$(kubectl cluster-info 2>&1 | head -1)`;
  if (!clusterInfo.contains("is running")) {
    console.log("❌ Cannot connect to Kubernetes cluster");
    exit(1);
  }

  console.log("✅ Kubernetes cluster accessible");

  // Check namespace exists
  let nsExists: string = "$(kubectl get namespace ${namespace} 2>/dev/null && echo "exists" || echo "missing")";
  if (nsExists.trim() != "exists") {
    console.log("❌ Namespace ${namespace} does not exist");
    exit(1);
  }

  console.log("✅ Namespace ${namespace} exists");

  // Check if image exists
  if (!isDryRun) {
    let imageExists: string = "$(docker manifest inspect myapp:${version} >/dev/null 2>&1 && echo "exists" || echo "missing")";
    if (imageExists.trim() != "exists") {
      console.log("❌ Docker image myapp:${version} not found");
      exit(1);
    }

    console.log("✅ Docker image myapp:${version} exists");
  }
}

// Backup current deployment
function backupCurrentDeployment(): void {
  console.log("Backing up current deployment...");

  let backupDir: string = "backups/${environment}";
  fs.createDirectory(backupDir);

  let timestamp: string = `$(date +%Y%m%d_%H%M%S)`;
  let backupFile: string = "${backupDir}/deployment-${timestamp}.yaml";

  if (!isDryRun) {
    "$(kubectl get deployment myapp -n ${namespace} -o yaml > ${backupFile} 2>/dev/null || echo "No existing deployment")";

    if (fs.exists(backupFile)) {
      console.log("✅ Deployment backed up to ${backupFile}");
    }
  } else {
    console.log("🧪 Would backup to ${backupFile}");
  }
}

// Deploy application
function deployApplication(): void {
  if (isRollback) {
    console.log("Rolling back to previous version...");

    if (!isDryRun) {
      "$(kubectl rollout undo deployment/myapp -n ${namespace})";
      console.log("✅ Rollback initiated");
    } else {
      console.log("🧪 Would rollback deployment");
    }
  } else {
    console.log("Deploying version ${version}...");

    // Read deployment manifest template from file
    let deploymentManifest: string = fs.readFile("deployment-template.yaml");

    // Replace placeholders with actual values
    deploymentManifest = string.replace(deploymentManifest, "{{NAMESPACE}}", namespace);
    deploymentManifest = string.replace(deploymentManifest, "{{REPLICAS}}", replicas.toString());
    deploymentManifest = string.replace(deploymentManifest, "{{VERSION}}", version);
      containers:
      - name: myapp
        image: myapp:${version}
        ports:
        - containerPort: 3000
        env:
        - name: ENVIRONMENT
          value: "${environment}"
        - name: VERSION
          value: "${version}"
`;

    let manifestFile: string = "deployment-${environment}.yaml";
    fs.writeFile(manifestFile, deploymentManifest);

    if (!isDryRun) {
      "$(kubectl apply -f ${manifestFile})";
      console.log("✅ Deployment applied");
    } else {
      console.log("🧪 Would apply deployment manifest: ${manifestFile}");
    }
  }
}

// Wait for deployment to complete
function waitForDeployment(): void {
  if (isDryRun) {
    console.log("🧪 Would wait for deployment completion");
    return;
  }

  console.log("Waiting for deployment to complete...");

  let timeout: number = 300; // 5 minutes
  "$(kubectl rollout status deployment/myapp -n ${namespace} --timeout=${timeout}s)";

  // Check if deployment is ready
  let readyReplicas: string = "$(kubectl get deployment myapp -n ${namespace} -o jsonpath='{.status.readyReplicas}')";
  let desiredReplicas: string = "$(kubectl get deployment myapp -n ${namespace} -o jsonpath='{.spec.replicas}')";

  if (readyReplicas == desiredReplicas) {
    console.log("✅ Deployment completed: ${readyReplicas}/${desiredReplicas} replicas ready");
  } else {
    console.log("❌ Deployment failed: ${readyReplicas}/${desiredReplicas} replicas ready");
    exit(1);
  }
}

// Health check
function performHealthCheck(): void {
  if (isDryRun) {
    console.log("🧪 Would perform health checks");
    return;
  }

  console.log("Performing health checks...");

  // Get pod IPs
  let podIPs: string[] = "$(kubectl get pods -n ${namespace} -l app=myapp -o jsonpath='{.items[*].status.podIP}')".split(" ");

  for (let podIP: string in podIPs) {
    if (podIP.trim() != "") {
      let healthCheck: string = "$(curl -s -o /dev/null -w "%{http_code}" http://${podIP}:3000/health 2>/dev/null || echo "000")";

      if (healthCheck == "200") {
        console.log("✅ Health check passed for pod ${podIP}");
      } else {
        console.log("❌ Health check failed for pod ${podIP} (HTTP ${healthCheck})");
      }
    }
  }
}

// Update service configuration
function updateService(): void {
  console.log("Updating service configuration...");

  // Read service manifest template from file
  let serviceManifest: string = fs.readFile("service-template.yaml");

  // Replace placeholders with actual values
  serviceManifest = string.replace(serviceManifest, "{{NAMESPACE}}", namespace);

  let serviceFile: string = "service-${environment}.yaml";
  fs.writeFile(serviceFile, serviceManifest);

  if (!isDryRun) {
    "$(kubectl apply -f ${serviceFile})";
    console.log("✅ Service updated");
  } else {
    console.log("🧪 Would update service: ${serviceFile}");
  }
}

// Post-deployment tasks
function postDeploymentTasks(): void {
  console.log("Running post-deployment tasks...");

  // Update deployment metadata
  let deploymentInfo: object = {
    "environment": environment,
    "version": version,
    "timestamp": `$(date -Iseconds)`,
    "replicas": replicas,
    "namespace": namespace,
    "domain": domain,
    "deployed_by": `$(whoami)`,
    "rollback": isRollback
  };

  let deploymentLogFile: string = "deployments/${environment}-deployments.log";
  fs.createDirectory("deployments");
  fs.appendFile(deploymentLogFile, json.stringify(deploymentInfo) + "\n");

  // Send notification (example)
  if (!isDryRun) {
    console.log(`📢 Deployment notification would be sent here`);
  }

  console.log("✅ Post-deployment tasks completed");
}

// Main deployment process
preDeploymentChecks();
backupCurrentDeployment();
deployApplication();
waitForDeployment();
performHealthCheck();
updateService();
postDeploymentTasks();

if (isRollback) {
  console.log("🔄 Rollback completed for ${environment}");
} else {
  console.log("🚀 Deployment completed: ${version} → ${environment}");
}
```

## GitHub Actions Integration

### GitHub Actions Workflow

```typescript
script.description("GitHub Actions CI/CD workflow integration");

// This script is designed to run within GitHub Actions
// Environment variables are automatically available

let githubEvent: string = env.get("GITHUB_EVENT_NAME") || "";
let githubRef: string = env.get("GITHUB_REF") || "";
let githubSha: string = env.get("GITHUB_SHA") || "";
let githubActor: string = env.get("GITHUB_ACTOR") || "";

console.log("GitHub Event: ${githubEvent}");
console.log("GitHub Ref: ${githubRef}");
console.log("Commit SHA: ${githubSha}");
console.log("Actor: ${githubActor}");

// Determine environment based on branch
function determineEnvironment(): string {
  if (githubRef == "refs/heads/main" || githubRef == "refs/heads/master") {
    return "production";
  } else if (githubRef == "refs/heads/staging") {
    return "staging";
  } else if (githubRef.startsWith("refs/heads/")) {
    return "development";
  } else if (githubRef.startsWith("refs/pull/")) {
    return "preview";
  } else {
    return "unknown";
  }
}

let environment: string = determineEnvironment();
console.log("Determined environment: ${environment}");

// Set GitHub Actions outputs
function setOutput(name: string, value: string): void {
  console.log("::set-output name=${name}::${value}");
}

// Set GitHub Actions environment variables
function setEnvVar(name: string, value: string): void {
  console.log("::set-env name=${name}::${value}");
}

// Create GitHub Actions step summary
function addStepSummary(markdown: string): void {
  let summaryFile: string = env.get("GITHUB_STEP_SUMMARY") || "";
  if (summaryFile != "") {
    fs.appendFile(summaryFile, markdown + "\n");
  }
}

// Install dependencies for CI
function installCIDependencies(): void {
  console.log("Installing CI dependencies...");

  // Install Utah CLI if not present
  if (!os.isInstalled("utah")) {
    console.log("Installing Utah CLI...");
    `$(curl -L https://github.com/polatengin/utah/releases/latest/download/utah-linux-amd64 -o /usr/local/bin/utah)`;
    `$(chmod +x /usr/local/bin/utah)`;
    console.log("✅ Utah CLI installed");
  }

  // Install other dependencies based on project type
  if (fs.exists("package.json")) {
    `$(npm ci)`;
    console.log("✅ NPM dependencies installed");
  }
}

// Run quality checks
function runQualityChecks(): boolean {
  console.log("Running quality checks...");

  let passed: boolean = true;
  let results: string[] = [];

  // Linting
  if (fs.exists("package.json")) {
    let exitCode: string = `$(npm run lint 2>&1; echo $?)`;
    if (exitCode.trim().endsWith("0")) {
      console.log("✅ Linting passed");
      results.push("✅ Linting: Passed");
    } else {
      console.log("❌ Linting failed");
      results.push("❌ Linting: Failed");
      passed = false;
    }
  }

  // Security scan
  if (fs.exists("package.json")) {
    let auditResult: string = `$(npm audit --audit-level high 2>&1; echo $?)`;
    if (auditResult.trim().endsWith("0")) {
      console.log("✅ Security audit passed");
      results.push("✅ Security Audit: Passed");
    } else {
      console.log("⚠️  Security vulnerabilities found");
      results.push("⚠️ Security Audit: Issues found");
    }
  }

  // Generate summary
  let summary: string = "## Quality Check Results\n\n${results.join("\n")}\n";
  addStepSummary(summary);

  return passed;
}

// Build and test
function buildAndTest(): boolean {
  console.log("Building and testing...");

  // Compile Utah scripts
  let utahFiles: string[] = `$(find . -name "*.shx" -not -path "./node_modules/*")`.split("\n");

  for (let utahFile: string in utahFiles) {
    if (utahFile.trim() != "") {
      console.log("Compiling ${utahFile}...");
      let compileResult: string = "$(utah compile ${utahFile} 2>&1; echo $?)";

      if (!compileResult.trim().endsWith("0")) {
        console.log("❌ Failed to compile ${utahFile}");
        return false;
      }

      console.log("✅ Compiled ${utahFile}");
    }
  }

  // Run tests
  if (fs.exists("package.json")) {
    let testResult: string = `$(npm test 2>&1; echo $?)`;
    if (!testResult.trim().endsWith("0")) {
      console.log("❌ Tests failed");
      return false;
    }

    console.log("✅ All tests passed");
  }

  return true;
}

// Create release artifacts
function createReleaseArtifacts(): void {
  if (environment != "production") {
    console.log("Skipping artifact creation for non-production environment");
    return;
  }

  console.log("Creating release artifacts...");

  let artifactsDir: string = "release-artifacts";
  fs.createDirectory(artifactsDir);

  // Package Utah scripts
  let utahFiles: string[] = `$(find . -name "*.shx" -not -path "./node_modules/*")`.split("\n");
  let compiledScripts: string[] = [];

  for (let utahFile: string in utahFiles) {
    if (utahFile.trim() != "") {
      let compiledFile: string = utahFile.replace(".shx", ".sh");
      if (fs.exists(compiledFile)) {
        compiledScripts.push(compiledFile);
      }
    }
  }

  // Create release package
  if (compiledScripts.length > 0) {
    let releasePackage: string = "${artifactsDir}/utah-scripts-${githubSha.substring(0, 8)}.tar.gz";
    let tarCmd: string = "tar -czf ${releasePackage}";

    for (let script: string in compiledScripts) {
      tarCmd += " ${script}";
    }

    "$(${tarCmd})";
    console.log("✅ Release package created: ${releasePackage}");

    setOutput("release-package", releasePackage);
  }

  // Create deployment manifest
  let deploymentManifest: object = {
    "version": githubSha.substring(0, 8),
    "environment": environment,
    "timestamp": `$(date -Iseconds)`,
    "branch": githubRef,
    "actor": githubActor,
    "scripts": compiledScripts
  };

  let manifestFile: string = "${artifactsDir}/deployment-manifest.json";
  fs.writeFile(manifestFile, json.stringify(deploymentManifest, true));

  console.log("✅ Deployment manifest created: ${manifestFile}");
  setOutput("deployment-manifest", manifestFile);
}

// Main CI/CD workflow
function runCICD(): void {
  let success: boolean = true;

  try {
    installCIDependencies();

    let qualityPassed: boolean = runQualityChecks();
    let buildPassed: boolean = buildAndTest();

    if (!qualityPassed || !buildPassed) {
      success = false;
    } else {
      createReleaseArtifacts();
    }

    // Set outputs for subsequent steps
    setOutput("environment", environment);
    setOutput("success", success.toString());
    setOutput("version", githubSha.substring(0, 8));

    // Generate final summary
    let status: string = success ? "✅ SUCCESS" : "❌ FAILED";
    let summary: string = "## CI/CD Pipeline Result: ${status}\n\n";
    summary += "- **Environment**: ${environment}\n";
    summary += "- **Version**: ${githubSha.substring(0, 8)}\n";
    summary += "- **Quality Checks**: ${qualityPassed ? "✅ Passed" : "❌ Failed"}\n";
    summary += "- **Build & Test**: ${buildPassed ? "✅ Passed" : "❌ Failed"}\n";

    addStepSummary(summary);

    if (!success) {
      exit(1);
    }

  } catch {
    console.log("❌ Pipeline failed with errors");
    setOutput("success", "false");
    exit(1);
  }
}

runCICD();
console.log("🎉 CI/CD pipeline completed successfully");
```

## Jenkins Integration

### Jenkins Pipeline Script

```typescript
script.description("Jenkins pipeline integration for Utah projects");

// Jenkins environment variables
let buildNumber: string = env.get("BUILD_NUMBER") || "unknown";
let jobName: string = env.get("JOB_NAME") || "unknown";
let workspace: string = env.get("WORKSPACE") || ".";
let gitBranch: string = env.get("GIT_BRANCH") || "unknown";
let gitCommit: string = env.get("GIT_COMMIT") || "unknown";

console.log("Jenkins Build: ${jobName} #${buildNumber}");
console.log("Branch: ${gitBranch}");
console.log("Commit: ${gitCommit}");
console.log("Workspace: ${workspace}");

// Stage: Checkout and Setup
function setupStage(): void {
  console.log("=== SETUP STAGE ===");

  // Change to workspace directory
  "$(cd ${workspace})";

  // Clean previous artifacts
  if (fs.exists("artifacts")) {
    `$(rm -rf artifacts)`;
  }

  fs.createDirectory("artifacts");

  // Validate workspace
  if (!fs.exists(".git")) {
    console.log("❌ Not a git repository");
    exit(1);
  }

  console.log("✅ Setup completed");
}

// Stage: Build
function buildStage(): void {
  console.log("=== BUILD STAGE ===");

  // Install dependencies
  if (fs.exists("package.json")) {
    `$(npm install)`;
    console.log("✅ NPM dependencies installed");
  }

  // Compile Utah scripts
  let utahFiles: string[] = `$(find . -name "*.shx" -not -path "./node_modules/*")`.split("\n");
  let compilationResults: object[] = [];

  for (let utahFile: string in utahFiles) {
    if (utahFile.trim() != "") {
      console.log("Compiling ${utahFile}...");

      let startTime: number = parseInt(`$(date +%s)`);
      let compileResult: string = "$(utah compile ${utahFile} 2>&1; echo "EXIT_CODE:$?")";
      let endTime: number = parseInt(`$(date +%s)`);

      let exitCode: string = compileResult.split("EXIT_CODE:")[1]?.trim() || "1";
      let duration: number = endTime - startTime;

      let result: object = {
        "file": utahFile,
        "success": exitCode == "0",
        "duration": duration,
        "output": compileResult.split("EXIT_CODE:")[0]
      };

      compilationResults.push(result);

      if (exitCode == "0") {
        console.log("✅ Compiled ${utahFile} (${duration}s)");
      } else {
        console.log("❌ Failed to compile ${utahFile}");
        console.log(compileResult);
      }
    }
  }

  // Generate compilation report
  let reportFile: string = "artifacts/compilation-report.json";
  fs.writeFile(reportFile, json.stringify(compilationResults, true));

  // Check if any compilation failed
  let failedCount: number = 0;
  for (let result: object in compilationResults) {
    if (!json.getBoolean(result, ".success")) {
      failedCount++;
    }
  }

  if (failedCount > 0) {
    console.log("❌ ${failedCount} compilation(s) failed");
    exit(1);
  }

  console.log("✅ Build stage completed");
}

// Stage: Test
function testStage(): void {
  console.log("=== TEST STAGE ===");

  // Run unit tests
  if (fs.exists("package.json")) {
    let packageContent: string = fs.readFile("package.json");
    let packageData: object = json.parse(packageContent);

    if (json.has(packageData, ".scripts.test")) {
      console.log("Running JavaScript tests...");
      `$(npm test)`;
      console.log("✅ JavaScript tests passed");
    }
  }

  // Run Utah script tests
  let testFiles: string[] = `$(find . -name "*.test.shx" -not -path "./node_modules/*")`.split("\n");

  for (let testFile: string in testFiles) {
    if (testFile.trim() != "") {
      console.log("Running ${testFile}...");
      "$(utah ${testFile})";
      console.log("✅ ${testFile} passed");
    }
  }

  console.log("✅ Test stage completed");
}

// Stage: Package
function packageStage(): void {
  console.log("=== PACKAGE STAGE ===");

  // Create distribution package
  let distDir: string = "artifacts/dist";
  fs.createDirectory(distDir);

  // Copy compiled scripts
  let compiledFiles: string[] = `$(find . -name "*.sh" -not -path "./node_modules/*" -not -path "./artifacts/*")`.split("\n");

  for (let file: string in compiledFiles) {
    if (file.trim() != "") {
      let destFile: string = "${distDir}/${fs.filename(file)}";
      fs.copy(file, destFile);
      console.log("📦 Packaged ${file} → ${destFile}");
    }
  }

  // Create version info
  let versionInfo: object = {
    "build_number": buildNumber,
    "job_name": jobName,
    "git_branch": gitBranch,
    "git_commit": gitCommit,
    "build_timestamp": `$(date -Iseconds)`,
    "built_by": "Jenkins"
  };

  fs.writeFile("${distDir}/version.json", json.stringify(versionInfo, true));

  // Create tarball
  let packageName: string = "utah-scripts-${buildNumber}.tar.gz";
  let packagePath: string = "artifacts/${packageName}";

  "$(cd artifacts && tar -czf ${packageName} dist/)";

  if (fs.exists(packagePath)) {
    let packageSize: string = "$(du -h ${packagePath} | cut -f1)";
    console.log("✅ Package created: ${packagePath} (${packageSize})");
  } else {
    console.log("❌ Package creation failed");
    exit(1);
  }

  console.log("✅ Package stage completed");
}

// Stage: Archive
function archiveStage(): void {
  console.log("=== ARCHIVE STAGE ===");

  // Archive artifacts for Jenkins
  let archivePattern: string = "artifacts/**/*";
  console.log("Archiving artifacts: ${archivePattern}");

  // Generate build summary
  let summary: object = {
    "build_info": {
      "number": buildNumber,
      "job": jobName,
      "branch": gitBranch,
      "commit": gitCommit,
      "timestamp": `$(date -Iseconds)`
    },
    "artifacts": {
      "compilation_report": "artifacts/compilation-report.json",
      "distribution": "artifacts/dist/",
      "package": "artifacts/utah-scripts-${buildNumber}.tar.gz"
    },
    "status": "success"
  };

  fs.writeFile("artifacts/build-summary.json", json.stringify(summary, true));

  console.log("✅ Archive stage completed");
}

// Stage: Deploy (conditional)
function deployStage(): void {
  console.log("=== DEPLOY STAGE ===");

  // Only deploy from main/master branch
  if (gitBranch != "origin/main" && gitBranch != "origin/master") {
    console.log("⏭️  Skipping deploy for branch: ${gitBranch}");
    return;
  }

  console.log("Deploying to staging environment...");

  // Extract package for deployment
  let packagePath: string = "artifacts/utah-scripts-${buildNumber}.tar.gz";
  let deployDir: string = "deploy";

  fs.createDirectory(deployDir);
  "$(cd deploy && tar -xzf ../${packagePath})";

  // Deploy scripts (example - copy to deployment location)
  let deploymentPath: string = "/opt/utah-scripts";

  if (fs.exists(deploymentPath)) {
    // Backup current deployment
    let backupPath: string = "${deploymentPath}.backup.${buildNumber}";
    "$(cp -r ${deploymentPath} ${backupPath})";
    console.log("📁 Current deployment backed up to ${backupPath}");
  }

  // Deploy new version
  "$(cp -r deploy/dist/* ${deploymentPath}/ 2>/dev/null || echo "Deploy path not accessible")";

  console.log("✅ Deploy stage completed");
}

// Main pipeline execution
function runPipeline(): void {
  try {
    setupStage();
    buildStage();
    testStage();
    packageStage();
    archiveStage();
    deployStage();

    console.log("🎉 Pipeline completed successfully: ${jobName} #${buildNumber}");

  } catch {
    console.log("❌ Pipeline failed");

    // Generate failure report
    let failureReport: object = {
      "build_number": buildNumber,
      "job_name": jobName,
      "failure_timestamp": `$(date -Iseconds)`,
      "git_info": {
        "branch": gitBranch,
        "commit": gitCommit
      }
    };

    fs.createDirectory("artifacts");
    fs.writeFile("artifacts/failure-report.json", json.stringify(failureReport, true));

    exit(1);
  }
}

runPipeline();
```

## Container Registry Integration

### Docker Registry Management

```typescript
script.description("Manage Docker registry operations for CI/CD");

args.define("--registry", "-r", "Docker registry URL", "string", false, "docker.io");
args.define("--repository", "-p", "Repository name", "string", true);
args.define("--tag", "-t", "Image tag", "string", true);
args.define("--action", "-a", "Action (build|push|pull|scan)", "string", true);

let registry: string = args.getString("--registry");
let repository: string = args.getString("--repository");
let tag: string = args.getString("--tag");
let action: string = args.getString("--action");

let fullImageName: string = "${registry}/${repository}:${tag}";

console.log("Docker Registry Operation: ${action}");
console.log("Image: ${fullImageName}");

// Build Docker image
function buildImage(): void {
  console.log("Building Docker image...");

  if (!fs.exists("Dockerfile")) {
    console.log("❌ Dockerfile not found");
    exit(1);
  }

  // Build image
  "$(docker build -t ${fullImageName} .)";

  // Verify image was built
  let imageExists: string = "$(docker images -q ${fullImageName})";
  if (imageExists.trim() != "") {
    console.log("✅ Image built successfully: ${fullImageName}");

    // Get image size
    let imageSize: string = "$(docker images ${fullImageName} --format "table {{.Size}}" | tail -1)";
    console.log("Image size: ${imageSize}");
  } else {
    console.log("❌ Failed to build image: ${fullImageName}");
    exit(1);
  }
}

// Push image to registry
function pushImage(): void {
  console.log("Pushing image to registry...");

  // Login to registry (assumes credentials are configured)
  if (registry != "docker.io") {
    "$(docker login ${registry})";
  }

  // Push image
  "$(docker push ${fullImageName})";

  // Verify push
  let pushResult: string = "$(docker manifest inspect ${fullImageName} >/dev/null 2>&1 && echo "success" || echo "failed")";
  if (pushResult.trim() == "success") {
    console.log("✅ Image pushed successfully: ${fullImageName}");
  } else {
    console.log("❌ Failed to push image: ${fullImageName}");
    exit(1);
  }
}

// Pull image from registry
function pullImage(): void {
  console.log("Pulling image from registry...");

  "$(docker pull ${fullImageName})";

  // Verify pull
  let imageExists: string = "$(docker images -q ${fullImageName})";
  if (imageExists.trim() != "") {
    console.log("✅ Image pulled successfully: ${fullImageName}");
  } else {
    console.log("❌ Failed to pull image: ${fullImageName}");
    exit(1);
  }
}

// Scan image for vulnerabilities
function scanImage(): void {
  console.log("Scanning image for vulnerabilities...");

  // Check if scanning tool is available
  if (os.isInstalled("trivy")) {
    console.log("Using Trivy for vulnerability scanning...");
    "$(trivy image ${fullImageName})";
  } else if (os.isInstalled("docker-scan")) {
    console.log("Using Docker scan for vulnerability scanning...");
    "$(docker scan ${fullImageName})";
  } else {
    console.log("⚠️  No vulnerability scanner available");
    console.log("Consider installing Trivy or enabling Docker scan");
  }
}

// Execute action
let validActions: string[] = ["build", "push", "pull", "scan"];
if (!validActions.contains(action)) {
  console.log("❌ Invalid action: ${action}");
  console.log("Valid actions: ${validActions.join(", ")}");
  exit(1);
}

if (action == "build") {
  buildImage();
} else if (action == "push") {
  pushImage();
} else if (action == "pull") {
  pullImage();
} else if (action == "scan") {
  scanImage();
}
```

## Best Practices

### Pipeline Error Handling

```typescript
// Robust error handling for CI/CD pipelines
script.exitOnError(false); // Handle errors manually

function runWithRetry(command: string, maxRetries: number = 3): boolean {
  for (let attempt: number = 1; attempt <= maxRetries; attempt++) {
    console.log("Attempt ${attempt}/${maxRetries}: ${command}");

    let result: string = "$(${command} 2>&1; echo "EXIT_CODE:$?")";
    let exitCode: string = result.split("EXIT_CODE:")[1]?.trim() || "1";

    if (exitCode == "0") {
      console.log("✅ Command succeeded on attempt ${attempt}");
      return true;
    } else {
      console.log("❌ Command failed on attempt ${attempt}");
      if (attempt < maxRetries) {
        console.log("Retrying in 10 seconds...");
        `$(sleep 10)`;
      }
    }
  }

  console.log("❌ Command failed after ${maxRetries} attempts");
  return false;
}

// Environment validation
function validateCIEnvironment(): boolean {
  let requiredVars: string[] = ["CI", "BUILD_NUMBER", "GIT_COMMIT"];

  for (let varName: string in requiredVars) {
    let value: string = env.get(varName) || "";
    if (value == "") {
      console.log("❌ Required environment variable missing: ${varName}");
      return false;
    }
  }

  return true;
}
```

### Performance Optimization

```typescript
// Cache management for faster builds
function manageBuildCache(): void {
  let cacheDir: string = ".utah-cache";
  fs.createDirectory(cacheDir);

  // Cache compiled scripts
  let sourceFiles: string[] = "$(find . -name "*.shx" -newer ${cacheDir}/last-build 2>/dev/null || find . -name "*.shx")".split("\n");

  if (sourceFiles.length == 1 && sourceFiles[0].trim() == "") {
    console.log("✅ No sources changed, using cache");
    return;
  }

  console.log("Building ${sourceFiles.length} changed files...");

  // Update cache timestamp
  "$(touch ${cacheDir}/last-build)";
}
```

## Next Steps

- **[Docker Integration](docker.md)** - Containerize Utah applications
- **[Cloud Automation](cloud.md)** - Deploy to cloud platforms
- **[Performance Optimization](performance.md)** - Optimize CI/CD pipelines

CI/CD integration with Utah enables powerful automation workflows while maintaining flexibility and reliability across different platforms and environments.
