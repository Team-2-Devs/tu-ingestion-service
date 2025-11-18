# DEV.md – Local Development Guide (Ingestion Service)

This guide explains how to run the **Trackunit Ingestion Service** locally, connect it to the Storage service and Redpanda, and perform an end-to-end smoke test of the upload flow (`/start → PUT → /confirm`).

Two modes are supported:

* **Option A (recommended): Docker Compose** – runs Ingestion.Api in a container, with SQLite DB created automatically.
* **Option B: `dotnet run`** – runs Ingestion.Api from your IDE/CLI for debugging.

Use only one mode at a time.

---

## 1. Prerequisites

* Docker Desktop
* .NET 8 SDK
* curl (Git Bash or PowerShell 7+)
* Access to the team Redpanda broker via Tailscale

Check installation:

```
docker version
dotnet --version
```

---

## 2. Dependencies

The Ingestion service depends on:

* **Storage service** (`tu-storage-service`)

  * Provides presigned PUT/GET URLs via `/internal/v1/storage`.
  * Uses MinIO as S3-compatible object storage.
* **Redpanda broker** (Kafka-compatible)

  * Ingestion publishes `images.uploaded.v0` events after `/confirm`.

Make sure you have cloned:

* `tu-storage-service`
* `tu-ingestion-service` (this repo)

---

## 3. Configuration Overview

### 3.1 Docker Compose – `.env`

In this repository, Docker Compose reads configuration from a local `.env` file.

* `.env` is **not committed**.
* `.env.example` contains safe defaults.
* Docker Compose injects these as environment variables into the container.

Keys used:

* `REDPANDA_BOOTSTRAP_SERVERS` – address of the Redpanda broker (e.g. `100.97.224.69:9092`).
* `INTERNAL_AUTH_API_KEY` – internal token used when calling Storage.Api.

  * Must match `InternalAuth:ApiKey` in `tu-storage-service`.
* `STORAGE_BASE_URL` – base URL for Storage.Api as seen from inside the ingestion container.

  * For local dev with Storage running on the host, this is  
`http://host.docker.internal:8080`.

### 3.2 `dotnet run` – appsettings

When you run the API via `dotnet run`, configuration is loaded from:

* `appsettings.json`
* `appsettings.Development.json`
* Environment variables
* (Optionally) user secrets

For local development, defaults in `appsettings.json` are usually enough. Override if needed using either:

* `appsettings.Development.json` (checked in), or
* User secrets (not checked in).

---

## 4. Option A – Run Ingestion via Docker Compose (Recommended)

### 4.1 Start the Storage service

Follow `tu-storage-service/docs/DEV.md` and start Storage + MinIO:

```
cd ../tu-storage-service
docker compose up --build
```

Verify:

* Storage.Api: `curl http://localhost:8080/health`.
* MinIO console: `http://localhost:9001`.
* Bucket `trackunit-images` exists.

### 4.2 Create `.env` from template

In the ingestion repo:

```
cd ../tu-ingestion-service
cp .env.example .env
```

Edit `.env` and adjust if needed:

```
REDPANDA_BOOTSTRAP_SERVERS=100.97.224.69:9092
INTERNAL_AUTH_API_KEY=changeme
STORAGE_BASE_URL=http://host.docker.internal:8080
```

Notes:

* `INTERNAL_AUTH_API_KEY` must equal the value used by Storage.Api.
* `REDPANDA_BOOTSTRAP_SERVERS` must be reachable over Tailscale.

### 4.3 Start Ingestion

From the ingestion repo root:

```
docker compose up --build
```

This will:

* Build the Ingestion API image from `src/Ingestion.Api/Dockerfile`.
* Start Ingestion.Api on port `8090` (host) → `8080` (container).
* Create or migrate the SQLite database at `/app/data/ingestion.dev.db`.

### 4.4 Health check

```
curl http://localhost:8090/health
```

Expected:

```
Healthy
```

### 4.5 Stop the environment

```
docker compose down
```

To reset the database:

```
docker compose down -v
```

This removes the `ingestion-data` volume. Next startup will create a fresh database.

---

## 5. Option B – Run Ingestion via `dotnet run` (Debug Mode)

### 5.1 Start Storage + MinIO

From the storage repo:

```
cd ../tu-storage-service
docker compose up --build
```

Ensure:

* Storage.Api is reachable.
* Bucket `trackunit-images` exists.

### 5.2 Ensure Redpanda is reachable

Be connected to the team Tailscale network.

Optional connectivity test:

```
rpk cluster info -X brokers=100.97.224.69:9092
```

### 5.3 Configuration for `dotnet run`

Defaults in `appsettings.json` include:

* `Messaging:Redpanda:BootstrapServers`.
* `ConnectionStrings:IngestionDb`.
* `Storage:BaseUrl`.
* `Storage:InternalAccess`.

Adjust them via:

* `appsettings.Development.json`, or
* User secrets.

### 5.4 Run the API

```
dotnet run --project src/Ingestion.Api
```

SQLite migrations are applied automatically at startup.

Health check:

```
curl http://localhost:5104/health
```

---

## 6. Smoke Test – Upload Flow (UC1)

### 6.1 Start a new upload

```
curl -v -X POST "http://localhost:8090/v1/uploads/start" \
  -H "Content-Type: application/json" \
  -d '{"filename":"sample.jpg","contentType":"image/jpeg"}'
```

Expected response:

```
{
  "uploadId": "<guid>",
  "key": "images/...",
  "putUrl": "http://localhost:9000/trackunit-images/...",
  "expiresAt": "<timestamp>"
}
```

### 6.2 Upload file to MinIO

Git Bash:

```
curl -T "$HOME/Desktop/sample.jpg" \
  -H "Content-Type: image/jpeg" \
  "<paste putUrl>"
```

PowerShell:

```
curl.exe -T "$HOME\Desktop\sample.jpg" `
  -H "Content-Type: image/jpeg" `
  "<paste putUrl>"
```

### 6.3 Compute checksum and byte length

Git Bash:

```
CHECKSUM="sha256:$(sha256sum "$HOME/Desktop/sample.jpg" | awk '{print $1}')"
BYTES=$(wc -c < "$HOME/Desktop/sample.jpg" | tr -d ' ')
echo $CHECKSUM
echo $BYTES
```

PowerShell:

```
$bytes = (Get-Item "$HOME\Desktop\sample.jpg").Length
$hash = (Get-FileHash "$HOME\Desktop\sample.jpg" -Algorithm SHA256).Hash.ToLower()
$checksum = "sha256:$hash"
$bytes
$checksum
```

### 6.4 Confirm the upload

```
curl -v -X POST "http://localhost:8090/v1/uploads/confirm" \
  -H "Content-Type: application/json" \
  -d "{\"uploadId\":\"<id>\",\"bytes\":$BYTES,\"checksum\":\"$CHECKSUM\"}"
```

Expected:

```
{"status":"Accepted"}
```

## 7. Verification

### 7.1 MinIO

In the MinIO console (http://localhost:9001):

* Open the `trackunit-images` bucket.
* Confirm the uploaded object exists under the expected key.

### 7.2 SQLite database

In the ingestion SQLite database:

* Path (Docker Compose): in the `ingestion-data` volume, mounted as `/app/data/ingestion.dev.db`.
* Path (dotnet run): as configured in `ConnectionStrings:IngestionDb`.

Open the DB with a SQLite tool and check:

* A row exists in `UploadSessions` for the `uploadId`.
* `Status` = `Uploaded`.
* `Bytes` and `Checksum` are populated.

### 7.3 Redpanda (optional)

If you have `rpk` and access to the broker:

```
rpk topic consume tu.images.uploaded -X brokers=100.97.224.69:9092
```

You should see an event for the confirmed upload.

---

## 8. Troubleshooting

| Problem                          | Cause                               | Resolution                                          |
| -------------------------------- | ----------------------------------- | --------------------------------------------------- |
| `Healthy` not returned           | API not running or wrong port       | Check Docker containers or `dotnet run` output      |
| `Request has expired`            | Presigned URL TTL elapsed           | Re-run `/start` and repeat the flow                 |
| `403 SignatureDoesNotMatch`      | Content-Type mismatch on PUT        | Use the same `Content-Type` as in `/start` response |
| `404 upload not found`           | Wrong or expired `uploadId`         | Start a new upload, use the new `uploadId`          |
| `Bucket does not exist`          | `trackunit-images` missing in MinIO | Create the bucket in the MinIO console              |
| Kafka/Redpanda errors on confirm | Broker unreachable or misconfigured | Check `REDPANDA_BOOTSTRAP_SERVERS` and Tailscale    |

---

## 9. Developer Checklist

* [ ] `.env` created from `.env.example` (Docker mode).
* [ ] Connected to Tailscale and Redpanda reachable.
* [ ] Storage.Api running on http://localhost:8080.
* [ ] MinIO console reachable on http://localhost:9001.
* [ ] Ingestion.Api reachable on http://localhost:8090.
* [ ] Database file created and migrations applied.
* [ ] `/v1/uploads/start` returns a valid upload payload.
* [ ] PUT upload to MinIO succeeds.
* [ ] `/v1/uploads/confirm` returns `{"status":"Accepted"}`.
* [ ] Uploaded object visible in MinIO.
* [ ] Upload session row present in SQLite.
* [ ] (Optional) Event visible on `tu.images.uploaded` topic in Redpanda.

---

## Reference
For service overview and related services, see [README.md](../README.md).

---

**Updated:** November 2025 (11/18)
