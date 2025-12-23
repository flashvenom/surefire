# Clients
Clients is the 360-degree view for an account. It brings contacts, policies, renewals, attachments, forms, and notes into a single workspace.

## Daily workflows
- **Search and select** - left-side list and FireSearch jump into a client quickly
- **Create new records** - toolbar actions for policies, forms, certificates, and contacts
- **Overview tab** - logo, quick stats, current policies, and primary contacts
- **Attachments + Forms** - drag-and-drop uploads and form revisions attached to the client
- **Notes** - global notes stay pinned to the client record

## Technical map
| Area | Files |
| --- | --- |
| Page + layout | `src/Quickfire.Blazor/Domain/Clients/Pages/Clients.razor (+ .cs, .razor.css)` |
| Toolbar + Outlook helpers | `src/Quickfire.Blazor/Domain/Clients/Components/_toolbar.razor` + `Domain/Ember/EmberService.cs` |
| Header + quick stats | `src/Quickfire.Blazor/Domain/Clients/Components/ClientHeader.razor` |
| Policies tab | `src/Quickfire.Blazor/Domain/Policies/Components/PolicyListGrid.razor` |
| Attachments tab | `src/Quickfire.Blazor/Domain/Attachments/Components/DropzoneContainer.razor`, `AttachmentListGrid.razor` |
| Forms tab | `src/Quickfire.Blazor/Domain/Forms/Components/FormDocList.razor` |
| Notes | `src/Quickfire.Blazor/Domain/Shared/Components/GlobalNotes.razor` |

## Configuration
- Outlook actions require `Quickfire.Tray` running and connected to `/emberHub`.
- Attachment storage defaults to local disk; see [[features/Files-and-Attachments]].

## Implementation tips
1. Keep tab state in `ClientStateService.ActiveTab` so navigation stays consistent.
2. Use `StateService` caches (`AllCarriers`, `AllProducts`) for lookups instead of re-querying.
3. Reuse `StringHelper` utilities for names, links, and file paths.

Also see: [[features/Renewals-and-Submissions]] for renewal data on client tabs and [[reference/Word-Doc-Integration]] for optional Word/Outlook tooling.
