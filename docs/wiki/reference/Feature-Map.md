Use this table as a quick navigation map for routes and owning components.

| Short Name | Route | Component | Notes |
| --- | --- | --- | --- |
| Home | `/` | `Domain/Home/Pages/Home.razor` | Dashboard widgets |
| Clients | `/Clients` | `Domain/Clients/Pages/Clients.razor` | 360 view with tabs |
| Client List | `/Clients/List` | `Domain/Clients/Pages/Index.razor` | List/grid view |
| Client Create | `/Clients/Create` | `Domain/Clients/Pages/Create.razor` | New client intake |
| Leads | `/Leads` | `Domain/Leads/Components/Leads.razor` | Pipeline view |
| Lead Details | `/Leads/{id}` | `Domain/Leads/Components/Details.razor` | Lead detail page |
| Renewals | `/Renewals` | `Domain/Renewals/Pages/Renewals.razor` | Calendar/board |
| Renewal Details | `/Renewals/Details/{id}` | `Domain/Renewals/Pages/Details.razor` | Tasks + submissions |
| Policies | `/Policies` | `Domain/Policies/Pages/Index.razor` | Policy list |
| Policy Details | `/Policies/Details/{id}` | `Domain/Policies/Pages/Details.razor` | Policy detail view |
| Carriers | `/Carriers` | `Domain/Carriers/Pages/Carriers.razor` | Carrier list/edit |
| Contacts | `/Contacts` | `Domain/Contacts/Pages/Contacts.razor` | Contact list/detail |
| Forms | `/Forms/Editor/{FormDocId}` | `Domain/Forms/Pages/FormEditor.razor` | Form editor |
| Certificates | `/Forms/Certificate/{CertificateId}` | `Domain/Forms/Pages/CertificateEditor.razor` | Certificate editor |
| System Settings | `/System` | `Domain/Profile/Pages/SystemSettings.razor` | Open-source note |
| Profile | `/Profile` | `Domain/Profile/Pages/Profile.razor` | User preferences |
| Task Admin | `/Tasks` | `Domain/Profile/Pages/Tasks.razor` | Master task editor |
| Users | `/Users` | `Domain/Profile/Pages/Users.razor` | User admin |
| Logs | `/Logs` | `Domain/Profile/Pages/Logs.razor` | Activity logs |
| Utilities | `/Utilities/{ClientId}` | `Domain/Utilities/Utilities.razor` | Utility tools |

### Tips
- When you add or rename routes, update this table so docs and navigation stay aligned.
