# Milestone 2 Refactor-Ready Backlog (Prioritized)

## Priority 0 (Prerequisite to execution)
1. Reconcile repository packaging so full Unity project is available in active development branch.
2. Confirm canonical Unity version and lock it in docs.

## Priority 1 (Architecture boundaries)
1. Define system boundaries: gameplay, UI, content data, persistence/integration.
2. Introduce folder and naming standards by domain.
3. Separate high-level game state management from per-feature MonoBehaviours.

## Priority 2 (Stability and decoupling)
1. Replace singleton hard assumptions with explicit dependency references where possible.
2. Formalize optional external SDK adapters (ledger/reward integration boundary).
3. Add explicit validation hooks for required scene references at startup.

## Priority 3 (Content model)
1. Expand `HazardData` into data-driven scenario units (hazard, objective, required tools, outcomes).
2. Define reusable scenario payload shape for future AI scenario generation compatibility.

## Priority 4 (Learning loop and progression alignment)
1. Align rewards and penalties to observable safe/unsafe decisions.
2. Define progression policy mapping to achievements, credits, and unlocks.
3. Remove interruption-heavy flows in favor of environment-based interactions.

## Priority 5 (Readiness for backend and teacher scope)
1. Define entities for users, classes/organizations, progress, analytics events.
2. Specify teacher dashboard MVP boundary and post-MVP extensions.
