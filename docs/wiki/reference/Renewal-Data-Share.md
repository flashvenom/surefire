Keep renewals, submissions, and tasks in sync by following these automation hooks. Whenever you touch the referenced files, update this doc.

## Status and task automation
- **Renewal status dropdown** - `Domain/Renewals/Pages/Details.razor` binds to `RenewalStatusOptions` in `Details.razor.cs` (Pending, Unneeded, Defected, Ghosted, Unplaceable, Purple, Yellow, Green).
- **Status changes** - `OnRenewalStatusChanged` persists the renewal and logs a `RenewalNote` (type `RenewalUpdate`).
- **Task completion rule** - when a task name contains "renew in epic", "renewed in epic", or "epic renewal", `CheckForRenewalStatusUpdate` sets the renewal to Renewed and logs a system note.

## Submission updates
- **Stepper updates** - `Submissions.razor.cs` updates `Submission.StatusInt` and `Submission.Status` via `StringHelper.GetSubmissionStatus`.
- **Audit trail** - status changes add a `RenewalNote` (type `SubmissionLog`) with carrier/wholesaler context.

## Implementation checklist
1. Log changes via `RenewalNote` for auditability.
2. Use `RenewalService.UpdateRenewalAsync` and `TaskService.UpdateTrackTaskModelAsync` to avoid partial writes.
3. Keep status labels and CSS classes in `Details.razor` consistent with `RenewalStatusOptions`.
