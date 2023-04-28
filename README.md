# SuperDuperProperties
A very basic .NET application for managing rental properties

## Web Application
Written in ASP.NET using Razor pages and some vanilla CSS.  The application provides an interface to manage rental properties along with the property owners, leases, and tenants.

## HTTP API
Uses the same controllers as the web api but with different endpoints add/update owners, properties, leases and tenants.  It also provides a basic search endpoint to find properties based on specific criteria such as number of bedrooms.

## Database
The data access layer was written to use stored procedures in SQLServer.  Dapper is used for the ORM.  Because this project is so small, no database migration tool was used.  The table creation queries and stored procedures are saved in the Data folder for convenience.
