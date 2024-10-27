# PlanningPoker

PlanningPoker is an ASP.NET Core Blazor application designed to facilitate agile planning and
estimation sessions. It uses Blazor Interactive Server Side Rendering (SSR), based on SignalR websocket connections
between server and client.

## Features

- Real-time collaborative planning
- User-friendly interface
- Connects to a Gitlab repository via REST-API

## Prerequisites

- .NET Core SDK 8.0
- Docker (if running the application using Docker). How to install a docker-daemon and getting started with docker, see here https://docs.docker.com/get-started/.

## Getting Started

### Configuration

Configuration can be done in either AppSettings.json or local environment variables. The following sections must be
overwritten:

```json
    {
    "Gitlab": {
        "Api": {
            "Url": "REST API URL, see https://docs.gitlab.com/ee/api/rest/",
            "Pat": "PersonalAccessToken - To be defined on group level in Gitlab"
        },
        "Repository": {
            "GroupName": "Group Name in Gitlab Repository"
        }
    }
}
```

### Running the Application Manually

1. **Clone the Repository**
   ```bash
   git clone https://github.com/PlanningPokerFaces/planningpoker.git
   cd planningpoker/PlanningPoker.Website

2. **Build and Run the Application**

    There are two predefined launch-profiles 'Test' and 'Production'.
The main difference is the more verbose error messages in 'Test' profile.
   ```bash
    dotnet run --project PlanningPoker.Website.csproj --launch-profile Production

3 **Access the Application**
   Open your web browser and navigate to https://localhost:7076.

### Running the Application with Docker

1. **Clone the Repository**
   ```bash
   git clone https://github.com/PlanningPokerFaces/planningpoker.git
   cd planningpoker

2. **Build the Docker Image**
   ```bash
    docker build -t planningpoker -f .\PlanningPoker.Website\Dockerfile .

3. **Run the Docker Container**
   ```bash
    docker run -d -p 8080:8080 --name planningpoker planningpoker
    ```
    Environment variables can be added with an environment file or with the -e argument:

   ```bash
    docker run -d -p 8080:8080 -e "ASPNETCORE_ENVIRONMENT=Test" -e "GITLAB__API__URL=..." -e "GITLAB__API__PAT=..." -e "GITLAB__REPOSITORY__GROUPNAME=..." --name planningpoker planningpoker
    ```

4. **Access the application**

    Open your web browser and navigate to http://localhost:8080.

### License

This project is licensed under the MIT License. See the LICENSE file for details.

### Maintenance

This solution was developed as part of a Master Thesis and is no longer actively maintained. Support and bug fixes are not provided. Pull requests may be reviewed and merged sporadically at the author's discretion.

Happy Planning! 🎉


