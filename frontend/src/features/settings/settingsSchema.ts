import { z } from "zod";
import { SupportedLocale } from "@/api/generated/tutoring-centre";

/** Client-side convenience only; the server repeats the rule (Centre.UpdateSettings). Messages are translation keys, resolved within the "settings" namespace. */
export const centreSettingsFormSchema = z.object({
  name: z.string().trim().min(1, "validation.nameRequired").max(120, "validation.nameTooLong"),
  defaultLocale: z.enum([SupportedLocale.ar, SupportedLocale.en]),
});

export type CentreSettingsFormValues = z.infer<typeof centreSettingsFormSchema>;
