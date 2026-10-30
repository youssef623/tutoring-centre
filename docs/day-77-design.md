# Day 77 design specification

The portal chat extends Hessa's existing white and teal interface. The primary concept is `C:/Users/youss/.codex/generated_images/01a18d5e-cf5e-7153-98b5-25e78725a8f6/exec-af0837ef-9274-470a-838e-d31a75b5895c.png`.

Use existing semantic tokens, Geist and IBM Plex Sans Arabic, 12 px panel radius, thin neutral borders, and Lucide outline icons. Keep the existing wordmark, language switcher, user menu, and portal navigation. The chat panel has one heading, a chronological message list, and a bottom composer. Family messages align at inline end with teal backgrounds; replies align at inline start with muted backgrounds. Timestamps use caption text. No raster assets belong in the implemented UI.

Required states: loading, empty with “Ask about attendance”, pending, stored, send failure with retry, rate limited, and older-message loading. Keep drafts and retry identifiers only in component state. The composer includes a label, counter and Send button. Mobile uses full available width with the existing wrapped portal header. Staff use the existing shell with a conversation list, selected thread, status chips and an accessible details dialog.

Allowed visible copy comes from portal translations and existing shell translations. Participant names and message texts come from the API. The concept's example reply is design data, never a fabricated runtime reply. Preserve the existing brand mark rather than replacing it with the generated wordmark.

## Verification status

The browser access host was unavailable. Native Chrome automation also stopped because it could not verify the current URL. The user explicitly deferred visual QA rather than authorizing an alternative browser automation method. No implementation screenshot, native-size comparison, responsive viewport inspection, or visual fidelity sign-off is claimed. Visual comparison remains pending for layout, typography, palette, message alignment, composer spacing, and mobile overflow. Automated component tests cover sends, retries, plain-text rendering, visibility-aware polling, statuses, and the details drawer.
