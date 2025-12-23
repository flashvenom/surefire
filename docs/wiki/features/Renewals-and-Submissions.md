# Renewals and Submissions
Renewals coordinate expiring policies, task lists, and carrier submissions. They are the glue between policies, clients, and workflow tracking.

## Overview
- **Board view** shows upcoming renewals by month/user (`Domain/Renewals/Pages/Renewals.razor`)
- **Tasks** are seeded from master templates and tracked per renewal
- **Submissions** group quotes by carrier/wholesaler and track status via the stepper
- **Activity log** captures notes and system updates

## Code map
| Area | Files |
| --- | --- |
| Pages | `src/Quickfire.Blazor/Domain/Renewals/Pages/Renewals.razor`, `Details.razor`, `Edit.razor` |
| Submissions | `src/Quickfire.Blazor/Domain/Renewals/Components/Submissions.razor (+ .cs)` |
| Tasks | `src/Quickfire.Blazor/Domain/Renewals/Pages/Details.razor`, `Domain/Renewals/Components/SubTaskList.razor`, `Domain/Renewals/Services/TaskService.cs` |
| Activity log | `src/Quickfire.Blazor/Domain/Renewals/Components/ActivityLog.razor` |
| Task admin | `src/Quickfire.Blazor/Domain/Profile/Pages/Tasks.razor`, `Domain/Renewals/Pages/MasterTaskGroupAdmin.razor` |

## Automation highlights
- **Auto-create renewals** from policies via `RenewalService.CreateRenewalFromPolicyAsync`.
- **Status updates** are persisted when the renewal dropdown changes (`Details.razor.cs`).
- **Task completion** can auto-set renewal status to Renewed for specific task names.

## Configuration tips
- Task templates live in the database and are managed via Task Admin.
- Renewal filters (month/year/user) are tracked in `StateService` for navigation continuity.

Related docs: [[reference/Renewal-Data-Share]] for status rules and [[features/Files-and-Attachments]] for renewal document handling.
