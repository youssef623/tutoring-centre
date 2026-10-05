# frontend/src/features/shell/AppShell.tsx

## Purpose

The authenticated page frame: desktop sidebar, mobile slide-in navigation, header with active centre and role, and the user menu (switch centre, language, log out).

## Where It Fits

frontend/src/features/shell. Rendered by `routes/_authenticated.tsx` around the matched child route. Uses Base UI `Dialog` (mobile nav) and `Menu` (user menu), `Badge`, `LanguageSwitcher`, `useSignOut`, i18n namespaces `shell` and `auth`.

## Walkthrough

- `initialsOf(displayName)` (14-23): first letter of up to the first two words, upper-cased; `?` for an empty name.
- `AppShell({ me, children })` (25-84): translators `tShell`, `tAuth`; `mobileNavOpen` state; `activeCentre = me.memberships.find(m => m.centreId === me.activeCentreId)`. A `Dialog.Root` wraps the layout: a `hidden md:block` sidebar with `SidebarNav`; a `Dialog.Portal` with backdrop and a sliding `Dialog.Popup` (positioned with logical `start-0`; slide direction uses `ltr:` / `rtl:` variants) containing a close button (`aria-label` from `nav.closeMenu`) and the same `SidebarNav`; a header with the mobile `Dialog.Trigger` (`md:hidden`, label `nav.openMenu`), the active centre name (`dir="auto"`) and role `Badge` (`tAuth("roles.<role>")`), and `UserMenu`; and a `main` with a centred max-width container for `children`.
- `SidebarNav({ onNavigate })` (86-101): a `nav` with one `Link to="/"` (label `nav.dashboard`, exact active match, active style from `data-[status=active]`).
- `UserMenu({ me })` (103-146): `canSwitchCentre = me.memberships.length > 1`. A `Menu.Root` trigger shows an initials avatar and the display name; the popup contains, conditionally, a `Menu.Item` rendered as `Link to="/select-centre"` (`userMenu.switchCentre`); a plain `div` holding `LanguageSwitcher` (so switching language does not close the menu); and a `Menu.Item` that calls `signOut()` (`userMenu.logout`).

## Concepts Used

### React components, props, state and re-rendering

#### What it means

A component is a function returning UI from props and state. When state a component depends on changes, React calls the function again (a re-render) and updates only the DOM that differs.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Components with local state and composition.

#### How it works here

`useState` for `mobileNavOpen`; small components `SidebarNav`, `UserMenu`.

#### Why it matters here

Open/closed state is local to the shell; navigation clicks close the mobile drawer through `onNavigate`.

### Tailwind, shadcn/ui and variants

#### What it means

Tailwind composes styles from small utility classes. shadcn/ui copies component source into your repo; `cva` (class-variance-authority) maps variant props like `variant="outline"` to class strings.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing](../../../../../PROJECT_OVERVIEW2.md#617-frontend-concepts-react-server-state-and-routing).)

#### Where it appears in this file

Logical properties and RTL variants.

#### How it works here

`border-e`, `start-0`, `ps-1.5 pe-3`, `ltr:`/`rtl:` slide classes.

#### Why it matters here

The same markup works in LTR and RTL; no JavaScript decides left or right.

### Internationalisation (i18next) and RTL layout

#### What it means

i18n moves all user-visible text into per-language resource files looked up by key. Arabic is right-to-left, so direction is set on the document and layout uses logical CSS properties (`start`/`end`) that flip automatically.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout](../../../../../PROJECT_OVERVIEW2.md#634-internationalisation-and-right-to-left-layout).)

#### Where it appears in this file

Translated chrome.

#### How it works here

`useTranslation("shell")`/`("auth")`.

#### Why it matters here

Every visible string except the user's own data comes from locale files.

### Frontend route guards, session state and global 401 handling

#### What it means

A *route guard* decides, before a page renders, whether the user may see it (here by reading the session query and redirecting). It only shapes the UI; the API remains the security boundary. A *global 401 handler* reacts to any request that finds the session gone.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling](../../../../../PROJECT_OVERVIEW2.md#632-frontend-sessions-route-guards-and-global-401-handling).)

#### Where it appears in this file

Centre switching entry point.

#### How it works here

Lines 122-129.

#### Why it matters here

The shell only offers 'Switch centre' when the user has more than one membership.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Only one navigation entry (dashboard) exists. Which menu items appear depends on `me` from the session query; authorization is not decided here (the API enforces it).

## Related Files

- [`frontend/src/routes/_authenticated.tsx`](../../routes/_authenticated.tsx.md)
- [`frontend/src/features/session/useSignOut.ts`](../session/useSignOut.ts.md)
- [`frontend/src/features/language/LanguageSwitcher.tsx`](../language/LanguageSwitcher.tsx.md)
- [`frontend/src/components/ui/badge.tsx`](../../components/ui/badge.tsx.md)
- [`frontend/src/i18n/locales/en/shell.json`](../../i18n/locales/en/shell.json.md)
