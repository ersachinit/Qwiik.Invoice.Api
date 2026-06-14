# Qwiik Invoice API

## Overview

Qwiik Invoice API is a multi-tenant invoice management system built using ASP.NET Core 8, Entity Framework Core, and SQL Server.

The solution supports invoice creation, retrieval, status updates, dashboard reporting, pagination, and tenant-level data isolation.

---

## Features

* Multi-Tenant Invoice Management
* Create Invoice
* List Invoices
* Get Invoice Details
* Update Invoice Status
* Dashboard Summary
* Pagination Support
* Global Exception Handling
* Validation Rules
* Swagger Documentation
* Entity Framework Core Migrations

---

## Technology Stack

* ASP.NET Core 8 Web API
* Entity Framework Core
* SQL Server
* Swagger / OpenAPI
* Docker Support

---

## Project Structure

Controllers
Data
DTOs
Entities
Exceptions
Middleware
Migrations
Services

---

## Database Setup

Update the connection string inside:

appsettings.json

Run migrations:

Add-Migration InitialCreate
Update-Database

Or if migrations already exist:

Update-Database

---

## Running the Application

Run the application:

dotnet run

Swagger will be available at:

https://localhost:<port>/swagger

---

## Tenant Configuration

The API uses the following request header:

X-Tenant-Id

Example:

X-Tenant-Id: 1

All invoice operations are scoped to the supplied tenant.

---

## API Endpoints

### Create Invoice

POST /api/invoices

### Get Invoices

GET /api/invoices?page=1&pageSize=20

### Get Invoice By Id

GET /api/invoices/{id}

### Update Invoice Status

PATCH /api/invoices/{id}/status

### Dashboard Summary

GET /api/invoices/dashboard

---

## Pagination

Invoice listing supports pagination:

GET /api/invoices?page=1&pageSize=20

Response includes:

* Page
* PageSize
* TotalRecords
* Items

---

## Validation Rules

Implemented validations include:

* Invoice Number is required
* Customer Name is required
* Amount must be greater than zero
* Due Date cannot be before Invoice Date
* Duplicate invoice numbers are not allowed within the same tenant

---

## Error Handling

The solution uses centralized exception handling through custom middleware.

Supported response types:

 HTTP Status  Scenario           
 -----------  ------------------ 
 400          Validation Error   
 404          Resource Not Found 
 409          Conflict           
 500          Unexpected Error   

---

## Assumptions

* Tenant information is provided through the 'X-Tenant-Id' header.
* Invoice numbers must be unique within a tenant.
* Authentication and authorization are outside the scope of this assignment.
* JWT-based tenant resolution would be implemented in a production environment.

---

## Production Considerations

Potential production enhancements:

* JWT Authentication & Authorization
* Azure App Service Deployment
* Azure SQL Database
* Azure Key Vault
* Azure Application Insights
* Automated Testing
* CI/CD Pipeline
* Distributed Caching

Refer to 'SOLUTION_NOTES.md' for detailed design decisions and architectural considerations.
