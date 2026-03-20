---
layout: default
title: Docker Functions
parent: Language Features
nav_order: 7
---

Utah provides Docker integration functions for container and image management directly from your shell scripts.

## Available Functions

### Container Management

| Function | Returns | Description |
|---|---|---|
| `docker.run(image, name?, ports?, volumes?)` | string | Run a container in detached mode |
| `docker.stop(container)` | void | Stop a running container |
| `docker.remove(container)` | void | Remove a container |
| `docker.restart(container)` | void | Restart a container |
| `docker.logs(container)` | string | Get container logs |
| `docker.exec(container, command)` | string | Execute a command in a running container |
| `docker.isRunning(container)` | boolean | Check if a container is running |
| `docker.list()` | string | List running container names |

### Image Management

| Function | Returns | Description |
|---|---|---|
| `docker.build(tag, path?)` | string | Build an image from a Dockerfile |
| `docker.pull(image)` | void | Pull an image from registry |
| `docker.push(image)` | void | Push an image to registry |
| `docker.removeImage(image)` | void | Remove a Docker image |
| `docker.imageExists(image)` | boolean | Check if an image exists locally |

## Container Management

### docker.run()

Runs a Docker container in detached mode. Returns the container ID.

```typescript
// Run with just an image
docker.run("nginx");

// Run with a name
docker.run("nginx", "my-nginx");

// Run with name and port mapping
docker.run("nginx", "web-server", "8080:80");

// Run with all options
docker.run("nginx", "full-server", "8080:80", "/data:/usr/share/nginx/html");

// Capture the container ID
let containerId: string = docker.run("redis", "my-redis");
console.log("Started container: ${containerId}");
```

**Generated Bash:**

```bash
# docker.run("nginx") becomes:
docker run -d nginx

# docker.run("nginx", "web-server", "8080:80") becomes:
docker run -d --name web-server -p 8080:80 nginx

# docker.run("nginx", "full-server", "8080:80", "/data:/usr/share/nginx/html") becomes:
docker run -d --name full-server -p 8080:80 -v /data:/usr/share/nginx/html nginx
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_run.shx`
- Tests all parameter combinations including variable capture

### docker.stop()

Stops a running container.

```typescript
// Stop by name
docker.stop("my-nginx");

// Stop using a variable
let container: string = "web-server";
docker.stop(container);
```

**Generated Bash:**

```bash
# docker.stop("my-nginx") becomes:
docker stop "my-nginx"

# docker.stop(container) with a variable becomes:
docker stop ${container}
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_stop_remove.shx`

### docker.remove()

Removes a container.

```typescript
// Remove by name
docker.remove("my-nginx");

// Stop then remove
docker.stop("web-server");
docker.remove("web-server");
```

**Generated Bash:**

```bash
# docker.remove("my-nginx") becomes:
docker rm "my-nginx"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_stop_remove.shx`

### docker.restart()

Restarts a container.

```typescript
docker.restart("my-nginx");

let container: string = "web-server";
docker.restart(container);
```

**Generated Bash:**

```bash
# docker.restart("my-nginx") becomes:
docker restart "my-nginx"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_restart.shx`

### docker.logs()

Returns the logs of a container as a string.

```typescript
let output: string = docker.logs("my-nginx");
console.log("Logs: ${output}");

let container: string = "web-server";
let logs: string = docker.logs(container);
```

**Generated Bash:**

```bash
# docker.logs("my-nginx") becomes:
output=$(docker logs "my-nginx")
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_logs.shx`

### docker.exec()

Executes a command inside a running container and returns the output.

```typescript
let result: string = docker.exec("my-nginx", "ls /usr/share/nginx/html");
console.log("Files: ${result}");

let container: string = "web-server";
docker.exec(container, "nginx -s reload");
```

**Generated Bash:**

```bash
# docker.exec("my-nginx", "ls /usr/share/nginx/html") becomes:
result=$(docker exec "my-nginx" ls /usr/share/nginx/html)
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_exec.shx`

### docker.isRunning()

Returns a boolean indicating whether a container is currently running.

```typescript
let running: boolean = docker.isRunning("my-nginx");

if (running) {
  console.log("Container is running");
} else {
  console.log("Container is not running");
}
```

**Generated Bash:**

```bash
# docker.isRunning("my-nginx") becomes:
running=$(docker inspect -f '{{.State.Running}}' "my-nginx" 2>/dev/null || echo "false")

# In an if condition:
if [ "${running}" = "true" ]; then
  echo "Container is running"
fi
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_is_running.shx`

### docker.list()

Returns the names of all running containers.

```typescript
let containers: string = docker.list();
console.log("Running containers: ${containers}");
```

**Generated Bash:**

```bash
# docker.list() becomes:
containers=$(docker ps --format '{{.Names}}')
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_list.shx`

## Image Management

### docker.build()

Builds a Docker image from a Dockerfile. Defaults to the current directory if no path is specified.

```typescript
// Build with tag (uses current directory)
docker.build("myapp:latest");

// Build with tag and path
docker.build("myapp:v2", "./app");

// Capture build output
let result: string = docker.build("myapp:v3");
```

**Generated Bash:**

```bash
# docker.build("myapp:latest") becomes:
docker build -t "myapp:latest" .

# docker.build("myapp:v2", "./app") becomes:
docker build -t "myapp:v2" "./app"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_build.shx`

### docker.pull()

Pulls a Docker image from a registry.

```typescript
docker.pull("nginx:latest");

let image: string = "redis:alpine";
docker.pull(image);
```

**Generated Bash:**

```bash
# docker.pull("nginx:latest") becomes:
docker pull "nginx:latest"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_pull_push.shx`

### docker.push()

Pushes a Docker image to a registry.

```typescript
docker.push("myregistry/myapp:latest");
```

**Generated Bash:**

```bash
# docker.push("myregistry/myapp:latest") becomes:
docker push "myregistry/myapp:latest"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_pull_push.shx`

### docker.removeImage()

Removes a Docker image from the local system.

```typescript
docker.removeImage("nginx:latest");
```

**Generated Bash:**

```bash
# docker.removeImage("nginx:latest") becomes:
docker rmi "nginx:latest"
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_image_management.shx`

### docker.imageExists()

Returns a boolean indicating whether a Docker image exists locally.

```typescript
let exists: boolean = docker.imageExists("nginx:latest");

if (exists) {
  console.log("Image exists");
  docker.removeImage("nginx:latest");
} else {
  console.log("Image not found");
}
```

**Generated Bash:**

```bash
# docker.imageExists("nginx:latest") becomes:
exists=$(docker image inspect "nginx:latest" > /dev/null 2>&1 && echo "true" || echo "false")
```

**Test Coverage:**

- File: `tests/positive_fixtures/docker_image_management.shx`

## Practical Examples

### Deploy a Web Application

```typescript
let imageName: string = "myapp:latest";

// Build the image
docker.build(imageName);

// Stop and remove old container if running
let running: boolean = docker.isRunning("myapp");
if (running) {
  docker.stop("myapp");
  docker.remove("myapp");
}

// Run the new container
docker.run(imageName, "myapp", "3000:3000");
console.log("Application deployed successfully");
```

### Container Health Check

```typescript
let containers: string[] = ["web", "api", "db"];

for (let name: string in containers) {
  let running: boolean = docker.isRunning(name);
  if (running) {
    console.log("✓ ${name} is running");
  } else {
    console.log("✗ ${name} is NOT running");
    docker.restart(name);
    console.log("  Restarted ${name}");
  }
}
```

### CI/CD Pipeline

```typescript
let version: string = "1.0.0";
let registry: string = "myregistry.io";

// Build
docker.build("${registry}/myapp:${version}");

// Push to registry
docker.push("${registry}/myapp:${version}");

console.log("Published ${registry}/myapp:${version}");
```
