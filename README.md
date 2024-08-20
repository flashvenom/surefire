# Surefire

## Overview
Surefire: an insurance agency management system for independant P&C brokers designed with speed and efficiency in mind.

## Features

- Track Clients, Contacts, Addresses and Policies. (Screencap: https://flashvenom.com/images/surefire/clientscreen.gif)
![Surefire's client browser](https://flashvenom.com/images/surefire/clientscreen.gif)
- Designed to be intuitive and keep you moving through your day quickly 
![Surefire's client browser](https://flashvenom.com/images/surefire/clients.jpg)
- Includes a renewal manager for employees to stay on top of thier marketing and submissions work-flow manager. (Screencap: https://flashvenom.com/images/surefire/renewals.gif)
![Simple renewal tracking system](https://flashvenom.com/images/surefire/renewals.gif)
- Set up a routine of tasks and keep and share status and notes on submissions to various carriers and MGAs/wholesalers.
![Surefire's client browser](https://flashvenom.com/images/surefire/renewals.jpg)
- The homepage tells you what tasks you have to complete for upcoming renewals and when they're due.
![Surefire's client browser](https://flashvenom.com/images/surefire/homepage.jpg)


## Technologies Used

- **ASP.NET Core 8**
- **Entity Framework Core**
- **Blazor**
- **SyncFusion Components (DataGrid)**
- **Newtonsoft.Json**
- **SQL Server**

 
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

### Version History
**.0.0.1 - 2021-08-20**
- Initial Release
- Includes all necessary tables and UI to add and edit Clients, Carriers, Contacts, Addresses, Policy Types and Policies
- Differentiates Carriers between Issuing Carriers and MGA/Wholesalers
- Uses Identity for user authentication and employee logins
- Renewal Center with submission tracking
- Master task editor to add tasks to be copied as workflow templates for renewals
- Much more...

### Roadmap / Wishlist
- **Connect to voip system - bring up toast/card showing who is calling based on callerID**
- **Add settings page with gear icon upper left**
- **Add picture for people**
- **Add Google Map link to address"**
- **Integrate OpenAI side panel**
- **Use AI and Google API to get JSON data of businesses/locations/addresses"**
- **Applications section will store basic data commonly used on apps like # of employees, gross sales, etc.**
- **Client portal with no sign-in required - a one time use key is emailed, they can update annual check-ups, see policies, etc**
- **Notes Areas - "Permanent Notes (for "personal touch" to store things in common / small talk and then a "Recent Events" field in which the notes get smaller lighter as weeks go by**
- **Add logo of business - grab first google image result of "business name + logo"**
- **Outlook Interop: Use email address of client to pull latest email chains and use OpenAI to summarize**
- **(IVANS Api?) Add more carrier data to cross check against clients/policies - Not just what lines they quote, but what lines they are good at, what SIC codes and industries they specialize in, 5 star rating AMBest rating, etc**
- **Use the JSON fields to add Targeted Lines of Buisness and Industries**
- **CLIENT PAGE: renewals section, menu section switches: Glance | Policies | Attachments | Claims | Applications**
- **Get a basic master search going using the top search bar**
- **Create dummy database with fake client data**
- DONE | Submission tab in renewal center
- **Duplicate renewal center for 'Leads' and new business**
- **SMART BREAD CRUMS: When you open a policy, certificate, etc - it adds it to the BreadCrum trail, and it stays there until you click the X or leave the client entirely - so you can switch between policies / other screens**
