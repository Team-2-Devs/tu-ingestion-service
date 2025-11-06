# Trackunit Ingestion Service
![CI](https://github.com/Team-2-Devs/tu-ingestion-service/actions/workflows/ci.yml/badge.svg)

Ingestion microservice for Trackunit.

## Status
- Under development

## Purpose
- Handle image upload initiation and confirmation
- Track upload sessions for validation and auditing
- Coordinate with Storage for pre-signed PUT URLs
- Publish `ImageUploaded` events for downstream services  

## Endpoints (v1)
*Note: endpoints are defined here as part of the design. They are not yet implemented unless otherwise stated.*

- POST /v1/uploads/start - create upload session, return pre-signed PUT URL  
- POST /v1/uploads/confirm - validate, store metadata, publish event  
- GET  /v1/uploads/{id}/status - (planned) check upload status  
- GET  /health – service health check    

## Tech
- .NET 8, ASP.NET Core Web API  
- Clean/hexagonal layering – Api, Application, Domain, Infrastructure
- Database (SQL) for session tracking and validation  
- Kafka messaging  
- CI via reusable org workflow (see [Team-2-Devs/.github](https://github.com/Team-2-Devs/.github))

## Related services
- [tu-storage-service](https://github.com/Team-2-Devs/tu-storage-service) – issues pre-signed PUT/GET URLs
- [tu-media-access-service](https://github.com/Team-2-Devs/tu-media-access-service) – provides authorized access to media via pre-signed GET from Storage

## Local dev
```bash
dotnet run --project src/Ingestion.Api
```

## Developer setup
For local infrastructure (MinIO) and smoke test instructions, see [DEV.md](./docs/DEV.md).

## API Contracts
Formal versioned specifications of service-to-service interfaces. 
See [v1-ingestion.md](./docs/api-contracts/v1-ingestion.md).

Frozen contract for `/v1/uploads` endpoints:
- `POST /start` (implemented)
- `POST /confirm` (implemented)