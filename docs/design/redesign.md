# Hessa — learning observatory

## Scope and design contract

React 19 / Vite / Tailwind / Base UI, with the existing .NET 10 API left intact. Owner, assistant, teacher, parent and student permissions, API hooks, URL filters, localization and mutations stay owned by their existing features.

The concept `hessa-concept.png` establishes the ink rail, cool gray workspace, white surfaces, violet actions, rising-page book mark, strong heading and open table/list model. Generated sample names, counts, copy and table columns are illustrative only: the repository's real translated strings, data and information architecture take precedence. The sign-in detail concept establishes its complete composition.

## Tokens and component inventory

- Background #F5F6FA, white surfaces, ink #171827; brand #8B5CF4. Small text/actions use #7040CF for readable white-label contrast. Mint/amber/red retain semantic meaning and written labels.
- Existing locally served Geist + IBM Plex Sans Arabic; headings 30–38px, body/controls 14–16px, metadata 12px. Arabic line height stays generous.
- 4px spacing scale; 24–40px page gutters; 12px surface radius; restrained shadows. Outline Lucide icons retain their existing task metaphors.
- Shared families: staff rail/header/mobile drawer, portal header, page heading, filter strip, data table, stat tile, empty state, buttons, fields, tabs, menus, dialogs, alerts and skeletons.
- Feature layouts: open dashboard action rail + capacity summaries/list; weekly timetable columns; attendance roster/status controls; scanner camera frame; student/parent profiles and billing tables; portal conversation and chronological attendance.
- Motion: short color/transform transitions, single subtle page entrance, no continuous decoration. Reduced motion disables transitions/entrances.
- SVG book mark is code-native as requested; no bitmap asset or overlay is used in production. The sign-in panel uses architectural lines in CSS.

## Copy and continuity

Use existing locale keys without inventing product claims, routes, dashboards, metrics or controls. Preserve every page's actual headings, labels and CTA order. The dashboard welcome uses the authenticated user's name. Counts derive from API data. Form error and success strings remain unchanged.

Authentication, centre selection, password change and status use the same brand. Reusable primitives carry the design through all CRUD dialogs, loading/empty/error states, billing, operational tools and both portal roles. Tables remain tables on desktop and labelled stacked rows on mobile. Print-card physical/QR dimensions remain unchanged.

## Review previews and fidelity ledger

The generated concepts were inspected with `view_image` alongside the final Playwright PNGs. Chrome DevTools was the first browser used for navigation, validation, console inspection, mobile emulation and reduced motion. Its screenshot export rejected the workspace path, so Playwright saved the review PNGs and supplied exact viewport sizes. This is screenshot export support, not a replacement for the initial Chrome inspection.

- Concept board: [hessa-concept.png](hessa-concept.png), 1536 × 1024. The production dashboard was captured at the same viewport in [dashboard-preview.png](dashboard-preview.png). The board contains multiple screens; the production screenshot correctly contains only the actual dashboard.
- Standalone sign-in concept: [hessa-login-concept.png](hessa-login-concept.png), 1586 × 992. Final matching viewport: [login-preview.png](login-preview.png).
- Native mobile previews at 390 × 844: [portal attendance](portal-mobile-preview.png) and [navigation drawer](navigation-mobile-preview.png). Tablet checks used 768 × 1024; ordinary desktop checks used 1440 × 900.

| Comparison | Concept evidence / rendered result | Resolution |
| --- | --- | --- |
| Palette | Ink rail/panel, cool gray workspace, white surfaces, violet controls | Matched; accessible darker violet used for small text and button gradients |
| Typography | Confident sans headings and legible task controls | Locally served Geist and IBM Plex Arabic retained; deliberate responsive sizes throughout |
| Sign-in composition | Bilingual mark above a framed form, dark feature panel | Enlarged and centered the mark after direct comparison; widened panel content and corrected heading wraps |
| Containers / density | Open workspace, action rail, list/table content and modest summary tiles | Retained actual tables and real-data capacity list; no fictional rows or charts |
| Branding / icons | Fanned open-book mark and restrained task icons | Production SVG book mark, light/dark SVG lockups and matching favicon; existing outline Lucide metaphors preserved |
| Mobile / RTL | Clear controls, attendance rows, logical alignment | Verified 390px and 768px layouts; drawer is scrollable, focus-trapped and dismissible; Arabic follows document direction |
| Dialog states | Layered surface, clear focus and validation | Fixed suppressed nested-context backdrops; checked full-viewport 40% ink overlay, field errors and keyboard dismissal |
| Motion | Restrained entrances and feedback | Short transitions; reduced-motion emulation confirmed 0.01ms duration |

Above-the-fold copy audit: existing sign-in strings, feature descriptions, operational headings, field labels and actions are retained. Actual authenticated names and API counts replace concept samples. Portal welcome intentionally now describes the existing services and offers Chat / Attendance shortcuts. Branded route error text and the translated skip link are intentional accessibility/completion additions. No invented metrics or new product claims were added.

The implemented visual system was faithfully reviewed against the concepts. Intentional functional adaptations: the concept's illustrative row names, dashboard columns, filled icons do not override the repository's real data, existing list structure, outline icon system. The dashboard can be empty when no real group meets its capacity threshold. These are documented production adaptations, not placeholders.

## Verification

- TypeScript typecheck, production build, ESLint (zero errors; 50 existing route fast-refresh warnings), locale parity (21 namespaces), and whitespace diff check.
- Full frontend unit suite: 63 files / 417 tests. Four workers avoid resource contention on this Windows environment. jsdom emits its existing `scrollTo` warning. A final focused run also passed 42 tests covering login, subjects, teachers, transfers and billing after the dialog refinement.
- Real API / PostgreSQL / Chromium journeys: 21 passed in the existing 22-test selection. The original remaining enrollment test matched multiple similarly named students in the reused development database. A temporary copy with an exact student-name selector passed the entire enrollment, last-seat refusal, transfer and withdrawal flow; no permanent test changes were needed.
- Passed journeys include authentication and generic credential errors; Arabic RTL and logout; centre switching; first-login password change; subject add/rename/archive/restore; student registration/search and parent reuse; tenant isolation and role restrictions; teacher/group editing; schedule conflict rejection and cancellation; attendance marking/correction/audit; QR manual scanning, repeats, replacement-card rejection and roster source; parent account creation and isolated portal onboarding.
- 57 visual captures across all five roles and desktop/tablet/mobile/RTL states: no horizontal overflow, browser exceptions or HTTP 5xx responses in the sweep. Included profile/billing tabs, parent profile, forms, validation, empty search, status and missing-page states. Chrome separately confirmed visible invalid fields, 44px mobile submit target, reduced motion and no console errors.
- API started from the unchanged .NET 10 project. The repository's existing local migration command completed. Browser suites used the existing configurable login budget raised for that process only, with jobs/outbox relay disabled. No backend or production configuration was edited.

Not verified: physical camera capture, physical printing, live WhatsApp delivery or external model responses. QR token entry and browser print-card rendering retain the existing implementation. Billing currently exposes charges; no additional payment workflow was invented. Build still reports the existing large-chunk advisory. No merge or deployment was performed.

## Run and preview

Use the root README for database/secrets setup. With PostgreSQL running and the existing local configuration in place:

```powershell
# Repository root, first terminal
dotnet run --project src/TutoringCentre.Api -- migrate
dotnet run --project src/TutoringCentre.Api --launch-profile https

# Second terminal
cd frontend
npm.cmd ci
npm.cmd run dev
```

Open http://localhost:5173 and use your configured seed password with the accounts documented in the root README. `npm.cmd` avoids PowerShell execution-policy restrictions on npm's script shim. Preview parent/student surfaces with `parent@nile.test` / `student@nile.test`; staff roles use the existing owner, secretary and teacher accounts. Do not put credentials into source control.

Changes are isolated to `redesigning-frontend`. Unrelated documentation deletions/moves that appeared during the session are left unstaged and preserved.

## Logo reference correction

The login now uses a fixed SVG lockup with six asymmetric fanned pages and tightly stacked English/Arabic baselines. The Arabic face is locally served at weight 600. Both light and dark login logos share the same geometry and spacing; document RTL cannot reverse the lockup. Shared mark assets and favicon follow the corrected fan. Compared the supplied reference against desktop (1586 ? 992) and mobile (390 ? 844) browser captures with view_image; page copy and authentication behavior remain unchanged.
