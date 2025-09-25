# tu-ingestion-service
![CI](https://github.com/Team-2-Devs/tu-ingestion-service/actions/workflows/ci.yml/badge.svg)

Ingestion microservice for Trackunit.

## Purpose
- Handle image upload initiation and confirmation.  
- Store upload session data for validation and auditing.  
- Coordinate with Storage for pre-signed PUT URLs.
- Notify the system when uploads are complete.  

## Endpoints (v1)
*Note: endpoints are defined here as part of the design. They are not yet implemented unless otherwise stated.*

- POST /v1/uploads/start - create upload session, return pre-signed PUT URL  
- POST /v1/uploads/confirm - validate, store metadata, publish event  
- GET  /v1/uploads/{id}/status - (planned) check upload status  
- GET  /health – service health check    

## Tech
- .NET 8, ASP.NET Core Web API  
- Clean/hexagonal layering: Api, Application, Domain, Infrastructure  
- Database persistence for upload session tracking and validation  
- Publishes `ImageUploaded` events to Kafka for downstream processing  
- CI via reusable org workflow (see [Team-2-Devs/.github](https://github.com/Team-2-Devs/.github))

## Related services
- [tu-storage-service](https://github.com/Team-2-Devs/tu-storage-service) – issues pre-signed PUT/GET URLs
- [tu-media-access-service](https://github.com/Team-2-Devs/tu-media-access-service) – provides authorized access to media via pre-signed GET from Storage

## Local dev
```bash
dotnet restore
dotnet build
dotnet run --project src/Ingestion.Api
```