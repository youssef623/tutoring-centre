# Frontend read ledger

Every listed file was opened as bytes and decoded as UTF-8 where possible. This ledger records extraction, not proof that every behavior was executed.

## frontend/.gitignore
33 lines; 445 bytes; SHA256 `85f4f5e33148ad2fa4e837c16ce159db63f5324e278f545ff1d38cb4ec1b3322`.


## frontend/.prettierignore
5 lines; 74 bytes; SHA256 `b0f4d2ed4ad8a94d486a0fc44dd9b07e4b9ebffbb34f882b0d2c567f7005f4b6`.


## frontend/.prettierrc
6 lines; 98 bytes; SHA256 `34d6e3036acef3b54775b7e7726cf6429a5409447b0c91f63828c220f30de572`.


## frontend/components.json
25 lines; 515 bytes; SHA256 `156858e2da20785ac0f1853580c7e34d5b8496f519e8839d2176e85947d4f779`.

Top-level keys: `$schema`, `style`, `rsc`, `tsx`, `tailwind`, `iconLibrary`, `rtl`, `aliases`, `menuColor`, `menuAccent`, `registries`

## frontend/e2e/auth.spec.ts
59 lines; 2935 bytes; SHA256 `2c7112733db1107f59bb825ca973443335fa037445a70f11e9070cd5e10d7ce4`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L18: `test("owner: sign in, dashboard, Arabic RTL, logout", async ({ page }) => {`
- L34: `test("teacher: centre picker, header reflects the choice, switch centre", async ({ page }) => {`
- L50: `test("a wrong password shows the generic error and stays on /login", async ({ page }) => {`

## frontend/e2e/enrollment.spec.ts
136 lines; 7585 bytes; SHA256 `702735e21be08181846862ba63afbabc4b1c4e0a971fa2c286aa8e269c1a51c8`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L20: `async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- L26: `async function createGroup(page: Page, name: string, capacity: number): Promise<string> {`
- L41: `async function enrollBySearch(page: Page, search: string, studentName: string): Promise<void> {`
- L52: `async function prepareEnrollFromStudentPage(browser: Browser, studentName: string, groupName: string) {`
- L68: `test("secretary: fills a two-seat group, sees a full group refuse politely, transfers and withdraws", async ({`
- L122: `test("teacher: sees their own group's roster, names only, with no actions", async ({ page }) => {`

## frontend/e2e/first-login.spec.ts
76 lines; 4074 bytes; SHA256 `1ad7b17efd2a26c25a9fab94c275629564282af0e00bda027976f11a142d8c04`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L15: `async function signIn(page: Page, email: string, withPassword: string): Promise<void> {`
- L23: `test("a new staff member is forced through a password change on first sign-in", async ({ page, browser }) => {`

## frontend/e2e/full-centre-setup.spec.ts
254 lines; 16592 bytes; SHA256 `43dc5e2305046b1f92cb665f35305b27e62b131a703739b6c7b7e221cbc9939a`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L17: `const ownerEmail = "owner@giza.test";`
- L18: `const ownerName = "Giza Owner";`
- L19: `const centreName = "مركز الجيزة الجديد";`
- L22: `function nextSaturdayInCairo(): string {`
- L34: `function uniquePhone(): string {`
- L39: `async function signIn(page: Page, email: string, withPassword: string): Promise<void> {`
- L48: `async function choose(page: Page, scope: Locator, label: string, option: string | RegExp): Promise<void> {`
- L54: `async function addWeeklySession(page: Page, groupPath: string, day: string, ninetyMinutes: string): Promise<void> {`
- L67: `test("owner: sets up an empty centre entirely from the UI, down to a weekly timetable its teacher sees", async ({ page, browser }) => {`

## frontend/e2e/schedule.spec.ts
171 lines; 9348 bytes; SHA256 `9dbeff848cba65801562b344ea8a4976b4d06b9a5be2c848d92e5b63343c51c3`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L14: `function nextSaturdayInCairo(): string {`
- L25: `async function signIn(page: Page, email: string): Promise<void> {`
- L35: `async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- L41: `async function addTeacher(page: Page, fullName: string): Promise<void> {`
- L50: `async function createGroup(page: Page, name: string, teacherName: string): Promise<string> {`
- L63: `async function addWeeklySession(`
- L82: `async function scheduleAnotherTeachersGroup(browser: Browser, groupName: string, saturday: string): Promise<void> {`
- L97: `test("owner: schedules two groups of one teacher, is refused the overlap, and cancels a session", async ({ page }) => {`
- L146: `test("teacher: sees only their own sessions in Nile, with no schedule controls", async ({ page, browser }) => {`

## frontend/e2e/staff-access.spec.ts
44 lines; 2032 bytes; SHA256 `cced064afc99e9d83ae41df66a51830af06e0c39ae44a2a136072c62c673a3a2`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L19: `test("secretary: no owner-only nav items, and no owner-only page by direct URL", async ({ page }) => {`

## frontend/e2e/staff-audit.spec.ts
74 lines; 4138 bytes; SHA256 `7e19b9de4aa3ecbb14faaf9bc6981e32cb968eae26971d6bff09be3078f1d650`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L14: `async function signIn(page: Page, email: string): Promise<void> {`
- L24: `async function addStaffMember(page: Page, email: string, displayName: string): Promise<void> {`
- L34: `async function changeRole(page: Page, displayName: string, roleName: "Teacher" | "Assistant"): Promise<void> {`
- L45: `test("owner: changing a member's role appears in the audit log with translated before/after values", async ({`

## frontend/e2e/students.spec.ts
114 lines; 5786 bytes; SHA256 `6ed9b6345236cf164bc83227106af2ef268b414b5ccc8f1dd353fd2b35470d9e`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L22: `function uniquePhone(): string {`
- L27: `test("secretary: registers a student with a new parent in Arabic, then finds them by search", async ({ page }) => {`
- L61: `test("secretary: registering a sibling with the same parent phone links the existing parent", async ({ page }) => {`
- L95: `test("tenant isolation: the Maadi owner sees none of Nile's students, and a teacher has no Students nav item", async ({`

## frontend/e2e/subjects.spec.ts
107 lines; 5724 bytes; SHA256 `a6f7ecc8d89309519ee9771ec8900504f55ca0f26fc07d8536454c6b7efe8f3e`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L21: `test("owner: add, rename, archive, show archived, restore a subject", async ({ page }) => {`
- L72: `test("two centres see different subject lists, including for a teacher switching between them", async ({ page }) => {`

## frontend/e2e/teachers-groups.spec.ts
128 lines; 6647 bytes; SHA256 `b87f07c904b66fde92ee6601beb5db782132746d8acfd161dcdb9d11903e8552`.

- L5: `const password: string = process.env.SEED_PASSWORD ?? "";`
- L10: `async function signIn(page: Page, email: string): Promise<void> {`
- L20: `async function choose(page: Page, scope: ReturnType<Page["getByRole"]>, label: string, option: string): Promise<void> {`
- L29: `async function urlOfGroupTaughtByMona(browser: Browser): Promise<string> {`
- L48: `test("owner: adds a teacher, creates a group for a seeded subject with them, then changes its capacity", async ({ page }) => {`
- L83: `test("teacher: one account sees only its own group in Nile, not another teacher's, and only Maadi's groups in Maadi", async ({`
- L116: `test("secretary: sees Teachers read-only, and can create groups", async ({ page }) => {`

## frontend/eslint.config.js
36 lines; 1284 bytes; SHA256 `9c78d7d14fff9836bd89d18025ee5676bf5b08dfda9b413694020ba3b78aa05c`.

- L8: `export default defineConfig([`

## frontend/index.html
13 lines; 357 bytes; SHA256 `df4e1d809d1749a5a4def673b9f19c8f3b0dabe1e77db5038490b42155131661`.


## frontend/openapi/TutoringCentre.Api.json
6020 lines; 147397 bytes; SHA256 `20c231078e844d9bb8e67483d9ba4fee9371ecc1da2216ce00dd426f92296ced`.

- `GET /api/system/info`: `GetSystemInfo`
- `GET /api/auth/antiforgery`: `GetAntiforgeryToken`
- `POST /api/auth/login`: `Login`
- `POST /api/auth/logout`: `Logout`
- `GET /api/me`: `GetMe`
- `POST /api/session/centre`: `SelectCentre`
- `POST /api/auth/change-password`: `ChangePassword`
- `GET /api/subjects`: `ListSubjects`
- `POST /api/subjects`: `CreateSubject`
- `GET /api/subjects/{id}`: `GetSubject`
- `PUT /api/subjects/{id}`: `RenameSubject`
- `POST /api/subjects/{id}/archive`: `ArchiveSubject`
- `POST /api/subjects/{id}/restore`: `RestoreSubject`
- `GET /api/teachers`: `ListTeachers`
- `POST /api/teachers`: `CreateTeacher`
- `GET /api/teachers/{id}`: `GetTeacher`
- `PUT /api/teachers/{id}`: `UpdateTeacher`
- `PUT /api/teachers/{id}/account`: `SetTeacherAccount`
- `POST /api/teachers/{id}/archive`: `ArchiveTeacher`
- `POST /api/teachers/{id}/restore`: `RestoreTeacher`
- `GET /api/groups`: `ListGroups`
- `POST /api/groups`: `CreateGroup`
- `GET /api/groups/{id}`: `GetGroup`
- `PUT /api/groups/{id}`: `RenameGroup`
- `PUT /api/groups/{id}/capacity`: `ChangeGroupCapacity`
- `PUT /api/groups/{id}/teacher`: `AssignGroupTeacher`
- `POST /api/groups/{id}/archive`: `ArchiveGroup`
- `POST /api/groups/{id}/restore`: `RestoreGroup`
- `POST /api/groups/{groupId}/enrollments`: `enrollStudent`
- `GET /api/groups/{groupId}/roster`: `getGroupRoster`
- `GET /api/students/{studentId}/enrollments`: `listStudentEnrollments`
- `POST /api/enrollments/{id}/withdraw`: `withdrawEnrollment`
- `POST /api/enrollments/{id}/transfer`: `transferEnrollment`
- `GET /api/groups/{groupId}/schedule-rules`: `listGroupScheduleRules`
- `POST /api/groups/{groupId}/schedule-rules`: `addScheduleRule`
- `POST /api/schedule-rules/{id}/end`: `endScheduleRule`
- `POST /api/schedule/extend`: `extendSchedule`
- `GET /api/timetable`: `getTimetable`
- `POST /api/sessions/{id}/cancel`: `cancelSession`
- `POST /api/parents`: `CreateParent`
- `GET /api/parents/{id}`: `GetParent`
- `PUT /api/parents/{id}`: `UpdateParent`
- `POST /api/parents/lookup`: `LookupParent`
- `GET /api/students`: `listStudents`
- `POST /api/students`: `registerStudent`
- `GET /api/students/{id}`: `getStudent`
- `PUT /api/students/{id}`: `updateStudent`
- `POST /api/students/{id}/archive`: `archiveStudent`
- `POST /api/students/{id}/restore`: `restoreStudent`
- `POST /api/students/{id}/parents`: `linkStudentParent`
- `POST /api/students/{id}/parents/{parentId}/unlink`: `unlinkStudentParent`
- `POST /api/students/{id}/parents/{parentId}/make-primary`: `makeStudentParentPrimary`
- `GET /api/staff`: `ListStaff`
- `POST /api/staff`: `CreateStaff`
- `PUT /api/staff/{membershipId}/role`: `ChangeStaffRole`
- `POST /api/staff/{membershipId}/deactivate`: `DeactivateStaff`
- `POST /api/staff/{membershipId}/reactivate`: `ReactivateStaff`
- `GET /api/audit`: `GetAuditLog`
- `GET /api/centre/settings`: `GetCentreSettings`
- `PUT /api/centre/settings`: `UpdateCentreSettings`

## frontend/orval.config.ts
20 lines; 713 bytes; SHA256 `607f2cadf68ce6b80421d904a0dbd926eb6bbcbf939dc9d829a5567f0f1e41b3`.

- L3: `export default defineConfig({`

## frontend/package-lock.json
10139 lines; 354287 bytes; SHA256 `4aa4841a1849ad1c77c934cba4ce57f8a166b417f094651adbd16ffb09085f9c`.

- `@base-ui/react`: `1.8.0`
- `@fontsource-variable/geist`: `5.3.0`
- `@fontsource/ibm-plex-sans-arabic`: `5.3.0`
- `@hookform/resolvers`: `5.9.1`
- `@tailwindcss/vite`: `4.3.3`
- `@tanstack/react-query`: `5.104.1`
- `@tanstack/react-router`: `1.170.41`
- `class-variance-authority`: `0.7.1`
- `cn`: `0.4.0`
- `i18next`: `26.4.2`
- `i18next-icu`: `2.4.4`
- `intl-messageformat`: `11.2.15`
- `lucide-react`: `1.49.0`
- `next-themes`: `0.4.6`
- `react`: `19.3.0`
- `react-dom`: `19.3.0`
- `react-hook-form`: `7.89.0`
- `react-i18next`: `17.0.15`
- `shadcn`: `4.21.1`
- `sonner`: `2.0.8`
- `tailwindcss`: `4.3.3`
- `tw-animate-css`: `1.4.0`
- `zod`: `4.6.5`
- `@eslint/js`: `10.0.1`
- `@playwright/test`: `1.63.0`
- `@tanstack/router-plugin`: `1.168.42`
- `@testing-library/dom`: `10.4.2`
- `@testing-library/jest-dom`: `7.0.1`
- `@testing-library/react`: `16.3.3`
- `@types/node`: `24.19.1`
- `@types/react`: `19.3.0`
- `@types/react-dom`: `19.3.0`
- `@vitejs/plugin-react`: `6.1.1`
- `eslint`: `10.11.0`
- `eslint-plugin-react-hooks`: `7.1.1`
- `eslint-plugin-react-refresh`: `0.5.7`
- `globals`: `17.13.0`
- `jsdom`: `30.1.1`
- `msw`: `2.15.0`
- `orval`: `8.39.0`
- `prettier`: `3.9.9`
- `typescript`: `6.0.3`
- `typescript-eslint`: `8.71.0`
- `vite`: `8.3.2`
- `vitest`: `5.0.3`

## frontend/package.json
68 lines; 1998 bytes; SHA256 `00db5d6061abc2e9ff28bc419d843c666d235b01f24a6a238d778a14ecd09d5e`.

Top-level keys: `name`, `private`, `version`, `type`, `scripts`, `dependencies`, `devDependencies`

## frontend/playwright.config.ts
44 lines; 1850 bytes; SHA256 `aa46941503cc9abd908af953d42b8b8f8959f3a06371a042e14f5a8ccf096e89`.

- L14: `const apiHttpsTarget = "https://localhost:7197";`
- L15: `const apiReadinessUrl = "http://localhost:5245/health/ready";`
- L17: `export default defineConfig({`

## frontend/public/favicon.svg
13 lines; 680 bytes; SHA256 `efe9f7ce5c4e29aaa2f5574e2af83d45e6cfe89dd86168719da01c12132d8cea`.


## frontend/public/icons.svg
24 lines; 5031 bytes; SHA256 `b45fa506195cfcdef406ba9f0c77b36ddc1a7c224040926ec70abc2fdea7b93a`.


## frontend/README.md
206 lines; 11842 bytes; SHA256 `efe83a5ec69c02c271dad20f5f435678badca29a1a1ef9fb0042fd0a1ac15c47`.

- L93: `'useCan(permission: Permission): boolean' reads the signed-in session's active-role permission list`
- L94: `('me.permissions', from 'GET /api/me') and answers whether it holds the given one. '<Can permission={...}>'`
- L115: `- **Fonts and images:** 'font-src 'self'', 'img-src 'self' data:' — self-host fonts ('vite.config.ts''s`
- L139: `export default defineConfig([`
- L173: `export default defineConfig([`

## frontend/scripts/check-i18n-keys.mjs
65 lines; 2393 bytes; SHA256 `db9193dff43feb0ff8c42c3ed00612be6312fcccf0208948aa7c8679c12fbd10`.

- L9: `const scriptDir = path.dirname(fileURLToPath(import.meta.url));`
- L10: `const localesDir = path.join(scriptDir, "..", "src", "i18n", "locales");`
- L11: `const Languages = ["en", "ar"];`
- L13: `function flattenKeys(value, prefix = "") {`
- L23: `function loadNamespace(language, namespace) {`
- L29: `function namespacesFor(language) {`
- L33: `const namespacesByLanguage = new Map(Languages.map((language) => [language, new Set(namespacesFor(language))]));`
- L34: `const allNamespaces = new Set(Languages.flatMap((language) => [...namespacesByLanguage.get(language)]));`

## frontend/src/api/apiFetch.test.ts
219 lines; 6637 bytes; SHA256 `202c6bb647d1be67c2a01d9ccf87ea7a8cfdbbc451f893bffb8b22e79f61277a`.

- L8: `describe("apiFetch", () => {`
- L9: `it("returns the parsed body for a 200 response", async () => {`
- L20: `it("returns undefined for a 204 response", async () => {`
- L31: `it("maps the 404 Problem Details fixture to a notFound ApiError with code and correlation id", async () => {`
- L48: `it("maps the 400 Problem Details fixture to a validation ApiError", async () => {`
- L64: `it("maps a network failure to network.unreachable with status 0", async () => {`
- L79: `it("maps a non-JSON 502 response to an unexpected ApiError", async () => {`
- L100: `describe("apiFetch CSRF handling", () => {`
- L106: `it("attaches X-XSRF-TOKEN on a non-GET request", async () => {`
- L124: `it("does not attach X-XSRF-TOKEN on a GET request", async () => {`
- L142: `it("refreshes the token and retries exactly once after a 403 auth.csrf_invalid, then succeeds", async () => {`
- L170: `it("surfaces the error when the retry also fails with auth.csrf_invalid, without retrying again", async () => {`
- L190: `describe("toApiError", () => {`
- L211: `it("falls back to http.<status> when the problem has no code", () => {`

## frontend/src/api/apiFetch.ts
179 lines; 5489 bytes; SHA256 `cba03bf420ed415871f9e997f05151d9dec7622b17f62d44884e8d0b376e27a5`.

- L5: `const CsrfHeaderName = "X-XSRF-TOKEN";`
- L8: `const SessionChangingPaths = new Set(["/api/auth/login", "/api/auth/logout", "/api/session/centre"]);`
- L14: `export class ApiRequestError extends Error implements ApiError {`
- L37: `function kindFromStatus(status: number): ErrorKind {`
- L57: `export function toApiError(body: unknown, status: number): ApiError {`
- L78: `export function asApiError(error: unknown): ApiError {`
- L91: `async function readBody(response: Response): Promise<unknown> {`
- L104: `function isCsrfError(error: unknown): boolean {`
- L108: `async function buildHeaders(init: RequestInit | undefined, attachCsrfToken: boolean): Promise<Headers> {`
- L123: `async function sendOnce<T>(url: string, init: RequestInit | undefined, headers: Headers): Promise<T> {`
- L158: `export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {`

## frontend/src/api/csrf.ts
28 lines; 1053 bytes; SHA256 `fc5cfa10f3cfe8ab5984e00f9f8aea353c3ee4610e6b5829a54ef868523cffa5`.

- L7: `async function fetchToken(): Promise<string> {`
- L19: `export function getCsrfToken(): Promise<string> {`
- L25: `export function refreshCsrfToken(): Promise<string> {`

## frontend/src/api/errorMessages.test.ts
94 lines; 2296 bytes; SHA256 `ef8b9cd38daacd8ec70c9080a58ce5ef8aa5c077cb1e031ba3470fa537d2c124`.

- L6: `describe("messageFor", () => {`
- L12: `it("returns the specific dictionary message for a known code", () => {`
- L30: `it("falls back to the kind's message for an unknown code", () => {`
- L46: `it("falls back to the generic message for an unknown code and unknown kind", () => {`
- L62: `it("returns the English message for tenant.not_selected", () => {`
- L78: `it("returns the Arabic message for tenant.not_selected", async () => {`

## frontend/src/api/errorMessages.ts
19 lines; 573 bytes; SHA256 `bfcd8208948d4cac23b9767e6981efb475cabe6fcc831b72d097cf2e41e5032f`.

- L4: `const Namespace = "errors";`
- L7: `export function messageFor(error: ApiError): string {`

## frontend/src/api/errors.ts
27 lines; 962 bytes; SHA256 `9e3bd85dca200025b513469312ea30c89a5de6b4a10e074ddf50fa9989161afd`.

- L5: `export type ErrorKind =`
- L15: `export interface ApiError {`

## frontend/src/api/fieldErrors.ts
45 lines; 1930 bytes; SHA256 `ca94866e6b1428f3c1fb5f10770ed99bbf2050098dcc584daf3e48698d1232c8`.

- L8: `function hasCodeMessage(error: ApiError): boolean {`
- L19: `export function applyTranslatedFieldErrors<TField extends string>(`
- L43: `export function setCodedFieldError<TField extends string>(error: ApiError, setError: SetFieldError<TField>, field: TField): void {`

## frontend/src/api/fixtures/problem-400.json
7 lines; 223 bytes; SHA256 `170762a53530aeccc9c532988eabcffd3e37f1d306f0bf9838c2027c411cd8d1`.

Top-level keys: `title`, `status`, `code`, `traceId`, `correlationId`

## frontend/src/api/fixtures/problem-404.json
8 lines; 279 bytes; SHA256 `1c25f9a567c5116e6ca36ad6693828cbe09e6138a24d8cd7be8d360a758f4236`.

Top-level keys: `title`, `status`, `detail`, `code`, `traceId`, `correlationId`

## frontend/src/api/generated/tutoring-centre.ts
5997 lines; 217650 bytes; SHA256 `c1727bedb53cd6c7ba0d9d367423068455adb7b6d2da0380d36fda6f4a25389d`.

- L5: `* OpenAPI spec version: 1.0.0`
- L27: `export type DayOfWeek = typeof DayOfWeek[keyof typeof DayOfWeek];`
- L30: `export const DayOfWeek = {`
- L40: `export interface AddScheduleRuleRequest {`
- L50: `export interface AddScheduleRuleResponse {`
- L56: `export interface AntiforgeryTokenResponse {`
- L60: `export interface AssignGroupTeacherRequest {`
- L63: `version: number | string;`
- L66: `export interface AuditChangeDto {`
- L74: `export interface AuditEntryDto {`
- L90: `export interface AuditPageDto {`
- L96: `export type SupportedLocale = typeof SupportedLocale[keyof typeof SupportedLocale];`
- L99: `export const SupportedLocale = {`
- L104: `export interface CentreSettingsDto {`
- L110: `version: number | string;`
- L113: `export interface ChangeGroupCapacityRequest {`
- L117: `version: number | string;`
- L120: `export interface ChangePasswordRequest {`
- L125: `export type StaffRole = typeof StaffRole[keyof typeof StaffRole] | null;`
- L128: `export const StaffRole = {`
- L134: `export interface ChangeStaffRoleRequest {`
- L137: `version: number | string;`
- L140: `export type ClassSessionStatus = typeof ClassSessionStatus[keyof typeof ClassSessionStatus];`
- L143: `export const ClassSessionStatus = {`
- L148: `export interface CreateGroupRequest {`
- L156: `export interface CreateGroupResponse {`
- L160: `export interface CreateParentRequest {`
- L165: `export interface CreateParentResponse {`
- L169: `export interface CreateStaffRequest {`
- L176: `export interface CreateStaffResponse {`
- L183: `export interface CreateSubjectRequest {`
- L187: `export interface CreateSubjectResponse {`
- L191: `export interface CreateTeacherRequest {`
- L197: `export interface CreateTeacherResponse {`
- L201: `export interface EndScheduleRuleRequest {`
- L204: `version: number | string;`
- L207: `export type EnrollmentEndReason = typeof EnrollmentEndReason[keyof typeof EnrollmentEndReason] | null;`
- L210: `export const EnrollmentEndReason = {`
- L215: `export type EnrollmentStatus = typeof EnrollmentStatus[keyof typeof EnrollmentStatus];`
- L218: `export const EnrollmentStatus = {`
- L223: `export interface EnrollmentVersionRequest {`
- L225: `version: number | string;`
- L228: `export interface EnrollStudentRequest {`
- L232: `export interface EnrollStudentResponse {`
- L236: `export interface ExtendScheduleRequest {`
- L240: `export interface ExtendScheduleResponse {`
- L245: `export type GroupStatus = typeof GroupStatus[keyof typeof GroupStatus];`
- L248: `export const GroupStatus = {`
- L253: `export interface GroupDto {`
- L266: `version: number | string;`
- L269: `export interface GroupListResponse {`
- L273: `export interface RosterEntryDto {`
- L279: `version: number | string;`
- L282: `export interface GroupRosterDto {`
- L292: `export interface GroupVersionRequest {`
- L294: `version: number | string;`
- L297: `export type HttpValidationProblemDetailsErrors = {[key: string]: string[]};`
- L299: `export interface HttpValidationProblemDetails {`
- L316: `export type ParentRelationship = typeof ParentRelationship[keyof typeof ParentRelationship];`
- L319: `export const ParentRelationship = {`
- L326: `export interface LinkStudentParentRequest {`
- L330: `version: number | string;`
- L333: `export interface LoginRequest {`
- L338: `export interface LookupParentRequest {`
- L342: `export interface MembershipDto {`
- L350: `export interface MeDto {`
- L363: `export type MembershipStatus = typeof MembershipStatus[keyof typeof MembershipStatus];`
- L366: `export const MembershipStatus = {`
- L371: `export type StudentStatus = typeof StudentStatus[keyof typeof StudentStatus];`
- L374: `export const StudentStatus = {`
- L379: `export interface ParentChildDto {`
- L387: `export interface ParentDetailDto {`
- L392: `version: number | string;`
- L396: `export interface ParentDto {`
- L401: `version: number | string;`
- L404: `export interface ParentLookupDto {`
- L408: `export interface ProblemDetails {`
- L424: `export interface RegisterStudentRequestParent {`
- L433: `export interface RegisterStudentRequest {`
- L443: `export interface RegisterStudentResponse {`
- L448: `export interface RenameGroupRequest {`
- L451: `version: number | string;`
- L454: `export interface RenameSubjectRequest {`
- L457: `version: number | string;`
- L460: `export interface ScheduleRuleDto {`
- L472: `version: number | string;`
- L475: `export interface ScheduleRuleListResponse {`
- L479: `export interface SelectCentreRequest {`
- L483: `export interface SessionVersionRequest {`
- L485: `version: number | string;`
- L488: `export interface SetTeacherAccountRequest {`
- L492: `version: number | string;`
- L495: `export interface StaffMemberDto {`
- L503: `version: number | string;`
- L507: `export interface StaffListResponse {`
- L511: `export interface StaffVersionRequest {`
- L513: `version: number | string;`
- L516: `export interface StudentParentDto {`
- L524: `export interface StudentDetailDto {`
- L533: `version: number | string;`
- L537: `export interface StudentEnrollmentDto {`
- L549: `version: number | string;`
- L552: `export interface StudentEnrollmentListResponse {`
- L556: `export interface StudentListItemDto {`
- L568: `export interface StudentPageDto {`
- L574: `export interface StudentVersionRequest {`
- L576: `version: number | string;`
- L579: `export type SubjectStatus = typeof SubjectStatus[keyof typeof SubjectStatus];`
- L582: `export const SubjectStatus = {`
- L587: `export interface SubjectDto {`
- L592: `version: number | string;`
- L595: `export interface SubjectListResponse {`
- L599: `export interface SubjectVersionRequest {`
- L601: `version: number | string;`
- L604: `export interface SystemInfoDto {`
- L612: `export type TeacherStatus = typeof TeacherStatus[keyof typeof TeacherStatus];`
- L615: `export const TeacherStatus = {`
- L620: `export interface TeacherDto {`
- L633: `version: number | string;`
- L636: `export interface TeacherListResponse {`
- L640: `export interface TeacherVersionRequest {`
- L642: `version: number | string;`
- L645: `export interface TimetableSessionDto {`
- L659: `version: number | string;`
- L662: `export interface TimetableDto {`
- L668: `export interface TransferEnrollmentRequest {`
- L671: `version: number | string;`
- L674: `export interface TransferEnrollmentResponse {`
- L678: `export interface UpdateCentreSettingsRequest {`
- L682: `version: number | string;`
- L685: `export interface UpdateParentRequest {`
- L689: `version: number | string;`
- L692: `export interface UpdateStudentRequest {`
- L699: `version: number | string;`
- L702: `export interface UpdateTeacherRequest {`
- L707: `version: number | string;`
- L710: `export type ListSubjectsParams = {`
- L714: `export type ListTeachersParams = {`
- L718: `export type ListGroupsParams = {`
- L724: `export type GetTimetableParams = {`
- L730: `export type ListStudentsParams = {`
- L740: `export type GetAuditLogParams = {`
- L757: `const withQueryKey = <T extends object, K>(query: T, queryKey: K): T & { queryKey: K } => {`
- L758: `const result = { queryKey } as T & { queryKey: K };`
- L772: `export const getGetSystemInfoUrl = () => {`
- L780: `export const getSystemInfo = async ( options?: Parameters<typeof apiFetch>[1]): Promise<SystemInfoDto> => {`
- L795: `export const getGetSystemInfoQueryKey = () => {`
- L802: `export const getGetSystemInfoQueryOptions = <TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>( options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getSystemInfo>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L817: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getSystemInfo>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L820: `export type GetSystemInfoQueryResult = NonNullable<Awaited<ReturnType<typeof getSystemInfo>>>`
- L821: `export type GetSystemInfoQueryError = ProblemDetails`
- L824: `export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- L833: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L834: `export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- L843: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L844: `export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- L847: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L849: `export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- L852: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L856: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L867: `export const getGetAntiforgeryTokenUrl = () => {`
- L875: `export const getAntiforgeryToken = async ( options?: Parameters<typeof apiFetch>[1]): Promise<AntiforgeryTokenResponse> => {`
- L890: `export const getGetAntiforgeryTokenQueryKey = () => {`
- L897: `export const getGetAntiforgeryTokenQueryOptions = <TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>( options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getAntiforgeryToken>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L912: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getAntiforgeryToken>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L915: `export type GetAntiforgeryTokenQueryResult = NonNullable<Awaited<ReturnType<typeof getAntiforgeryToken>>>`
- L916: `export type GetAntiforgeryTokenQueryError = unknown`
- L919: `export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- L928: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L929: `export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- L938: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L939: `export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- L942: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L944: `export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- L947: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L951: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L962: `export const getLoginUrl = () => {`
- L970: `export const login = async (loginRequest: LoginRequest, options?: Parameters<typeof apiFetch>[1]): Promise<MeDto> => {`
- L999: `export const getLoginMutationKey = () => ['login'] as const;`
- L1001: `export const getLoginMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L1005: `const mutationKey = getLoginMutationKey();`
- L1031: `export type LoginMutationVariables = {data: LoginRequest}`
- L1041: `return useMutation(getLoginMutationOptions(options), queryClient);`
- L1044: `export const getLogoutUrl = () => {`
- L1052: `export const logout = async ( options?: Parameters<typeof apiFetch>[1]): Promise<void> => {`
- L1067: `export const getLogoutMutationKey = () => ['logout'] as const;`
- L1069: `export const getLogoutMutationOptions = <TError = ProblemDetails,`
- L1073: `const mutationKey = getLogoutMutationKey();`
- L1109: `return useMutation(getLogoutMutationOptions(options), queryClient);`
- L1112: `export const getGetMeUrl = () => {`
- L1120: `export const getMe = async ( options?: Parameters<typeof apiFetch>[1]): Promise<MeDto> => {`
- L1135: `export const getGetMeQueryKey = () => {`
- L1142: `export const getGetMeQueryOptions = <TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>( options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getMe>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L1157: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getMe>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1160: `export type GetMeQueryResult = NonNullable<Awaited<ReturnType<typeof getMe>>>`
- L1161: `export type GetMeQueryError = ProblemDetails`
- L1164: `export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- L1173: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1174: `export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- L1183: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1184: `export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- L1187: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1189: `export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- L1192: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L1196: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L1207: `export const getSelectCentreUrl = () => {`
- L1215: `export const selectCentre = async (selectCentreRequest: SelectCentreRequest, options?: Parameters<typeof apiFetch>[1]): Promise<void> => {`
- L1244: `export const getSelectCentreMutationKey = () => ['selectCentre'] as const;`
- L1246: `export const getSelectCentreMutationOptions = <TError = ProblemDetails,`
- L1250: `const mutationKey = getSelectCentreMutationKey();`
- L1276: `export type SelectCentreMutationVariables = {data: SelectCentreRequest}`
- L1286: `return useMutation(getSelectCentreMutationOptions(options), queryClient);`
- L1289: `export const getChangePasswordUrl = () => {`
- L1297: `export const changePassword = async (changePasswordRequest: ChangePasswordRequest, options?: Parameters<typeof apiFetch>[1]): Promise<void> => {`
- L1326: `export const getChangePasswordMutationKey = () => ['changePassword'] as const;`
- L1328: `export const getChangePasswordMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L1332: `const mutationKey = getChangePasswordMutationKey();`
- L1358: `export type ChangePasswordMutationVariables = {data: ChangePasswordRequest}`
- L1368: `return useMutation(getChangePasswordMutationOptions(options), queryClient);`
- L1371: `export const getListSubjectsUrl = (params?: ListSubjectsParams,) => {`
- L1386: `export const listSubjects = async (params?: ListSubjectsParams, options?: Parameters<typeof apiFetch>[1]): Promise<SubjectListResponse> => {`
- L1401: `export const getListSubjectsQueryKey = (params?: ListSubjectsParams,) => {`
- L1408: `export const getListSubjectsQueryOptions = <TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(params?: ListSubjectsParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listSubjects>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L1423: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listSubjects>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1426: `export type ListSubjectsQueryResult = NonNullable<Awaited<ReturnType<typeof listSubjects>>>`
- L1427: `export type ListSubjectsQueryError = ProblemDetails`
- L1430: `export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- L1439: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1440: `export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- L1449: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1450: `export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- L1453: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1455: `export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- L1458: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L1462: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L1473: `export const getCreateSubjectUrl = () => {`
- L1481: `export const createSubject = async (createSubjectRequest: CreateSubjectRequest, options?: Parameters<typeof apiFetch>[1]): Promise<CreateSubjectResponse> => {`
- L1510: `export const getCreateSubjectMutationKey = () => ['createSubject'] as const;`
- L1512: `export const getCreateSubjectMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L1516: `const mutationKey = getCreateSubjectMutationKey();`
- L1542: `export type CreateSubjectMutationVariables = {data: CreateSubjectRequest}`
- L1552: `return useMutation(getCreateSubjectMutationOptions(options), queryClient);`
- L1555: `export const getGetSubjectUrl = (id: string,) => {`
- L1563: `export const getSubject = async (id: string, options?: Parameters<typeof apiFetch>[1]): Promise<SubjectDto> => {`
- L1578: `export const getGetSubjectQueryKey = (id: string,) => {`
- L1585: `export const getGetSubjectQueryOptions = <TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(id: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getSubject>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L1600: `return  { queryKey, queryFn, enabled: id !== null && id !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getSubject>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1603: `export type GetSubjectQueryResult = NonNullable<Awaited<ReturnType<typeof getSubject>>>`
- L1604: `export type GetSubjectQueryError = ProblemDetails`
- L1607: `export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- L1616: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1617: `export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- L1626: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1627: `export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- L1630: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1632: `export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- L1635: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L1639: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L1650: `export const getRenameSubjectUrl = (id: string,) => {`
- L1658: `export const renameSubject = async (id: string,`
- L1688: `export const getRenameSubjectMutationKey = () => ['renameSubject'] as const;`
- L1690: `export const getRenameSubjectMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L1694: `const mutationKey = getRenameSubjectMutationKey();`
- L1720: `export type RenameSubjectMutationVariables = {id: string;data: RenameSubjectRequest}`
- L1730: `return useMutation(getRenameSubjectMutationOptions(options), queryClient);`
- L1733: `export const getArchiveSubjectUrl = (id: string,) => {`
- L1741: `export const archiveSubject = async (id: string,`
- L1771: `export const getArchiveSubjectMutationKey = () => ['archiveSubject'] as const;`
- L1773: `export const getArchiveSubjectMutationOptions = <TError = ProblemDetails,`
- L1777: `const mutationKey = getArchiveSubjectMutationKey();`
- L1803: `export type ArchiveSubjectMutationVariables = {id: string;data: SubjectVersionRequest}`
- L1813: `return useMutation(getArchiveSubjectMutationOptions(options), queryClient);`
- L1816: `export const getRestoreSubjectUrl = (id: string,) => {`
- L1824: `export const restoreSubject = async (id: string,`
- L1854: `export const getRestoreSubjectMutationKey = () => ['restoreSubject'] as const;`
- L1856: `export const getRestoreSubjectMutationOptions = <TError = ProblemDetails,`
- L1860: `const mutationKey = getRestoreSubjectMutationKey();`
- L1886: `export type RestoreSubjectMutationVariables = {id: string;data: SubjectVersionRequest}`
- L1896: `return useMutation(getRestoreSubjectMutationOptions(options), queryClient);`
- L1899: `export const getListTeachersUrl = (params?: ListTeachersParams,) => {`
- L1914: `export const listTeachers = async (params?: ListTeachersParams, options?: Parameters<typeof apiFetch>[1]): Promise<TeacherListResponse> => {`
- L1929: `export const getListTeachersQueryKey = (params?: ListTeachersParams,) => {`
- L1936: `export const getListTeachersQueryOptions = <TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(params?: ListTeachersParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listTeachers>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L1951: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listTeachers>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1954: `export type ListTeachersQueryResult = NonNullable<Awaited<ReturnType<typeof listTeachers>>>`
- L1955: `export type ListTeachersQueryError = ProblemDetails`
- L1958: `export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- L1967: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1968: `export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- L1977: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1978: `export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- L1981: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L1983: `export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- L1986: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L1990: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L2001: `export const getCreateTeacherUrl = () => {`
- L2009: `export const createTeacher = async (createTeacherRequest: CreateTeacherRequest, options?: Parameters<typeof apiFetch>[1]): Promise<CreateTeacherResponse> => {`
- L2038: `export const getCreateTeacherMutationKey = () => ['createTeacher'] as const;`
- L2040: `export const getCreateTeacherMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2044: `const mutationKey = getCreateTeacherMutationKey();`
- L2070: `export type CreateTeacherMutationVariables = {data: CreateTeacherRequest}`
- L2080: `return useMutation(getCreateTeacherMutationOptions(options), queryClient);`
- L2083: `export const getGetTeacherUrl = (id: string,) => {`
- L2091: `export const getTeacher = async (id: string, options?: Parameters<typeof apiFetch>[1]): Promise<TeacherDto> => {`
- L2106: `export const getGetTeacherQueryKey = (id: string,) => {`
- L2113: `export const getGetTeacherQueryOptions = <TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(id: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getTeacher>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L2128: `return  { queryKey, queryFn, enabled: id !== null && id !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getTeacher>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2131: `export type GetTeacherQueryResult = NonNullable<Awaited<ReturnType<typeof getTeacher>>>`
- L2132: `export type GetTeacherQueryError = ProblemDetails`
- L2135: `export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- L2144: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2145: `export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- L2154: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2155: `export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- L2158: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2160: `export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- L2163: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L2167: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L2178: `export const getUpdateTeacherUrl = (id: string,) => {`
- L2186: `export const updateTeacher = async (id: string,`
- L2216: `export const getUpdateTeacherMutationKey = () => ['updateTeacher'] as const;`
- L2218: `export const getUpdateTeacherMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2222: `const mutationKey = getUpdateTeacherMutationKey();`
- L2248: `export type UpdateTeacherMutationVariables = {id: string;data: UpdateTeacherRequest}`
- L2258: `return useMutation(getUpdateTeacherMutationOptions(options), queryClient);`
- L2261: `export const getSetTeacherAccountUrl = (id: string,) => {`
- L2269: `export const setTeacherAccount = async (id: string,`
- L2299: `export const getSetTeacherAccountMutationKey = () => ['setTeacherAccount'] as const;`
- L2301: `export const getSetTeacherAccountMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2305: `const mutationKey = getSetTeacherAccountMutationKey();`
- L2331: `export type SetTeacherAccountMutationVariables = {id: string;data: SetTeacherAccountRequest}`
- L2341: `return useMutation(getSetTeacherAccountMutationOptions(options), queryClient);`
- L2344: `export const getArchiveTeacherUrl = (id: string,) => {`
- L2352: `export const archiveTeacher = async (id: string,`
- L2382: `export const getArchiveTeacherMutationKey = () => ['archiveTeacher'] as const;`
- L2384: `export const getArchiveTeacherMutationOptions = <TError = ProblemDetails,`
- L2388: `const mutationKey = getArchiveTeacherMutationKey();`
- L2414: `export type ArchiveTeacherMutationVariables = {id: string;data: TeacherVersionRequest}`
- L2424: `return useMutation(getArchiveTeacherMutationOptions(options), queryClient);`
- L2427: `export const getRestoreTeacherUrl = (id: string,) => {`
- L2435: `export const restoreTeacher = async (id: string,`
- L2465: `export const getRestoreTeacherMutationKey = () => ['restoreTeacher'] as const;`
- L2467: `export const getRestoreTeacherMutationOptions = <TError = ProblemDetails,`
- L2471: `const mutationKey = getRestoreTeacherMutationKey();`
- L2497: `export type RestoreTeacherMutationVariables = {id: string;data: TeacherVersionRequest}`
- L2507: `return useMutation(getRestoreTeacherMutationOptions(options), queryClient);`
- L2510: `export const getListGroupsUrl = (params?: ListGroupsParams,) => {`
- L2525: `export const listGroups = async (params?: ListGroupsParams, options?: Parameters<typeof apiFetch>[1]): Promise<GroupListResponse> => {`
- L2540: `export const getListGroupsQueryKey = (params?: ListGroupsParams,) => {`
- L2547: `export const getListGroupsQueryOptions = <TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(params?: ListGroupsParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listGroups>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L2562: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listGroups>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2565: `export type ListGroupsQueryResult = NonNullable<Awaited<ReturnType<typeof listGroups>>>`
- L2566: `export type ListGroupsQueryError = ProblemDetails`
- L2569: `export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- L2578: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2579: `export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- L2588: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2589: `export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- L2592: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2594: `export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- L2597: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L2601: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L2612: `export const getCreateGroupUrl = () => {`
- L2620: `export const createGroup = async (createGroupRequest: CreateGroupRequest, options?: Parameters<typeof apiFetch>[1]): Promise<CreateGroupResponse> => {`
- L2649: `export const getCreateGroupMutationKey = () => ['createGroup'] as const;`
- L2651: `export const getCreateGroupMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2655: `const mutationKey = getCreateGroupMutationKey();`
- L2681: `export type CreateGroupMutationVariables = {data: CreateGroupRequest}`
- L2691: `return useMutation(getCreateGroupMutationOptions(options), queryClient);`
- L2694: `export const getGetGroupUrl = (id: string,) => {`
- L2702: `export const getGroup = async (id: string, options?: Parameters<typeof apiFetch>[1]): Promise<GroupDto> => {`
- L2717: `export const getGetGroupQueryKey = (id: string,) => {`
- L2724: `export const getGetGroupQueryOptions = <TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(id: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getGroup>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L2739: `return  { queryKey, queryFn, enabled: id !== null && id !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getGroup>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2742: `export type GetGroupQueryResult = NonNullable<Awaited<ReturnType<typeof getGroup>>>`
- L2743: `export type GetGroupQueryError = ProblemDetails`
- L2746: `export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- L2755: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2756: `export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- L2765: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2766: `export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- L2769: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L2771: `export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- L2774: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L2778: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L2789: `export const getRenameGroupUrl = (id: string,) => {`
- L2797: `export const renameGroup = async (id: string,`
- L2827: `export const getRenameGroupMutationKey = () => ['renameGroup'] as const;`
- L2829: `export const getRenameGroupMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2833: `const mutationKey = getRenameGroupMutationKey();`
- L2859: `export type RenameGroupMutationVariables = {id: string;data: RenameGroupRequest}`
- L2869: `return useMutation(getRenameGroupMutationOptions(options), queryClient);`
- L2872: `export const getChangeGroupCapacityUrl = (id: string,) => {`
- L2880: `export const changeGroupCapacity = async (id: string,`
- L2910: `export const getChangeGroupCapacityMutationKey = () => ['changeGroupCapacity'] as const;`
- L2912: `export const getChangeGroupCapacityMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2916: `const mutationKey = getChangeGroupCapacityMutationKey();`
- L2942: `export type ChangeGroupCapacityMutationVariables = {id: string;data: ChangeGroupCapacityRequest}`
- L2952: `return useMutation(getChangeGroupCapacityMutationOptions(options), queryClient);`
- L2955: `export const getAssignGroupTeacherUrl = (id: string,) => {`
- L2963: `export const assignGroupTeacher = async (id: string,`
- L2993: `export const getAssignGroupTeacherMutationKey = () => ['assignGroupTeacher'] as const;`
- L2995: `export const getAssignGroupTeacherMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L2999: `const mutationKey = getAssignGroupTeacherMutationKey();`
- L3025: `export type AssignGroupTeacherMutationVariables = {id: string;data: AssignGroupTeacherRequest}`
- L3035: `return useMutation(getAssignGroupTeacherMutationOptions(options), queryClient);`
- L3038: `export const getArchiveGroupUrl = (id: string,) => {`
- L3046: `export const archiveGroup = async (id: string,`
- L3076: `export const getArchiveGroupMutationKey = () => ['archiveGroup'] as const;`
- L3078: `export const getArchiveGroupMutationOptions = <TError = ProblemDetails,`
- L3082: `const mutationKey = getArchiveGroupMutationKey();`
- L3108: `export type ArchiveGroupMutationVariables = {id: string;data: GroupVersionRequest}`
- L3118: `return useMutation(getArchiveGroupMutationOptions(options), queryClient);`
- L3121: `export const getRestoreGroupUrl = (id: string,) => {`
- L3129: `export const restoreGroup = async (id: string,`
- L3159: `export const getRestoreGroupMutationKey = () => ['restoreGroup'] as const;`
- L3161: `export const getRestoreGroupMutationOptions = <TError = ProblemDetails,`
- L3165: `const mutationKey = getRestoreGroupMutationKey();`
- L3191: `export type RestoreGroupMutationVariables = {id: string;data: GroupVersionRequest}`
- L3201: `return useMutation(getRestoreGroupMutationOptions(options), queryClient);`
- L3204: `export const getEnrollStudentUrl = (groupId: string,) => {`
- L3212: `export const enrollStudent = async (groupId: string,`
- L3242: `export const getEnrollStudentMutationKey = () => ['enrollStudent'] as const;`
- L3244: `export const getEnrollStudentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3248: `const mutationKey = getEnrollStudentMutationKey();`
- L3274: `export type EnrollStudentMutationVariables = {groupId: string;data: EnrollStudentRequest}`
- L3284: `return useMutation(getEnrollStudentMutationOptions(options), queryClient);`
- L3287: `export const getGetGroupRosterUrl = (groupId: string,) => {`
- L3295: `export const getGroupRoster = async (groupId: string, options?: Parameters<typeof apiFetch>[1]): Promise<GroupRosterDto> => {`
- L3310: `export const getGetGroupRosterQueryKey = (groupId: string,) => {`
- L3317: `export const getGetGroupRosterQueryOptions = <TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(groupId: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getGroupRoster>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L3332: `return  { queryKey, queryFn, enabled: groupId !== null && groupId !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getGroupRoster>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3335: `export type GetGroupRosterQueryResult = NonNullable<Awaited<ReturnType<typeof getGroupRoster>>>`
- L3336: `export type GetGroupRosterQueryError = ProblemDetails`
- L3339: `export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- L3348: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3349: `export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- L3358: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3359: `export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- L3362: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3364: `export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- L3367: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L3371: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L3382: `export const getListStudentEnrollmentsUrl = (studentId: string,) => {`
- L3390: `export const listStudentEnrollments = async (studentId: string, options?: Parameters<typeof apiFetch>[1]): Promise<StudentEnrollmentListResponse> => {`
- L3405: `export const getListStudentEnrollmentsQueryKey = (studentId: string,) => {`
- L3412: `export const getListStudentEnrollmentsQueryOptions = <TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(studentId: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listStudentEnrollments>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L3427: `return  { queryKey, queryFn, enabled: studentId !== null && studentId !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listStudentEnrollments>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3430: `export type ListStudentEnrollmentsQueryResult = NonNullable<Awaited<ReturnType<typeof listStudentEnrollments>>>`
- L3431: `export type ListStudentEnrollmentsQueryError = ProblemDetails`
- L3434: `export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- L3443: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3444: `export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- L3453: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3454: `export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- L3457: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3459: `export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- L3462: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L3466: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L3477: `export const getWithdrawEnrollmentUrl = (id: string,) => {`
- L3485: `export const withdrawEnrollment = async (id: string,`
- L3515: `export const getWithdrawEnrollmentMutationKey = () => ['withdrawEnrollment'] as const;`
- L3517: `export const getWithdrawEnrollmentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3521: `const mutationKey = getWithdrawEnrollmentMutationKey();`
- L3547: `export type WithdrawEnrollmentMutationVariables = {id: string;data: EnrollmentVersionRequest}`
- L3557: `return useMutation(getWithdrawEnrollmentMutationOptions(options), queryClient);`
- L3560: `export const getTransferEnrollmentUrl = (id: string,) => {`
- L3568: `export const transferEnrollment = async (id: string,`
- L3598: `export const getTransferEnrollmentMutationKey = () => ['transferEnrollment'] as const;`
- L3600: `export const getTransferEnrollmentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3604: `const mutationKey = getTransferEnrollmentMutationKey();`
- L3630: `export type TransferEnrollmentMutationVariables = {id: string;data: TransferEnrollmentRequest}`
- L3640: `return useMutation(getTransferEnrollmentMutationOptions(options), queryClient);`
- L3643: `export const getListGroupScheduleRulesUrl = (groupId: string,) => {`
- L3651: `export const listGroupScheduleRules = async (groupId: string, options?: Parameters<typeof apiFetch>[1]): Promise<ScheduleRuleListResponse> => {`
- L3666: `export const getListGroupScheduleRulesQueryKey = (groupId: string,) => {`
- L3673: `export const getListGroupScheduleRulesQueryOptions = <TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(groupId: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listGroupScheduleRules>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L3688: `return  { queryKey, queryFn, enabled: groupId !== null && groupId !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listGroupScheduleRules>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3691: `export type ListGroupScheduleRulesQueryResult = NonNullable<Awaited<ReturnType<typeof listGroupScheduleRules>>>`
- L3692: `export type ListGroupScheduleRulesQueryError = ProblemDetails`
- L3695: `export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- L3704: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3705: `export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- L3714: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3715: `export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- L3718: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L3720: `export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- L3723: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L3727: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L3738: `export const getAddScheduleRuleUrl = (groupId: string,) => {`
- L3746: `export const addScheduleRule = async (groupId: string,`
- L3776: `export const getAddScheduleRuleMutationKey = () => ['addScheduleRule'] as const;`
- L3778: `export const getAddScheduleRuleMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3782: `const mutationKey = getAddScheduleRuleMutationKey();`
- L3808: `export type AddScheduleRuleMutationVariables = {groupId: string;data: AddScheduleRuleRequest}`
- L3818: `return useMutation(getAddScheduleRuleMutationOptions(options), queryClient);`
- L3821: `export const getEndScheduleRuleUrl = (id: string,) => {`
- L3829: `export const endScheduleRule = async (id: string,`
- L3859: `export const getEndScheduleRuleMutationKey = () => ['endScheduleRule'] as const;`
- L3861: `export const getEndScheduleRuleMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3865: `const mutationKey = getEndScheduleRuleMutationKey();`
- L3891: `export type EndScheduleRuleMutationVariables = {id: string;data: EndScheduleRuleRequest}`
- L3901: `return useMutation(getEndScheduleRuleMutationOptions(options), queryClient);`
- L3904: `export const getExtendScheduleUrl = () => {`
- L3912: `export const extendSchedule = async (extendScheduleRequest: ExtendScheduleRequest, options?: Parameters<typeof apiFetch>[1]): Promise<ExtendScheduleResponse> => {`
- L3941: `export const getExtendScheduleMutationKey = () => ['extendSchedule'] as const;`
- L3943: `export const getExtendScheduleMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L3947: `const mutationKey = getExtendScheduleMutationKey();`
- L3973: `export type ExtendScheduleMutationVariables = {data: ExtendScheduleRequest}`
- L3983: `return useMutation(getExtendScheduleMutationOptions(options), queryClient);`
- L3986: `export const getGetTimetableUrl = (params?: GetTimetableParams,) => {`
- L4001: `export const getTimetable = async (params?: GetTimetableParams, options?: Parameters<typeof apiFetch>[1]): Promise<TimetableDto> => {`
- L4016: `export const getGetTimetableQueryKey = (params?: GetTimetableParams,) => {`
- L4023: `export const getGetTimetableQueryOptions = <TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(params?: GetTimetableParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getTimetable>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L4038: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getTimetable>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4041: `export type GetTimetableQueryResult = NonNullable<Awaited<ReturnType<typeof getTimetable>>>`
- L4042: `export type GetTimetableQueryError = HttpValidationProblemDetails | ProblemDetails`
- L4045: `export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4054: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4055: `export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4064: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4065: `export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4068: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4070: `export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4073: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L4077: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L4088: `export const getCancelSessionUrl = (id: string,) => {`
- L4096: `export const cancelSession = async (id: string,`
- L4126: `export const getCancelSessionMutationKey = () => ['cancelSession'] as const;`
- L4128: `export const getCancelSessionMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4132: `const mutationKey = getCancelSessionMutationKey();`
- L4158: `export type CancelSessionMutationVariables = {id: string;data: SessionVersionRequest}`
- L4168: `return useMutation(getCancelSessionMutationOptions(options), queryClient);`
- L4171: `export const getCreateParentUrl = () => {`
- L4179: `export const createParent = async (createParentRequest: CreateParentRequest, options?: Parameters<typeof apiFetch>[1]): Promise<CreateParentResponse> => {`
- L4208: `export const getCreateParentMutationKey = () => ['createParent'] as const;`
- L4210: `export const getCreateParentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4214: `const mutationKey = getCreateParentMutationKey();`
- L4240: `export type CreateParentMutationVariables = {data: CreateParentRequest}`
- L4250: `return useMutation(getCreateParentMutationOptions(options), queryClient);`
- L4253: `export const getGetParentUrl = (id: string,) => {`
- L4261: `export const getParent = async (id: string, options?: Parameters<typeof apiFetch>[1]): Promise<ParentDetailDto> => {`
- L4276: `export const getGetParentQueryKey = (id: string,) => {`
- L4283: `export const getGetParentQueryOptions = <TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(id: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getParent>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L4298: `return  { queryKey, queryFn, enabled: id !== null && id !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getParent>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4301: `export type GetParentQueryResult = NonNullable<Awaited<ReturnType<typeof getParent>>>`
- L4302: `export type GetParentQueryError = ProblemDetails`
- L4305: `export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- L4314: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4315: `export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- L4324: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4325: `export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- L4328: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4330: `export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- L4333: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L4337: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L4348: `export const getUpdateParentUrl = (id: string,) => {`
- L4356: `export const updateParent = async (id: string,`
- L4386: `export const getUpdateParentMutationKey = () => ['updateParent'] as const;`
- L4388: `export const getUpdateParentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4392: `const mutationKey = getUpdateParentMutationKey();`
- L4418: `export type UpdateParentMutationVariables = {id: string;data: UpdateParentRequest}`
- L4428: `return useMutation(getUpdateParentMutationOptions(options), queryClient);`
- L4431: `export const getLookupParentUrl = () => {`
- L4439: `export const lookupParent = async (lookupParentRequest: LookupParentRequest, options?: Parameters<typeof apiFetch>[1]): Promise<ParentLookupDto> => {`
- L4468: `export const getLookupParentMutationKey = () => ['lookupParent'] as const;`
- L4470: `export const getLookupParentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4474: `const mutationKey = getLookupParentMutationKey();`
- L4500: `export type LookupParentMutationVariables = {data: LookupParentRequest}`
- L4510: `return useMutation(getLookupParentMutationOptions(options), queryClient);`
- L4513: `export const getListStudentsUrl = (params?: ListStudentsParams,) => {`
- L4528: `export const listStudents = async (params?: ListStudentsParams, options?: Parameters<typeof apiFetch>[1]): Promise<StudentPageDto> => {`
- L4543: `export const getListStudentsQueryKey = (params?: ListStudentsParams,) => {`
- L4550: `export const getListStudentsQueryOptions = <TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(params?: ListStudentsParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listStudents>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L4565: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listStudents>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4568: `export type ListStudentsQueryResult = NonNullable<Awaited<ReturnType<typeof listStudents>>>`
- L4569: `export type ListStudentsQueryError = HttpValidationProblemDetails | ProblemDetails`
- L4572: `export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4581: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4582: `export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4591: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4592: `export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4595: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4597: `export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L4600: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L4604: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L4615: `export const getRegisterStudentUrl = () => {`
- L4623: `export const registerStudent = async (registerStudentRequest: RegisterStudentRequest, options?: Parameters<typeof apiFetch>[1]): Promise<RegisterStudentResponse> => {`
- L4652: `export const getRegisterStudentMutationKey = () => ['registerStudent'] as const;`
- L4654: `export const getRegisterStudentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4658: `const mutationKey = getRegisterStudentMutationKey();`
- L4684: `export type RegisterStudentMutationVariables = {data: RegisterStudentRequest}`
- L4694: `return useMutation(getRegisterStudentMutationOptions(options), queryClient);`
- L4697: `export const getGetStudentUrl = (id: string,) => {`
- L4705: `export const getStudent = async (id: string, options?: Parameters<typeof apiFetch>[1]): Promise<StudentDetailDto> => {`
- L4720: `export const getGetStudentQueryKey = (id: string,) => {`
- L4727: `export const getGetStudentQueryOptions = <TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(id: string, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getStudent>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L4742: `return  { queryKey, queryFn, enabled: id !== null && id !== undefined, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getStudent>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4745: `export type GetStudentQueryResult = NonNullable<Awaited<ReturnType<typeof getStudent>>>`
- L4746: `export type GetStudentQueryError = ProblemDetails`
- L4749: `export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- L4758: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4759: `export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- L4768: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4769: `export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- L4772: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L4774: `export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- L4777: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L4781: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L4792: `export const getUpdateStudentUrl = (id: string,) => {`
- L4800: `export const updateStudent = async (id: string,`
- L4830: `export const getUpdateStudentMutationKey = () => ['updateStudent'] as const;`
- L4832: `export const getUpdateStudentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L4836: `const mutationKey = getUpdateStudentMutationKey();`
- L4862: `export type UpdateStudentMutationVariables = {id: string;data: UpdateStudentRequest}`
- L4872: `return useMutation(getUpdateStudentMutationOptions(options), queryClient);`
- L4875: `export const getArchiveStudentUrl = (id: string,) => {`
- L4883: `export const archiveStudent = async (id: string,`
- L4913: `export const getArchiveStudentMutationKey = () => ['archiveStudent'] as const;`
- L4915: `export const getArchiveStudentMutationOptions = <TError = ProblemDetails,`
- L4919: `const mutationKey = getArchiveStudentMutationKey();`
- L4945: `export type ArchiveStudentMutationVariables = {id: string;data: StudentVersionRequest}`
- L4955: `return useMutation(getArchiveStudentMutationOptions(options), queryClient);`
- L4958: `export const getRestoreStudentUrl = (id: string,) => {`
- L4966: `export const restoreStudent = async (id: string,`
- L4996: `export const getRestoreStudentMutationKey = () => ['restoreStudent'] as const;`
- L4998: `export const getRestoreStudentMutationOptions = <TError = ProblemDetails,`
- L5002: `const mutationKey = getRestoreStudentMutationKey();`
- L5028: `export type RestoreStudentMutationVariables = {id: string;data: StudentVersionRequest}`
- L5038: `return useMutation(getRestoreStudentMutationOptions(options), queryClient);`
- L5041: `export const getLinkStudentParentUrl = (id: string,) => {`
- L5049: `export const linkStudentParent = async (id: string,`
- L5079: `export const getLinkStudentParentMutationKey = () => ['linkStudentParent'] as const;`
- L5081: `export const getLinkStudentParentMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L5085: `const mutationKey = getLinkStudentParentMutationKey();`
- L5111: `export type LinkStudentParentMutationVariables = {id: string;data: LinkStudentParentRequest}`
- L5121: `return useMutation(getLinkStudentParentMutationOptions(options), queryClient);`
- L5124: `export const getUnlinkStudentParentUrl = (id: string,`
- L5133: `export const unlinkStudentParent = async (id: string,`
- L5164: `export const getUnlinkStudentParentMutationKey = () => ['unlinkStudentParent'] as const;`
- L5166: `export const getUnlinkStudentParentMutationOptions = <TError = ProblemDetails,`
- L5170: `const mutationKey = getUnlinkStudentParentMutationKey();`
- L5196: `export type UnlinkStudentParentMutationVariables = {id: string;parentId: string;data: StudentVersionRequest}`
- L5206: `return useMutation(getUnlinkStudentParentMutationOptions(options), queryClient);`
- L5209: `export const getMakeStudentParentPrimaryUrl = (id: string,`
- L5218: `export const makeStudentParentPrimary = async (id: string,`
- L5249: `export const getMakeStudentParentPrimaryMutationKey = () => ['makeStudentParentPrimary'] as const;`
- L5251: `export const getMakeStudentParentPrimaryMutationOptions = <TError = ProblemDetails,`
- L5255: `const mutationKey = getMakeStudentParentPrimaryMutationKey();`
- L5281: `export type MakeStudentParentPrimaryMutationVariables = {id: string;parentId: string;data: StudentVersionRequest}`
- L5291: `return useMutation(getMakeStudentParentPrimaryMutationOptions(options), queryClient);`
- L5294: `export const getListStaffUrl = () => {`
- L5302: `export const listStaff = async ( options?: Parameters<typeof apiFetch>[1]): Promise<StaffListResponse> => {`
- L5317: `export const getListStaffQueryKey = () => {`
- L5324: `export const getListStaffQueryOptions = <TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>( options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof listStaff>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L5339: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof listStaff>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5342: `export type ListStaffQueryResult = NonNullable<Awaited<ReturnType<typeof listStaff>>>`
- L5343: `export type ListStaffQueryError = ProblemDetails`
- L5346: `export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- L5355: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5356: `export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- L5365: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5366: `export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- L5369: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5371: `export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- L5374: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L5378: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L5389: `export const getCreateStaffUrl = () => {`
- L5397: `export const createStaff = async (createStaffRequest: CreateStaffRequest, options?: Parameters<typeof apiFetch>[1]): Promise<CreateStaffResponse> => {`
- L5426: `export const getCreateStaffMutationKey = () => ['createStaff'] as const;`
- L5428: `export const getCreateStaffMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L5432: `const mutationKey = getCreateStaffMutationKey();`
- L5458: `export type CreateStaffMutationVariables = {data: CreateStaffRequest}`
- L5468: `return useMutation(getCreateStaffMutationOptions(options), queryClient);`
- L5471: `export const getChangeStaffRoleUrl = (membershipId: string,) => {`
- L5479: `export const changeStaffRole = async (membershipId: string,`
- L5509: `export const getChangeStaffRoleMutationKey = () => ['changeStaffRole'] as const;`
- L5511: `export const getChangeStaffRoleMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L5515: `const mutationKey = getChangeStaffRoleMutationKey();`
- L5541: `export type ChangeStaffRoleMutationVariables = {membershipId: string;data: ChangeStaffRoleRequest}`
- L5551: `return useMutation(getChangeStaffRoleMutationOptions(options), queryClient);`
- L5554: `export const getDeactivateStaffUrl = (membershipId: string,) => {`
- L5562: `export const deactivateStaff = async (membershipId: string,`
- L5592: `export const getDeactivateStaffMutationKey = () => ['deactivateStaff'] as const;`
- L5594: `export const getDeactivateStaffMutationOptions = <TError = ProblemDetails,`
- L5598: `const mutationKey = getDeactivateStaffMutationKey();`
- L5624: `export type DeactivateStaffMutationVariables = {membershipId: string;data: StaffVersionRequest}`
- L5634: `return useMutation(getDeactivateStaffMutationOptions(options), queryClient);`
- L5637: `export const getReactivateStaffUrl = (membershipId: string,) => {`
- L5645: `export const reactivateStaff = async (membershipId: string,`
- L5675: `export const getReactivateStaffMutationKey = () => ['reactivateStaff'] as const;`
- L5677: `export const getReactivateStaffMutationOptions = <TError = ProblemDetails,`
- L5681: `const mutationKey = getReactivateStaffMutationKey();`
- L5707: `export type ReactivateStaffMutationVariables = {membershipId: string;data: StaffVersionRequest}`
- L5717: `return useMutation(getReactivateStaffMutationOptions(options), queryClient);`
- L5720: `export const getGetAuditLogUrl = (params?: GetAuditLogParams,) => {`
- L5735: `export const getAuditLog = async (params?: GetAuditLogParams, options?: Parameters<typeof apiFetch>[1]): Promise<AuditPageDto> => {`
- L5750: `export const getGetAuditLogQueryKey = (params?: GetAuditLogParams,) => {`
- L5757: `export const getGetAuditLogQueryOptions = <TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(params?: GetAuditLogParams, options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getAuditLog>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L5772: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getAuditLog>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5775: `export type GetAuditLogQueryResult = NonNullable<Awaited<ReturnType<typeof getAuditLog>>>`
- L5776: `export type GetAuditLogQueryError = HttpValidationProblemDetails | ProblemDetails`
- L5779: `export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L5788: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5789: `export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L5798: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5799: `export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L5802: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5804: `export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- L5807: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L5811: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L5822: `export const getGetCentreSettingsUrl = () => {`
- L5830: `export const getCentreSettings = async ( options?: Parameters<typeof apiFetch>[1]): Promise<CentreSettingsDto> => {`
- L5845: `export const getGetCentreSettingsQueryKey = () => {`
- L5852: `export const getGetCentreSettingsQueryOptions = <TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>( options?: { query?:Partial<UseQueryOptions<Awaited<ReturnType<typeof getCentreSettings>>, TError, TData>>, request?: SecondParameter<typeof apiFetch>}`
- L5867: `return  { queryKey, queryFn, ...queryOptions} as UseQueryOptions<Awaited<ReturnType<typeof getCentreSettings>>, TError, TData> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5870: `export type GetCentreSettingsQueryResult = NonNullable<Awaited<ReturnType<typeof getCentreSettings>>>`
- L5871: `export type GetCentreSettingsQueryError = ProblemDetails`
- L5874: `export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- L5883: `):  DefinedUseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5884: `export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- L5893: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5894: `export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- L5897: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> }`
- L5899: `export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- L5902: `):  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> } {`
- L5906: `const query = useQuery(queryOptions, queryClient) as  UseQueryResult<TData, TError> & { queryKey: DataTag<QueryKey, TData, TError> };`
- L5917: `export const getUpdateCentreSettingsUrl = () => {`
- L5925: `export const updateCentreSettings = async (updateCentreSettingsRequest: UpdateCentreSettingsRequest, options?: Parameters<typeof apiFetch>[1]): Promise<void> => {`
- L5954: `export const getUpdateCentreSettingsMutationKey = () => ['updateCentreSettings'] as const;`
- L5956: `export const getUpdateCentreSettingsMutationOptions = <TError = HttpValidationProblemDetails | ProblemDetails,`
- L5960: `const mutationKey = getUpdateCentreSettingsMutationKey();`
- L5986: `export type UpdateCentreSettingsMutationVariables = {data: UpdateCentreSettingsRequest}`
- L5996: `return useMutation(getUpdateCentreSettingsMutationOptions(options), queryClient);`

## frontend/src/api/problemDetails.test.ts
37 lines; 796 bytes; SHA256 `739d948acb738e6c506cd48f35b796dd76dc6ece8fea1a22b3ef953073c26677`.

- L4: `describe("isProblemDetails", () => {`
- L5: `it("returns true for a valid Problem Details object", () => {`
- L16: `it("returns false for a plain string", () => {`
- L27: `it("returns false when status is missing", () => {`

## frontend/src/api/problemDetails.ts
26 lines; 860 bytes; SHA256 `6cfd245257d76900b655f1d70147c0b5b8b4997ad3e1a68b0035ffd3544d2541`.

- L2: `export interface ProblemDetails {`
- L17: `export function isProblemDetails(value: unknown): value is ProblemDetails {`

## frontend/src/api/showApiError.test.ts
75 lines; 2110 bytes; SHA256 `2ba5af69315ded67a26e578904298f7fc510632f44ce1f08df849e52c2359245`.

- L5: `const toastError = vi.hoisted(() => vi.fn());`
- L8: `describe("showApiError", () => {`
- L13: `it("shows the translated message with a reference when a correlation id exists", () => {`
- L32: `it("shows no description when there is no correlation id", () => {`
- L44: `describe("applyFieldErrors", () => {`
- L45: `it("applies the first message of known fields only", () => {`
- L64: `it("does nothing when the error has no field errors", () => {`

## frontend/src/api/showApiError.ts
35 lines; 1295 bytes; SHA256 `3ecec1d6a25a0a971f8744d115a0970ad6610cc422fcb04b21d702d0d304eeaa`.

- L6: `export function showApiError(error: ApiError): void {`
- L17: `export function applyFieldErrors<TField extends string>(`

## frontend/src/app/queryClient.ts
74 lines; 2711 bytes; SHA256 `4ad5bd2e4f1a8a0bddbf2a3f0f5fbe45069cab1392e44d22725c5c27ef250a36`.

- L9: `export function setLoginRedirect(fn: LoginRedirect): void {`
- L13: `const loginMutationKey = getLoginMutationKey();`
- L15: `function isLoginMutation(mutationKey: readonly unknown[] | undefined): boolean {`
- L28: `export function createAppQueryClient(): QueryClient {`
- L74: `export const queryClient = createAppQueryClient();`

## frontend/src/components/brand/HessaLogo.tsx
62 lines; 2040 bytes; SHA256 `d31c354f29f10b9161ef3c86cb7ca40ae44f61e860dff05962952c6d5cf32b65`.

- L8: `export function HessaMark({ className }: { className?: string }) {`
- L9: `const gradientId = useId();`
- L42: `export function HessaWordmark({`

## frontend/src/components/shared/DataTable.tsx
77 lines; 2406 bytes; SHA256 `9eca60c204e1cf0fa8db17a9f7d4e95619c624737098e9e2a6360b6317f5ed0e`.

- L9: `export function DataTable({ className, children }: { className?: string; children: ReactNode }) {`
- L19: `export function DataTableHead({ children }: { children: ReactNode }) {`
- L27: `export function DataTableColumn({ children, hidden = false }: { children: ReactNode; hidden?: boolean }) {`
- L35: `export function DataTableBody({ children }: { children: ReactNode }) {`
- L44: `export function DataTableRow({ muted = false, children }: { muted?: boolean; children: ReactNode }) {`
- L56: `export function DataTableCell({`

## frontend/src/components/shared/EmptyState.tsx
32 lines; 1102 bytes; SHA256 `e2d92c18445d9205f8644317ea087887c6b3195b21f1ae43386b2142aa94a39a`.

- L9: `export function EmptyState({`

## frontend/src/components/shared/FilterBar.tsx
19 lines; 700 bytes; SHA256 `2dcdaa99d50986c07a1d5cb6b229a4d762631690b713ea576d7d0fd296705967`.

- L4: `export function FilterBar({ label, children }: { label: string; children: ReactNode }) {`
- L17: `export function FilterToggle({ children }: { children: ReactNode }) {`

## frontend/src/components/shared/LoadMoreButton.test.tsx
16 lines; 648 bytes; SHA256 `be50ba66e10d5f796f9b9878d201ebca4b2d1314b01df303ebb168217810f4bf`.

- L5: `describe("LoadMoreButton", () => {`
- L6: `it("calls onClick when pressed, and is disabled while pending", () => {`

## frontend/src/components/shared/LoadMoreButton.tsx
13 lines; 505 bytes; SHA256 `3eb0af48dbc8eb8f20bdd97820e8eb899c66cd5aa094a96ebdb0a8bc22eae06d`.

- L5: `export function LoadMoreButton({ onClick, pending }: { onClick: () => void; pending?: boolean }) {`
- L6: `const { t } = useTranslation("common");`

## frontend/src/components/shared/PageHeader.tsx
25 lines; 845 bytes; SHA256 `89f78d05a2b69a1ea43a5d95a2281109380b9093f67fb810289b7444132cbe66`.

- L7: `export function PageHeader({`

## frontend/src/components/shared/ParentLookupField.tsx
58 lines; 1832 bytes; SHA256 `c9cbe239ce4da9aceb60879c648107439cc4b9f886eb9b53f6f3bd63c6f0a519`.

- L13: `export function ParentLookupField({`

## frontend/src/components/shared/PhoneInput.test.tsx
18 lines; 626 bytes; SHA256 `f48f0ceb6d91ba009e15e778761e7403667f49bec268420c89af5ecb0d5d8e6f`.

- L5: `describe("PhoneInput", () => {`
- L6: `it("is always left-to-right, even when rendered inside an rtl document", () => {`

## frontend/src/components/shared/PhoneInput.tsx
48 lines; 1227 bytes; SHA256 `9908b0c39007a74f82c5ac602a115a68e9e4fea4123c4d22c2738d314c9d504f`.

- L9: `export function PhoneInput({`

## frontend/src/components/shared/PhoneText.test.tsx
24 lines; 803 bytes; SHA256 `908062cb0ebcc140d14aea566858bfcc2810de6bd7e4729440b90cd666bb626c`.

- L5: `describe("PhoneText", () => {`
- L6: `it("renders left-to-right even when the document is rtl", () => {`
- L17: `it("renders a tel: link when asLink is set", () => {`

## frontend/src/components/shared/PhoneText.tsx
18 lines; 835 bytes; SHA256 `cc1b93e649a8035c46df695e465cd0f951f1e5cd05895a3a2d32f3eab0b2ae12`.

- L8: `export function PhoneText({ value, asLink = false }: { value: string; asLink?: boolean }) {`

## frontend/src/components/shared/SearchInput.test.tsx
39 lines; 1308 bytes; SHA256 `a23d8a0adab63d56121ded226b7ccbd69246e4555c008ee10ef699653cd90e2c`.

- L5: `describe("SearchInput", () => {`
- L7: `vi.useFakeTimers();`
- L11: `vi.useRealTimers();`
- L14: `it("debounces typing and calls onChange exactly once after the pause", () => {`
- L30: `it("the clear button resets immediately, without waiting for the debounce", () => {`

## frontend/src/components/shared/SearchInput.tsx
87 lines; 2756 bytes; SHA256 `7bc082129353410fee64535b937ba53feba2752fda1708c85ce4d8b9e292c70c`.

- L6: `const DefaultDebounceMs = 300;`
- L14: `export function SearchInput({`
- L25: `const { t } = useTranslation("common");`
- L26: `const [draft, setDraft] = useState(value);`
- L27: `const [syncedValue, setSyncedValue] = useState(value);`
- L28: `const onChangeRef = useRef(onChange);`
- L29: `useEffect(() => {`
- L42: `useEffect(() => {`

## frontend/src/components/shared/StatTile.tsx
22 lines; 1071 bytes; SHA256 `6ca2fa844d6eeca1809522de14dc86699a6fa1ef21565b957cd1f0c926183be4`.

- L7: `export function StatTile({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: number }) {`

## frontend/src/components/shared/StatusBadge.test.tsx
15 lines; 497 bytes; SHA256 `2aafd7499b37ded6331ed0dd85cc01bdd624ccf940cb8a1c6cfb831b65b9039b`.

- L5: `describe("StatusBadge", () => {`
- L6: `it("renders the active label", () => {`
- L11: `it("renders the archived label", () => {`

## frontend/src/components/shared/StatusBadge.tsx
14 lines; 680 bytes; SHA256 `ad1b02b20871e50f084b004b777af063da722c4d2852325d060924f2b4941beb`.

- L9: `export function StatusBadge({ status }: { status: "active" | "archived" }) {`
- L10: `const { t } = useTranslation("common");`

## frontend/src/components/shared/useParentLookup.ts
76 lines; 2745 bytes; SHA256 `5ebce58b90e98e6dc10858ee55f787e69acd21f9891f5b57723465476dcc69a0`.

- L6: `export type ParentLookupOutcome = { ok: true; parent: ParentDto | null } | { ok: false };`
- L17: `export function useParentLookup() {`
- L18: `const mutation = useLookupParent();`
- L22: `const latestPhoneRef = useRef("");`
- L50: `const result = await mutation.mutateAsync({ data: { phone } });`

## frontend/src/components/ui/alert-dialog.tsx
185 lines; 5249 bytes; SHA256 `f679acf47722af0435957a886e2f70bfbdd1e9e8438f08e4285e8c53f2237997`.

- L7: `function AlertDialog({ ...props }: AlertDialogPrimitive.Root.Props) {`
- L11: `function AlertDialogTrigger({ ...props }: AlertDialogPrimitive.Trigger.Props) {`
- L17: `function AlertDialogPortal({ ...props }: AlertDialogPrimitive.Portal.Props) {`
- L23: `function AlertDialogOverlay({`
- L39: `function AlertDialogContent({`
- L62: `function AlertDialogHeader({`
- L78: `function AlertDialogFooter({`
- L94: `function AlertDialogMedia({`
- L110: `function AlertDialogTitle({`
- L126: `function AlertDialogDescription({`
- L142: `function AlertDialogAction({`
- L155: `function AlertDialogCancel({`
- L172: `export {`

## frontend/src/components/ui/alert.tsx
75 lines; 2037 bytes; SHA256 `a3521f446f0e66965a8de9ff29998d8036847286e1bb838f1a317c0709af85f8`.

- L5: `const alertVariants = cva(`
- L21: `function Alert({`
- L36: `function AlertTitle({ className, ...props }: React.ComponentProps<"div">) {`
- L49: `function AlertDescription({`
- L65: `function AlertAction({ className, ...props }: React.ComponentProps<"div">) {`
- L75: `export { Alert, AlertTitle, AlertDescription, AlertAction }`

## frontend/src/components/ui/badge-variants.ts
24 lines; 1332 bytes; SHA256 `f601a632cc1b3f237561a60cbc7ccf36d9879b4ca30cb3142c7d5dc2f5c04f92`.

- L3: `export const badgeVariants = cva(`

## frontend/src/components/ui/badge.tsx
29 lines; 666 bytes; SHA256 `846b9735c9d51f3bfb32fbd0c4b86544bc341789b5d8b3b06c3316a8993b080e`.

- L7: `function Badge({`
- L13: `return useRender({`
- L29: `export { Badge }`

## frontend/src/components/ui/button-variants.ts
38 lines; 2787 bytes; SHA256 `88403677cde11c0fcf576e4d9509d4379642b4afa17871af5ed21ee094397713`.

- L3: `export const buttonVariants = cva(`

## frontend/src/components/ui/button.tsx
21 lines; 527 bytes; SHA256 `18d4119dff18d4f8b503af04e5595adfc0bd1d2000db9df4ff620bfe42f4a0c2`.

- L6: `function Button({`
- L21: `export { Button }`

## frontend/src/components/ui/card.tsx
102 lines; 2620 bytes; SHA256 `8ad3a18d36bda0598dc0e9aa96ea22854944d5443ee4946f1d173815e115375f`.

- L4: `function Card({`
- L22: `function CardHeader({ className, ...props }: React.ComponentProps<"div">) {`
- L35: `function CardTitle({ className, ...props }: React.ComponentProps<"div">) {`
- L48: `function CardDescription({ className, ...props }: React.ComponentProps<"div">) {`
- L58: `function CardAction({ className, ...props }: React.ComponentProps<"div">) {`
- L71: `function CardContent({ className, ...props }: React.ComponentProps<"div">) {`
- L81: `function CardFooter({ className, ...props }: React.ComponentProps<"div">) {`
- L94: `export {`

## frontend/src/components/ui/dialog.tsx
163 lines; 4195 bytes; SHA256 `8913b2887b5e8ecda71ab795e88cbab36e8601bd2a10b3ef580325cfa0538925`.

- L9: `function Dialog({ ...props }: DialogPrimitive.Root.Props) {`
- L13: `function DialogTrigger({ ...props }: DialogPrimitive.Trigger.Props) {`
- L17: `function DialogPortal({ ...props }: DialogPrimitive.Portal.Props) {`
- L21: `function DialogClose({ ...props }: DialogPrimitive.Close.Props) {`
- L25: `function DialogOverlay({`
- L41: `function DialogContent({`
- L49: `const { t } = useTranslation("common")`
- L84: `function DialogHeader({ className, ...props }: React.ComponentProps<"div">) {`
- L94: `function DialogFooter({`
- L102: `const { t } = useTranslation("common")`
- L123: `function DialogTitle({ className, ...props }: DialogPrimitive.Title.Props) {`
- L136: `function DialogDescription({`
- L152: `export {`

## frontend/src/components/ui/dropdown-menu.tsx
267 lines; 8726 bytes; SHA256 `ffd69006ed9c79e16ffefffbf8a9ebbf1c34ca38f40cb9939c946a71943127d7`.

- L8: `function DropdownMenu({ ...props }: MenuPrimitive.Root.Props) {`
- L12: `function DropdownMenuPortal({ ...props }: MenuPrimitive.Portal.Props) {`
- L16: `function DropdownMenuTrigger({ ...props }: MenuPrimitive.Trigger.Props) {`
- L20: `function DropdownMenuContent({`
- L51: `function DropdownMenuGroup({ ...props }: MenuPrimitive.Group.Props) {`
- L55: `function DropdownMenuLabel({`
- L75: `function DropdownMenuItem({`
- L98: `function DropdownMenuSub({ ...props }: MenuPrimitive.SubmenuRoot.Props) {`
- L102: `function DropdownMenuSubTrigger({`
- L126: `function DropdownMenuSubContent({`
- L147: `function DropdownMenuCheckboxItem({`
- L181: `function DropdownMenuRadioGroup({ ...props }: MenuPrimitive.RadioGroup.Props) {`
- L190: `function DropdownMenuRadioItem({`
- L222: `function DropdownMenuSeparator({`
- L235: `function DropdownMenuShortcut({`
- L251: `export {`

## frontend/src/components/ui/input.tsx
19 lines; 1030 bytes; SHA256 `dab990dfefa0ba78854ab6602ad074270dba9d7d1262184d9bfbb62bd990903b`.

- L5: `function Input({ className, type, ...props }: React.ComponentProps<"input">) {`
- L19: `export { Input }`

## frontend/src/components/ui/label.tsx
17 lines; 494 bytes; SHA256 `86232709f634da7f357a8aaec0e815f3f562604bb67202c88997fae97659b450`.

- L4: `function Label({ className, ...props }: React.ComponentProps<"label">) {`
- L17: `export { Label }`

## frontend/src/components/ui/radio-group.tsx
26 lines; 1183 bytes; SHA256 `9e98ba25fc28e5763a6a48e69dfbb1848f6bcc4ebbb2afa17969171ac702334a`.

- L5: `function RadioGroup({ className, ...props }: RadioGroupPrimitive.Props) {`
- L9: `function RadioGroupItem({ className, ...props }: RadioPrimitive.Root.Props) {`
- L26: `export { RadioGroup, RadioGroupItem }`

## frontend/src/components/ui/select.tsx
198 lines; 6631 bytes; SHA256 `d3c275e0212325c0bba459800fb155839783d33d5c91d363b5b974973aa7e9e0`.

- L6: `const Select = SelectPrimitive.Root`
- L8: `function SelectGroup({ className, ...props }: SelectPrimitive.Group.Props) {`
- L18: `function SelectValue({ className, ...props }: SelectPrimitive.Value.Props) {`
- L28: `function SelectTrigger({`
- L56: `function SelectContent({`
- L95: `function SelectLabel({`
- L108: `function SelectItem({`
- L136: `function SelectSeparator({`
- L149: `function SelectScrollUpButton({`
- L168: `function SelectScrollDownButton({`
- L187: `export {`

## frontend/src/components/ui/skeleton.tsx
13 lines; 266 bytes; SHA256 `4c2af7fa9c645358a0fb188779deab6f0b981733b62befc25e18aaebea1350dc`.

- L3: `function Skeleton({ className, ...props }: React.ComponentProps<"div">) {`
- L13: `export { Skeleton }`

## frontend/src/components/ui/sonner.tsx
50 lines; 1318 bytes; SHA256 `d71c70c6b224c7ba60c60cf125a758e7fa27d1d52ea2c82d5e77a616154ec35c`.

- L6: `const Toaster = ({ ...props }: ToasterProps) => {`
- L7: `const { theme = "system" } = useTheme()`
- L8: `const { i18n } = useTranslation()`
- L50: `export { Toaster }`

## frontend/src/components/ui/switch.tsx
29 lines; 1787 bytes; SHA256 `367ad27cfe2f4ce5648fd3941410d3c3504876ee23e4360a220f1e6cd05f5c2d`.

- L4: `function Switch({`
- L29: `export { Switch }`

## frontend/src/components/ui/tabs.tsx
50 lines; 1533 bytes; SHA256 `4908b115a1af6c8454ee3b0e2d5cc481c6c9dd7768971e266af80033cc1260cf`.

- L4: `function Tabs({ className, ...props }: TabsPrimitive.Root.Props) {`
- L14: `function TabsList({ className, ...props }: TabsPrimitive.List.Props) {`
- L27: `function TabsTrigger({ className, ...props }: TabsPrimitive.Tab.Props) {`
- L40: `function TabsContent({ className, ...props }: TabsPrimitive.Panel.Props) {`
- L50: `export { Tabs, TabsList, TabsTrigger, TabsContent }`

## frontend/src/components/ui/tooltip.tsx
53 lines; 1776 bytes; SHA256 `9de7b6ef1606fd4b9b3e977a4aef0433643b6a9199175021db5861d106e8e918`.

- L5: `function TooltipProvider({ delay = 0, ...props }: TooltipPrimitive.Provider.Props) {`
- L9: `function Tooltip({ ...props }: TooltipPrimitive.Root.Props) {`
- L13: `function TooltipTrigger({ ...props }: TooltipPrimitive.Trigger.Props) {`
- L17: `function TooltipContent({`
- L51: `export { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger }`
- L53: `export type TooltipContentProps = React.ComponentProps<typeof TooltipContent>`

## frontend/src/features/audit/auditFormatting.ts
59 lines; 2075 bytes; SHA256 `7b135e56a36933235978ab86f1aa8f6a6413dca8a84f3c63defd386be3ef24b5`.

- L5: `const TranslatedValueFields = new Set(["status", "relationship", "isPrimary", "endReason", "dayOfWeek"]);`
- L8: `export const AuditEntityTypes = [`
- L22: `export type AuditEntityType = (typeof AuditEntityTypes)[number];`
- L35: `export function formatAuditValue(`

## frontend/src/features/audit/AuditLog.tsx
189 lines; 6545 bytes; SHA256 `caab8bc4929f73b074079460ff23d5626fcaa29cbfc0a068e578a2a074c26529`.

- L15: `export interface AuditFilters {`
- L22: `export function AuditLog({ filters }: { filters: AuditFilters }) {`
- L23: `const { t } = useTranslation("audit");`
- L24: `const { t: tCommon } = useTranslation("common");`
- L33: `const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({`
- L34: `queryKey: getGetAuditLogQueryKey(params),`
- L109: `function AuditRow({ item }: { item: AuditEntryDto }) {`
- L110: `const { t } = useTranslation("audit");`
- L111: `const { t: tAuth } = useTranslation("auth");`
- L112: `const { t: tCommon } = useTranslation("common");`
- L113: `const { i18n } = useTranslation();`
- L114: `const [copied, setCopied] = useState(false);`

## frontend/src/features/auth/changePasswordSchema.ts
22 lines; 932 bytes; SHA256 `29a7b66c38257168d7592d4155829a85265cc2609026645db2b3c98714e2ec45`.

- L5: `const MinPasswordLength = 10;`
- L7: `export const changePasswordFormSchema = z`
- L22: `export type ChangePasswordFormValues = z.infer<typeof changePasswordFormSchema>;`

## frontend/src/features/dashboard/DashboardOverview.test.tsx
66 lines; 3021 bytes; SHA256 `758a29b3d02705c6ef45688d51ff00b982e9f7d0703fd655030ce45c2f659d87`.

- L8: `describe("Dashboard overview", () => {`
- L28: `it("sums the server's seat counts into tiles", async () => {`
- L38: `it("lists the groups filling up, fullest first, linking to their roster", async () => {`
- L49: `it("shows only the shortcuts the session may use", async () => {`
- L56: `it("without groups.view sends no groups request and shows no seat overview", async () => {`

## frontend/src/features/dashboard/DashboardOverview.tsx
161 lines; 6876 bytes; SHA256 `7278d6542d888a561da44d8e409b6a04b496909876e8d0495448559f1c01c866`.

- L17: `const FillingUpShare = 0.8;`
- L18: `const FillingUpLimit = 5;`
- L21: `export function QuickActions() {`
- L22: `const { t } = useTranslation("shell");`
- L26: `<Can permission={Permissions.StudentsManage}>`
- L32: `<Can permission={Permissions.StudentsView}>`
- L38: `<Can permission={Permissions.GroupsView}>`
- L53: `export function GroupsOverview() {`
- L54: `const { t } = useTranslation("shell");`
- L55: `const { t: tCommon } = useTranslation("common");`
- L56: `const { data, error, isPending, refetch } = useListGroups();`

## frontend/src/features/enrollments/EnrollInGroupDialog.tsx
172 lines; 5922 bytes; SHA256 `c4bcb9177fc1c706dffa0512f40f0a7439e323b584705b50d953210ddf4c29f0`.

- L30: `export function EnrollInGroupDialog({`
- L41: `const { t } = useTranslation("enrollments");`
- L42: `const { t: tCommon } = useTranslation("common");`
- L43: `const { enroll, isEnrolling } = useEnrollmentMutations();`
- L47: `const groupList = useListGroups(`

## frontend/src/features/enrollments/enrollmentMessages.ts
24 lines; 1100 bytes; SHA256 `08d5dc37bb6c008cf6ab0bd63469c04c43ed58c07fc225815e8bd820867bbc52`.

- L9: `const OutcomeMessages: Record<string, string> = {`
- L21: `export function enrollmentMessageFor(error: ApiError): string {`

## frontend/src/features/enrollments/EnrollmentRowMenu.tsx
42 lines; 1395 bytes; SHA256 `9b01644feac3a745d997a8fdf59460459a20a775dc5f38193afed88d18b55c4c`.

- L12: `export function EnrollmentRowMenu({`
- L21: `const { t } = useTranslation("enrollments");`

## frontend/src/features/enrollments/enrollmentTestData.ts
71 lines; 2101 bytes; SHA256 `697f284e930ef2929492b58f3d5bcb98879a036a5406a008492d127f4901bb7b`.

- L3: `export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- L17: `export const secretaryMe = me("secretary", [`
- L28: `export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "subjects.view"]);`
- L30: `export function group(overrides: Record<string, unknown> = {}) {`
- L41: `version: 3,`
- L46: `export function rosterEntry(overrides: Record<string, unknown> = {}) {`
- L52: `version: 7,`
- L57: `export function roster(items = [rosterEntry(), rosterEntry({ enrollmentId: "e-2", studentId: "st-2", studentName: "Sara Ali", version: 4 })], overrides: Record<string, unknown> = {}) {`
- L61: `export function studentListItem(overrides: Record<string, unknown> = {}) {`

## frontend/src/features/enrollments/EnrollStudentDialog.test.tsx
162 lines; 6419 bytes; SHA256 `a6e74ee006ceb34bf3c280693bac180b81358ff29cae6cc5251c52d7bd99e360`.

- L10: `const enrollUrl = "/api/groups/g-1/enrollments";`
- L12: `describe("EnrollStudentDialog", () => {`
- L61: `it("searches through the existing students list with a page of 10, and shows no contact details", async () => {`
- L74: `it("enroll sends only { studentId }, then closes, toasts and refetches the roster", async () => {`
- L98: `it("students already on the roster are marked and cannot be picked", async () => {`
- L109: `it("the list is keyboard operable: arrows move, Enter picks", async () => {`
- L121: `it("409 group.full keeps the dialog open with its message and refetches the roster and the group", async () => {`
- L137: `it("409 enrollment.already_enrolled keeps the dialog open with its own message and refetches the roster", async () => {`
- L151: `it("a 404 (the group is gone) toasts and closes the dialog", async () => {`

## frontend/src/features/enrollments/EnrollStudentDialog.tsx
120 lines; 3851 bytes; SHA256 `f5fd3a16394c2faa1bc7aeeae24ae56989dba59427b568853e1aa79f3e6f46bf`.

- L25: `export function EnrollStudentDialog({`
- L36: `const { t } = useTranslation("enrollments");`
- L37: `const { enroll, isEnrolling } = useEnrollmentMutations();`

## frontend/src/features/enrollments/GroupChoiceList.tsx
66 lines; 2488 bytes; SHA256 `d1b03cd766e36c5261524451ccd2a7753e747ad3690e6e5a913a3b5bc8149c89`.

- L11: `export function GroupChoiceList({`
- L24: `const { t } = useTranslation("enrollments");`

## frontend/src/features/enrollments/GroupRosterTab.test.tsx
148 lines; 6813 bytes; SHA256 `be5ea8e9378c460d20c7fef3e81270ad65fcc9b18bfe2082f82893c6cb120fbe`.

- L10: `const rosterUrl = "/api/groups/g-1/roster";`
- L12: `describe("GroupRosterTab", () => {`
- L32: `it("shows a loading skeleton, then the roster with seats and linked names", async () => {`
- L44: `it("an empty roster says so and offers the enroll button", async () => {`
- L53: `it("a failed load shows the error with Retry, and Retry loads the roster", async () => {`
- L69: `it("with enrollments.view only (the group's teacher) shows plain names and no actions", async () => {`
- L81: `it("at capacity the enroll button is disabled with its reason, and Full is written out", async () => {`
- L94: `it("withdraw asks for confirmation naming student and group, then sends only the row's version", async () => {`
- L113: `expect(body).toStrictEqual({ version: 4 });`
- L119: `it("a stale withdraw is a toast followed by a refetch, not an error screen", async () => {`
- L137: `it("in Arabic the page runs RTL while the seat pair stays LTR", async () => {`

## frontend/src/features/enrollments/GroupRosterTab.tsx
209 lines; 7429 bytes; SHA256 `12ddb6bee8b9feb03d5a8df84dd63a764c8ae2785f8a0c16f0b7df14cb77ebbd`.

- L27: `export function GroupRosterTab({ group }: { group: GroupDto }) {`
- L28: `const { t } = useTranslation("enrollments");`
- L29: `const { t: tCommon } = useTranslation("common");`
- L30: `const linkStudents = useCan(Permissions.StudentsView);`
- L31: `const canManage = useCan(Permissions.EnrollmentsManage);`
- L32: `const { data, error, isPending, refetch } = useGetGroupRoster(group.id);`
- L33: `const [enrollOpen, setEnrollOpen] = useState(false);`
- L36: `const enrolledStudentIds = useMemo(() => new Set(data?.items.map((entry) => entry.studentId)), [data]);`
- L82: `version: entry.version,`
- L103: `{data.items.length > 0 && <Can permission={Permissions.EnrollmentsManage}>{enrollButton}</Can>}`
- L111: `<Can permission={Permissions.EnrollmentsManage}>`
- L137: `<Can permission={Permissions.EnrollmentsManage}>`
- L167: `function EnrollButton({`
- L174: `const { t } = useTranslation("enrollments");`

## frontend/src/features/enrollments/RosterTable.tsx
59 lines; 2303 bytes; SHA256 `7495fb7517e1260b427f4acb9e40f75c289ab5fa9fab399542d02b75e6ceec2b`.

- L19: `export function RosterTable({`
- L28: `const { t, i18n } = useTranslation("enrollments");`

## frontend/src/features/enrollments/SeatMeter.test.tsx
27 lines; 998 bytes; SHA256 `5427089c001cf2ded5a3fd2530142dc87cff4cc96f831bddc22eea29eb75026c`.

- L5: `describe("SeatMeter", () => {`
- L6: `it("below 80% shows only the seat pair", () => {`
- L14: `it("from 80% says Almost full in words", () => {`
- L21: `it("at capacity says Full instead", () => {`

## frontend/src/features/enrollments/SeatMeter.tsx
56 lines; 2315 bytes; SHA256 `7f960e8735c165d71e81824a3ee715bf721038a40f7d2a7b5403cc363f7c067f`.

- L6: `const AlmostFullShare = 0.8;`
- L15: `export function SeatMeter({`
- L24: `const { t } = useTranslation("enrollments");`

## frontend/src/features/enrollments/StudentEnrollmentsCard.test.tsx
162 lines; 7186 bytes; SHA256 `baed822eb2da2fd984ca71e9d0457a669d5141e1b183c9bb0934d17ced1ccb52`.

- L10: `const studentUrl = "/api/students/s-1";`
- L11: `const enrollmentsUrl = '${studentUrl}/enrollments';`
- L13: `const studentDetail = {`
- L19: `version: 1,`
- L23: `function enrollment(overrides: Record<string, unknown> = {}) {`
- L34: `version: 7,`
- L39: `const endedEnrollment = enrollment({`
- L49: `const mathA = group({ id: "g-5", name: "Math A", subjectId: "s-math", subjectName: "Math", teacherName: "Karim Adel", capacity: 10, enrolledCount: 3 });`
- L50: `const mathFull = group({ id: "g-6", name: "Math B", subjectId: "s-math", subjectName: "Math", capacity: 2, enrolledCount: 2 });`
- L52: `describe("StudentEnrollmentsCard", () => {`
- L68: `{ id: "s-math", name: "Math", status: "active", version: 1 },`
- L69: `{ id: "s-phys", name: "Physics", status: "active", version: 1 },`
- L77: `it("without enrollments.view there is no card and no enrollment request", async () => {`
- L87: `it("shows current groups first with their details, then past ones with end date and reason", async () => {`
- L102: `it("enrolls through the shared mutation: subject, then a group with seats, sending only { studentId }", async () => {`
- L132: `it("a full group answered with 409 keeps the picker open with the reason", async () => {`
- L149: `it("archiving a student who still has groups shows the specific message and keeps the student", async () => {`

## frontend/src/features/enrollments/StudentEnrollmentsCard.tsx
199 lines; 7703 bytes; SHA256 `1445e25e324f22b398ddc93ef84db81076a7a35ef98e3a6ce97b77c782ad9196`.

- L23: `export function StudentEnrollmentsCard({`
- L30: `const { t, i18n } = useTranslation("enrollments");`
- L31: `const { t: tCommon } = useTranslation("common");`
- L32: `const canManage = useCan(Permissions.EnrollmentsManage);`
- L33: `const linkGroups = useCan(Permissions.GroupsView);`
- L34: `const { data, error, isPending, refetch } = useListStudentEnrollments(student.id);`
- L35: `const [enrollOpen, setEnrollOpen] = useState(false);`
- L46: `version: item.version,`

## frontend/src/features/enrollments/StudentPicker.tsx
159 lines; 6077 bytes; SHA256 `cff5e47da96b090a66008d6fa3dd7bbf30aaf21ba1a6bffd813c881a40f90793`.

- L13: `const PickerPageSize = 10;`
- L21: `export function StudentPicker({`
- L30: `const { t } = useTranslation("enrollments");`
- L31: `const { t: tCommon } = useTranslation("common");`
- L32: `const listId = useId();`
- L33: `const [search, setSearch] = useState("");`
- L34: `const [activeIndex, setActiveIndex] = useState(0);`
- L35: `const { data, error, isPending, refetch } = useListStudents(`

## frontend/src/features/enrollments/TransferDialog.test.tsx
147 lines; 7352 bytes; SHA256 `32fcb0e9290bb9a2b8b8ae0d5e2cc178c37375d3fa023e17069dd835dbb6689c`.

- L9: `const transferUrl = "/api/enrollments/e-1/transfer";`
- L11: `const physicsB = group({ id: "g-2", name: "Physics B", teacherName: "Mona Said", capacity: 10, enrolledCount: 9 });`
- L12: `const physicsFull = group({ id: "g-3", name: "Physics C", teacherName: "Karim Adel", capacity: 5, enrolledCount: 5 });`
- L13: `const physicsArchived = group({ id: "g-4", name: "Physics Old", status: "archived" });`
- L15: `describe("TransferDialog", () => {`
- L55: `it("lists only other active groups of the same subject, with seats, and full ones disabled", async () => {`
- L68: `it("with no other group for the subject it says so", async () => {`
- L76: `it("sends one request with only { targetGroupId, version } and refreshes both rosters", async () => {`
- L103: `expect(body).toStrictEqual({ targetGroupId: "g-2", version: 7 });`
- L117: `it("409 group.full keeps the student, shows the message and refetches the target list", async () => {`
- L135: `it("a 422 subject mismatch from the server is shown in the dialog in words", async () => {`

## frontend/src/features/enrollments/TransferDialog.tsx
203 lines; 6700 bytes; SHA256 `ee3c0ed7abc6e8bfecfaaa78f1c3a3f7cd14e5cf4bb5b709d2e15f3ac8b21d2f`.

- L30: `export function TransferDialog({`
- L39: `const { t } = useTranslation("enrollments");`
- L40: `const { transfer, isTransferring } = useEnrollmentMutations();`
- L59: `version: target.version,`
- L111: `function TransferForm({`
- L130: `const { t } = useTranslation("enrollments");`
- L131: `const { t: tCommon } = useTranslation("common");`
- L133: `const sourceGroup = useGetGroup(target.groupId, { query: { enabled: knownSubjectId === undefined } });`
- L135: `const groupList = useListGroups({ subjectId }, { query: { enabled: subjectId !== undefined } });`

## frontend/src/features/enrollments/useEnrollmentMutations.ts
92 lines; 3355 bytes; SHA256 `8482436e40f45ff406139c64d776eabaa2bc63e53328f3ab5d9e93dccaaa1e35`.

- L15: `export type EnrollmentOutcome = { ok: true } | { ok: false; error: ApiError };`
- L23: `export function useEnrollmentMutations() {`
- L24: `const queryClient = useQueryClient();`
- L25: `const enrollMutation = useEnrollStudent();`
- L26: `const withdrawMutation = useWithdrawEnrollment();`
- L27: `const transferMutation = useTransferEnrollment();`
- L32: `queryClient.invalidateQueries({ queryKey: getGetGroupRosterQueryKey(groupId) }),`
- L33: `queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(groupId) }),`
- L35: `queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),`
- L36: `queryClient.invalidateQueries({ queryKey: getListStudentEnrollmentsQueryKey(studentId) }),`
- L54: `run(() => enrollMutation.mutateAsync({ groupId, data: { studentId } }), [groupId], studentId),`
- L63: `version: number | string;`
- L66: `}) => run(() => withdrawMutation.mutateAsync({ id: enrollmentId, data: { version } }), [groupId], studentId),`
- L77: `version: number | string;`
- L83: `() => transferMutation.mutateAsync({ id: enrollmentId, data: { targetGroupId, version } }),`

## frontend/src/features/enrollments/WithdrawEnrollmentDialog.tsx
87 lines; 2605 bytes; SHA256 `954ff7f523ac24e6058b649755fb91ae854876bcc78539101d90154aba045286`.

- L16: `export interface EnrollmentTarget {`
- L18: `version: number | string;`
- L30: `export function WithdrawEnrollmentDialog({`
- L37: `const { t } = useTranslation("enrollments");`
- L38: `const { withdraw, isWithdrawing } = useEnrollmentMutations();`

## frontend/src/features/groups/CreateGroupDialog.tsx
233 lines; 8686 bytes; SHA256 `c36e9b7b9372cc245ab63cd72b7aa6d45fa4385d31726743b56b267139f5df08`.

- L33: `const KnownFields = ["name", "subjectId", "teacherId", "capacity"] as const;`
- L34: `const SubjectRefusals = new Set(["group.subject_archived", "subject.not_found"]);`
- L35: `const TeacherRefusals = new Set(["group.teacher_archived", "teacher.not_found"]);`
- L37: `const EmptyValues = { name: "", subjectId: "", teacherId: "", capacity: 20 } satisfies CreateGroupFormValues;`
- L45: `export function CreateGroupDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`
- L46: `const { t } = useTranslation("groups");`
- L47: `const queryClient = useQueryClient();`
- L48: `const navigate = useNavigate();`
- L49: `const mutation = useCreateGroup();`
- L65: `data: { name: values.name, subjectId: values.subjectId, teacherId: values.teacherId, capacity: values.capacity },`
- L67: `await queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() });`
- L76: `await queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });`
- L82: `await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`

## frontend/src/features/groups/GroupEditDialogs.tsx
345 lines; 10773 bytes; SHA256 `719173452b9f46cc37b86ac0019a70f1379144527de6e007480ccc15a36dfb5b`.

- L55: `function useGroupMutationOutcome(group: GroupDto, onOpenChange: (open: boolean) => void) {`
- L56: `const queryClient = useQueryClient();`
- L60: `queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(group.id) }),`
- L61: `queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),`
- L81: `function FieldMessage({ id, error, t }: { id: string; error: FieldError | undefined; t: (key: string) => string }) {`
- L93: `function GroupDialogFrame({`
- L112: `const { t } = useTranslation("groups");`
- L157: `export function RenameGroupDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- L158: `const { t } = useTranslation("groups");`
- L159: `const mutation = useRenameGroup();`
- L160: `const { succeed, fail } = useGroupMutationOutcome(group, onOpenChange);`
- L173: `await mutation.mutateAsync({ id: group.id, data: { name: values.name, version: group.version } });`
- L211: `export function ChangeGroupCapacityDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- L212: `const { t } = useTranslation("groups");`
- L213: `const mutation = useChangeGroupCapacity();`
- L214: `const { succeed, fail } = useGroupMutationOutcome(group, onOpenChange);`
- L227: `await mutation.mutateAsync({ id: group.id, data: { capacity: values.capacity, version: group.version } });`
- L278: `const TeacherRefusals = new Set(["group.teacher_archived", "teacher.not_found"]);`
- L280: `export function AssignGroupTeacherDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- L281: `const { t } = useTranslation("groups");`
- L282: `const mutation = useAssignGroupTeacher();`
- L283: `const { succeed, fail, queryClient } = useGroupMutationOutcome(group, onOpenChange);`
- L296: `await mutation.mutateAsync({ id: group.id, data: { teacherId: values.teacherId, version: group.version } });`
- L303: `await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`

## frontend/src/features/groups/groupSchemas.ts
37 lines; 1630 bytes; SHA256 `05d2a04b482b48996f081533e8a98e6814c7aad6137396aa7f016565109e6eb0`.

- L8: `export const groupNameSchema = z.string().trim().min(1, "validation.nameRequired").max(80, "validation.nameTooLong");`
- L10: `export const groupCapacitySchema = z`
- L16: `export const createGroupFormSchema = z.object({`
- L23: `export type CreateGroupFormValues = z.infer<typeof createGroupFormSchema>;`
- L25: `export const renameGroupFormSchema = z.object({ name: groupNameSchema });`
- L27: `export type RenameGroupFormValues = z.infer<typeof renameGroupFormSchema>;`
- L29: `export const groupCapacityFormSchema = z.object({ capacity: groupCapacitySchema });`
- L31: `export type GroupCapacityFormValues = z.infer<typeof groupCapacityFormSchema>;`
- L33: `export const assignGroupTeacherFormSchema = z.object({`
- L37: `export type AssignGroupTeacherFormValues = z.infer<typeof assignGroupTeacherFormSchema>;`

## frontend/src/features/groups/GroupsList.tsx
186 lines; 7291 bytes; SHA256 `3bd9c525bdd6159017a01d6f5bef54f0d8b3a0fcb393f7e1bfd1f6b998a2e1b0`.

- L32: `export function GroupsList({`
- L45: `const { t } = useTranslation("groups");`
- L46: `const { t: tCommon } = useTranslation("common");`
- L47: `const canManage = useCan(Permissions.GroupsManage);`
- L48: `const { data, error, isPending, refetch } = useListGroups({`
- L110: `<Can permission={Permissions.GroupsManage}>`

## frontend/src/features/groups/showGroupError.ts
26 lines; 1026 bytes; SHA256 `e21b4a99d55290fc1f733439f13de5887e9fb5bdcba54fc2f4990e3995f4b8b1`.

- L11: `const GroupRuleMessages: Record<string, string> = {`
- L18: `export function showGroupError(error: ApiError): void {`

## frontend/src/features/language/LanguageSwitcher.test.tsx
40 lines; 1333 bytes; SHA256 `a4fee68319bee2b8a716a2728e7bd29a5995d01141e440031b4997fbceaa41ae`.

- L6: `describe("LanguageSwitcher", () => {`
- L12: `it("switching to Arabic sets dir=rtl on the document and persists the choice", async () => {`
- L26: `it("switching back to English sets dir=ltr", async () => {`

## frontend/src/features/language/LanguageSwitcher.tsx
18 lines; 596 bytes; SHA256 `45edd734046854330555e14a0b84947b62e21f6f112d0c77fda17d8696fbc2b4`.

- L5: `export function LanguageSwitcher() {`
- L6: `const { i18n, t } = useTranslation("common");`

## frontend/src/features/parents/ParentEditDialog.tsx
176 lines; 6119 bytes; SHA256 `e3a024c27acbe9bf4e09518bd5c4e282a43a64d79db2b16ad8e9820e76242cf8`.

- L25: `const KnownFields = ["fullName", "phone"] as const;`
- L36: `export function ParentEditDialog({`
- L45: `const { t } = useTranslation("parents");`
- L46: `const queryClient = useQueryClient();`
- L47: `const mutation = useUpdateParent();`
- L62: `queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parent.id) }),`
- L63: `queryClient.invalidateQueries({`
- L73: `data: { fullName: values.fullName, phone: values.phone, version: parent.version },`

## frontend/src/features/parents/updateParentSchema.ts
9 lines; 490 bytes; SHA256 `ecd177bfcd9bf31b423a16b7de907a4bc5b6864a9f6e89537d985ffb52664351`.

- L4: `export const updateParentFormSchema = z.object({`
- L9: `export type UpdateParentFormValues = z.infer<typeof updateParentFormSchema>;`

## frontend/src/features/schedule/AddScheduleRuleDialog.test.tsx
169 lines; 6783 bytes; SHA256 `3bd07ff753652469a90f4b9ae737a97797a48118a2a65200d4455db85874c940`.

- L10: `const rulesUrl = "/api/groups/g-1/schedule-rules";`
- L13: `async function choose(dialog: HTMLElement, label: string, option: string) {`
- L20: `async function openDialog() {`
- L27: `async function fillSaturdayEvening(dialog: HTMLElement) {`
- L36: `describe("AddScheduleRuleDialog", () => {`
- L40: `vi.useFakeTimers({ toFake: ["Date"] });`
- L54: `vi.useRealTimers();`
- L58: `it("the start date defaults to today in the centre's zone, not the browser's", async () => {`
- L68: `it("sends exactly the five contract fields as local values, then toasts the count and refreshes the rules", async () => {`
- L95: `it("the rule request never carries a teacher, a centre, an offset or an instant", async () => {`
- L130: `it("a 400 schedule.start_time_invalid lands under the start time", async () => {`
- L154: `it("without a day nothing is sent and the day select says why", async () => {`

## frontend/src/features/schedule/AddScheduleRuleDialog.tsx
395 lines; 14455 bytes; SHA256 `590fe93424dd7e79daa8f4f7d3cb245df2fad2e968199378a2db3c72f3c4e085`.

- L38: `const FieldForServerField: Partial<Record<string, FormField>> = {`
- L47: `const MaxEffectiveFromDaysAhead = 366;`
- L56: `export function AddScheduleRuleDialog({`
- L69: `const { t } = useTranslation("schedule");`
- L102: `function AddScheduleRuleForm({`
- L113: `const { t } = useTranslation("schedule");`
- L114: `const durationLabel = useDurationLabel();`
- L115: `const { addRule } = useScheduleMutations();`
- L384: `function FieldMessage({ id, error }: { id: string; error: FieldError | undefined }) {`
- L385: `const { t } = useTranslation("schedule");`

## frontend/src/features/schedule/CancelSessionDialog.tsx
86 lines; 2796 bytes; SHA256 `302cb038964cf9b1a6c36fa8223e4b5bd2bf93531825a08a601e0ebdf487a09d`.

- L24: `export function CancelSessionDialog({`
- L33: `const { t, i18n } = useTranslation("schedule");`
- L34: `const { cancelSession, isCancelling } = useScheduleMutations();`
- L41: `const outcome = await cancelSession({ sessionId: session.id, version: session.version });`

## frontend/src/features/schedule/centreTime.test.ts
65 lines; 3368 bytes; SHA256 `1ab016e5a0eda5069321e545a45768c4632a27b69b498790443e01f641c8e23d`.

- L4: `const Cairo = "Africa/Cairo";`
- L7: `function hourMinute(instant: string, locale: string): { hour: string; minute: string; dayPeriod: string } {`
- L15: `describe("centreTime", () => {`
- L32: `it("formats in the zone it is given, not the runner's: the same instant reads 6 PM in Cairo and 4 PM in London", () => {`
- L38: `it("in Arabic shows the same hour and minute, judged by value rather than by digit shapes", () => {`
- L49: `it("formats a session's range in the centre's zone", () => {`
- L53: `it("todayIn gives the centre's date, which can differ from UTC's around midnight", () => {`
- L60: `it("formats a rule's wall-clock time without any zone, and computes its end", () => {`

## frontend/src/features/schedule/centreTime.ts
79 lines; 3761 bytes; SHA256 `808067b2d42f819a873e39480d38910fa25cc669f6cfe2ea3e6cbec34279b7dd`.

- L9: `const TimeOptions: Intl.DateTimeFormatOptions = { hour: "numeric", minute: "2-digit" };`
- L11: `const LocalTimePattern = /^([01]\d|2[0-3]):([0-5]\d)$/;`
- L14: `export function formatTime(instant: string, timeZoneId: string, locale: string): string {`
- L19: `export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {`
- L24: `export function todayIn(timeZoneId: string, now: Date = new Date()): string {`
- L36: `export function isLocalTime(value: string): boolean {`
- L37: `return LocalTimePattern.test(value);`
- L41: `export function minutesOf(localTime: string): number {`
- L51: `export function addMinutes(localTime: string, minutes: number): string {`
- L60: `export function formatLocalTime(localTime: string, locale: string): string {`
- L67: `export function formatLocalTimeRange(start: string, end: string, locale: string): string {`

## frontend/src/features/schedule/EndScheduleRuleDialog.tsx
157 lines; 4957 bytes; SHA256 `f10c185f5dbfd9749ef8efcb88b9c197f5d051398e3dddf3ac8f4198ba9b8f3b`.

- L27: `export function EndScheduleRuleDialog({`
- L38: `const { t, i18n } = useTranslation("schedule");`
- L66: `function EndScheduleRuleForm({`
- L77: `const { t, i18n } = useTranslation("schedule");`
- L78: `const { endRule, isEnding } = useScheduleMutations();`
- L80: `const [lastDate, setLastDate] = useState(rule.effectiveFrom > today ? rule.effectiveFrom : today);`
- L96: `const outcome = await endRule({ ruleId: rule.id, groupId, lastDate, version: rule.version });`

## frontend/src/features/schedule/ExtendScheduleDialog.test.tsx
92 lines; 3852 bytes; SHA256 `c2ae84c6464a402d801352c218c59edcf9f9ace16e879a70a52a4746ec632272`.

- L10: `async function openDialog() {`
- L16: `describe("ExtendScheduleDialog", () => {`
- L18: `vi.useFakeTimers({ toFake: ["Date"] });`
- L29: `vi.useRealTimers();`
- L33: `it("is bounded to today through 92 days ahead in the centre's zone, and sends the chosen date", async () => {`
- L53: `it("reports a repeat that creates nothing as already up to date", async () => {`
- L62: `it("a date beyond 92 days is refused under the field before any request", async () => {`
- L79: `it("a double booking found while extending names its kind and advises a review, keeping the dialog open", async () => {`

## frontend/src/features/schedule/ExtendScheduleDialog.tsx
164 lines; 5314 bytes; SHA256 `87526ecdd5e42a6ab41a7bf64150ebace79d77e911415de9332876a1c93dc8a9`.

- L22: `export const MaxExtendDaysAhead = 92;`
- L25: `const SuggestedDaysAhead = 56;`
- L33: `export function ExtendScheduleDialog({`
- L42: `const { t } = useTranslation("schedule");`
- L71: `function ExtendScheduleForm({ today, onDone }: { today: string; onDone: () => void }) {`
- L72: `const { t, i18n } = useTranslation("schedule");`
- L73: `const { extend, isExtending } = useScheduleMutations();`
- L75: `const [through, setThrough] = useState(addDays(today, SuggestedDaysAhead));`

## frontend/src/features/schedule/GroupScheduleTab.test.tsx
120 lines; 5623 bytes; SHA256 `f470976bee91cff98d7d1d5e1315d33b510d663233f49ebfa58b829167ee7799`.

- L10: `const rulesUrl = "/api/groups/g-1/schedule-rules";`
- L12: `describe("GroupScheduleTab", () => {`
- L16: `vi.useFakeTimers({ toFake: ["Date"] });`
- L32: `vi.useRealTimers();`
- L36: `it("shows a loading state, then the rules Saturday first, times left to right and an ended rule labelled", async () => {`
- L56: `it("an empty list says so and offers to add a weekly session", async () => {`
- L65: `it("a failed load shows the error with Retry, and Retry loads the rules", async () => {`
- L79: `it("with schedule.view only (the group's teacher) the rules are read-only", async () => {`
- L89: `it("end asks for a last date not before the rule's start, then sends exactly { lastDate, version }", async () => {`
- L115: `expect(body).toStrictEqual({ lastDate: "2027-04-27", version: 5 });`

## frontend/src/features/schedule/GroupScheduleTab.tsx
178 lines; 6562 bytes; SHA256 `5974b9b946c36aeb474575d668ca3378df1cec3349a64105ac608d825e73d538`.

- L28: `export function GroupScheduleTab({ group }: { group: GroupDto }) {`
- L29: `const { t } = useTranslation("schedule");`
- L30: `const { t: tCommon } = useTranslation("common");`
- L31: `const timeZoneId = useCentreTimeZone();`
- L32: `const canManage = useCan(Permissions.ScheduleManage);`
- L33: `const { data, error, isPending, refetch } = useListGroupScheduleRules(group.id);`
- L34: `const [addOpen, setAddOpen] = useState(false);`
- L36: `const timeRange = useRuleTimeRange();`
- L87: `{data.items.length > 0 && <Can permission={Permissions.ScheduleManage}>{addButton}</Can>}`
- L95: `action={<Can permission={Permissions.ScheduleManage}>{addButton}</Can>}`
- L124: `<Can permission={Permissions.ScheduleManage}>`
- L144: `function AddRuleButton({ archived, onClick }: { archived: boolean; onClick: () => void }) {`
- L145: `const { t } = useTranslation("schedule");`

## frontend/src/features/schedule/scheduleMessages.ts
32 lines; 1494 bytes; SHA256 `d78c460ff4b7d7e97eb2642bddd5ff979b88935a44c86b4e25ef60fa2fdda640`.

- L10: `const OutcomeMessages: Record<string, string> = {`
- L24: `export function scheduleMessageFor(error: ApiError): string {`
- L30: `export function isDoubleBooking(error: ApiError): boolean {`

## frontend/src/features/schedule/scheduleRules.ts
35 lines; 1644 bytes; SHA256 `92f6e551a7845ab652fcab3d52fde0502d4e892e26b12d7082450ddec63e101f`.

- L6: `export type RuleState = "active" | "upcoming" | "ending" | "ended";`
- L12: `export function ruleState(rule: ScheduleRuleDto, today: string | null): RuleState {`
- L25: `export function useRuleTimeRange() {`
- L26: `const { i18n } = useTranslation();`
- L32: `export function sortRules(rules: ScheduleRuleDto[]): ScheduleRuleDto[] {`

## frontend/src/features/schedule/ScheduleRulesTable.tsx
85 lines; 3688 bytes; SHA256 `1f647b36de276158137d6a7cbf5514fe4d7d8aab895f187a1528cc3df7561d32`.

- L20: `export function ScheduleRulesTable({`
- L29: `const { t, i18n } = useTranslation("schedule");`
- L30: `const timeRange = useRuleTimeRange();`

## frontend/src/features/schedule/scheduleSchemas.ts
63 lines; 2946 bytes; SHA256 `25bf6f698f68879fc33f97ba44cb8e8bef75f1fb9ab2e5728e6d8a3e233775fe`.

- L11: `export const MinDurationMinutes = 30;`
- L12: `export const MaxDurationMinutes = 240;`
- L13: `export const RoomMaxLength = 40;`
- L16: `export const DurationOptions = Array.from(`
- L21: `export const Hours12 = Array.from({ length: 12 }, (_, index) => index + 1);`
- L23: `export const MinuteOptions = Array.from({ length: 12 }, (_, index) => index * 5);`
- L25: `export const scheduleRuleSchema = z.object({`
- L45: `export type ScheduleRuleFormValues = z.infer<typeof scheduleRuleSchema>;`
- L48: `export function toLocalTime(hour: number, minute: number, period: "am" | "pm"): string {`
- L54: `export function toAddScheduleRuleRequest(values: ScheduleRuleFormValues) {`

## frontend/src/features/schedule/scheduleTestData.ts
92 lines; 2876 bytes; SHA256 `fc13aa95cafac55199b70365820b0577168504b8c93f3a18732a10eb5d62af86`.

- L5: `export const Now = new Date("2027-04-20T08:00:00Z");`
- L7: `export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- L21: `export const secretaryMe = me("secretary", [`
- L34: `export const teacherMe = me("teacher", ["enrollments.view", "groups.view", "schedule.view", "subjects.view"]);`
- L36: `export function group(overrides: Record<string, unknown> = {}) {`
- L47: `version: 3,`
- L52: `export function rule(overrides: Record<string, unknown> = {}) {`
- L61: `version: 5,`
- L66: `export function session(overrides: Record<string, unknown> = {}) {`
- L80: `version: 9,`
- L85: `export function timetable(weekStart: string, sessions: ReturnType<typeof session>[] = [], timeZoneId = "Africa/Cairo") {`
- L89: `export const teachers = [`
- L90: `{ id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },`
- L91: `{ id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },`

## frontend/src/features/schedule/SessionCard.tsx
100 lines; 3910 bytes; SHA256 `45a5726548153e2d0a5e9caaf6c3318ef5a5731cde267c820d516605c48ecbd1`.

- L17: `export function SessionCard({`
- L26: `const { t, i18n } = useTranslation("schedule");`

## frontend/src/features/schedule/TimetableGrid.tsx
84 lines; 3508 bytes; SHA256 `3388350b55a5154ffea1e8b0bf56ff138ee53b8b9bbfe433ad7d9b3d9ada5b35`.

- L14: `export function TimetableGrid({`
- L27: `const { t, i18n } = useTranslation("schedule");`

## frontend/src/features/schedule/TimetablePage.test.tsx
163 lines; 7689 bytes; SHA256 `257c4098f876df0054bf33ba6f6171657b3dfcaf9e8853945471ed428bc12d33`.

- L9: `describe("TimetablePage", () => {`
- L16: `vi.useFakeTimers({ toFake: ["Date"] });`
- L38: `vi.useRealTimers();`
- L42: `it("without a week in the URL opens the centre's current week, from Saturday", async () => {`
- L51: `it("places sessions by the server's localDate and formats times in the response's zone", async () => {`
- L69: `it("uses the zone the response names, whatever the browser's or the membership's zone is", async () => {`
- L79: `it("keeps a cancelled session visible, struck through and labelled", async () => {`
- L91: `it("week navigation goes through the URL and fetches that week", async () => {`
- L109: `it("cancel asks for confirmation naming group, date and time, then sends only the version", async () => {`
- L126: `expect(body).toStrictEqual({ version: 9 });`
- L129: `it("without schedule.manage or teachers.view there is no manage control, no teacher filter and no teachers request", async () => {`
- L142: `it("with schedule.manage an empty week hints at adding weekly sessions or extending", async () => {`
- L150: `it("in Arabic the page runs right to left, Saturday first, while times stay left to right", async () => {`

## frontend/src/features/schedule/TimetablePage.tsx
258 lines; 9106 bytes; SHA256 `2e904957e2787e27bd0c1c3238ea1ab3cdb3e45651f10c4738a4c6913b95d2b9`.

- L28: `export interface TimetableSearchPatch {`
- L41: `export function TimetablePage({`
- L52: `const { t } = useTranslation("schedule");`
- L53: `const zone = useCentreTimeZone();`
- L57: `const [extendOpen, setExtendOpen] = useState(false);`
- L65: `<Can permission={Permissions.ScheduleManage}>`
- L82: `<Can permission={Permissions.GroupsView}>`
- L93: `<Can permission={Permissions.TeachersView}>`
- L122: `<Can permission={Permissions.ScheduleManage}>`
- L130: `function TimetableWeek({`
- L141: `const { t } = useTranslation("schedule");`
- L142: `const { t: tCommon } = useTranslation("common");`
- L143: `const canManage = useCan(Permissions.ScheduleManage);`
- L144: `const { data, error, isPending, refetch } = useGetTimetable({ weekStart, groupId, teacherId });`
- L211: `<Can permission={Permissions.ScheduleManage}>`
- L224: `const AllGroupsValue = "__all";`
- L227: `function GroupFilterSelect({ value, onChange }: { value: string | null; onChange: (value: string | null) => void }) {`
- L228: `const { t } = useTranslation("schedule");`
- L229: `const { data, isPending } = useListGroups();`

## frontend/src/features/schedule/useDurationLabel.ts
11 lines; 415 bytes; SHA256 `66e9d4bc8e21e080e45aad0f3d1fd3132db21705315c12592b5b7a1b66826858`.

- L4: `export function useDurationLabel() {`
- L5: `const { t } = useTranslation("schedule");`

## frontend/src/features/schedule/useScheduleMutations.ts
66 lines; 2904 bytes; SHA256 `d72142604d3f5ae0a3fdfa934b5c0bd716dd1123145733be5ad8e3f52d439a42`.

- L15: `export type ScheduleOutcome<T = undefined> = { ok: true; value: T } | { ok: false; error: ApiError };`
- L24: `export function useScheduleMutations() {`
- L25: `const queryClient = useQueryClient();`
- L26: `const addMutation = useAddScheduleRule();`
- L27: `const endMutation = useEndScheduleRule();`
- L28: `const extendMutation = useExtendSchedule();`
- L29: `const cancelMutation = useCancelSession();`
- L33: `...(groupId === undefined ? [] : [queryClient.invalidateQueries({ queryKey: getListGroupScheduleRulesQueryKey(groupId) })]),`
- L34: `queryClient.invalidateQueries({ queryKey: getGetTimetableQueryKey() }),`
- L50: `addRule: (groupId: string, data: AddScheduleRuleRequest) =>`
- L53: `endRule: ({ ruleId, groupId, lastDate, version }: { ruleId: string; groupId: string; lastDate: string; version: number | string }) =>`
- L54: `run(() => endMutation.mutateAsync({ id: ruleId, data: { lastDate, version } }), groupId),`
- L56: `extend: (through: string) => run(async () => Number((await extendMutation.mutateAsync({ data: { through } })).sessionsCreated)),`
- L58: `cancelSession: ({ sessionId, version }: { sessionId: string; version: number | string }) =>`
- L59: `run(() => cancelMutation.mutateAsync({ id: sessionId, data: { version } })),`

## frontend/src/features/schedule/week.test.ts
40 lines; 1489 bytes; SHA256 `be889a3560e449436d617c7f447a529e8e8ba36ed957a439373a899c06d5d4a6`.

- L4: `describe("week", () => {`
- L5: `it("the start of the week for a Wednesday is the previous Saturday", () => {`
- L9: `it("a Saturday is its own week start, and a Friday belongs to the Saturday six days before", () => {`
- L14: `it("adding 7 days across Cairo's 30 April clock change keeps the weekday and the date arithmetic exact", () => {`
- L21: `it("lists the seven days of a week from Saturday, across a month end", () => {`
- L34: `it("accepts real calendar dates only", () => {`

## frontend/src/features/schedule/week.ts
94 lines; 4148 bytes; SHA256 `fc38aabcb1370cb4033dd7659c8b38fb7e9a86eced499b4eb2d63fa75956a1ea`.

- L7: `const IsoDatePattern = /^(\d{4})-(\d{2})-(\d{2})$/;`
- L8: `const MillisecondsPerDay = 86_400_000;`
- L11: `export const WeekStartDay = 6;`
- L14: `export const WeekdaysFromSaturday = ["saturday", "sunday", "monday", "tuesday", "wednesday", "thursday", "friday"] as const;`
- L16: `export type Weekday = (typeof WeekdaysFromSaturday)[number];`
- L18: `const WeekdaysFromSunday: readonly Weekday[] = ["sunday", "monday", "tuesday", "wednesday", "thursday", "friday", "saturday"];`
- L21: `export function isIsoDate(value: unknown): value is string {`
- L36: `function toDayNumber(isoDate: string): number {`
- L45: `function fromDayNumber(dayNumber: number): string {`
- L53: `export function addDays(isoDate: string, days: number): string {`
- L58: `export function dayOfWeekNumber(isoDate: string): number {`
- L63: `export function weekdayOf(isoDate: string): Weekday {`
- L68: `export function startOfWeek(isoDate: string): string {`
- L73: `export function weekDates(weekStart: string): string[] {`
- L78: `export function daysBetween(from: string, to: string): number {`
- L86: `export function formatCalendarDate(isoDate: string, locale: string, options: Intl.DateTimeFormatOptions = { dateStyle: "medium" }): string {`
- L91: `export function formatCalendarRange(from: string, to: string, locale: string): string {`

## frontend/src/features/schedule/WeekNavigator.tsx
66 lines; 2111 bytes; SHA256 `f7d48a6cafd1a55f9b374786ae6dc9074685c4736c7ba1f21b127ee5d9eaa2a4`.

- L10: `export function WeekNavigator({`
- L19: `const { t, i18n } = useTranslation("schedule");`

## frontend/src/features/session/Can.tsx
8 lines; 409 bytes; SHA256 `6971034cb3dee6641af9be72a6209b343163fe7ed17fd8cf683d37ad877da695`.

- L6: `export function Can({ permission, children }: { permission: Permission; children: ReactNode }) {`
- L7: `return useCan(permission) ? children : null;`

## frontend/src/features/session/meQueryOptions.ts
24 lines; 821 bytes; SHA256 `7d5d16a297dd12d81700b5bd3099b5da8decdfec11bf2018cd2b5e950f3995cc`.

- L10: `export const meQueryOptions = queryOptions({`
- L11: `queryKey: getGetMeQueryKey(),`

## frontend/src/features/session/permissions.ts
21 lines; 785 bytes; SHA256 `c8f7a6e0675f484ca62f38eceebbc845ab7a66b4263eac755e1cc0c58afae43b`.

- L2: `export const Permissions = {`
- L21: `export type Permission = (typeof Permissions)[keyof typeof Permissions];`

## frontend/src/features/session/refetchMeOnPermissionDenied.ts
10 lines; 555 bytes; SHA256 `9f159eaaeeaf835e0adac2585f411b8dfdfae499f7677dd7f167b88f30950d06`.

- L6: `export function refetchMeOnPermissionDenied(error: ApiError, queryClient: QueryClient): void {`
- L8: `void queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() });`

## frontend/src/features/session/RouteGuard.tsx
31 lines; 1464 bytes; SHA256 `991e2ade3128567a339fb705e92e6d0c8ce17bd11e0cdffbce474da6e91700c3`.

- L15: `export function RouteGuard({ permission, children }: { permission: Permission; children: ReactNode }) {`
- L16: `const { t } = useTranslation("shell");`
- L17: `const allowed = useCan(permission);`

## frontend/src/features/session/useCan.ts
8 lines; 400 bytes; SHA256 `e9fcbafab1a5de1ba29fb016757d3b9da58896aa0500de8c173a901457647c90`.

- L5: `export function useCan(permission: Permission): boolean {`
- L6: `const { me } = useSession();`

## frontend/src/features/session/useCentreTimeZone.ts
11 lines; 521 bytes; SHA256 `bc8d4b7e99371cd1087b34fd4855e0ee4ae3275ba8f5e236475342f32a4cadd5`.

- L8: `export function useCentreTimeZone(): string | null {`
- L9: `const { me } = useSession();`

## frontend/src/features/session/useSession.test.ts
85 lines; 2520 bytes; SHA256 `49960541a5d29aa32fe4620fbcb14639221453367f835be9236aaba78bc9e7a3`.

- L8: `describe("useSession", () => {`
- L9: `it("resolves to a signed-out session (not an error) on a 401 from /api/me", async () => {`
- L13: `const { result } = renderHookWithQueryClient(() => useSession());`
- L23: `it("resolves to the signed-in user's profile when /api/me succeeds", async () => {`
- L55: `const { result } = renderHookWithQueryClient(() => useSession());`
- L65: `it("reports a real error for a non-401 failure, e.g. a 500", async () => {`
- L77: `const { result } = renderHookWithQueryClient(() => useSession());`

## frontend/src/features/session/useSession.ts
14 lines; 388 bytes; SHA256 `00da071826054ab37ff02fe526c74571a306142064db74d16233a8c689cef87e`.

- L5: `export function useSession() {`
- L6: `const { data, isPending, isError, error } = useQuery(meQueryOptions);`

## frontend/src/features/session/useSignOut.ts
18 lines; 675 bytes; SHA256 `700a1c2acad955e09191add81a3112d3ce4acc2ffd87487cb9a65bc2f9deed95`.

- L7: `export function useSignOut(): () => Promise<void> {`
- L8: `const logoutMutation = useLogout();`
- L9: `const queryClient = useQueryClient();`
- L10: `const navigate = useNavigate();`

## frontend/src/features/settings/settingsSchema.ts
10 lines; 556 bytes; SHA256 `8511c2a1043a74e9ba812c16da439071e828d21c5b6f17f4f064229bff5f35dd`.

- L5: `export const centreSettingsFormSchema = z.object({`
- L10: `export type CentreSettingsFormValues = z.infer<typeof centreSettingsFormSchema>;`

## frontend/src/features/shell/AppShell.tsx
231 lines; 9718 bytes; SHA256 `168240628798c3e1c5b7cf01ec640143b4fca521c6534a103ac7d919478a7459`.

- L31: `export function AppShell({ me, children }: { me: MeDto; children: ReactNode }) {`
- L32: `const { t: tShell } = useTranslation("shell");`
- L33: `const { t: tAuth } = useTranslation("auth");`
- L34: `const [mobileNavOpen, setMobileNavOpen] = useState(false);`
- L37: `const isWide = useMatches({ select: (matches) => matches.some((match) => match.staticData.layout === "wide") });`
- L101: `function SidebarBrand({ onNavigate }: { onNavigate?: () => void }) {`
- L102: `const { t } = useTranslation("shell");`
- L116: `function SidebarNav({ onNavigate }: { onNavigate?: () => void }) {`
- L117: `const { t } = useTranslation("shell");`
- L128: `<Can permission={Permissions.SubjectsView}>`
- L134: `<Can permission={Permissions.StudentsView}>`
- L140: `<Can permission={Permissions.TeachersView}>`
- L146: `<Can permission={Permissions.GroupsView}>`
- L152: `<Can permission={Permissions.ScheduleView}>`
- L158: `<Can permission={Permissions.StaffView}>`
- L164: `<Can permission={Permissions.CentreSettingsManage}>`
- L170: `<Can permission={Permissions.AuditView}>`
- L180: `function UserMenu({ me }: { me: MeDto }) {`
- L181: `const { t } = useTranslation("shell");`
- L182: `const signOut = useSignOut();`

## frontend/src/features/shell/routeStaticData.ts
11 lines; 344 bytes; SHA256 `5fdac2a804b375730b6399baae55d101a2595a0172987dd915c0651821517624`.

- L11: `export {};`

## frontend/src/features/staff/ChangeRoleDialog.tsx
128 lines; 4545 bytes; SHA256 `97692c5c56d34ed3603ef991cfe7430d9d0b7c0454ff1e7b246584f63b9b2edc`.

- L27: `export function ChangeRoleDialog({`
- L34: `const { t } = useTranslation("staff");`
- L35: `const { t: tAuth } = useTranslation("auth");`
- L36: `const queryClient = useQueryClient();`
- L37: `const mutation = useChangeStaffRole();`
- L50: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });`
- L58: `await mutation.mutateAsync({ membershipId: member.membershipId, data: { role, version: member.version } });`

## frontend/src/features/staff/StaffFormDialog.tsx
284 lines; 10398 bytes; SHA256 `c2b0b1c90d191f6246647beba239579ea1a2d21af4522391d136ea410f81c770`.

- L26: `const KnownFields = ["email", "displayName"] as const;`
- L34: `export function StaffFormDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`
- L35: `const { t } = useTranslation("staff");`
- L36: `const queryClient = useQueryClient();`
- L37: `const createMutation = useCreateStaff();`
- L40: `const [copied, setCopied] = useState(false);`
- L65: `const result = await createMutation.mutateAsync({ data: values });`
- L66: `await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });`
- L225: `function RoleField({`
- L232: `const { t } = useTranslation("auth");`
- L254: `function LocaleField({`
- L261: `const { t } = useTranslation("common");`

## frontend/src/features/staff/StaffList.tsx
251 lines; 9013 bytes; SHA256 `7345679e6374786dc2cc615ce13315d44cd953cabc7f79e0363f330bc80d217b`.

- L47: `export function StaffList({ onChangeRole }: { onChangeRole: (member: StaffMemberDto) => void }) {`
- L48: `const { t } = useTranslation("staff");`
- L49: `const { t: tAuth } = useTranslation("auth");`
- L50: `const { t: tCommon } = useTranslation("common");`
- L51: `const queryClient = useQueryClient();`
- L52: `const { data, error, isPending, refetch } = useListStaff();`
- L53: `const reactivateMutation = useReactivateStaff();`
- L54: `const deactivateMutation = useDeactivateStaff();`
- L58: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });`
- L68: `data: { version: deactivateTarget.version },`
- L85: `await reactivateMutation.mutateAsync({ membershipId: member.membershipId, data: { version: member.version } });`

## frontend/src/features/staff/staffSchema.ts
15 lines; 749 bytes; SHA256 `6a03028aee9d449cd8a1b470ba237bff6cd4b31f6b983975405b3d24a3b97b20`.

- L8: `export const createStaffFormSchema = z.object({`
- L15: `export type CreateStaffFormValues = z.infer<typeof createStaffFormSchema>;`

## frontend/src/features/status/api.ts
23 lines; 709 bytes; SHA256 `07d41330d69b916e025c9e744de504ce403b6b7dc33438b1907b6e8f37384abd`.

- L1: `export type ReadinessStatus = {`
- L11: `export async function fetchReadiness(): Promise<ReadinessStatus> {`

## frontend/src/features/status/DemoBanner.test.tsx
37 lines; 1274 bytes; SHA256 `e7d93c46e77a9c93ac6407cdd19f392cb75026cd7ad0ff7321e476979a47261d`.

- L8: `describe("DemoBanner", () => {`
- L9: `it("renders nothing while loading and nothing once demoMode resolves to false", async () => {`
- L19: `it("renders the message when demoMode is true", async () => {`

## frontend/src/features/status/DemoBanner.tsx
21 lines; 718 bytes; SHA256 `bc95dea3ae2ee903b65d7ed49fe764a45ff8f329f1e27dee2824c81857f5f766`.

- L8: `export function DemoBanner() {`
- L9: `const { t } = useTranslation("common");`
- L10: `const { data, isPending } = useGetSystemInfo();`

## frontend/src/features/status/StatusCard.test.tsx
47 lines; 1779 bytes; SHA256 `7473ecdacdd8f95e52b6d7aa594f4397c632e3262b66ae2fbbc1697af6a1b522`.

- L8: `const readinessUrl = "/health/ready";`
- L9: `const healthy = () => new HttpResponse("Healthy", { status: 200 });`
- L10: `const unavailable = () => new HttpResponse("Unhealthy", { status: 503 });`
- L12: `describe("StatusCard", () => {`
- L13: `it("shows the healthy state when the API returns 200", async () => {`
- L24: `it("shows the database-unavailable state with Retry when the API returns 503", async () => {`
- L37: `it("refetches and shows the healthy state after Retry", async () => {`

## frontend/src/features/status/StatusCard.tsx
68 lines; 2353 bytes; SHA256 `a6b3b60178a8ef22e2ac382cf143cd2c5c3d5463f6cc814fd1e7903dafb3d2f2`.

- L8: `export function StatusCard() {`
- L9: `const { t } = useTranslation("status");`
- L10: `const { t: tCommon } = useTranslation("common");`
- L11: `const readiness = useReadiness();`

## frontend/src/features/status/SystemInfoCard.test.tsx
46 lines; 1623 bytes; SHA256 `f374ae55744227c216b0dcff18bd5bea43bebe4e5aa462761b6276c74c871ad6`.

- L8: `describe("SystemInfoCard", () => {`
- L9: `it("shows version, latest migration and the up-to-date badge on success", async () => {`
- L21: `it("shows the translated error message and the correlation reference on a 500 Problem Details response", async () => {`

## frontend/src/features/status/SystemInfoCard.tsx
74 lines; 2633 bytes; SHA256 `9173cf534ca32cf6d01e7228491f45177f756002a1a4562898acc811a3b121bc`.

- L13: `export function SystemInfoCard() {`
- L14: `const { t } = useTranslation("status");`
- L15: `const { t: tCommon } = useTranslation("common");`
- L16: `const { data, error, isPending, refetch } = useGetSystemInfo();`

## frontend/src/features/status/useReadiness.ts
11 lines; 348 bytes; SHA256 `bd0af44e45d73fb3fa27e4cfdadde711f8ee7da468ededad1f2cf0944bf3da34`.

- L5: `export function useReadiness() {`
- L6: `return useQuery({`
- L7: `queryKey: ["health", "ready"],`

## frontend/src/features/students/AddParentDialog.tsx
256 lines; 9467 bytes; SHA256 `892e6e11aa4ef70c71e1d651a9db73f80f8fe70f08d0af06ebb05cd028a9da5f`.

- L36: `const KnownFields = ["fullName", "phone"] as const;`
- L44: `export function AddParentDialog({`
- L53: `const { t } = useTranslation("students");`
- L54: `const queryClient = useQueryClient();`
- L55: `const lookup = useParentLookup();`
- L56: `const createMutation = useCreateParent();`
- L57: `const linkMutation = useLinkStudentParent();`
- L59: `const [linkFailed, setLinkFailed] = useState(false);`
- L83: `queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),`
- L84: `queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),`
- L89: `await linkMutation.mutateAsync({ id: student.id, data: { parentId, relationship, version: student.version } });`
- L100: `await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });`
- L128: `const created = await createMutation.mutateAsync({ data: { fullName, phone: values.phone } });`

## frontend/src/features/students/addParentSchema.ts
19 lines; 787 bytes; SHA256 `26c5c251331c409bf51e1a969ce0b8ee5bd981adaef80ec3ff4e6fa05f3ef38c`.

- L8: `export const addParentFormSchema = z.object({`
- L19: `export type AddParentFormValues = z.infer<typeof addParentFormSchema>;`

## frontend/src/features/students/ParentsCard.tsx
213 lines; 7830 bytes; SHA256 `2f4cda7c73226411af6d4a796b05fafb2e60cb58b25fd4869cceedcc8aa9e9e9`.

- L39: `const MaxParents = 4;`
- L47: `export function ParentsCard({ student, canManage }: { student: StudentDetailDto; canManage: boolean }) {`
- L48: `const { t } = useTranslation("students");`
- L49: `const queryClient = useQueryClient();`
- L50: `const makePrimaryMutation = useMakeStudentParentPrimary();`
- L51: `const unlinkMutation = useUnlinkStudentParent();`
- L52: `const [addOpen, setAddOpen] = useState(false);`
- L61: `queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),`
- L62: `queryClient.invalidateQueries({ queryKey: getGetParentQueryKey(parentId) }),`
- L70: `data: { version: student.version },`
- L76: `await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });`
- L91: `data: { version: student.version },`
- L98: `await queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) });`

## frontend/src/features/students/RegisterStudentForm.tsx
277 lines; 10442 bytes; SHA256 `c6a36e117939c3bf372cd6d419136e9b3219013c6a2ccb90aa977bde5f88fc9e`.

- L27: `const KnownFields = ["fullName", "phone", "schoolYear", "parent.phone", "parent.fullName"] as const;`
- L36: `export function RegisterStudentForm() {`
- L37: `const { t } = useTranslation("students");`
- L38: `const queryClient = useQueryClient();`
- L39: `const navigate = useNavigate();`
- L40: `const registerMutation = useRegisterStudent();`
- L41: `const lookup = useParentLookup();`
- L81: `data: {`
- L90: `await queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() });`

## frontend/src/features/students/registerStudentSchema.ts
32 lines; 1578 bytes; SHA256 `11b96c4e7f72763a61424529066e0c70bd9a8d6e49a6cec6b5f2d65f31460c15`.

- L14: `const PhoneMaxRawLength = 30;`
- L16: `export const registerStudentFormSchema = z.object({`
- L32: `export type RegisterStudentFormValues = z.infer<typeof registerStudentFormSchema>;`

## frontend/src/features/students/StudentEditDialog.tsx
195 lines; 6642 bytes; SHA256 `9d7186702495cfc0e6d33764f3f568f6c5b110af8430a90aeeedfa58a11071c9`.

- L30: `const KnownFields = ["fullName", "phone", "schoolYear"] as const;`
- L37: `export function StudentEditDialog({`
- L46: `const { t } = useTranslation("students");`
- L47: `const queryClient = useQueryClient();`
- L48: `const mutation = useUpdateStudent();`
- L63: `queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),`
- L64: `queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() }),`
- L71: `data: {`
- L75: `version: student.version,`

## frontend/src/features/students/StudentsList.tsx
165 lines; 5633 bytes; SHA256 `d3570c1182320dd666085a11ff5da357405ac205bf74a17643dad74f2d624629`.

- L31: `const PageSize = 25;`
- L33: `export function StudentsList({ search, includeArchived }: { search: string; includeArchived: boolean }) {`
- L34: `const { t } = useTranslation("students");`
- L35: `const { t: tCommon } = useTranslation("common");`
- L36: `const canManage = useCan(Permissions.StudentsManage);`
- L44: `const { data, error, isPending, refetch, fetchNextPage, hasNextPage, isFetchingNextPage } = useInfiniteQuery({`
- L45: `queryKey: getListStudentsQueryKey(params),`
- L140: `function StudentRow({ student }: { student: StudentListItemDto }) {`
- L141: `const { t } = useTranslation("students");`

## frontend/src/features/students/updateStudentSchema.ts
14 lines; 708 bytes; SHA256 `82a9a54b95840e474a499caf75c776c651b7dd075f7bbfb84de60482c74430fb`.

- L8: `export const updateStudentFormSchema = z.object({`
- L14: `export type UpdateStudentFormValues = z.infer<typeof updateStudentFormSchema>;`

## frontend/src/features/subjects/SubjectFormDialog.tsx
162 lines; 5011 bytes; SHA256 `e3a851fc62f63ea1f949849679d588f54ed849fe04e97cd5c0bfa5d59c4196c8`.

- L30: `export type SubjectDialogState = { mode: "create" } | { mode: "rename"; subject: SubjectDto } | null;`
- L32: `const KnownFields = ["name"] as const;`
- L34: `export function SubjectFormDialog({`
- L41: `const { t } = useTranslation("subjects");`
- L42: `const queryClient = useQueryClient();`
- L43: `const createMutation = useCreateSubject();`
- L44: `const renameMutation = useRenameSubject();`
- L57: `useEffect(() => {`
- L63: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });`
- L72: `await createMutation.mutateAsync({ data: { name: values.name } });`
- L79: `data: { name: values.name, version: state.subject.version },`

## frontend/src/features/subjects/subjectSchema.test.ts
35 lines; 984 bytes; SHA256 `7228c0439b95c1f3c27111ab1f10ba8b288bdbe9e5816cad7ac627fb192e6a52`.

- L4: `describe("subjectNameSchema", () => {`
- L5: `it("rejects an empty name", () => {`
- L11: `it("accepts a name of exactly 80 characters", () => {`
- L17: `it("rejects a name of 81 characters", () => {`
- L23: `it("trims surrounding spaces", () => {`
- L30: `it("rejects a name that is only whitespace", () => {`

## frontend/src/features/subjects/subjectSchema.ts
17 lines; 503 bytes; SHA256 `21cef8a122e2213d89064cd238c8ae1be0c13b9ed09b6f97b5a53321a0f5af57`.

- L7: `export const subjectNameSchema = z`
- L13: `export const subjectFormSchema = z.object({`
- L17: `export type SubjectFormValues = z.infer<typeof subjectFormSchema>;`

## frontend/src/features/subjects/SubjectSelect.tsx
77 lines; 2951 bytes; SHA256 `933b384d101c0c2d8207748e34f4ecc2a8509395982e0e85d4dde828d832624a`.

- L6: `const AllValue = "__all";`
- L13: `export function SubjectSelect({`
- L32: `const { t } = useTranslation("groups");`
- L33: `const { data, isPending, isError } = useListSubjects({ includeArchived: false });`

## frontend/src/features/subjects/SubjectsList.tsx
247 lines; 8377 bytes; SHA256 `0d61252c467393d5b9ae5070fce20419fa2bdf08bf30945f2908e74f9825fa17`.

- L49: `export function SubjectsList({`
- L56: `const { t } = useTranslation("subjects");`
- L57: `const { t: tCommon } = useTranslation("common");`
- L58: `const queryClient = useQueryClient();`
- L59: `const canManage = useCan(Permissions.SubjectsManage);`
- L60: `const { data, error, isPending, refetch } = useListSubjects({ includeArchived });`
- L61: `const archiveMutation = useArchiveSubject();`
- L62: `const restoreMutation = useRestoreSubject();`
- L66: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListSubjectsQueryKey() });`
- L74: `await archiveMutation.mutateAsync({ id: archiveTarget.id, data: { version: archiveTarget.version } });`
- L90: `await restoreMutation.mutateAsync({ id: subject.id, data: { version: subject.version } });`

## frontend/src/features/teachers/eligibleStaff.ts
21 lines; 929 bytes; SHA256 `456112699176581c7b80f5e71e802da9a71b0c950e0e909b2b28c4bd2b32ad6c`.

- L9: `export function eligibleStaffForTeacher(`

## frontend/src/features/teachers/TeacherAccountDialog.tsx
238 lines; 9431 bytes; SHA256 `0666b709ff0386a5d7a1efcf82fd5f73f79ab13be744d66d5ef81ab3e7457bf2`.

- L35: `const AccountFieldCodes = new Set(["teacher.user_not_eligible", "teacher.account_taken"]);`
- L45: `export function TeacherAccountDialog({`
- L54: `const { t } = useTranslation("teachers");`
- L55: `const queryClient = useQueryClient();`
- L56: `const staffQuery = useListStaff();`
- L57: `const mutation = useSetTeacherAccount();`
- L67: `await mutation.mutateAsync({ id: teacher.id, data: { userId, version: teacher.version } });`
- L68: `await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`
- L77: `await queryClient.invalidateQueries({ queryKey: getListStaffQueryKey() });`
- L83: `await queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`

## frontend/src/features/teachers/TeacherFormDialog.tsx
195 lines; 6404 bytes; SHA256 `0a842358c1d84c19a970727c77ea5badf13dd32bb6dded642bc80b64239416e3`.

- L31: `export type TeacherDialogState = { mode: "create" } | { mode: "edit"; teacher: TeacherDto } | null;`
- L33: `const KnownFields = ["fullName", "phone"] as const;`
- L39: `export function TeacherFormDialog({`
- L46: `const { t } = useTranslation("teachers");`
- L47: `const queryClient = useQueryClient();`
- L48: `const createMutation = useCreateTeacher();`
- L49: `const updateMutation = useUpdateTeacher();`
- L69: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`
- L80: `await createMutation.mutateAsync({ data: { fullName: values.fullName, phone } });`
- L86: `data: { fullName: values.fullName, phone, version: state.teacher.version },`

## frontend/src/features/teachers/teacherSchema.ts
12 lines; 521 bytes; SHA256 `d3bb2ecefc347a5e92d3a8c1aa940d0c77028e303eb450fe1c8ac312d37598f5`.

- L7: `export const teacherFormSchema = z.object({`
- L12: `export type TeacherFormValues = z.infer<typeof teacherFormSchema>;`

## frontend/src/features/teachers/TeacherSelect.tsx
78 lines; 2973 bytes; SHA256 `11fb5094bcf8b84b2a2dddd4cceacbc55a596e3b01bde91365da87d4ffabba07`.

- L6: `const AllValue = "__all";`
- L14: `export function TeacherSelect({`
- L33: `const { t } = useTranslation("groups");`
- L34: `const { data, isPending, isError } = useListTeachers({ includeArchived: false });`

## frontend/src/features/teachers/TeachersList.tsx
344 lines; 12748 bytes; SHA256 `86d32153711a3223ef895597c1037668f374639b7ec55f77d8a2ad3c7f50494f`.

- L60: `export function TeachersList({`
- L69: `const { t } = useTranslation("teachers");`
- L70: `const { t: tCommon } = useTranslation("common");`
- L71: `const queryClient = useQueryClient();`
- L72: `const canManage = useCan(Permissions.TeachersManage);`
- L73: `const { data, error, isPending, refetch } = useListTeachers({ includeArchived });`
- L74: `const archiveMutation = useArchiveTeacher();`
- L75: `const restoreMutation = useRestoreTeacher();`
- L80: `const linkedUserIds = useMemo(`
- L85: `const invalidate = () => queryClient.invalidateQueries({ queryKey: getListTeachersQueryKey() });`
- L93: `await archiveMutation.mutateAsync({ id: archiveTarget.id, data: { version: archiveTarget.version } });`
- L109: `await restoreMutation.mutateAsync({ id: teacher.id, data: { version: teacher.version } });`
- L172: `<Can permission={Permissions.TeachersManage}>`

## frontend/src/i18n/index.ts
131 lines; 4610 bytes; SHA256 `c7817dbefeec0caff47d3fcf6d533c28683666e48ae713287d94883880973d48`.

- L35: `const StorageKey = "tcm.lang";`
- L38: `function isSupportedLanguage(value: unknown): value is SupportedLanguage {`
- L43: `function detectInitialLanguage(): SupportedLanguage {`
- L58: `function applyDocumentDirection(language: string): void {`
- L67: `const initialLanguage = detectInitialLanguage();`
- L131: `export default i18next;`

## frontend/src/i18n/locales/ar/audit.json
105 lines; 3046 bytes; SHA256 `303cb4b1b9186830b6db1bf95cf44571ce5d1a242f5f48029432aae577c6da36`.

Top-level keys: `page`, `filters`, `entityType`, `action`, `field`, `value`, `system`, `referenceLabel`, `copyReference`, `copied`, `loadMore`, `loading`, `error`, `emptyState`, `nullValue`

## frontend/src/i18n/locales/ar/auth.json
68 lines; 4242 bytes; SHA256 `4a617999e2fade01c2211b9b098f70aeadac3862978bba544e1b5fa30dc7a2da`.

Top-level keys: `login`, `panel`, `validation`, `changePassword`, `errors`, `selectCentre`, `roles`

## frontend/src/i18n/locales/ar/common.json
22 lines; 588 bytes; SHA256 `a74bf1b26087a2d2707794a2cc9831f99821577fbe88b999222ed503541094d6`.

Top-level keys: `retry`, `close`, `loadMore`, `comingSoon`, `search`, `status`, `language`, `demoBanner`

## frontend/src/i18n/locales/ar/enrollments.json
100 lines; 4590 bytes; SHA256 `ede934cd0724c4b0b071d009fc85fef186dd1959e893cfecff6616511248e1b4`.

Top-level keys: `seats`, `roster`, `actions`, `enrollDisabled`, `form`, `enrollDialog`, `picker`, `outcomes`, `toasts`, `withdrawDialog`, `transferDialog`, `studentCard`, `groupPicker`, `endReasons`, `groupsFilter`

## frontend/src/i18n/locales/ar/errors.json
153 lines; 10319 bytes; SHA256 `53808f6761186e88296bfacf1c33e3ac841023213f521a85f89fa275afb8470c`.

Top-level keys: `byCode`, `byKind`, `generic`

## frontend/src/i18n/locales/ar/groups.json
136 lines; 6215 bytes; SHA256 `5b2f4e9d6e8ddd75f0f1df88856bcfb8b361076f6e3d232154340f85d0eb8391`.

Top-level keys: `page`, `filters`, `stats`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `overview`, `tabs`, `archivedBanner`, `dialogs`, `form`, `selects`, `confirmations`, `ruleErrors`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/parents.json
57 lines; 1882 bytes; SHA256 `dd5720e03f79bbd36f375b0e3483e2f649454c7cc45caf3f1ecc7127b0e134ea`.

Top-level keys: `loading`, `error`, `detailError`, `notFound`, `card`, `children`, `relationship`, `actions`, `dialogs`, `form`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/schedule.json
146 lines; 8365 bytes; SHA256 `3f5b11e90e92b5276eab6e9d237127d5e2faefc52f2dca225f7e78c62f2a7dd8`.

Top-level keys: `weekdays`, `durations`, `timeZoneNote`, `rules`, `actions`, `addDisabled`, `addDialog`, `endDialog`, `cancelDialog`, `extendDialog`, `form`, `validation`, `outcomes`, `toasts`, `timetable`

## frontend/src/i18n/locales/ar/settings.json
29 lines; 988 bytes; SHA256 `9aeab70ee3fe5160643da5b63566770f081cc7d3e657d0162b54d11f9a42155b`.

Top-level keys: `page`, `loading`, `error`, `form`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/shell.json
53 lines; 2079 bytes; SHA256 `e12ef84c79de38d9158bf3cc262c5474c175b47f5f5edad9cec570e21875295c`.

Top-level keys: `nav`, `header`, `userMenu`, `noAccess`, `dashboard`

## frontend/src/i18n/locales/ar/staff.json
79 lines; 3053 bytes; SHA256 `b5fa1f7aa6fb8de55c9fcac768e36f20a15dabb553a852a4ad5fa042bffebe7b`.

Top-level keys: `page`, `columns`, `status`, `you`, `loading`, `error`, `emptyState`, `actions`, `dialogs`, `form`, `confirmations`, `password`, `existingAccount`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/status.json
23 lines; 1223 bytes; SHA256 `0e101afa15d8f3c21a739f45988e08828c25be39af23927b3da6ba26d908829c`.

Top-level keys: `readiness`, `systemInfo`

## frontend/src/i18n/locales/ar/students.json
119 lines; 4992 bytes; SHA256 `9d84af87c56a6d01f32e7cbdb280638d9a3b56fb6bf5f805927582b21f488aa6`.

Top-level keys: `page`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `parentsCard`, `addParent`, `archivedBanner`, `form`, `dialogs`, `confirmations`, `lookup`, `relationship`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/subjects.json
60 lines; 1877 bytes; SHA256 `df1f6ee1861d0a71207cf5f97e3ebdd65a8e82d61bcd1f7be9ecc112a61088bc`.

Top-level keys: `page`, `columns`, `status`, `actions`, `dialogs`, `form`, `loading`, `error`, `emptyState`, `confirmations`, `toasts`, `validation`

## frontend/src/i18n/locales/ar/teachers.json
87 lines; 3898 bytes; SHA256 `5c4e49d6dbb767f230359e2780a247ff9699db1f2adc533163761faf4d5d123f`.

Top-level keys: `page`, `stats`, `columns`, `actions`, `loading`, `error`, `emptyState`, `dialogs`, `form`, `account`, `confirmations`, `toasts`, `validation`

## frontend/src/i18n/locales/en/audit.json
105 lines; 2559 bytes; SHA256 `36c6e3ed0ff59cc3cb3e151f73a07a140e4adab6325bb5a6e7c56eb7467a43bc`.

Top-level keys: `page`, `filters`, `entityType`, `action`, `field`, `value`, `system`, `referenceLabel`, `copyReference`, `copied`, `loadMore`, `loading`, `error`, `emptyState`, `nullValue`

## frontend/src/i18n/locales/en/auth.json
68 lines; 2937 bytes; SHA256 `c5e167ccb82aea107356e0b6b57e44fe478d64ff56b7c9276d480fef54cd993b`.

Top-level keys: `login`, `panel`, `validation`, `changePassword`, `errors`, `selectCentre`, `roles`

## frontend/src/i18n/locales/en/common.json
22 lines; 474 bytes; SHA256 `2f32c9e1a3129d492cfa8cf51efcd4a4f57695fb8381bb4aed313aa658c83d7a`.

Top-level keys: `retry`, `close`, `loadMore`, `comingSoon`, `search`, `status`, `language`, `demoBanner`

## frontend/src/i18n/locales/en/enrollments.json
100 lines; 3572 bytes; SHA256 `f37e0c6639b5769d79b12e2f7f120f908556bb50b41cd4a0751af7c794ed6c97`.

Top-level keys: `seats`, `roster`, `actions`, `enrollDisabled`, `form`, `enrollDialog`, `picker`, `outcomes`, `toasts`, `withdrawDialog`, `transferDialog`, `studentCard`, `groupPicker`, `endReasons`, `groupsFilter`

## frontend/src/i18n/locales/en/errors.json
153 lines; 8023 bytes; SHA256 `ad9b1db252abff2c8d64d7f12d1e1a1265349e20a5b1e7fcea29d4f2b1631048`.

Top-level keys: `byCode`, `byKind`, `generic`

## frontend/src/i18n/locales/en/groups.json
136 lines; 4710 bytes; SHA256 `2c23181086349b6b35fb2f2c677f7ae009f4f7de1cf24247822baa9fa8b31492`.

Top-level keys: `page`, `filters`, `stats`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `overview`, `tabs`, `archivedBanner`, `dialogs`, `form`, `selects`, `confirmations`, `ruleErrors`, `toasts`, `validation`

## frontend/src/i18n/locales/en/parents.json
57 lines; 1399 bytes; SHA256 `20be492c1087545792b4c308487cd642c4efcaa19a247a92e00fffd336f1acee`.

Top-level keys: `loading`, `error`, `detailError`, `notFound`, `card`, `children`, `relationship`, `actions`, `dialogs`, `form`, `toasts`, `validation`

## frontend/src/i18n/locales/en/schedule.json
146 lines; 6200 bytes; SHA256 `5d8e76c0737a5cd62130cdf9bc342e061f6539cf2d46bed9950864e69f6b5ace`.

Top-level keys: `weekdays`, `durations`, `timeZoneNote`, `rules`, `actions`, `addDisabled`, `addDialog`, `endDialog`, `cancelDialog`, `extendDialog`, `form`, `validation`, `outcomes`, `toasts`, `timetable`

## frontend/src/i18n/locales/en/settings.json
29 lines; 759 bytes; SHA256 `2cff58f187be02d1dc78b51f0c0cfa66c61f4c837b19afca18ed556bbef8d061`.

Top-level keys: `page`, `loading`, `error`, `form`, `toasts`, `validation`

## frontend/src/i18n/locales/en/shell.json
53 lines; 1526 bytes; SHA256 `523fa2184e6fc98c1a4695bc8c8c2e8ce354046971a3679bfb326286cc5ec621`.

Top-level keys: `nav`, `header`, `userMenu`, `noAccess`, `dashboard`

## frontend/src/i18n/locales/en/staff.json
79 lines; 2325 bytes; SHA256 `3849f8be38863813077b4c762c8f3fd5d9083f8d7e69a888224a18e86eeea36e`.

Top-level keys: `page`, `columns`, `status`, `you`, `loading`, `error`, `emptyState`, `actions`, `dialogs`, `form`, `confirmations`, `password`, `existingAccount`, `toasts`, `validation`

## frontend/src/i18n/locales/en/status.json
23 lines; 902 bytes; SHA256 `5450badb1c106679f0562dd671106f434f7ef3c4aacb646e53da533f81982419`.

Top-level keys: `readiness`, `systemInfo`

## frontend/src/i18n/locales/en/students.json
119 lines; 3880 bytes; SHA256 `0f6e8fcafed585f481d327a8eec4ff13a6f95ccac6eae6af10959fd2c6ad1714`.

Top-level keys: `page`, `columns`, `actions`, `loading`, `error`, `detailError`, `notFound`, `emptyState`, `detail`, `parentsCard`, `addParent`, `archivedBanner`, `form`, `dialogs`, `confirmations`, `lookup`, `relationship`, `toasts`, `validation`

## frontend/src/i18n/locales/en/subjects.json
60 lines; 1509 bytes; SHA256 `0b634a395cf2a3d79ffae6b0a747f92f2a3b1e57bfcfcc69055e9e4bb3533277`.

Top-level keys: `page`, `columns`, `status`, `actions`, `dialogs`, `form`, `loading`, `error`, `emptyState`, `confirmations`, `toasts`, `validation`

## frontend/src/i18n/locales/en/teachers.json
87 lines; 2893 bytes; SHA256 `7522ac58c21e129d4ae56f540f6dc9842591639e6b54cfc93d7e84fcc7bb821f`.

Top-level keys: `page`, `stats`, `columns`, `actions`, `loading`, `error`, `emptyState`, `dialogs`, `form`, `account`, `confirmations`, `toasts`, `validation`

## frontend/src/index.css
220 lines; 7923 bytes; SHA256 `5d4d818019040c241c6c20b46e5406205e7065b45f86aac4b0b0655e5725d60d`.


## frontend/src/lib/bidi.test.ts
8 lines; 270 bytes; SHA256 `68fbccc4c0e79a76a01081e4e9dc89f45bc831c6fcfc6942ca020b96f54f16db`.

- L4: `describe("isolateLtr", () => {`
- L5: `it("wraps the value in a left-to-right isolate and its closing pop", () => {`

## frontend/src/lib/bidi.ts
10 lines; 585 bytes; SHA256 `802ea5f568391b80feef88dca5d231f7d5488c5ee8583391b479325cf9a7a0d8`.

- L8: `export function isolateLtr(value: string): string {`

## frontend/src/lib/initials.ts
11 lines; 338 bytes; SHA256 `c415e9dd4007a2e70cdca9e855ae641288eefba4549dcca7d6e8156faac70722`.

- L2: `export function initialsOf(name: string): string {`

## frontend/src/lib/utils.ts
1 lines; 24 bytes; SHA256 `14300d758170c270c75b057f38fedb3793a5920ea872f0477e29fb131185e8a7`.

- L1: `export { cn } from "cn"`

## frontend/src/main.tsx
34 lines; 1079 bytes; SHA256 `a1ae3c7218e8c12fafeca18de913d6860f221b9ba80ffa537cc5498125f83b99`.

- L10: `const router = createRouter({ routeTree, context: { queryClient } });`
- L23: `const rootElement = document.getElementById("root");`

## frontend/src/routes/__root.tsx
24 lines; 791 bytes; SHA256 `01275bad2ff296b5254c4c1461c34bfdfcf0e66143833aae1020d84d2ab80567`.

- L7: `export interface RouterContext {`
- L11: `export const Route = createRootRouteWithContext<RouterContext>()({`
- L15: `function RootLayout() {`

## frontend/src/routes/_authenticated/audit.test.tsx
286 lines; 10055 bytes; SHA256 `b4d51d9816dc346a80bc8e7bb86fd07f357f0de8780d4865d9568a47661ec881`.

- L9: `const meUrl = "/api/me";`
- L10: `const auditUrl = "/api/audit";`
- L12: `const nileOwnerMe = {`
- L24: `function entry(overrides: Record<string, unknown> = {}) {`
- L40: `function problem(status: number, code: string, detail: string) {`
- L44: `describe("AuditPage", () => {`
- L56: `it("shows a loading skeleton, then the rows", async () => {`
- L76: `it("shows an error with Retry, which succeeds", async () => {`
- L88: `it("shows the empty state when there is no activity for the filters", async () => {`
- L94: `it("renders translated field names and translated before/after values, never a raw key", async () => {`
- L127: `it("the correlation reference isolates only the id in ltr direction and can be copied", async () => {`
- L150: `it("Load more fetches the next page and appends its rows", async () => {`
- L178: `it("filters update the request: entity type, from and to", async () => {`
- L209: `it("filters by a Month 3 entity type and labels a schedule rule's fields and weekday", async () => {`
- L249: `it("without audit.view, the no-access state is shown and no audit request is issued", async () => {`
- L265: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/audit.tsx
124 lines; 4570 bytes; SHA256 `aff42a69a70ac86725297bc27790fc64b683ccfdae21ffac7c0ddefde869b8ef`.

- L1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L13: `const DefaultPageSize = 50;`
- L15: `export interface AuditSearch {`
- L22: `const KnownEntityTypes = new Set<string>(AuditEntityTypes);`
- L24: `export const Route = createFileRoute("/_authenticated/audit")({`
- L41: `function AuditPage() {`
- L43: `<RouteGuard permission={Permissions.AuditView}>`
- L49: `function AuditPageContent() {`
- L50: `const { t } = useTranslation("audit");`
- L51: `const search = Route.useSearch();`
- L52: `const navigate = useNavigate({ from: Route.fullPath });`

## frontend/src/routes/_authenticated/groups.$groupId.test.tsx
265 lines; 10915 bytes; SHA256 `5e19745e62dac69ebf9208070186112459fa76bcbfab62ad62246e099c916579`.

- L8: `const meUrl = "/api/me";`
- L9: `const groupUrl = "/api/groups/g-1";`
- L10: `const teachersUrl = "/api/teachers";`
- L12: `function me(role: "owner" | "teacher", permissions: string[]) {`
- L25: `const ownerMe = me("owner", [`
- L36: `const teacherMe = me("teacher", ["groups.view", "subjects.view"]);`
- L38: `function group(overrides: Record<string, unknown> = {}) {`
- L49: `version: 11,`
- L54: `const teachers = [`
- L55: `{ id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },`
- L56: `{ id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 0, version: 1 },`
- L59: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L66: `describe("GroupDetailPage", () => {`
- L84: `it("shows the overview with seats as a left-to-right number pair", async () => {`
- L93: `it("a 404 shows the single not-found state", async () => {`
- L102: `it("the tab parameter selects the tab, and Roster shows the roster", async () => {`
- L109: `it("choosing a tab writes it to the URL", async () => {`
- L121: `it("rename sends the new name with the loaded version", async () => {`
- L137: `expect(body).toEqual({ name: "Physics B", version: 11 });`
- L141: `it("change capacity sends a number with the loaded version", async () => {`
- L157: `expect(body).toEqual({ capacity: 25, version: 11 });`
- L161: `it("change teacher sends the chosen teacher with the loaded version", async () => {`
- L182: `expect(body).toEqual({ teacherId: "t-2", version: 11 });`
- L186: `it("a stale 409 shows the conflict toast and refetches the group", async () => {`
- L211: `it("an enrolment rule refusal gets its specific message", async () => {`
- L225: `it("changing to a teacher busy at one of the group's sessions says so and keeps the current teacher", async () => {`
- L243: `it("an archived group hides the edit actions and explains why", async () => {`
- L255: `it("a session with groups.view only sees the page read-only", async () => {`

## frontend/src/routes/_authenticated/groups.$groupId.tsx
418 lines; 15204 bytes; SHA256 `f8bfa8b9c57dabb377be058147ca818f67c048ca259886999ab467c3292c24df`.

- L2: `import { Link, createFileRoute, useNavigate } from "@tanstack/react-router";`
- L55: `const GroupTabs = ["overview", "roster", "schedule"] as const;`
- L58: `export interface GroupDetailSearch {`
- L62: `function isGroupTab(value: unknown): value is GroupTab {`
- L66: `export const Route = createFileRoute("/_authenticated/groups/$groupId")({`
- L73: `function GroupDetailPage() {`
- L75: `<RouteGuard permission={Permissions.GroupsView}>`
- L81: `function GroupDetailPageContent() {`
- L82: `const { groupId } = Route.useParams();`
- L83: `const { t } = useTranslation("groups");`
- L84: `const { t: tCommon } = useTranslation("common");`
- L85: `const { data, error, isPending, refetch } = useGetGroup(groupId);`
- L143: `function GroupNotFound() {`
- L144: `const { t } = useTranslation("groups");`
- L160: `function GroupDetailContent({ group }: { group: GroupDto }) {`
- L161: `const { t } = useTranslation("groups");`
- L162: `const { tab: requestedTab } = Route.useSearch();`
- L163: `const canViewRoster = useCan(Permissions.EnrollmentsView);`
- L164: `const canViewSchedule = useCan(Permissions.ScheduleView);`
- L169: `const navigate = useNavigate({ from: Route.fullPath });`
- L170: `const queryClient = useQueryClient();`
- L171: `const archiveMutation = useArchiveGroup();`
- L172: `const restoreMutation = useRestoreGroup();`
- L174: `const [archiveConfirmOpen, setArchiveConfirmOpen] = useState(false);`
- L179: `queryClient.invalidateQueries({ queryKey: getGetGroupQueryKey(group.id) }),`
- L180: `queryClient.invalidateQueries({ queryKey: getListGroupsQueryKey() }),`
- L185: `await archiveMutation.mutateAsync({ id: group.id, data: { version: group.version } });`
- L199: `await restoreMutation.mutateAsync({ id: group.id, data: { version: group.version } });`
- L247: `<Can permission={Permissions.GroupsManage}>`
- L391: `<Can permission={Permissions.GroupsManage}>`

## frontend/src/routes/_authenticated/groups.index.test.tsx
287 lines; 12473 bytes; SHA256 `0c1d1813b2b40620a7cf236e0b02e83ca51540f993cf598ee96694d731308b04`.

- L9: `const meUrl = "/api/me";`
- L10: `const groupsUrl = "/api/groups";`
- L11: `const subjectsUrl = "/api/subjects";`
- L12: `const teachersUrl = "/api/teachers";`
- L14: `function me(role: "owner" | "teacher", permissions: string[]) {`
- L27: `const ownerMe = me("owner", ["groups.manage", "groups.view", "subjects.manage", "subjects.view", "teachers.manage", "teachers.view"]);`
- L29: `const teacherMe = me("teacher", ["groups.view", "subjects.view"]);`
- L31: `function group(overrides: Record<string, unknown> = {}) {`
- L42: `version: 3,`
- L47: `const subjects = [`
- L48: `{ id: "s-phys", name: "Physics", status: "active", version: 1 },`
- L49: `{ id: "s-chem", name: "Chemistry", status: "active", version: 1 },`
- L51: `const teachers = [`
- L52: `{ id: "t-1", fullName: "Amina Hassan", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 1, version: 1 },`
- L53: `{ id: "t-2", fullName: "Mona Said", phone: null, status: "active", userId: null, userDisplayName: null, activeGroupCount: 0, version: 1 },`
- L56: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L63: `describe("GroupsPage", () => {`
- L77: `it("lists groups with a link to each group's page and seats as enrolled / capacity", async () => {`
- L91: `it("filters write the URL parameters and the request's query parameters", async () => {`
- L125: `it("with groups.view only: no create button, no teacher filter, and /api/teachers is never requested", async () => {`
- L143: `it("an empty list for a session without groups.manage shows the teacher's text", async () => {`
- L155: `it("an empty list for a manager offers to create the first group", async () => {`
- L164: `it("create sends exactly four fields with a numeric capacity, then opens the new group", async () => {`
- L197: `it("the subject and teacher selects show their hint until something is chosen", async () => {`
- L206: `it("create maps group.name_taken onto the name field", async () => {`
- L234: `it("a refusal about the subject lands under the subject select and refetches the subjects", async () => {`
- L266: `it("client checks cover presence and range before any request", async () => {`

## frontend/src/routes/_authenticated/groups.index.tsx
138 lines; 5056 bytes; SHA256 `cbf604abb79af603008905c77ff608c6ed332c0ce945e1817a8c1c1e7dd0a55f`.

- L1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L18: `export interface GroupsSearch {`
- L25: `export const Route = createFileRoute("/_authenticated/groups/")({`
- L35: `function GroupsPage() {`
- L37: `<RouteGuard permission={Permissions.GroupsView}>`
- L48: `function GroupsPageContent() {`
- L49: `const { t } = useTranslation("groups");`
- L50: `const { t: tEnrollments } = useTranslation("enrollments");`
- L51: `const { subject, teacher, archived = false, freeSeats = false } = Route.useSearch();`
- L52: `const navigate = useNavigate({ from: Route.fullPath });`
- L53: `const [createOpen, setCreateOpen] = useState(false);`
- L69: `<Can permission={Permissions.GroupsManage}>`
- L90: `<Can permission={Permissions.TeachersView}>`
- L133: `<Can permission={Permissions.GroupsManage}>`

## frontend/src/routes/_authenticated/groups.tsx
14 lines; 481 bytes; SHA256 `d45d2f88edcda6b2c575cb5b05478cbcc81f405a449b2268bf90a469b469bbbe`.

- L1: `import { Outlet, createFileRoute } from "@tanstack/react-router";`
- L8: `export const Route = createFileRoute("/_authenticated/groups")({`
- L12: `function GroupsLayout() {`

## frontend/src/routes/_authenticated/index.tsx
38 lines; 1307 bytes; SHA256 `0b29c6599bf4f0531b2b0aa20d9a3c27418af78e7d9b75998343a3a451c9135f`.

- L1: `import { createFileRoute } from "@tanstack/react-router";`
- L9: `export const Route = createFileRoute("/_authenticated/")({`
- L14: `function DashboardPage() {`
- L15: `const { t } = useTranslation("shell");`
- L16: `const { me } = useSession();`
- L33: `<Can permission={Permissions.GroupsView}>`

## frontend/src/routes/_authenticated/parents.$parentId.test.tsx
146 lines; 5753 bytes; SHA256 `bb38e12c6608b051785c3fc9c3b33931838050f680c11c5dcd4579696d88c85a`.

- L8: `const meUrl = "/api/me";`
- L9: `const parentUrl = "/api/parents/p-1";`
- L11: `const nileSecretaryMe = {`
- L23: `function parentDetail(overrides: Record<string, unknown> = {}) {`
- L28: `version: 1,`
- L37: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L44: `describe("ParentDetailPage", () => {`
- L53: `it("lists every child with name link, relationship, primary badge and status", async () => {`
- L68: `it("shows the empty state when the parent has no children linked", async () => {`
- L76: `it("a 404 shows the Parent not found state with a link back to students", async () => {`
- L85: `it("a phone conflict (409 parent.phone_taken) on edit shows a field error on phone", async () => {`
- L104: `it("edit sends the loaded version and invalidates on success", async () => {`
- L120: `expect(body).toEqual({ fullName: "Mahmoud H. Hassan", phone: "+201000000001", version: 1 });`
- L125: `it("with only students.view, Edit is not rendered", async () => {`
- L137: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/parents.$parentId.tsx
188 lines; 6608 bytes; SHA256 `ad5be6d9cd314fa8476570ea35282b8d6c0a0b276c6cd47724c3afca522fc54b`.

- L1: `import { Link, createFileRoute } from "@tanstack/react-router";`
- L29: `export const Route = createFileRoute("/_authenticated/parents/$parentId")({`
- L33: `function ParentDetailPage() {`
- L35: `<RouteGuard permission={Permissions.StudentsView}>`
- L41: `function ParentDetailPageContent() {`
- L42: `const { parentId } = Route.useParams();`
- L43: `const { t } = useTranslation("parents");`
- L44: `const { t: tCommon } = useTranslation("common");`
- L45: `const { data, error, isPending, refetch } = useGetParent(parentId);`
- L95: `function ParentNotFound() {`
- L96: `const { t } = useTranslation("parents");`
- L108: `function ParentDetailContent({ parent }: { parent: ParentDetailDto }) {`
- L109: `const { t } = useTranslation("parents");`
- L110: `const canManage = useCan(Permissions.StudentsManage);`
- L111: `const [editOpen, setEditOpen] = useState(false);`
- L166: `function ChildRow({ child }: { child: ParentChildDto }) {`
- L167: `const { t } = useTranslation("parents");`

## frontend/src/routes/_authenticated/settings.test.tsx
232 lines; 7910 bytes; SHA256 `1d80998e04ed008101d2f8b917522c8b9a405b87b4e773de127bd347415d2656`.

- L9: `const meUrl = "/api/me";`
- L10: `const settingsUrl = "/api/centre/settings";`
- L12: `const nileOwnerMe = {`
- L24: `function settings(overrides: Record<string, unknown> = {}) {`
- L30: `version: 1,`
- L35: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L42: `describe("SettingsPage", () => {`
- L54: `it("shows a loading skeleton, then the form prefilled with the current settings", async () => {`
- L78: `it("shows an error with Retry, which succeeds", async () => {`
- L90: `it("Save is disabled until a field changes, then saves and invalidates /api/me", async () => {`
- L117: `expect(body).toEqual({ name: "Nile Tutoring Centre — Giza", defaultLocale: "en", version: 1 });`
- L125: `it("changing the default language select sends the new value", async () => {`
- L140: `expect(body).toEqual({ name: "Nile Tutoring Centre", defaultLocale: "ar", version: 1 });`
- L144: `it("client validation blocks an empty name with no request sent", async () => {`
- L162: `it("a 409 concurrency.stale on save shows the conflict toast and refetches settings", async () => {`
- L190: `it("a 400 field error from the server is mapped onto the name field", async () => {`
- L207: `it("without centre.settings.manage, the no-access state is shown and no settings request is issued", async () => {`
- L223: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/settings.tsx
230 lines; 7837 bytes; SHA256 `f0d6cbffc0d3837a98e597f3f615b11a8da8db34a85df05869cd801a6f70250d`.

- L3: `import { createFileRoute } from "@tanstack/react-router";`
- L31: `export const Route = createFileRoute("/_authenticated/settings")({`
- L35: `const KnownFields = ["name", "defaultLocale"] as const;`
- L37: `function SettingsPage() {`
- L39: `<RouteGuard permission={Permissions.CentreSettingsManage}>`
- L45: `function SettingsPageContent() {`
- L46: `const { t } = useTranslation("settings");`
- L47: `const { t: tCommon } = useTranslation("common");`
- L48: `const queryClient = useQueryClient();`
- L49: `const { data, error, isPending, refetch } = useGetCentreSettings();`
- L50: `const mutation = useUpdateCentreSettings();`
- L67: `queryClient.invalidateQueries({ queryKey: getGetCentreSettingsQueryKey() }),`
- L68: `queryClient.invalidateQueries({ queryKey: getGetMeQueryKey() }),`
- L78: `data: { name: values.name, defaultLocale: values.defaultLocale, version: data.version },`
- L94: `await queryClient.invalidateQueries({ queryKey: getGetCentreSettingsQueryKey() });`
- L200: `function LocaleSelect({`
- L209: `const { t } = useTranslation("common");`

## frontend/src/routes/_authenticated/staff.test.tsx
375 lines; 15014 bytes; SHA256 `b265be6b5c258462a082ed6288442fc9b147bcd50757333b5c5b68aaeeb27e69`.

- L9: `const meUrl = "/api/me";`
- L10: `const staffUrl = "/api/staff";`
- L12: `const nileOwnerMe = {`
- L24: `function member(overrides: Record<string, unknown> = {}) {`
- L32: `version: 1,`
- L38: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L45: `describe("StaffPage", () => {`
- L57: `it("shows a loading skeleton, then the rows", async () => {`
- L77: `it("shows an error with Retry, which succeeds", async () => {`
- L89: `it("the current user's row is marked You and has no action menu; inactive rows are muted", async () => {`
- L117: `it("add staff: creates a member, shows the one-time password once, then an empty form on reopen", async () => {`
- L147: `it("add staff: an existing account shows the no-password message instead", async () => {`
- L164: `it("add staff: a 409 already_member shows the field error and keeps the dialog open", async () => {`
- L183: `it("add staff: client validation blocks an empty email with no request sent", async () => {`
- L201: `it("change role: Save is disabled until the role changes, then sends { role, version }", async () => {`
- L224: `expect(body).toEqual({ role: "secretary", version: 1 });`
- L229: `it("change role is not offered on your own row or on inactive rows", async () => {`
- L258: `it("a stale 409 on change role shows the conflict toast and refetches the list", async () => {`
- L290: `it("deactivate: the confirmation names the person and the centre-only scope, then requires confirmation", async () => {`
- L316: `it("reactivate: offered only on inactive rows, with no confirmation", async () => {`
- L337: `it("with staff.manage, the Add button and row menus are present", async () => {`
- L348: `it("without staff.view, the no-access state is shown and no staff request is issued", async () => {`
- L365: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/staff.tsx
69 lines; 2045 bytes; SHA256 `4fb854d67093acc97062575373e2d2bdf4a265377eca1989cd0721641dc7302e`.

- L1: `import { createFileRoute } from "@tanstack/react-router";`
- L15: `export const Route = createFileRoute("/_authenticated/staff")({`
- L19: `function StaffPage() {`
- L21: `<RouteGuard permission={Permissions.StaffView}>`
- L27: `function StaffPageContent() {`
- L28: `const { t } = useTranslation("staff");`
- L29: `const [createOpen, setCreateOpen] = useState(false);`
- L38: `<Can permission={Permissions.StaffManage}>`

## frontend/src/routes/_authenticated/students.$studentId.test.tsx
351 lines; 14295 bytes; SHA256 `d800f128864c6edb4a015f3538458478cc50116a563835c0286cb278f6048397`.

- L8: `const meUrl = "/api/me";`
- L9: `const studentUrl = "/api/students/s-1";`
- L11: `const nileSecretaryMe = {`
- L23: `function parent(overrides: Record<string, unknown> = {}) {`
- L27: `function studentDetail(overrides: Record<string, unknown> = {}) {`
- L34: `version: 1,`
- L40: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L48: `function secondRowMenuButton(): HTMLElement {`
- L57: `describe("StudentDetailPage", () => {`
- L66: `it("a 404 shows the Student not found state with a link back to the list", async () => {`
- L75: `it("edit sends the loaded version", async () => {`
- L91: `expect(body).toEqual({ fullName: "Ahmed M. Mahmoud", phone: null, schoolYear: null, version: 1 });`
- L96: `it("a stale 409 on edit shows a toast, refetches and closes the dialog", async () => {`
- L121: `it("archived hides Edit, Archive and the parents card's manage actions, and shows a banner", async () => {`
- L134: `it("archive confirmation names the student and requires confirmation", async () => {`
- L158: `it("restore requires no confirmation", async () => {`
- L177: `it("parents card: no Unlink or Make primary on the only, primary parent", async () => {`
- L187: `it("parents card: a non-primary parent (not the only one) offers both Make primary and Unlink", async () => {`
- L205: `it("parents card: Add parent is not offered at four parents", async () => {`
- L227: `it("make primary refetches the student so the next action uses a fresh version", async () => {`
- L234: `version: getCalls,`
- L255: `it("add parent: an unmatched phone creates the parent, then links it", async () => {`
- L284: `expect(linkBody).toEqual({ parentId: "p-new", relationship: "father", version: 1 });`
- L289: `it("add parent: if linking fails after the parent was created, a retry only repeats the link", async () => {`
- L326: `it("with only students.view, no manage action is rendered anywhere on the page", async () => {`
- L342: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/students.$studentId.tsx
272 lines; 9799 bytes; SHA256 `2474bb2ef26674b45869920de201e3428f2566717e34d126389eca930a040c21`.

- L2: `import { Link, createFileRoute } from "@tanstack/react-router";`
- L46: `export const Route = createFileRoute("/_authenticated/students/$studentId")({`
- L50: `function StudentDetailPage() {`
- L52: `<RouteGuard permission={Permissions.StudentsView}>`
- L58: `function StudentDetailPageContent() {`
- L59: `const { studentId } = Route.useParams();`
- L60: `const { t } = useTranslation("students");`
- L61: `const { t: tCommon } = useTranslation("common");`
- L62: `const { data, error, isPending, refetch } = useGetStudent(studentId);`
- L112: `function StudentNotFound() {`
- L113: `const { t } = useTranslation("students");`
- L125: `function StudentDetailContent({ student }: { student: StudentDetailDto }) {`
- L126: `const { t } = useTranslation("students");`
- L127: `const queryClient = useQueryClient();`
- L128: `const canManage = useCan(Permissions.StudentsManage);`
- L129: `const archiveMutation = useArchiveStudent();`
- L130: `const restoreMutation = useRestoreStudent();`
- L131: `const [archiveConfirmOpen, setArchiveConfirmOpen] = useState(false);`
- L132: `const [editOpen, setEditOpen] = useState(false);`
- L138: `queryClient.invalidateQueries({ queryKey: getGetStudentQueryKey(student.id) }),`
- L139: `queryClient.invalidateQueries({ queryKey: getListStudentsQueryKey() }),`
- L144: `await archiveMutation.mutateAsync({ id: student.id, data: { version: student.version } });`
- L154: `await queryClient.invalidateQueries({ queryKey: getListStudentEnrollmentsQueryKey(student.id) });`
- L165: `await restoreMutation.mutateAsync({ id: student.id, data: { version: student.version } });`
- L245: `<Can permission={Permissions.EnrollmentsView}>`

## frontend/src/routes/_authenticated/students.index.test.tsx
225 lines; 7980 bytes; SHA256 `071561b0e5a00982c0da983daea87cfa945527c0c5888603e841233405ddc37d`.

- L8: `const meUrl = "/api/me";`
- L9: `const studentsUrl = "/api/students";`
- L11: `const nileOwnerMe = {`
- L22: `const nileTeacherMe = {`
- L28: `function student(overrides: Record<string, unknown> = {}) {`
- L40: `function problem(status: number, code: string, detail: string) {`
- L44: `describe("StudentsPage", () => {`
- L56: `it("shows a loading skeleton, then the rows", async () => {`
- L76: `it("shows the no-search empty state with a register link", async () => {`
- L83: `it("shows the no-match empty state when a search returns nothing", async () => {`
- L96: `it("shows an error with Retry, which succeeds", async () => {`
- L108: `it("typing sends exactly one request with search after the debounce", async () => {`
- L109: `vi.useFakeTimers({ shouldAdvanceTime: true });`
- L135: `vi.useRealTimers();`
- L138: `it("the archived switch requests includeArchived=true", async () => {`
- L156: `it("load more requests the next cursor and appends the next page", async () => {`
- L175: `it("without students.manage the register button and link are absent", async () => {`
- L187: `it("in Arabic the document direction is rtl and the phone cell stays ltr", async () => {`
- L199: `it("no request is sent to any URL containing a centre id", async () => {`
- L217: `it("a teacher without students.view sees the no-access state, no navigation item", async () => {`

## frontend/src/routes/_authenticated/students.index.tsx
77 lines; 2787 bytes; SHA256 `3bd4cefdae1049b7aea1600ede056ddf45542c5afe9a9e203f8a335f32875cc7`.

- L1: `import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";`
- L15: `export interface StudentsSearch {`
- L20: `export const Route = createFileRoute("/_authenticated/students/")({`
- L28: `function StudentsPage() {`
- L30: `<RouteGuard permission={Permissions.StudentsView}>`
- L36: `function StudentsPageContent() {`
- L37: `const { t } = useTranslation("students");`
- L38: `const { q = "", archived = false } = Route.useSearch();`
- L39: `const navigate = useNavigate({ from: Route.fullPath });`
- L55: `<Can permission={Permissions.StudentsManage}>`

## frontend/src/routes/_authenticated/students.new.test.tsx
286 lines; 10906 bytes; SHA256 `8e1df81081c11c62a05ee6efac85e93f2b10bef194cec26d09f28f941939609c`.

- L9: `const meUrl = "/api/me";`
- L10: `const lookupUrl = "/api/parents/lookup";`
- L11: `const registerUrl = "/api/students";`
- L13: `const nileSecretaryMe = {`
- L25: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L32: `async function fillStudentName(name: string) {`
- L36: `async function fillParentPhone(phone: string) {`
- L41: `describe("RegisterStudentForm", () => {`
- L50: `it("lookup found: shows the existing-parent panel and sends parent: { id } only", async () => {`
- L53: `HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } }),`
- L84: `it("lookup empty: shows the name field and sends parent: { fullName, phone }", async () => {`
- L117: `it("phone.invalid on lookup shows a field error under the parent phone input", async () => {`
- L126: `it("changing the phone after a found lookup clears the panel", async () => {`
- L129: `HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } }),`
- L143: `it("a 409 parent.phone_taken on submit re-runs the lookup and shows the existing-parent panel", async () => {`
- L150: `: HttpResponse.json({ parent: { id: "p-1", fullName: "Mahmoud Hassan", phone: "+201000000001", version: 1 } });`
- L173: `it("server field errors on parent.phone and parent.fullName reach their inputs", async () => {`
- L196: `it("submit is disabled while the lookup is pending", async () => {`
- L221: `it("an empty student name blocks submission with no request sent", async () => {`
- L237: `it("with only students.view, /students/new shows the no-access state and issues no request", async () => {`
- L254: `it("the relationship select changes the request's relationship value", async () => {`
- L278: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/_authenticated/students.new.tsx
28 lines; 855 bytes; SHA256 `6fb3f854c2a1de983081d43baafdd5e0cd2a422856cba2b1b023b07f7604ac60`.

- L1: `import { createFileRoute } from "@tanstack/react-router";`
- L7: `export const Route = createFileRoute("/_authenticated/students/new")({`
- L11: `function RegisterStudentPage() {`
- L13: `<RouteGuard permission={Permissions.StudentsManage}>`
- L19: `function RegisterStudentPageContent() {`
- L20: `const { t } = useTranslation("students");`

## frontend/src/routes/_authenticated/students.tsx
17 lines; 818 bytes; SHA256 `c6cbb4e3b94e333f6c93889d90eb4a30b44f8d9d875e08b9c8896ddc91b7fb2e`.

- L1: `import { Outlet, createFileRoute } from "@tanstack/react-router";`
- L11: `export const Route = createFileRoute("/_authenticated/students")({`
- L15: `function StudentsLayout() {`

## frontend/src/routes/_authenticated/subjects.test.tsx
329 lines; 12746 bytes; SHA256 `cb50602af7194d72ca2f8f955a780d0da5cbd19ea48421f44d8967e2ead1940b`.

- L8: `const meUrl = "/api/me";`
- L9: `const subjectsUrl = "/api/subjects";`
- L11: `const nileOwnerMe = {`
- L22: `function subject(overrides: Record<string, unknown> = {}) {`
- L23: `return { id: "s-1", name: "Mathematics", status: "active", version: 1, ...overrides };`
- L26: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L33: `describe("SubjectsPage", () => {`
- L45: `it("shows a loading skeleton, then the rows", async () => {`
- L67: `it("shows the empty state when the centre has no subjects", async () => {`
- L74: `it("shows an error with Retry, which succeeds", async () => {`
- L86: `it("the toggle requests includeArchived=true", async () => {`
- L104: `it("create submits { name } only and includes the CSRF header", async () => {`
- L126: `it("create with a 409 name conflict shows the field error and keeps the dialog open", async () => {`
- L145: `it("client validation blocks an empty name with no request sent", async () => {`
- L162: `it("rename sends the version the dialog was opened with", async () => {`
- L163: `server.use(http.get(subjectsUrl, () => HttpResponse.json({ items: [subject({ version: 7 })] })));`
- L179: `expect(body).toEqual({ name: "Pure Mathematics", version: 7 });`
- L183: `it("a stale 409 on rename shows the conflict toast and refetches the list", async () => {`
- L215: `it("archive requires confirmation before the request is sent", async () => {`
- L239: `it("restore is offered only for archived rows", async () => {`
- L269: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`
- L279: `it("with only subjects.view, the Add button and row menus are absent", async () => {`
- L292: `it("with subjects.manage, the Add button and row menus are present", async () => {`
- L303: `it("a 403 auth.permission_denied on archive shows the toast and refetches /api/me", async () => {`

## frontend/src/routes/_authenticated/subjects.tsx
80 lines; 2583 bytes; SHA256 `e080f06d8aa083563e719df5d03038a94775f9e07efcbb494ee82e605908e088`.

- L1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L15: `export interface SubjectsSearch {`
- L19: `export const Route = createFileRoute("/_authenticated/subjects")({`
- L26: `function SubjectsPage() {`
- L27: `const { t } = useTranslation("subjects");`
- L28: `const { archived } = Route.useSearch();`
- L29: `const navigate = useNavigate({ from: Route.fullPath });`
- L42: `<Can permission={Permissions.SubjectsManage}>`

## frontend/src/routes/_authenticated/teachers.test.tsx
322 lines; 12444 bytes; SHA256 `67cb52d1f18d1efbf4032078b2a4c451ca6776b4876266043b92a66b86c34a71`.

- L9: `const meUrl = "/api/me";`
- L10: `const teachersUrl = "/api/teachers";`
- L11: `const staffUrl = "/api/staff";`
- L13: `function me(role: "owner" | "secretary", permissions: string[]) {`
- L27: `const ownerMe = me("owner", ["groups.manage", "groups.view", "staff.manage", "staff.view", "subjects.view", "teachers.manage", "teachers.view"]);`
- L28: `const secretaryMe = me("secretary", ["groups.manage", "groups.view", "subjects.view", "teachers.view"]);`
- L30: `function teacher(overrides: Record<string, unknown> = {}) {`
- L39: `version: 7,`
- L44: `function staffMember(overrides: Record<string, unknown> = {}) {`
- L52: `version: 1,`
- L58: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L66: `async function pickStaff(displayName: string) {`
- L73: `async function openRowMenu(name: string) {`
- L77: `describe("TeachersPage", () => {`
- L89: `it("owner sees the add button and row actions", async () => {`
- L98: `it("secretary sees the same list read-only: no add button, no row actions", async () => {`
- L108: `it("shows linked accounts by display name and the active-group count", async () => {`
- L125: `it("create sends { fullName, phone } only", async () => {`
- L146: `it("a stale edit shows the conflict toast and refetches the list", async () => {`
- L173: `it("the staff list is requested only when the account dialog opens, and offers only eligible accounts", async () => {`
- L210: `it("linking sends { userId, version }", async () => {`
- L227: `expect(body).toEqual({ userId: "u-eligible", version: 7 });`
- L259: `it("removing a link sends userId null", async () => {`
- L278: `expect(body).toEqual({ userId: null, version: 7 });`
- L282: `it("archiving a teacher with active groups shows the specific explanation", async () => {`
- L300: `it("in Arabic the document is rtl, labels are Arabic and counts use Arabic plural forms with Western digits", async () => {`

## frontend/src/routes/_authenticated/teachers.tsx
89 lines; 2889 bytes; SHA256 `fe36d8e68faa32a118660a887e3400f4796b6d30b38b84b4c4bc2b0d309ed36f`.

- L1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L16: `export interface TeachersSearch {`
- L20: `export const Route = createFileRoute("/_authenticated/teachers")({`
- L27: `function TeachersPage() {`
- L29: `<RouteGuard permission={Permissions.TeachersView}>`
- L35: `function TeachersPageContent() {`
- L36: `const { t } = useTranslation("teachers");`
- L37: `const { archived = false } = Route.useSearch();`
- L38: `const navigate = useNavigate({ from: Route.fullPath });`
- L55: `<Can permission={Permissions.TeachersManage}>`

## frontend/src/routes/_authenticated/timetable.tsx
47 lines; 1718 bytes; SHA256 `e532a0a291a8bfff287f558184f45d838bf02f47f0faf9c8c557f8a7b179b5e6`.

- L1: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L7: `export interface TimetableSearch {`
- L14: `export const Route = createFileRoute("/_authenticated/timetable")({`
- L24: `function TimetableRoute() {`
- L26: `<RouteGuard permission={Permissions.ScheduleView}>`
- L33: `function TimetableRouteContent() {`
- L34: `const { week, group, teacher } = Route.useSearch();`
- L35: `const navigate = useNavigate({ from: Route.fullPath });`

## frontend/src/routes/_authenticated.test.tsx
93 lines; 3943 bytes; SHA256 `ae66854383fb9b9c543e6d1f77f96231df2dc9b79946cfc913c53e937bf6d0fa`.

- L8: `const meUrl = "/api/me";`
- L9: `const selectCentreUrl = "/api/session/centre";`
- L10: `const logoutUrl = "/api/auth/logout";`
- L12: `const nileMembership = { centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "teacher" };`
- L13: `const maadiMembership = { centreId: "c-maadi", centreName: "Maadi Learning Hub", centreSlug: "maadi-hub", role: "teacher" };`
- L15: `function me(overrides: Partial<{ activeCentreId: string | null; memberships: unknown[] }> = {}) {`
- L30: `describe("the protected layout's guard", () => {`
- L31: `it("redirects a signed-out visitor to /login?redirect=%2F", async () => {`
- L41: `it("redirects a signed-in user with no active centre to /select-centre", async () => {`
- L51: `it("renders the shell with the display name and active centre when a centre is set", async () => {`
- L61: `describe("global 401 handling", () => {`
- L62: `it("a later request returning 401 clears the cache and returns to login", async () => {`
- L81: `it("logout clears the cache", async () => {`

## frontend/src/routes/_authenticated.tsx
48 lines; 1779 bytes; SHA256 `20eb0202a27c0bd87a95d2669e062c1f539a2e9210b4913b43f07ec1c4f06919`.

- L3: `import { Outlet, createFileRoute, redirect } from "@tanstack/react-router";`
- L14: `export const Route = createFileRoute("/_authenticated")({`
- L35: `function AuthenticatedLayout() {`
- L36: `const { me } = useSession();`

## frontend/src/routes/change-password.test.tsx
215 lines; 9478 bytes; SHA256 `3e2a81d8f419a5496c517d2b19d8bec91e43d21ad65dbad33d8819fce75085fb`.

- L8: `const meUrl = "/api/me";`
- L9: `const changePasswordUrl = "/api/auth/change-password";`
- L10: `const antiforgeryUrl = "/api/auth/antiforgery";`
- L12: `function me(overrides: Record<string, unknown> = {}) {`
- L27: `function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- L34: `describe("ChangePasswordPage", () => {`
- L39: `it("redirects to login when signed out", async () => {`
- L49: `it("shows the voluntary subtitle for a user who isn't forced to change their password", async () => {`
- L57: `it("shows the first-login subtitle when mustChangePassword is true", async () => {`
- L67: `it("client validation blocks an empty submit with no request sent", async () => {`
- L85: `it("client validation blocks a new password that doesn't match the confirmation", async () => {`
- L97: `it("success: submits the passwords, refreshes CSRF, invalidates /api/me, toasts, and goes to the dashboard", async () => {`
- L131: `it("success with no active centre goes to the centre picker instead", async () => {`
- L154: `it("a wrong current password sets a field error on the current password field", async () => {`
- L171: `it("a weak-password field error from the server is mapped onto the new password field", async () => {`
- L190: `it("a 429 rate limit shows the generic toast", async () => {`
- L205: `it("in Arabic the document direction is rtl and labels are Arabic", async () => {`

## frontend/src/routes/change-password.tsx
176 lines; 7769 bytes; SHA256 `3f9cc7943d6f577d6fe9d17b05347a476f49db200c65d0ce0647fceef8172ff2`.

- L5: `import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";`
- L22: `const KnownFields = ["currentPassword", "newPassword"] as const;`
- L24: `export const Route = createFileRoute("/change-password")({`
- L34: `function ChangePasswordPage() {`
- L35: `const { t } = useTranslation("auth");`
- L36: `const { me } = useSession();`
- L37: `const queryClient = useQueryClient();`
- L38: `const navigate = useNavigate();`
- L39: `const mutation = useChangePassword();`
- L56: `data: { currentPassword: values.currentPassword, newPassword: values.newPassword },`
- L62: `await queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey });`

## frontend/src/routes/login.test.tsx
108 lines; 4543 bytes; SHA256 `b09a87342118518fbc3c8df8a116e5e0ea4a77d2a135cc37df019e20a75a8dec`.

- L7: `const loginUrl = "/api/auth/login";`
- L9: `function problem(status: number, code: string, detail: string) {`
- L13: `describe("LoginPage", () => {`
- L14: `it("empty submit shows translated field errors and sends no request", async () => {`
- L29: `it("a 401 shows the generic message and never mentions the email", async () => {`
- L42: `it("a 429 shows the rate-limited message", async () => {`
- L53: `it("success with an active centre navigates to the redirect value", async () => {`
- L81: `it("an external redirect is ignored and lands on /", async () => {`

## frontend/src/routes/login.tsx
226 lines; 9724 bytes; SHA256 `dd6042853e035000dbb63834f6e221bac43593ed4eeee557e506247499ac4a55`.

- L3: `import { createFileRoute, useNavigate } from "@tanstack/react-router";`
- L21: `export interface LoginSearch {`
- L25: `export const Route = createFileRoute("/login")({`
- L32: `const loginSchema = z.object({`
- L38: `const KnownFields = ["email", "password"] as const;`
- L40: `const PANEL_FEATURES: { icon: LucideIcon; key: "attendance" | "groups" | "fees" | "assistant" }[] = [`
- L48: `function resolveTarget(me: MeDto, redirect: string | undefined): string {`
- L60: `function LoginPage() {`
- L61: `const { t } = useTranslation("auth");`
- L62: `const { redirect } = Route.useSearch();`
- L63: `const navigate = useNavigate();`
- L64: `const queryClient = useQueryClient();`
- L65: `const loginMutation = useLogin();`
- L78: `const me = await loginMutation.mutateAsync({ data: values });`

## frontend/src/routes/select-centre.test.tsx
92 lines; 3675 bytes; SHA256 `c614d59a6caee60e0cc997a59eed77349238adf4515a6e7a9c5f0a76c17231b0`.

- L8: `const meUrl = "/api/me";`
- L9: `const selectCentreUrl = "/api/session/centre";`
- L11: `const nileMembership = { centreId: "c-nile", centreName: "Nile Tutoring Centre", centreSlug: "nile-centre", role: "teacher" };`
- L12: `const maadiMembership = { centreId: "c-maadi", centreName: "Maadi Learning Hub", centreSlug: "maadi-hub", role: "teacher" };`
- L14: `function meWithMemberships(memberships: unknown[], activeCentreId: string | null = null) {`
- L27: `describe("SelectCentrePage", () => {`
- L28: `it("lists two centres with their translated role", async () => {`
- L37: `it("choosing a centre posts its id and navigates to /", async () => {`
- L65: `it("no active memberships shows the no-active-centre state with the signed-in email and a single logout button", async () => {`
- L77: `it("check again refetches /api/me and shows the list once a membership appears", async () => {`

## frontend/src/routes/select-centre.tsx
268 lines; 10311 bytes; SHA256 `7c77f704d17128460386c3a1edba3902b034753b946f9af54ec44c12190b12ba`.

- L4: `import { createFileRoute, redirect, useNavigate } from "@tanstack/react-router";`
- L33: `export const Route = createFileRoute("/select-centre")({`
- L48: `const ROLE_STYLE: Record<string, { icon: LucideIcon; className: string }> = {`
- L53: `const DefaultRoleStyle: { icon: LucideIcon; className: string } = { icon: Shield, className: "bg-muted text-muted-foreground" };`
- L56: `const AVATAR_PALETTE = [`
- L64: `function avatarClassName(centreId: string): string {`
- L73: `function SelectCentrePage() {`
- L74: `const { t } = useTranslation("auth");`
- L75: `const { me } = useSession();`
- L76: `const queryClient = useQueryClient();`
- L77: `const navigate = useNavigate();`
- L78: `const selectCentreMutation = useSelectCentre();`
- L79: `const signOut = useSignOut();`
- L90: `await selectCentreMutation.mutateAsync({ data: { centreId } });`
- L92: `await queryClient.invalidateQueries({ queryKey: meQueryOptions.queryKey });`
- L150: `function NoActiveCentre({ email, onSignOut }: { email: string; onSignOut: () => Promise<void> }) {`
- L151: `const { t } = useTranslation("auth");`
- L152: `const queryClient = useQueryClient();`
- L153: `const [isChecking, setIsChecking] = useState(false);`
- L154: `const [copied, setCopied] = useState(false);`
- L159: `await queryClient.refetchQueries({ queryKey: meQueryOptions.queryKey });`
- L223: `function CentreCard({`
- L230: `const { t } = useTranslation("auth");`

## frontend/src/routes/status.tsx
23 lines; 703 bytes; SHA256 `20247b53e88cc6085fee259dca7cfe10831379bafe066036f85650a37c04e2b4`.

- L1: `import { createFileRoute } from "@tanstack/react-router";`
- L6: `export const Route = createFileRoute("/status")({`
- L10: `function StatusPage() {`

## frontend/src/routeTree.gen.ts
489 lines; 18267 bytes; SHA256 `1f5f43ee1946a28e5c5c5232a3de9db12c929fb30aedb2d00c2db67b191a7c9b`.

- L33: `const AuthenticatedRoute = AuthenticatedRouteImport.update({`
- L37: `const ChangePasswordRoute = ChangePasswordRouteImport.update({`
- L42: `const LoginRoute = LoginRouteImport.update({`
- L47: `const SelectCentreRoute = SelectCentreRouteImport.update({`
- L52: `const StatusRoute = StatusRouteImport.update({`
- L57: `const AuthenticatedIndexRoute = AuthenticatedIndexRouteImport.update({`
- L62: `const AuthenticatedAuditRoute = AuthenticatedAuditRouteImport.update({`
- L67: `const AuthenticatedGroupsRoute = AuthenticatedGroupsRouteImport.update({`
- L72: `const AuthenticatedSettingsRoute = AuthenticatedSettingsRouteImport.update({`
- L77: `const AuthenticatedStaffRoute = AuthenticatedStaffRouteImport.update({`
- L82: `const AuthenticatedStudentsRoute = AuthenticatedStudentsRouteImport.update({`
- L87: `const AuthenticatedSubjectsRoute = AuthenticatedSubjectsRouteImport.update({`
- L92: `const AuthenticatedTeachersRoute = AuthenticatedTeachersRouteImport.update({`
- L97: `const AuthenticatedTimetableRoute = AuthenticatedTimetableRouteImport.update({`
- L102: `const AuthenticatedGroupsIndexRoute =`
- L108: `const AuthenticatedGroupsGroupIdRoute =`
- L114: `const AuthenticatedParentsParentIdRoute =`
- L120: `const AuthenticatedStudentsIndexRoute =`
- L126: `const AuthenticatedStudentsStudentIdRoute =`
- L132: `const AuthenticatedStudentsNewRoute =`
- L139: `export interface FileRoutesByFullPath {`
- L160: `export interface FileRoutesByTo {`
- L179: `export interface FileRoutesById {`
- L202: `export interface FileRouteTypes {`
- L267: `export interface RootRouteChildren {`
- L425: `const AuthenticatedGroupsRouteChildren: AuthenticatedGroupsRouteChildren = {`
- L430: `const AuthenticatedGroupsRouteWithChildren =`
- L439: `const AuthenticatedStudentsRouteChildren: AuthenticatedStudentsRouteChildren = {`
- L445: `const AuthenticatedStudentsRouteWithChildren =`
- L463: `const AuthenticatedRouteChildren: AuthenticatedRouteChildren = {`
- L476: `const AuthenticatedRouteWithChildren = AuthenticatedRoute._addFileChildren(`
- L480: `const rootRouteChildren: RootRouteChildren = {`
- L487: `export const routeTree = rootRouteImport`

## frontend/src/test/msw/handlers.ts
24 lines; 950 bytes; SHA256 `d845f20390268fc6ea3cd342c3eab0374be8c9300d62ac784f25ed0ebfdaa056`.

- L4: `export const handlers = [`

## frontend/src/test/msw/problemDetails.ts
35 lines; 1498 bytes; SHA256 `58db34e9029e7e3e795c3a2517a56d461cefb04d1f7c06d51b58afc666c153aa`.

- L3: `const TitleByStatus: Record<number, string> = {`
- L15: `export function problemDetails(status: number, code: string, detail: string) {`
- L31: `export const groupFullProblem = () => problemDetails(409, "group.full", "This group has no free seats.");`
- L34: `export const alreadyEnrolledProblem = () =>`

## frontend/src/test/msw/server.ts
4 lines; 128 bytes; SHA256 `83a51eb356c17ca185e7259ad42122fb6da4af234b226a7d0d7fb5f73e14a093`.

- L4: `export const server = setupServer(...handlers);`

## frontend/src/test/render.tsx
28 lines; 1139 bytes; SHA256 `836f18d4c1d6600a0238a4611b257de40947c9c5502f71316ead71ae5dbfd54e`.

- L5: `function newTestQueryClient(): QueryClient {`
- L12: `export function renderWithQueryClient(ui: ReactElement): RenderResult {`
- L19: `export function renderHookWithQueryClient<TResult, TProps>(`

## frontend/src/test/renderRouter.tsx
28 lines; 1232 bytes; SHA256 `4246c9d88e953b217b9bfc3e63afe3edcd31461e5cc0f899e4f2e4ec407def1a`.

- L12: `export function renderRouter(initialPath: string) {`

## frontend/src/test/selectOption.ts
16 lines; 810 bytes; SHA256 `6fa8256cecbe8140437c829305819c6e986c4e2231ffcdd492d25199d9642cde`.

- L11: `export async function selectOption(triggerName: string, optionName: string): Promise<void> {`

## frontend/src/test/setup.ts
33 lines; 1111 bytes; SHA256 `e369f3a5e27109193557c984e4364f64620bc5345b7a39ac03fa798e80197973`.


## frontend/tsconfig.app.json
31 lines; 760 bytes; SHA256 `9f97ee9a6e62bf3e4189ee5f80b8f0147d2fd0ec77067c2c9b67bd5c0a29cd80`.

Top-level keys:

## frontend/tsconfig.json
9 lines; 181 bytes; SHA256 `fbe0631e0f524ce62e1d197f9f251ac0d8998a6b684ef66c8da174d66cbb31ca`.

Top-level keys: `files`, `references`, `compilerOptions`

## frontend/tsconfig.node.json
23 lines; 608 bytes; SHA256 `80215eba24ed8aa678ced80294ae3aab406cbc52aa37624836dd40cdd4a208ae`.

Top-level keys:

## frontend/vite.config.ts
46 lines; 2077 bytes; SHA256 `e67663cdb35d7011139e76aa0b69c70c8208d8fd3a41d253b46cf77f5dd5a68f`.

- L12: `const apiTarget = process.env.API_TARGET ?? "https://localhost:7197";`
- L14: `export default defineConfig({`
- L28: `// 0, not the Vite default (4096): a small font file inlined as a data: URI is blocked by`
