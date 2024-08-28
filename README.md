# Surefire

## Overview
Surefire: an insurance agency management system for independant P&C brokers designed with speed and efficiency in mind.

**Video Update**
https://youtu.be/4MuP97-Afqo

**Demo Site**
[Click here to login](https://flashvenomdesign-001-site6.atempurl.com/)

## Features

- Track and manage clients, contacts, addresses, locations, policies, carriers and more using a modern and intuitive interface
- Simplify your renewal workflow by putting all your renewals, quotes, leads and submissions in one central place.
- Set up a routine of tasks and keep and share status and notes on submissions to various carriers and MGAs/wholesalers.
- Set goal dates and assign sub-tasks to other employees. The homepage tells you what tasks you have to complete next for upcoming renewals and when they're due.
- Store basic policy data like limits, rates and coverages. Attach endorsements to be included in certificates.
- Issue, store and manage certificates quickly and easily
![Using the client screen and UI](https://flashvenom.com/surefire/surefire2.png)
![Surefire's client browser](https://flashvenom.com/surefire/surefire3.png)
![Simple renewal tracking system](https://flashvenom.com/surefire/surefire4.png)
![Tasks and submissions](https://flashvenom.com/surefire/surefire5.png)
![Surefire homepage](https://flashvenom.com/surefire/surefire6.png)
 
## Technologies Used

- **ASP.NET Core 8**
- **Entity Framework Core**
- **Blazor**
- **SyncFusion Components (DataGrid mostly)**
- **Microsoft FluentUI**

### Version History
**.0.0.2 - 2021-08-28**
- Enhanced client browsing
- Policy coverage details screens for GL, WC and Auto
- Certificate editor using SfPdfViewer2
- Store endorsements as attachments and include them with certificates

**.0.0.1 - 2021-08-20**
- Initial Release
- Includes all necessary tables and UI to add and edit Clients, Carriers, Contacts, Addresses, Policy Types and Policies
- Differentiates Carriers between Issuing Carriers and MGA/Wholesalers
- Uses Identity for user authentication and employee logins
- Renewal Center with submission tracking
- Master task editor to add tasks to be copied as workflow templates for renewals
- Much more...


## Wishlist
### General

- **Connect to voip system - bring up toast/card showing who is calling based on callerID** 
- https://hooks.officeathand.att.com/webhook/eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJvdCI6ImMiLCJvaSI6IjE2ODg4NDk4NjM5ODMxMDUiLCJpZCI6IjE2ODg4NTA2ODMwMTkyOTEifQ.9bLb55tTA0yasuN1YEM3ChXJcrBuKmrq3NIHm-OGNBw
- https://www.ringcentral.com/apps/office-at-hand/embeddable-voice
- **Add AI side panel for common tasks**
- **Add settings page with gear icon upper left**
- **Add picture for people**
- **Add Google Map link to address"**
- **Use AI to get JSON data of any location/address"**
- **Use AI to search client website and address lookup to get company details, industry codes, etc - new info pops up as (?) you click a check to confirm data, red x to throw away bad data**

- **Applications section will store basic data commonly used on apps like # of employees, gross sales, etc.**
- **Client portal with no sign-in required - a one time use key is emailed, they can update annual check-ups, see policies, etc**
### Clients
- **NOTES fields - "Permanent Notes (for "personal touch" to store things in common / small talk and then a "Recent Events" field in which the notes get smaller lighter as weeks go by**
- **Add logo of business - grab first google image result of "business name + logo"**
- **BRAIN: Use email address of client to pull latest email chains and use AI to summarize**
### Carriers
- **(IVANS Api?) Add more carrier data to cross check against clients/policies - Not just what lines they quote, but what lines they are good at, what SIC codes and industries they specialize in, 5 star rating AMBest rating, etc**
- **Use the JSON fields to add Targeted Lines of Buisness and Industries**


- ## ToDo
- **CLIENT PAGE: renewals section, menu section switches: Glance | Policies | Attachments | Claims | Applications**
- **Get a basic master search going using the top search bar**
- **Create dummy database with fake client data**
- DONE | Submission tab in renewal center
- **Duplicate renewal center for 'Leads' and new business**
- **SMART BREAD CRUMS: When you open a policy, certificate, etc - it adds it to the BreadCrum trail, and it stays there until you click the X or leave the client entirely - so you can switch between policies / other screens**

 
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

### Roadmap / Wishlist
- **Get a basic master search going using the top search bar**
- **Add settings page with gear icon upper right**
- **Add picture for people**
- **Add Google Map link to address"**
- **Integrate OpenAI side panel**
- **Use AI and Google API to get JSON data of businesses/locations/addresses"**
- **Applications section will store basic data commonly used on apps like # of employees, gross sales, etc.**
- **Client portal with no sign-in required - a one time use key is emailed, they can update annual check-ups, see policies, etc**
- **Notes Areas - "Permanent Notes (for "personal touch" to store things in common / small talk and then a "Recent Events" field in which the notes get smaller lighter as weeks go by**
- - **Connect to voip system - bring up toast/card showing who is calling based on callerID**
- **Add logo of business - grab first google image result of "business name + logo"**
- **Outlook Interop: Use email address of client to pull latest email chains and use OpenAI to summarize**
- **(IVANS Api?) Add more carrier data to cross check against clients/policies - Not just what lines they quote, but what lines they are good at, what SIC codes and industries they specialize in, 5 star rating AMBest rating, etc**
- **Use the JSON fields to add Targeted Lines of Buisness and Industries**
- **CLIENT PAGE: renewals section, menu section switches: Glance | Policies | Attachments | Claims | Applications**
- **Duplicate renewal center for 'Leads' and new business**
- **SMART BREAD CRUMS: When you open a policy, certificate, etc - it adds it to the BreadCrum trail, and it stays there until you click the X or leave the client entirely - so you can switch between policies / other screens**
- DONE | Create dummy database with fake client data**
- DONE | Submission tab in renewal center
