# frontend/src/test/render.tsx

## Purpose
`renderWithQueryClient(ui)` renders a component inside a **fresh** `QueryClient`, so tests never share cache, with **retries off** so failures surface immediately (line 5).

## Where it fits
Test infrastructure. Used by `StatusCard.test.tsx`. It implements the convention in `frontend/README.md:41`.

## Walkthrough
- **Lines 6–12:** a new `QueryClient({ defaultOptions: { queries: { retry: false } } })`, then `render(<QueryClientProvider client={queryClient}>{ui}</QueryClientProvider>)`, returning the `RenderResult`.

## Concepts used
- **Test isolation**, and a **custom render helper** wrapping providers.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- **It doesn't wrap a router.** Components that use `<Link>` or router hooks will need a router-aware helper later.
- **It doesn't copy the app's `staleTime`,** so test and production behaviour differ slightly. Intentional.

## Related files
- [StatusCard.test.tsx](../features/status/StatusCard.test.tsx.md)
- [queryClient.ts](../app/queryClient.ts.md)
