# 🛡️ GamesterInc v4.0: The 29th Node
Architecture by Paul Cassidy. Collaborator: Jamie.
Status: PHASE 1 DEPLOYED.
Nodes: Lily-Pi, Oakley Vanguards, 4D RAMS Unity Engine.

## AI + Google enterprise bridge

- `POST /enterprise/briefing` now generates a local edge briefing by default and can optionally enrich it with Google Gemini or Vertex AI when `use_google_enterprise` is enabled.
- `GET /enterprise/status` reports local-first mode plus Google enterprise availability.
- Configure `GEMINI_API_KEY` for direct Gemini access or `GOOGLE_CLOUD_PROJECT` plus `GOOGLE_CLOUD_ACCESS_TOKEN` for Vertex AI enterprise routing.
