import { useSession } from "./useSession";
import type { Permission } from "./permissions";

/** Whether the signed-in session's active role holds the given permission. Advice for the UI only: the dispatcher's permission step is the actual decision. */
export function useCan(permission: Permission): boolean {
  const { me } = useSession();
  return me?.permissions.includes(permission) ?? false;
}
