# Files and Attachments
Quickfire treats attachments as first-class data. Files are uploaded, hashed, stored on disk, and referenced by database metadata so they can surface everywhere.

## User capabilities
- Drag and drop into `DropzoneContainer` on clients, renewals, and policies
- Preview PDF thumbnails and open files from the attachments grid
- Reuse attachments across entities via attachment groups

## Implementation details
| Concern | Files |
| --- | --- |
| UI grids + previews | `src/Quickfire.Blazor/Domain/Attachments/Components/AttachmentListGrid.razor`, `AttachmentIcons.razor`, `AttachmentPreview.razor` |
| Drop zones | `src/Quickfire.Blazor/Domain/Attachments/Components/DropzoneContainer.razor` |
| Service layer | `src/Quickfire.Blazor/Domain/Attachments/Services/AttachmentService.cs`, `AttachmentUploaderApi.cs` |
| Helpers | `src/Quickfire.Blazor/Domain/Shared/Helpers/StringHelpers.cs` |
| Storage mapping | `src/Quickfire.Blazor/Domain/Shared/Services/FileStorageResolver.cs` |

## Notes on storage
- Files are written under `wwwroot/uploads/<entity>/<id>/` with a hashed filename suffix.
- PDF thumbnails and data files live under a `.data` folder adjacent to the upload.
- `FileStorageSettings` in `Domain/Shared/Models/FileStorageSettings.cs` controls local vs mapped paths.

## Best practices
1. Reuse `AttachmentGroupId` when you want files to appear across related entities.
2. Use helper components (icons/list grid) to keep keyboard and drag behavior consistent.
3. Follow [[reference/Binding-Events]] when wiring attachment dialogs to avoid double updates.

Related docs: [[features/Renewals-and-Submissions]] for renewal document flows and [[features/Forms-and-ACORD]] for form output attachments.
