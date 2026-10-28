/**
 * The attendance window's state is the server's to decide (`window` on every response) and the browser only
 * shows it. The one number the browser needs is how long before a session its window opens, to label a closed
 * window with "Opens at 15:30"; it mirrors `AttendanceWindow.OpensBefore` in the domain and is never used to
 * decide whether marking is allowed.
 */
export const WindowOpensBeforeMinutes = 30;

export type WindowState = "not_open" | "open" | "closed";

export function isWindowState(value: string): value is WindowState {
  return value === "not_open" || value === "open" || value === "closed";
}

/** The instant a session's window opens, for formatting in the centre's zone. */
export function windowOpensAt(startsAt: string): string {
  return new Date(Date.parse(startsAt) - WindowOpensBeforeMinutes * 60_000).toISOString();
}
