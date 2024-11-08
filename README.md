# Surefire

## Overview
Surefire: an insurance agency management system for independant P&C brokers designed with speed and efficiency in mind.

**November 2024 - Development Progress Update Video**
https://youtu.be/OEAfL_fraq4

![Surefire MVP Progress](https://files.flashvenom.com/surefireflyer.jpg)

**MVP rc1 Coming Soon**
-New Features: Attachments, Leads Management, Outlook Desktop control via Interop, 
-API Integrations: VoiP Call Logs & RingCentral API, ePayPolicy API, OpenAI integration, Google Maps API, Microsoft Graph and more...

I will be merging the private dev branch to public early 2025. Contact me for a demo or to discuss the project.

## Table of Contents

## Features
- Track and manage clients, contacts, addresses, locations, policies, carriers and more using a modern and intuitive interface
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
- **ASP.NET Core 8**
- **Entity Framework Core**
- **Blazor**
- **SyncFusion Components**
- **Microsoft FluentUI**

## Update
Have very exciting news and massive new updates and features on the private branch. Plan on merging with public in 2025 with a version that works with disjointed API integrations. See the November 2024 Dev Progress Update YouTube video. Please contact me for a demo or to discuss the project.

### Version 
**.0.1.01 - 2024-11-07**
- Added migrations for upcoming mvp

**.0.1.0 - 2024-09-13**
- SfPdfViewer2 implemented for creating and editing PDF forms like certificates, supplementals and applications
- Switched SfGrid to FluentDataGrid for security reasons and future use
- Major changes to Client and Renewal pages for performance and UI/UX experience
- Enhanced main layout and UI including beautiful NavMenu rollovers and tooltips
- Methods for 3rd party APIs to fetch contacts and policy lists for clients

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
**Video Update - September 1**
https://youtu.be/4MuP97-Afqo
- 
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


## Features Roadmap
### Productivity
- **Ring Central API - Implement a webhook that monitors incoming phone calls and displays a toast notification with the caller's name and number**
- **Microsoft Graph API - For sending emails, searching for client and policy correspondances, etc**
- **Zywave API - Integrate the company research calls, content downloads, and insurance code lookup calls**
- **Client Portal - Provide clients with a way to access basic policy data, request a certificate, and update renewal information.**
- **Leads Management - Take the layout of Carriers and create a new page for tracking leads and collecting information.

### Crucial Functionality
- **Profile Pages - For Identity users to change password, email, name, etc.
- **Proper Register - With roles and permissions. Confirmation emails and Microsoft OAuth authorizations logic
- **CRUD - Confirm there is a way to safely crud all the things.

### General
- **Settings - Add settings page (gear icon in the upper right top menu) to assign a subemployee/assistant, enter API credentials and store UI preferences.**
- **Forms Tab: Change certificates to forms, and add a library of PDFs which can be manipulated and the JSON saved in the database.**
- **Smart Breadrums: Dynamically build breadcrums as the user navigates around the different areas of the site.
- **Add CRUD to FluentDataGrids


### Carriers
- **Passwords: Store the user's credentials for carrier websites.
- **Carrier Data: Provide system for storing target markets, AmBest rating, acceptable risks, ratings and IVANs data to provide more. 

### AI/LLM
- **Side Panel - Create side panel to provide UI to interactive with OpenAI for common tasks**
- - Tool: Get JSON data for locations/address
- - Tool: Get JSON data for clients and policies
- - Tool: Fetch and suggest images for client logo and contact headshots


## Installation
### Steps

1. **Clone the repository:**
    ```bash
    git clone https://github.com/yourusername/mantis.git
    cd mantis
    ```

2. **Set up the database:**
    Update the `appsettings.json` file with your database connection string.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=your_server;Database=mantis;User Id=your_user;Password=your_password;"
    }
    ```

2. **Enter a SyncFusion License:**
    Register at syncfusion.com and update the `Program.cs` file with your license key.


3. **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Run the application:**
    ```bash
    dotnet run
    ```
