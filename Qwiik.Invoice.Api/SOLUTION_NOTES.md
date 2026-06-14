# Qwiik Invoice API – Solution Notes

## 1. How to Run the Project

### Prerequisites

* .NET 8 SDK
* SQL Server or SQL Server LocalDB
* Visual Studio 2022 or Visual Studio Code

### Steps

1. Update the database connection string in 'appsettings.json'.
2. Apply migrations:

Update-Database

3. Run the application:

dotnet run

4. Open Swagger:

https://localhost:<port>/swagger

5. Insert one manually created tenant record into the 'Tenants' table using SQL Server Management Studio or a similar tool:
Insert into Tenants (Name, CreatedAt) Values ('Test Tenant', GETUTCDATE())

6. Use the following request header when testing APIs:

X-Tenant-Id: 1

---

## 2. Assumptions

The following assumptions were made while implementing the solution:

* Tenant information is supplied through the 'X-Tenant-Id' request header.
* Invoice numbers must be unique within a tenant.
* A tenant must already exist before invoices can be created.
* Authentication and authorization are outside the scope of this assignment.
* SQL Server is used as the primary datastore.
* The focus is on backend API implementation rather than UI development.

---

## 3. Architecture Overview

The solution follows a layered architecture:

Controllers
    ↓
Services
    ↓
Entity Framework Core DbContext
    ↓
SQL Server

### Design Principles

* Separation of concerns
* Maintainability
* Simplicity
* Production awareness
* Multi-tenant isolation

### Why No Repository Pattern?

Entity Framework Core already provides Repository and Unit of Work behavior through 'DbContext' and 'DbSet'.

To avoid unnecessary abstraction and keep the implementation pragmatic, data access is performed directly through the service layer using 'ApplicationDbContext'.

This reduces boilerplate code while maintaining a clear separation between API, business logic, and persistence layers.

---

## 4. Domain Model Explanation

### Tenant

Represents a customer or organization using the platform.

Fields:

* Id
* Name
* CreatedAt

### Invoice

Represents a customer invoice.

Fields:

* Id
* TenantId
* InvoiceNumber
* CustomerName
* Amount
* InvoiceDate
* DueDate
* Status
* CreatedAt
* UpdatedAt

### Invoice Status

Draft
Pending
Paid
Cancelled

The status model supports a simple invoice lifecycle while keeping the implementation lightweight.

---

## 5. Database Design Explanation

### Tables

#### Tenants

Stores tenant information.

#### Invoices

Stores invoice records and references the owning tenant through 'TenantId'.

### Relationships

Tenant (1)
    ↓
Invoices (Many)

Each invoice belongs to exactly one tenant.

### Migration Strategy

Entity Framework Core migrations are used to create and manage database schema changes.

### Primary Key Strategy

The solution uses INT identity columns for primary keys.

Given the expected application scope and centralized SQL Server deployment model, INT identities were chosen to keep indexes compact, reduce storage overhead, and improve query efficiency.

For larger distributed systems requiring globally unique identifiers across multiple services or databases, GUIDs could also be considered depending on the architectural requirements.
---

## 6. API Design Explanation

The API follows REST-style conventions.

### Create Invoice

POST /api/invoices

Creates a new invoice.

### Get Invoices

GET /api/invoices?page=1&pageSize=20

Returns paginated invoices for the current tenant.

### Get Invoice Details

GET /api/invoices/{id}

Returns a specific invoice.

### Update Invoice Status

PATCH /api/invoices/{id}/status

Updates invoice status.

### Dashboard Summary

GET /api/invoices/dashboard

Returns invoice summary information.

### DTO Usage

DTOs are used to:

* Separate API contracts from persistence models.
* Prevent over-posting.
* Reduce unnecessary data exposure.
* Improve maintainability.

---

## 7. Validation Approach

Validation is performed inside the service layer.

Implemented validations include:

* Invoice Number is required.
* Customer Name is required.
* Amount must be greater than zero.
* Due Date cannot be before Invoice Date.
* Duplicate invoice numbers are not allowed within the same tenant.

Custom exceptions are used to return meaningful API responses.

---

## 8. Tenant Isolation Approach

Tenant isolation is implemented using a custom middleware.

### Flow

Request
    ↓
TenantMiddleware
    ↓
TenantContext
    ↓
Service Layer
    ↓
Database Query

The middleware:

1. Reads the 'X-Tenant-Id' header.
2. Stores the value in 'TenantContext'.
3. Makes it available throughout the request lifecycle.

All invoice queries are filtered by TenantId.

Example:

.Where(x => x.TenantId == _tenantContext.TenantId)

This ensures that one tenant cannot access another tenant's data.

---

## 9. Indexing and Performance Strategy

The following indexes were implemented:

### TenantId

Supports tenant-based filtering.

### TenantId + Status

Supports dashboard and status-based queries.

### TenantId + InvoiceNumber (Unique)

Ensures invoice numbers are unique per tenant while allowing multiple tenants to use the same invoice number.

### Pagination

Invoice listing supports:

GET /api/invoices?page=1&pageSize=20

Implemented using:

* Skip()
* Take()
* CountAsync()

This prevents large result sets from being returned and improves scalability.

### Key Selection Considerations

INT identity keys were selected to reduce index size and improve database efficiency.

Compared to GUID-based keys, INT values require less storage, produce smaller indexes, reduce page reads, and can improve cache utilization. These characteristics help reduce database resource consumption as data volume grows.
---

## 10. Testing Approach

The solution was manually tested using Swagger.

Scenarios verified:

* Invoice creation
* Duplicate invoice prevention
* Validation failures
* Invoice retrieval
* Invoice status updates
* Dashboard aggregation
* Tenant isolation
* Pagination

Given the assignment time constraints, automated unit and integration tests were not implemented.

---

## 11. Azure Deployment and Monitoring Considerations

A production deployment could use:

### Hosting

* Azure App Service

### Database

* Azure SQL Database

### Monitoring

* Azure Application Insights

### Secrets Management

* Azure Key Vault

### CI/CD

* Azure DevOps Pipelines

These services would provide scalability, monitoring, operational visibility, and secure configuration management.

### Production Best Practices

For a production deployment, I would follow the following practices:

#### Scaling

* Deploy the API to Azure App Service with autoscaling enabled.
* Scale based on CPU utilization, memory consumption, or request volume.
* Azure SQL Database can be configured with appropriate performance tiers depending on workload requirements.

#### Rollback Strategy

* Use Azure DevOps CI/CD pipelines with deployment slots.
* Deploy new versions to a staging slot first.
* Perform validation and smoke testing.
* Swap staging and production slots after verification.
* Roll back quickly by swapping back to the previous slot if issues are detected.

#### Resource Utilization

* Monitor CPU, memory, response times, and database performance through Azure Application Insights.
* Use pagination to reduce unnecessary database and network load.
* Review query execution plans and indexing strategy as data volume grows.
* Configure alerts for abnormal resource consumption and application failures.

#### High Availability

* Use Azure App Service's built-in availability features.
* Enable automated database backups.
* Use Azure SQL high availability options for production workloads.

#### Observability

* Centralize application logs using Application Insights.
* Track exceptions, failed requests, dependency calls, and performance metrics.
* Configure dashboards and alerts for proactive monitoring.

---

## 12. Security Considerations

For simplicity, tenant identification is handled using the 'X-Tenant-Id' request header.

In a production SaaS environment:

* JWT Authentication would be implemented.
* Tenant information would be derived from JWT claims.
* Authorization policies would enforce access control.
* Secrets would be stored in Azure Key Vault.
* HTTPS would be enforced for all traffic.

Input validation and centralized exception handling have also been implemented.

---

## 13. Known Limitations

The following items were intentionally left out to keep the solution aligned with the assignment scope:

* JWT Authentication and Authorization
* Automated Unit Tests
* Automated Integration Tests
* Docker Compose setup
* CI/CD pipeline implementation
* Audit logging
* Advanced analytics and reporting
* Distributed caching

---

## 14. What I Would Improve With More Time

If more time were available, I would consider implementing:

* JWT Authentication & Authorization
* Automated Unit Tests
* Automated Integration Tests
* Docker Compose configuration
* CI/CD Pipeline
* Audit Logging
* Soft Delete support
* Distributed Caching
* OpenTelemetry Observability
* Advanced Dashboard Analytics
* Background Processing
* Azure-native deployment automation

---

## Logging

The solution uses ASP.NET Core's built-in logging abstraction ('ILogger').

For production environments, logs would typically be routed to:

* Azure Application Insights
* Serilog
* Centralized logging platforms

---

## Summary

The goal of this implementation was to provide a clean, maintainable, and production-aware multi-tenant invoice management API while keeping the solution pragmatic and aligned with the assignment scope.

Key focus areas included:

* Multi-tenant data isolation
* Pagination and query efficiency
* Centralized exception handling
* Validation
* Clean architecture
* Production deployment considerations
