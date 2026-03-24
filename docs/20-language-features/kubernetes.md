---
layout: default
title: Kubernetes Functions
parent: Language Features
nav_order: 7
---

Utah provides Kubernetes integration functions for managing clusters, workloads, and resources directly from your shell scripts. These functions wrap `kubectl` commands into clean, type-safe operations.

## Available Functions

### Cluster & Context

| Function | Returns | Description |
|---|---|---|
| `k8s.getContext()` | string | Get the current kubectl context name |
| `k8s.setContext(name)` | void | Switch to a different kubectl context |
| `k8s.getNamespace()` | string | Get the current namespace (defaults to "default") |
| `k8s.setNamespace(name)` | void | Set the active namespace for the current context |
| `k8s.clientVersion()` | string | Get the kubectl client version |
| `k8s.serverVersion()` | string | Get the Kubernetes server version |

### Resource Operations

| Function | Returns | Description |
|---|---|---|
| `k8s.get(resource, name?, namespace?)` | string | Get resource(s) as JSON output |
| `k8s.describe(resource, name, namespace?)` | string | Get detailed description of a resource |
| `k8s.exists(resource, name, namespace?)` | boolean | Check if a resource exists |

### Pod Operations

| Function | Returns | Description |
|---|---|---|
| `k8s.logs(pod, container?, tail?, previous?)` | string | Get pod logs with optional filtering |
| `k8s.exec(pod, command, container?)` | string | Execute a command inside a pod |
| `k8s.portForward(pod, localPort, remotePort)` | string | Forward a local port to a pod (returns PID) |
| `k8s.isReady(pod, namespace?)` | boolean | Check if a pod is in Ready state |

### Scaling

| Function | Returns | Description |
|---|---|---|
| `k8s.scale(resource, name, replicas)` | void | Scale a resource to the specified number of replicas |

### Secrets

| Function | Returns | Description |
|---|---|---|
| `k8s.getSecret(name, key?, namespace?)` | string | Get a secret (full JSON or base64-decoded key value) |
| `k8s.setSecret(name, key, value)` | void | Create a generic secret with a key-value pair |

### Monitoring

| Function | Returns | Description |
|---|---|---|
| `k8s.topPods(namespace?)` | string | Show resource usage (CPU/memory) for pods |
| `k8s.topNodes()` | string | Show resource usage (CPU/memory) for nodes |

## Cluster & Context

### k8s.getContext()

Returns the name of the current kubectl context.

```typescript
let ctx: string = k8s.getContext();
console.log("Current context: ${ctx}");
```

**Generated Bash:**

```bash
# k8s.getContext() becomes:
ctx=$(kubectl config current-context)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_context.shx`

### k8s.setContext()

Switches to a different kubectl context.

```typescript
// Switch to production context
k8s.setContext("production");

// Switch using a variable
let targetCtx: string = "staging";
k8s.setContext(targetCtx);
```

**Generated Bash:**

```bash
# k8s.setContext("production") becomes:
kubectl config use-context "production"

# k8s.setContext(targetCtx) with a variable becomes:
kubectl config use-context ${targetCtx}
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_context.shx`

### k8s.getNamespace()

Returns the current namespace. Falls back to `"default"` if no namespace is explicitly set.

```typescript
let ns: string = k8s.getNamespace();
console.log("Namespace: ${ns}");
```

**Generated Bash:**

```bash
# k8s.getNamespace() becomes:
ns=$(kubectl config view --minify --output 'jsonpath={..namespace}' 2>/dev/null || echo "default")
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_context.shx`

### k8s.setNamespace()

Sets the active namespace for the current context.

```typescript
k8s.setNamespace("kube-system");

let targetNs: string = "monitoring";
k8s.setNamespace(targetNs);
```

**Generated Bash:**

```bash
# k8s.setNamespace("kube-system") becomes:
kubectl config set-context --current --namespace="kube-system"
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_context.shx`

### k8s.clientVersion()

Returns the kubectl client version string.

```typescript
let clientVer: string = k8s.clientVersion();
console.log("Client: ${clientVer}");
```

**Generated Bash:**

```bash
# k8s.clientVersion() becomes:
clientVer=$(kubectl version --client -o json 2>/dev/null | jq -r '.clientVersion.gitVersion')
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_version.shx`

### k8s.serverVersion()

Returns the Kubernetes server version string.

```typescript
let serverVer: string = k8s.serverVersion();
console.log("Server: ${serverVer}");
```

**Generated Bash:**

```bash
# k8s.serverVersion() becomes:
serverVer=$(kubectl version -o json 2>/dev/null | jq -r '.serverVersion.gitVersion')
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_version.shx`

## Resource Operations

### k8s.get()

Gets resource(s) as JSON output. Supports optional name and namespace filtering.

```typescript
// Get all pods
let allPods: string = k8s.get("pods");

// Get a specific pod
let pod: string = k8s.get("pods", "my-pod");

// Get a resource in a specific namespace
let svc: string = k8s.get("services", "api-gateway", "production");

// Using variables
let resourceType: string = "deployments";
let resourceName: string = "web-app";
let result: string = k8s.get(resourceType, resourceName);
```

**Generated Bash:**

```bash
# k8s.get("pods") becomes:
allPods=$(kubectl get "pods" -o json)

# k8s.get("pods", "my-pod") becomes:
pod=$(kubectl get "pods" "my-pod" -o json)

# k8s.get("services", "api-gateway", "production") becomes:
svc=$(kubectl get "services" "api-gateway" -n "production" -o json)

# k8s.get(resourceType, resourceName) with variables becomes:
result=$(kubectl get ${resourceType} ${resourceName} -o json)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_resources.shx`

### k8s.describe()

Gets a detailed description of a resource.

```typescript
// Describe a pod
let desc: string = k8s.describe("pod", "my-pod");

// Describe with namespace
let details: string = k8s.describe("deployment", "web-app", "staging");
```

**Generated Bash:**

```bash
# k8s.describe("pod", "my-pod") becomes:
desc=$(kubectl describe "pod" "my-pod")

# k8s.describe("deployment", "web-app", "staging") becomes:
details=$(kubectl describe "deployment" "web-app" -n "staging")
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_resources.shx`

### k8s.exists()

Returns a boolean indicating whether a resource exists. Works directly in `if` conditions.

```typescript
let exists: boolean = k8s.exists("pod", "my-pod");
if (exists) {
  console.log("Pod exists");
}

// With namespace
if (!k8s.exists("service", "api-svc", "default")) {
  console.log("Service not found");
}
```

**Generated Bash:**

```bash
# k8s.exists("pod", "my-pod") becomes:
exists=$(kubectl get "pod" "my-pod" > /dev/null 2>&1 && echo "true" || echo "false")

# In an if condition:
if [ "${exists}" = "true" ]; then
  echo "Pod exists"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_resources.shx`

## Pod Operations

### k8s.logs()

Returns pod logs with optional container, tail, and previous container filtering.

```typescript
// Basic pod logs
let output: string = k8s.logs("my-pod");

// Logs from a specific container
let containerLogs: string = k8s.logs("my-pod", "nginx");

// Tail the last 100 lines
let tailedLogs: string = k8s.logs("my-pod", "nginx", 100);

// Get previous container logs (useful for crash debugging)
let prevLogs: string = k8s.logs("my-pod", "sidecar", 50, true);

// Using variables
let podName: string = "api-server";
let containerName: string = "app";
let logs: string = k8s.logs(podName, containerName);
```

**Generated Bash:**

```bash
# k8s.logs("my-pod") becomes:
output=$(kubectl logs "my-pod")

# k8s.logs("my-pod", "nginx", 100) becomes:
tailedLogs=$(kubectl logs "my-pod" --container="nginx" --tail=100)

# k8s.logs("my-pod", "sidecar", 50, true) becomes:
prevLogs=$(kubectl logs "my-pod" --container="sidecar" --tail=50 --previous)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_logs.shx`

### k8s.exec()

Executes a command inside a pod. Supports optional container targeting.

```typescript
// Execute a command
let result: string = k8s.exec("my-pod", "ls /app");

// Execute in a specific container
let config: string = k8s.exec("my-pod", "cat /etc/config", "nginx");

// Execute as a statement (no return value needed)
k8s.exec("my-pod", "nginx -s reload");

// Using variables
let pod: string = "web-server";
let cmd: string = "whoami";
k8s.exec(pod, cmd);
```

**Generated Bash:**

```bash
# k8s.exec("my-pod", "ls /app") becomes:
result=$(kubectl exec "my-pod" -- ls /app)

# k8s.exec("my-pod", "cat /etc/config", "nginx") becomes:
config=$(kubectl exec "my-pod" --container="nginx" -- cat /etc/config)

# k8s.exec("my-pod", "nginx -s reload") as statement becomes:
kubectl exec "my-pod" -- nginx -s reload
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_exec.shx`

### k8s.portForward()

Forwards a local port to a pod. Runs in the background and returns the PID when used as an expression.

```typescript
// Port forward as a statement (runs in background)
k8s.portForward("my-pod", 8080, 80);

// Port forward and capture PID
let pid: string = k8s.portForward("api-pod", 3000, 8080);
console.log("Port forward PID: ${pid}");
```

**Generated Bash:**

```bash
# k8s.portForward("my-pod", 8080, 80) as statement becomes:
kubectl port-forward "my-pod" 8080:80 > /dev/null 2>&1 &

# k8s.portForward("api-pod", 3000, 8080) as expression becomes:
pid=$(kubectl port-forward "api-pod" 3000:8080 > /dev/null 2>&1 & echo $!)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_port_forward.shx`

### k8s.isReady()

Returns a boolean indicating whether a pod is in Ready state. Works directly in `if` conditions.

```typescript
let ready: boolean = k8s.isReady("my-pod");
if (ready) {
  console.log("Pod is ready");
}

// With namespace
if (!k8s.isReady("api-pod", "production")) {
  console.log("Pod is not ready yet");
}
```

**Generated Bash:**

```bash
# k8s.isReady("my-pod") becomes:
ready=$(kubectl get pod "my-pod" -o jsonpath='{.status.conditions[?(@.type=="Ready")].status}' 2>/dev/null | grep -qi "true" && echo "true" || echo "false")

# In an if condition:
if [ "${ready}" = "true" ]; then
  echo "Pod is ready"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_is_ready.shx`

## Scaling

### k8s.scale()

Scales a resource to the specified number of replicas.

```typescript
// Scale a deployment
k8s.scale("deployment", "web-app", 3);

// Scale using variables
let resource: string = "statefulset";
let name: string = "database";
let replicas: number = 5;
k8s.scale(resource, name, replicas);

// Capture output
let result: string = k8s.scale("deployment", "api-server", 2);
```

**Generated Bash:**

```bash
# k8s.scale("deployment", "web-app", 3) becomes:
kubectl scale deployment/web-app --replicas=3

# k8s.scale(resource, name, replicas) with variables becomes:
kubectl scale ${resource}/${name} --replicas=${replicas}
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_scale.shx`

## Secrets

### k8s.getSecret()

Gets a Kubernetes secret. Returns full JSON when called with just a name, or the base64-decoded value of a specific key.

```typescript
// Get full secret as JSON
let secretData: string = k8s.getSecret("my-secret");

// Get a specific key (automatically base64 decoded)
let password: string = k8s.getSecret("db-credentials", "password");

// Get a secret key from a specific namespace
let token: string = k8s.getSecret("api-key", "token", "production");
```

**Generated Bash:**

```bash
# k8s.getSecret("my-secret") becomes:
secretData=$(kubectl get secret "my-secret" -o json)

# k8s.getSecret("db-credentials", "password") becomes:
password=$(kubectl get secret "db-credentials" -o jsonpath='{.data.password}' | base64 -d)

# k8s.getSecret("api-key", "token", "production") becomes:
token=$(kubectl get secret "api-key" -n "production" -o jsonpath='{.data.token}' | base64 -d)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_secrets.shx`

### k8s.setSecret()

Creates a generic Kubernetes secret with a key-value pair.

```typescript
// Create a secret
k8s.setSecret("my-secret", "api-key", "abc123");

// Create using variables
let secretName: string = "db-creds";
let key: string = "password";
let value: string = "s3cret";
k8s.setSecret(secretName, key, value);
```

**Generated Bash:**

```bash
# k8s.setSecret("my-secret", "api-key", "abc123") becomes:
kubectl create secret generic "my-secret" --from-literal=api-key=abc123

# k8s.setSecret(secretName, key, value) with variables becomes:
kubectl create secret generic ${secretName} --from-literal=${key}=${value}
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_secrets.shx`

## Monitoring

### k8s.topPods()

Shows resource usage (CPU/memory) for pods. Supports optional namespace filtering.

```typescript
// Get pod resource usage
let podUsage: string = k8s.topPods();
console.log("Pod usage: ${podUsage}");

// Get pod usage in a specific namespace
let nsUsage: string = k8s.topPods("kube-system");

// Using a variable
let ns: string = "monitoring";
let monUsage: string = k8s.topPods(ns);
```

**Generated Bash:**

```bash
# k8s.topPods() becomes:
podUsage=$(kubectl top pods)

# k8s.topPods("kube-system") becomes:
nsUsage=$(kubectl top pods -n "kube-system")
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_monitoring.shx`

### k8s.topNodes()

Shows resource usage (CPU/memory) for cluster nodes.

```typescript
let nodeUsage: string = k8s.topNodes();
console.log("Node usage: ${nodeUsage}");
```

**Generated Bash:**

```bash
# k8s.topNodes() becomes:
nodeUsage=$(kubectl top nodes)
```

**Test Coverage:**

- File: `tests/positive_fixtures/k8s_monitoring.shx`

## Practical Examples

### Deployment Script

```typescript
let env: string = "production";

// Switch to the right context and namespace
k8s.setContext(env);
k8s.setNamespace("app");

// Check if old deployment exists
if (k8s.exists("deployment", "web-app")) {
  // Scale down before updating
  k8s.scale("deployment", "web-app", 0);
}

// Scale up the new deployment
k8s.scale("deployment", "web-app", 3);

// Wait and verify readiness
let ready: boolean = k8s.isReady("web-app-pod");
if (ready) {
  console.log("Deployment successful");
} else {
  console.log("Deployment may need attention");
  let logs: string = k8s.logs("web-app-pod");
  console.log("Logs: ${logs}");
}
```

### Cluster Health Check

```typescript
let ctx: string = k8s.getContext();
console.log("Checking cluster: ${ctx}");

let clientVer: string = k8s.clientVersion();
let serverVer: string = k8s.serverVersion();
console.log("Client: ${clientVer}, Server: ${serverVer}");

// Check critical pods
let pods: string[] = ["api-server", "database", "cache"];
for (let pod: string in pods) {
  if (k8s.isReady(pod)) {
    console.log("✓ ${pod} is ready");
  } else {
    console.log("✗ ${pod} is NOT ready");
    let logs: string = k8s.logs(pod, "", 20);
    console.log("Recent logs: ${logs}");
  }
}

// Show resource usage
let nodeUsage: string = k8s.topNodes();
console.log("Node resources: ${nodeUsage}");
```

### Secret Rotation

```typescript
let secretName: string = "api-credentials";
let ns: string = "production";

// Get current secret value
let currentKey: string = k8s.getSecret(secretName, "api-key", ns);
console.log("Current key starts with: ${currentKey}");

// Set new secret
let newKey: string = utility.uuid();
k8s.setSecret(secretName, "api-key", newKey);
console.log("Secret rotated successfully");

// Restart the deployment to pick up new secret
k8s.scale("deployment", "api-server", 0);
k8s.scale("deployment", "api-server", 3);
```

### Debug a Pod

```typescript
let podName: string = "web-server";
let ns: string = "staging";

// Check if pod exists and is ready
if (!k8s.exists("pod", podName, ns)) {
  console.log("Pod ${podName} not found in ${ns}");
  exit(1);
}

let ready: boolean = k8s.isReady(podName, ns);
console.log("Pod ready: ${ready}");

// Get recent logs
let logs: string = k8s.logs(podName, "", 50);
console.log("Recent logs: ${logs}");

// Check resource usage
let usage: string = k8s.topPods(ns);
console.log("Resource usage: ${usage}");

// Port forward for local debugging
let pid: string = k8s.portForward(podName, 8080, 80);
console.log("Port forwarding on localhost:8080 (PID: ${pid})");

// Exec into the pod for investigation
let env: string = k8s.exec(podName, "env");
console.log("Environment: ${env}");
```

## Technical Notes

- All functions require `kubectl` to be installed and configured
- `k8s.clientVersion()` and `k8s.serverVersion()` require `jq` for JSON parsing
- `k8s.getNamespace()` falls back to `"default"` if no namespace is set
- `k8s.portForward()` runs in the background using `&` and redirects output to `/dev/null`
- `k8s.isReady()` checks the pod's `Ready` condition status via jsonpath
- `k8s.getSecret()` with a key automatically pipes through `base64 -d` for decoding
- `k8s.exists()` and `k8s.isReady()` work as boolean expressions in `if` conditions, including negation with `!`
