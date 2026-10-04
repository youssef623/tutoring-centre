import { getAntiforgeryToken } from "./generated/tutoring-centre";

// In-memory only — never localStorage, sessionStorage or a cookie the script writes. A script injected later
// (e.g. via a dependency compromise) can't read a token that was never stored anywhere readable.
let cachedToken: Promise<string> | null = null;

async function fetchToken(): Promise<string> {
  try {
    const response = await getAntiforgeryToken();
    return response.token;
  } catch (error) {
    // Don't cache a failure forever — the next call should try again instead of being stuck.
    cachedToken = null;
    throw error;
  }
}

/** The current CSRF token, fetched once (through the generated client) and cached in memory. */
export function getCsrfToken(): Promise<string> {
  cachedToken ??= fetchToken();
  return cachedToken;
}

/** Forces the next {@link getCsrfToken} call to fetch a fresh token — the token is bound to the signed-in user. */
export function refreshCsrfToken(): Promise<string> {
  cachedToken = fetchToken();
  return cachedToken;
}
