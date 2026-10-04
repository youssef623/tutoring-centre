import { createFileRoute } from "@tanstack/react-router";

export interface LoginSearch {
  redirect?: string;
}

export const Route = createFileRoute("/login")({
  validateSearch: (search: Record<string, unknown>): LoginSearch => ({
    redirect: typeof search.redirect === "string" ? search.redirect : undefined,
  }),
  component: LoginPage,
});

// Placeholder; the real form lands in Task 17.3.
function LoginPage() {
  return <section>Login</section>;
}
