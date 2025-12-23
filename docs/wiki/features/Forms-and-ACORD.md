# Forms and ACORD
Forms provide revision-safe PDF editing and JSON field mappings so teams can generate certificates, SL-2s, and ACORD forms quickly.

## Broker workflow
- Launch a form from Clients, Renewals, or the Forms list
- Prefill fields using JSON templates in `wwwroot/forms/_json`
- Save revisions without overwriting prior versions
- Export form outputs back to attachments

## Components and services
| Piece | Files |
| --- | --- |
| Editors | `src/Quickfire.Blazor/Domain/Forms/Pages/FormEditor.razor`, `CertificateEditor.razor` |
| Lists | `src/Quickfire.Blazor/Domain/Forms/Components/FormDocList.razor`, `CertificateList.razor` |
| Requests | `src/Quickfire.Blazor/Domain/Forms/Pages/CertificateRequestsList.razor` |
| Service logic | `src/Quickfire.Blazor/Domain/Forms/Services/FormService.cs` |
| Template assets | `src/Quickfire.Blazor/wwwroot/forms/*.pdf`, `wwwroot/forms/_json/*.json` |

## Tips
- JSON templates map Quickfire fields to PDF IDs. Keep the JSON in sync with the PDF layout.
- Revisions store metadata and file references so you can revert without re-uploading.
- When adding new forms, drop the PDF and JSON under `wwwroot/forms` and update `FormService` metadata or seed data.

Cross references: [[features/Files-and-Attachments]] for storage behavior and [[features/Renewals-and-Submissions]] for renewal-linked forms.
