# Quickfire AMS (Openfire 1.1.0)
![Quickfire](https://quickfireams.com/images/github/home-small.png)

## Primer
Quickfire is an insurance agency management system for independent P&C brokers. Openfire is the open source edition, now fully refactored as of 1.1.0 for a cleaner architecture, faster iteration, and a stronger foundation for plugins and automation.

[Take a Quick Video Tour](https://www.youtube.com/watch?v=ARkqg0iJG0g)

## Editions
Openfire is the open source core framework of Quickfire and is focused on workflows. The fully featured, closed source versions are available now:
- **Quickfire** and **Quickfire Pro**: Production-ready, fully featured builds at https://quickfireams.com

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



## Line of Fire / Version History
**Openfire 1.1.0 - 2025-11-11**
- Major refactor and solution restructure
- Clearer project boundaries and services for faster development
- Expanded plugin system foundation and integration points
- Database flow and seeding cleaned up for local or production use
- UI polish and layout consistency improvements

**v1.0.1-alpha - 2025-01-24**
- ALPHA preview release merged from a private branch
- SQLite database for local and Desktop app use
- Accounting screen for invoicing and tracking costs
- Global profile screen with system settings stored in database
- SMARTpaste for extracting data from text blobs into forms
- Attachments stored locally, on a network drive, or on Azure Blob
- Centralized details tab for client and policy data
- Leads tracking and prioritization
- Outlook interop search and workflow integrations
- RingCentral call lookup and click-to-dial support
- Custom Trigger component for mailto/tel/hyperlink actions
- Custom SmartPaste component for lead and client forms
- .NET 9 migration and component reorganization
- Plugin system groundwork

**v0.1.0 - 2024-10-17**
- Leads management, RingCentral webhook, CRUD foundation
- Forms tab with PDF manipulation and JSON storage
- Identity-based auth and renewal center workflows
- Homepage task templates and workflow tracking
