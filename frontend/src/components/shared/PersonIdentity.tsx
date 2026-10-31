import type { ReactNode } from "react";
import { initialsOf } from "@/lib/initials";

export function PersonIdentity({ name, children }: { name: string; children?: ReactNode }) {
  const tone = Array.from(name).reduce((sum, letter) => sum + letter.charCodeAt(0), 0) % 4;
  return (
    <span className="person-identity">
      <span aria-hidden="true" className={`person-avatar person-tone-${String(tone)}`}>
        {initialsOf(name)}
      </span>
      <span className="person-name">{children ?? <bdi>{name}</bdi>}</span>
    </span>
  );
}
