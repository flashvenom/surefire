# Homepage
The homepage is the shared cockpit for CSRs, producers, and ops. It surfaces daily tasks, renewal flow status, leads, certificate requests, and expiring items using `HomeService` and `StateService`.

## What users see
- **Daily Task List** - highlighted and nearest renewal tasks (`Domain/Home/Components/DailyTaskList.razor`)
- **Renewal Flow Tasks** - renewal-driven work items (`Domain/Home/Components/RenewalFlowTasks.razor`)
- **Certificate Requests** - open requests and quick links (`Domain/Home/Components/CertificateRequestList.razor`)
- **Leads snapshot** - inbound and active leads (`Domain/Home/Components/LeadsHomeList.razor`)
- **Expiring Soon + Incomplete Tasks** - items at risk (`Domain/Home/Components/ExpiringSoon.razor`, `IncompleteTasks.razor`)
- **Cheat Sheet + Daily Inspiration** - reference and motivation widgets

## Key implementation points
| Concern | Where it lives |
| --- | --- |
| Layout + sections | `src/Quickfire.Blazor/Domain/Home/Pages/Home.razor` |
| Data loader | `src/Quickfire.Blazor/Domain/Shared/Services/HomeService.cs` + `StateService.GetHomepageDataAsync()` |
| Widgets | `src/Quickfire.Blazor/Domain/Home/Components/*.razor` |
| Parallax/animations | `src/Quickfire.Blazor/Domain/Shared/Services/AppJsInterop.cs` |

## Configuration tips
- User preferences (Profile page) control simple mode, layout, and animation toggles.
- Homepage data is cached in `StateService`; reload via `GetHomepageDataAsync()` after bulk updates.

## Extending the Homepage
1. Create a new component under `Domain/Home/Components`.
2. Add its data fetch in `HomeService` or a dedicated state helper.
3. Insert the component in `Home.razor` and respect simple mode toggles.

Cross-reference: [[features/Renewals-and-Submissions]] for task and renewal data, [[features/Forms-and-ACORD]] for certificate flows.
