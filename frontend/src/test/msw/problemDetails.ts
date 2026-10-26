import { HttpResponse } from "msw";

const TitleByStatus: Record<number, string> = {
  400: "One or more validation errors occurred.",
  403: "You are not allowed to perform this action.",
  404: "The requested resource was not found.",
  409: "The request conflicts with the current state.",
  422: "The request breaks a business rule.",
};

/**
 * A Problem Details response shaped like the API's own (ResultHttpExtensions + ProblemResult.Enrich): status,
 * title by kind, the authored detail, the stable `code`, and the trace and correlation IDs.
 */
export function problemDetails(status: number, code: string, detail: string) {
  return HttpResponse.json(
    {
      type: `https://tools.ietf.org/html/rfc9110#section-15.5.${status === 422 ? "21" : String(status - 399)}`,
      title: TitleByStatus[status] ?? "Error",
      status,
      detail,
      code,
      traceId: "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-01",
      correlationId: "test-correlation-id",
    },
    { status, headers: { "Content-Type": "application/problem+json" } },
  );
}

/** 409 when the last seat went to someone else between loading the page and submitting. */
export const groupFullProblem = () => problemDetails(409, "group.full", "This group has no free seats.");

/** 409 when the student already holds an active enrollment in the group. */
export const alreadyEnrolledProblem = () =>
  problemDetails(409, "enrollment.already_enrolled", "This student is already enrolled in this group.");
