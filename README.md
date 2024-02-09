# The Guardian
This project trys to apply main concept of protecting background services. A gateway provides background services to authentication and authorization dynamics.

## Services

### Gateway Service
Gateway Service is using Ocelot library. 

### Auth Service
Auth Service is reachable service directly (w/o token) but over gateway. The service's main mission to generate token which will be used for Employee Service.

### Employee Service
Employee Service is protected by a gateway. However, for proving that we have a opportunity to open an end-point for public, there is one end-point which is '/api/employee'. The end-point may be called w/o token.

## Developer Notes
PLEASE, CHECK PROJECT'S ISSUE PAGE TO SEE WHAT ARE MISSING ON THE PROJECT

## Running Project
- Be sure Docker is installed and up to date on PC/Mac
- Open a terminal
- Go to root file of the project.
- Run command bellow
```
docker-compose up -d --no-deps --build
```