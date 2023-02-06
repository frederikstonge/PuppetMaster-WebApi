[![License: GPL v3](https://img.shields.io/badge/License-GPL_v3-blue.svg)](https://www.gnu.org/licenses/gpl-3.0)

# PuppetMaster WebApi Server

Client repo is here : [PuppetMaster Client](https://github.com/frederikstonge/PuppetMaster-Client)

## Technologies
- ASPNET Core WebApi
- Swagger
- SignalR
- Openiddict
- Quartz.NET
- EntityFramework
- AutoMapper

## Prerequisite
- Visual Studio
- .NET 6 SDK
- Microsoft SQL Server database

## Before you run
You will need to update your appsettings (secrets if you are running locally) and add your database connection string.

### If you deploy to azure

- You will need to generate two pfx certificate files (Encryption and Signing)
- Add them to your app service and use the environment variable `WEBSITE_LOAD_CERTIFICATES` with comma-separated thumbprint values.
