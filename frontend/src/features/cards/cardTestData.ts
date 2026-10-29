import { http, HttpResponse } from "msw";
import { group, me, roster, rosterEntry } from "@/features/enrollments/enrollmentTestData";
import type { IssuedCard, StudentCardDto, StudentDetailDto } from "@/api/generated/tutoring-centre";
import { server } from "@/test/msw/server";

export const Token = `TCM1.${"A".repeat(43)}`;
export const Card: StudentCardDto = { cardId: "card-1", status: "active", issuedAt: "2026-12-05T10:00:00Z", issuedByName: "Mona", revocationReason: null, revokedAt: null, version: 17 };
export const Student: StudentDetailDto = { id: "st-1", fullName: "Ahmed Mahmoud", phone: null, schoolYear: null, status: "active", version: 3, parents: [{ parentId: "p-1", fullName: "Mahmoud", phone: "+201000000001", relationship: "father", isPrimary: true }] };
export const Secretary = me("secretary", ["students.view", "groups.view", "attendance.cards.manage"]);
export const Teacher = me("teacher", ["groups.view", "enrollments.view"]);
export const CardUrl = "/api/students/st-1/card";
export const BulkUrl = "/api/groups/g-1/cards";
export const RevokeUrl = "/api/student-cards/card-1/revoke";
export const RosterUrl = "/api/groups/g-1/roster";
export function issued(studentId = "st-1"): IssuedCard {
  return { cardId: `card-${studentId}`, studentId, studentName: studentId === "st-1" ? Student.fullName : studentId, token: Token, issuedAt: Card.issuedAt };
}
export function serveCards(card: StudentCardDto | null = null, student = Student) {
  server.use(
    http.get("/api/me", () => HttpResponse.json(Secretary)),
    http.get("/api/students/st-1", () => HttpResponse.json(student)),
    http.get("/api/students/st-1/enrollments", () => HttpResponse.json({ items: [] })),
    http.get(CardUrl, () => HttpResponse.json({ card })),
    http.get("/api/groups/g-1", () => HttpResponse.json(group())),
    http.get(RosterUrl, () => HttpResponse.json(roster())),
  );
}
export { group, roster, rosterEntry };
