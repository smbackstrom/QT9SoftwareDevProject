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
1. Run the supplied SQL script, in the movieDB.txt file, to create the `MoviesDB` database.
2. Open `QT9SoftwareDevProject.sln` in Visual Studio.
3. Restore NuGet packages if prompted (or select **Restore NuGet Packages** from the solution).
4. Update the SQL Server connection string in Movies.Api/appsettings.json to your SQL Server instance.
5. Configure Multiple Startup Projects:
   - Movies.Api – Start
   - Movies.WebForms – Start
6. Start the solution.
7. The API will be available at:
   - https://localhost:7165/api/movies
   - http://localhost:5267/api/movies
8. Verify that the Web Forms application loads the movie list successfully.
 
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
