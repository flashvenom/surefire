USE [quickfire]
GO

/****** Object:  StoredProcedure [dbo].[SearchAllWithRenewals]    Script Date: 12/20/2025 10:35:50 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SearchAllWithRenewals]
    @SearchTerm NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        DECLARE @SearchTermLower NVARCHAR(100) = LOWER(@SearchTerm);
        DECLARE @Today DATE = GETDATE();

        WITH SearchResults AS (
            -- Clients search
            SELECT TOP 10
                'Client' AS DataType,
                c.ClientId AS Id,
                c.Name AS [Primary],
                '' AS Parent,
                '' AS AddressType
            FROM Clients c WITH (NOLOCK)
            WHERE 
                LOWER(c.Name) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(c.Email) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(c.LookupCode) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(c.PhoneNumber) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(c.Website) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Contacts search with normalized structure
            SELECT TOP 10
                'Contact' AS DataType,
                co.ContactId AS Id,
                co.FirstName + ' ' + co.LastName AS [Primary],
                COALESCE(cl.Name, cr.CarrierName, '') AS Parent,
                '' AS AddressType
            FROM Contacts co WITH (NOLOCK)
            LEFT JOIN Clients cl WITH (NOLOCK) ON co.ClientId = cl.ClientId
            LEFT JOIN Carriers cr WITH (NOLOCK) ON co.CarrierId = cr.CarrierId
            LEFT JOIN EmailAddresses ea WITH (NOLOCK) ON co.ContactId = ea.ContactId
            LEFT JOIN PhoneNumbers pn WITH (NOLOCK) ON co.ContactId = pn.ContactId
            WHERE 
                LOWER(co.FirstName + ' ' + co.LastName) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(ea.Email) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(pn.Number) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(co.Title) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Carriers search
            SELECT TOP 10
                'Carrier' AS DataType,
                cr.CarrierId AS Id,
                cr.CarrierName AS [Primary],
                '' AS Parent,
                '' AS AddressType
            FROM Carriers cr WITH (NOLOCK)
            WHERE 
                LOWER(cr.CarrierName) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(cr.LookupCode) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(cr.CarrierNickname) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Policies search
            SELECT TOP 10
                'Policy' AS DataType,
                p.PolicyId AS Id,
                p.PolicyNumber AS [Primary],
                cl.Name AS Parent,
                '' AS AddressType
            FROM Policies p WITH (NOLOCK)
            LEFT JOIN Clients cl WITH (NOLOCK) ON p.ClientId = cl.ClientId
            WHERE 
                LOWER(p.PolicyNumber) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Address search with AddressType
            SELECT TOP 10
                'Address' AS DataType,
                ISNULL(cl.ClientId, ISNULL(cr.CarrierId, 0)) AS Id,
                a.AddressLine1 + ', ' + a.City + ', ' + a.State AS [Primary],
                COALESCE(cl.Name, cr.CarrierName, '') AS Parent,
                CASE
                    WHEN cl.ClientId IS NOT NULL THEN 'Client'
                    WHEN cr.CarrierId IS NOT NULL THEN 'Carrier'
                    ELSE ''
                END AS AddressType
            FROM Address a WITH (NOLOCK)
            LEFT JOIN Clients cl WITH (NOLOCK) ON a.AddressId = cl.AddressId
            LEFT JOIN Carriers cr WITH (NOLOCK) ON a.AddressId = cr.AddressId
            WHERE 
                LOWER(a.AddressLine1) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(a.City) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(a.PostalCode) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Lead search
            SELECT TOP 10
                'Lead' AS DataType,
                l.LeadId AS Id,
                l.CompanyName AS [Primary],
                '' AS Parent,
                '' AS AddressType
            FROM Leads l WITH (NOLOCK)
            WHERE 
                LOWER(l.CompanyName) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(l.ContactName) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(l.Email) LIKE '%' + @SearchTermLower + '%' OR
                LOWER(l.PhoneNumber) LIKE '%' + @SearchTermLower + '%'

            UNION ALL

            -- Renewals search with date range filter and separated Primary and Parent fields
            SELECT TOP 10
                'Renewal' AS DataType,
                r.RenewalId AS Id,
                cl.Name AS [Primary],  -- ClientName as Primary
                FORMAT(r.RenewalDate, 'MM/dd') + ' ' + ISNULL(p.LineNickname, '') AS Parent,  -- Date and Product Nickname as Parent
                '' AS AddressType
            FROM Renewals r WITH (NOLOCK)
            INNER JOIN Clients cl WITH (NOLOCK) ON r.ClientId = cl.ClientId
            LEFT JOIN Products p WITH (NOLOCK) ON r.ProductId = p.ProductId  -- Join to get LineNickname from Products
            WHERE 
                LOWER(cl.Name) LIKE '%' + @SearchTermLower + '%'
                AND r.RenewalDate BETWEEN DATEADD(DAY, -30, @Today) AND DATEADD(DAY, 120, @Today)
        )

        SELECT *
        FROM SearchResults
        ORDER BY
            CASE DataType
                WHEN 'Client' THEN 1
                WHEN 'Contact' THEN 2
                WHEN 'Carrier' THEN 3
                WHEN 'Renewal' THEN 4
                WHEN 'Policy' THEN 5
                WHEN 'Address' THEN 6
                WHEN 'Lead' THEN 7
                ELSE 8
            END,
            [Primary];
    END TRY
    BEGIN CATCH
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();

        RAISERROR (@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;
GO