# QT9 Software Developer Project Take-Home Assignment

Prerequisites
-------------
- Visual Studio 2022 or later
- .NET 9 SDK
- .NET Framework 4.8 development tools
- SQL Server 2019 or later
- SQL Server Management Studio

Technologies Used
---------------
- ASP.NET Core Web API
- ASP.NET Web Forms (.NET Framework)
- SQL Server
- ADO.NET (no Entity Framework)
- C#

Setup
--------
1. Run the supplied SQL script to create the MoviesDB database.
2. Update `QT9.Api/appsettings.json` if your SQL Server instance is not `SBCOMPUTER`.
3. Configure both `Movies.Api` and `Movies.WebForms` as startup projects.
4. Start the API using its HTTP profile.
5. Confirm that the API is available at:`http://localhost:5267/api/movies`
6. Run the Web Forms application.
 
Solution Structure
------------------
Movies.Api
    REST API retrieves movie data from SQL Server using ADO.NET

Movies.WebForms
    Consumes the API and displays the results in a GridView on the default page. Clicking the linkbutton on a row will populate a separate textbox with the data from the row clicked.

Notes
-----
This solution was developed using ADO.NET to satisfy the assignment requirement of not using Entity Framework.

The connection string is stored in appsettings.json for simplicity.

In a production environment, sensitive configuration such as database connection strings should be managed securely using Azure Key Vault, environment variables, or another enterprise secrets management solution.