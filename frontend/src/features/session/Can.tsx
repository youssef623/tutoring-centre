import type { ReactNode } from "react";
import { useCan } from "./useCan";
import type { Permission } from "./permissions";

/** Renders its children only when the signed-in session holds the given permission. Hides for usability; never the security boundary. */
export function Can({ permission, children }: { permission: Permission; children: ReactNode }) {
  return useCan(permission) ? children : null;
}
