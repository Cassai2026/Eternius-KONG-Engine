# Phase 1 Feature Status Matrix

| Feature | Scene/System | Status (Working/Partial/Broken) | Notes | Owner |
|---|---|---|---|---|
| Sovereign metrics manager (`SovereignOverseer`) | Unity runtime manager | Partial | Tracks time/capital/B-ROI and updates UI; requires scene wiring and UI refs. | Unassigned |
| Animus movement + cognitive load (`AnimusController`) | Player controller | Partial | WASD movement and stress model present; now guarded against missing overseer singleton. | Unassigned |
| Hazard definition model (`HazardData`) | ScriptableObject model | Working | Data container is valid; requires asset instances in Unity project. | Unassigned |
| Slab placement + reward (`SovereignSlab_Placement`) | Placement interaction | Partial | Placement lock and feedback logic present; null guards and SDK fallback added for stability. | Unassigned |
| External sovereign ledger minting | SovereignMesh integration | Partial | Optional integration now resolved dynamically; skips mint if SDK unavailable. | Unassigned |
| Browser prototype movement (`index.html`) | Web demo | Working | Standalone Three.js movement + camera-follow loop works independently of Unity. | Unassigned |
| Scene-based startup flow | Unity scene graph | Broken (in this snapshot) | No Unity scenes committed in this branch snapshot; flow not directly executable here. | Unassigned |
