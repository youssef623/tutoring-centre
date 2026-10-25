import { describe, expect, it } from "vitest";
import { addDays, isIsoDate, startOfWeek, weekDates, weekdayOf } from "@/features/schedule/week";

describe("week", () => {
  it("the start of the week for a Wednesday is the previous Saturday", () => {
    expect(startOfWeek("2027-04-28")).toBe("2027-04-24");
  });

  it("a Saturday is its own week start, and a Friday belongs to the Saturday six days before", () => {
    expect(startOfWeek("2027-04-24")).toBe("2027-04-24");
    expect(startOfWeek("2027-04-30")).toBe("2027-04-24");
  });

  it("adding 7 days across Cairo's 30 April clock change keeps the weekday and the date arithmetic exact", () => {
    expect(addDays("2027-04-24", 7)).toBe("2027-05-01");
    expect(weekdayOf("2027-05-01")).toBe("saturday");
    expect(addDays("2027-10-23", 7)).toBe("2027-10-30");
    expect(weekdayOf("2027-10-30")).toBe("saturday");
  });

  it("lists the seven days of a week from Saturday, across a month end", () => {
    expect(weekDates("2027-04-24")).toEqual([
      "2027-04-24",
      "2027-04-25",
      "2027-04-26",
      "2027-04-27",
      "2027-04-28",
      "2027-04-29",
      "2027-04-30",
    ]);
    expect(addDays("2027-03-01", -1)).toBe("2027-02-28");
  });

  it("accepts real calendar dates only", () => {
    expect(isIsoDate("2027-04-24")).toBe(true);
    expect(isIsoDate("2027-02-30")).toBe(false);
    expect(isIsoDate("24-04-2027")).toBe(false);
    expect(isIsoDate("2027-04-24T00:00:00Z")).toBe(false);
  });
});
