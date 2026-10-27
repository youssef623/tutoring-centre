/** The closed set of permission keys (mirrors the backend's Domain-level `Permissions` catalogue, Day 28). */
export const Permissions = {
  SubjectsView: "subjects.view",
  SubjectsManage: "subjects.manage",
  StaffView: "staff.view",
  StaffManage: "staff.manage",
  AuditView: "audit.view",
  CentreSettingsManage: "centre.settings.manage",
  StudentsView: "students.view",
  StudentsManage: "students.manage",
  TeachersView: "teachers.view",
  TeachersManage: "teachers.manage",
  GroupsView: "groups.view",
  GroupsManage: "groups.manage",
  EnrollmentsView: "enrollments.view",
  EnrollmentsManage: "enrollments.manage",
  ScheduleView: "schedule.view",
  ScheduleManage: "schedule.manage",
  AttendanceView: "attendance.view",
  AttendanceMark: "attendance.mark",
  AttendanceCorrect: "attendance.correct",
} as const;

export type Permission = (typeof Permissions)[keyof typeof Permissions];
