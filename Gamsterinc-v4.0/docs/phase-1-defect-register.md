# Phase 1 Defect Register

| Severity (Blocker/Major/Minor/Placeholder) | Area | Description | Repro Steps | Impact | Proposed Action | Owner | Target Milestone |
|---|---|---|---|---|---|---|---|
| Blocker | Repository packaging | Full Unity project structure (scenes/settings/packages) missing from snapshot. | Inspect repo root for `Assets/` and `ProjectSettings/` (not present). | Cannot fully open/run Unity MVP from this branch alone. | Restore/commit required Unity project directories or point to canonical project branch. | Unassigned | Milestone 1 |
| Major | External SDK coupling | Slab reward flow referenced `SovereignMesh.*` directly and could fail when SDK absent. | Run project without SovereignMesh assemblies. | Runtime flow interruption / integration failure. | Implement optional SDK fallback (completed in this branch). | Unassigned | Milestone 1 |
| Major | Null reference risk | Slab placement required `hobbitNodeAnchor` and `Rigidbody` without checks. | Trigger placement with missing references/components. | Runtime exceptions during placement lock. | Add guard clauses and warning logs (completed in this branch). | Unassigned | Milestone 1 |
| Major | Null reference risk | `AnimusController` assumes `SovereignOverseer.Instance` exists. | Trigger `SpikeCognitiveLoad` before overseer initialization. | Runtime exception path from stress event. | Add null check with warning (completed in this branch). | Unassigned | Milestone 1 |
| Minor | Audio robustness | `kongVoiceSynth` could be present with null clip. | Trigger validation with no clip assigned. | Non-fatal runtime error risk. | Add clip null guard (completed in this branch). | Unassigned | Milestone 1 |
| Placeholder | Ownership | Feature owners not assigned in documentation tables. | Review docs artifacts. | Slower execution accountability. | Assign domain owners during sprint planning. | Product/Tech Lead | Milestone 1 |
