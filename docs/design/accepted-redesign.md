# Accepted frontend implementation

Reference set: `C:\Users\youss\.codex\generated_images\01a19301-12e1-7a51-b08c-de4910f5aeaf\redesign-01-*.png` through `redesign-33-*.png`.

The implemented system uses an ink canvas (#141522), floating white masthead, violet (#8B5CF4) primary actions, navy text, white working planes, lavender selection and mint success. Desktop gutters are 44px and workspace radius is 18px. Following review, the user requested smaller navigation and sign-in: the desktop masthead is now 62px, its logo is 98px wide, and its icons are 17px. Main form controls remain comfortable touch targets. Geist and IBM Plex Sans Arabic serve both directions. Motion is short and disabled under reduced-motion preferences.

## Surface inventory

01 dashboard; 02 sign in; 03 centre selection; 04 password; 05 students; 06 registration; 07 student profile; 08 parent profile; 09 subjects; 10 teachers; 11 groups; 12 group overview; 13 roster/enrollment; 14 group schedule; 15 timetable; 16 attendance sessions; 17 register; 18 scanner; 19 cards; 20 billing; 21 staff; 22 settings; 23 audit; 24 conversations; 25 teacher workspace; 26 portal home; 27 portal chat; 28 portal attendance RTL; 29 mobile students; 30 mobile portal RTL; 31 mobile sign in; 32 shared states; 33 system status.

## Functional adaptations

- Names, dates, counts and status always come from existing APIs. Concept examples are not seed data.
- Portal navigation exposes only portal permissions, even where the Arabic reference depicts staff links.
- Portal credentials retain the existing email authentication contract.
- Empty and populated states are mutually exclusive; the dashboard concept depicts both for illustration.
- Seat imagery represents capacity, not an invented seat reservation feature.
- Existing QR printing dimensions and scanner behavior remain authoritative.
- `/students/:studentId` was explicitly approved during implementation and excluded from subsequent layout changes.
- Subject and person names use the actual stored names; the application does not invent bilingual translations or school years that its DTOs do not contain.
- Audit filtering retains the API's entity/date filters. The concept's additional actor/action filters are not backed by this API.
- Overlapping timetable sessions expand into an accessible stack, preserving every session instead of obscuring simultaneous events.
- Card previews show real selected names; secure QR codes are rendered only from the one-time issuance response.
- The portal calendar is decorative imagery, not fabricated attendance. Attendance records remain in the connected attendance view.

## Verification ledger

Accepted concepts and browser renders were inspected directly with `view_image`. Browser checks began with Chrome DevTools; screenshot capture became unreliable, so Playwright Chromium was used for repeatable exports. Desktop comparisons used 1536×1024, compact sign-in used 1366×768, and mobile checks used 390×844. Representative roles: owner, secretary, teacher, parent and student.

| Comparison              | Concept evidence   | Implementation evidence / resolution                                                                                                                          |
| ----------------------- | ------------------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Navigation and logo     | 01, 05, 29         | Floating permission-aware masthead and mobile bottom navigation; subsequently reduced to the user's requested compact proportions.                            |
| Sign-in composition     | 02, 31             | Separate generated folded-page asset, native form, ink canvas and feature strip. No document overflow at 1366×768 or 390×844.                                 |
| Dashboard hierarchy     | 01, 25             | Editorial welcome, capacity ribbon, real group table, quick actions; separate teacher groups/next-session workspace.                                          |
| Directory rhythm        | 05, 09, 10, 11     | Alphabet index, contact rows, numbered subject bands, faculty rows and subject lanes; long real names wrap.                                                   |
| Group workflows         | 12–19              | Compact Actions menu, roster, enrollment dialog, seat visualization, weekly rules, session timeline, time-axis timetable, register, scanner and print studio. |
| Administration          | 21–24              | Staff role drawer, settings document, selectable audit table/change inspector and searchable three-pane read-only conversations.                              |
| Portal composition      | 26–30              | Real-name welcome, asymmetric attendance/chat paths, separate calendar/chat illustrations, RTL attendance and mirrored mobile navigation.                     |
| Shared states           | 32–33              | Loading skeletons, actionable empty state, recoverable API error/Retry, validation, permission guards, success notifications and live system status.          |
| Palette, type and icons | Full reference set | Ink/violet/mint system, local Geist/Arabic fonts, consistent outline icons, visible focus, reduced-motion support and scalable bilingual SVG lockup.          |

Above-the-fold copy was checked against the references and existing translations. Differences are real account/centre/data values, retained functional descriptions, the user-requested compact chrome, and the adaptations listed above. Concept example statistics and sample conversations were not copied into the product.

Final review screenshots are in [implementation](implementation/). They contain local development data, not fabricated production metrics.

## Verification

- Production build, TypeScript, ESLint and translation-key parity pass. ESLint retains the repository's 50 route fast-refresh warnings; the build retains its existing large-chunk advisory.
- 418 tests across 63 files pass, including a new overlapping-session disclosure check.
- 22 existing browser journeys pass across focused runs: authentication/RTL/logout, centre switching, first-login password change, subjects, student/parent registration and lookup, teacher/group creation, capacity, isolation, scheduling/conflict/cancellation, enrollment/full capacity/transfer/withdrawal, attendance/correction/audited reasons, QR duplicate/replaced-card handling, staff access/roles/audit and Arabic portal login.
- Real desktop/mobile page sweeps reported no page exceptions or horizontal document overflow. Controlled test interception verified loading, empty and API-error states; Retry returned to real data.
- Physical camera scanning and a physical printer were not available. Manual QR entry, generated QR rendering and print-layout invariants were tested. Live assistant/provider delivery was not exercised; local QA disabled background delivery.

## Run and preview

Use the repository's configured PostgreSQL instance and existing local secrets. From the repository root run `dotnet run --project src/TutoringCentre.Api --launch-profile https`. In another terminal run `cd frontend` then `npm.cmd run dev`, and open `http://localhost:5173`.

Review branch: `redesigning-frontend`. No merge or deployment is part of this work.
