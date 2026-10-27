# Backend test file catalogue

Every tracked backend test source is read by the inventory pass; this catalogue records its declared test entry points. Fixture and helper files without test methods remain in the read ledger. Names describe assertions to inspect; passing results are reported separately.

### `tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs`

Task 49.7: tenant isolation for enrollment at the HTTP boundary, every case in both directions. Enrollment joins three resources (group, student, enrollment), so each of the three is the foreign one in at least one case, and mixed cases put one foreign ID inside an otherwise own request — the "everything foreign" case alone would be stopped by the group lookup and prove nothing about the student or target-group lookups. Each case answers the 404 of the first foreign thing the handler meets (group before student; enrollment before target group), and the rows of both centres are compared before and after through the superuser connection.

Declared test entry points (declarations are not execution results):

- Line 33: `public async Task Enroll_OwnGroupWithTheOtherCentresStudent_Returns404StudentNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- Line 49: `public async Task Enroll_TheOtherCentresGroupWithOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- Line 65: `public async Task Enroll_TheOtherCentresGroupWithThatCentresOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- Line 81: `public async Task Withdraw_TheOtherCentresEnrollmentWithItsRealVersion_Returns404EnrollmentNotFoundAndLeavesItActive(string attackerEmail, string victimEmail)`
- Line 98: `public async Task Transfer_TheOtherCentresEnrollmentIntoOwnGroup_Returns404EnrollmentNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)`
- Line 116: `public async Task Transfer_TheOtherCentresEnrollmentWithinThatCentre_Returns404EnrollmentNotFoundAndChangesNothing(string attackerEmail, string victimEmail)`
- Line 133: `public async Task Transfer_OwnEnrollmentIntoTheOtherCentresGroup_Returns404GroupNotFoundAndTheEnrollmentStaysActive(string attackerEmail, string victimEmail)`
- Line 152: `public async Task Roster_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)`
- Line 167: `public async Task StudentEnrollments_OfTheOtherCentresStudent_Returns404StudentNotFound(string attackerEmail, string victimEmail)`

### `tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs`

Task 49.6: the five enrollment endpoints' contract through the full HTTP pipeline — status, code, body shape and headers, never messages. Every test builds its own subjects, teachers, groups, students and enrollments through the earlier endpoints over HTTP, so each one also exercises them, and the seeded demo data never decides a result. Every error row of the Day 49 outcome table has its own case.

Declared test entry points (declarations are not execution results):

- Line 37: `public async Task Enroll_Returns201WithIdAndLocationAtTheRoster_AndTheRosterListsTheStudent()`
- Line 61: `public async Task Enroll_SameStudentAgain_Returns409AlreadyEnrolledAndWritesNoRow()`
- Line 77: `public async Task Enroll_WithAFieldBeyondStudentId_Returns400RequestMalformedAndWritesNoRow(string extraField)`
- Line 97: `public async Task Enroll_MalformedJson_Returns400RequestMalformed()`
- Line 108: `public async Task Enroll_EmptyStudentId_Returns400WithErrorsStudentId()`
- Line 121: `public async Task Enroll_UnknownGroup_Returns404GroupNotFound()`
- Line 132: `public async Task Enroll_UnknownStudent_Returns404StudentNotFound()`
- Line 144: `public async Task Enroll_IntoAFullGroup_Returns409GroupFullAndWritesNoRow()`
- Line 158: `public async Task Enroll_IntoAnArchivedGroup_Returns422GroupArchived()`
- Line 172: `public async Task Enroll_AnArchivedStudent_Returns422StudentArchived()`
- Line 186: `public async Task Enroll_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()`
- Line 199: `public async Task Enroll_AsTeacher_Returns403PermissionDenied()`
- Line 213: `public async Task Enroll_BeforeSelectingACentre_Returns403TenantNotSelected()`
- Line 224: `public async Task Roster_NoCookie_Returns401()`
- Line 237: `public async Task Roster_IsOrderedByNormalizedStudentName_HasTheGroupsFactsAndNamesOnly()`
- Line 270: `public async Task Roster_UnknownGroup_Returns404GroupNotFound()`
- Line 280: `public async Task GroupList_EnrolledCount_EqualsTheRosterLength()`
- Line 310: `public async Task StudentEnrollments_ListsActiveFirstThenEnded_WithTheFullShape()`
- Line 355: `public async Task StudentEnrollments_UnknownStudent_Returns404StudentNotFound_AndAKnownStudentWithNoneReturnsEmptyItems()`
- Line 369: `public async Task StudentDetail_StaysUnchanged_AndEmbedsNoEnrollments()`
- Line 384: `public async Task Withdraw_Returns204_ThenAgainReturns422AlreadyEnded_AndTheRowStaysAsHistory()`
- Line 411: `public async Task Withdraw_StaleVersion_Returns409ConcurrencyStaleAndTheEnrollmentStaysActive()`
- Line 425: `public async Task Withdraw_UnknownEnrollment_Returns404EnrollmentNotFound()`
- Line 437: `public async Task Withdraw_MissingVersionOrUnknownField_Returns400RequestMalformed(string json)`
- Line 447: `public async Task Transfer_ToASameSubjectGroup_Returns200WithTheNewId_EndsTheOldAsTransferredAndStartsTheNew()`
- Line 479: `public async Task Transfer_ToAGroupOfAnotherSubject_Returns422SubjectMismatchAndChangesNothing()`
- Line 496: `public async Task Transfer_ToItsOwnGroup_Returns422SameGroup()`
- Line 511: `public async Task Transfer_AnEndedEnrollment_Returns422AlreadyEnded()`
- Line 532: `public async Task Transfer_ToAnArchivedGroup_Returns422GroupArchivedAndTheStudentStaysInTheSource()`
- Line 549: `public async Task Transfer_ToAFullGroup_Returns409GroupFullAndTheStudentStaysInTheSource()`
- Line 567: `public async Task Transfer_ToAGroupTheStudentIsAlreadyIn_Returns409AlreadyEnrolled()`
- Line 585: `public async Task Transfer_StaleVersion_Returns409ConcurrencyStaleAndRollsBackBothChanges()`
- Line 600: `public async Task Transfer_UnknownEnrollment_Returns404EnrollmentNotFound_AndUnknownTarget_Returns404GroupNotFound()`
- Line 616: `public async Task Transfer_EmptyTargetGroupId_Returns400WithErrorsTargetGroupId()`
- Line 628: `public async Task Transfer_WithAStudentIdInTheBody_Returns400RequestMalformed()`

### `tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs`

Task 49.7: the ownership guard on enrollment, as black-box behaviour through teacher@both.test's real session. The roster inherits its group's rule: 200 for the own group, 404 (never 403) for another teacher's. A student enrolled in both an own and another teacher's group shows the teacher only the own enrollment. Writes are 403 even on the own group, because permission is evaluated before ownership, and they change nothing.

Declared test entry points (declarations are not execution results):

- Line 31: `public async Task LinkedTeacher_RosterOfTheOwnGroup_Returns200WithItsStudents()`
- Line 45: `public async Task LinkedTeacher_RosterOfAnotherTeachersGroup_Returns404GroupNotFound()`
- Line 56: `public async Task LinkedTeacher_StudentEnrolledInOwnAndOtherGroup_ListsOnlyTheOwnEnrollment()`
- Line 78: `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`

### `tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs`

Task 45.7: tenant isolation for groups at the HTTP boundary, every case in both directions. Ordinary probes attack the victim's group directly on all six ID routes with its real current version, so a 409 could never mask a missing guard. Reference probes put the victim centre's subject or teacher ID inside the attacker's own request: the handler must resolve it inside the attacker's centre and answer not-found, with no row written on either side.

Declared test entry points (declarations are not execution results):

- Line 29: `public async Task EveryIdRoute_OnTheOtherCentresGroup_Returns404AndLeavesTheRowUnchanged(string attackerEmail, string victimEmail)`
- Line 66: `public async Task List_ContainsNoneOfTheOtherCentresGroups(string attackerEmail, string victimEmail)`
- Line 88: `public async Task Create_WithTheOtherCentresSubject_Returns404SubjectNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- Line 105: `public async Task Create_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- Line 122: `public async Task AssignTeacher_WithTheOtherCentresTeacher_Returns404TeacherNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)`
- Line 141: `public async Task Create_WithANameUsedInTheOtherCentre_Returns201(string attackerEmail, string victimEmail)`

### `tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs`

Task 45.5: the eight group endpoints' contract through the full HTTP pipeline — status, code, body shape and headers, never messages — plus the one outcome Day 44 added to Month 2's subject archive endpoint, and the two outcomes scheduling (Day 52) added to reassigning and archiving a group (Task 53.4). Every test creates its own subjects, teachers and groups, so the seeded demo data never decides a result.

Declared test entry points (declarations are not execution results):

- Line 33: `public async Task Create_ThenGet_Returns201WithLocationThen200WithFullShape()`
- Line 67: `public async Task Create_NameTakenIgnoringCase_Returns409NameTakenWithErrorsName()`
- Line 86: `public async Task Create_CapacityOutOfRange_Returns400WithErrorsCapacityAndCreatesNoRow(int capacity)`
- Line 104: `public async Task Create_EmptyName_Returns400WithErrorsName(string name)`
- Line 119: `public async Task Create_WithOverPostedField_Returns400RequestMalformedAndCreatesNoRow(string extraField)`
- Line 142: `public async Task Create_UnknownSubject_Returns404SubjectNotFound()`
- Line 154: `public async Task Create_UnknownTeacher_Returns404TeacherNotFound()`
- Line 166: `public async Task Create_ArchivedSubject_Returns422SubjectArchived()`
- Line 179: `public async Task Create_ArchivedTeacher_Returns422TeacherArchived()`
- Line 192: `public async Task Rename_HappyPathThenStaleVersion()`
- Line 208: `public async Task Rename_ToAnotherGroupsName_Returns409NameTaken()`
- Line 228: `public async Task Put_WithSubjectIdInBody_Returns400RequestMalformedAndLeavesSubjectUnchanged(string suffix)`
- Line 251: `public async Task ChangeCapacity_HappyPathThenZeroReturns400WithErrorsCapacity()`
- Line 270: `public async Task AssignTeacher_ActiveTeacher_Returns204AndGetShowsTheNewTeacher()`
- Line 287: `public async Task AssignTeacher_ArchivedTeacher_Returns422TeacherArchived()`
- Line 303: `public async Task AssignTeacher_UnknownTeacher_Returns404TeacherNotFound()`
- Line 316: `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- Line 341: `public async Task Restore_AfterItsTeacherWasArchived_Returns422TeacherArchived()`
- Line 355: `public async Task Restore_AfterItsSubjectWasArchived_Returns422SubjectArchived()`
- Line 369: `public async Task Get_UnknownId_Returns404GroupNotFound()`
- Line 379: `public async Task List_FiltersBySubjectAndByTeacher_AndHidesArchivedUnlessAsked()`
- Line 398: `public async Task SubjectArchive_WithAnActiveGroup_Returns422HasActiveGroupsAndSubjectStaysActive()`
- Line 422: `public async Task ChangeCapacity_BelowTheEnrolledCount_Returns422CapacityBelowEnrolledAndCapacityStays()`
- Line 444: `public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndGroupStaysActive()`
- Line 470: `public async Task List_NoCookie_Returns401()`
- Line 483: `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndCreatesNoRow()`
- Line 497: `public async Task AssignTeacher_ToATeacherBusyAtOneOfTheGroupsSessions_Returns409TeacherDoubleBookedAndKeepsTeacherAndSessions()`
- Line 518: `public async Task Archive_WithAScheduleRuleInEffect_Returns422HasActiveSchedule()`

### `tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs`

Task 45.6: the ownership guard proven as black-box behaviour through real sessions. The owner arranges, over HTTP, two teachers and two groups in Nile and links one teacher record to teacher@both.test; the tests then act as that user. The unlinked teacher is covered too, because that is exactly where a default of "All" instead of "None" would hide. Reads outside the scope are 404, never 403; writes are 403 for a teacher even on their own group, because permission is evaluated before ownership.

Declared test entry points (declarations are not execution results):

- Line 29: `public async Task LinkedTeacher_List_ContainsExactlyTheOwnGroup()`
- Line 38: `public async Task LinkedTeacher_GetOwnGroup_Returns200()`
- Line 52: `public async Task LinkedTeacher_GetOtherTeachersGroup_Returns404GroupNotFound()`
- Line 63: `public async Task LinkedTeacher_FilterByOtherTeacher_ReturnsEmptyItems()`
- Line 78: `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`
- Line 105: `public async Task UnlinkedTeacher_List_IsEmpty()`
- Line 117: `public async Task UnlinkedTeacher_GetEitherGroupById_Returns404GroupNotFound()`
- Line 133: `public async Task AfterOwnerReassignsTheOtherGroup_LinkedTeachersNextListContainsBoth()`
- Line 154: `public async Task AfterOwnerRemovesTheAccountLink_LinkedTeachersNextListIsEmptyAndOwnGroupIs404()`
- Line 171: `public async Task SameAccountWithMaadiSelected_SeesNothingOfNile()`
- Line 184: `public async Task Owner_OnTheSameRoutes_SeesBothGroups()`

### `tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs`

Task 49.8, the schedule's Week 8 demo through the whole HTTP pipeline: one seat left, ten parallel requests, one success. Day 48's LastSeatConcurrencyTests proved the use case; this proves nothing between the client and the handler (a shared scope, a retry, a cached count) breaks the guarantee. Ten clients, each with its own cookies and its own antiforgery token fetched before the race (five signed in as the owner, five as the secretary), send at the same instant behind one gate. Repeated with a fresh group each time, because a race test that runs once can pass by luck. Counts are read through the superuser connection.

Declared test entry points (declarations are not execution results):

- Line 38: `public async Task TenParallelEnrollRequestsIntoTheLastSeat_ExactlyOne201AndNine409GroupFull_AcrossFiveRepetitions()`

### `tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs`

Task 39.9: owner A cannot see or touch B's parents, proven in both directions with CrossTenantProbe, using a real victim parent and its real current version so a 409 could never mask a leak. The lookup and create checks confirm phone uniqueness and visibility are per-centre, never global.

Declared test entry points (declarations are not execution results):

- Line 25: `public Task NileOwner_CannotSeeOrTouchMaadisParents() =>`
- Line 29: `public Task MaadiOwner_CannotSeeOrTouchNilesParents() =>`

### `tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs`

Task 39.9: proves the four Parent endpoints' behaviour through the full HTTP pipeline, against the Day 39 contract's outcome table.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task Create_ThenLookupWithDifferentSpelling_ThenGet_ThenUpdate_HappyPath()`
- Line 61: `public async Task Create_DuplicatePhone_Returns409WithErrorsPhone()`
- Line 79: `public async Task Create_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()`
- Line 93: `public async Task Create_OneCharacterName_Returns400WithErrorsFullName()`
- Line 106: `public async Task Create_EmptyPhone_Returns400WithErrorsPhone()`
- Line 119: `public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()`
- Line 134: `public async Task Update_StaleVersion_Returns409ConcurrencyStale()`
- Line 154: `public async Task Update_UnknownId_Returns404ParentNotFound()`
- Line 168: `public async Task Get_UnknownId_Returns404ParentNotFound()`
- Line 180: `public async Task Lookup_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()`
- Line 193: `public async Task Lookup_NoSuchParent_Returns200WithNullParent()`
- Line 205: `public async Task Get_NoCookie_Returns401()`
- Line 218: `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- Line 230: `public async Task Lookup_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- Line 242: `public async Task Create_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`

### `tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs`

Task 53.4: tenant isolation for schedules at the HTTP boundary, every case in both directions. Each of the three schedule resources (group, rule, session) is the foreign one in at least one case, attacked with its real ID and real version, and each answers the 404 of that resource with the victim's schedule rows compared before and after through the superuser connection. The timetable has no ID to probe, so it is checked by content: the other centre's sessions never appear, filtered or not — not even for the one account that is staff in both centres. The clock is fixed so the arranged sessions fall in a known week.

Declared test entry points (declarations are not execution results):

- Line 40: `public async Task ListRules_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)`
- Line 54: `public async Task AddRule_ToTheOtherCentresGroup_Returns404GroupNotFoundAndWritesNothing(string attackerEmail, string victimEmail)`
- Line 68: `public async Task EndRule_TheOtherCentresRuleWithItsRealVersion_Returns404RuleNotFoundAndLeavesItOpen(string attackerEmail, string victimEmail)`
- Line 84: `public async Task CancelSession_TheOtherCentresSessionWithItsRealVersion_Returns404SessionNotFoundAndLeavesItScheduled(string attackerEmail, string victimEmail)`
- Line 100: `public async Task Timetable_NeverListsTheOtherCentresSessions_FilteredByItsGroupOrTeacherOrNot(string attackerEmail, string victimEmail)`
- Line 123: `public async Task TwoCentreTeacher_Timetable_ListsOnlyTheSelectedCentresOwnSession(string selectedSlug, string otherSlug)`

### `tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs`

Task 53.4: the double-booking guarantee through the whole HTTP pipeline. Two staff add colliding weekly rules for the same teacher on two of that teacher's groups at the same instant; the exclusion constraint lets exactly one in, and the other's rule and sessions roll back with it. Each request has its own client, cookies and antiforgery token, built before the gate opens. Repeated with fresh groups, because a race test that runs once can pass by luck. Counts are read through the superuser connection. The clock is fixed, so the winner's rule always generates exactly four sessions.

Declared test entry points (declarations are not execution results):

- Line 38: `public async Task TwoParallelCollidingRulesForOneTeacher_One201AndOne409TeacherDoubleBooked_ExactlyFourSessions()`

### `tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs`

Task 53.4: the six schedule endpoints' contract through the full HTTP pipeline — status, code, body shape and formats, never messages. The application clock is fixed at , a Tuesday ten days before Egypt moves its clocks forward (Friday 30 April 2027), so "today", the 28-day horizon, "in the past" and the session instants are the same on every run and every machine. Every test builds its own subject, teacher and groups over HTTP, so seeded data never decides a result.

Declared test entry points (declarations are not execution results):

- Line 47: `public async Task AddRule_Returns201WithIdAndSessionsCreated_AndLocationAtTheGroupsRules()`
- Line 66: `public async Task ListRules_ReturnsTheContractShape_WithStartTimeAsHourMinuteAndLowerCaseWeekday()`
- Line 90: `public async Task ListRules_UnknownGroup_Returns404GroupNotFound_AndAGroupWithNoneReturnsEmptyItems()`
- Line 106: `public async Task AddRule_WithATeacherOrCentreInTheBody_Returns400RequestMalformedAndWritesNoRow(string extraField)`
- Line 127: `public async Task AddRule_NonLocalOrMisshapenValue_Returns400RequestMalformedAndWritesNoRow(string field, string value)`
- Line 140: `public async Task AddRule_MissingRequiredFieldOrMalformedJson_Returns400RequestMalformed()`
- Line 154: `public async Task AddRule_StartTimeOffTheFiveMinuteGrid_Returns400StartTimeInvalidOnErrorsStartTime()`
- Line 167: `public async Task AddRule_DurationOffTheFiveMinuteStep_Returns400DurationInvalidOnErrorsDurationMinutes()`
- Line 183: `public async Task AddRule_DurationOutOfRange_Returns400OnErrorsDurationMinutes(int durationMinutes)`
- Line 196: `public async Task AddRule_RoomOverFortyCharacters_Returns400OnErrorsRoom()`
- Line 209: `public async Task AddRule_UnknownGroup_Returns404GroupNotFound()`
- Line 219: `public async Task AddRule_OverlappingTheSameTeachersOtherGroup_Returns409TeacherDoubleBookedAndCreatesNothing()`
- Line 236: `public async Task AddRule_OverlappingTheGroupsOwnRule_Returns409GroupDoubleBookedAndCreatesNothing()`
- Line 250: `public async Task AddRule_ToAnArchivedGroup_Returns422GroupArchived()`
- Line 263: `public async Task AddRule_TakingEffectMoreThanAYearAhead_Returns422RangeInvalidOnErrorsEffectiveFrom()`
- Line 275: `public async Task AddRule_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()`
- Line 287: `public async Task ScheduleRoutes_NoCookie_Return401()`
- Line 302: `public async Task ScheduleWrite_BeforeSelectingACentre_Returns403TenantNotSelected()`
- Line 313: `public async Task EndRule_Returns204_CancelsLaterSessionsOnly_ThenAgainReturns422AlreadyEnded()`
- Line 335: `public async Task EndRule_BeforeItTakesEffect_Returns422RuleEndInvalidAndChangesNothing()`
- Line 350: `public async Task EndRule_StaleVersion_Returns409ConcurrencyStale_AndUnknownRule_Returns404RuleNotFound()`
- Line 366: `public async Task Extend_ReturnsTheCountCreated_AndRepeatingItReturnsZero()`
- Line 389: `public async Task Extend_OutsideTodayToNinetyTwoDaysAhead_Returns422RangeInvalidOnErrorsThrough(string through)`
- Line 400: `public async Task Extend_WithACentreInTheBody_Returns400RequestMalformed()`
- Line 410: `public async Task Timetable_WithoutWeekStart_Returns400OnErrorsWeekStart_AndAMisshapenOneIsMalformed()`
- Line 424: `public async Task Timetable_ReturnsTheZoneAndTheContractShape_WithInstantsInUtcEndingInZ()`
- Line 450: `public async Task Timetable_AcrossCairosSpringChange_KeepsSaturdayAtSixInTheEvening()`
- Line 466: `public async Task Timetable_ListsTheWeeksSessionsByStart_AndTheFiltersNarrowIt()`
- Line 490: `public async Task CancelSession_Returns204AndTheTimetableStillListsItAsCancelled_ThenAgainReturns422AlreadyCancelled()`
- Line 512: `public async Task CancelSession_ThatHasStarted_Returns422SessionInPastAndStaysScheduled()`
- Line 529: `public async Task CancelSession_StaleVersion_Returns409ConcurrencyStale_AndUnknownSession_Returns404SessionNotFound()`

### `tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs`

Task 53.4: the ownership guard on schedules, as black-box behaviour through teacher@both.test's real session. A group's rules inherit the group's rule: 200 for the own group, 404 (never 403) for another teacher's. The timetable lists only the teacher's own sessions, and its filters only narrow that scope. Writes are 403 even on the own group, because permission is evaluated before ownership, and they change nothing. The clock is fixed so the arranged sessions fall in a known week.

Declared test entry points (declarations are not execution results):

- Line 36: `public async Task LinkedTeacher_RulesOfTheOwnGroup_Returns200WithTheRule()`
- Line 49: `public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_Returns404GroupNotFound()`
- Line 60: `public async Task LinkedTeacher_Timetable_ListsOnlyTheOwnSessions()`
- Line 80: `public async Task LinkedTeacher_TimetableFilteredToAnotherTeacherOrTheirGroup_ReturnsNoSessions()`
- Line 99: `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`

### `tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs`

Task 41.5: owner A cannot see or touch B's students, proven in both directions with CrossTenantProbe, using a real victim student and its real current version so a 409 could never mask a leak. Reference probes cover the second kind of attack: the attacker's own, legitimate URL with the victim's id planted inside it (a body field or a route segment), which an ordinary cross-tenant probe — always aimed at the victim's own URL — cannot exercise.

Declared test entry points (declarations are not execution results):

- Line 27: `public Task NileOwner_CannotSeeOrTouchMaadisStudents() =>`
- Line 31: `public Task MaadiOwner_CannotSeeOrTouchNilesStudents() =>`
- Line 35: `public Task NileOwner_ReferenceAttacks_CannotReachMaadisParentOrStudent() =>`
- Line 39: `public Task MaadiOwner_ReferenceAttacks_CannotReachNilesParentOrStudent() =>`

### `tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs`

Task 41.5: proves the nine student endpoints' behaviour through the full HTTP pipeline, against the Day 41 contract's outcome table.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task Register_NewParent_ThenGet_ThenUpdate_HappyPath()`
- Line 59: `public async Task Register_ExistingParentForSibling_SharesTheSameParentId()`
- Line 74: `public async Task Register_DuplicateParentPhone_Returns409WithNestedParentPhoneField()`
- Line 90: `public async Task Register_BothParentFormsGiven_Returns400WithErrorsParentAndCreatesNoRow()`
- Line 106: `public async Task Register_NeitherParentFormGiven_Returns400WithErrorsParent()`
- Line 120: `public async Task Register_InvalidNestedParentPhone_Returns400PhoneInvalidWithNestedField()`
- Line 135: `public async Task Register_InvalidStudentOwnPhone_Returns400PhoneInvalidWithTopLevelField()`
- Line 150: `public async Task Register_OneCharacterName_Returns400WithErrorsFullName()`
- Line 164: `public async Task Register_UnknownFieldAtTopLevel_Returns400RequestMalformedAndCreatesNoRow()`
- Line 180: `public async Task Register_UnknownFieldInsideNestedParent_Returns400RequestMalformedAndCreatesNoRow()`
- Line 196: `public async Task List_EmptySearch_ReturnsActiveStudentsInNormalizedOrder()`
- Line 215: `public async Task List_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()`
- Line 228: `public async Task List_PageSizeTooLarge_Returns400WithErrorsPageSize()`
- Line 240: `public async Task List_MalformedCursor_Returns400WithErrorsCursor()`
- Line 252: `public async Task List_NextCursorRoundTrips_SecondPageHasNoOverlap()`
- Line 273: `public async Task Update_StaleVersion_Returns409ConcurrencyStale()`
- Line 293: `public async Task Update_UnknownId_Returns404StudentNotFound()`
- Line 307: `public async Task Get_UnknownId_Returns404StudentNotFound()`
- Line 319: `public async Task ArchiveThenArchiveAgain_SecondReturns422AlreadyArchived()`
- Line 338: `public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndStudentStaysActive()`
- Line 359: `public async Task RestoreAnActiveStudent_Returns422NotArchived()`
- Line 373: `public async Task UpdateAnArchivedStudent_Returns422ArchivedReadOnly()`
- Line 392: `public async Task LinkParent_UnknownParentId_Returns404ParentNotFound()`
- Line 407: `public async Task LinkParent_AlreadyLinked_Returns409AlreadyLinked()`
- Line 422: `public async Task FullLinkSequence_MakePrimaryThenUnlinkThenLastParentRefused()`
- Line 455: `public async Task UnlinkParent_NotLinked_Returns404StudentParentNotLinked()`
- Line 470: `public async Task MakePrimary_AlreadyPrimary_Returns422AlreadyPrimary()`
- Line 485: `public async Task LinkParent_FifthParent_Returns422TooManyParents()`
- Line 512: `public async Task Get_NoCookie_Returns401()`
- Line 525: `public async Task Register_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- Line 538: `public async Task Register_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`
- Line 553: `public async Task Register_AsTeacherWithoutPermission_Returns403PermissionDenied()`
- Line 567: `public async Task ErrorBody_HasCodeTraceIdAndCorrelationIdAndNoStackTrace()`

### `tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs`

Task 26.10: the week's first demo as an automated test — owner A cannot see or touch B's subjects, proven in both directions with CrossTenantProbe, using a real victim subject and its real current version so a 409 could never mask a leak.

Declared test entry points (declarations are not execution results):

- Line 25: `public Task NileOwner_CannotSeeOrTouchMaadisSubjects() =>`
- Line 29: `public Task MaadiOwner_CannotSeeOrTouchNilesSubjects() =>`
- Line 33: `public async Task TwoCentreTeacher_SeesNilesListThenMaadisAfterSwitching_NeverAMix()`

### `tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs`

Task 26.9: proves the six Subject endpoints' behaviour through the full HTTP pipeline, against the Day 26 contract.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task List_ReturnsOnlyTheCallersCentresSubjectsInNameOrder()`
- Line 39: `public async Task Create_ThenGet_Returns201WithLocationThen200()`
- Line 56: `public async Task Create_Duplicate_Returns409WithErrorsName()`
- Line 75: `public async Task Create_EmptyName_Returns400WithErrorsName(string name)`
- Line 87: `public async Task Create_EightyOneCharacterName_Returns400WithErrorsName()`
- Line 99: `public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()`
- Line 113: `public async Task Rename_HappyPathThenStaleVersion()`
- Line 130: `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- Line 159: `public async Task List_NoCookie_Returns401()`
- Line 172: `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- Line 184: `public async Task List_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`
- Line 197: `public async Task Get_NonGuidId_Returns404RouteNotFound()`
- Line 209: `public async Task ErrorBody_AlwaysHasCodeTraceIdCorrelationIdAndNoStackTrace()`

### `tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs`

Task 28.10: the Day 28 permission matrix proved through HTTP for the six Subject endpoints, over Nile's three roles (the Day 26 harness). 18 literal cases (6 endpoints x 3 roles) plus one extra: permission is evaluated before existence, so a forbidden request against an unknown ID is still 403, never 404.

Declared test entry points (declarations are not execution results):

- Line 27: `public async Task Teacher_List_Returns200()`
- Line 37: `public async Task Teacher_GetById_Returns200()`
- Line 49: `public async Task Teacher_Create_IsForbiddenAndCreatesNoRow()`
- Line 62: `public async Task Teacher_Rename_IsForbiddenAndLeavesTheRowUnchanged()`
- Line 76: `public async Task Teacher_Archive_IsForbiddenAndLeavesTheRowUnchanged()`
- Line 89: `public async Task Teacher_Restore_IsForbiddenAndLeavesTheRowUnchanged()`
- Line 104: `public async Task OwnerAndSecretary_CanListGetCreateRenameArchiveAndRestore(string email)`
- Line 146: `public async Task Teacher_ForbiddenRenameAgainstAnUnknownId_Returns403NotFound_PermissionBeforeExistence()`

### `tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs`

Task 43.14: owner A cannot see or touch B's teachers, proven in both directions with CrossTenantProbe across all five ID routes, using a real victim teacher and its real current version so a 409 could never mask a leak. The account link gets its own reference probe: linking the attacker's own teacher to a user who belongs to the victim's centre must answer teacher.user_not_eligible, the same answer as any other ineligible user, never disclosing that the ID itself resolved to someone real.

Declared test entry points (declarations are not execution results):

- Line 27: `public Task NileOwner_CannotSeeOrTouchMaadisTeachers() =>`
- Line 31: `public Task MaadiOwner_CannotSeeOrTouchNilesTeachers() =>`
- Line 35: `public async Task SetAccount_WithAnotherCentresUserId_ReturnsUserNotEligibleAndChangesNothing()`

### `tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs`

Task 43.14: proves the seven Teacher endpoints' behaviour through the full HTTP pipeline, against the Day 43 contract.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task Create_ThenGet_Returns201WithLocationThen200()`
- Line 43: `public async Task Create_TwoTeachersSharingAName_BothSucceed()`
- Line 58: `public async Task Create_EmptyName_Returns400WithErrorsFullName(string fullName)`
- Line 70: `public async Task Create_WithExtraUserIdField_Returns400RequestMalformedAndCreatesNoRow()`
- Line 85: `public async Task Update_WithExtraUserIdField_Returns400RequestMalformedAndLeavesLinkUnchanged()`
- Line 100: `public async Task Update_HappyPathThenStaleVersion()`
- Line 119: `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- Line 148: `public async Task Update_ArchivedTeacher_Returns422ArchivedReadOnly()`
- Line 169: `public async Task SetAccount_IneligibleUserId_Returns422UserNotEligibleAndLeavesLinkUnchanged()`
- Line 188: `public async Task SetAccount_LinkThenUnlink_Returns204EachAndListShowsDisplayName()`
- Line 214: `public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_Returns409AccountTaken()`
- Line 236: `public async Task List_NoCookie_Returns401()`
- Line 249: `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- Line 261: `public async Task Get_NonGuidId_Returns404RouteNotFound()`

### `tests/TutoringCentre.Api.Tests/Audit/AuditEndpointTests.cs`

Task 32.4: proves the GET /api/audit contract (Day 32) through the full HTTP pipeline.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task Get_AfterRenameAndRoleChange_ReturnsBothEntriesNewestFirstWithBeforeAndAfter()`
- Line 52: `public async Task Get_AfterGroupCapacityChange_ReturnsTheNumericBeforeAndAfterAsText()`
- Line 88: `public async Task Get_FilteredByEntityIdOfSubject_ReturnsOnlyThatSubjectsHistory()`
- Line 112: `public async Task Get_FilteredByAMonthThreeEntityType_ReturnsOnlyThatTypesEntries(string entityType)`
- Line 127: `public async Task Get_FilteredByClassSession_IsAcceptedEvenWithNoEntries()`
- Line 140: `public async Task Get_PageSizeOne_YieldsCursorAndSecondPageReturnsTheNextEntry()`
- Line 165: `public async Task Get_WithInvalidQueryParameter_Returns400(string query)`
- Line 175: `public async Task Get_AsMaadiOwner_SeesNoneOfNilesEntries_WithOrWithoutNilesEntityIdFilter()`
- Line 199: `public async Task Get_AsSecretaryOrTeacher_Returns403(string role)`
- Line 214: `public async Task UnsupportedMethod_Returns404Or405(HttpMethod method)`
- Line 234: `public async Task Get_ResponseContainsNoEmailOrPasswordField()`

### `tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs`

The login rate limiter already partitions by the connection's remote address (Task 34.4's own requirement: change it only if it reads anything else — it does not), so once Task 34.3's forwarded-header processing is wired up, these prove the limiter is automatically keyed by the real client behind the trusted proxy. Deliberately bypasses 's per-call random partition header: these tests need the real partitioning logic, not the testing-only escape hatch.

Declared test entry points (declarations are not execution results):

- Line 22: `public async Task FromATrustedProxy_TheRateLimitPartitionsByTheForwardedAddress()`
- Line 41: `public async Task FromAnUntrustedSource_ChangingTheForwardedHeaderDoesNotResetTheBudget()`

### `tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs`

Declared test entry points (declarations are not execution results):

- Line 35: `public async Task Login_AsOwner_SetsHostPrefixedSecureCookieAndAutoSelectsTheirOnlyCentre()`
- Line 67: `public async Task Login_AsSecretaryWithOneCentre_AutoSelectsAndReturnsItsPermissions()`
- Line 85: `public async Task Login_AsTeacherWithTwoCentres_LeavesNoActiveCentreSelected()`
- Line 102: `public async Task Login_WhileAlreadySignedIn_ReplacesTheSessionWithTheNewIdentity()`
- Line 127: `public async Task SelectCentre_ForAMembershipTheUserHolds_Returns204AndActivatesIt()`
- Line 142: `public async Task SelectCentre_ForACentreTheUserDoesNotBelongTo_Returns403AndLeavesSessionUnchanged()`
- Line 162: `public async Task GetMe_AsOwner_ReturnsAllSixteenPermissionsSorted()`
- Line 179: `public async Task GetMe_AsSecretary_ReturnsStudentsAndSubjectsPermissionsSorted()`
- Line 196: `public async Task GetMe_AsTwoCentreTeacherBeforeSelectingACentre_ReturnsNoPermissions()`
- Line 212: `public async Task Logout_Returns204AndClearsTheCookie()`
- Line 228: `public async Task GetMe_AfterLogout_Returns401()`

### `tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs`

Proves the attacks Day 16 closes are actually closed: user enumeration, CSRF (16.6, written by hand), and cookie theft, tenant escalation, stale sessions, claim forgery, spraying, brute force, broken identity propagation and forgotten authorization (16.7). Uses rather than the plain because two cases need its test-only endpoints (current actor, default protection).

Declared test entry points (declarations are not execution results):

- Line 43: `public async Task Login_WrongPasswordForKnownUserAndUnknownUser_ReturnIdenticalResponses()`
- Line 73: `public async Task Logout_WithoutCsrfHeader_Returns403AndSessionStillWorksAfterwards()`
- Line 94: `public async Task Login_WithoutCsrfHeader_Returns403()`
- Line 109: `public async Task Login_SetsAHostPrefixedSecureHttpOnlyLaxCookie()`
- Line 124: `public async Task GetMe_WithNoCookie_Returns401()`
- Line 134: `public async Task SelectCentre_ForACentreOwnerDoesNotBelongTo_Returns403AndMeStillShowsTheOriginalCentre()`
- Line 155: `public async Task RevokedMembership_NextRequestAfterRevocation_Returns401()`
- Line 178: `public async Task ChangedSecurityStamp_NextRequest_Returns401()`
- Line 195: `public async Task RoleChangedByTheOwner_NextRequestForThatUser_Returns401()`
- Line 231: `public async Task TamperedCookie_Returns401()`
- Line 257: `public async Task Login_11TimesWithinAMinuteFromOneClient_11thReturns429WithRetryAfter()`
- Line 279: `public async Task Login_FiveWrongPasswordsThenCorrect_StillReturns401WithTheOrdinaryFailureBody()`
- Line 299: `public async Task CurrentActor_AfterSelectingACentre_IsAStaffActorWithThatCentreInApplication()`
- Line 330: `public async Task EndpointWithNoAuthorizationAttribute_Returns401ByDefault()`

### `tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs`

Task 30.8: enumerates the running application's own endpoints and demands that every one of them has been explicitly classified into one of three lists — anonymous, any authenticated user, or the permission matrix (). An endpoint nobody classified fails the build, so it cannot ship unprotected by accident. Expectations are explicit routes, never wildcards or generated from the role map.

Declared test entry points (declarations are not execution results):

- Line 38: `public void EveryRunningEndpoint_IsClassifiedExactlyOnce()`

### `tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs`

Task 30.8 (extended Task 32.11, Task 43.14, Task 45.9, Task 49.9, Task 53.4): one table of role x endpoint -> expected status, executed over real HTTP against Nile's three roles, now including the seven teacher endpoints (21 more literal cases), the eight group endpoints (24 more), the five enrollment endpoints (15 more) and the six schedule endpoints (18 more). The matrix records the permission decision only: Teacher is allowed on the two group reads, the two enrollment reads and the two schedule reads (and reads a group it owns here), while what a teacher may see is ownership, proven separately by GroupTeacherScopeTests, EnrollmentTeacherScopeTests and ScheduleTeacherScopeTests. The schedule cases run on a fixed application clock, so no case depends on today. Allowed cases assert a non-403 success status; denied cases assert 403 auth.permission_denied and an unchanged database, compared against the exact pre-request state, not a snapshot taken after the request already ran. Values are written literally here, never derived from RolePermissions — that would prove the matrix agrees with itself, not with the contract.

Declared test entry points (declarations are not execution results):

- Line 101: `public async Task SubjectsList(string role, HttpStatusCode expected)`
- Line 114: `public async Task SubjectsGetById(string role, HttpStatusCode expected)`
- Line 128: `public async Task SubjectsCreate(string role, HttpStatusCode expected)`
- Line 148: `public async Task SubjectsRename(string role, HttpStatusCode expected)`
- Line 168: `public async Task SubjectsArchive(string role, HttpStatusCode expected)`
- Line 188: `public async Task SubjectsRestore(string role, HttpStatusCode expected)`
- Line 208: `public async Task StaffList(string role, HttpStatusCode expected)`
- Line 225: `public async Task StaffCreate(string role, HttpStatusCode expected)`
- Line 248: `public async Task StaffChangeRole(string role, HttpStatusCode expected)`
- Line 268: `public async Task StaffDeactivate(string role, HttpStatusCode expected)`
- Line 288: `public async Task StaffReactivate(string role, HttpStatusCode expected)`
- Line 308: `public async Task AuditList(string role, HttpStatusCode expected)`
- Line 325: `public async Task CentreSettingsGet(string role, HttpStatusCode expected)`
- Line 342: `public async Task CentreSettingsUpdate(string role, HttpStatusCode expected)`
- Line 364: `public async Task ParentsCreate(string role, HttpStatusCode expected)`
- Line 385: `public async Task ParentsGetById(string role, HttpStatusCode expected)`
- Line 403: `public async Task ParentsUpdate(string role, HttpStatusCode expected)`
- Line 425: `public async Task ParentsLookup(string role, HttpStatusCode expected)`
- Line 443: `public async Task StudentsList(string role, HttpStatusCode expected)`
- Line 460: `public async Task StudentsGetById(string role, HttpStatusCode expected)`
- Line 478: `public async Task StudentsRegister(string role, HttpStatusCode expected)`
- Line 503: `public async Task StudentsUpdate(string role, HttpStatusCode expected)`
- Line 525: `public async Task StudentsArchive(string role, HttpStatusCode expected)`
- Line 545: `public async Task StudentsRestore(string role, HttpStatusCode expected)`
- Line 565: `public async Task StudentsLinkParent(string role, HttpStatusCode expected)`
- Line 587: `public async Task StudentsUnlinkParent(string role, HttpStatusCode expected)`
- Line 608: `public async Task StudentsMakePrimary(string role, HttpStatusCode expected)`
- Line 628: `public async Task TeachersList(string role, HttpStatusCode expected)`
- Line 645: `public async Task TeachersGetById(string role, HttpStatusCode expected)`
- Line 663: `public async Task TeachersCreate(string role, HttpStatusCode expected)`
- Line 683: `public async Task TeachersUpdate(string role, HttpStatusCode expected)`
- Line 704: `public async Task TeachersSetAccount(string role, HttpStatusCode expected)`
- Line 725: `public async Task TeachersArchive(string role, HttpStatusCode expected)`
- Line 745: `public async Task TeachersRestore(string role, HttpStatusCode expected)`
- Line 765: `public async Task GroupsList(string role, HttpStatusCode expected)`
- Line 782: `public async Task GroupsGetById(string role, HttpStatusCode expected)`
- Line 802: `public async Task GroupsCreate(string role, HttpStatusCode expected)`
- Line 825: `public async Task GroupsRename(string role, HttpStatusCode expected)`
- Line 846: `public async Task GroupsChangeCapacity(string role, HttpStatusCode expected)`
- Line 867: `public async Task GroupsAssignTeacher(string role, HttpStatusCode expected)`
- Line 889: `public async Task GroupsArchive(string role, HttpStatusCode expected)`
- Line 910: `public async Task GroupsRestore(string role, HttpStatusCode expected)`
- Line 931: `public async Task EnrollmentsEnroll(string role, HttpStatusCode expected)`
- Line 953: `public async Task EnrollmentsRoster(string role, HttpStatusCode expected)`
- Line 973: `public async Task EnrollmentsListForStudent(string role, HttpStatusCode expected)`
- Line 991: `public async Task EnrollmentsWithdraw(string role, HttpStatusCode expected)`
- Line 1013: `public async Task EnrollmentsTransfer(string role, HttpStatusCode expected)`
- Line 1037: `public async Task ScheduleListRules(string role, HttpStatusCode expected)`
- Line 1058: `public async Task ScheduleAddRule(string role, HttpStatusCode expected)`
- Line 1080: `public async Task ScheduleEndRule(string role, HttpStatusCode expected)`
- Line 1103: `public async Task ScheduleExtend(string role, HttpStatusCode expected)`
- Line 1125: `public async Task ScheduleTimetable(string role, HttpStatusCode expected)`
- Line 1143: `public async Task ScheduleCancelSession(string role, HttpStatusCode expected)`

### `tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs`

Task 32.10: proves the GET/PUT /api/centre/settings contract (Day 32) through the full HTTP pipeline.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task Get_AsOwner_ReturnsTheContractShape()`
- Line 39: `public async Task Put_HappyPath_ThenGetShowsTheNewNameAndVersion_AndMeShowsTheNewName()`
- Line 64: `public async Task Put_BodyWithSlug_Returns400()`
- Line 76: `public async Task Put_EmptyName_Returns400WithErrorsName()`
- Line 90: `public async Task Get_AsSecretary_Returns403()`
- Line 102: `public async Task Put_AsSecretary_Returns403()`
- Line 115: `public async Task Get_AsTwoCentreTeacher_Returns403()`
- Line 127: `public async Task Put_AsTwoCentreTeacher_Returns403()`

### `tests/TutoringCentre.Api.Tests/Cli/MigrateCommandTests.cs`

Declared test entry points (declarations are not execution results):

- Line 10: `public async Task RunAsync_CalledTwice_SecondRunIsANoOp()`

### `tests/TutoringCentre.Api.Tests/Cli/ResetPasswordCommandTests.cs`

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task RunAsync_ExistingUser_OldPasswordStopsWorkingAndNewPasswordWorks()`
- Line 35: `public async Task RunAsync_UnknownEmail_ReturnsNonZeroWithoutThrowing()`

### `tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs`

Declared test entry points (declarations are not execution results):

- Line 18: `public void SeedGate_OutsideProduction_AlwaysAllowed(string environmentName) =>`
- Line 22: `public void SeedGate_Production_WithDemoDisabled_IsRefused() =>`
- Line 26: `public void SeedGate_Production_WithDemoEnabled_IsAllowed() =>`
- Line 32: `public void SeedGate_OutsideProduction_IncludesTheSetupCentre(string environmentName) =>`
- Line 36: `public void SeedGate_Production_NeverIncludesTheSetupCentre_EvenOnADemoInstance()`
- Line 53: `public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentresAndTheEmptySetupCentre()`
- Line 69: `public async Task RunAsync_LeavesTheSetupCentreEmpty_ExceptForItsOneOwner()`
- Line 96: `public async Task RunAsync_CalledTwice_CreatesExactlyTheStaffSetIdempotently()`
- Line 119: `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSubjectsIdempotently()`
- Line 146: `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoStudentsAndParentsIdempotently()`
- Line 208: `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoTeachersAndGroupsIdempotently()`
- Line 288: `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoEnrollmentsIdempotently_LeavingOneSeatInTheTwoSeatGroup()`
- Line 319: `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSchedulesIdempotently_FromTheSeedDay()`

### `tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs`

Fetches and attaches an antiforgery token so tests exercise the real CSRF protection, never bypass it.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs`

All database-backed API tests share one factory/container; they run sequentially.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs`

Hosts the real Api in-process against a throwaway PostgreSQL 17 container, on the same three-role split as everywhere else (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated as tutoring_owner, the app itself running as tutoring_app. Environment "Testing" means no development auto-migration and no user-secrets: the test controls the database.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs`

Test-only endpoints. They exist only in the test host — never in the product or its OpenAPI document.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs`

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs`

Proves the actor set by ActorMiddleware actually reaches the Application layer (Day 16, Task 16.7).

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs`

Convention tests share one factory/container; they run sequentially.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`

The real API plus test-only endpoints/handlers and an in-memory log sink. Own container, own collection.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs`

Attacks a victim's resource as a given (attacker) client and checks two things: the response itself, and that the victim's row — read through a caller-supplied snapshot, taken over the owner/superuser connection — is unchanged. Feature-agnostic: knows nothing about subjects or any other module, only HTTP and a snapshot function. A probe that checked only the status would prove nothing; this is why the snapshot is required, not optional.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs`

Task 26.8: a probe that cannot fail proves nothing. This proves CrossTenantProbe actually can — using a real subject as a convenient, already-seeded resource (the helper itself still knows nothing about subjects).

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task RunAsync_AttackerIsActuallyAMemberOfTheSameCentre_FailsInsteadOfPassingSilently()`

### `tests/TutoringCentre.Api.Tests/Fixtures/FixableTimeProvider.cs`

The application clock's time source in the Api tests (Day 53). Real time by default, so every earlier test runs exactly as before; a test that depends on "today" or "in the past" fixes it with and the fixture puts it back with . The collection runs its classes one at a time, so a fixed time never leaks into a concurrently running test.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersCollection.cs`

Forwarded-header and forwarded-IP-rate-limit tests share one factory/container; they run sequentially.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/ForwardedHeadersFactory.cs`

Trusts 10.0.0.0/8 as the proxy network (Task 34.3), with a simulated direct-connection address per request.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs`

Captures Serilog events in memory so tests can assert on what the API logged.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/RoleBootstrap.cs`

Runs the real db/bootstrap-roles.sh (Day 19) inside a throwaway Testcontainers PostgreSQL instance, with test-only passwords, so the test fixture exercises the exact same role split as production instead of a stand-in for it.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/SignedInClient.cs`

An HTTP client signed in as a specific seeded user through the real login endpoint — its own cookies, its own session. Feature-agnostic: knows nothing about subjects or any other module. Never shared between tests or users; each sign-in gets its own client.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/SimulatedDirectConnectionStartupFilter.cs`

Testing-only escape hatch for forwarded-header tests (Task 34.3): an in-memory TestServer request has no real socket, so HttpContext.Connection.RemoteIpAddress is always null — there is no "direct connection address" for ForwardedHeadersMiddleware to check against a trusted network. This simulates one, read from a header a real caller could never reach (not honoured outside the "Testing" environment, same convention as ), and runs before next(app) so it takes effect before Program's own forwarded-headers middleware.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingCollection.cs`

SPA hosting tests share one factory/container; its web root is fixed for the whole class.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Fixtures/SpaHostingFactory.cs`

The real API with its web root pointed at a temporary directory holding a marker index.html and one hashed asset — standing in for a real frontend build, without needing one on disk for this test run.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task Liveness_ReturnsOk()`
- Line 29: `public async Task Readiness_WhenDatabaseUnreachable_Returns503()`

### `tests/TutoringCentre.Api.Tests/Health/ReadinessPrivilegeTests.cs`

The API test half of Task 19.7's eight tests: the runtime-role tripwire (Day 19) fails readiness closed.

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task GetReady_WithOwnerLoginAsRuntimeConnection_Returns503()`

### `tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task GetReady_WithReachableDatabase_Returns200()`

### `tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task NoHeaderSupplied_GeneratesCorrelationId()`
- Line 24: `public async Task ValidHeaderSupplied_IsEchoedBack()`
- Line 35: `public async Task InvalidHeaderTooLong_IsReplaced()`
- Line 46: `public async Task InvalidHeaderBadCharacters_IsReplaced()`
- Line 57: `public async Task CorrelationId_MatchesBetweenResponseHeaderAndProblemBody()`
- Line 69: `public async Task CorrelationId_AppearsInRequestLogLine()`

### `tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs`

Proves the key ring is shared, not per-process (Task 34.6): a cookie minted by one application instance is accepted by a second, independent instance that only shares the same database — exactly what two container replicas, or one replica before and after a restart, look like. Without persistence this would 401, because each instance would have generated its own, different key ring in memory.

Declared test entry points (declarations are not execution results):

- Line 43: `public async Task CookieFromOneInstance_IsAcceptedByASecondInstance_OnTheSameDatabase()`

### `tests/TutoringCentre.Api.Tests/Http/ForwardedHeadersTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task FromATrustedProxy_TheForwardedAddressIsLogged()`
- Line 25: `public async Task FromAnUntrustedSource_TheForwardedAddressIsIgnored()`

### `tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task SuccessfulRequest_LogsOneCompletionLineAtInformation()`
- Line 26: `public async Task UnhandledException_LogsAtErrorLevel()`

### `tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task Ok_ReturnsSuccessBody()`
- Line 22: `public async Task Validation_Returns400WithProblemJsonAndCode()`
- Line 32: `public async Task NotFound_Returns404WithCode()`
- Line 42: `public async Task Conflict_Returns409WithCode()`
- Line 52: `public async Task Rule_Returns422WithCode()`
- Line 62: `public async Task Forbidden_Returns403WithCode()`
- Line 72: `public async Task UnhandledException_Returns500WithoutLeakingInternals()`
- Line 85: `public async Task MalformedJsonBody_Returns400RequestMalformed()`
- Line 96: `public async Task UnknownJsonField_Returns400RequestMalformed()`
- Line 106: `public async Task EmptyName_FailsValidation_Returns400()`
- Line 115: `public async Task ValidName_ReturnsGreeting()`
- Line 125: `public async Task UnknownApiRoute_Returns404RouteNotFound()`
- Line 135: `public async Task UnknownApiRoute_PostMethod_Returns404RouteNotFound()`
- Line 145: `public async Task UnknownNonApiRoute_Returns404ProblemJson()`
- Line 157: `public async Task ErrorResponses_IncludeTraceId(string path)`
- Line 166: `public async Task ErrorResponse_IncludesCorrelationIdMatchingRequestHeader()`
- Line 178: `public async Task ErrorResponses_ContentTypeIsProblemJson()`
- Line 186: `public async Task ValidationError_IncludesFieldErrorsDictionary()`
- Line 197: `public async Task NonValidationError_DoesNotIncludeErrorsDictionary()`

### `tests/TutoringCentre.Api.Tests/Http/ProxyOptionsValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 10: `public void Production_WithNoKnownNetworks_Fails()`
- Line 20: `public void Production_WithAKnownNetwork_Succeeds()`
- Line 32: `public void OutsideProduction_WithNoKnownNetworks_Succeeds(string environmentName)`
- Line 42: `public void AnyEnvironment_WithAMalformedNetwork_Fails()`

### `tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs`

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task ToHttpResult_FailureKind_MapsStatusAndCode(ErrorKind kind, int expectedStatus, string code)`
- Line 33: `public async Task ToHttpResult_ValidationWithFields_IncludesErrorsObject()`
- Line 47: `public async Task ToHttpResult_ConflictWithFields_IncludesErrorsObjectToo()`
- Line 64: `public async Task ToHttpResult_Failure_IncludesTraceIdAndCorrelationId()`
- Line 76: `public async Task ToHttpResult_Success_ReturnsWhatOnSuccessProduces()`
- Line 87: `public async Task ToHttpResult_NonGenericSuccess_Returns204()`
- Line 96: `public async Task ToHttpResult_NonGenericFailure_Returns409WithCode()`
- Line 106: `public async Task ToCreatedHttpResult_Success_Returns201WithLocationAndBody()`
- Line 125: `public async Task ToCreatedHttpResult_FailureKind_MapsToTheSameStatusAsTheExistingMapping(ErrorKind kind, int expectedStatus, string code)`
- Line 137: `public async Task ToCreatedHttpResult_NoLocationVariant_Returns201WithBodyAndNoLocationHeader()`
- Line 156: `public async Task ToCreatedHttpResult_NoLocationVariant_FailureKind_MapsToTheSameStatusAsTheExistingMapping(`

### `tests/TutoringCentre.Api.Tests/Http/SecurityHeadersTests.cs`

Declared test entry points (declarations are not execution results):

- Line 21: `public async Task EveryResponseKind_CarriesTheSecurityHeaders(string path)`
- Line 36: `public async Task ApiResponse_WithNoOwnCacheControl_GetsNoStore()`
- Line 44: `public async Task ApiErrorResponse_AlsoGetsNoStore()`
- Line 52: `public async Task HtmlResponse_KeepsItsOwnCacheControl_NotNoStore()`
- Line 60: `public async Task OutsideProduction_NoHstsHeader()`

### `tests/TutoringCentre.Api.Tests/Http/SpaHostingTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task Root_WithNoSession_ServesTheAppShell()`
- Line 22: `public async Task DeepLink_WithNoSession_ServesTheAppShell()`
- Line 31: `public async Task IndexHtml_HasNoCacheHeader()`
- Line 39: `public async Task HashedAsset_HasImmutableCacheHeader()`
- Line 49: `public async Task MissingAsset_Is404_NotTheAppShell()`
- Line 58: `public async Task UnknownApiRoute_StaysJsonNotFound()`
- Line 69: `public async Task HealthEndpoints_AreNotShadowedByTheFallback()`
- Line 77: `public async Task DirectIndexHtmlPath_IsServedByStaticFiles()`

### `tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public void Destructure_MasksPasswordTokenAndPhoneProperties()`
- Line 23: `public void Destructure_LeavesNonSensitivePropertiesIntact()`
- Line 32: `public void Destructure_ObjectWithNoSensitiveProperties_IsUnaffected()`

### `tests/TutoringCentre.Api.Tests/Staff/ChangePasswordHttpTests.cs`

Task 30.7: POST /api/auth/change-password and the Day 30 first-login gate, through the full HTTP pipeline.

Declared test entry points (declarations are not execution results):

- Line 28: `public async Task WrongCurrentPassword_Returns422()`
- Line 41: `public async Task WeakNewPassword_Returns400WithErrorsNewPassword()`
- Line 55: `public async Task Success_ReissuesTheCookieAndTheOldCookiesNextRequestIs401()`
- Line 75: `public async Task SixthAttemptInAMinute_Returns429()`
- Line 97: `public async Task FirstLoginGateSequence_FromCreateToCentreSelection()`

### `tests/TutoringCentre.Api.Tests/Staff/StaffCrossTenantProbeTests.cs`

Task 30.7: a Maadi owner cannot see or touch a Nile membership, and vice versa — proven in both directions with CrossTenantProbe, using the victim's real membership ID and real current version so a 409 could never mask a leak. identity.memberships carries no RLS and no EF query filter (Day 29), so this is the only proof.

Declared test entry points (declarations are not execution results):

- Line 25: `public Task NileOwner_CannotTouchMaadisOwnMembership() => RunDirectionAsync(`
- Line 29: `public Task MaadiOwner_CannotTouchNilesSecretaryMembership() => RunDirectionAsync(`

### `tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs`

Task 30.7: proves the five staff endpoints' contract behaviour through the full HTTP pipeline (Day 30 contract).

Declared test entry points (declarations are not execution results):

- Line 26: `public async Task List_AsOwner_ReturnsAllNileMembersWithOnlyContractFields()`
- Line 45: `public async Task Create_NewEmail_Returns201WithPasswordAndNoStore()`
- Line 60: `public async Task Create_EmailOfAnExistingAccount_Returns201WithNullPasswordAndNoStore()`
- Line 74: `public async Task Create_EmailAlreadyAMemberOfThisCentre_Returns409WithErrorsEmail()`
- Line 88: `public async Task Create_UnknownRole_Returns400RequestMalformed()`
- Line 101: `public async Task Create_OverPostedCentreId_Returns400RequestMalformedAndCreatesNoRow()`
- Line 119: `public async Task Create_InvalidEmail_Returns400WithErrorsEmail(string email)`
- Line 132: `public async Task ChangeRole_HappyPathThenStaleVersion()`
- Line 149: `public async Task ChangeRole_ValidationErrors_HaveFieldNames()`
- Line 161: `public async Task Deactivate_ThenReactivate_FlipsStatusInTheList()`
- Line 180: `public async Task Reactivate_AlreadyActive_Returns422MembershipAlreadyActive()`
- Line 194: `public async Task Deactivate_AlreadyInactive_Returns422MembershipAlreadyInactive()`

### `tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs`

Task 30.7: threat-driven tests for the five staff endpoints — self-escalation, over-posting, a secretary attempting to manage staff, and a role change ending the affected user's session.

Declared test entry points (declarations are not execution results):

- Line 32: `public async Task Owner_ChangesOwnRole_Returns422CannotChangeSelf()`
- Line 46: `public async Task Owner_DeactivatesSelf_Returns422CannotChangeSelf()`
- Line 66: `public async Task LastOwnerError_MapsTo422ThroughTheRealHttpMapping()`
- Line 86: `public async Task ChangeRole_OverPostedOrUnknownRole_Returns400(string rawBody)`
- Line 103: `public async Task Secretary_PostsValidCreateBody_Returns403AndCreatesNoUserOrMembership()`
- Line 120: `public async Task Secretary_GetsStaffList_Returns403()`
- Line 132: `public async Task ChangeRole_AffectedUsersNextRequest_Returns401()`

### `tests/TutoringCentre.Api.Tests/Telemetry/TelemetrySetupTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void NoEndpointConfigured_RegistersNoTracerProvider()`
- Line 23: `public void EndpointConfigured_RegistersATracerProvider()`

### `tests/TutoringCentre.Application.Tests/Academics/AddScheduleRuleHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFoundAndAddsNothing()`
- Line 24: `public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchivedAndAddsNothing()`
- Line 35: `public async Task HandleAsync_EffectiveFromMoreThan366DaysAhead_ReturnsRangeInvalidOnEffectiveFrom()`
- Line 46: `public async Task HandleAsync_EffectiveFromExactly366DaysAhead_AddsTheRuleWithNoSessionsYet()`
- Line 59: `public async Task HandleAsync_RuleFromToday_AddsTheRuleAndFourWeeksOfSessionsForTheGroupsTeacher()`
- Line 86: `public async Task HandleAsync_TakesTheTeachersScheduleLockBeforeAddingAnySession()`
- Line 97: `public async Task HandleAsync_RuleThatTookEffectInThePast_GeneratesNoDateBeforeToday()`
- Line 109: `public async Task HandleAsync_RuleFromLaterInTheHorizon_GeneratesFromItsFirstDateOnly()`
- Line 122: `public async Task HandleAsync_DurationOffTheFiveMinuteStep_ReturnsTheRulesOwnError()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveGroupHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_ActiveGroup_Archives()`
- Line 39: `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- Line 55: `public async Task HandleAsync_GroupWithActiveEnrollments_ReturnsHasActiveEnrollmentsAndStaysActive()`
- Line 72: `public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndStaysActive()`
- Line 88: `public async Task HandleAsync_GroupWithARuleStillInEffect_ReturnsHasActiveScheduleAndStaysActive()`
- Line 106: `public async Task HandleAsync_GroupWhoseOnlyRuleEndedBeforeToday_Archives()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveRestoreStudentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task Archive_ActiveStudent_Succeeds()`
- Line 28: `public async Task Archive_AlreadyArchived_ReturnsAlreadyArchived()`
- Line 43: `public async Task Archive_UnknownStudentId_ReturnsStudentNotFound()`
- Line 54: `public async Task Archive_StudentWithAnActiveEnrollment_ReturnsHasActiveEnrollmentsAndStaysActive()`
- Line 72: `public async Task Archive_StudentWhoseEnrollmentsHaveAllEnded_Succeeds()`
- Line 90: `public async Task Restore_ArchivedStudent_Succeeds()`
- Line 105: `public async Task Restore_AlreadyActive_ReturnsNotArchived()`
- Line 119: `public async Task Restore_UnknownStudentId_ReturnsStudentNotFound()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 26: `public async Task HandleAsync_ActiveSubject_Archives()`
- Line 40: `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- Line 56: `public async Task HandleAsync_SubjectWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()`
- Line 74: `public async Task HandleAsync_SubjectWithOnlyArchivedGroups_Archives()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveSubjectValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyId_ReportsErrorOnSubjectId()`
- Line 17: `public void Validate_ValidCommand_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 26: `public async Task HandleAsync_ActiveTeacher_ArchivesAndKeepsLink()`
- Line 43: `public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()`
- Line 59: `public async Task HandleAsync_TeacherWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()`
- Line 77: `public async Task HandleAsync_TeacherWithOnlyArchivedGroups_Archives()`

### `tests/TutoringCentre.Application.Tests/Academics/ArchiveTeacherValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- Line 17: `public void Validate_ValidId_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/AssignGroupTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()`
- Line 26: `public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFoundAndKeepsTeacher()`
- Line 39: `public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchivedAndKeepsTeacher()`
- Line 53: `public async Task HandleAsync_ActiveTeacher_Reassigns()`
- Line 65: `public async Task HandleAsync_SameTeacher_SucceedsWithNoChange()`
- Line 77: `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`
- Line 91: `public async Task HandleAsync_ActiveTeacher_MovesFutureScheduledSessionsAndLeavesPastAndCancelledOnes()`

### `tests/TutoringCentre.Application.Tests/Academics/CancelSessionHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task HandleAsync_UnknownSession_ReturnsSessionNotFound()`
- Line 24: `public async Task HandleAsync_FutureSession_IsCancelled()`
- Line 35: `public async Task HandleAsync_PastSession_ReturnsSessionInPastAndStaysScheduled()`
- Line 48: `public async Task HandleAsync_AlreadyCancelledSession_ReturnsSessionAlreadyCancelled()`

### `tests/TutoringCentre.Application.Tests/Academics/ChangeGroupCapacityHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_InRange_ChangesCapacity()`
- Line 38: `public async Task HandleAsync_OutOfRange_ReturnsCapacityInvalidAndKeepsCapacity(int capacity)`
- Line 51: `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`
- Line 64: `public async Task HandleAsync_BelowTheActiveEnrollmentCount_ReturnsCapacityBelowEnrolledAndKeepsCapacity()`
- Line 78: `public async Task HandleAsync_EqualToTheActiveEnrollmentCount_ChangesCapacity()`
- Line 90: `public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndKeepsCapacity()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task HandleAsync_UnknownSubject_ReturnsSubjectNotFound()`
- Line 27: `public async Task HandleAsync_ArchivedSubject_ReturnsSubjectArchived()`
- Line 39: `public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFound()`
- Line 51: `public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchived()`
- Line 63: `public async Task HandleAsync_ArchivedSubjectAndUnknownTeacher_ReportsTheSubjectFirst()`
- Line 76: `public async Task HandleAsync_CapacityOutOfRange_ReturnsCapacityInvalid(int capacity)`
- Line 88: `public async Task HandleAsync_NameAlreadyUsed_ReturnsNameTakenOnNameField()`
- Line 102: `public async Task HandleAsync_Valid_AddsGroupInTheActorsCentre()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task HandleAsync_Valid_AddsParentWithActorsCentreAndCanonicalPhone()`
- Line 32: `public async Task HandleAsync_DifferentlySpelledPhoneAlreadyExists_ReturnsPhoneTakenAndAddsNothing()`
- Line 50: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithFieldAndAddsNothing()`
- Line 67: `public async Task HandleAsync_OneCharacterName_ReturnsNameInvalidAndAddsNothing()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateParentValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyFullName_ReportsErrorOnFullName()`
- Line 17: `public void Validate_EmptyPhone_ReportsErrorOnPhone()`
- Line 26: `public void Validate_ValidInput_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task HandleAsync_Valid_AddsSubjectWithActorsCentreAndReturnsId()`
- Line 35: `public async Task HandleAsync_DuplicateDifferingByCaseAndSpaces_ReturnsNameTakenAndAddsNothing()`
- Line 56: `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateSubjectValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyName_ReportsErrorOnName()`
- Line 17: `public void Validate_NameOverEightyCharacters_ReportsErrorOnName()`
- Line 26: `public void Validate_ValidName_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_Valid_AddsTeacherWithActorsCentreAndReturnsId()`
- Line 31: `public async Task HandleAsync_TwoTeachersSharingAName_BothSucceed()`
- Line 47: `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndAddsNothing()`
- Line 62: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndAddsNothing()`

### `tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyName_ReportsErrorOnFullName()`
- Line 17: `public void Validate_NameOver120Characters_ReportsErrorOnFullName()`
- Line 26: `public void Validate_PhoneOverRawLength_ReportsErrorOnPhone()`
- Line 35: `public void Validate_ValidWithNoPhone_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/EndScheduleRuleHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_UnknownRule_ReturnsRuleNotFound()`
- Line 26: `public async Task HandleAsync_EndsTheRuleAndCancelsOnlyTheSessionsAfterTheLastDate()`
- Line 41: `public async Task HandleAsync_ASessionAfterTheLastDateThatHasStarted_IsLeftScheduled()`
- Line 54: `public async Task HandleAsync_AlreadyEndedRule_ReturnsRuleAlreadyEndedAndCancelsNothing()`
- Line 67: `public async Task HandleAsync_LastDateBeforeTheRuleTakesEffect_ReturnsRuleEndInvalid()`

### `tests/TutoringCentre.Application.Tests/Academics/EnrollStudentHandlerTests.cs`

Branching of the enroll handler against fakes: the order of checks and the code each one answers. The fake group repository reports a configurable active count and takes no lock; the lock itself is proven against a real database (LastSeatConcurrencyTests).

Declared test entry points (declarations are not execution results):

- Line 20: `public async Task HandleAsync_UnknownGroup_ReturnsGroupNotFound()`
- Line 30: `public async Task HandleAsync_UnknownGroupAndUnknownStudent_ReturnsGroupNotFoundBeforeAnythingAboutTheStudent()`
- Line 40: `public async Task HandleAsync_UnknownStudent_ReturnsStudentNotFound()`
- Line 50: `public async Task HandleAsync_ArchivedStudent_ReturnsStudentArchived()`
- Line 61: `public async Task HandleAsync_AlreadyEnrolled_ReturnsAlreadyEnrolled()`
- Line 72: `public async Task HandleAsync_CountEqualToCapacity_ReturnsGroupFull()`
- Line 83: `public async Task HandleAsync_ArchivedGroup_ReturnsGroupArchived()`
- Line 94: `public async Task HandleAsync_LastSeat_AddsOneActiveEnrollmentInTheActorsCentreAtTheClocksTime()`

### `tests/TutoringCentre.Application.Tests/Academics/EnrollmentDispatcherPermissionTests.cs`

Through the real dispatcher: tenancy and permission. Owner and Secretary hold enrollments.manage; Teacher holds only enrollments.view, so the three commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every enrollment request as tenant.not_selected.

Declared test entry points (declarations are not execution results):

- Line 41: `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- Line 56: `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- Line 70: `public async Task SendAsync_Enroll_SucceedsForOwnerAndSecretary(StaffRole role)`
- Line 81: `public async Task QueryAsync_RosterAndStudentEnrollments_AreAllowedForTeacher()`
- Line 96: `public async Task QueryAsync_RosterAndStudentEnrollments_ReturnTenantNotSelectedForACentrelessActor()`

### `tests/TutoringCentre.Application.Tests/Academics/EnrollmentQueryHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task GetRoster_GroupOutsideTheTeachersScope_ReturnsGroupNotFound()`
- Line 33: `public async Task GetRoster_OwnGroup_ReturnsTheRoster()`
- Line 47: `public async Task ListStudentEnrollments_UnknownStudent_ReturnsStudentNotFound()`
- Line 59: `public async Task ListStudentEnrollments_KnownStudentWithOnlyOutOfScopeEnrollments_ReturnsAnEmptyList()`

### `tests/TutoringCentre.Application.Tests/Academics/ExtendScheduleHandlerTests.cs`

One Saturday rule whose first four weeks (6-27 March) already exist, as after adding it.

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_ThroughBeforeToday_ReturnsRangeInvalidOnThrough()`
- Line 24: `public async Task HandleAsync_ThroughMoreThan92DaysAhead_ReturnsRangeInvalidOnThrough()`
- Line 34: `public async Task HandleAsync_ThroughExactly92DaysAhead_IsAccepted()`
- Line 44: `public async Task HandleAsync_AddsOnlyTheDatesThatHaveNoSessionYet()`
- Line 56: `public async Task HandleAsync_RunTwice_TheSecondRunCreatesNothing()`
- Line 71: `public async Task HandleAsync_NewSessionsGoToTheGroupsCurrentTeacher()`
- Line 83: `public async Task HandleAsync_TakesTheTeachersScheduleLocksBeforeAddingAnySession()`

### `tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 24: `public async Task HandleAsync_KnownId_ReturnsDetail()`

### `tests/TutoringCentre.Application.Tests/Academics/GetStudentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task HandleAsync_Known_ReturnsDetail()`
- Line 25: `public async Task HandleAsync_Unknown_ReturnsStudentNotFound()`

### `tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_KnownId_ReturnsDto()`

### `tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_KnownId_ReturnsDto()`

### `tests/TutoringCentre.Application.Tests/Academics/GroupDispatcherPermissionTests.cs`

Through the real dispatcher: tenancy and permission, the first two of the three checks. Owner and Secretary hold groups.manage; Teacher holds only groups.view, so all six commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every group request as tenant.not_selected.

Declared test entry points (declarations are not execution results):

- Line 46: `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- Line 61: `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- Line 75: `public async Task SendAsync_CreateGroup_SucceedsForOwnerAndSecretary(StaffRole role)`
- Line 86: `public async Task QueryAsync_ListAndGet_AreAllowedForTeacher()`
- Line 100: `public async Task QueryAsync_ListAndGet_ReturnTenantNotSelectedForACentrelessActor()`

### `tests/TutoringCentre.Application.Tests/Academics/GroupQueryHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_PassesTheProvidersScopeAndTheFiltersToTheReadService()`
- Line 30: `public async Task HandleAsync_OwnScope_ReturnsOnlyThatTeachersGroups()`
- Line 48: `public async Task HandleAsync_InScope_ReturnsTheGroup()`
- Line 62: `public async Task HandleAsync_OutOfScope_ReturnsNotFound()`
- Line 75: `public async Task HandleAsync_NoneScope_ReturnsNotFound()`
- Line 88: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`

### `tests/TutoringCentre.Application.Tests/Academics/GroupValidatorTests.cs`

Shape checks only; the real name and capacity rules live in Group and are tested there.

Declared test entry points (declarations are not execution results):

- Line 16: `public void CreateGroup_Valid_Passes()`
- Line 24: `public void CreateGroup_EmptyNameAndIds_FailsOnEachField()`
- Line 34: `public void CreateGroup_CapacityOutOfRange_IsLeftToTheEntity()`
- Line 42: `public void RenameGroup_EmptyIdAndName_FailsOnBoth()`
- Line 50: `public void RenameGroup_NameOver80_Fails()`
- Line 58: `public void ChangeGroupCapacity_EmptyId_Fails()`
- Line 66: `public void AssignGroupTeacher_EmptyIds_FailsOnBoth()`
- Line 74: `public void ArchiveGroup_EmptyId_Fails()`
- Line 82: `public void RestoreGroup_EmptyId_Fails()`

### `tests/TutoringCentre.Application.Tests/Academics/ListStudentsHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task HandleAsync_PassesCriteriaThroughAndReturnsThePage()`

### `tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_PageSizeZero_ReportsErrorOnPageSize()`
- Line 17: `public void Validate_PageSizeAboveHundred_ReportsErrorOnPageSize()`
- Line 26: `public void Validate_GarbageCursor_ReportsErrorOnCursor()`
- Line 35: `public void Validate_NoCursorAndValidPageSize_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/ListSubjectsHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)`
- Line 24: `public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()`

### `tests/TutoringCentre.Application.Tests/Academics/ListTeachersHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_PassesIncludeArchivedFlagThrough(bool includeArchived)`
- Line 24: `public async Task HandleAsync_ReturnsWhatTheReadServiceProvides()`

### `tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task HandleAsync_FoundByDifferentSpelling_ReturnsTheParent()`
- Line 25: `public async Task HandleAsync_NoParentWithThisPhone_SucceedsWithNull()`
- Line 37: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`

### `tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs`

Through the real dispatcher: Teacher is refused students.manage/students.view for all four parent requests, and a centre-less actor is refused all four before any handler runs — layer 1 (tenant scope) and the permission step, both proven before any transaction, mirroring Subject's Day 24/28 proofs.

Declared test entry points (declarations are not execution results):

- Line 29: `public async Task SendAsync_CreateParent_ReturnsPermissionDeniedForTeacher()`
- Line 41: `public async Task SendAsync_UpdateParent_ReturnsPermissionDeniedForTeacher()`
- Line 52: `public async Task QueryAsync_GetParent_ReturnsPermissionDeniedForTeacher()`
- Line 63: `public async Task QueryAsync_LookupParentByPhone_ReturnsPermissionDeniedForTeacher()`
- Line 76: `public async Task SendAsync_CreateParent_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 87: `public async Task SendAsync_CreateParent_ReturnsTenantNotSelectedForCentrelessActor()`
- Line 99: `public async Task SendAsync_UpdateParent_ReturnsTenantNotSelectedForCentrelessActor()`
- Line 110: `public async Task QueryAsync_GetParent_ReturnsTenantNotSelectedForCentrelessActor()`
- Line 121: `public async Task QueryAsync_LookupParentByPhone_ReturnsTenantNotSelectedForCentrelessActor()`

### `tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task LinkParent_ExistingParentNotYetLinked_Succeeds()`
- Line 32: `public async Task LinkParent_UnknownParentId_ReturnsParentNotFound()`
- Line 46: `public async Task LinkParent_AlreadyLinked_ReturnsAlreadyLinked()`
- Line 63: `public async Task UnlinkParent_NonPrimarySecondParent_Succeeds()`
- Line 79: `public async Task UnlinkParent_LastParent_ReturnsLastParentRule()`
- Line 94: `public async Task UnlinkParent_NotLinked_ReturnsNotLinked()`
- Line 108: `public async Task SetPrimaryParent_AlreadyLinkedSecondParent_BecomesPrimary()`
- Line 124: `public async Task SetPrimaryParent_NotLinked_ReturnsNotLinked()`

### `tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task HandleAsync_ExistingParentForm_AddsStudentLinkedToThatParent()`
- Line 33: `public async Task HandleAsync_NewParentForm_AddsBothParentAndStudent()`
- Line 50: `public async Task HandleAsync_UnknownExistingParentId_ReturnsParentNotFoundAndAddsNothing()`
- Line 68: `public async Task HandleAsync_NewParentPhoneAlreadyTaken_ReturnsParentPhoneTakenWithNestedFieldAndAddsNothing()`
- Line 86: `public async Task HandleAsync_NewParentInvalidPhone_ReturnsNestedParentPhoneFieldAndAddsNothing()`
- Line 102: `public async Task HandleAsync_StudentNameInvalid_AfterNewParentWasStaged_StudentIsNeverAdded()`
- Line 120: `public async Task HandleAsync_StudentOwnPhoneInvalid_ReturnsTopLevelPhoneFieldAndAddsNothing()`

### `tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 9: `public void Validate_ExistingParentIdAlone_HasNoErrors()`
- Line 17: `public void Validate_NewParentNameAndPhoneTogether_HasNoErrors()`
- Line 25: `public void Validate_NeitherFormGiven_ReportsErrorOnParent()`
- Line 34: `public void Validate_BothFormsGivenAtOnce_ReportsErrorOnParent()`
- Line 43: `public void Validate_NewParentNameOnlyMissingPhone_ReportsErrorOnParentAndParentPhone()`
- Line 53: `public void Validate_EmptyStudentName_ReportsErrorOnFullName()`
- Line 64: `public void Validate_UndefinedRelationship_ReportsErrorOnRelationship()`

### `tests/TutoringCentre.Application.Tests/Academics/RenameGroupHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_NewUniqueName_Renames()`
- Line 39: `public async Task HandleAsync_SameName_SucceedsWithNoChange()`
- Line 53: `public async Task HandleAsync_NameUsedByAnotherGroup_ReturnsNameTaken()`
- Line 71: `public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()`

### `tests/TutoringCentre.Application.Tests/Academics/RenameSubjectHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 27: `public async Task HandleAsync_NewNameAlreadyTakenByAnotherSubject_ReturnsNameTaken()`
- Line 44: `public async Task HandleAsync_SameName_SucceedsWithNoChange()`
- Line 58: `public async Task HandleAsync_ArchivedSubject_ReturnsArchivedReadOnly()`
- Line 73: `public async Task HandleAsync_ValidNewName_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Academics/RenameSubjectValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyId_ReportsErrorOnSubjectId()`
- Line 17: `public void Validate_NameOverEightyCharacters_ReportsErrorOnName()`
- Line 26: `public void Validate_ValidCommand_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/RestoreGroupHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 25: `public async Task HandleAsync_SubjectArchived_ReturnsSubjectArchivedAndStaysArchived()`
- Line 39: `public async Task HandleAsync_TeacherArchived_ReturnsTeacherArchivedAndStaysArchived()`
- Line 53: `public async Task HandleAsync_BothReferencesActive_Restores()`
- Line 64: `public async Task HandleAsync_NotArchived_ReturnsNotArchived()`

### `tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 26: `public async Task HandleAsync_ActiveSubject_ReturnsNotArchived()`
- Line 40: `public async Task HandleAsync_ArchivedSubject_BecomesActive()`

### `tests/TutoringCentre.Application.Tests/Academics/RestoreSubjectValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyId_ReportsErrorOnSubjectId()`
- Line 17: `public void Validate_ValidCommand_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 26: `public async Task HandleAsync_ArchivedTeacher_Restores()`
- Line 41: `public async Task HandleAsync_NotArchived_ReturnsNotArchived()`

### `tests/TutoringCentre.Application.Tests/Academics/RestoreTeacherValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- Line 17: `public void Validate_ValidId_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/ScheduleDispatcherPermissionTests.cs`

Through the real dispatcher: tenancy and permission. Owner and Secretary hold schedule.manage; Teacher holds only schedule.view, so the four commands are denied for Teacher before any handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is refused every schedule request as tenant.not_selected.

Declared test entry points (declarations are not execution results):

- Line 42: `public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)`
- Line 58: `public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)`
- Line 72: `public async Task SendAsync_AddRule_SucceedsForOwnerAndSecretary(StaffRole role)`
- Line 83: `public async Task QueryAsync_TimetableAndRules_AreAllowedForTeacher()`
- Line 98: `public async Task QueryAsync_TimetableAndRules_ReturnTenantNotSelectedForACentrelessActor()`

### `tests/TutoringCentre.Application.Tests/Academics/ScheduleQueryHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task GetTimetable_ReturnsTheCentresZoneIdAndWeekStart_AndPassesTheScopeAndFilters()`
- Line 33: `public async Task GetTimetable_OwnScope_ReturnsOnlyTheTeachersOwnSessions()`
- Line 48: `public async Task ListGroupScheduleRules_GroupInScope_ReturnsItsRules()`
- Line 64: `public async Task ListGroupScheduleRules_GroupOutOfScope_ReturnsGroupNotFound()`

### `tests/TutoringCentre.Application.Tests/Academics/ScheduleTestData.cs`

Fixed dates for the schedule handler tests (never the real date). "Now" is Monday 1 March 2027, 10:00 in Cairo (08:00 UTC, winter time UTC+2), so "today" in the centre is 1 March and the four-week horizon ends on 28 March. Sessions only come from the real , the one way to obtain them.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs`

The link is the security-relevant part of Task 43.8, so most cases here target its refusals rather than the happy path alone: an unknown user, a user of another centre, a user with another role and an inactive member must all answer teacher.user_not_eligible identically, and a user already linked elsewhere must answer teacher.account_taken.

Declared test entry points (declarations are not execution results):

- Line 21: `public async Task HandleAsync_UnknownTeacherId_ReturnsNotFound()`
- Line 33: `public async Task HandleAsync_NullUserId_Unlinks()`
- Line 48: `public async Task HandleAsync_EligibleUser_Links()`
- Line 63: `public async Task HandleAsync_UnknownUserId_ReturnsUserNotEligible()`
- Line 77: `public async Task HandleAsync_UserOfAnotherCentre_ReturnsUserNotEligible()`
- Line 92: `public async Task HandleAsync_UserWithAnotherRole_ReturnsUserNotEligible()`
- Line 106: `public async Task HandleAsync_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()`
- Line 128: `public async Task HandleAsync_RelinkingTheSameTeacherToItsOwnUser_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- Line 17: `public void Validate_NullUserId_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs`

Through the real dispatcher: Teacher is refused students.manage/students.view for all nine student requests, and a centre-less actor is refused all nine before any handler runs — layer 1 (tenant scope) and the permission step, both proven before any transaction, mirroring Parent's Day 39 proofs.

Declared test entry points (declarations are not execution results):

- Line 36: `public async Task EveryManageCommand_ReturnsPermissionDeniedForTeacher(StaffRole role)`
- Line 64: `public async Task QueryAsync_GetStudent_ReturnsPermissionDeniedForTeacher()`
- Line 75: `public async Task QueryAsync_ListStudents_ReturnsPermissionDeniedForTeacher()`
- Line 88: `public async Task SendAsync_RegisterStudent_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 100: `public async Task SendAsync_RegisterStudent_ReturnsTenantNotSelectedForCentrelessActor()`
- Line 113: `public async Task QueryAsync_GetStudent_ReturnsTenantNotSelectedForCentrelessActor()`

### `tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs`

Task 28.6: through the real dispatcher, Teacher is read-only at the use-case level for all six Subject requests, while Secretary and Owner can do everything. Closes the Day 24 authorization gap.

Declared test entry points (declarations are not execution results):

- Line 32: `public async Task SendAsync_CreateSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 43: `public async Task SendAsync_CreateSubject_ReturnsPermissionDeniedForTeacher()`
- Line 57: `public async Task SendAsync_RenameSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 68: `public async Task SendAsync_RenameSubject_ReturnsPermissionDeniedForTeacher()`
- Line 81: `public async Task SendAsync_ArchiveSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 92: `public async Task SendAsync_ArchiveSubject_ReturnsPermissionDeniedForTeacher()`
- Line 105: `public async Task SendAsync_RestoreSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 116: `public async Task SendAsync_RestoreSubject_ReturnsPermissionDeniedForTeacher()`
- Line 130: `public async Task QueryAsync_ListSubjects_SucceedsForEveryRole(StaffRole role)`
- Line 144: `public async Task QueryAsync_GetSubject_SucceedsForEveryRole(StaffRole role)`

### `tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs`

Task 24.9: the layer 1 proof for this feature — through the real dispatcher, a centre-less staff actor is refused for every one of the six Subject requests, and the handler (and therefore the fake ports behind it) is never invoked.

Declared test entry points (declarations are not execution results):

- Line 29: `public async Task SendAsync_CreateSubject_ReturnsTenantNotSelected()`
- Line 41: `public async Task SendAsync_RenameSubject_ReturnsTenantNotSelected()`
- Line 52: `public async Task SendAsync_ArchiveSubject_ReturnsTenantNotSelected()`
- Line 63: `public async Task SendAsync_RestoreSubject_ReturnsTenantNotSelected()`
- Line 74: `public async Task QueryAsync_ListSubjects_ReturnsTenantNotSelected()`
- Line 85: `public async Task QueryAsync_GetSubject_ReturnsTenantNotSelected()`

### `tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs`

Through the real dispatcher: only Owner holds teachers.manage, so the five commands succeed for Owner and are denied for both Secretary and Teacher. Both Owner and Secretary hold teachers.view, so the two queries succeed for both and are denied only for Teacher.

Declared test entry points (declarations are not execution results):

- Line 33: `public async Task SendAsync_CreateTeacher_SucceedsForOwner()`
- Line 46: `public async Task SendAsync_CreateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 58: `public async Task SendAsync_UpdateTeacher_SucceedsForOwner()`
- Line 71: `public async Task SendAsync_UpdateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 82: `public async Task SendAsync_SetTeacherAccount_SucceedsForOwner()`
- Line 95: `public async Task SendAsync_SetTeacherAccount_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 106: `public async Task SendAsync_ArchiveTeacher_SucceedsForOwner()`
- Line 119: `public async Task SendAsync_ArchiveTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 130: `public async Task SendAsync_RestoreTeacher_SucceedsForOwner()`
- Line 143: `public async Task SendAsync_RestoreTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 156: `public async Task QueryAsync_ListTeachers_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 167: `public async Task QueryAsync_ListTeachers_ReturnsPermissionDeniedForTeacher()`
- Line 180: `public async Task QueryAsync_GetTeacher_SucceedsForSecretaryAndOwner(StaffRole role)`
- Line 191: `public async Task QueryAsync_GetTeacher_ReturnsPermissionDeniedForTeacher()`

### `tests/TutoringCentre.Application.Tests/Academics/TransferEnrollmentHandlerTests.cs`

Branching of the transfer handler against fakes. Every failure asserts both effects are absent together — the old enrollment still active and untouched, and no new enrollment added — not each one on its own.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()`
- Line 29: `public async Task HandleAsync_EndedEnrollment_ReturnsAlreadyEnded()`
- Line 43: `public async Task HandleAsync_SameGroup_ReturnsSameGroup()`
- Line 53: `public async Task HandleAsync_UnknownTarget_ReturnsGroupNotFound()`
- Line 63: `public async Task HandleAsync_TargetOfAnotherSubject_ReturnsSubjectMismatch()`
- Line 75: `public async Task HandleAsync_StudentAlreadyInTarget_ReturnsAlreadyEnrolled()`
- Line 86: `public async Task HandleAsync_TargetFull_ReturnsGroupFullAndChangesNothing()`
- Line 97: `public async Task HandleAsync_Success_EndsTheOldAsTransferredAndAddsANewActiveEnrollmentInTheTarget()`

### `tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 27: `public async Task HandleAsync_SamePhone_SucceedsWithoutRunningAUniquenessQuery()`
- Line 43: `public async Task HandleAsync_PhoneChangedAndTakenByAnother_ReturnsPhoneTaken()`
- Line 62: `public async Task HandleAsync_PhoneChangedToAFreeOne_Succeeds()`
- Line 77: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`

### `tests/TutoringCentre.Application.Tests/Academics/UpdateStudentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_Valid_ReplacesAllThreeFields()`
- Line 30: `public async Task HandleAsync_OmittedPhoneAndSchoolYear_ClearsBoth()`
- Line 45: `public async Task HandleAsync_UnknownStudentId_ReturnsStudentNotFound()`
- Line 56: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()`
- Line 70: `public async Task HandleAsync_ArchivedStudent_ReturnsArchivedReadOnly()`

### `tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 27: `public async Task HandleAsync_Valid_UpdatesNameAndPhone()`
- Line 43: `public async Task HandleAsync_ArchivedTeacher_ReturnsArchivedReadOnly()`
- Line 59: `public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidOnPhoneFieldAndLeavesTeacherUnchanged()`

### `tests/TutoringCentre.Application.Tests/Academics/UpdateTeacherValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()`
- Line 17: `public void Validate_EmptyName_ReportsErrorOnFullName()`
- Line 26: `public void Validate_Valid_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Academics/WithdrawEnrollmentHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_UnknownEnrollment_ReturnsEnrollmentNotFound()`
- Line 25: `public async Task HandleAsync_AlreadyEnded_ReturnsAlreadyEndedAndKeepsTheFirstEnd()`
- Line 44: `public async Task HandleAsync_ActiveEnrollment_EndsItAsWithdrawnAtTheClocksTime()`

### `tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs`

Through the real dispatcher: Secretary and Teacher are refused (auth.permission_denied), and a centre-less actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline guarantees proved for Staff and Subjects.

Declared test entry points (declarations are not execution results):

- Line 26: `public async Task QueryAsync_GetAuditLog_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 39: `public async Task QueryAsync_GetAuditLog_CentrelessActor_ReturnsTenantNotSelected()`

### `tests/TutoringCentre.Application.Tests/Audit/GetAuditLogHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 10: `public async Task HandleAsync_PassesEveryFilterThroughUnchanged()`
- Line 33: `public async Task HandleAsync_ReturnsThePageFromTheReadService()`

### `tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void Validate_WithEveryFilterOmitted_Succeeds()`
- Line 21: `public void Validate_WithPageSizeOutOfRange_Fails(int pageSize)`
- Line 32: `public void Validate_WithPageSizeAtBoundary_Succeeds(int pageSize)`
- Line 40: `public void Validate_WithFromAfterTo_Fails()`
- Line 50: `public void Validate_WithFromEqualToTo_Succeeds()`
- Line 59: `public void Validate_WithUnknownEntityType_Fails()`
- Line 87: `public void Validate_WithEntityIdButNoEntityType_Fails()`
- Line 96: `public void Validate_WithEntityIdAndEntityType_Succeeds()`
- Line 104: `public void Validate_WithGarbageCursor_Fails()`
- Line 113: `public void Validate_WithWellFormedCursor_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Centres/CentreCalendarTests.cs`

Task 51.6: "today" is the date in the centre's zone, not the UTC or server date.

Declared test entry points (declarations are not execution results):

- Line 12: `public void Today_At2230UtcOn31December_IsAlready1JanuaryInCairo()`
- Line 21: `public void Today_At2159UtcOn31December_IsStill31DecemberInCairo()`

### `tests/TutoringCentre.Application.Tests/Centres/CentreSettingsDispatcherAuthorizationTests.cs`

Through the real dispatcher: Secretary and Teacher are refused both centre-settings requests (auth.permission_denied), and a centre-less actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline guarantees proved for Audit, Staff and Subjects.

Declared test entry points (declarations are not execution results):

- Line 28: `public async Task QueryAsync_GetCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 41: `public async Task SendAsync_UpdateCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 52: `public async Task QueryAsync_GetCentreSettings_CentrelessActor_ReturnsTenantNotSelected()`
- Line 63: `public async Task SendAsync_UpdateCentreSettings_CentrelessActor_ReturnsTenantNotSelected()`

### `tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task HandleAsync_AnonymousActor_ReturnsForbiddenAndDoesNotAdd()`
- Line 32: `public async Task HandleAsync_SlugAlreadyExists_ReturnsConflictAndDoesNotAdd()`
- Line 53: `public async Task HandleAsync_InvalidSlug_ReturnsDomainErrorAndDoesNotAdd()`
- Line 72: `public async Task HandleAsync_SystemActorValidInput_AddsCentreAndReturnsResult()`

### `tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 9: `public void Validate_EmptyName_ReportsErrorOnName()`
- Line 23: `public void Validate_SlugOverSixtyCharacters_ReportsErrorOnSlug()`
- Line 37: `public void Validate_ValidCommand_HasNoErrors()`

### `tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_ReturnsTheActorsOwnCentreSettings()`
- Line 29: `public async Task HandleAsync_MissingCentre_Throws()`

### `tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task HandleAsync_ValidUpdate_ChangesNameAndLocale()`
- Line 29: `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndChangesNothing()`
- Line 47: `public async Task HandleAsync_MissingCentre_Throws()`

### `tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsValidatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void Validate_WithValidInput_Succeeds()`
- Line 19: `public void Validate_WithEmptyName_Fails()`
- Line 28: `public void Validate_With121CharacterName_Fails()`
- Line 37: `public void Validate_WithUnknownLocale_Fails()`

### `tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs`

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task SendAsync_ValidCommandHandlerSucceeds_BeginsHandlesSavesAndCommits()`
- Line 31: `public async Task SendAsync_InvalidCommand_DoesNotBeginOrCallHandler()`
- Line 46: `public async Task SendAsync_HandlerReturnsFailure_RollsBackWithoutSavingOrCommitting()`
- Line 58: `public async Task SendAsync_HandlerThrows_RollsBackAndRethrows()`
- Line 69: `public async Task SendAsync_SaveChangesThrows_RollsBackAndDoesNotCommit()`
- Line 81: `public async Task SendAsync_SaveFails_RollsBackWithoutCommittingAndReturnsTheSaveError()`
- Line 96: `public async Task QueryAsync_ValidQuery_BeginsReadOnlyHandlesAndCommitsWithoutSaving()`
- Line 108: `public async Task QueryAsync_InvalidQuery_DoesNotBeginOrCallHandler()`
- Line 120: `public async Task QueryAsync_HandlerThrows_RollsBackAndRethrows()`
- Line 131: `public async Task SendAsync_SeveralFieldFailures_GroupsMessagesByCamelCaseFieldName()`
- Line 144: `public async Task SendAsync_TenantScopedCommandStaffActorWithoutCentre_ReturnsTenantNotSelected()`
- Line 160: `public async Task QueryAsync_TenantScopedQueryStaffActorWithoutCentre_ReturnsTenantNotSelected()`
- Line 176: `public async Task SendAsync_TenantScopedCommandAnonymousActor_ReturnsNotAuthenticated()`
- Line 191: `public async Task QueryAsync_TenantScopedQueryAnonymousActor_ReturnsNotAuthenticated()`
- Line 206: `public async Task SendAsync_TenantScopedCommandActorWithCentre_InvokesHandler()`
- Line 220: `public async Task QueryAsync_TenantScopedQueryActorWithCentre_InvokesHandler()`
- Line 234: `public async Task SendAsync_NonScopedRequestActorWithoutCentre_InvokesHandler()`
- Line 247: `public async Task SendAsync_TenantScopedCommandInvalidAndActorWithoutCentre_ValidationWinsOverTenantStep()`
- Line 262: `public async Task SendAsync_StaffActorWithThePermission_InvokesHandler()`
- Line 276: `public async Task SendAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()`
- Line 292: `public async Task QueryAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()`
- Line 308: `public async Task SendAsync_SystemActor_IsAllowedRegardlessOfPermission()`
- Line 322: `public async Task SendAsync_PermissionRequiringCommandActorWithoutCentre_TenantStepWins()`
- Line 337: `public async Task QueryAsync_PermissionRequiringQueryActorWithoutCentre_TenantStepWins()`

### `tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs`

Tells the test handlers how to behave in the current test.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeAuditReadService.cs`

In-memory stand-in for audit reads. Records every filter it was last asked for, so a handler test can assert they were passed through unchanged.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreReadService.cs`

In-memory stand-in for centre settings reads.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs`

In-memory centre repository. simulates stored rows; records Add calls.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeCentreTimeZoneProvider.cs`

A settable stand-in for ; starts at Cairo, the zone every seeded centre uses.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeClassSessionRepository.cs`

In-memory class-session repository. simulates stored rows (built with the real SessionGenerator, the only way to obtain a session); records AddRange calls. No overlap check: the exclusion constraints are proven against a real database, not here.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeClock.cs`

A fixed "now" for handlers that stamp times; time-zone conversion is not needed by any handler test.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeCurrentActor.cs`

A settable stand-in for ; starts anonymous like the real context does.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentReadService.cs`

In-memory stand-in for enrollment reads. Each row carries its group's teacher, so the scope is applied the way the real adapter applies it (rows outside it are invisible); records the last scope it was asked for.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeEnrollmentRepository.cs`

In-memory enrollment repository. simulates stored rows; records Add calls.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeGroupReadService.cs`

In-memory stand-in for group reads. Applies the scope the way the real adapter does (rows outside it are invisible) and records the last criteria and scope it was asked for, so a handler test can assert both were passed through.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeGroupRepository.cs`

In-memory group repository. simulates stored rows; records Add calls; is the count reports (0 when absent). No lock: concurrency is proven against a real database, not here.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs`

In-memory stand-in for the three canned responses a handler test needs.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipRepository.cs`

In-memory stand-in for the staff write-side port. seeds existing rows for any number of centres; every method honours the centre argument it is given, so a scoping bug in a handler shows up as a wrong-centre test failing instead of passing by accident.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs`

In-memory stand-in for parent reads.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs`

In-memory parent repository. simulates stored rows; records Add calls. lets a test prove the uniqueness query never runs when the phone is unchanged on update.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleReadService.cs`

In-memory stand-in for schedule reads. Each group's rules carry the group's teacher and each session its own teacher, so the scope is applied the way the real adapter applies it; records the last scope and filters asked for.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeScheduleRuleRepository.cs`

In-memory schedule-rule repository. simulates stored rows; records Add calls. stands in for the join to active groups that makes; a rule whose group is not listed there is treated as belonging to an archived group.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffAccountService.cs`

In-memory stand-in for account creation. seeds accounts that already exist (normalized email → user ID); any other email is "created" with a fake temporary password.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffDirectory.cs`

In-memory stand-in for Identity's published staff-directory contract. holds (centreId, userId, role) tuples for every active membership the test wants to simulate; anything not in the list answers false, the same single answer the real adapter gives for an unknown user, a user of another centre and a user with another role.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeStaffReadService.cs`

Records the centre and current-user arguments the handler passed, so a test can prove they came from the actor, not a parameter.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeStudentReadService.cs`

In-memory stand-in for student reads.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeStudentRepository.cs`

In-memory student repository. simulates stored rows; records Add calls.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectReadService.cs`

In-memory stand-in for subject reads. Records the last includeArchived flag it was asked for, so a handler test can assert it was passed through.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeSubjectRepository.cs`

In-memory subject repository. simulates stored rows; records Add calls.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherReadService.cs`

In-memory stand-in for teacher reads. Records the last includeArchived flag it was asked for, so a handler test can assert it was passed through.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherRepository.cs`

In-memory teacher repository. simulates stored rows; records Add calls.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeTeacherScopeProvider.cs`

A settable stand-in for ; starts at None, failing closed like the real one.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs`

A working in-memory unit of work that records every call, in order. Use one instance per test.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()`
- Line 29: `public async Task HandleAsync_NoActiveMembershipInCentre_ReturnsTenantNoMembership()`
- Line 47: `public async Task HandleAsync_MustChangePasswordPending_ReturnsPasswordChangeRequiredWithoutCheckingMembership()`
- Line 70: `public async Task HandleAsync_ActiveMembershipInCentre_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()`
- Line 29: `public async Task HandleAsync_NoProfileForActor_ReturnsNotAuthenticated()`
- Line 47: `public async Task HandleAsync_ActorCentreStillActive_ReportsActiveCentreAndRole()`
- Line 73: `public async Task HandleAsync_SecretaryWithActiveCentre_ReportsSortedPermissions()`
- Line 98: `public async Task HandleAsync_ActorCentreNoLongerActive_DropsActiveCentreAndRole()`

### `tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_UnknownUser_ReturnsFalse()`
- Line 30: `public async Task HandleAsync_WrongSecurityStamp_ReturnsFalse()`
- Line 45: `public async Task HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse()`
- Line 60: `public async Task HandleAsync_RoleNoLongerMatchesTheMembership_ReturnsFalse()`
- Line 75: `public async Task HandleAsync_MatchingStampAndNoCentre_ReturnsTrue()`
- Line 90: `public async Task HandleAsync_MatchingStampActiveCentreMembershipAndRole_ReturnsTrue()`

### `tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 9: `public async Task HandleAsync_NoPendingMigrations_ReportsDatabaseUpToDate()`
- Line 23: `public async Task HandleAsync_TwoPendingMigrations_ReportsDatabaseNotUpToDate()`
- Line 36: `public async Task HandleAsync_AnyStatus_ReturnsNonEmptyVersion()`
- Line 54: `public async Task HandleAsync_ReportsDemoModeFromProvider(bool demoEnabled)`

### `tests/TutoringCentre.Application.Tests/Security/CorrelationContextTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void CorrelationId_ByDefault_IsNull()`
- Line 16: `public void Set_ThenCorrelationId_ReturnsTheSetValue()`
- Line 26: `public void Set_CalledTwice_ThrowsInvalidOperationException()`

### `tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs`

Declared test entry points (declarations are not execution results):

- Line 9: `public void Actor_ByDefault_IsAnonymous()`
- Line 17: `public void Set_ThenActor_ReturnsTheSetActor()`
- Line 31: `public void Set_CalledTwice_ThrowsInvalidOperationException()`
- Line 40: `public void Reauthenticate_AfterSet_ReplacesTheActor()`
- Line 56: `public void Reauthenticate_ThenSet_ThrowsInvalidOperationException()`

### `tests/TutoringCentre.Application.Tests/Security/TeacherScopeTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void All_AllowsAnyTeacher()`
- Line 14: `public void None_AllowsNoTeacher()`
- Line 20: `public void Own_AllowsExactlyItsTeacher()`

### `tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task HandleAsync_UnknownId_ReturnsNotFound()`
- Line 27: `public async Task HandleAsync_MembershipBelongsToAnotherCentre_ReturnsNotFound()`
- Line 42: `public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()`
- Line 57: `public async Task HandleAsync_DemotingTheOnlyActiveOwner_ReturnsLastOwner()`
- Line 73: `public async Task HandleAsync_DemotingOneOfTwoActiveOwners_Succeeds()`
- Line 90: `public async Task HandleAsync_SameRole_ReturnsRoleUnchanged()`

### `tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_NewEmail_AddsMembershipInTheActorsCentreAndReturnsATemporaryPassword()`
- Line 31: `public async Task HandleAsync_ExistingAccount_AddsMembershipWithNoTemporaryPassword()`
- Line 49: `public async Task HandleAsync_AlreadyAnActiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()`
- Line 68: `public async Task HandleAsync_AlreadyAnInactiveMemberOfThisCentre_ReturnsAlreadyMemberAndAddsNothing()`

### `tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_TargetIsTheActorsOwnMembership_ReturnsCannotChangeSelf()`
- Line 30: `public async Task HandleAsync_DeactivatingTheOnlyActiveOwner_ReturnsLastOwner()`
- Line 46: `public async Task HandleAsync_AlreadyInactive_ReturnsAlreadyInactive()`
- Line 62: `public async Task HandleAsync_ActiveTeacher_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task HandleAsync_PassesTheActorsCentreAndUserToTheReadService()`

### `tests/TutoringCentre.Application.Tests/Staff/ReactivateStaffHandlerTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task HandleAsync_AlreadyActive_ReturnsAlreadyActive()`
- Line 28: `public async Task HandleAsync_Inactive_Succeeds()`

### `tests/TutoringCentre.Application.Tests/Staff/StaffDispatcherAuthorizationTests.cs`

Task 29.11: through the real dispatcher, Secretary and Teacher are refused all five staff requests (auth.permission_denied), and a centre-less actor is refused all five before permission is even considered (tenant.not_selected) — the same two pipeline guarantees Day 28/22 already proved for Subjects.

Declared test entry points (declarations are not execution results):

- Line 30: `public async Task SendAsync_CreateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 43: `public async Task SendAsync_ChangeStaffRole_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 56: `public async Task SendAsync_DeactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 69: `public async Task SendAsync_ReactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 82: `public async Task QueryAsync_ListStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- Line 93: `public async Task SendAsync_CreateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- Line 104: `public async Task SendAsync_ChangeStaffRole_CentrelessActor_ReturnsTenantNotSelected()`
- Line 115: `public async Task SendAsync_DeactivateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- Line 126: `public async Task SendAsync_ReactivateStaff_CentrelessActor_ReturnsTenantNotSelected()`
- Line 137: `public async Task QueryAsync_ListStaff_CentrelessActor_ReturnsTenantNotSelected()`

### `tests/TutoringCentre.Application.Tests/Staff/StaffValidatorsTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public void Validate_InvalidEmail_ReportsErrorOnEmail()`
- Line 21: `public void Validate_EmailOverTwoHundredFiftySixCharacters_ReportsErrorOnEmail()`
- Line 31: `public void Validate_EmptyDisplayName_ReportsErrorOnDisplayName()`
- Line 40: `public void Validate_DisplayNameOverOneHundredTwentyCharacters_ReportsErrorOnDisplayName()`
- Line 49: `public void Validate_UndefinedRole_ReportsErrorOnRole()`
- Line 58: `public void Validate_UnsupportedLocale_ReportsErrorOnPreferredLocale()`
- Line 67: `public void Validate_ValidCommand_HasNoErrors()`
- Line 78: `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`
- Line 90: `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`
- Line 102: `public void Validate_EmptyMembershipId_ReportsErrorOnMembershipId()`

### `tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public void Endpoints_DoNotDependOnInfrastructureRepositoriesOrDomainEntities()`
- Line 52: `public void OnlyProgramAndCli_ReferenceInfrastructure()`

### `tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`

An interface named I*Repository (for example ICentreRepository).

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs`

Task 28.7: forgetting to declare a permission, or declaring a misspelt one, becomes a failing build. Rule 1 and 2 use reflection (NetArchTest-style, like ); rule 3 is a source scan (like ), since "does a handler branch on a role" is not something compiled-type inspection alone can see.

Declared test entry points (declarations are not execution results):

- Line 34: `public void EveryTenantScopedRequest_AlsoDeclaresIRequirePermission()`
- Line 45: `public void EveryRequirePermissionRequest_DeclaresAKeyFromTheCatalogue()`
- Line 61: `public void ApplicationCommandAndQueryHandlers_NeverBranchOnAStaffRoleValue()`

### `tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void EveryCommandAndQuery_HasExactlyOneHandler()`
- Line 28: `public void Handlers_AreSealedAndNotPublic()`
- Line 41: `public void QueryHandlers_DoNotDependOnRepositories()`

### `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`

Type-level dependency rule: fails when compiled code in a layer USES a forbidden namespace.

Declared test entry points (declarations are not execution results):

- Line 38: `public void Domain_DoesNotDependOnOtherLayersOrFrameworks() =>`
- Line 42: `public void Application_DoesNotDependOnInfrastructureApiOrFrameworks() =>`
- Line 46: `public void Infrastructure_DoesNotDependOnApi() =>`
- Line 50: `public void Application_DoesNotDependOnSerilogHangfireOrHttpClient()`

### `tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs`

Identity (UserManager, ApplicationUser, password hashing) is an Infrastructure concern. Application and the Api — other than the composition root (Program.cs) and the CLI (which wires up Identity at startup and for the development seeder) — see it only through Application's own ports (IAuthenticationService, IMembershipReadService), never directly.

Declared test entry points (declarations are not execution results):

- Line 18: `public void Application_DoesNotDependOnIdentity()`
- Line 31: `public void Api_OutsideCompositionRootAndCli_DoesNotDependOnIdentity()`

### `tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs`

Task 30.9: keeps the monolith modular without separate assemblies. Three modules exist today, by Application and Domain namespace: Platform (with Centres), Identity (with Staff), Academics. A module reaches another only through its requests (commands, queries, DTOs — explicitly allowed) or an explicit published interface in that module's own Contracts namespace (Task 43.3, e.g. TutoringCentre.Application.Identity.Contracts); it may never reach into another module's repository, read service, handler, validator or Domain entity directly. A Contracts namespace is itself narrow by construction: every type in one must be an interface or a record, and may not depend on its own module's repositories, read services or Domain entities — a contract publishes a question another module may ask, never a door into how it is answered.

Declared test entry points (declarations are not execution results):

- Line 53: `public void ApplicationTypes_DoNotDependOnAnotherModulesRepositoriesReadServicesHandlersOrValidators()`
- Line 59: `public void ApplicationTypes_DoNotDependOnAnotherModulesDomainTypes()`
- Line 65: `public void ContractTypes_AreInterfacesOrRecords()`
- Line 77: `public void ContractTypes_DoNotDependOnTheirOwnModulesPortsOrDomainTypes()`

### `tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs`

Reads the declared ProjectReference items of the src projects.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`

Declared-reference rule: fails when a .csproj DECLARES a reference outside the allowed set, even if no code uses it (the compiler drops unused references, so DependencyRuleTests can't see them).

Declared test entry points (declarations are not execution results):

- Line 14: `public void Domain_ReferencesNoProjects() =>`
- Line 18: `public void Application_ReferencesOnlyDomain() =>`
- Line 22: `public void Infrastructure_ReferencesOnlyApplicationAndDomain() =>`
- Line 26: `public void Api_ReferencesOnlyApplicationAndInfrastructure() =>`

### `tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs`

Source scan (Task 20.7, rule A): NetArchTest inspects compiled type dependencies, not method calls, so a call that bypasses the tenant query filter or runs raw SQL on a tenant table has to be caught by reading the actual .cs text. Everywhere outside the allow-list, these calls are forbidden; the allow-list exists because each listed place already proved it needs them (Day 19/20), not because the rule is inconvenient elsewhere.

Declared test entry points (declarations are not execution results):

- Line 49: `public void ProductionSource_ForbidsQueryFilterBypassAndRawSqlOutsideTheAllowList()`

### `tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs`

Declared test entry points (declarations are not execution results):

- Line 6: `public void RepositoryInterfaces_LiveInTheApplicationProject()`
- Line 24: `public void RepositoryClasses_LiveInTheInfrastructureProject()`

### `tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Architecture.Tests/SourceFiles.cs`

Reads production .cs source files directly, for rules NetArchTest cannot express (it inspects compiled type dependencies, not method calls or text). Mirrors ProjectFiles' repository-root lookup.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Architecture.Tests/TenantModelTests.cs`

Task 20.7 rule B, extended by Task 21.6: every ITenantOwned entity in the real model carries a query filter and the ConfigureTenantOwned keys — and every tenant-to-tenant foreign key is composite.

Declared test entry points (declarations are not execution results):

- Line 17: `public void EveryTenantOwnedEntityInTheProductionModel_HasAQueryFilter()`

### `tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs`

Task 22.4: every command or query inside a tenant module must declare (Task 22.1), so the dispatcher's layer-1 step (Task 22.2) can never be skipped by a use case over tenant data. The inverse guard pins that Month 1's non-tenant namespaces (Identity, Centres, Platform) never declare it. Matching is by implemented interface, never by class-name suffix.

Declared test entry points (declarations are not execution results):

- Line 36: `public void EveryRequestInATenantModule_DeclaresTenantScoped()`
- Line 47: `public void EveryMonth1Request_DoesNotDeclareTenantScoped()`

### `tests/TutoringCentre.Domain.Tests/Academics/EnrollmentTests.cs`

Declared test entry points (declarations are not execution results):

- Line 14: `public void Start_WithValidInput_IsActiveWithNoEndFields()`
- Line 31: `public void Start_WithEmptyCentre_ReturnsCentreRequired() =>`
- Line 35: `public void Start_WithEmptyStudent_ReturnsStudentRequired() =>`
- Line 39: `public void Start_WithEmptyGroup_ReturnsGroupRequired() =>`
- Line 45: `public void End_SetsEndedWithTheGivenTimeAndReason(EnrollmentEndReason reason)`
- Line 59: `public void End_AtTheSameInstantItStarted_Succeeds()`
- Line 67: `public void End_Twice_ReturnsAlreadyEndedAndKeepsTheFirstEnd()`
- Line 83: `public void End_BeforeItStarted_ReturnsEndBeforeStartAndStaysActive()`

### `tests/TutoringCentre.Domain.Tests/Academics/GroupTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public void Create_WithValidInput_SucceedsAsActive()`
- Line 29: `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- Line 33: `public void Create_WithEmptySubject_ReturnsSubjectRequired() =>`
- Line 37: `public void Create_WithEmptyTeacher_ReturnsTeacherRequired() =>`
- Line 41: `public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>`
- Line 45: `public void Create_With80CharacterName_Succeeds()`
- Line 57: `public void Create_With81CharacterName_ReturnsNameInvalid() =>`
- Line 61: `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()`
- Line 72: `public void Create_WithCapacityZero_ReturnsCapacityInvalid() =>`
- Line 76: `public void Create_WithCapacityOne_Succeeds()`
- Line 86: `public void Create_WithCapacity200_Succeeds()`
- Line 96: `public void Create_WithCapacity201_ReturnsCapacityInvalid() =>`
- Line 100: `public void Rename_WithValidName_ChangesNameAndNormalizedName()`
- Line 112: `public void Rename_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()`
- Line 124: `public void ChangeCapacity_WithValidValue_Changes()`
- Line 135: `public void ChangeCapacity_OutOfRange_FailsAndLeavesPreviousCapacityUnchanged()`
- Line 147: `public void AssignTeacher_WithADifferentTeacher_Changes()`
- Line 159: `public void AssignTeacher_WithTheCurrentTeacher_SucceedsAndChangesNothing()`
- Line 170: `public void AssignTeacher_WithEmptyId_ReturnsTeacherRequiredAndLeavesTeacherUnchanged()`
- Line 182: `public void Archive_Twice_ReturnsAlreadyArchived()`
- Line 195: `public void Restore_WhenActive_ReturnsNotArchived()`
- Line 207: `public void Restore_WhileArchived_ReturnsToActive()`
- Line 219: `public void Rename_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()`
- Line 233: `public void ChangeCapacity_WhileArchived_ReturnsArchivedReadOnlyAndLeavesCapacityUnchanged()`
- Line 246: `public void AssignTeacher_WhileArchived_ReturnsArchivedReadOnlyAndLeavesTeacherUnchanged()`
- Line 259: `public void EnsureCanEnroll_NobodyEnrolledInAOneSeatGroup_Succeeds()`
- Line 267: `public void EnsureCanEnroll_OneSeatLeft_Succeeds()`
- Line 275: `public void EnsureCanEnroll_EverySeatTaken_ReturnsFullAsConflict()`
- Line 287: `public void EnsureCanEnroll_MoreEnrolledThanSeats_ReturnsFull()`
- Line 300: `public void EnsureCanEnroll_ArchivedGroup_ReturnsArchivedWhateverTheCount(int activeEnrollmentCount)`
- Line 313: `public void ChangeCapacity_ToExactlyTheEnrolledCount_Succeeds()`
- Line 324: `public void ChangeCapacity_BelowTheEnrolledCount_ReturnsCapacityBelowEnrolledAndLeavesCapacityUnchanged()`

### `tests/TutoringCentre.Domain.Tests/Academics/ParentTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public void Create_WithValidInput_Succeeds()`
- Line 26: `public void Create_WithEmptyCentre_ReturnsCentreRequired()`
- Line 36: `public void Create_With1CharacterName_ReturnsNameInvalid() => AssertNameInvalid(Parent.Create(CentreId, "O", Phone));`
- Line 39: `public void Create_With121CharacterName_ReturnsNameInvalid() =>`
- Line 43: `public void Create_With2CharacterName_Succeeds()`
- Line 51: `public void Create_With120CharacterName_Succeeds()`
- Line 62: `public void Create_WithArabicName_IsKeptAsTypedWithNormalizedSearchName()`
- Line 74: `public void Update_WithValidInput_Succeeds()`
- Line 86: `public void Update_WithInvalidName_FailsAndLeavesNameAndPhoneUnchanged()`

### `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ClassSessionTests.cs`

Task 51.5. Every session comes from : it is the only way to obtain one.

Declared test entry points (declarations are not execution results):

- Line 20: `public void Cancel_FutureSession_IsCancelled()`
- Line 31: `public void Cancel_Twice_ReturnsSessionAlreadyCancelled()`
- Line 43: `public void Cancel_AtTheStartInstant_ReturnsSessionInPastAndStaysScheduled()`
- Line 54: `public void ReassignTeacher_FutureScheduledSession_ChangesTheTeacher()`
- Line 65: `public void ReassignTeacher_StartedSession_SucceedsAndKeepsTheTeacher()`
- Line 76: `public void ReassignTeacher_CancelledFutureSession_SucceedsAndKeepsTheTeacher()`

### `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/ScheduleRuleTests.cs`

Declared test entry points (declarations are not execution results):

- Line 16: `public void Create_WithValidInput_StoresLocalValuesAndIsOpenEnded()`
- Line 34: `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- Line 39: `public void Create_WithEmptyGroup_ReturnsGroupRequired() =>`
- Line 49: `public void Create_DurationBoundaries(int durationMinutes, bool valid)`
- Line 67: `public void Create_WithStartTimeOffTheFiveMinuteGrid_ReturnsStartTimeInvalid(int hour, int minute, int second) =>`
- Line 74: `public void Create_With41CharacterRoom_ReturnsRoomInvalid() =>`
- Line 81: `public void Create_With40CharacterRoom_Succeeds()`
- Line 96: `public void Create_WithBlankRoom_StoresNull(string? room)`
- Line 112: `public void OccursOn_MatchesTheWeekdayInsideTheEffectiveRange(int year, int month, int day, bool expected)`
- Line 121: `public void OccursOn_OpenEndedRule_KeepsOccurringOnTheWeekday()`
- Line 129: `public void End_OnTheEffectiveFromDate_Succeeds()`
- Line 138: `public void End_BeforeEffectiveFrom_ReturnsRuleEndInvalidAndStaysOpen()`
- Line 151: `public void End_Twice_ReturnsRuleAlreadyEndedAndKeepsTheFirstEnd()`

### `tests/TutoringCentre.Domain.Tests/Academics/Scheduling/SessionGeneratorTests.cs`

Task 51.4. Every expected instant is worked out by hand and written as a UTC literal; none is derived with the conversion calls the generator itself makes. Egypt (Africa/Cairo) since 2023: UTC+2 in winter, UTC+3 in summer. Clocks go forward at 00:00 on the last Friday of April and back at 24:00 on the last Thursday of October. For 2027 that is Friday 30 April (00:00 becomes 01:00) and Thursday 28 October (24:00 becomes 23:00, so 23:00-23:59 happens twice). If Egypt's law changes, these dates change and the generator does not. checks the machine's time-zone data agrees before any failure below is trusted.

Declared test entry points (declarations are not execution results):

- Line 26: `public void TimeZoneData_HasEgypts2027Transitions()`
- Line 36: `public void Generate_WinterSaturdays_ProducesFourSessionsAtSixteenUtc()`
- Line 64: `public void Generate_RangeStartingMidWeek_FirstSessionIsOnTheNextMatchingWeekday()`
- Line 77: `public void Generate_EffectiveFromAndToInsideTheRange_BoundTheOutput()`
- Line 90: `public void Generate_AcrossSpringForward_WallClockStaysEighteenAndUtcMovesAnHourEarlier()`
- Line 104: `public void Generate_AcrossFallBack_WallClockStaysEighteenAndUtcMovesAnHourLater()`
- Line 118: `public void Generate_StartInTheSkippedHour_MovesForwardByTheGap()`
- Line 132: `public void Generate_StartInTheRepeatedHour_TakesTheFirstOccurrence()`
- Line 146: `public void Generate_SessionCrossingFallBack_LastsExactlyItsDurationInElapsedTime()`
- Line 160: `public void Generate_JustAfterLocalMidnight_LocalDateIsNotTheUtcDate()`
- Line 174: `public void Generate_ToBeforeFrom_ReturnsEmpty()`
- Line 185: `public void Generate_Twice_GivesTheSameDatesAndInstantsWithNewIds()`

### `tests/TutoringCentre.Domain.Tests/Academics/StudentTests.cs`

Declared test entry points (declarations are not execution results):

- Line 28: `public void Register_WithValidInput_SucceedsActiveWithOnePrimaryLinkAndZeroRevision()`
- Line 47: `public void Register_WithEmptyCentre_ReturnsCentreRequired()`
- Line 57: `public void Register_With1CharacterName_ReturnsNameInvalid()`
- Line 67: `public void Register_With121CharacterName_ReturnsNameInvalid()`
- Line 77: `public void Register_With41CharacterSchoolYear_ReturnsSchoolYearInvalid()`
- Line 88: `public void Register_WithEmptyParentId_ReturnsParentRequired()`
- Line 97: `public void Register_WithNullPhoneAndSchoolYear_Succeeds()`
- Line 108: `public void AddParent_SecondParent_IsNotPrimaryAndRevisionIncreases()`
- Line 123: `public void AddParent_SameParentTwice_ReturnsParentAlreadyLinked()`
- Line 138: `public void AddParent_AFifthParent_ReturnsTooManyParents()`
- Line 156: `public void RemoveParent_Unknown_ReturnsParentNotLinked()`
- Line 170: `public void RemoveParent_TheOnlyParent_ReturnsLastParent()`
- Line 185: `public void RemoveParent_ThePrimaryWhileOthersRemain_ReturnsPrimaryParentRequired()`
- Line 201: `public void RemoveParent_ANonPrimaryParent_IsRemovedAndRevisionIncreases()`
- Line 216: `public void SetPrimaryParent_Unknown_ReturnsParentNotLinked()`
- Line 230: `public void SetPrimaryParent_AlreadyPrimary_ReturnsAlreadyPrimary()`
- Line 245: `public void SetPrimaryParent_Valid_ExactlyOnePrimaryAndRevisionIncreases()`
- Line 262: `public void Archive_Twice_ReturnsAlreadyArchived()`
- Line 277: `public void Restore_WhenActive_ReturnsNotArchived()`
- Line 290: `public void Restore_WhileArchived_ReturnsToActive()`
- Line 303: `public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnly()`
- Line 317: `public void AddParent_WhileArchived_ReturnsArchivedReadOnly()`
- Line 332: `public void RemoveParent_WhileArchived_ReturnsArchivedReadOnly()`
- Line 347: `public void SetPrimaryParent_WhileArchived_ReturnsArchivedReadOnly()`
- Line 363: `public void UpdateDetails_WithInvalidName_FailsAndLeavesStateUnchanged()`

### `tests/TutoringCentre.Domain.Tests/Academics/SubjectTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()`
- Line 26: `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- Line 30: `public void Create_WithEmptyName_ReturnsNameInvalidWithNameField() =>`
- Line 34: `public void Create_WithWhitespaceOnlyName_ReturnsNameInvalidWithNameField() =>`
- Line 38: `public void Create_With81CharacterName_ReturnsNameInvalidWithNameField() =>`
- Line 44: `public void Create_With80CharacterName_Succeeds()`
- Line 56: `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingNormalizedName()`
- Line 67: `public void Create_WithArabicName_IsAcceptedUnchanged()`
- Line 76: `public void Rename_WithValidName_Succeeds()`
- Line 87: `public void Rename_WithInvalidName_FailsAndLeavesPreviousName()`
- Line 99: `public void Archive_Twice_ReturnsAlreadyArchived()`
- Line 112: `public void Restore_WhenActive_ReturnsNotArchived()`
- Line 124: `public void Rename_WhileArchived_ReturnsArchivedReadOnly()`
- Line 138: `public void Restore_WhileArchived_ReturnsToActive()`

### `tests/TutoringCentre.Domain.Tests/Academics/TeacherTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void Create_WithValidInput_SucceedsAsActiveWithTrimmedNameAndAssignedId()`
- Line 26: `public void Create_WithEmptyCentre_ReturnsCentreRequired() =>`
- Line 30: `public void Create_WithEmptyName_ReturnsNameInvalidWithFullNameField() =>`
- Line 34: `public void Create_WithOneCharacterName_ReturnsNameInvalid() =>`
- Line 38: `public void Create_With121CharacterName_ReturnsNameInvalid() =>`
- Line 42: `public void Create_With2CharacterName_Succeeds()`
- Line 54: `public void Create_With120CharacterName_Succeeds()`
- Line 66: `public void Create_CollapsesInnerWhitespaceAndDerivesMatchingSearchName()`
- Line 77: `public void UpdateDetails_WithValidName_ChangesNameAndSearchName()`
- Line 89: `public void UpdateDetails_WithInvalidName_FailsAndLeavesPreviousNameUnchanged()`
- Line 101: `public void LinkUser_WithEmptyId_ReturnsUserRequiredAndLeavesLinkUnchanged()`
- Line 114: `public void LinkUser_WithAnId_SetsUserId()`
- Line 126: `public void LinkUser_WithTheAlreadyLinkedUser_SucceedsAndChangesNothing()`
- Line 139: `public void UnlinkUser_WhenLinked_ClearsTheLink()`
- Line 151: `public void Archive_Twice_ReturnsAlreadyArchived()`
- Line 164: `public void Restore_WhenActive_ReturnsNotArchived()`
- Line 176: `public void Restore_WhileArchived_ReturnsToActive()`
- Line 188: `public void Archive_KeepsTheAccountLink()`
- Line 200: `public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnlyAndLeavesNameUnchanged()`
- Line 214: `public void LinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()`
- Line 227: `public void UnlinkUser_WhileArchived_ReturnsArchivedReadOnlyAndLeavesLinkUnchanged()`

### `tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs`

Declared test entry points (declarations are not execution results):

- Line 13: `public void Create_WithValidInput_SucceedsAndTrimsName()`
- Line 29: `public void Create_WithEmptyName_ReturnsNameRequired() =>`
- Line 33: `public void Create_With121CharacterName_ReturnsNameTooLong() =>`
- Line 43: `public void Create_WithInvalidSlug_ReturnsSlugInvalid(string slug) =>`
- Line 47: `public void Create_WithUnknownTimeZone_ReturnsTimeZoneInvalid() =>`
- Line 53: `public void Create_WithCairoTimeZone_Succeeds()`
- Line 63: `public void Create_WithWhitespaceOnlyName_ReturnsNameRequired() =>`
- Line 68: `public void Create_With120CharacterName_Succeeds()`
- Line 87: `public void UpdateSettings_WithValidInput_SucceedsAndTrimsNameAndChangesLocale()`
- Line 101: `public void UpdateSettings_WithEmptyName_ReturnsNameInvalidAndChangesNothing()`
- Line 116: `public void UpdateSettings_With121CharacterName_ReturnsNameInvalidAndChangesNothing()`
- Line 129: `public void UpdateSettings_WithBothValuesUnchanged_SucceedsAndChangesNothing()`

### `tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void NewEntities_GetDifferentIds()`
- Line 20: `public void Id_IsVersion7Uuid()`

### `tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Validation_WithFields_ExposesTheFields()`
- Line 23: `public void Validation_WithoutFields_HasNullFields()`

### `tests/TutoringCentre.Domain.Tests/Common/PhoneNumberTests.cs`

Declared test entry points (declarations are not execution results):

- Line 51: `public void Create_NeverThrows()`
- Line 59: `public void TwoInstances_FromDifferentSpellingsOfOneNumber_AreEqualWithEqualHashCodes()`
- Line 69: `public void ToString_EqualsValue()`
- Line 77: `public void Masked_ForEgyptianNumber_ShowsCountryCodeAndLastFourDigits()`
- Line 85: `public void Masked_ForInternationalNumber_ShowsOnlyLastFourDigitsOfSubscriberNumber()`

### `tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs`

Declared test entry points (declarations are not execution results):

- Line 10: `public void Success_HasNoErrorAndExposesValue()`
- Line 23: `public void Failure_ExposesItsError()`
- Line 33: `public void Value_OnFailure_ThrowsInvalidOperationException()`
- Line 41: `public void Failure_WithNullError_ThrowsArgumentNullException()`

### `tests/TutoringCentre.Domain.Tests/Common/SearchTextTests.cs`

Declared test entry points (declarations are not execution results):

- Line 8: `public void Normalize_NullValue_ReturnsEmpty() => Assert.Equal(string.Empty, SearchText.Normalize(null));`
- Line 15: `public void Normalize_AhmedSpellingVariants_AllProduceTheSameString(string input)`
- Line 21: `public void Normalize_IbrahimWithAndWithoutInitialHamza_AreEqual() =>`
- Line 25: `public void Normalize_FatimaWithTaaMarbutaAndHaa_AreEqual() =>`
- Line 29: `public void Normalize_MustafaWithAlifMaksuraAndYaa_AreEqual() =>`
- Line 33: `public void Normalize_TrimsAndCollapsesWhitespaceAndLowersCase() =>`
- Line 37: `public void Normalize_IsCaseInsensitive() => Assert.Equal(SearchText.Normalize("omar"), SearchText.Normalize("OMAR"));`
- Line 45: `public void Normalize_IsIdempotent(string input)`

### `tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void Create_WithValidInput_SucceedsAsActiveWithTheGivenRole()`
- Line 26: `public void Create_WithEmptyUserId_ReturnsUserRequired()`
- Line 37: `public void Create_WithEmptyCentreId_ReturnsCentreRequired()`
- Line 48: `public void ChangeRole_WhenActive_SucceedsAndChangesTheRole()`
- Line 62: `public void ChangeRole_WhenInactive_FailsAndLeavesTheRoleUnchanged()`
- Line 78: `public void ChangeRole_ToTheSameRole_FailsAndLeavesTheRoleUnchanged()`
- Line 93: `public void Deactivate_WhenActive_SucceedsAndBecomesInactive()`
- Line 107: `public void Deactivate_WhenAlreadyInactive_ReturnsAlreadyInactive()`
- Line 122: `public void Activate_WhenAlreadyActive_ReturnsAlreadyActive()`

### `tests/TutoringCentre.Domain.Tests/Identity/PermissionsTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void All_HasSixteenDistinctKeysMatchingTheNamingPattern()`

### `tests/TutoringCentre.Domain.Tests/Identity/RolePermissionsTests.cs`

Declared test entry points (declarations are not execution results):

- Line 63: `public void Owner_HoldsEveryPermission()`
- Line 72: `public void For_OnlyReturnsPermissionsFromTheCatalogue(StaffRole role)`
- Line 81: `public void Holds_WithAnUnknownPermission_IsDeniedForEveryRole(StaffRole role)`

### `tests/TutoringCentre.Domain.Tests/Identity/RoleScopesTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void IsLimitedToOwnGroups_MatchesTheMatrixCellLiterally(StaffRole role, bool expected)`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs`

Task 31.7: the audit trail proven through real commands against real PostgreSQL. Every assertion reads audit.audit_entries through the superuser connection (), never through the application's own filtered view of it.

Declared test entry points (declarations are not execution results):

- Line 34: `public async Task CreateSubject_AsStaffOwner_WritesOneCreatedAuditRow()`
- Line 60: `public async Task RenameSubject_ChangesName_WritesOneUpdatedAuditRowWithNoStatusKey()`
- Line 84: `public async Task RenameSubject_ToItsOwnCurrentName_WritesNoAdditionalAuditRow()`
- Line 99: `public async Task ArchiveSubject_WritesUpdatedAuditRowWithStatusActiveBeforeAndArchivedAfter()`
- Line 120: `public async Task ChangeStaffRole_WritesUpdatedAuditRowWithRoleBeforeAndAfter()`
- Line 141: `public async Task CreateStaff_WritesCreatedMembershipAuditRowWithNoPasswordOrEmail()`
- Line 160: `public async Task CommandThatRenamesThenFails_WritesNoAuditRowAndNoDataChange()`
- Line 178: `public async Task CreateSubject_LosesUniqueRace_WritesNoAuditRowForTheLoser()`
- Line 203: `public async Task CreateSubject_AsSystemActorWithCentre_WritesAuditRowWithSystemActorTypeAndNullUser()`
- Line 219: `public async Task DevelopmentSeederAndCentrelessCentreCreation_WriteNoAuditRows()`
- Line 245: `public async Task AuditEntry_CorrelationId_MatchesTheOneSetOnTheScope()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs`

Task 47.5: the table's business constraints, as the superuser so row-level security is not what is under test. Composite foreign keys keep an enrollment inside one centre; the partial unique index allows at most one active enrollment per student and group while keeping ended ones as history; the checks make an ended row without its time, or ending before it started, unstorable.

Declared test entry points (declarations are not execution results):

- Line 22: `public async Task NileEnrollmentJoiningNileStudentToMaadiGroup_Throws23503()`
- Line 34: `public async Task NileEnrollmentJoiningMaadiStudentToNileGroup_Throws23503()`
- Line 46: `public async Task TwoActiveEnrollmentsForOneStudentAndGroup_Throws23505()`
- Line 59: `public async Task OneEndedAndOneActiveForOneStudentAndGroup_IsAllowed()`
- Line 74: `public async Task EndedWithoutEndedAt_Throws23514()`
- Line 87: `public async Task EndedBeforeEnrolled_Throws23514()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs`

Task 47.5: the decisive row-level security checks on the real academics.enrollments table, as the real runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under test cannot have biased them.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task NoSetting_SelectCount_IsZero()`
- Line 29: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 46: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 71: `public async Task SettingNile_DeleteOwnEnrollment_Throws42501BecauseDeleteIsNotGranted()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs`

Task 48.10: ownership for derived resources. A roster and a student's enrollments belong to groups, so the group's teacher scope applies to them too, through the real provider, read service and PostgreSQL. In Nile, G1 belongs to teacher T1 and G2 to teacher T2; the actor is a Teacher-role member linked to T1.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task LinkedTeacher_ReadsTheOwnGroupsRosterAndGetsGroupNotFoundForAnothers()`
- Line 31: `public async Task LinkedTeacher_ListingAStudentsEnrollments_SeesOnlyThoseInOwnGroups()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs`

Task 48.10: tenancy in both directions. Every ID in an enrollment command is resolved inside the actor's own centre, so another centre's group, student or enrollment answers the same not-found as an unknown ID — never a foreign-key 500 from the composite keys behind them — and the other centre's rows are unchanged.

Declared test entry points (declarations are not execution results):

- Line 22: `public async Task Enroll_IntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)`
- Line 33: `public async Task Enroll_TheOtherCentresStudentIntoAnOwnGroup_ReturnsStudentNotFound(bool nileActsOnMaadi)`
- Line 45: `public async Task Withdraw_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)`
- Line 58: `public async Task Transfer_TheOtherCentresEnrollment_ReturnsEnrollmentNotFound(bool nileActsOnMaadi)`
- Line 70: `public async Task Transfer_AnOwnEnrollmentIntoTheOtherCentresGroup_ReturnsGroupNotFound(bool nileActsOnMaadi)`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs`

Shared setup for Task 48.10's enrollment integration tests. Bulk rows (the students and the enrollments that fill a group before a race) are seeded directly as the superuser, in one connection, so twenty repetitions stay fast; everything under test goes through the real dispatcher as tutoring_app. Every assertion on persisted state reads through the superuser connection, never through the application's own filtered view.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs`

Task 48.10: a transfer is one transaction that ends the old enrollment and starts a new one. It moves exactly one seat, changes nothing when it fails, and — because it locks only the target — two opposite transfers in parallel both complete instead of waiting on each other in a cycle.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task Transfer_MovesTheSeatFromTheSourceToTheTarget()`
- Line 39: `public async Task Transfer_IntoAFullTarget_ReturnsGroupFullAndLeavesBothGroupsUnchanged()`
- Line 56: `public async Task Transfer_WithAStaleVersion_ReturnsConcurrencyStaleAndChangesNothing()`
- Line 72: `public async Task TwoOppositeTransfersInParallel_BothCompleteWithoutDeadlock_AcrossTwentyRepetitions()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs`

Task 48.10: the enrollment use cases and the rules they added to earlier ones, through the real dispatcher, real adapters and real PostgreSQL, plus the audit rows each command writes through the policy alone.

Declared test entry points (declarations are not execution results):

- Line 30: `public async Task Withdraw_FreesTheSeatThatTheNextEnrollmentTakes()`
- Line 48: `public async Task ReEnrollAfterWithdrawal_CreatesASecondRowAndKeepsTheFirstAsHistory()`
- Line 70: `public async Task ArchiveGroup_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()`
- Line 85: `public async Task ArchiveStudent_WithAnActiveEnrollment_IsRefusedUntilTheStudentIsWithdrawn()`
- Line 100: `public async Task ChangeCapacity_BelowTheEnrolledCount_IsRefusedAndEqualToItSucceeds()`
- Line 115: `public async Task GroupListDetailAndRoster_ShowTheRealEnrolledCountAndNamesOnly()`
- Line 136: `public async Task Enroll_WritesOneCreatedAuditRow()`
- Line 151: `public async Task Withdraw_WritesOneUpdatedAuditRowFromActiveToEndedWithTheReason()`
- Line 166: `public async Task Transfer_WritesAnUpdatedRowForTheOldEnrollmentAndACreatedRowForTheNewOne()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs`

Task 44.14: the raw constraints, as the superuser so row-level security is not what is under test. The composite foreign keys make a Nile group that points at a Maadi subject or teacher unstorable even through privileged SQL; the check constraint holds capacity to 1–200.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task NileGroupWithMaadiSubject_Throws23503()`
- Line 30: `public async Task NileGroupWithMaadiTeacher_Throws23503()`
- Line 44: `public async Task CapacityOutOfRange_Throws23514(int capacity)`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs`

Task 44.14: the decisive row-level security checks on the real academics.groups table, as the real runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under test cannot have biased them.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task NoSetting_SelectCount_IsZero()`
- Line 29: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 46: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 72: `public async Task SettingNile_UpdateMaadiGroupById_AffectsZeroRows()`
- Line 92: `public async Task SettingNile_DeleteOwnGroup_Throws42501BecauseDeleteIsNotGranted()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTeacherScopeTests.cs`

Task 44.14: ownership, the third check, through the real provider, read service and PostgreSQL. In Nile, G1 belongs to teacher T1 and G2 to teacher T2; three actors look at them: the owner, a Teacher-role actor linked to T1, and a Teacher-role actor with no linked record. The scope is proven on the list, on the list's teacher filter and on the item read, never on the list alone.

Declared test entry points (declarations are not execution results):

- Line 22: `public async Task Owner_ListsBothGroups()`
- Line 32: `public async Task TeacherLinkedToT1_ListsOnlyG1()`
- Line 42: `public async Task TeacherLinkedToT1_FilteringByT2_GetsAnEmptyListNotAnError()`
- Line 52: `public async Task TeacherLinkedToT1_GetG2_ReturnsGroupNotFound()`
- Line 65: `public async Task UnlinkedTeacher_ListsNothingAndGetsNothing()`
- Line 77: `public async Task AfterReassigningG2ToT1_TheT1ActorListsBoth()`
- Line 91: `public async Task AfterT1IsArchived_TheT1ActorListsNothing()`
- Line 107: `public async Task TeacherLinkedToT1_IncludingArchived_StillSeesOnlyOwnGroups()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTenantIsolationTests.cs`

Task 44.14: tenancy, the first check, in both directions. Each centre's owner aims every group request at the other centre's group and gets group.not_found, the same answer as an unknown ID; the other centre's row is unchanged afterwards. Creating a group with the other centre's subject or teacher is resolved inside the actor's centre and answers subject.not_found or teacher.not_found, never a foreign-key 500.

Declared test entry points (declarations are not execution results):

- Line 42: `public async Task RequestAimedAtTheOtherCentresGroup_ReturnsGroupNotFoundAndChangesNothing(string request, bool nileActsOnMaadi)`
- Line 59: `public async Task CreateNileGroup_WithAMaadiSubject_ReturnsSubjectNotFound()`
- Line 71: `public async Task CreateNileGroup_WithAMaadiTeacher_ReturnsTeacherNotFound()`
- Line 83: `public async Task AssignNileGroup_ToAMaadiTeacher_ReturnsTeacherNotFound()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs`

Shared setup for Task 44.14's group integration tests: every reference is created through the real use cases as the given actor, so the rows are exactly what production would have stored.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs`

Task 44.14: the six group commands through the real dispatcher, real adapters and real PostgreSQL, plus the two places the database, not the handler, must arbitrate: a name that differs only by hamza form, and ten parallel creates racing past the handler's own uniqueness check.

Declared test entry points (declarations are not execution results):

- Line 30: `public async Task Create_ThenGet_ReturnsTheGroupWithSubjectAndTeacherNames()`
- Line 50: `public async Task Rename_PersistsTheNewName()`
- Line 62: `public async Task Rename_WithAStaleVersion_ReturnsConcurrencyStale()`
- Line 77: `public async Task ChangeCapacity_PersistsInRangeAndRefusesOutOfRange()`
- Line 92: `public async Task AssignTeacher_MovesTheGroupAndAuditsTeacherIdBeforeAndAfter()`
- Line 109: `public async Task ArchiveThenRestore_KeepsTheRowAndTogglesStatus()`
- Line 126: `public async Task Restore_UnderAnArchivedSubject_ReturnsSubjectArchived()`
- Line 144: `public async Task Create_NameDifferingOnlyByHamzaForm_ReturnsNameTaken()`
- Line 158: `public async Task Create_TenParallelCreatesWithOneName_LeavesExactlyOneRow()`
- Line 182: `public async Task TeacherRead_ActiveGroupCount_CountsOnlyActiveGroups()`
- Line 196: `public async Task ArchiveSubjectAndTeacher_WithAnActiveGroup_AreRefused()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs`

Task 48.10, the schedule's Week 8 demo: the capacity lock (ADR 0011) under real parallel transactions against real PostgreSQL, each attempt in its own scope and connection, repeated because a race test that runs once can pass by luck. Every count is read through the superuser connection, not inferred from the returned results. A loser is a valid request that lost the race, so it gets the Domain's own answer, never an exception.

Declared test entry points (declarations are not execution results):

- Line 25: `public void AppConnectionPool_IsNotSmallerThanTheLargestParallelism()`
- Line 33: `public async Task TenParallelEnrollmentsIntoTheLastSeat_ExactlyOneSucceedsAndNineGetGroupFull_AcrossTwentyRepetitions()`
- Line 62: `public async Task FiftyParallelEnrollmentsIntoASingleSeatGroup_ExactlyOneSucceeds()`
- Line 83: `public async Task TheSameStudentEnrolledTwiceInParallel_OneSucceedsAndTheOtherGetsAlreadyEnrolled()`
- Line 106: `public async Task EnrollAndShrinkInParallel_NeverLeaveMoreStudentsThanSeats()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentAuditTrailTests.cs`

Task 39.9: the audit trail for the production parent commands, proven through real HTTP-shaped use cases against real PostgreSQL — the counterpart to Month 2's AuditTrailTests. Every assertion reads audit.audit_entries through the superuser connection.

Declared test entry points (declarations are not execution results):

- Line 20: `public async Task CreateParent_WritesOneCreatedAuditRowWithMaskedPhoneOnly()`
- Line 42: `public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs`

Task 39.9: the production parent use cases exercised through the real AppDbContext, interceptors and unit of work — the repository, read service and dispatcher wiring all real, nothing test-only.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task CreateParent_PersistsWithActorsCentreAndCanonicalPhone()`
- Line 39: `public async Task GetParent_WithChildrenSeededBySql_ReturnsDetailOrderedByStudentName()`
- Line 59: `public async Task UpdateParent_ChangesNameAndPhoneAndBumpsVersion()`
- Line 79: `public async Task LookupParentByPhone_DifferentlySpelled_FindsTheParent()`
- Line 92: `public async Task LookupParentByPhone_NoSuchParent_SucceedsWithNull()`
- Line 103: `public async Task UpdateParent_StaleVersion_ReturnsConcurrencyStale()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ParentRaceTests.cs`

Task 39.9: both callers pass the application-level uniqueness check; only ux_parents_centre_phone can arbitrate who actually wins. The ten spellings all normalize to the same E.164 number, proving the index — not string equality — is what the race is decided on. Each attempt dispatches in its own scope.

Declared test entry points (declarations are not execution results):

- Line 32: `public async Task Create_TenParallelCreatesWithTheSamePhoneInDifferentSpellings_LeavesExactlyOneRowAndNineCleanConflicts()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/RowLevelSecurityCoverageTests.cs`

Task 23.5: catalogue-driven coverage so a future tenant table that forgets row-level security fails the build by name, rather than relying on someone remembering to add a test for it. Discovers every table from PostgreSQL's own catalogue; never enumerates tables by hand.

Declared test entry points (declarations are not execution results):

- Line 25: `public async Task EveryTableWithACentreIdColumn_HasForcedRowLevelSecurityAndACompletePolicy()`
- Line 58: `public async Task TutoringApp_HasNoDeletePrivilegeOnSubjects()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleDstPersistenceTests.cs`

Task 52.11: daylight saving time as stored and as selected. Cairo moves from UTC+2 to UTC+3 at 00:00 on Friday 30 April 2027 (the clocks jump to 01:00). "Today" is Tuesday 20 April 2027, before the change.

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task SaturdayAtSixPm_IsStoredAtSixteenUtcBeforeTheChangeAndFifteenUtcAfterIt()`
- Line 30: `public async Task FridayAtHalfPastMidnight_OnTheChangeDay_IsInTheTimetableUnderItsLocalDateNotItsUtcDate()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs`

Task 52.11: the decisive row-level security checks on academics.schedule_rules and academics.class_sessions, as the real runtime role, independent of EF or the application pipeline (the style of Tasks 38.4 and 47.5). Rows are seeded as the superuser so the policy under test cannot have biased them.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task NoSetting_SelectCount_IsZero(string table)`
- Line 32: `public async Task SettingNile_SelectsOnlyNileRows(string table)`
- Line 49: `public async Task SettingNile_InsertRuleWithMaadiCentre_Throws42501()`
- Line 73: `public async Task SettingNile_InsertSessionWithMaadiCentre_Throws42501()`
- Line 103: `public async Task SettingNile_DeleteOwnRow_Throws42501BecauseDeleteIsNotGranted(string table)`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTeacherScopeTests.cs`

Task 52.11: teacher ownership through the real provider, read service and PostgreSQL. In Nile, G1 and G2 belong to T1 and G3 to T2; the actor is a Teacher-role member linked to T1, and every group has a rule.

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task LinkedTeacher_TimetableContainsOnlyTheirOwnSessions()`
- Line 28: `public async Task LinkedTeacher_RulesOfAnotherTeachersGroup_IsGroupNotFound()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTenantIsolationTests.cs`

Task 52.11: tenant isolation in both directions, through the real pipeline. Each centre has its own owner, teachers, groups and a Saturday 18:00 rule; another centre's IDs answer exactly like unknown IDs.

Declared test entry points (declarations are not execution results):

- Line 20: `public async Task AddRule_ForAnotherCentresGroup_IsGroupNotFound()`
- Line 32: `public async Task EndRule_OfAnotherCentre_IsRuleNotFoundAndTheRuleStaysOpen()`
- Line 46: `public async Task CancelSession_OfAnotherCentre_IsSessionNotFoundAndTheSessionStaysScheduled()`
- Line 60: `public async Task Timetable_NeverContainsAnotherCentresSessions_InEitherDirection()`
- Line 72: `public async Task ListRules_OfAnotherCentresGroup_IsGroupNotFound()`
- Line 84: `public async Task ANileTeacherAndAMaadiTeacher_MayHoldSessionsAtTheSameInstant()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs`

Shared setup for Task 52.11's schedule tests. Every schedule use case runs through : the production container with one change, a fixed by default at Monday 1 March 2027, 08:00 UTC (10:00 in Cairo), so "today" in the centre is 1 March and no test depends on the real date. References (staff, subjects, teachers, groups) are created through the real use cases; rows that test the database itself are inserted as the superuser. Every assertion on persisted state reads through the superuser connection.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleUseCaseTests.cs`

Task 52.11: the schedule use cases end to end — dispatcher, handler, EF, interceptors, the exclusion constraints and their translation, one transaction per command — with the clock fixed at Monday 1 March 2027 (Cairo winter time, UTC+2). In Nile, G1 and G2 are taught by T1 and G3 by T2. Saturday 18:00 for 90 minutes is 16:00-17:30 UTC.

Declared test entry points (declarations are not execution results):

- Line 26: `public async Task AddRule_StoresOneRuleAndFourSessions_AuditsTheRuleAndNoSession()`
- Line 40: `public async Task AddRule_SecondGroupOfTheSameTeacherAtAnOverlappingTime_IsTeacherDoubleBookedAndLeavesNoTrace()`
- Line 55: `public async Task AddRule_SecondRuleForTheSameGroupOverlappingTheFirst_IsGroupDoubleBooked()`
- Line 68: `public async Task AddRule_TwoParallelCollidingRulesForOneTeacher_ExactlyOneSucceeds()`
- Line 90: `public async Task ExtendSchedule_RunTwice_TheSecondRunCreatesNothing()`
- Line 105: `public async Task ExtendSchedule_TwoRulesThatOnlyNowReachASharedSlot_FailsWholeWithNothingGenerated()`
- Line 122: `public async Task EndRule_CancelsLaterSessions_AndTheirSlotCanBeTakenByAnotherGroupOfTheSameTeacher()`
- Line 144: `public async Task CancelSession_FreesItsSlot_ForAnotherSessionOfTheSameTeacher()`
- Line 163: `public async Task AssignGroupTeacher_MovesFutureSessionsToTheNewTeacher_AndLeavesPastOnes()`
- Line 181: `public async Task AssignGroupTeacher_ToATeacherBusyAtThatTime_IsTeacherDoubleBookedAndChangesNothing()`
- Line 198: `public async Task ArchiveGroup_WithARuleStillInEffect_IsRefusedWithHasActiveSchedule()`
- Line 213: `public async Task CancelSession_ThatHasStarted_IsRefusedAndStaysScheduled()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs`

Task 52.11: the double-booking rules proven against the constraints alone — raw SQL as the superuser, no EF, no handler, no row-level security. The use-case tests prove the same rules again through translation and rollback; these prove the database refuses even a caller that skips the application entirely.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task OverlappingScheduledSessionsForOneTeacher_AreRefusedByTheTeacherConstraint()`
- Line 32: `public async Task BackToBackSessionsForOneTeacher_AreAllowedBecauseTheRangeIsHalfOpen()`
- Line 44: `public async Task OverlapWithACancelledSession_IsAllowedBecauseTheConstraintsArePartial()`
- Line 55: `public async Task TheSameTimesAndTeacherIdUnderAnotherCentre_AreAllowedBecauseTheConstraintsIncludeCentreId()`
- Line 71: `public async Task OverlappingSessionsForOneGroupWithDifferentTeachers_AreRefusedByTheGroupConstraint()`
- Line 84: `public async Task ASecondSessionForTheSameRuleAndLocalDate_IsRefusedByTheUniqueIndex()`
- Line 97: `public async Task Catalogue_ClassSessionsHasBothExclusionConstraints_TenantScopedHalfOpenAndPartial()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentAggregatePersistenceTests.cs`

Task 38.5: the Student aggregate exercised through the real AppDbContext, interceptors and unit of work, using the test-only requests in — the repositories and use cases that would normally carry this arrive on Days 39–40.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task NileActorSavesParentAndStudent_PersistsThreeRowsWithNileCentreNormalizedSearchNameAndE164Phone()`
- Line 46: `public async Task Reload_StudentHasOnePrimaryLink()`
- Line 62: `public async Task AddSecondParentAndSave_LinksRevisionIsOneAndStudentVersionChanged()`
- Line 83: `public async Task TwoContextsLoadSameStudentAndEachAddsADifferentParent_SecondSaveThrowsConcurrencyException()`
- Line 112: `public async Task SetPrimaryParentAndSave_SucceedsWithExactlyOnePrimaryRow()`
- Line 136: `public async Task RemoveParentAndSave_LinkRowIsGone()`
- Line 156: `public async Task MaadiActorListsStudents_DoesNotContainNilesStudent()`
- Line 170: `public async Task NileActorSavesStudentWhoseLinkPointsAtAMaadiParent_SaveFailsOnForeignKeyAndNothingStored()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentAuditTrailTests.cs`

Task 38.6: the audit trail for a parent's phone change, proven through a real command against real PostgreSQL — the counterpart to Month 2's AuditTrailTests for Subject and Membership. Every assertion reads audit.audit_entries through the superuser connection, never through the application's own filtered view of it.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task UpdateParentPhone_WritesOneUpdatedAuditRowWithMaskedValuesOnly()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs`

Task 38.4: proves the composite foreign keys and unique constraints hold even for the superuser, who bypasses row-level security entirely — the guarantee RLS alone cannot give (docs/architecture/tenancy.md, Layer 4). Rows are seeded and the violating statement run over the owner connection directly, never through the application.

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task LinkWithNileCentreButMaadiParent_Throws23503()`
- Line 40: `public async Task SamePhoneTwiceInNile_Throws23505OnUxParentsCentrePhone()`
- Line 61: `public async Task SamePhoneOnceInEachCentre_Succeeds()`
- Line 73: `public async Task TwoPrimaryLinksForOneStudent_Throws23505OnUxStudentParentsOnePrimary()`
- Line 99: `public async Task NonE164Phone_Throws23514()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs`

Task 38.4: repeats the decisive row-level security checks on the three new tables, as the real runtime role, independent of EF or the application pipeline. Parents and students have no DELETE privilege at all (Task 38.3); student_parents does, since links are removed and the removal is audited.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task NoSetting_Parents_SelectCountIsZero()`
- Line 30: `public async Task NoSetting_Students_SelectCountIsZero()`
- Line 43: `public async Task NoSetting_StudentParents_SelectCountIsZero()`
- Line 57: `public async Task SettingNile_Parents_SelectsOnlyNileRows()`
- Line 74: `public async Task SettingNile_Students_SelectsOnlyNileRows()`
- Line 91: `public async Task SettingNile_StudentParents_SelectsOnlyNileRows()`
- Line 111: `public async Task SettingNile_InsertParentWithMaadiCentre_Throws42501()`
- Line 132: `public async Task SettingNile_InsertStudentWithMaadiCentre_Throws42501()`
- Line 153: `public async Task SettingNile_InsertStudentParentWithMaadiCentre_Throws42501()`
- Line 179: `public async Task Delete_AnyParent_Throws42501NoPrivilege()`
- Line 195: `public async Task Delete_AnyStudent_Throws42501NoPrivilege()`
- Line 211: `public async Task SettingNile_DeleteNileLink_AffectsOneRow()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentTestRequests.cs`

Task 38.5/38.6: test-only requests that exercise the Student and Parent aggregates through the real AppDbContext, interceptors and unit of work — standing in for the repositories and use cases that arrive on Days 39–40.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentReadServicePhoneSmokeTest.cs`

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task PhoneFragmentSearch_FindsStudentByParentPhoneSuffix()`
- Line 46: `public async Task NameSearch_LogsPredicateParameterAndLimit()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentUseCaseIntegrationTests.cs`

Task 40.13: student search, cross-tenant isolation and optimistic-concurrency proofs through the real dispatcher.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task ListStudents_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()`
- Line 36: `public async Task ListStudents_PhoneFragment_FindsStudentByOwnPhoneSuffix()`
- Line 48: `public async Task ListStudents_PhoneFragment_FindsStudentByParentPhoneSuffix()`
- Line 60: `public async Task ListStudents_ArchivedExcludedByDefault_IncludedWhenRequested()`
- Line 75: `public async Task ListStudents_KeysetPaging_SecondPageContinuesAfterTheFirst()`
- Line 93: `public async Task MaadiActor_CannotSeeOrRegisterAgainstNilesParent()`
- Line 108: `public async Task MaadiActor_ListStudents_DoesNotSeeNilesStudent()`
- Line 121: `public async Task MaadiActor_GetStudent_SeesNotFoundForNilesStudent()`
- Line 134: `public async Task UpdateStudent_StaleVersion_ReturnsConcurrencyStale()`
- Line 152: `public async Task LinkParent_StaleVersion_ReturnsConcurrencyStale()`
- Line 171: `public async Task RegisterStudent_StudentValidationFailsAfterNewParentWasStaged_NothingPersists()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs`

Task 25.9: the first Tenant A → Tenant B tests on real business data. Every assertion has two parts — the attacker's (Maadi's) result, and the victim's (Nile's) row, read back through the superuser connection, never through the application. Maadi always uses S's real ID and real version, never an invented one.

Declared test entry points (declarations are not execution results):

- Line 28: `public async Task Get_MaadiOwnerForNileSubject_ReturnsNotFound()`
- Line 39: `public async Task Rename_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 54: `public async Task Archive_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 68: `public async Task Restore_MaadiOwnerForNileSubjectWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 82: `public async Task List_MaadiOwner_ContainsNoNileSubject()`
- Line 93: `public async Task Create_MaadiOwnerWithNilesExactName_Succeeds()`
- Line 103: `public async Task CentrelessActor_AllSixRequests_ReturnTenantNotSelected()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectPersistenceTests.cs`

Task 23.6: Subject exercised through the real AppDbContext, interceptors and unit of work, as different actors, using the test-only requests in — the repository and read service that would normally carry this arrive on Day 25.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task NileActorAddsMathematics_PersistsWithNileCentreCreatedAtNoUpdatedAtActiveStatus()`
- Line 40: `public async Task MaadiActorListsSubjects_DoesNotContainNilesSubject()`
- Line 51: `public async Task NileActorAddsWhitespaceVariantOfMathematics_TranslatedToNameTakenConflict()`
- Line 65: `public async Task MaadiActorAddsMathematics_Succeeds()`
- Line 75: `public async Task StatusOutsideAllowedValuesWrittenByRawSql_ChecksViolation()`
- Line 94: `public async Task Rename_SetsUpdatedAtAndChangesVersion()`
- Line 111: `public async Task ConcurrentRenameFromTwoContexts_SecondSaveThrowsConcurrencyException()`
- Line 135: `public async Task NileActorAddsSubjectConstructedWithMaadiCentre_TenantViolationAndNothingStored()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRaceTests.cs`

Task 25.7: both callers pass the application-level uniqueness check; only the unique index can arbitrate who actually wins. Each attempt dispatches in its own scope (its own context and connection), never sharing one.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task Create_TenParallelCreatesWithSameName_LeavesExactlyOneRowAndNineCleanConflicts()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectRawSqlRowLevelSecurityTests.cs`

Task 23.4: repeats the decisive row-level security checks (Task 21.4) on the real academics.subjects table, as the real runtime role, independent of EF or the application pipeline — the direct evidence that this table, not just the probe, is actually wired to the policy. Test 7 is specific to subjects: tutoring_app has no DELETE privilege on it at all (Task 23.3), so a delete is refused before row-level security ever runs.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task NoSetting_SelectCount_IsZero()`
- Line 31: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 48: `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- Line 65: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 88: `public async Task SettingNile_UpdateNileSubjectCentreIdToMaadi_Throws42501()`
- Line 106: `public async Task SettingNile_UpdateMaadiSubjectById_AffectsZeroRows()`
- Line 129: `public async Task Delete_AnySubject_Throws42501NoPrivilege()`
- Line 145: `public async Task AsOwnerWithNoSetting_SelectCount_IsZero()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs`

Task 25.8: optimistic concurrency proven end to end — the server never substitutes the current version for the client's. No test fetches a fresh version before its second write; that is exactly the anomaly under test.

Declared test entry points (declarations are not execution results):

- Line 25: `public async Task Rename_WithTheVersionJustRead_Succeeds()`
- Line 38: `public async Task Rename_AgainWithTheSameStaleVersion_ReturnsConcurrencyStaleAndKeepsTheFirstRename()`
- Line 58: `public async Task Archive_WithAStaleVersion_ReturnsConcurrencyStale()`
- Line 73: `public async Task Rename_TwoParallelCarryingTheSameVersion_ExactlyOneSucceeds()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectTestRequests.cs`

Task 23.6: test-only requests that exercise Subject through the real AppDbContext, interceptors and unit of work — standing in for the repository and read service that arrive on Day 25. The centre is taken as an explicit parameter, not the acting actor's own centre, only so AddSubjectCommand can also play the "constructed with another centre" scenario (test 8); every other test passes the actor's own centre.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs`

Task 25.6: each Subject use case proven through the real dispatcher, unit of work, repository and read service against real PostgreSQL — the first time these six requests run against the database they were built for. Reads happen through the same actor in a fresh dispatch, never back through a scope that wrote.

Declared test entry points (declarations are not execution results):

- Line 28: `public async Task Create_Succeeds_AndListContainsItWithAVersion()`
- Line 44: `public async Task Create_SameNameDifferentCase_ReturnsNameTakenAndStillOneRow()`
- Line 56: `public async Task Rename_SetsNewNameUpdatedTimestampAndChangesVersion()`
- Line 75: `public async Task Rename_ToAnExistingName_ReturnsNameTaken()`
- Line 89: `public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()`
- Line 106: `public async Task Archive_Again_ReturnsAlreadyArchived()`
- Line 121: `public async Task Restore_BecomesActive()`
- Line 137: `public async Task Get_UnknownId_ReturnsNotFound()`
- Line 146: `public async Task Create_EightyOneCharacterName_ValidationFailureAndNoTransactionOpened()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs`

Task 43.14: the account link is the security-relevant part of Task 43.8, so most cases here are refusals rather than the happy path alone — an unknown user, a user of another centre, a user with another role and an inactive member all answer teacher.user_not_eligible identically through the real IStaffDirectory adapter and real PostgreSQL; a user already linked answers teacher.account_taken, including under a real race that only the unique index can arbitrate.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task SetAccount_EligibleTeacherRoleUser_Links()`
- Line 39: `public async Task SetAccount_SecretaryUserId_ReturnsUserNotEligible()`
- Line 52: `public async Task SetAccount_MaadiOnlyUser_ReturnsUserNotEligible()`
- Line 65: `public async Task SetAccount_UnknownUserId_ReturnsUserNotEligible()`
- Line 77: `public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()`
- Line 97: `public async Task SetAccount_NullUserId_UnlinksAPreviouslyLinkedTeacher()`
- Line 116: `public async Task SetAccount_TwoParallelLinksOfOneUserToTwoTeachers_OneSucceedsOneAccountTaken()`
- Line 148: `public async Task SetAccount_Links_WritesOneUpdatedAuditRowWithUserIdBeforeNullAndAfterTheId()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherRowLevelSecurityTests.cs`

Task 43.14: repeats the decisive row-level security checks on the real academics.teachers table, as the real runtime role, independent of EF or the application pipeline, plus the two integrity checks that are specific to this table's account link: the composite foreign key into identity.memberships (23503, a Maadi-only user is simply not a valid (centre_id, user_id) pair for Nile) and the partial unique index on (centre_id, user_id) (23505, the same user linked to a second Nile teacher). Both integrity checks run as the superuser — row-level security is not what is under test there, the raw constraint is.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task NoSetting_SelectCount_IsZero()`
- Line 36: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 53: `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- Line 70: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 93: `public async Task SettingNile_UpdateNileTeacherCentreIdToMaadi_Throws42501()`
- Line 111: `public async Task SettingNile_UpdateMaadiTeacherById_AffectsZeroRows()`
- Line 134: `public async Task AsSuperuser_InsertTeacherWithNileCentreAndMaadiOnlyUser_Throws23503ForeignKeyViolation()`
- Line 157: `public async Task AsSuperuser_LinkSameNileUserToASecondNileTeacher_Throws23505UniqueViolation()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs`

Task 43.14: each Teacher use case (excluding the account link, proven separately in ) through the real dispatcher, unit of work, repository and read service against real PostgreSQL. Reads happen through the same actor in a fresh dispatch, never back through a scope that wrote.

Declared test entry points (declarations are not execution results):

- Line 26: `public async Task Create_Succeeds_AndListContainsItWithAVersion()`
- Line 42: `public async Task Update_SetsNewDetailsAndChangesVersion()`
- Line 60: `public async Task Archive_ExcludedFromDefaultList_IncludedWhenArchivedRequested()`
- Line 78: `public async Task Archive_Again_ReturnsAlreadyArchived()`
- Line 94: `public async Task Restore_BecomesActive()`
- Line 111: `public async Task Get_UnknownId_ReturnsNotFound()`

### `tests/TutoringCentre.Infrastructure.Tests/Academics/UnlinkParentReconciliationSmokeTest.cs`

Proves UnitOfWork.ReconcileOrphanedStudentParentLinks through the real production handlers, not the Day 38 test-only ones in StudentParentTestRequests.cs: a link removed through UnlinkParentCommand must actually disappear from the database, not just from the aggregate's in-memory collection.

Declared test entry points (declarations are not execution results):

- Line 24: `public async Task UnlinkParent_ThroughRealHandler_RemovesTheLinkRow()`

### `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderMaskingTests.cs`

Task 38.6: pins the masking rule in directly against plain values, no database — whether a field changed is decided before masking ever runs, so two different numbers that happen to share their last four digits are never silently dropped.

Declared test entry points (declarations are not execution results):

- Line 17: `public void Updated_MaskedFieldChanged_RecordsMaskedBeforeAndAfterValues()`
- Line 30: `public void Updated_TwoDifferentNumbersWithTheSameLastFourDigits_StillProducesAnEntry()`
- Line 45: `public void Created_MaskedField_RecordsNullBeforeAndMaskedAfterValue()`

### `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditDiffBuilderTests.cs`

Task 31.5: pure tests, no database — exercises every rule in the Day 31 contract's "Changes document" section directly against plain values.

Declared test entry points (declarations are not execution results):

- Line 12: `public void Created_IncludesEveryAllowedField_WithNullBeforeAndTheValueAfter()`
- Line 28: `public void Updated_IncludesOnlyModifiedAllowedFieldsThatActuallyDiffer()`
- Line 42: `public void Deleted_IncludesEveryAllowedField_WithTheValueBeforeAndNullAfter()`
- Line 58: `public void Updated_AModifiedButEqualValue_IsSkipped()`
- Line 71: `public void Updated_ANonAllowedField_IsNeverPresentEvenWhenModified()`
- Line 87: `public void Updated_WithNoAllowedFieldReallyChanged_YieldsNull()`
- Line 101: `public void EnumValues_AreFormattedAsTheirLowerCaseName()`
- Line 114: `public void GuidValues_AreFormattedAsStrings()`
- Line 126: `public void NullValues_AreWrittenAsJsonNull_NotTheStringNull()`
- Line 139: `public void FieldNames_AreCamelCasedFromThePascalCasePropertyName()`
- Line 150: `public void Created_WritesDatesAndTimesAsInvariantIsoText()`

### `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditImmutabilityTests.cs`

Task 31.8: proves the audit log cannot be altered through tutoring_app, nor read across centres — at the database-role level, independent of EF or the application pipeline, the same way Task 23.4 proved it for academics.subjects ().

Declared test entry points (declarations are not execution results):

- Line 22: `public async Task SettingNile_UpdateAnyRow_Throws42501()`
- Line 37: `public async Task SettingNile_DeleteAnyRow_Throws42501()`
- Line 52: `public async Task SettingNile_Truncate_Throws42501()`
- Line 67: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 84: `public async Task SettingMaadi_SelectsNoneOfNilesRows()`
- Line 100: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 122: `public async Task ThroughEf_LoadingThenModifyingAnAuditEntry_TenantGuardThrows()`

### `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditPolicyTests.cs`

Task 31.4: the allow-list is asserted literally, never by reflecting over the audited entities — a property the reflection would find is exactly what this policy must be free to leave unaudited.

Declared test entry points (declarations are not execution results):

- Line 16: `public void Entries_HasExactlyElevenAuditedTypes()`
- Line 33: `public void Subject_IsAuditedAsSubject_WithNameAndStatusOnly()`
- Line 42: `public void Membership_IsAuditedAsMembership_WithUserIdRoleAndStatusOnly()`
- Line 51: `public void Centre_IsAuditedAsCentre_WithNameAndDefaultLocaleOnly()`
- Line 60: `public void Subject_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 69: `public void Membership_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 78: `public void Centre_ResolvesItsCentreId_FromItsOwnId()`
- Line 87: `public void Entries_NeverIncludesAuditEntriesOrIdentityUsers()`
- Line 94: `public void Parent_IsAuditedAsParent_WithFullNameAndMaskedPhoneOnly()`
- Line 104: `public void Student_IsAuditedAsStudent_WithFullNameSchoolYearStatusAndMaskedPhoneOnly()`
- Line 114: `public void StudentParentLink_IsAuditedAsStudentParent_WithStudentIdParentIdRelationshipAndIsPrimaryOnly()`
- Line 124: `public void Parent_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 133: `public void Student_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 143: `public void Teacher_IsAuditedAsTeacher_WithFullNameUserIdStatusAndMaskedPhoneOnly()`
- Line 153: `public void Teacher_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 162: `public void Group_IsAuditedAsGroup_WithNameSubjectIdTeacherIdCapacityAndStatusOnly()`
- Line 172: `public void Group_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 182: `public void Enrollment_IsAuditedAsEnrollment_WithStudentIdGroupIdStatusAndEndReasonOnly()`
- Line 192: `public void Enrollment_ResolvesItsCentreId_FromItsOwnCentreIdProperty()`
- Line 202: `public void ScheduleRule_IsAuditedAsScheduleRule_ForEveryAction_WithItsLocalValuesOnly()`
- Line 216: `public void ClassSession_IsAuditedAsClassSession_OnUpdateOnly_WithStatusAndTeacherIdOnly()`
- Line 228: `public void EveryEntryWithoutAnActionList_RecordsEveryAction_AsBeforeActionListsExisted()`

### `tests/TutoringCentre.Infrastructure.Tests/Audit/AuditReadServiceTests.cs`

Task 32.2: the audit read side against real PostgreSQL, through the real dispatcher (so row-level security and the EF query filter are exercised exactly as a real request would hit them), with rows seeded directly by so timing and ordering are fully controlled.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task GetPageAsync_With120SeededEntries_PagesAsFiftyFiftyTwenty_WithNoDuplicatesOrGaps()`
- Line 54: `public async Task GetPageAsync_FilteredByEntityType_ReturnsOnlyThatType()`
- Line 71: `public async Task GetPageAsync_FilteredByEntityId_ReturnsOnlyThatRecordsHistory()`
- Line 87: `public async Task GetPageAsync_FilteredByActorUserId_ReturnsOnlyThatActorsRows()`
- Line 106: `public async Task GetPageAsync_FilteredByFromAndTo_ReturnsOnlyEntriesInRange()`
- Line 123: `public async Task GetPageAsync_WithCombinedFilters_AppliesAllOfThem()`
- Line 142: `public async Task GetPageAsync_AsMaadiActor_NeverReceivesANileEntry()`
- Line 167: `public async Task GetPageAsync_EntryInsertedBetweenPageRequests_DoesNotShiftTheNextPage()`

### `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSettingsTests.cs`

Task 32.10: proves updating centre settings against real PostgreSQL — isolation, the audit row it produces, and concurrency.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task UpdateCentreSettings_AsNileOwner_ChangesNileOnly_MaadiRowUnchanged()`
- Line 37: `public async Task UpdateCentreSettings_WritesExactlyOneCentreUpdatedAuditRowWithNameBeforeAndAfter()`
- Line 67: `public async Task UpdateCentreSettings_WithStaleVersion_ReturnsConcurrencyStale()`

### `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs`

TOCTOU (time-of-check to time-of-use): the application checks "slug free?" and then inserts. Two requests can both see "free". The unique index is the real guarantee. Task 25.3/25.4 closes the gap this test used to document: the loser now always receives a translated centre.slug_taken Result, never an unhandled exception.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task SendAsync_TwoParallelCreatesWithSameSlug_LeavesExactlyOneRowAndLoserGetsSlugTakenConflict()`

### `tests/TutoringCentre.Infrastructure.Tests/Centres/CentreTimeZoneProviderTests.cs`

Task 51.6: proves the adapter's hand-written centre predicate follows the actor — platform.centres has no EF query filter and no row-level security, so nothing underneath narrows it automatically. Nile and Maadi are both Africa/Cairo, so a third centre in a different zone is created: with two centres sharing a zone, a lookup that ignored the actor could still pass.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task GetForCurrentCentreAsync_ActorsOfTwoCentres_EachGetTheirOwnCentresZone()`
- Line 35: `public async Task GetForCurrentCentreAsync_CalledTwiceInOneScope_ReadsTheCentreOnce()`

### `tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs`

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task SendAsync_SystemActorValidCommand_PersistsCentreWithCreatedAtAndNoUpdatedAt()`
- Line 27: `public async Task SendAsync_SlugAlreadyExists_ReturnsConflictAndKeepsOneRow()`
- Line 40: `public async Task SendAsync_AnonymousActor_ReturnsForbiddenAndWritesNothing()`
- Line 51: `public async Task SendAsync_InvalidTimeZone_ReturnsDomainErrorAndWritesNothing()`

### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs`

Dispatches a use case in a new scope as the given actor — a scope per call, like a request or job.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs`

All real-database tests share one container (started once per run); tests in a collection run sequentially.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs`

One throwaway PostgreSQL 17 container per test run, on the same three-role split as everywhere else (Day 19): roles bootstrapped by the real db/bootstrap-roles.sh, migrated with the REAL migrations as tutoring_owner, application services registered on tutoring_app. The container's superuser connection is never used by the app under test — only to bootstrap roles and to reset between tests.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs`

Base class for real-database tests: every test starts with empty tables.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/ProbeScopeExtensions.cs`

A DI scope holding a TenantProbeDbContext for one actor, with a real transaction already begun through the real IUnitOfWork — exactly the production pipeline, so row-level security (Task 21.3) sees the same app.current_centre setting a real request would set. Disposing commits (or rolls back, if the transaction was left aborted by a thrown exception) and ends the scope.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs`

Runs the real db/bootstrap-roles.sh (Day 19) inside a throwaway Testcontainers PostgreSQL instance, with test-only passwords, so the test fixture exercises the exact same role split as production instead of a stand-in for it.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs`

Proves the authentication adapter's lockout and no-enumeration behaviour against real PostgreSQL.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task VerifyCredentialsAsync_CorrectPassword_SucceedsWithUserIdAndStamp()`
- Line 32: `public async Task VerifyCredentialsAsync_WrongPassword_FailsAndRecordsOneFailedAttempt()`
- Line 47: `public async Task VerifyCredentialsAsync_UnknownEmail_FailsWithTheIdenticalError()`
- Line 60: `public async Task VerifyCredentialsAsync_FiveWrongAttemptsThenCorrectPassword_StillFails()`
- Line 80: `public async Task VerifyCredentialsAsync_SuccessAfterAFailure_ResetsTheCounter()`
- Line 96: `public async Task ChangePasswordAsync_CorrectCurrentAndValidNew_RotatesStampClearsFlagAndAppliesPolicy()`
- Line 116: `public async Task ChangePasswordAsync_WrongCurrentPassword_FailsWithoutRotatingTheStampOrClearingTheFlag()`
- Line 131: `public async Task ChangePasswordAsync_NewPasswordFailsThePolicy_FailsWithFieldError()`
- Line 145: `public async Task ChangePasswordAsync_NewPasswordSameAsCurrent_FailsAsTooWeak()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs`

Proves the database itself — not just application code — prevents duplicate, orphaned or cascaded identity data.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task Migrations_AppliedToEmptyDatabase_CreateExactlyTheFiveIdentityTablesAndNoRoleTables()`
- Line 37: `public async Task SaveChanges_SecondMembershipForSameCentreAndUser_ViolatesUniqueIndex()`
- Line 55: `public async Task SaveChanges_MembershipForNonExistentCentre_ViolatesForeignKey()`
- Line 69: `public async Task Delete_CentreWithAMembership_IsRejected()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs`

Proves the read side against real PostgreSQL, using the Day 13 staff set.

Declared test entry points (declarations are not execution results):

- Line 18: `public async Task GetProfileAsync_TwoCentreTeacher_SeesBothCentres()`
- Line 31: `public async Task GetProfileAsync_Secretary_SeesOneCentre()`
- Line 44: `public async Task GetProfileAsync_InactiveMember_SeesNoCentres()`
- Line 57: `public async Task GetActiveMembershipAsync_OwnerAskingForAForeignCentre_ReturnsNull()`
- Line 69: `public async Task GetProfileAsync_NeverIncludesAnotherUsersMemberships()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountServiceTests.cs`

Task 29.4: proves IStaffAccountService over real PostgreSQL, including that it rolls back with the command that calls it.

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task EnsureAccountAsync_NewEmail_CreatesOneUserWithAHashedPasswordAndTheMustChangeFlagSet()`
- Line 35: `public async Task EnsureAccountAsync_NewEmail_TheReturnedTemporaryPasswordVerifiesThroughTheAuthenticationPort()`
- Line 49: `public async Task EnsureAccountAsync_ExistingEmail_ReturnsTheSameIdCreatesNothingAndLeavesTheProfileUntouched()`
- Line 70: `public async Task AFailingCommandAfterAccountCreation_LeavesNoUser()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffAccountTestRequests.cs`

Task 29.4: a test-only command that ensures an account exists, then always fails — standing in for CreateStaffCommand (Day 29.5) so the rollback test can prove a new account does not survive a command that fails afterward, before that command exists.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffCrossTenantTests.cs`

Task 29.12: with no row-level security and no EF query filter on identity.memberships (the Day 29 contract's documented exemption), these are the only proof that staff data does not leak across centres. Maadi always uses Nile's real membership ID and real version, never an invented one.

Declared test entry points (declarations are not execution results):

- Line 21: `public async Task ChangeRole_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 38: `public async Task Deactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 55: `public async Task Reactivate_MaadiOwnerForNileMembershipWithItsRealVersion_ReturnsNotFoundAndLeavesRowUnchanged()`
- Line 76: `public async Task List_MaadiOwner_ContainsNoNileOnlyUser()`
- Line 88: `public async Task List_TwoCentreTeacher_AppearsOnceInEachCentresListWithThatCentresRole()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffDirectoryTests.cs`

Task 43.3: proves the adapter writes its own centre predicate — identity.memberships has no EF query filter and no row-level security, so nothing underneath this port narrows it automatically — and answers true only for an active member of that exact centre with that exact role.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task IsActiveMemberWithRoleAsync_TwoCentreTeacherAskedAboutTheirTeachingCentre_ReturnsTrue()`
- Line 32: `public async Task IsActiveMemberWithRoleAsync_SecretaryAskedAboutTeacherRole_ReturnsFalse()`
- Line 42: `public async Task IsActiveMemberWithRoleAsync_InactiveNileTeacher_ReturnsFalse()`
- Line 56: `public async Task IsActiveMemberWithRoleAsync_MaadiOnlyTeacherAskedAboutNile_ReturnsFalse()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffProbeTestBase.cs`

Shared setup for Task 29.12's staff integration tests. The first owner of a freshly created centre is bootstrapped as a SystemActor — the same actor kind the seed CLI uses — since CreateStaffCommand itself requires staff.manage, which no one holds in a brand-new centre yet.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffRaceTests.cs`

Task 29.12: the two concurrency invariants only a real database, under real concurrent transactions, can prove — the last-active-owner lock and the membership uniqueness constraint. Each attempt dispatches in its own scope (its own context and connection), never sharing one, exactly like SubjectRaceTests.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task ChangeRole_TwoOwnersDemotingEachOtherSimultaneously_ExactlyOneSucceedsAcrossTwentyRepetitions()`
- Line 72: `public async Task Create_TwoParallelCreatesForTheSameEmailInTheSameCentre_LeavesExactlyOneMembershipAndTheLoserGetsAlreadyMember()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/StaffUseCaseTests.cs`

Task 29.12: the five staff use cases exercised through the real dispatcher, the real Infrastructure adapters and real PostgreSQL.

Declared test entry points (declarations are not execution results):

- Line 19: `public async Task Create_NewEmailAsSystemActor_CreatesAnAccountAndAnActiveMembershipWithATemporaryPassword()`
- Line 37: `public async Task ChangeRole_ActiveTeacherToSecretary_Succeeds()`
- Line 53: `public async Task Deactivate_ActiveTeacher_Succeeds()`
- Line 68: `public async Task Reactivate_DeactivatedTeacher_Succeeds()`
- Line 88: `public async Task List_AsNileOwner_ReturnsEveryNileMemberOrderedByDisplayName()`
- Line 102: `public async Task ChangeRole_AgainWithTheSameStaleVersion_ReturnsConcurrencyStale()`
- Line 123: `public async Task EnsureAccountThenFail_NewEmail_RollsBackTheAccountLeavingNoUser()`

### `tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs`

Task 44.4: the ownership guard resolved through the real provider, the real teacher read service and real PostgreSQL, inside a dispatched request so row-level security is in force. Fails closed: every case that is not explicitly All or Own must come back None.

Declared test entry points (declarations are not execution results):

- Line 23: `public async Task SystemActor_ResolvesAll()`
- Line 31: `public async Task Owner_ResolvesAll()`
- Line 41: `public async Task Secretary_ResolvesAll()`
- Line 51: `public async Task TeacherLinkedToActiveRecord_ResolvesOwnWithThatId()`
- Line 63: `public async Task TeacherWithNoLinkedRecord_ResolvesNone()`
- Line 73: `public async Task TeacherLinkedToArchivedRecord_ResolvesNone()`
- Line 88: `public async Task AnonymousActor_ResolvesNone()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/DatabaseOptionsValidationTests.cs`

Pure options-binding tests: no database is needed, since these assert on configuration shape, not connectivity.

Declared test entry points (declarations are not execution results):

- Line 16: `public void ResolvingDatabaseOptions_WithoutRuntimeConnectionString_ThrowsNamingTheKey()`
- Line 27: `public void ResolvingDatabaseOptions_WithOnlyRuntimeConnectionString_Succeeds()`
- Line 38: `public async Task ApplyMigrationsAsync_WithoutMigrationsConnectionString_ThrowsNamingTheKey()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs`

A brand-new environment (CI, production) must be buildable from the migrations alone.

Declared test entry points (declarations are not execution results):

- Line 9: `public async Task Migrations_AppliedToEmptyDatabase_CreateCentresTable()`
- Line 17: `public async Task Migrations_AppliedToEmptyDatabase_CreateUniqueSlugIndex()`
- Line 27: `public async Task Migrations_AppliedToEmptyDatabase_CreateDefaultLocaleCheckConstraint()`
- Line 36: `public async Task Migrations_AppliedToEmptyDatabase_KeepHistoryTableInPlatformSchema()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs`

Declared test entry points (declarations are not execution results):

- Line 12: `public void TryTranslate_UniqueViolationOnRegisteredConstraint_ReturnsMappedConflict()`
- Line 24: `public void TryTranslate_UniqueViolationOnParentPhoneConstraint_ReturnsParentPhoneTaken()`
- Line 36: `public void TryTranslate_UniqueViolationOnStudentParentConstraint_ReturnsParentAlreadyLinked()`
- Line 48: `public void TryTranslate_UniqueViolationOnActiveEnrollmentIndex_ReturnsAlreadyEnrolledConflict()`
- Line 61: `public void TryTranslate_UniqueViolationOnOnePrimaryIndex_IsNotTranslated()`
- Line 71: `public void TryTranslate_UniqueViolationOnUnregisteredConstraint_ReturnsGenericDuplicate()`
- Line 83: `public void TryTranslate_ForeignKeyViolation_IsNotTranslated()`
- Line 94: `public void TryTranslate_CheckViolation_IsNotTranslated()`
- Line 105: `public void TryTranslate_ConcurrencyException_ReturnsConcurrencyStale()`
- Line 117: `public void TryTranslate_UnrelatedException_IsNotTranslated()`
- Line 127: `public void TryTranslate_ExclusionViolationOnTeacherOverlap_ReturnsTeacherDoubleBookedConflict()`
- Line 141: `public void TryTranslate_ExclusionViolationOnGroupOverlap_ReturnsGroupDoubleBookedConflict()`
- Line 154: `public void TryTranslate_ExclusionViolationOnUnlistedConstraint_IsNotTranslated()`
- Line 164: `public void TryTranslate_UniqueViolationOnSessionRuleDate_IsNotTranslated()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs`

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task QueryAsync_HandlerAttemptsInsert_PostgresRejectsItAndNoRowExists()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/RuntimeRolePrivilegeTests.cs`

Raw-SQL evidence of what the runtime role can and cannot do (Day 19). Valid evidence only because the fixture itself runs the app under test as tutoring_app — a test connecting as a superuser could never fail these. Every denial asserts the SQLSTATE (42501, permission denied), not merely that something was thrown.

Declared test entry points (declarations are not execution results):

- Line 17: `public async Task CurrentUser_ThroughTheAppConnection_IsTutoringAppAndUnprivileged()`
- Line 33: `public async Task CreateTable_InPlatformSchema_IsDenied() =>`
- Line 37: `public async Task AlterTable_Centres_IsDenied() =>`
- Line 41: `public async Task Delete_FromCentres_IsDenied() =>`
- Line 45: `public async Task Insert_IntoMigrationsHistory_IsDenied() =>`
- Line 51: `public async Task SelectAndInsert_OnMemberships_Succeed()`
- Line 82: `public async Task Update_DataProtectionKeys_IsDenied() =>`
- Line 86: `public async Task Delete_FromDataProtectionKeys_IsDenied() =>`
- Line 90: `public async Task AllPlatformAndIdentityTables_AreOwnedByTutoringOwner()`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/TenantRowLevelSecurityTests.cs`

Task 21.3: pure SQL-generation tests for the RLS migration helper — no database involved.

Declared test entry points (declarations are not execution results):

- Line 11: `public void BuildEnableStatements_ForATable_IssuesEnableForceAndOnePolicyWithBothClausesAndNoToClause()`
- Line 25: `public void BuildDisableStatements_ForATable_DropsThePolicyThenUnforcesThenDisables()`
- Line 42: `public void BuildEnableStatements_UnsafeSchemaName_Throws(string schema)`
- Line 51: `public void BuildEnableStatements_UnsafeTableName_Throws(string table)`

### `tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs`

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task SendAsync_HandlerThrowsAfterRowWasWritten_LeavesNoRowAndRethrows()`
- Line 27: `public async Task SendAsync_HandlerReturnsFailureAfterRowWasWritten_LeavesNoRow()`
- Line 40: `public async Task SendAsync_HandlerSucceedsAfterRowWasWritten_PersistsTheRow()`

### `tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public async Task QueryAsync_MigratedDatabase_ReportsLatestMigrationAsUpToDate()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRawSqlRowLevelSecurityTests.cs`

Task 21.4: proves the row-level security policy itself (Task 21.3) against raw connections as tutoring_app, independent of EF, UnitOfWork or the application pipeline — the database-level guarantee the other layers depend on actually existing.

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task NoSetting_SelectCount_IsZero()`
- Line 27: `public async Task SettingNile_SelectsOnlyNileRows()`
- Line 44: `public async Task SettingMaadi_SelectsOnlyMaadiRows()`
- Line 61: `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- Line 78: `public async Task SettingNile_UpdateCentreIdToMaadiOnNileRow_Throws42501()`
- Line 96: `public async Task SettingNile_UpdateAndDeleteOnMaadiRow_AffectZeroRowsAndLeaveItIntact()`
- Line 126: `public async Task NoSetting_Insert_Throws42501()`
- Line 142: `public async Task SettingIsNotAUuid_StatementErrorsAndReturnsNoRows()`
- Line 153: `public async Task AsOwnerWithNoSetting_SelectCount_IsZero()`
- Line 166: `public async Task AsAppWithRowSecurityOff_SelectErrorsRatherThanReturningUnfilteredRows()`
- Line 184: `public async Task SettingFromACommittedTransaction_IsAbsentInTheNextTransactionOnTheSameConnection()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/ProbeRowLevelSecurityCatalogueTests.cs`

Task 21.3: confirms the fixture actually applied row-level security to probe.tenant_probes through Postgres's own catalogue, not just that TenantRowLevelSecurityTests' statement text looks right in isolation.

Declared test entry points (declarations are not execution results):

- Line 13: `public async Task ProbeTable_RowSecurityIsEnabledAndForced()`
- Line 28: `public async Task ProbeTable_HasExactlyOneTenantIsolationPolicyForAllCommandsWithNoToClause()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantContextLifetimeTests.cs`

Task 21.2: app.current_centre's lifetime — set per transaction, read back through a real dispatched request.

Declared test entry points (declarations are not execution results):

- Line 15: `public async Task Query_AsNileActor_SeesNilesId()`
- Line 25: `public async Task Query_AsMaadiActor_SeesMaadisId()`
- Line 35: `public async Task Query_AsActorWithoutACentre_SeesEmptyString()`
- Line 45: `public async Task Dispatch_NileThenNoCentre_OnTheSameConnection_SecondSeesEmpty()`
- Line 78: `public async Task AfterAFailedCommandRollsBack_ANewTransactionOnTheSameConnection_SeesNoValue()`
- Line 102: `public async Task Dispatch_AsCommandOrQuery_BothSeeTheSetting()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIndependenceProbeRequests.cs`

Task 21.5: test-only requests that each depend on exactly one isolation layer, so a layer's own test can be proven to hold even when the other layer is bypassed or absent.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantIsolationIndependenceTests.cs`

Task 21.5: the EF query filter (Day 20) and row-level security (Task 21.3) are each a complete control on their own — each test here removes or bypasses the OTHER layer and proves the one layer under test still holds.

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task BypassingTheEfFilter_AsNileActor_StillReturnsOnlyNileRows()`
- Line 29: `public async Task EfFilterAlone_WithSuperuserConnectionInPlaceOfApp_StillReturnsOnlyNileRows()`
- Line 47: `public async Task RawSqlInsideADispatchedQuery_AsNileActor_StillReturnsOnlyNileRows()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbe.cs`

Test-only entity (Task 20.3) that proves the tenant query filter convention and write guard work on any ITenantOwned type without entity-specific code, not just whatever production happens to ship first.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChild.cs`

Task 21.6: a tenant-owned child of TenantProbe, referencing it by (CentreId, ProbeId) — a composite foreign key — rather than by ProbeId alone, so a row naming a real probe in a different centre cannot be stored.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeChildConfiguration.cs`

Maps TenantProbeChild to probe.tenant_probe_children, created directly by SQL in PostgresFixture — never by a migration.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeCompositeForeignKeyTests.cs`

Task 21.6: the composite foreign key on probe.tenant_probe_children makes a cross-centre reference impossible to store — proven as superuser, so nothing about role privilege (Day 19) or row-level security (Task 21.3) is what's actually stopping it.

Declared test entry points (declarations are not execution results):

- Line 14: `public async Task InsertChildWithNilesCentreAndAMaadiProbesId_AsSuperuser_Throws23503()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeConfiguration.cs`

Maps TenantProbe to probe.tenant_probes, created directly by SQL in PostgresFixture — never by a migration.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeDbContext.cs`

Extends the production AppDbContext (Task 20.3) with the test-only TenantProbe mapping, so the production tenant query filter convention, interceptors and unit of work all apply to it unchanged.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeSmokeTests.cs`

Task 20.3's closing proof: the probe table is reachable end to end, and the tenant query filter convention (Task 20.2) really does translate to a parameterized WHERE clause on centre_id, not a client-side filter.

Declared test entry points (declarations are not execution results):

- Line 16: `public async Task SeedProbeAsync_TwoRowsPerCentre_CountsFourAsSuperuser()`
- Line 29: `public async Task ListQuery_AsNileActor_FiltersCentreIdWithAParameter()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantProbeTestBase.cs`

Shared setup for Tasks 20.5/20.6: seeds Nile and Maadi fresh for every test (tables start empty — Day 19's PostgresTestBase), and offers the one helper every filter/guard test uses to set its actor.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantQueryFilterTests.cs`

Task 20.5: the tenant query filter convention (Task 20.2), proven on the probe entity against real PostgreSQL.

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task List_AsNileStaffActor_ReturnsOnlyNileRows()`
- Line 26: `public async Task List_AsMaadiStaffActor_ReturnsOnlyMaadiRows()`
- Line 40: `public async Task List_AsStaffActorWithNoCentre_ReturnsZeroRows()`
- Line 52: `public async Task List_AsAnonymousActor_ReturnsZeroRows()`
- Line 64: `public async Task FindById_MaadiProbeAsNileActor_FindsNothing()`
- Line 75: `public async Task CountAndAny_AsNileActor_RespectTheFilter()`
- Line 89: `public async Task List_TwoScopesDifferentActorsInParallel_EachSeesOnlyItsOwnRows()`

### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantSettingProbeRequests.cs`

Task 21.2: test-only requests that read app.current_centre back from inside a real dispatched request, proving what UnitOfWork.BeginAsync (Task 21.1) actually set for that transaction — not a stand-in assertion.

Declared test entry points (declarations are not execution results):



### `tests/TutoringCentre.Infrastructure.Tests/Tenancy/TenantWriteGuardTests.cs`

Task 20.6: the tenant write guard interceptor (Task 20.4), proven on the probe entity against real PostgreSQL.

Declared test entry points (declarations are not execution results):

- Line 12: `public async Task Add_NileActorAddsNileProbe_Saved()`
- Line 26: `public async Task Add_NileActorAddsProbeCarryingMaadiCentre_ThrowsAndWritesNothing()`
- Line 37: `public async Task Add_ActorWithoutCentre_Throws()`
- Line 48: `public async Task Modify_TrackedNileProbeCentreChangedToMaadi_EfItselfRefusesBeforeTheGuardEverRuns()`
- Line 65: `public async Task Delete_MaadiProbeAttachedAndDeletedByNileActor_ThrowsAndLeavesRowPresent()`
- Line 78: `public async Task Add_SystemActorWithCentreSucceeds_SystemActorWithoutCentreThrows()`

### `tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs`

Declared test entry points (declarations are not execution results):

- Line 11: `public void UtcNow_ReturnsTheTimeProvidersTime()`
- Line 25: `public void FromLocal_InCairoWinter_UsesUtcPlusTwo()`
- Line 39: `public void FromLocal_InCairoSummer_UsesUtcPlusThree()`
