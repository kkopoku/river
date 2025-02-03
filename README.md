# River

## Overview

The **River Project** is a distributed digital banking system consisting of two main applications:

1. **River.API** - A Web API that provides RESTful endpoints for managing users, organisations, corridors, and transactions.
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
│   └-- Program.cs                       # Entry point of the worker service
│
│-- docker-compose.yml               # Docker Compose file for containerization
│-- .env.example                      # Environment variable configuration example
│-- River.sln                         # Solution file
│-- README.md                         # Project documentation
```

---

## Technologies Used

- **.NET 8** (or the applicable version)
- **C#**
- **MongoDB** (for data storage)
- **Docker & Docker Compose** (for containerization)

---

## Setup Instructions

### Prerequisites

Ensure you have the following installed:

- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [MongoDB](https://www.mongodb.com/try/download/community) (if running outside Docker)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Running the Web API Locally

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
4. The API should now be running at `http://localhost:5141` (or your configured port).

### Running the Worker Service Locally

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

## Running with Docker

### Building the Images

1. Navigate to the project root directory:
   ```sh
   cd river
   ```
2. Build the Docker images:
   ```sh
   docker-compose build
   ```

### Running the Containers

1. Start the containers:

   ```sh
   docker-compose up -d
   ```

   This will start all services in detached mode.

2. To check running containers:

   ```sh
   docker ps
   ```

3. To view logs:

   ```sh
   docker-compose logs -f
   ```

4. To stop the containers:

   ```sh
   docker-compose down
   ```

### Accessing the Application

- The API will be available at `http://localhost:8080`
- The worker service runs in the background processing transactions
- Ensure MongoDB is correctly configured in your `.env` file

---

## Worker Service Functionality

The `River.TransactionProcessingService` runs as a background process, handling tasks such as:

- Processing financial transactions
- Handling failed transactions
- Updating transaction statuses asynchronously
- Reversal of transactions

---

## Environment Variables

Both applications use environment variables for configuration. Set up your `.env` file based on `.env.example` in each application before running:

```sh
cp .env.example .env
```

Ensure necessary environment variables like database connection strings and API keys are set.

For production, store these securely using a secrets manager or Kubernetes secrets.

---

## Deployment to a Server

1. SSH into your server:
   ```sh
   ssh user@your-server-ip
   ```
2. Pull the latest images from Docker Hub:
   ```sh
   docker pull your-dockerhub-username/river-api:latest
   docker pull your-dockerhub-username/river-worker:latest
   ```
3. Run the containers:
   ```sh
   docker-compose up -d
   ```

This will ensure the latest versions of your services are running on the server.

---

## Troubleshooting

- To restart a specific service:
  ```sh
  docker-compose restart river-api
  ```
- To remove all containers and networks:
  ```sh
  docker-compose down --volumes
  ```

For further debugging, check logs using:

```sh
docker logs <container_id>
```