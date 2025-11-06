# Redpanda Access via Cloudflare Tunnel

This guide explains how external services (e.g., AI Service) can connect securely to the hosted Redpanda broker.

---

## Prerequisites
- Access granted through Cloudflare Zero Trust policy.
- Membership in the approved GitHub organization.

---

## First-Time Setup
1. Download **cloudflared**:  
   https://developers.cloudflare.com/cloudflare-one/connections/connect-apps/install-and-setup/installation/

---

## Running the Tunnel
Run this command in a terminal before starting your service:
```bash
cloudflared access tcp --hostname redpanda.kennethsorensen.dev --url localhost:9092
```

When prompted to log in:
1. Follow the link shown in the terminal.
2. Authenticate with your GitHub account.

---

## When Connected
When you see:
```
Connection available at localhost:9092
```
you can start your service.

Your Kafka client (e.g., Confluent.Kafka) should be configured as:
```json
{
  "Kafka": {
    "BootstrapServers": "localhost:9092"
  }
}
```

---

## Topic Information

| Setting | Value |
|----------|--------|
| **Topic name** | `tu.images.uploaded` |
| **Partitions** | 4 |
| **Replication** | 1 |
| **Retention** | 7 days |
| **Cleanup policy** | delete |
| **Key** | `objectKey` |

The topic already exists and is centrally managed.
Only predefined topics are allowed. Ask Kenneth Sørensen to create new ones if needed.


---

## Notes
- Connection will close when you terminate `cloudflared`.
- Authentication is handled through Cloudflare Zero Trust policy and currently expires once per week.
- The broker is **development-only** (single node, no redundancy).
