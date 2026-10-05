# frontend/src/routes/select-centre.test.tsx

## Purpose

Tests of the centre picker: listing, choosing a centre, and the empty state.

## Where It Fits

frontend/src/routes. Uses `renderRouter` and MSW.

## Walkthrough

Helper `meWithMemberships(memberships, activeCentreId = null)`. Test 1: two memberships render as two buttons (`Nile Tutoring Centre`, `Maadi Learning Hub`). Test 2: clicking Maadi posts `{ centreId: "c-maadi" }` (captured from the request body) and the router ends at `/`; the `/api/me` handler returns the updated active centre on its second call. Test 3: no memberships shows `Your account has no active centre. Contact your centre owner.` and a `Log out` button. 3 tests.

## Concepts Used

### Mocking the network with MSW

#### What it means

Mock Service Worker intercepts `fetch` at the network layer, so components and hooks run unchanged while the test decides what the 'server' answers.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Stateful handler.

#### How it works here

`meCallCount` in test 2.

#### Why it matters here

The handler changes its answer after the first call, simulating the server-side session change.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`frontend/src/routes/select-centre.tsx`](select-centre.tsx.md)
- [`frontend/src/test/renderRouter.tsx`](../test/renderRouter.tsx.md)
