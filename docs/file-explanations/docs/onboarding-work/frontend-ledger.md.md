# `docs/onboarding-work/frontend-ledger.md`

## Purpose

Documentation covering Frontend read ledger; frontend/.gitignore; frontend/.prettierignore; frontend/.prettierrc; frontend/components.json. Treat historical claims according to the revision described in the file.

## Where it fits

Category: **documentation**. [Open current file](../../../onboarding-work/frontend-ledger.md). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 3498 lines and 225,069 bytes. Source hash: `ba103ec5e4441703277f481e3cb81ee3063276ec67d8adf541082807dbb221a3`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 26](../../../onboarding-work/frontend-ledger.md#L26): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 35](../../../onboarding-work/frontend-ledger.md#L35): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 36](../../../onboarding-work/frontend-ledger.md#L36): `- L20: async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- [Line 37](../../../onboarding-work/frontend-ledger.md#L37): `- L26: async function createGroup(page: Page, name: string, capacity: number): Promise<string> {`
- [Line 38](../../../onboarding-work/frontend-ledger.md#L38): `- L41: async function enrollBySearch(page: Page, search: string, studentName: string): Promise<void> {`
- [Line 39](../../../onboarding-work/frontend-ledger.md#L39): `- L52: async function prepareEnrollFromStudentPage(browser: Browser, studentName: string, groupName: string) {`
- [Line 47](../../../onboarding-work/frontend-ledger.md#L47): `- L15: async function signIn(page: Page, email: string, withPassword: string): Promise<void> {`
- [Line 57](../../../onboarding-work/frontend-ledger.md#L57): `- L22: function nextSaturdayInCairo(): string {`
- [Line 58](../../../onboarding-work/frontend-ledger.md#L58): `- L34: function uniquePhone(): string {`
- [Line 59](../../../onboarding-work/frontend-ledger.md#L59): `- L39: async function signIn(page: Page, email: string, withPassword: string): Promise<void> {`
- [Line 60](../../../onboarding-work/frontend-ledger.md#L60): `- L48: async function choose(page: Page, scope: Locator, label: string, option: string | RegExp): Promise<void> {`
- [Line 61](../../../onboarding-work/frontend-ledger.md#L61): `- L54: async function addWeeklySession(page: Page, groupPath: string, day: string, ninetyMinutes: string): Promise<void> {`
- [Line 68](../../../onboarding-work/frontend-ledger.md#L68): `- L14: function nextSaturdayInCairo(): string {`
- [Line 69](../../../onboarding-work/frontend-ledger.md#L69): `- L25: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 70](../../../onboarding-work/frontend-ledger.md#L70): `- L35: async function choose(page: Page, scope: Locator, label: string, option: string): Promise<void> {`
- [Line 71](../../../onboarding-work/frontend-ledger.md#L71): `- L41: async function addTeacher(page: Page, fullName: string): Promise<void> {`
- [Line 72](../../../onboarding-work/frontend-ledger.md#L72): `- L50: async function createGroup(page: Page, name: string, teacherName: string): Promise<string> {`
- [Line 73](../../../onboarding-work/frontend-ledger.md#L73): `- L63: async function addWeeklySession(`
- [Line 74](../../../onboarding-work/frontend-ledger.md#L74): `- L82: async function scheduleAnotherTeachersGroup(browser: Browser, groupName: string, saturday: string): Promise<void> {`
- [Line 82](../../../onboarding-work/frontend-ledger.md#L82): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 89](../../../onboarding-work/frontend-ledger.md#L89): `- L14: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 90](../../../onboarding-work/frontend-ledger.md#L90): `- L24: async function addStaffMember(page: Page, email: string, displayName: string): Promise<void> {`
- [Line 91](../../../onboarding-work/frontend-ledger.md#L91): `- L34: async function changeRole(page: Page, displayName: string, roleName: "Teacher" | "Assistant"): Promise<void> {`
- [Line 98](../../../onboarding-work/frontend-ledger.md#L98): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 99](../../../onboarding-work/frontend-ledger.md#L99): `- L22: function uniquePhone(): string {`
- [Line 108](../../../onboarding-work/frontend-ledger.md#L108): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 116](../../../onboarding-work/frontend-ledger.md#L116): `- L10: async function signIn(page: Page, email: string): Promise<void> {`
- [Line 117](../../../onboarding-work/frontend-ledger.md#L117): `- L20: async function choose(page: Page, scope: ReturnType<Page["getByRole"]>, label: string, option: string): Promise<void> {`
- [Line 118](../../../onboarding-work/frontend-ledger.md#L118): `- L29: async function urlOfGroupTaughtByMona(browser: Browser): Promise<string> {`
- [Line 285](../../../onboarding-work/frontend-ledger.md#L285): `- L13: function flattenKeys(value, prefix = "") {`
- [Line 286](../../../onboarding-work/frontend-ledger.md#L286): `- L23: function loadNamespace(language, namespace) {`
- [Line 287](../../../onboarding-work/frontend-ledger.md#L287): `- L29: function namespacesFor(language) {`
- [Line 314](../../../onboarding-work/frontend-ledger.md#L314): `- L14: export class ApiRequestError extends Error implements ApiError {`
- [Line 315](../../../onboarding-work/frontend-ledger.md#L315): `- L37: function kindFromStatus(status: number): ErrorKind {`
- [Line 316](../../../onboarding-work/frontend-ledger.md#L316): `- L57: export function toApiError(body: unknown, status: number): ApiError {`
- [Line 317](../../../onboarding-work/frontend-ledger.md#L317): `- L78: export function asApiError(error: unknown): ApiError {`
- [Line 318](../../../onboarding-work/frontend-ledger.md#L318): `- L91: async function readBody(response: Response): Promise<unknown> {`
- [Line 319](../../../onboarding-work/frontend-ledger.md#L319): `- L104: function isCsrfError(error: unknown): boolean {`
- [Line 320](../../../onboarding-work/frontend-ledger.md#L320): `- L108: async function buildHeaders(init: RequestInit | undefined, attachCsrfToken: boolean): Promise<Headers> {`
- [Line 321](../../../onboarding-work/frontend-ledger.md#L321): `- L123: async function sendOnce<T>(url: string, init: RequestInit | undefined, headers: Headers): Promise<T> {`
- [Line 322](../../../onboarding-work/frontend-ledger.md#L322): `- L158: export async function apiFetch<T>(url: string, init?: RequestInit): Promise<T> {`
- [Line 327](../../../onboarding-work/frontend-ledger.md#L327): `- L7: async function fetchToken(): Promise<string> {`
- [Line 328](../../../onboarding-work/frontend-ledger.md#L328): `- L19: export function getCsrfToken(): Promise<string> {`
- [Line 329](../../../onboarding-work/frontend-ledger.md#L329): `- L25: export function refreshCsrfToken(): Promise<string> {`
- [Line 345](../../../onboarding-work/frontend-ledger.md#L345): `- L7: export function messageFor(error: ApiError): string {`
- [Line 351](../../../onboarding-work/frontend-ledger.md#L351): `- L15: export interface ApiError {`
- [Line 356](../../../onboarding-work/frontend-ledger.md#L356): `- L8: function hasCodeMessage(error: ApiError): boolean {`
- [Line 357](../../../onboarding-work/frontend-ledger.md#L357): `- L19: export function applyTranslatedFieldErrors<TField extends string>(`
- [Line 358](../../../onboarding-work/frontend-ledger.md#L358): `- L43: export function setCodedFieldError<TField extends string>(error: ApiError, setError: SetFieldError<TField>, field: TField): void {`
- [Line 376](../../../onboarding-work/frontend-ledger.md#L376): `- L40: export interface AddScheduleRuleRequest {`
- [Line 377](../../../onboarding-work/frontend-ledger.md#L377): `- L50: export interface AddScheduleRuleResponse {`
- [Line 378](../../../onboarding-work/frontend-ledger.md#L378): `- L56: export interface AntiforgeryTokenResponse {`
- [Line 379](../../../onboarding-work/frontend-ledger.md#L379): `- L60: export interface AssignGroupTeacherRequest {`
- [Line 381](../../../onboarding-work/frontend-ledger.md#L381): `- L66: export interface AuditChangeDto {`
- [Line 382](../../../onboarding-work/frontend-ledger.md#L382): `- L74: export interface AuditEntryDto {`
- [Line 383](../../../onboarding-work/frontend-ledger.md#L383): `- L90: export interface AuditPageDto {`
- [Line 386](../../../onboarding-work/frontend-ledger.md#L386): `- L104: export interface CentreSettingsDto {`
- [Line 388](../../../onboarding-work/frontend-ledger.md#L388): `- L113: export interface ChangeGroupCapacityRequest {`
- [Line 390](../../../onboarding-work/frontend-ledger.md#L390): `- L120: export interface ChangePasswordRequest {`
- [Line 393](../../../onboarding-work/frontend-ledger.md#L393): `- L134: export interface ChangeStaffRoleRequest {`
- [Line 397](../../../onboarding-work/frontend-ledger.md#L397): `- L148: export interface CreateGroupRequest {`
- [Line 398](../../../onboarding-work/frontend-ledger.md#L398): `- L156: export interface CreateGroupResponse {`
- [Line 399](../../../onboarding-work/frontend-ledger.md#L399): `- L160: export interface CreateParentRequest {`
- [Line 400](../../../onboarding-work/frontend-ledger.md#L400): `- L165: export interface CreateParentResponse {`
- [Line 401](../../../onboarding-work/frontend-ledger.md#L401): `- L169: export interface CreateStaffRequest {`
- [Line 402](../../../onboarding-work/frontend-ledger.md#L402): `- L176: export interface CreateStaffResponse {`
- [Line 403](../../../onboarding-work/frontend-ledger.md#L403): `- L183: export interface CreateSubjectRequest {`
- [Line 404](../../../onboarding-work/frontend-ledger.md#L404): `- L187: export interface CreateSubjectResponse {`
- [Line 405](../../../onboarding-work/frontend-ledger.md#L405): `- L191: export interface CreateTeacherRequest {`
- [Line 406](../../../onboarding-work/frontend-ledger.md#L406): `- L197: export interface CreateTeacherResponse {`
- [Line 407](../../../onboarding-work/frontend-ledger.md#L407): `- L201: export interface EndScheduleRuleRequest {`
- [Line 413](../../../onboarding-work/frontend-ledger.md#L413): `- L223: export interface EnrollmentVersionRequest {`
- [Line 415](../../../onboarding-work/frontend-ledger.md#L415): `- L228: export interface EnrollStudentRequest {`
- [Line 416](../../../onboarding-work/frontend-ledger.md#L416): `- L232: export interface EnrollStudentResponse {`
- [Line 417](../../../onboarding-work/frontend-ledger.md#L417): `- L236: export interface ExtendScheduleRequest {`
- [Line 418](../../../onboarding-work/frontend-ledger.md#L418): `- L240: export interface ExtendScheduleResponse {`
- [Line 421](../../../onboarding-work/frontend-ledger.md#L421): `- L253: export interface GroupDto {`
- [Line 423](../../../onboarding-work/frontend-ledger.md#L423): `- L269: export interface GroupListResponse {`
- [Line 424](../../../onboarding-work/frontend-ledger.md#L424): `- L273: export interface RosterEntryDto {`
- [Line 426](../../../onboarding-work/frontend-ledger.md#L426): `- L282: export interface GroupRosterDto {`
- [Line 427](../../../onboarding-work/frontend-ledger.md#L427): `- L292: export interface GroupVersionRequest {`
- [Line 430](../../../onboarding-work/frontend-ledger.md#L430): `- L299: export interface HttpValidationProblemDetails {`
- [Line 433](../../../onboarding-work/frontend-ledger.md#L433): `- L326: export interface LinkStudentParentRequest {`
- [Line 435](../../../onboarding-work/frontend-ledger.md#L435): `- L333: export interface LoginRequest {`
- [Line 436](../../../onboarding-work/frontend-ledger.md#L436): `- L338: export interface LookupParentRequest {`
- [Line 437](../../../onboarding-work/frontend-ledger.md#L437): `- L342: export interface MembershipDto {`
- [Line 438](../../../onboarding-work/frontend-ledger.md#L438): `- L350: export interface MeDto {`
- [Line 443](../../../onboarding-work/frontend-ledger.md#L443): `- L379: export interface ParentChildDto {`
- [Line 444](../../../onboarding-work/frontend-ledger.md#L444): `- L387: export interface ParentDetailDto {`
- [Line 446](../../../onboarding-work/frontend-ledger.md#L446): `- L396: export interface ParentDto {`
- [Line 448](../../../onboarding-work/frontend-ledger.md#L448): `- L404: export interface ParentLookupDto {`
- [Line 449](../../../onboarding-work/frontend-ledger.md#L449): `- L408: export interface ProblemDetails {`
- [Line 450](../../../onboarding-work/frontend-ledger.md#L450): `- L424: export interface RegisterStudentRequestParent {`
- [Line 451](../../../onboarding-work/frontend-ledger.md#L451): `- L433: export interface RegisterStudentRequest {`
- [Line 452](../../../onboarding-work/frontend-ledger.md#L452): `- L443: export interface RegisterStudentResponse {`
- [Line 453](../../../onboarding-work/frontend-ledger.md#L453): `- L448: export interface RenameGroupRequest {`
- [Line 455](../../../onboarding-work/frontend-ledger.md#L455): `- L454: export interface RenameSubjectRequest {`
- [Line 457](../../../onboarding-work/frontend-ledger.md#L457): `- L460: export interface ScheduleRuleDto {`
- [Line 459](../../../onboarding-work/frontend-ledger.md#L459): `- L475: export interface ScheduleRuleListResponse {`
- [Line 460](../../../onboarding-work/frontend-ledger.md#L460): `- L479: export interface SelectCentreRequest {`
- [Line 461](../../../onboarding-work/frontend-ledger.md#L461): `- L483: export interface SessionVersionRequest {`
- [Line 463](../../../onboarding-work/frontend-ledger.md#L463): `- L488: export interface SetTeacherAccountRequest {`
- [Line 465](../../../onboarding-work/frontend-ledger.md#L465): `- L495: export interface StaffMemberDto {`
- [Line 467](../../../onboarding-work/frontend-ledger.md#L467): `- L507: export interface StaffListResponse {`
- [Line 468](../../../onboarding-work/frontend-ledger.md#L468): `- L511: export interface StaffVersionRequest {`
- [Line 470](../../../onboarding-work/frontend-ledger.md#L470): `- L516: export interface StudentParentDto {`
- [Line 471](../../../onboarding-work/frontend-ledger.md#L471): `- L524: export interface StudentDetailDto {`
- [Line 473](../../../onboarding-work/frontend-ledger.md#L473): `- L537: export interface StudentEnrollmentDto {`
- [Line 475](../../../onboarding-work/frontend-ledger.md#L475): `- L552: export interface StudentEnrollmentListResponse {`
- [Line 476](../../../onboarding-work/frontend-ledger.md#L476): `- L556: export interface StudentListItemDto {`
- [Line 477](../../../onboarding-work/frontend-ledger.md#L477): `- L568: export interface StudentPageDto {`
- [Line 478](../../../onboarding-work/frontend-ledger.md#L478): `- L574: export interface StudentVersionRequest {`
- [Line 482](../../../onboarding-work/frontend-ledger.md#L482): `- L587: export interface SubjectDto {`
- [Line 484](../../../onboarding-work/frontend-ledger.md#L484): `- L595: export interface SubjectListResponse {`
- [Line 485](../../../onboarding-work/frontend-ledger.md#L485): `- L599: export interface SubjectVersionRequest {`
- [Line 487](../../../onboarding-work/frontend-ledger.md#L487): `- L604: export interface SystemInfoDto {`
- [Line 490](../../../onboarding-work/frontend-ledger.md#L490): `- L620: export interface TeacherDto {`
- [Line 492](../../../onboarding-work/frontend-ledger.md#L492): `- L636: export interface TeacherListResponse {`
- [Line 493](../../../onboarding-work/frontend-ledger.md#L493): `- L640: export interface TeacherVersionRequest {`
- [Line 495](../../../onboarding-work/frontend-ledger.md#L495): `- L645: export interface TimetableSessionDto {`
- [Line 497](../../../onboarding-work/frontend-ledger.md#L497): `- L662: export interface TimetableDto {`
- [Line 498](../../../onboarding-work/frontend-ledger.md#L498): `- L668: export interface TransferEnrollmentRequest {`
- [Line 500](../../../onboarding-work/frontend-ledger.md#L500): `- L674: export interface TransferEnrollmentResponse {`
- [Line 501](../../../onboarding-work/frontend-ledger.md#L501): `- L678: export interface UpdateCentreSettingsRequest {`
- [Line 503](../../../onboarding-work/frontend-ledger.md#L503): `- L685: export interface UpdateParentRequest {`
- [Line 505](../../../onboarding-work/frontend-ledger.md#L505): `- L692: export interface UpdateStudentRequest {`
- [Line 507](../../../onboarding-work/frontend-ledger.md#L507): `- L702: export interface UpdateTeacherRequest {`
- [Line 524](../../../onboarding-work/frontend-ledger.md#L524): `- L824: export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- [Line 526](../../../onboarding-work/frontend-ledger.md#L526): `- L834: export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- [Line 528](../../../onboarding-work/frontend-ledger.md#L528): `- L844: export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- [Line 530](../../../onboarding-work/frontend-ledger.md#L530): `- L849: export function useGetSystemInfo<TData = Awaited<ReturnType<typeof getSystemInfo>>, TError = ProblemDetails>(`
- [Line 540](../../../onboarding-work/frontend-ledger.md#L540): `- L919: export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- [Line 542](../../../onboarding-work/frontend-ledger.md#L542): `- L929: export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- [Line 544](../../../onboarding-work/frontend-ledger.md#L544): `- L939: export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- [Line 546](../../../onboarding-work/frontend-ledger.md#L546): `- L944: export function useGetAntiforgeryToken<TData = Awaited<ReturnType<typeof getAntiforgeryToken>>, TError = unknown>(`
- [Line 569](../../../onboarding-work/frontend-ledger.md#L569): `- L1164: export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- [Line 571](../../../onboarding-work/frontend-ledger.md#L571): `- L1174: export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- [Line 573](../../../onboarding-work/frontend-ledger.md#L573): `- L1184: export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- [Line 575](../../../onboarding-work/frontend-ledger.md#L575): `- L1189: export function useGetMe<TData = Awaited<ReturnType<typeof getMe>>, TError = ProblemDetails>(`
- [Line 599](../../../onboarding-work/frontend-ledger.md#L599): `- L1430: export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- [Line 601](../../../onboarding-work/frontend-ledger.md#L601): `- L1440: export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- [Line 603](../../../onboarding-work/frontend-ledger.md#L603): `- L1450: export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- [Line 605](../../../onboarding-work/frontend-ledger.md#L605): `- L1455: export function useListSubjects<TData = Awaited<ReturnType<typeof listSubjects>>, TError = ProblemDetails>(`
- [Line 622](../../../onboarding-work/frontend-ledger.md#L622): `- L1607: export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- [Line 624](../../../onboarding-work/frontend-ledger.md#L624): `- L1617: export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- [Line 626](../../../onboarding-work/frontend-ledger.md#L626): `- L1627: export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- [Line 628](../../../onboarding-work/frontend-ledger.md#L628): `- L1632: export function useGetSubject<TData = Awaited<ReturnType<typeof getSubject>>, TError = ProblemDetails>(`
- [Line 659](../../../onboarding-work/frontend-ledger.md#L659): `- L1958: export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- [Line 661](../../../onboarding-work/frontend-ledger.md#L661): `- L1968: export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- [Line 663](../../../onboarding-work/frontend-ledger.md#L663): `- L1978: export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- [Line 665](../../../onboarding-work/frontend-ledger.md#L665): `- L1983: export function useListTeachers<TData = Awaited<ReturnType<typeof listTeachers>>, TError = ProblemDetails>(`
- [Line 682](../../../onboarding-work/frontend-ledger.md#L682): `- L2135: export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- [Line 684](../../../onboarding-work/frontend-ledger.md#L684): `- L2145: export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- [Line 686](../../../onboarding-work/frontend-ledger.md#L686): `- L2155: export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- [Line 688](../../../onboarding-work/frontend-ledger.md#L688): `- L2160: export function useGetTeacher<TData = Awaited<ReturnType<typeof getTeacher>>, TError = ProblemDetails>(`
- [Line 726](../../../onboarding-work/frontend-ledger.md#L726): `- L2569: export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- [Line 728](../../../onboarding-work/frontend-ledger.md#L728): `- L2579: export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- [Line 730](../../../onboarding-work/frontend-ledger.md#L730): `- L2589: export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- [Line 732](../../../onboarding-work/frontend-ledger.md#L732): `- L2594: export function useListGroups<TData = Awaited<ReturnType<typeof listGroups>>, TError = ProblemDetails>(`
- [Line 749](../../../onboarding-work/frontend-ledger.md#L749): `- L2746: export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- [Line 751](../../../onboarding-work/frontend-ledger.md#L751): `- L2756: export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- [Line 753](../../../onboarding-work/frontend-ledger.md#L753): `- L2766: export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- [Line 755](../../../onboarding-work/frontend-ledger.md#L755): `- L2771: export function useGetGroup<TData = Awaited<ReturnType<typeof getGroup>>, TError = ProblemDetails>(`
- [Line 807](../../../onboarding-work/frontend-ledger.md#L807): `- L3339: export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- [Line 809](../../../onboarding-work/frontend-ledger.md#L809): `- L3349: export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- [Line 811](../../../onboarding-work/frontend-ledger.md#L811): `- L3359: export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- [Line 813](../../../onboarding-work/frontend-ledger.md#L813): `- L3364: export function useGetGroupRoster<TData = Awaited<ReturnType<typeof getGroupRoster>>, TError = ProblemDetails>(`
- [Line 823](../../../onboarding-work/frontend-ledger.md#L823): `- L3434: export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- [Line 825](../../../onboarding-work/frontend-ledger.md#L825): `- L3444: export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- [Line 827](../../../onboarding-work/frontend-ledger.md#L827): `- L3454: export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- [Line 829](../../../onboarding-work/frontend-ledger.md#L829): `- L3459: export function useListStudentEnrollments<TData = Awaited<ReturnType<typeof listStudentEnrollments>>, TError = ProblemDetails>(`
- [Line 853](../../../onboarding-work/frontend-ledger.md#L853): `- L3695: export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- [Line 855](../../../onboarding-work/frontend-ledger.md#L855): `- L3705: export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- [Line 857](../../../onboarding-work/frontend-ledger.md#L857): `- L3715: export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- [Line 859](../../../onboarding-work/frontend-ledger.md#L859): `- L3720: export function useListGroupScheduleRules<TData = Awaited<ReturnType<typeof listGroupScheduleRules>>, TError = ProblemDetails>(`
- [Line 890](../../../onboarding-work/frontend-ledger.md#L890): `- L4045: export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 892](../../../onboarding-work/frontend-ledger.md#L892): `- L4055: export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 894](../../../onboarding-work/frontend-ledger.md#L894): `- L4065: export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 896](../../../onboarding-work/frontend-ledger.md#L896): `- L4070: export function useGetTimetable<TData = Awaited<ReturnType<typeof getTimetable>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 920](../../../onboarding-work/frontend-ledger.md#L920): `- L4305: export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- [Line 922](../../../onboarding-work/frontend-ledger.md#L922): `- L4315: export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- [Line 924](../../../onboarding-work/frontend-ledger.md#L924): `- L4325: export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- [Line 926](../../../onboarding-work/frontend-ledger.md#L926): `- L4330: export function useGetParent<TData = Awaited<ReturnType<typeof getParent>>, TError = ProblemDetails>(`
- [Line 950](../../../onboarding-work/frontend-ledger.md#L950): `- L4572: export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 952](../../../onboarding-work/frontend-ledger.md#L952): `- L4582: export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 954](../../../onboarding-work/frontend-ledger.md#L954): `- L4592: export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 956](../../../onboarding-work/frontend-ledger.md#L956): `- L4597: export function useListStudents<TData = Awaited<ReturnType<typeof listStudents>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 973](../../../onboarding-work/frontend-ledger.md#L973): `- L4749: export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- [Line 975](../../../onboarding-work/frontend-ledger.md#L975): `- L4759: export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- [Line 977](../../../onboarding-work/frontend-ledger.md#L977): `- L4769: export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- [Line 979](../../../onboarding-work/frontend-ledger.md#L979): `- L4774: export function useGetStudent<TData = Awaited<ReturnType<typeof getStudent>>, TError = ProblemDetails>(`
- [Line 1031](../../../onboarding-work/frontend-ledger.md#L1031): `- L5346: export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- [Line 1033](../../../onboarding-work/frontend-ledger.md#L1033): `- L5356: export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- [Line 1035](../../../onboarding-work/frontend-ledger.md#L1035): `- L5366: export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- [Line 1037](../../../onboarding-work/frontend-ledger.md#L1037): `- L5371: export function useListStaff<TData = Awaited<ReturnType<typeof listStaff>>, TError = ProblemDetails>(`
- [Line 1075](../../../onboarding-work/frontend-ledger.md#L1075): `- L5779: export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 1077](../../../onboarding-work/frontend-ledger.md#L1077): `- L5789: export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 1079](../../../onboarding-work/frontend-ledger.md#L1079): `- L5799: export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 1081](../../../onboarding-work/frontend-ledger.md#L1081): `- L5804: export function useGetAuditLog<TData = Awaited<ReturnType<typeof getAuditLog>>, TError = HttpValidationProblemDetails | ProblemDetails>(`
- [Line 1091](../../../onboarding-work/frontend-ledger.md#L1091): `- L5874: export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- [Line 1093](../../../onboarding-work/frontend-ledger.md#L1093): `- L5884: export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- [Line 1095](../../../onboarding-work/frontend-ledger.md#L1095): `- L5894: export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- [Line 1097](../../../onboarding-work/frontend-ledger.md#L1097): `- L5899: export function useGetCentreSettings<TData = Awaited<ReturnType<typeof getCentreSettings>>, TError = ProblemDetails>(`
- [Line 1119](../../../onboarding-work/frontend-ledger.md#L1119): `- L2: export interface ProblemDetails {`
- [Line 1120](../../../onboarding-work/frontend-ledger.md#L1120): `- L17: export function isProblemDetails(value: unknown): value is ProblemDetails {`
- [Line 1136](../../../onboarding-work/frontend-ledger.md#L1136): `- L6: export function showApiError(error: ApiError): void {`
- [Line 1137](../../../onboarding-work/frontend-ledger.md#L1137): `- L17: export function applyFieldErrors<TField extends string>(`
- [Line 1142](../../../onboarding-work/frontend-ledger.md#L1142): `- L9: export function setLoginRedirect(fn: LoginRedirect): void {`
- [Line 1144](../../../onboarding-work/frontend-ledger.md#L1144): `- L15: function isLoginMutation(mutationKey: readonly unknown[] | undefined): boolean {`
- [Line 1145](../../../onboarding-work/frontend-ledger.md#L1145): `- L28: export function createAppQueryClient(): QueryClient {`
- [Line 1151](../../../onboarding-work/frontend-ledger.md#L1151): `- L8: export function HessaMark({ className }: { className?: string }) {`
- [Line 1153](../../../onboarding-work/frontend-ledger.md#L1153): `- L42: export function HessaWordmark({`
- [Line 1158](../../../onboarding-work/frontend-ledger.md#L1158): `- L9: export function DataTable({ className, children }: { className?: string; children: ReactNode }) {`
- [Line 1159](../../../onboarding-work/frontend-ledger.md#L1159): `- L19: export function DataTableHead({ children }: { children: ReactNode }) {`
- [Line 1160](../../../onboarding-work/frontend-ledger.md#L1160): `- L27: export function DataTableColumn({ children, hidden = false }: { children: ReactNode; hidden?: boolean }) {`
- [Line 1161](../../../onboarding-work/frontend-ledger.md#L1161): `- L35: export function DataTableBody({ children }: { children: ReactNode }) {`
- [Line 1162](../../../onboarding-work/frontend-ledger.md#L1162): `- L44: export function DataTableRow({ muted = false, children }: { muted?: boolean; children: ReactNode }) {`
- [Line 1163](../../../onboarding-work/frontend-ledger.md#L1163): `- L56: export function DataTableCell({`
- [Line 1168](../../../onboarding-work/frontend-ledger.md#L1168): `- L9: export function EmptyState({`
- [Line 1173](../../../onboarding-work/frontend-ledger.md#L1173): `- L4: export function FilterBar({ label, children }: { label: string; children: ReactNode }) {`
- [Line 1174](../../../onboarding-work/frontend-ledger.md#L1174): `- L17: export function FilterToggle({ children }: { children: ReactNode }) {`
- [Line 1185](../../../onboarding-work/frontend-ledger.md#L1185): `- L5: export function LoadMoreButton({ onClick, pending }: { onClick: () => void; pending?: boolean }) {`
- [Line 1191](../../../onboarding-work/frontend-ledger.md#L1191): `- L7: export function PageHeader({`
- [Line 1196](../../../onboarding-work/frontend-ledger.md#L1196): `- L13: export function ParentLookupField({`
- [Line 1207](../../../onboarding-work/frontend-ledger.md#L1207): `- L9: export function PhoneInput({`
- [Line 1219](../../../onboarding-work/frontend-ledger.md#L1219): `- L8: export function PhoneText({ value, asLink = false }: { value: string; asLink?: boolean }) {`
- [Line 1234](../../../onboarding-work/frontend-ledger.md#L1234): `- L14: export function SearchInput({`
- [Line 1245](../../../onboarding-work/frontend-ledger.md#L1245): `- L7: export function StatTile({ icon: Icon, label, value }: { icon: LucideIcon; label: string; value: number }) {`
- [Line 1257](../../../onboarding-work/frontend-ledger.md#L1257): `- L9: export function StatusBadge({ status }: { status: "active" | "archived" }) {`
- [Line 1264](../../../onboarding-work/frontend-ledger.md#L1264): `- L17: export function useParentLookup() {`
- [Line 1272](../../../onboarding-work/frontend-ledger.md#L1272): `- L7: function AlertDialog({ ...props }: AlertDialogPrimitive.Root.Props) {`
- [Line 1273](../../../onboarding-work/frontend-ledger.md#L1273): `- L11: function AlertDialogTrigger({ ...props }: AlertDialogPrimitive.Trigger.Props) {`
- [Line 1274](../../../onboarding-work/frontend-ledger.md#L1274): `- L17: function AlertDialogPortal({ ...props }: AlertDialogPrimitive.Portal.Props) {`
- [Line 1275](../../../onboarding-work/frontend-ledger.md#L1275): `- L23: function AlertDialogOverlay({`
- [Line 1276](../../../onboarding-work/frontend-ledger.md#L1276): `- L39: function AlertDialogContent({`
- [Line 1277](../../../onboarding-work/frontend-ledger.md#L1277): `- L62: function AlertDialogHeader({`
- [Line 1278](../../../onboarding-work/frontend-ledger.md#L1278): `- L78: function AlertDialogFooter({`
- [Line 1279](../../../onboarding-work/frontend-ledger.md#L1279): `- L94: function AlertDialogMedia({`
- [Line 1280](../../../onboarding-work/frontend-ledger.md#L1280): `- L110: function AlertDialogTitle({`
- [Line 1281](../../../onboarding-work/frontend-ledger.md#L1281): `- L126: function AlertDialogDescription({`
- [Line 1282](../../../onboarding-work/frontend-ledger.md#L1282): `- L142: function AlertDialogAction({`
- [Line 1283](../../../onboarding-work/frontend-ledger.md#L1283): `- L155: function AlertDialogCancel({`
- [Line 1290](../../../onboarding-work/frontend-ledger.md#L1290): `- L21: function Alert({`
- [Line 1291](../../../onboarding-work/frontend-ledger.md#L1291): `- L36: function AlertTitle({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1292](../../../onboarding-work/frontend-ledger.md#L1292): `- L49: function AlertDescription({`
- [Line 1293](../../../onboarding-work/frontend-ledger.md#L1293): `- L65: function AlertAction({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1304](../../../onboarding-work/frontend-ledger.md#L1304): `- L7: function Badge({`
- [Line 1316](../../../onboarding-work/frontend-ledger.md#L1316): `- L6: function Button({`
- [Line 1322](../../../onboarding-work/frontend-ledger.md#L1322): `- L4: function Card({`
- [Line 1323](../../../onboarding-work/frontend-ledger.md#L1323): `- L22: function CardHeader({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1324](../../../onboarding-work/frontend-ledger.md#L1324): `- L35: function CardTitle({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1325](../../../onboarding-work/frontend-ledger.md#L1325): `- L48: function CardDescription({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1326](../../../onboarding-work/frontend-ledger.md#L1326): `- L58: function CardAction({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1327](../../../onboarding-work/frontend-ledger.md#L1327): `- L71: function CardContent({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1328](../../../onboarding-work/frontend-ledger.md#L1328): `- L81: function CardFooter({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1334](../../../onboarding-work/frontend-ledger.md#L1334): `- L9: function Dialog({ ...props }: DialogPrimitive.Root.Props) {`
- [Line 1335](../../../onboarding-work/frontend-ledger.md#L1335): `- L13: function DialogTrigger({ ...props }: DialogPrimitive.Trigger.Props) {`
- [Line 1336](../../../onboarding-work/frontend-ledger.md#L1336): `- L17: function DialogPortal({ ...props }: DialogPrimitive.Portal.Props) {`
- [Line 1337](../../../onboarding-work/frontend-ledger.md#L1337): `- L21: function DialogClose({ ...props }: DialogPrimitive.Close.Props) {`
- [Line 1338](../../../onboarding-work/frontend-ledger.md#L1338): `- L25: function DialogOverlay({`
- [Line 1339](../../../onboarding-work/frontend-ledger.md#L1339): `- L41: function DialogContent({`
- [Line 1341](../../../onboarding-work/frontend-ledger.md#L1341): `- L84: function DialogHeader({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1342](../../../onboarding-work/frontend-ledger.md#L1342): `- L94: function DialogFooter({`
- [Line 1344](../../../onboarding-work/frontend-ledger.md#L1344): `- L123: function DialogTitle({ className, ...props }: DialogPrimitive.Title.Props) {`
- [Line 1345](../../../onboarding-work/frontend-ledger.md#L1345): `- L136: function DialogDescription({`
- [Line 1351](../../../onboarding-work/frontend-ledger.md#L1351): `- L8: function DropdownMenu({ ...props }: MenuPrimitive.Root.Props) {`
- [Line 1352](../../../onboarding-work/frontend-ledger.md#L1352): `- L12: function DropdownMenuPortal({ ...props }: MenuPrimitive.Portal.Props) {`
- [Line 1353](../../../onboarding-work/frontend-ledger.md#L1353): `- L16: function DropdownMenuTrigger({ ...props }: MenuPrimitive.Trigger.Props) {`
- [Line 1354](../../../onboarding-work/frontend-ledger.md#L1354): `- L20: function DropdownMenuContent({`
- [Line 1355](../../../onboarding-work/frontend-ledger.md#L1355): `- L51: function DropdownMenuGroup({ ...props }: MenuPrimitive.Group.Props) {`
- [Line 1356](../../../onboarding-work/frontend-ledger.md#L1356): `- L55: function DropdownMenuLabel({`
- [Line 1357](../../../onboarding-work/frontend-ledger.md#L1357): `- L75: function DropdownMenuItem({`
- [Line 1358](../../../onboarding-work/frontend-ledger.md#L1358): `- L98: function DropdownMenuSub({ ...props }: MenuPrimitive.SubmenuRoot.Props) {`
- [Line 1359](../../../onboarding-work/frontend-ledger.md#L1359): `- L102: function DropdownMenuSubTrigger({`
- [Line 1360](../../../onboarding-work/frontend-ledger.md#L1360): `- L126: function DropdownMenuSubContent({`
- [Line 1361](../../../onboarding-work/frontend-ledger.md#L1361): `- L147: function DropdownMenuCheckboxItem({`
- [Line 1362](../../../onboarding-work/frontend-ledger.md#L1362): `- L181: function DropdownMenuRadioGroup({ ...props }: MenuPrimitive.RadioGroup.Props) {`
- [Line 1363](../../../onboarding-work/frontend-ledger.md#L1363): `- L190: function DropdownMenuRadioItem({`
- [Line 1364](../../../onboarding-work/frontend-ledger.md#L1364): `- L222: function DropdownMenuSeparator({`
- [Line 1365](../../../onboarding-work/frontend-ledger.md#L1365): `- L235: function DropdownMenuShortcut({`
- [Line 1371](../../../onboarding-work/frontend-ledger.md#L1371): `- L5: function Input({ className, type, ...props }: React.ComponentProps<"input">) {`
- [Line 1377](../../../onboarding-work/frontend-ledger.md#L1377): `- L4: function Label({ className, ...props }: React.ComponentProps<"label">) {`
- [Line 1383](../../../onboarding-work/frontend-ledger.md#L1383): `- L5: function RadioGroup({ className, ...props }: RadioGroupPrimitive.Props) {`
- [Line 1384](../../../onboarding-work/frontend-ledger.md#L1384): `- L9: function RadioGroupItem({ className, ...props }: RadioPrimitive.Root.Props) {`
- [Line 1391](../../../onboarding-work/frontend-ledger.md#L1391): `- L8: function SelectGroup({ className, ...props }: SelectPrimitive.Group.Props) {`
- [Line 1392](../../../onboarding-work/frontend-ledger.md#L1392): `- L18: function SelectValue({ className, ...props }: SelectPrimitive.Value.Props) {`
- [Line 1393](../../../onboarding-work/frontend-ledger.md#L1393): `- L28: function SelectTrigger({`
- [Line 1394](../../../onboarding-work/frontend-ledger.md#L1394): `- L56: function SelectContent({`
- [Line 1395](../../../onboarding-work/frontend-ledger.md#L1395): `- L95: function SelectLabel({`
- [Line 1396](../../../onboarding-work/frontend-ledger.md#L1396): `- L108: function SelectItem({`
- [Line 1397](../../../onboarding-work/frontend-ledger.md#L1397): `- L136: function SelectSeparator({`
- [Line 1398](../../../onboarding-work/frontend-ledger.md#L1398): `- L149: function SelectScrollUpButton({`
- [Line 1399](../../../onboarding-work/frontend-ledger.md#L1399): `- L168: function SelectScrollDownButton({`
- [Line 1405](../../../onboarding-work/frontend-ledger.md#L1405): `- L3: function Skeleton({ className, ...props }: React.ComponentProps<"div">) {`
- [Line 1419](../../../onboarding-work/frontend-ledger.md#L1419): `- L4: function Switch({`
- [Line 1425](../../../onboarding-work/frontend-ledger.md#L1425): `- L4: function Tabs({ className, ...props }: TabsPrimitive.Root.Props) {`
- [Line 1426](../../../onboarding-work/frontend-ledger.md#L1426): `- L14: function TabsList({ className, ...props }: TabsPrimitive.List.Props) {`
- [Line 1427](../../../onboarding-work/frontend-ledger.md#L1427): `- L27: function TabsTrigger({ className, ...props }: TabsPrimitive.Tab.Props) {`
- [Line 1428](../../../onboarding-work/frontend-ledger.md#L1428): `- L40: function TabsContent({ className, ...props }: TabsPrimitive.Panel.Props) {`
- [Line 1434](../../../onboarding-work/frontend-ledger.md#L1434): `- L5: function TooltipProvider({ delay = 0, ...props }: TooltipPrimitive.Provider.Props) {`
- [Line 1435](../../../onboarding-work/frontend-ledger.md#L1435): `- L9: function Tooltip({ ...props }: TooltipPrimitive.Root.Props) {`
- [Line 1436](../../../onboarding-work/frontend-ledger.md#L1436): `- L13: function TooltipTrigger({ ...props }: TooltipPrimitive.Trigger.Props) {`
- [Line 1437](../../../onboarding-work/frontend-ledger.md#L1437): `- L17: function TooltipContent({`
- [Line 1447](../../../onboarding-work/frontend-ledger.md#L1447): `- L35: export function formatAuditValue(`
- [Line 1452](../../../onboarding-work/frontend-ledger.md#L1452): `- L15: export interface AuditFilters {`
- [Line 1453](../../../onboarding-work/frontend-ledger.md#L1453): `- L22: export function AuditLog({ filters }: { filters: AuditFilters }) {`
- [Line 1458](../../../onboarding-work/frontend-ledger.md#L1458): `- L109: function AuditRow({ item }: { item: AuditEntryDto }) {`
- [Line 1486](../../../onboarding-work/frontend-ledger.md#L1486): `- L21: export function QuickActions() {`
- [Line 1491](../../../onboarding-work/frontend-ledger.md#L1491): `- L53: export function GroupsOverview() {`
- [Line 1499](../../../onboarding-work/frontend-ledger.md#L1499): `- L30: export function EnrollInGroupDialog({`
- [Line 1509](../../../onboarding-work/frontend-ledger.md#L1509): `- L21: export function enrollmentMessageFor(error: ApiError): string {`
- [Line 1514](../../../onboarding-work/frontend-ledger.md#L1514): `- L12: export function EnrollmentRowMenu({`
- [Line 1520](../../../onboarding-work/frontend-ledger.md#L1520): `- L3: export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- [Line 1523](../../../onboarding-work/frontend-ledger.md#L1523): `- L30: export function group(overrides: Record<string, unknown> = {}) {`
- [Line 1525](../../../onboarding-work/frontend-ledger.md#L1525): `- L46: export function rosterEntry(overrides: Record<string, unknown> = {}) {`
- [Line 1527](../../../onboarding-work/frontend-ledger.md#L1527): `- L57: export function roster(items = [rosterEntry(), rosterEntry({ enrollmentId: "e-2", studentId: "st-2", studentName: "Sara Ali", version: 4 })], overrides: Record<string, unknown> = {}) {`
- [Line 1528](../../../onboarding-work/frontend-ledger.md#L1528): `- L61: export function studentListItem(overrides: Record<string, unknown> = {}) {`
- [Line 1546](../../../onboarding-work/frontend-ledger.md#L1546): `- L25: export function EnrollStudentDialog({`
- [Line 1553](../../../onboarding-work/frontend-ledger.md#L1553): `- L11: export function GroupChoiceList({`
- [Line 1574](../../../onboarding-work/frontend-ledger.md#L1574): `- L27: export function GroupRosterTab({ group }: { group: GroupDto }) {`
- [Line 1586](../../../onboarding-work/frontend-ledger.md#L1586): `- L167: function EnrollButton({`
- [Line 1592](../../../onboarding-work/frontend-ledger.md#L1592): `- L19: export function RosterTable({`
- [Line 1607](../../../onboarding-work/frontend-ledger.md#L1607): `- L15: export function SeatMeter({`
- [Line 1617](../../../onboarding-work/frontend-ledger.md#L1617): `- L23: function enrollment(overrides: Record<string, unknown> = {}) {`
- [Line 1634](../../../onboarding-work/frontend-ledger.md#L1634): `- L23: export function StudentEnrollmentsCard({`
- [Line 1647](../../../onboarding-work/frontend-ledger.md#L1647): `- L21: export function StudentPicker({`
- [Line 1673](../../../onboarding-work/frontend-ledger.md#L1673): `- L30: export function TransferDialog({`
- [Line 1677](../../../onboarding-work/frontend-ledger.md#L1677): `- L111: function TransferForm({`
- [Line 1687](../../../onboarding-work/frontend-ledger.md#L1687): `- L23: export function useEnrollmentMutations() {`
- [Line 1705](../../../onboarding-work/frontend-ledger.md#L1705): `- L16: export interface EnrollmentTarget {`
- [Line 1707](../../../onboarding-work/frontend-ledger.md#L1707): `- L30: export function WithdrawEnrollmentDialog({`
- [Line 1718](../../../onboarding-work/frontend-ledger.md#L1718): `- L45: export function CreateGroupDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`
- [Line 1731](../../../onboarding-work/frontend-ledger.md#L1731): `- L55: function useGroupMutationOutcome(group: GroupDto, onOpenChange: (open: boolean) => void) {`
- [Line 1735](../../../onboarding-work/frontend-ledger.md#L1735): `- L81: function FieldMessage({ id, error, t }: { id: string; error: FieldError | undefined; t: (key: string) => string }) {`
- [Line 1736](../../../onboarding-work/frontend-ledger.md#L1736): `- L93: function GroupDialogFrame({`
- [Line 1738](../../../onboarding-work/frontend-ledger.md#L1738): `- L157: export function RenameGroupDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- [Line 1743](../../../onboarding-work/frontend-ledger.md#L1743): `- L211: export function ChangeGroupCapacityDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- [Line 1749](../../../onboarding-work/frontend-ledger.md#L1749): `- L280: export function AssignGroupTeacherDialog({ group, open, onOpenChange }: GroupDialogProps) {`
- [Line 1773](../../../onboarding-work/frontend-ledger.md#L1773): `- L32: export function GroupsList({`
- [Line 1784](../../../onboarding-work/frontend-ledger.md#L1784): `- L18: export function showGroupError(error: ApiError): void {`
- [Line 1796](../../../onboarding-work/frontend-ledger.md#L1796): `- L5: export function LanguageSwitcher() {`
- [Line 1803](../../../onboarding-work/frontend-ledger.md#L1803): `- L36: export function ParentEditDialog({`
- [Line 1821](../../../onboarding-work/frontend-ledger.md#L1821): `- L13: async function choose(dialog: HTMLElement, label: string, option: string) {`
- [Line 1822](../../../onboarding-work/frontend-ledger.md#L1822): `- L20: async function openDialog() {`
- [Line 1823](../../../onboarding-work/frontend-ledger.md#L1823): `- L27: async function fillSaturdayEvening(dialog: HTMLElement) {`
- [Line 1838](../../../onboarding-work/frontend-ledger.md#L1838): `- L56: export function AddScheduleRuleDialog({`
- [Line 1840](../../../onboarding-work/frontend-ledger.md#L1840): `- L102: function AddScheduleRuleForm({`
- [Line 1844](../../../onboarding-work/frontend-ledger.md#L1844): `- L384: function FieldMessage({ id, error }: { id: string; error: FieldError | undefined }) {`
- [Line 1850](../../../onboarding-work/frontend-ledger.md#L1850): `- L24: export function CancelSessionDialog({`
- [Line 1859](../../../onboarding-work/frontend-ledger.md#L1859): `- L7: function hourMinute(instant: string, locale: string): { hour: string; minute: string; dayPeriod: string } {`
- [Line 1872](../../../onboarding-work/frontend-ledger.md#L1872): `- L14: export function formatTime(instant: string, timeZoneId: string, locale: string): string {`
- [Line 1873](../../../onboarding-work/frontend-ledger.md#L1873): `- L19: export function formatTimeRange(startsAt: string, endsAt: string, timeZoneId: string, locale: string): string {`
- [Line 1874](../../../onboarding-work/frontend-ledger.md#L1874): `- L24: export function todayIn(timeZoneId: string, now: Date = new Date()): string {`
- [Line 1875](../../../onboarding-work/frontend-ledger.md#L1875): `- L36: export function isLocalTime(value: string): boolean {`
- [Line 1877](../../../onboarding-work/frontend-ledger.md#L1877): `- L41: export function minutesOf(localTime: string): number {`
- [Line 1878](../../../onboarding-work/frontend-ledger.md#L1878): `- L51: export function addMinutes(localTime: string, minutes: number): string {`
- [Line 1879](../../../onboarding-work/frontend-ledger.md#L1879): `- L60: export function formatLocalTime(localTime: string, locale: string): string {`
- [Line 1880](../../../onboarding-work/frontend-ledger.md#L1880): `- L67: export function formatLocalTimeRange(start: string, end: string, locale: string): string {`
- [Line 1885](../../../onboarding-work/frontend-ledger.md#L1885): `- L27: export function EndScheduleRuleDialog({`
- [Line 1887](../../../onboarding-work/frontend-ledger.md#L1887): `- L66: function EndScheduleRuleForm({`
- [Line 1896](../../../onboarding-work/frontend-ledger.md#L1896): `- L10: async function openDialog() {`
- [Line 1910](../../../onboarding-work/frontend-ledger.md#L1910): `- L33: export function ExtendScheduleDialog({`
- [Line 1912](../../../onboarding-work/frontend-ledger.md#L1912): `- L71: function ExtendScheduleForm({ today, onDone }: { today: string; onDone: () => void }) {`
- [Line 1934](../../../onboarding-work/frontend-ledger.md#L1934): `- L28: export function GroupScheduleTab({ group }: { group: GroupDto }) {`
- [Line 1945](../../../onboarding-work/frontend-ledger.md#L1945): `- L144: function AddRuleButton({ archived, onClick }: { archived: boolean; onClick: () => void }) {`
- [Line 1952](../../../onboarding-work/frontend-ledger.md#L1952): `- L24: export function scheduleMessageFor(error: ApiError): string {`
- [Line 1953](../../../onboarding-work/frontend-ledger.md#L1953): `- L30: export function isDoubleBooking(error: ApiError): boolean {`
- [Line 1959](../../../onboarding-work/frontend-ledger.md#L1959): `- L12: export function ruleState(rule: ScheduleRuleDto, today: string | null): RuleState {`
- [Line 1960](../../../onboarding-work/frontend-ledger.md#L1960): `- L25: export function useRuleTimeRange() {`
- [Line 1962](../../../onboarding-work/frontend-ledger.md#L1962): `- L32: export function sortRules(rules: ScheduleRuleDto[]): ScheduleRuleDto[] {`
- [Line 1967](../../../onboarding-work/frontend-ledger.md#L1967): `- L20: export function ScheduleRulesTable({`
- [Line 1982](../../../onboarding-work/frontend-ledger.md#L1982): `- L48: export function toLocalTime(hour: number, minute: number, period: "am" | "pm"): string {`
- [Line 1983](../../../onboarding-work/frontend-ledger.md#L1983): `- L54: export function toAddScheduleRuleRequest(values: ScheduleRuleFormValues) {`
- [Line 1989](../../../onboarding-work/frontend-ledger.md#L1989): `- L7: export function me(role: "owner" | "secretary" | "teacher", permissions: string[]) {`
- [Line 1992](../../../onboarding-work/frontend-ledger.md#L1992): `- L36: export function group(overrides: Record<string, unknown> = {}) {`
- [Line 1994](../../../onboarding-work/frontend-ledger.md#L1994): `- L52: export function rule(overrides: Record<string, unknown> = {}) {`
- [Line 1996](../../../onboarding-work/frontend-ledger.md#L1996): `- L66: export function session(overrides: Record<string, unknown> = {}) {`
- [Line 1998](../../../onboarding-work/frontend-ledger.md#L1998): `- L85: export function timetable(weekStart: string, sessions: ReturnType<typeof session>[] = [], timeZoneId = "Africa/Cairo") {`
- [Line 2006](../../../onboarding-work/frontend-ledger.md#L2006): `- L17: export function SessionCard({`
- [Line 2012](../../../onboarding-work/frontend-ledger.md#L2012): `- L14: export function TimetableGrid({`
- [Line 2035](../../../onboarding-work/frontend-ledger.md#L2035): `- L28: export interface TimetableSearchPatch {`
- [Line 2036](../../../onboarding-work/frontend-ledger.md#L2036): `- L41: export function TimetablePage({`
- [Line 2044](../../../onboarding-work/frontend-ledger.md#L2044): `- L130: function TimetableWeek({`
- [Line 2051](../../../onboarding-work/frontend-ledger.md#L2051): `- L227: function GroupFilterSelect({ value, onChange }: { value: string | null; onChange: (value: string | null) => void }) {`
- [Line 2058](../../../onboarding-work/frontend-ledger.md#L2058): `- L4: export function useDurationLabel() {`
- [Line 2065](../../../onboarding-work/frontend-ledger.md#L2065): `- L24: export function useScheduleMutations() {`
- [Line 2099](../../../onboarding-work/frontend-ledger.md#L2099): `- L21: export function isIsoDate(value: unknown): value is string {`
- [Line 2100](../../../onboarding-work/frontend-ledger.md#L2100): `- L36: function toDayNumber(isoDate: string): number {`
- [Line 2101](../../../onboarding-work/frontend-ledger.md#L2101): `- L45: function fromDayNumber(dayNumber: number): string {`
- [Line 2102](../../../onboarding-work/frontend-ledger.md#L2102): `- L53: export function addDays(isoDate: string, days: number): string {`
- [Line 2103](../../../onboarding-work/frontend-ledger.md#L2103): `- L58: export function dayOfWeekNumber(isoDate: string): number {`
- [Line 2104](../../../onboarding-work/frontend-ledger.md#L2104): `- L63: export function weekdayOf(isoDate: string): Weekday {`
- [Line 2105](../../../onboarding-work/frontend-ledger.md#L2105): `- L68: export function startOfWeek(isoDate: string): string {`
- [Line 2106](../../../onboarding-work/frontend-ledger.md#L2106): `- L73: export function weekDates(weekStart: string): string[] {`
- [Line 2107](../../../onboarding-work/frontend-ledger.md#L2107): `- L78: export function daysBetween(from: string, to: string): number {`
- [Line 2108](../../../onboarding-work/frontend-ledger.md#L2108): `- L86: export function formatCalendarDate(isoDate: string, locale: string, options: Intl.DateTimeFormatOptions = { dateStyle: "medium" }): string {`
- [Line 2109](../../../onboarding-work/frontend-ledger.md#L2109): `- L91: export function formatCalendarRange(from: string, to: string, locale: string): string {`
- [Line 2114](../../../onboarding-work/frontend-ledger.md#L2114): `- L10: export function WeekNavigator({`
- [Line 2120](../../../onboarding-work/frontend-ledger.md#L2120): `- L6: export function Can({ permission, children }: { permission: Permission; children: ReactNode }) {`
- [Line 2138](../../../onboarding-work/frontend-ledger.md#L2138): `- L6: export function refetchMeOnPermissionDenied(error: ApiError, queryClient: QueryClient): void {`
- [Line 2144](../../../onboarding-work/frontend-ledger.md#L2144): `- L15: export function RouteGuard({ permission, children }: { permission: Permission; children: ReactNode }) {`
- [Line 2151](../../../onboarding-work/frontend-ledger.md#L2151): `- L5: export function useCan(permission: Permission): boolean {`
- [Line 2157](../../../onboarding-work/frontend-ledger.md#L2157): `- L8: export function useCentreTimeZone(): string | null {`
- [Line 2174](../../../onboarding-work/frontend-ledger.md#L2174): `- L5: export function useSession() {`
- [Line 2180](../../../onboarding-work/frontend-ledger.md#L2180): `- L7: export function useSignOut(): () => Promise<void> {`
- [Line 2194](../../../onboarding-work/frontend-ledger.md#L2194): `- L31: export function AppShell({ me, children }: { me: MeDto; children: ReactNode }) {`
- [Line 2199](../../../onboarding-work/frontend-ledger.md#L2199): `- L101: function SidebarBrand({ onNavigate }: { onNavigate?: () => void }) {`
- [Line 2201](../../../onboarding-work/frontend-ledger.md#L2201): `- L116: function SidebarNav({ onNavigate }: { onNavigate?: () => void }) {`
- [Line 2211](../../../onboarding-work/frontend-ledger.md#L2211): `- L180: function UserMenu({ me }: { me: MeDto }) {`
- [Line 2223](../../../onboarding-work/frontend-ledger.md#L2223): `- L27: export function ChangeRoleDialog({`
- [Line 2235](../../../onboarding-work/frontend-ledger.md#L2235): `- L34: export function StaffFormDialog({ open, onOpenChange }: { open: boolean; onOpenChange: (open: boolean) => void }) {`
- [Line 2242](../../../onboarding-work/frontend-ledger.md#L2242): `- L225: function RoleField({`
- [Line 2244](../../../onboarding-work/frontend-ledger.md#L2244): `- L254: function LocaleField({`
- [Line 2250](../../../onboarding-work/frontend-ledger.md#L2250): `- L47: export function StaffList({ onChangeRole }: { onChangeRole: (member: StaffMemberDto) => void }) {`
- [Line 2272](../../../onboarding-work/frontend-ledger.md#L2272): `- L11: export async function fetchReadiness(): Promise<ReadinessStatus> {`
- [Line 2284](../../../onboarding-work/frontend-ledger.md#L2284): `- L8: export function DemoBanner() {`
- [Line 2302](../../../onboarding-work/frontend-ledger.md#L2302): `- L8: export function StatusCard() {`
- [Line 2317](../../../onboarding-work/frontend-ledger.md#L2317): `- L13: export function SystemInfoCard() {`
- [Line 2325](../../../onboarding-work/frontend-ledger.md#L2325): `- L5: export function useReadiness() {`
- [Line 2333](../../../onboarding-work/frontend-ledger.md#L2333): `- L44: export function AddParentDialog({`
- [Line 2356](../../../onboarding-work/frontend-ledger.md#L2356): `- L47: export function ParentsCard({ student, canManage }: { student: StudentDetailDto; canManage: boolean }) {`
- [Line 2373](../../../onboarding-work/frontend-ledger.md#L2373): `- L36: export function RegisterStudentForm() {`
- [Line 2393](../../../onboarding-work/frontend-ledger.md#L2393): `- L37: export function StudentEditDialog({`
- [Line 2406](../../../onboarding-work/frontend-ledger.md#L2406): `- L33: export function StudentsList({ search, includeArchived }: { search: string; includeArchived: boolean }) {`
- [Line 2412](../../../onboarding-work/frontend-ledger.md#L2412): `- L140: function StudentRow({ student }: { student: StudentListItemDto }) {`
- [Line 2426](../../../onboarding-work/frontend-ledger.md#L2426): `- L34: export function SubjectFormDialog({`
- [Line 2457](../../../onboarding-work/frontend-ledger.md#L2457): `- L13: export function SubjectSelect({`
- [Line 2464](../../../onboarding-work/frontend-ledger.md#L2464): `- L49: export function SubjectsList({`
- [Line 2479](../../../onboarding-work/frontend-ledger.md#L2479): `- L9: export function eligibleStaffForTeacher(`
- [Line 2485](../../../onboarding-work/frontend-ledger.md#L2485): `- L45: export function TeacherAccountDialog({`
- [Line 2500](../../../onboarding-work/frontend-ledger.md#L2500): `- L39: export function TeacherFormDialog({`
- [Line 2519](../../../onboarding-work/frontend-ledger.md#L2519): `- L14: export function TeacherSelect({`
- [Line 2526](../../../onboarding-work/frontend-ledger.md#L2526): `- L60: export function TeachersList({`
- [Line 2544](../../../onboarding-work/frontend-ledger.md#L2544): `- L38: function isSupportedLanguage(value: unknown): value is SupportedLanguage {`
- [Line 2545](../../../onboarding-work/frontend-ledger.md#L2545): `- L43: function detectInitialLanguage(): SupportedLanguage {`
- [Line 2546](../../../onboarding-work/frontend-ledger.md#L2546): `- L58: function applyDocumentDirection(language: string): void {`
- [Line 2713](../../../onboarding-work/frontend-ledger.md#L2713): `- L8: export function isolateLtr(value: string): string {`
- [Line 2718](../../../onboarding-work/frontend-ledger.md#L2718): `- L2: export function initialsOf(name: string): string {`
- [Line 2734](../../../onboarding-work/frontend-ledger.md#L2734): `- L7: export interface RouterContext {`
- [Line 2736](../../../onboarding-work/frontend-ledger.md#L2736): `- L15: function RootLayout() {`
- [Line 2744](../../../onboarding-work/frontend-ledger.md#L2744): `- L24: function entry(overrides: Record<string, unknown> = {}) {`
- [Line 2745](../../../onboarding-work/frontend-ledger.md#L2745): `- L40: function problem(status: number, code: string, detail: string) {`
- [Line 2763](../../../onboarding-work/frontend-ledger.md#L2763): `- L15: export interface AuditSearch {`
- [Line 2766](../../../onboarding-work/frontend-ledger.md#L2766): `- L41: function AuditPage() {`
- [Line 2768](../../../onboarding-work/frontend-ledger.md#L2768): `- L49: function AuditPageContent() {`
- [Line 2779](../../../onboarding-work/frontend-ledger.md#L2779): `- L12: function me(role: "owner" | "teacher", permissions: string[]) {`
- [Line 2782](../../../onboarding-work/frontend-ledger.md#L2782): `- L38: function group(overrides: Record<string, unknown> = {}) {`
- [Line 2787](../../../onboarding-work/frontend-ledger.md#L2787): `- L59: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 2810](../../../onboarding-work/frontend-ledger.md#L2810): `- L58: export interface GroupDetailSearch {`
- [Line 2811](../../../onboarding-work/frontend-ledger.md#L2811): `- L62: function isGroupTab(value: unknown): value is GroupTab {`
- [Line 2813](../../../onboarding-work/frontend-ledger.md#L2813): `- L73: function GroupDetailPage() {`
- [Line 2815](../../../onboarding-work/frontend-ledger.md#L2815): `- L81: function GroupDetailPageContent() {`
- [Line 2820](../../../onboarding-work/frontend-ledger.md#L2820): `- L143: function GroupNotFound() {`
- [Line 2822](../../../onboarding-work/frontend-ledger.md#L2822): `- L160: function GroupDetailContent({ group }: { group: GroupDto }) {`
- [Line 2846](../../../onboarding-work/frontend-ledger.md#L2846): `- L14: function me(role: "owner" | "teacher", permissions: string[]) {`
- [Line 2849](../../../onboarding-work/frontend-ledger.md#L2849): `- L31: function group(overrides: Record<string, unknown> = {}) {`
- [Line 2857](../../../onboarding-work/frontend-ledger.md#L2857): `- L56: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 2874](../../../onboarding-work/frontend-ledger.md#L2874): `- L18: export interface GroupsSearch {`
- [Line 2876](../../../onboarding-work/frontend-ledger.md#L2876): `- L35: function GroupsPage() {`
- [Line 2878](../../../onboarding-work/frontend-ledger.md#L2878): `- L48: function GroupsPageContent() {`
- [Line 2893](../../../onboarding-work/frontend-ledger.md#L2893): `- L12: function GroupsLayout() {`
- [Line 2900](../../../onboarding-work/frontend-ledger.md#L2900): `- L14: function DashboardPage() {`
- [Line 2911](../../../onboarding-work/frontend-ledger.md#L2911): `- L23: function parentDetail(overrides: Record<string, unknown> = {}) {`
- [Line 2913](../../../onboarding-work/frontend-ledger.md#L2913): `- L37: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 2929](../../../onboarding-work/frontend-ledger.md#L2929): `- L33: function ParentDetailPage() {`
- [Line 2931](../../../onboarding-work/frontend-ledger.md#L2931): `- L41: function ParentDetailPageContent() {`
- [Line 2936](../../../onboarding-work/frontend-ledger.md#L2936): `- L95: function ParentNotFound() {`
- [Line 2938](../../../onboarding-work/frontend-ledger.md#L2938): `- L108: function ParentDetailContent({ parent }: { parent: ParentDetailDto }) {`
- [Line 2942](../../../onboarding-work/frontend-ledger.md#L2942): `- L166: function ChildRow({ child }: { child: ParentChildDto }) {`
- [Line 2951](../../../onboarding-work/frontend-ledger.md#L2951): `- L24: function settings(overrides: Record<string, unknown> = {}) {`
- [Line 2953](../../../onboarding-work/frontend-ledger.md#L2953): `- L35: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 2973](../../../onboarding-work/frontend-ledger.md#L2973): `- L37: function SettingsPage() {`
- [Line 2975](../../../onboarding-work/frontend-ledger.md#L2975): `- L45: function SettingsPageContent() {`
- [Line 2985](../../../onboarding-work/frontend-ledger.md#L2985): `- L200: function LocaleSelect({`
- [Line 2994](../../../onboarding-work/frontend-ledger.md#L2994): `- L24: function member(overrides: Record<string, unknown> = {}) {`
- [Line 2996](../../../onboarding-work/frontend-ledger.md#L2996): `- L38: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3020](../../../onboarding-work/frontend-ledger.md#L3020): `- L19: function StaffPage() {`
- [Line 3022](../../../onboarding-work/frontend-ledger.md#L3022): `- L27: function StaffPageContent() {`
- [Line 3033](../../../onboarding-work/frontend-ledger.md#L3033): `- L23: function parent(overrides: Record<string, unknown> = {}) {`
- [Line 3034](../../../onboarding-work/frontend-ledger.md#L3034): `- L27: function studentDetail(overrides: Record<string, unknown> = {}) {`
- [Line 3036](../../../onboarding-work/frontend-ledger.md#L3036): `- L40: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3037](../../../onboarding-work/frontend-ledger.md#L3037): `- L48: function secondRowMenuButton(): HTMLElement {`
- [Line 3062](../../../onboarding-work/frontend-ledger.md#L3062): `- L50: function StudentDetailPage() {`
- [Line 3064](../../../onboarding-work/frontend-ledger.md#L3064): `- L58: function StudentDetailPageContent() {`
- [Line 3069](../../../onboarding-work/frontend-ledger.md#L3069): `- L112: function StudentNotFound() {`
- [Line 3071](../../../onboarding-work/frontend-ledger.md#L3071): `- L125: function StudentDetailContent({ student }: { student: StudentDetailDto }) {`
- [Line 3093](../../../onboarding-work/frontend-ledger.md#L3093): `- L28: function student(overrides: Record<string, unknown> = {}) {`
- [Line 3094](../../../onboarding-work/frontend-ledger.md#L3094): `- L40: function problem(status: number, code: string, detail: string) {`
- [Line 3114](../../../onboarding-work/frontend-ledger.md#L3114): `- L15: export interface StudentsSearch {`
- [Line 3116](../../../onboarding-work/frontend-ledger.md#L3116): `- L28: function StudentsPage() {`
- [Line 3118](../../../onboarding-work/frontend-ledger.md#L3118): `- L36: function StudentsPageContent() {`
- [Line 3131](../../../onboarding-work/frontend-ledger.md#L3131): `- L25: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3132](../../../onboarding-work/frontend-ledger.md#L3132): `- L32: async function fillStudentName(name: string) {`
- [Line 3133](../../../onboarding-work/frontend-ledger.md#L3133): `- L36: async function fillParentPhone(phone: string) {`
- [Line 3155](../../../onboarding-work/frontend-ledger.md#L3155): `- L11: function RegisterStudentPage() {`
- [Line 3157](../../../onboarding-work/frontend-ledger.md#L3157): `- L19: function RegisterStudentPageContent() {`
- [Line 3165](../../../onboarding-work/frontend-ledger.md#L3165): `- L15: function StudentsLayout() {`
- [Line 3173](../../../onboarding-work/frontend-ledger.md#L3173): `- L22: function subject(overrides: Record<string, unknown> = {}) {`
- [Line 3175](../../../onboarding-work/frontend-ledger.md#L3175): `- L26: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3199](../../../onboarding-work/frontend-ledger.md#L3199): `- L15: export interface SubjectsSearch {`
- [Line 3201](../../../onboarding-work/frontend-ledger.md#L3201): `- L26: function SubjectsPage() {`
- [Line 3213](../../../onboarding-work/frontend-ledger.md#L3213): `- L13: function me(role: "owner" | "secretary", permissions: string[]) {`
- [Line 3216](../../../onboarding-work/frontend-ledger.md#L3216): `- L30: function teacher(overrides: Record<string, unknown> = {}) {`
- [Line 3218](../../../onboarding-work/frontend-ledger.md#L3218): `- L44: function staffMember(overrides: Record<string, unknown> = {}) {`
- [Line 3220](../../../onboarding-work/frontend-ledger.md#L3220): `- L58: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3221](../../../onboarding-work/frontend-ledger.md#L3221): `- L66: async function pickStaff(displayName: string) {`
- [Line 3222](../../../onboarding-work/frontend-ledger.md#L3222): `- L73: async function openRowMenu(name: string) {`
- [Line 3241](../../../onboarding-work/frontend-ledger.md#L3241): `- L16: export interface TeachersSearch {`
- [Line 3243](../../../onboarding-work/frontend-ledger.md#L3243): `- L27: function TeachersPage() {`
- [Line 3245](../../../onboarding-work/frontend-ledger.md#L3245): `- L35: function TeachersPageContent() {`
- [Line 3255](../../../onboarding-work/frontend-ledger.md#L3255): `- L7: export interface TimetableSearch {`
- [Line 3257](../../../onboarding-work/frontend-ledger.md#L3257): `- L24: function TimetableRoute() {`
- [Line 3259](../../../onboarding-work/frontend-ledger.md#L3259): `- L33: function TimetableRouteContent() {`
- [Line 3271](../../../onboarding-work/frontend-ledger.md#L3271): `- L15: function me(overrides: Partial<{ activeCentreId: string | null; memberships: unknown[] }> = {}) {`
- [Line 3285](../../../onboarding-work/frontend-ledger.md#L3285): `- L35: function AuthenticatedLayout() {`
- [Line 3294](../../../onboarding-work/frontend-ledger.md#L3294): `- L12: function me(overrides: Record<string, unknown> = {}) {`
- [Line 3295](../../../onboarding-work/frontend-ledger.md#L3295): `- L27: function problem(status: number, code: string, detail: string, errors?: Record<string, string[]>) {`
- [Line 3315](../../../onboarding-work/frontend-ledger.md#L3315): `- L34: function ChangePasswordPage() {`
- [Line 3328](../../../onboarding-work/frontend-ledger.md#L3328): `- L9: function problem(status: number, code: string, detail: string) {`
- [Line 3340](../../../onboarding-work/frontend-ledger.md#L3340): `- L21: export interface LoginSearch {`
- [Line 3345](../../../onboarding-work/frontend-ledger.md#L3345): `- L48: function resolveTarget(me: MeDto, redirect: string | undefined): string {`
- [Line 3346](../../../onboarding-work/frontend-ledger.md#L3346): `- L60: function LoginPage() {`
- [Line 3361](../../../onboarding-work/frontend-ledger.md#L3361): `- L14: function meWithMemberships(memberships: unknown[], activeCentreId: string | null = null) {`
- [Line 3376](../../../onboarding-work/frontend-ledger.md#L3376): `- L64: function avatarClassName(centreId: string): string {`
- [Line 3377](../../../onboarding-work/frontend-ledger.md#L3377): `- L73: function SelectCentrePage() {`
- [Line 3386](../../../onboarding-work/frontend-ledger.md#L3386): `- L150: function NoActiveCentre({ email, onSignOut }: { email: string; onSignOut: () => Promise<void> }) {`
- [Line 3392](../../../onboarding-work/frontend-ledger.md#L3392): `- L223: function CentreCard({`
- [Line 3400](../../../onboarding-work/frontend-ledger.md#L3400): `- L10: function StatusPage() {`
- [Line 3425](../../../onboarding-work/frontend-ledger.md#L3425): `- L139: export interface FileRoutesByFullPath {`
- [Line 3426](../../../onboarding-work/frontend-ledger.md#L3426): `- L160: export interface FileRoutesByTo {`
- [Line 3427](../../../onboarding-work/frontend-ledger.md#L3427): `- L179: export interface FileRoutesById {`
- [Line 3428](../../../onboarding-work/frontend-ledger.md#L3428): `- L202: export interface FileRouteTypes {`
- [Line 3429](../../../onboarding-work/frontend-ledger.md#L3429): `- L267: export interface RootRouteChildren {`
- [Line 3448](../../../onboarding-work/frontend-ledger.md#L3448): `- L15: export function problemDetails(status: number, code: string, detail: string) {`
- [Line 3460](../../../onboarding-work/frontend-ledger.md#L3460): `- L5: function newTestQueryClient(): QueryClient {`
- [Line 3461](../../../onboarding-work/frontend-ledger.md#L3461): `- L12: export function renderWithQueryClient(ui: ReactElement): RenderResult {`
- [Line 3462](../../../onboarding-work/frontend-ledger.md#L3462): `- L19: export function renderHookWithQueryClient<TResult, TProps>(`
- [Line 3467](../../../onboarding-work/frontend-ledger.md#L3467): `- L12: export function renderRouter(initialPath: string) {`
- [Line 3472](../../../onboarding-work/frontend-ledger.md#L3472): `- L11: export async function selectOption(triggerName: string, optionName: string): Promise<void> {`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```text
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

```

The excerpt stops at line 24; use the original file for the remaining 3474 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
