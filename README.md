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

### Client View Page
- Relevant Carriers (so you can you to service and quoting website and see underwriters and emails and phone numbers)
- Relevant people for carriers, and company
- Notes field for knowing what's going on with them
- Notes from renewals in process
- Certificates
- 

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
