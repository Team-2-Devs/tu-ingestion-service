# Developer Guide – Ingestion Service

This document explains how to run the **Trackunit Ingestion Service** locally, connect it to the Storage service and MinIO, and perform a full smoke test verifying the end-to-end upload flow (`/start → PUT → /confirm`).

---

## 1. Prerequisites

- **Docker Desktop** (WSL2 + Virtual Machine Platform enabled)  
- **.NET 8 SDK**  
- Optional: `curl` (included with Git Bash / PowerShell 7+)  
- Optional: SQLite viewer (e.g. *DB Browser for SQLite*)

Check installation:
```bash
docker version
dotnet --version
```

---

## 2. Local infrastructure (MinIO + Storage)

> Ensure Docker Desktop is running before proceeding.

1. **Start MinIO**
   ```bash
   docker compose up -d
   ```

   Verify:
   ```bash
   docker ps
   ```

   Expected output includes:
   ```
   minio/minio:latest   0.0.0.0:9000-9001->9000-9001/tcp
   ```

2. **Access MinIO Console**
   - URL: http://localhost:9001  
   - Username: `minioadmin`  
   - Password: `minioadmin`

3. **Default bucket**  
   `trackunit-images`  
   (Create it once if missing.)

4. **Run the Storage service**  
   (Needed for `/internal/v1/storage/presign-put`.)
   ```bash
   dotnet run --project ../tu-storage-service/src/Storage.Api
   ```
   Confirm it listens on:  
   `http://localhost:5136`

---

## 3. Run the Ingestion API

Start the service:
```bash
dotnet run --project src/Ingestion.Api
```

Health check:
```bash
curl -s http://localhost:5104/health
```

Expected output:
```
Healthy
```

---

## 4. Configuration

Edit `src/Ingestion.Api/appsettings.Development.json` if needed:

```json
{
  "ConnectionStrings": {
    "IngestionDb": "Data Source=../Ingestion.Infrastructure/Persistence/EFCore/Data/Ingestion.db"
  },
  "Storage": {
    "BaseUrl": "http://localhost:5136",
    "InternalToken": "dev-secret-only"
  }
}
```

---

## 5. Database initialization

First-time only:
```bash
dotnet ef database update --project src/Ingestion.Infrastructure --startup-project src/Ingestion.Api
```

---

## 6. Smoke test (UC1 – Upload Flow)

### Step 1 – Start a new upload
```bash
curl -s -X POST http://localhost:5104/v1/uploads/start \
  -H "Content-Type: application/json" \
  -d '{"filename":"sample.jpg","contentType":"image/jpeg"}'
```

Example response:
```json
{
  "uploadId": "ddbfb206-d2c6-4e11-8ea4-bc0668e2141e",
  "key": "images/2025/11/01/45268be6-daeb-4830-a3e6-f8715df6c4e0.jpg",
  "putUrl": "http://localhost:9000/trackunit-images/...signature...",
  "expiresAt": "2025-11-01T12:44:38Z"
}
```

---

### Step 2 – Upload the file to MinIO

> **Assumption:**  
> For this example, a test image named `sample.jpg` is located on your Desktop.  
> You can replace the path with any local image file of your choice.

**Git Bash:**
```bash
curl -T ~/Desktop/sample.jpg -H "Content-Type: image/jpeg" "<paste putUrl here>"
```

**PowerShell:**
```powershell
curl.exe -T "C:\Users\<you>\Desktop\sample.jpg" -H "Content-Type: image/jpeg" "<paste putUrl here>"
```

Expected: silent success (`HTTP 200` or `204`).

---

### Step 3 – Compute checksum and byte length
**Git Bash:**
```bash
CHECKSUM="sha256:$(sha256sum ~/Desktop/sample.jpg | awk '{print $1}')"
BYTES=$(wc -c < ~/Desktop/sample.jpg | tr -d ' ')
echo $CHECKSUM
echo $BYTES
```

**PowerShell:**
```powershell
$bytes = (Get-Item "$HOME\Desktop\sample.jpg").Length
$hash = (Get-FileHash "$HOME\Desktop\sample.jpg" -Algorithm SHA256).Hash.ToLower()
$checksum = "sha256:$hash"
```

---

### Step 4 – Confirm upload
```bash
curl -s -X POST http://localhost:5104/v1/uploads/confirm \
  -H "Content-Type: application/json" \
  -d "{\"uploadId\":\"ddbfb206-d2c6-4e11-8ea4-bc0668e2141e\",\"bytes\":$BYTES,\"checksum\":\"$CHECKSUM\"}"
```

Expected:
```json
{"status":"Accepted"}
```

---

## 7. Verification

### In MinIO
Check the uploaded object under `trackunit-images/images/yyyy/MM/dd/…`.

### In SQLite
Open `src/Ingestion.Infrastructure/Persistence/EFCore/Data/Ingestion.db`  
and verify:
- `Status` = `Uploaded`
- `Bytes` and `Checksum` set

---

## 8. Troubleshooting

| Problem | Cause | Fix |
|----------|--------|-----|
| `422 UnprocessableEntity` | Invalid filename or unsupported MIME type | Use `image/jpeg`, `image/png`, `image/webp` |
| `404 upload not found` | Wrong or expired uploadId | Re-run /start |
| `403 SignatureDoesNotMatch` | `Content-Type` mismatch on PUT | Use the same header as returned from /start |
| `Request has expired` | URL TTL (300 s) elapsed | Re-start upload |
| `Bucket does not exist` | `trackunit-images` missing | Create it in MinIO Console |

---

## 9. Tear down

Stop MinIO:
```bash
docker compose down
```

Remove volumes (optional):
```bash
docker compose down -v
```

---

## 10. Developer checklist

- [ ] MinIO running (`docker compose up -d`)
- [ ] Storage service running on `http://localhost:5136`
- [ ] Ingestion service running on `http://localhost:5104`
- [ ] `dotnet run` → `/health` returns Healthy
- [ ] `/start` returns presigned URL
- [ ] `PUT` upload succeeds
- [ ] `/confirm` returns Accepted
- [ ] Object visible in MinIO Console
- [ ] Row updated in SQLite

---

## Reference
For service overview and related services, see [README.md](../README.md).

---

**Updated:** November 2025
