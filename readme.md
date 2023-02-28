# Development

I'm using vscode with ionide for development and .net Core Test Explorer for tests.
Dev environment configuration is uploaded to allow for test discovery.

Opening vscode from the src/ folder has webapi as start project.

# Testing

To run the services use 'docker compose up' in the repository root folder.

   - swagger is up for webapi even on containers for test purposes.
   - webapi: http://localhost:8080/swagger
   - telemetry: http://localhost:9411
   - rabbitMQ: http://localhost:15672 user: guest, pass: guest

To run unit tests:
run_tests.ps1 script runs unit tests for all projects

To run integration tests:
   - navigate to src/integration-tests
   - run the test environment with 'docker compose up -d'
   - run dotnet test

test coverage configuration for coverlet is to be done.
