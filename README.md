# River Project

## Overview

The **River Project** is a distributed system consisting of two main applications:

1. **River.API** - A Web API that provides RESTful endpoints for managing users, organizations, corridors, and transactions.
2. **River.TransactionProcessingService** - A Worker Service responsible for processing transactions asynchronously.

Both applications use **MongoDB** as the database and are structured to ensure scalability, reliability, and maintainability.

---

## Project Structure

The project is organized as follows:

```
river/
│-- River.API/                      # Web API application
│   │-- Controllers/                 # API controllers
│   │-- DTOs/                        # Data Transfer Objects
│   │-- Models/                      # Database models
│   │-- Repositories/                # Data access layer
│   │-- Services/                    # Business logic layer
│   │-- Configurations/              # Configuration settings
│   └-- Program.cs                   # Entry point of the API
│
│-- River.TransactionProcessingService/  # Worker service
│   │-- Services/                        # Background processing services
│   │-- Models/                          # Database models used in the service
│   └-- Program.cs                       #Entry point of the worker service
│
│-- River.sln                     # Solution file
│-- README.md                      # Project documentation
```

---

## Technologies Used

- **.NET 8** (or the applicable version)
- **C#**
- **MongoDB** (for data storage)

---

## Setup Instructions

### Prerequisites

Ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [MongoDB](https://www.mongodb.com/try/download/community)

### Running the Web API

1. Navigate to the `River.API` directory:
   ```sh
   cd River.API
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Run the application:
   ```sh
   dotnet run
   ```
4. The API should now be running at `http://localhost:5000` (or your configured port).

### Running the Worker Service

1. Navigate to the `River.TransactionProcessingService` directory:
   ```sh
   cd River.TransactionProcessingService
   ```
2. Restore dependencies:
   ```sh
   dotnet restore
   ```
3. Run the worker service:
   ```sh
   dotnet run
   ```

---

## Worker Service Functionality

The `River.TransactionProcessingService` runs as a background process, handling tasks such as:

- Processing financial transactions
- Handling failed transactions
- Updating transaction statuses asynchronously

---

## Environment Variables

Both applications use environment variables for configuration. Set the following variables before running:

```sh
reference the .env.example to setup your environment
```

For production, store these securely in a `.env` file or use a secret manager.

---
