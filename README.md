# Quickfire AMS (Openfire 1.1.1)
![Quickfire](https://quickfireams.com/images/github/home-small.png)

## Primer
Quickfire is an insurance agency management system for independent P&C brokers. Openfire is the open source edition, now fully refactored as of 1.1.1 for a cleaner architecture, faster iteration, and a stronger foundation for plugins and automation.

[Take a Quick Video Tour](https://www.youtube.com/watch?v=ARkqg0iJG0g)

## Editions
Openfire is the open source core framework of Quickfire and is focused on workflows. The fully featured, closed source builds for Mac (Desktop only) and Windows (Desktop and Server) are available now at https://quickfireams.com

![Quickfire](https://quickfireams.com/images/github/qf-header2.png)

## Scope
- Track and manage clients, contacts, addresses, locations, policies, carriers, and more with a modern, fast UI
- Consolidate your APIs to track payments, phone calls, leads, documents, and forms in one place
- Use OpenAI integration to build custom prompts for data entry, summaries, and workflows
- Centralize renewals, quotes, leads, and submissions with clear next actions
- Assign tasks, set goal dates, and keep carrier and MGA notes organized
- Store policy data (limits, rates, coverages) and include endorsements on certificates
- Issue, store, and edit certificates, PDFs, and applications with a built-in editor
- Talk to your data in natural language to unlock bleeding edge insights and time savers
- Spawn background workers to handle follow ups, perform routine duties and more

![Quickfire](https://quickfireams.com/images/github/renewals-small.png)

## Loadout
- **ASP.NET Core 10**
- **Blazor (Server Side Interactivity)**
- **Entity Framework Core**
- **Microsoft FluentUI**
- **SyncFusion Blazor**
- **Outlook Interop**
- **SQL Server and SQLite**

![Quickfire](https://quickfireams.com/images/github/outreach-short-2.png)

## Triggerfinger
**Ready, Aim, Fire...**

1. **Clone the repository:**
    ```bash
    git clone https://github.com/flashvenom/Quickfire.git
    cd Quickfire
    ```

2. **Run build-installer.bat to build a desktop/SQLite installation** or **set up your SQL Server database (optional):**
    Create a `.env` file with your database connection string. Not providing a string will default the system to use a local SQLite database.
    ```txt
    DEFAULTCONNECTION={CONNECTIONSTRING}
    ```
    You must exclude either the Data/Migrations (SQL Server) or the Data/MigrationsLocal (SQLite) folder in your solution.

3. **Enter a SyncFusion License Key and OpenAI API Secret:**
    Register at syncfusion.com and get your free SyncFusion license key, then add your SyncFusion license and OpenAI secret to the .env file.
    ```txt
    SYNCFUSION={LICENSESTRING}
    OPENAI={APIKEY}
    ```

4. **Apply migrations:**
    ```bash
    dotnet ef database update
    ```

5. **Run the application to seed initial data:**
    ```bash
    dotnet run
    ```

6. **Sign in with the default admin account (first run):**
    ```txt
    Username/Email: admin@quickfire.local
    Password: Password123!
    ```
    Override seeded credentials with `.env` values if needed:
    ```txt
    ADMIN_EMAIL={EMAIL}
    ADMIN_USERNAME={USERNAME}
    ADMIN_PASSWORD={PASSWORD}
    ```


## Latest Release
### Openfire 1.1.1 (2026-02-07)

Focused release: **Company Manual** and **Forms Library**.

#### Company Manual
New workflow for maintaining internal procedures with publishing controls.

- Build structured manual content with nested pages and a table of contents
- Author and update procedures in a rich text editor
- Keep published revisions with change summaries and revision history
- Allow non-admin users to submit suggested edits for admin approval/rejection
- Search across published procedure titles and content
- Track activity with an audit trail (publish, suggest, approve/reject, archive, restore, reorder)
- Manage metadata like procedure type, line of business, owner, SLA target, and review dates
- Print a single page or the full manual in a print-friendly layout
- Support image/video embeds with lightbox viewing in the manual UI

#### Forms Library
New centralized library for reusable PDF forms and version management.

- Upload and catalog forms with metadata (carrier, wholesaler, market tag)
- Filter and search by text, tags, archived state, bookmarks, and rating
- Keep version history per form and set the active version
- Auto-read PDF metadata (page count and form field count) for each version
- Generate first-page thumbnails for quick visual preview
- Open a form directly from the library
- Attach a form to a client and open it in the Form Editor
- Fill a form for a selected client in the Form Editor
- Create an Outlook email draft with the selected form attached
- Enforce PDF-only uploads with size validation (up to 200 MB)
