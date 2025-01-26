# Surefire AMS
An insurance agency management system for independant P&C brokers. Designed with speed and efficiency in mind, it features several built-in productivity tools and well as an advanced plugin system for consolodating APIs, writing custom integrations and providing a platform to use your data with your own AI prompts.

## Download
**Alpha Preview v1.0.1**

[Take a Quick Video Tour](https://youtu.be/4MuP97-Afqo) or [Download the Windows x64 Standalone Installer](https://www.dropbox.com/scl/fi/vfye3p7una4iy7pq6wl1m/Install_Surefire.exe?rlkey=5gmxu5ywisjyqgspzhaepe6fh&dl=0)

![Surefire MVP Progress](https://files.flashvenom.com/surefireflyer.jpg)

## Features
- Track and manage clients, contacts, addresses, locations, policies, carriers and more using a modern and intuitive interface
- Bring all your APIs together to track payments, phone calls, leads, signed documents, forms and more
- Leverage OpenAI integration to make custom prompts to process your data and get work done faster
- Simplify your renewal workflow by putting all your renewals, quotes, leads and submissions in one central place.
- Set up a routine of tasks and keep and share status and notes on submissions to various carriers and MGAs/wholesalers.
- Set goal dates and assign sub-tasks to other employees. The homepage tells you what tasks you have to complete next for upcoming renewals and when they're due.
- Store basic policy data like limits, rates and coverages. Attach endorsements to be included in certificates.
- Issue, store and manage certificates, PDF and applications quickly and easily with a built-in PDF editor
 
## Technologies Used
- **ASP.NET Core 9**
- **Blazor (Server Side Interactivity)**
- **Entity Framework Core**
- **Microsoft FluentUI**
- **SyncFusion Blazor**
- **Outlook Interop**
- **SQL Server and SQLite**

## Plugins Available
- **RingCentral API**
- **ePayPolicy API**
- **AppliedEpic API**
- **AMS360 SDK API**

### Version History
**.1.0.1 - 2025-01-24**
- ALPHA preview release is a massive merge from my personal branch and includes tons of new features and updates
- Complete restructure of solution and projects
- SQLite Database for local and Desktop app use
- Accounting screen for invoicing and tracking costs
- Global profile screen with system settings stored in database
- SMARTpaste: Paste in an email footer or other blob of text and AI will fill in fields on the screen
- ATTACHMENTS: Store files locally, on a network drive or on an Azure Blob
- DETAILS TAB: A centralized place to quickly save, sync and view crucial client and policy data
- LEADS: Store and prioritize leads and new business quotes and proposals
- DESKTOP OUTLOOK INTEGRATION: Perform advanced, customized searches on your Outlook emails
- INCOMING CALL: Link your RingCentral API to search incoming calls against the database so you can click to a customer's screen and see their policies before you even pick up the phone
- CLICK TO DIAL: Click a phone number to automatically place an outgoing call on your desk phone
- TRIGGER LINKS: Click to perform the function, Shift+Click to copy the text to clipboard, or Alt+Shuft+Click to search for that string in Outlook Desktop
- Add attachments and associate with clients and policies. Uses a local network drive for lightning fast document access.
- Integrated with Outlook using interop. Use preset custom searches to find emails fast.
- Control-Click a email, phone number or full name of a contact to copy it to the clipboard
- Easily create ePay Links on the renewal and client screens
- Added SL-2 forms to the forms library  
- Added Logging System
- View all recent calls and payments in the profile (gear icon) pages
- Sort and group and filtering in client policies screen
- Easily create a lead by extracting data pasted in from an email or lead sheet
- Updated SyncFusion from 26.2.9 to 27.1.56
- Switched persistance to global StateService instead of database
- Renewal submissions tab asks if you want to make incumbant submission if there's none others found
- BETA: Paste in policy and client XML data on the client details tab to extract relevant data
- Added custom component: "Trigger" which handles tel, mailto, and hyperlink tags. Click to launch, Ctrl+Click to copy, Ctrl+Shift+Click to search in Outlook
- Major overhaul of dbcontext and code cleanup and refactoring
- Better error handling on FireSearch (universal search bar at the top)
- Added custom SmartPaste component to Lead and Client create and edit forms so you can paste a block of text and have Surefire fill in the appropriate fields automatically
- Migrated to .net9
- BETA: Added Plugin System for custom components
- Reorganization of file structure with a focus on components and services with a new root namespace
- ...AND MUCH MUCH MORE...

**.0.1.0 - 2024-10-17**
- Leads Management - Take the layout of Carriers and create a new page for tracking leads and collecting information.
- Ring Central API - Implement a webhook that monitors incoming phone calls and displays a toast notification with the caller's name and number**
- CRUD - Confirm there is a way to safely crud all the things.
- Forms Tab: Change certificates to forms, and add a library of PDFs which can be manipulated and the JSON saved in the database.**
- Passwords: Store the user's credentials for carrier websites.
- Homepage Layout Polished and Wrapping Correctly
- - save order of tasks on homepage

**.0.0.4 - 2024-09-08**
- Implemented BaseUrl and IConfigure logic for environment variables for easy development and publishing
- Many UI/UX/Style/Performance enhancements with a focus on Clients and NavMenu
- Added upload functionality for contact headshots and client logos
- Enhanced the client primary contact logic including several bugs

**.0.0.3 - 2024-09-05**
- Renewal filter save state using browser session
- Enhanced renewal task lists
- Fast Search now has keyboard control
- File organization and Quality of Life fixes
- Misc bug fixes

**.0.0.2 - 2024-08-28**
- Enhanced client browsing
- Policy coverage details screens for GL, WC and Auto
- Certificate editor using SfPdfViewer2
- Store endorsements as attachments and include them with certificates

**.0.0.1 - 2024-08-20**
- Initial Release
- Includes all necessary tables and UI to add and edit Clients, Carriers, Contacts, Addresses, Policy Types and Policies
- Differentiates Carriers between Issuing Carriers and MGA/Wholesalers
- Uses Identity for user authentication and employee logins
- Renewal Center with submission tracking
- Master task editor to add tasks to be copied as workflow templates for renewals
- Much more...

## Installation
### Steps

1. **Clone the repository:**
    ```bash
    git clone https://github.com/flashvenom/surefire.git
    cd Surefire
    ```

2. **Set up your database (optional):**
    Create a `.env` file with your database connection string. Not providing a string will default the system to use a local SQLite database.
    ```txt
    DEFAULTCONNECTION={CONNECTIONSTRING}
    ```
    You must exclude either the Data/Migrations (SQL Server) or the Data/MigrationsLocal (SQLite) folder in your solution.

2. **Enter a SyncFusion License Key and OpenAI API Secret:**
    Register at syncfusion.com and get your free SyncFusion license key, then add your sync function license and OpenAI Secret key to the .env file.
    ```txt
    SYNCFUSION={LICENSESTRING}
    OPENAI={APIKEY}
    ```

3. **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Run the application to seed initial data:**
    ```bash
    dotnet run
    ```
