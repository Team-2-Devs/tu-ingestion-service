# API Contract – Ingestion Service (v1)

**Version:** 1.0 (frozen)  
**Last updated:** November 2025  
**Owner:** Trackunit Ingestion Service  
**Scope:** Public upload interface used by client applications.  
**Status:** Stable – breaking changes require version bump to /v2/.

---

## Overview

The Ingestion Service manages upload sessions for incoming media (e.g., images).  
It provides endpoints for:
- Creating a new upload session and receiving a presigned PUT URL (from Storage).  
- Confirming that an upload has completed and recording its metadata.  

It persists upload sessions but does not store the actual file data — files are uploaded directly to object storage via the presigned URL.

---

## Base URL

```bash
http://localhost:5104/v1/uploads
```

Replace host and port when deployed, for example:
```bash
https://ingestion.<env>.trackunit.internal/v1/uploads
```

---

## Endpoints

### 1. POST /v1/uploads/start

Create a new upload session and receive a presigned PUT URL for direct upload to Storage.

#### Description
Generates a unique upload session and object key, then requests a presigned PUT URL from the Storage service.  
Returns the upload ID, object key, presigned URL, and expiry timestamp.  
The presigned URL TTL is server-controlled (default 300 seconds). Clients do not send or override TTL.

#### Request
```json
{
  "filename": "sample.jpg",
  "contentType": "image/jpeg"
}
```

#### Parameters
| Field | Type | Required | Description |
|-------|------|-----------|--------------|
| `filename` | string | yes | Original filename; extension used to determine object type. |
| `contentType` | string | yes | MIME type, must be one of: image/jpeg, image/png, image/webp. |

#### Response 200 OK
```json
{
  "uploadId": "0f2b8c1f-7c10-4f3f-9a46-2c6a9d7b1b2d",
  "key": "images/2025/10/18/1f3a4b5c-6d7e-8f90-a1b2-c3d4e5f67890.jpg",
  "putUrl": "http://localhost:9000/trackunit-images/images/2025/10/18/1f3a4b5c-6d7e-8f90-a1b2-c3d4e5f67890.jpg?...",
  "expiresAt": "2025-10-18T14:25:00Z"
}
```

| Field | Type | Description |
|-------|------|--------------|
| `uploadId` | string (UUID) | Unique ID for this upload session. |
| `key` | string | Generated object key under images/{yyyy}/{MM}/{dd}/{guid}.{ext}. |
| `putUrl` | string | Presigned PUT URL returned from Storage service. |
| `expiresAt` | string (ISO 8601) | UTC timestamp when the presigned URL expires. |

#### Response 422 Unprocessable Entity
```json
{
  "errors": {
    "filename": ["Filename cannot be empty"],
    "contentType": ["Unsupported MIME type; allowed: image/jpeg, image/png, image/webp"]
  }
}
```

#### Response 500 Internal Server Error
```json
{
  "type": "about:blank",
  "title": "Internal Server Error",
  "status": 500,
  "detail": "Unexpected failure while creating upload session."
}
```

---

### 2. POST /v1/uploads/confirm

Confirm that a file has been successfully uploaded.

#### Description
Marks the given upload session as Uploaded, and records metadata such as byte size and checksum.  
In future versions, this will also emit an ImageUploaded event for downstream consumers.

#### Request
```json
{
  "uploadId": "0f2b8c1f-7c10-4f3f-9a46-2c6a9d7b1b2d",
  "bytes": 345678,
  "checksum": "sha256:8b1a9953c4611296a827abf8c47804d7..."
}
```

#### Parameters
| Field | Type | Required | Description |
|-------|------|-----------|--------------|
| `uploadId` | string (UUID) | yes | ID returned from /start. |
| `bytes` | integer | yes | File size in bytes. Must be > 0. |
| `checksum` | string | yes | SHA256 checksum in the format sha256:<hex>. |

#### Response 202 Accepted
```json
{
  "status": "Accepted"
}
```

#### Response 404 Not Found
```json
{
  "type": "about:blank",
  "title": "Upload not found",
  "status": 404,
  "detail": "No upload session found for the given ID."
}
```

#### Response 422 Unprocessable Entity
```json
{
  "errors": {
    "bytes": ["Must be greater than zero"],
    "checksum": ["Invalid checksum format"]
  }
}
```

---

### 3. GET /health

Simple health probe used by orchestrators or load balancers.

#### Response 200 OK
```text
Healthy
```

---

## Example sequence (use case 1: upload)

1. Client (mobile/web) calls:
```bash
POST /v1/uploads/start
```
to obtain a presigned PUT URL.

2. Client uploads directly to the returned URL using HTTP PUT:
```bash
curl -T ./sample.jpg -H "Content-Type: image/jpeg" "<putUrl>"
```

3. Client confirms completion:
```bash
POST /v1/uploads/confirm
```

4. Ingestion Service marks upload as complete and may later emit an event (e.g. ImageUploaded).

---

## Notes

- URLs expire automatically (typically after 300s).  
- Only image/jpeg, image/png, and image/webp are allowed in v1.  
- Ingestion enforces the MIME allowlist; Storage only validates syntactic MIME format.  
- /v1/uploads/start must be called before uploading to Storage.  
- /v1/uploads/confirm must be called after upload success.  
- Database persistence is handled internally; client should only care about the upload ID.

---

## Changelog

| Date | Version | Changes |
|------|----------|----------|
| 2025-10-18 | v1.0 | Initial frozen contract for /v1/uploads/start and /v1/uploads/confirm. |

---

## Reference
For service overview and related services, see [README.md](../../README.md).

---

**End of document**
