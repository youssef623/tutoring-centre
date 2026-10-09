/** The closed set of permission keys (mirrors the backend's Domain-level `Permissions` catalogue, Day 28). */
export const Permissions = {
  SubjectsView: "subjects.view",
  SubjectsManage: "subjects.manage",
  StaffView: "staff.view",
  StaffManage: "staff.manage",
  AuditView: "audit.view",
  CentreSettingsManage: "centre.settings.manage",
} as const;

export type Permission = (typeof Permissions)[keyof typeof Permissions];
