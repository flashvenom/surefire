# Surefire

## Overview

Surefire: an insurance agency management system for P&C brokers.

## Features

- **Track Client, Contact, Addresses, Policies and Renewals.

## Technologies Used

- **ASP.NET Core 8**
- **Entity Framework Core**
- **Blazor**
- **SyncFusion Components**
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

### Notes

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
