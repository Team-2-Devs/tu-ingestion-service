# Messaging Contract – ImageUploaded (v0)

**Version:** Draft v0 (unfrozen)  
**Last updated:** 2025-11-02  
**Owner:** Trackunit Ingestion Service  
**Status:** Draft – breaking changes allowed until v1

---

## Overview
Emitted when an image upload is confirmed stored.  
Signals that the file exists in object storage and is ready for downstream processing (e.g., AI).

---

## Topic and Key
- **Topic:** `tu.images.uploaded.v0`  
- **Message key:** `objectKey`  
  - Guarantees ordering for all events related to the same image.

---

## Headers
| Header | Example | Description |
|---------|----------|-------------|
| x-schema | images.uploaded.v0 | Schema version identifier |
| x-producer | ingestion | Originating service |
| x-correlation-id | 7c9f10a6c21f45e4a2f42c8b2a2b3a19 | Always present; generated or propagated for traceability |

---

## Payload
| Field | Type | Description |
|-------|------|-------------|
| uploadId | string | UUID of the upload session |
| objectKey | string | Object storage key (`images/yyyy/MM/dd/{guid}.{ext}`) |
| bytes | number | File size in bytes (>0) |
| checksum | string | SHA-256 in format `sha256:<64-hex>` |
| occurredAt | string | ISO 8601 UTC time when upload was confirmed |

Example:
```json
{
  "uploadId": "2c2c7f4d-2a3d-4e23-8b2f-3a4f5a6b7c8d",
  "objectKey": "images/2025/11/02/edb2ccee-4b9b-4d9f-9f5a-111122223333.jpg",
  "bytes": 438127,
  "checksum": "sha256:3e4f...a9c",
  "occurredAt": "2025-11-02T16:05:43Z"
}
```

---

## Semantics
- Represents a confirmed upload (file persisted in storage).  
- Published by `ConfirmUpload` use case after persistence succeeds.  
- Does not imply any further processing such as virus scan or resizing.

---

## Delivery Semantics
- **Delivery:** at-least-once; duplicates possible.  
- **Ordering:** guaranteed per `objectKey`; not across keys.  
- **Consumer rule:** deduplicate by `uploadId` or `objectKey`.

---

## Versioning Policy
- v0: freely changeable during development.  
- v1 (frozen): no breaking changes allowed; new schema required for breaks (`images.uploaded.v2`).  
- Consumers should check `x-schema` for version compatibility.

---

## Consumer Notes
- Handle duplicate messages idempotently (at-least-once delivery).  
- Ignore unknown fields (forward compatibility).  
- Propagate `x-correlation-id` when emitting downstream events.

---

## Changelog
| Date | Version | Notes |
|------|----------|-------|
| 2025-11-02 | v0 | Initial draft version |

---

**End of document**
