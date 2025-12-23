The shell wraps the top bar, FireSearch, nav menu, status bar, and profile preferences.

## FireSearch
- Top bar search lives in `src/Quickfire.Blazor/App/Layout/_searchbar.razor`.
- Uses `src/Quickfire.Blazor/Domain/Shared/Services/SearchService.cs` to query clients, carriers, contacts, policies, renewals, and leads.
- Keyboard shortcuts: `/` to focus, arrows to navigate, `Enter` to open.

## Navigation and shell
- `src/Quickfire.Blazor/App/Layout/MainLayout.razor` wires the top bar, nav, toasts, and status bar.
- `src/Quickfire.Blazor/App/Layout/NavMenu.razor` defines main sections and respects simple mode preferences.
- `AppJsInterop` triggers top bar animations when navigating between core sections.

## Status and preferences
- `src/Quickfire.Blazor/App/Layout/_statusbar.razor` listens to `StateService.UpdateStatus`.
- User preferences live in `src/Quickfire.Blazor/Domain/Shared/Models/UserPreferences.cs` and are edited in `Domain/Profile/Pages/Profile.razor`.

## Accessibility and UX standards
- Use Fluent UI and Syncfusion components consistently; follow [[reference/Binding-Events]] for bindings.
- Theme styles live in `src/Quickfire.Blazor/wwwroot/css/app.css` and component-level `.razor.css` files.
- Keep page-specific JS in `wwwroot/js` and minimize cross-component coupling.

Cross-reference: [[reference/Binding-Events]] and [[reference/Dropdowns]].
