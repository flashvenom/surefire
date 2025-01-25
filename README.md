# Surefire

## Overview
Surefire: an insurance agency management system for independant P&C brokers designed with speed and efficiency in mind.

**ALPHA PREVIEW 1.0.0 RELEASED - Happy New Year**
https://youtu.be/4MuP97-Afqo

**Demo App**
[Available Soon to Download Here](https://surefireams.com/)

## Features
- Track and manage clients, contacts, addresses, locations, policies, carriers and more using a modern and intuitive interface
- Bring all your APIs together to track payments, phone calls, leads, signed documents, forms and more
- Leverage OpenAI integration to make custom prompts to process your data and get work done faster
- Simplify your renewal workflow by putting all your renewals, quotes, leads and submissions in one central place.
- Set up a routine of tasks and keep and share status and notes on submissions to various carriers and MGAs/wholesalers.
- Set goal dates and assign sub-tasks to other employees. The homepage tells you what tasks you have to complete next for upcoming renewals and when they're due.
- Store basic policy data like limits, rates and coverages. Attach endorsements to be included in certificates.
- Issue, store and manage certificates quickly and easily
![Using the client screen and UI](https://flashvenom.com/surefire/surefire2.png)
![Surefire's client browser](https://flashvenom.com/surefire/surefire10.png)
![Simple renewal tracking system](https://flashvenom.com/surefire/surefire4.png)
![Tasks and submissions](https://flashvenom.com/surefire/surefire5.png)
![Submission tracker](https://flashvenom.com/surefire/surefire11.png)
![Surefire homepage](https://flashvenom.com/surefire/surefire6.png)
 
## Technologies Used
- **ASP.NET Core 9**
- **Blazor (Server Side Interactivity)**
- **Entity Framework Core**
- **Microsoft FluentUI**
- **SyncFusion Blazor**
- **Outlook Inerop**

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



## High Priority Bugs

## Low Priority Tasks
- CLIENT      = New Client form needs additional validation
- LOCATIONS   = Needs work to finish

## Enhancements
- CARRIERS    = Add a webchat link field for carrier and add icons to renewal and carrier screens
- SUBMISSIONS = Renewals: Put submission summary and list of all recent notes
- CALL HUB    = Enhance with persona and icons
- RENEWAL     = Edit screen should have "archive" bool, when checked mark all tasks complete, hide from renewal lists

### Phase 0
- PROFILE: User Profile Stuff (Change Password, etc)
- PLUGINS: Settings and Embedded razor views (ePay link builder))
- SMS/RingCentral CHAT
- Move OpenAI to Plugin

### Phase 1
- Master Task Groups / Create Renewal Settings
- FORMS REVISIONS STORAGE
- LEADS INBOX (Connect to website)
- Attachments Enhancements - Embedded PDF Viewer

### Phase 2
- MARKETING
- REPORTS
- AI-Assisted Search
- Universal Notes (Add notes model, clientid nav, associatedType associatedId fields. When clicking phone icon, add note icon attaches to client and phone call in phone call log)

### Phase 3 - Accounting
- PURPLE SHEET / ACCOUNTING SYSTEM

## Features Roadmap
### Productivity
- **Microsoft Graph API - For sending emails, searching for client and policy correspondances, etc**
- **Client Portal - Provide clients with a way to access basic policy data, request a certificate, and update renewal information.**

### Crucial Functionality
- **Profile Pages - For Identity users to change password, email, name, etc.
- **Proper Register - With roles and permissions. Confirmation emails and Microsoft OAuth authorizations logic

### General
- **Settings - Add settings page (gear icon in the upper right top menu) to assign a subemployee/assistant, enter API credentials and store UI preferences.**

### Carriers
- **Carrier Data: Provide system for storing target markets, AmBest rating, acceptable risks, ratings and IVANs data to provide more. 

### AI/LLM
- **Side Panel - Create side panel to provide UI to interactive with OpenAI for common tasks**
- Tool: Get JSON data for locations/address
- Tool: Get JSON data for clients and policies
- Tool: Fetch and suggest images for client logo and contact headshots

### DONE


## Installation
### Steps

1. **Clone the repository:**
    ```bash
    git clone https://github.com/yourusername/Surefire.git
    cd mantis
    ```

2. **Set up the database (optional):**
    Create a `.env` file with your database connection string. Not providing a string will default the system to use a local SQLite database.
    ```txt
    DEFAULTCONNECTION={YOURSTRING}
    ```

2. **Enter a SyncFusion License:**
    Register at syncfusion.com and update the `Program.cs` file with your SyncFusion license key (free).


3. **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Run the application to seed initial data:**
    ```bash
    dotnet run
    ```
