# Mantis Project

## Overview

Mantis is a comprehensive insurance management system designed to streamline the operations of commercial insurance brokers. It features robust client and policy management, seamless integration with external APIs, and efficient data handling capabilities.

## Features

- **Client Management**: Manage clients with detailed information, including primary contact, address, producer, CSR, and more.
- **Policy Management**: Handle policies with various attributes such as policy number, type, status, and associated carriers and wholesalers.
- **External API Integration**: Integrates with external APIs to fetch policy lines and related details.
- **Authentication and Authorization**: Securely manage user access and roles using ASP.NET Core Identity.
- **Data Persistence**: Efficiently manage data using Entity Framework Core.

## Technologies Used

- **ASP.NET Core**
- **Entity Framework Core**
- **Blazor**
- **SyncFusion Components**
- **Newtonsoft.Json**
- **SQL Server**

## Roadmap

- **Connect to voip system - bring up toast/card showing who is calling based on callerID**
- **Add AI side panel for common tasks**
- **Add settings page with gear icon upper left**
- **Add picture for people**
- **Add logo of business - grab first google image result of "business name + logo"**
- **Add Google Map link to address"**
- **Use AI to get JSON data of any location/address"**
- **Add "touch field" to store things in common / small talk"**
- **Use AI to search client website and address lookup to get company details, industry codes, etc - new info pops up as (?) you click a check to confirm data, red x to throw away bad data**
- **CLIENT PAGE: renewals section, menu section switches: Glance | Policies | Attachments | Claims | Applications**
- **Applications section will store basic data commonly used on apps like # of employees, gross sales, etc.**
- **Client portal with no sign-in required - a one time use key is emailed, they can update annual check-ups, see policies, etc**

- ## ToDo
- **Create dummy database with fake client data**
- **Submission tab in renewal center**
- **Duplicate renewal center for 'Leads Processing'**
- **SMART BREAD CRUMS: When you open a policy, certificate, etc - it adds it to the BreadCrum trail, and it stays there until you click the X or leave the client entirely - so you can switch between policies / other screens**

 
## Installation

### Prerequisites

- .NET 6 SDK or later
- SQL Server or SQL Server Express

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

3. **Apply Migrations:**
    ```bash
    dotnet ef database update
    ```

4. **Run the application:**
    ```bash
    dotnet run
    ```

## Usage

### Client Management

- Add new clients with detailed information.
- Update client details and associate policies.
- Delete clients as necessary.

### Policy Management

- Add new policies and associate them with clients.
- Fetch policy lines and details from external APIs.
- Update policy details, including carrier and wholesaler information.

### API Integration

#### Fetch Policy Lines

To fetch policy lines from the external API, use the `GetPolicyLinesAsync` method in `CrmApiService`.

Example:
```csharp
var accessToken = await CrmApiService.GetAccessTokenAsync();
var policyLinesJson = await CrmApiService.GetPolicyLinesAsync(ePolicyId, accessToken);
