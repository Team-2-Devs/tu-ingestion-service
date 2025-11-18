# Trackunit Ingestion Service

![CI](https://github.com/Team-2-Devs/tu-ingestion-service/actions/workflows/ci.yml/badge.svg)

Ingestion microservice for Trackunit.

## Status

* Active Development

## Purpose

* Handle image upload initiation and confirmation
* Coordinate with Storage to fetch pre-signed PUT URLs
* Track upload sessions in a local SQL database
* Validate uploaded objects (checksum, byte length)
* Publish `ImageUploaded` events for downstream services

## Endpoints (v1)

*Note: endpoints are defined here as part of the design. They are not yet implemented unless otherwise stated.*

* POST `/v1/uploads/start` – create upload session and return pre-signed PUT URL
* POST `/v1/uploads/confirm` – validate object, store metadata, publish event
* GET  `/v1/uploads/{id}/status` – (planned) retrieve upload session state
* GET  `/health` – service health check

## Tech

* .NET 8, ASP.NET Core Web API
* Clean/hexagonal layering – Api, Application, Domain, Infrastructure
* SQLite for local development
* Kafka-compatible messaging via Redpanda
* CI via reusable org workflow (see [Team-2-Devs/.github](https://github.com/Team-2-Devs/.github))

## Related services

* [tu-storage-service](https://github.com/Team-2-Devs/tu-storage-service) – issues pre-signed PUT/GET URLs
* [tu-media-access-service](https://github.com/Team-2-Devs/tu-media-access-service) – provides authorized access to media via pre-signed GET from Storage

## Local development

See [DEV.md](./docs/DEV.md) for full setup instructions.

Two modes are supported:

* **Docker Compose (recommended)** – runs Ingestion.Api in a container with SQLite
* **dotnet run** – for debugging in IDE

## API Contracts

Formal versioned specifications for `/v1/uploads` endpoints.
See [v1-ingestion.md](./docs/api-contracts/v1-ingestion.md).

Frozen contract for v1:

* `POST /start` (implemented)
* `POST /confirm` (implemented)
* `GET  /health` (implemented)

## Messaging Contracts

Formal definitions of published ingestion events.
See [`ImageUploaded` (v0)](./docs/event-contracts/v0-ingestion-imageuploaded.md).
