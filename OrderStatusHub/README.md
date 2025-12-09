# Order Status Integration Hub
## Project Overview

This project integrates order data from two different legacy systems (System A and System B), each providing data in a different format (JSON and CSV).
The system reads both files, normalizes the data (including dates and status codes), and exposes a unified REST API.
A simple frontend UI is included to demonstrate how a client application can consume the API, view the orders, and filter them by status.

The solution focuses on:

 - Data normalization
 - Clean API design
 - Error handling
 - Separation of concerns
 - Readability and maintainability
 - Working within real-world constraints (limited time, legacy formats)


# Technology Stack

Backend (API):
  - .NET 8 Web API
  - C#
  - CsvHelper for CSV parsing
  - Dependency Injection for clean architecture
  - xUnit + Moq (for test project)

Frontend:
  - Vanilla HTML + JavaScript + CSS
  - Built to demonstrate API usage with minimal tooling

Benefits of this stack:
  .NET 8 is modern, fast, and ideal for working with structured data.
  Mircosoft technologies offer long time support and continous enhancements.
  System.Text.Json and CsvHelper provide quick parsing tools.


## 📁 Project Structure

```
OrderStatusHub/
├── backend/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Program.cs
│   └── OrderStatusHub.Api.csproj
│
├── OrderStatusHub.Tests/
│    ├── OrderStatusHub.Tests.csproj
│    ├── OrdersControllerTests.cs
│
├── frontend/
│   ├── index.html
│   ├── script.js
│   └── styles.css
│
├── data/
│   ├── system_a_orders.json
│   └── system_b_orders.csv
│
└── README.md
```

---

## 🚀 Setup Instructions

### Prerequisites
- Visual Studio 2022
- .NET SDK 8.0+
- Node.js (optional for frontend server)

---


# Backend Setup

### 1. Open the Project in Visual Studio

Open:
```
OrderStatusHub/OrderStatusHub.sln
```

### 2. Restore NuGet Packages

Visual Studio restores them automatically.

### 3. Build the API
```
Build → Build Solution
```

### 4. Run the API

Swagger opens at:
```
https://localhost:<port>/swagger
```

### 5. Verify Data Loads
```
https://localhost:<port>/api/orders
```

---

# Frontend Setup

## Option A — Open HTML Directly
Open:
```
frontend/index.html
```

If blocked by CORS or mixed content rules, use Option B.

## Option B — Serve with Local Static Server
```
cd frontend
npx http-server -p 8080
```

Open:
```
http://localhost:8080
```

---


# API Documentation

## Base URL
```
https://localhost:<port>/api
```

---

## GET /api/health
Basic service health check.

### Success Response
```json
{
  "status": "ok",
  "timestamp": "2025-01-20T09:25:00Z"
}
```

---

## GET /api/orders
Returns unified orders.

---

## GET /api/orders/{orderId}

### 404 Example
```json
{
  "message": "Order with ID 'XYZ' was not found.",
  "code": "ORDER_NOT_FOUND"
}
```

---

## GET /api/orders/search?status=Processing
Filters by normalized status.

### 400 Example
```json
{
  "message": "Invalid status 'XYZ'. Valid values: Pending, Processing, Shipped, Completed, Cancelled",
  "code": "INVALID_STATUS"
}
```

---

# Design Decisions

- Unified statuses: `Pending`, `Processing`, `Shipped`, `Completed`, `Cancelled`
- Unified date format: `yyyy-MM-dd`
- In-memory storage for simplicity
- Service abstraction via interface
- Basic frontend with API consumption

---

# Running Tests ()

```
cd tests
dotnet test
```

---

# Known Limitations

  - Sanitize data 
  - Handle incorrect data structure
  - Possible Injection attacks

# Future Improvements

  - Add a comprehensive health check end point, this is just to check if api is running.
  - Include performance metrics to identify and mitigate bottlenecks and perfomance issues
  - Implement Caching 
  - Token authentication
  - Move Hardcoded constants to enums or a constant file
  - Add Appropriate comments round the logic

# Deployment (Render)

### Build command:
```
cd backend && dotnet publish -c Release -o publish
```

### Publish API on Azure:
```
Right click on the api project and publish to Azure directory
```

Ensure `/data` folder is included in repo root.

---
# End of README
