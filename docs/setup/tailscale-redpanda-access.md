# Redpanda Access via Tailscale

This guide explains how team members connect securely to the hosted Redpanda broker running in the homelab environment.

---

## Overview

Redpanda is accessible only through the private Tailscale network.
Access is restricted by ACLs, allowing team members to reach only the Kafka broker port (9092).
No SSH or other system-level access is provided.

---

## Prerequisites
Team members must be invited to the Tailscale network before connecting.  
Contact Kenneth Sørensen for access.

* Tailscale installed on your local device.
* Logged in using the approved GitHub account.
* Device visible and active in the Tailscale admin dashboard.

Download Tailscale here:
[https://tailscale.com/download](https://tailscale.com/download)

---

## Connecting to Redpanda

Once logged in, your device joins the private Tailscale network.

Use the following address to connect to the broker:

```
Bootstrap server: 100.97.224.69:9092
```

This is the Tailscale IP of the Redpanda node.

No Cloudflare Tunnel.
No port forwarding.
No SSH required.

The Tailscale client handles secure, encrypted connectivity automatically.

---

## Client Configuration

Example application configuration:

```json
{
  "Kafka": {
    "BootstrapServers": "100.97.224.69:9092"
  }
}
```

Your Kafka client will communicate directly with Redpanda through Tailscale’s WireGuard-encrypted mesh network.

---

## Topic Information

| Setting            | Value                |
| ------------------ | -------------------- |
| **Topic name**     | `tu.images.uploaded` |
| **Partitions**     | 4                    |
| **Replication**    | 1                    |
| **Retention**      | 7 days               |
| **Cleanup policy** | delete               |
| **Key**            | `objectKey`          |

| Setting            | Value                      |
| ------------------ | -------------------------- |
| **Topic name**     | `tu.recognition.completed` |
| **Partitions**     | 4                          |
| **Replication**    | 1                          |
| **Retention**      | 7 days                     |
| **Cleanup policy** | delete                     |
| **Key**            | `objectKey`                |

These topics already exist and is centrally managed.  
If you need a new topic, contact Kenneth Sørensen.

---

## Security Notes

* Tailscale uses WireGuard encryption by default.
* ACLs restrict teammates to port 9092 only.
* Teammates cannot access any other service or port on the node.
* Access can be revoked or modified at any time.

---

## Troubleshooting

1. Ensure Tailscale is running.
2. Confirm your device appears as **Active** in the Tailscale dashboard.
3. Verify your application uses the correct bootstrap server.
4. If connection fails, contact Kenneth Sørensen to validate ACL rules.

---

## Reference
For service overview and related services, see [README.md](../README.md).

---

**End of document**
