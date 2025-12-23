# Quickfire Wiki

Quickfire (open-source edition) is an insurance AMS for P&C agencies. This repo focuses on core workflows: clients, carriers, contacts, policies, renewals, leads, attachments, and forms, plus optional desktop and tray tooling. Integrations and AI services were removed for the open-source release.

## Why Quickfire
- Core AMS coverage: clients, renewals, leads, policies, and forms on a single Blazor surface
- Desktop-ready: MAUI shell runs the host locally; optional tray app adds Outlook and Word helpers
- Local-first storage: attachments and form revisions live on disk with database-backed metadata
- Configurable data layer: SQLite by default, SQL Server when `DEFAULTCONNECTION` is provided

## Jump In
- [[Getting-Started]] - prerequisites, environment variables, and the quick `dotnet` loop
- [[System-Architecture]] - solution layout, runtime modes, and service map
- [[Release-Notes]] - open-source release notes
- [[features/Homepage|Feature pages]] - Homepage, Clients, Attachments, Forms, Interface, and Renewals
- [[reference/Binding-Events|Reference]] - binding patterns, dropdowns, product types, renewal rules, and desktop builds

## Quick Links
- Feature Deep Dives: [[features/Homepage|Homepage]], [[features/Clients|Clients]], [[features/Files-and-Attachments|Attachments]], [[features/Forms-and-ACORD|Forms]], [[features/Interface|Interface]], [[features/Renewals-and-Submissions|Renewals]]
- Reference and Guides: [[reference/Binding-Events|Binding events]], [[reference/Dropdowns|Dropdowns]], [[reference/Feature-Map|Feature map]], [[reference/Product-Types|Product types]], [[reference/Renewal-Data-Share|Renewal data share]], [[guides/Quickfire-Desktop-Builds|Desktop builds]]

## Need Help Fast?
- Use GitHub wiki search (Ctrl/Cmd+K) to jump between pages
- Each page cites the owning files (e.g., `src/Quickfire.Blazor/Domain/Clients/Pages/Clients.razor`) so you can open code beside the doc

_Last updated: 2025-12-22_
