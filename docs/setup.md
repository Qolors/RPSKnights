# Self-Hosting RPS Knights
This guide provides comprehensive instructions for self-hosting RPS Knights on your local machine. The process involves setting up Docker and configuring the necessary files. Please adhere to the steps meticulously to ensure a successful setup.


Before you begin, ensure Docker is installed on your machine. If not, follow these steps:

1. Install Docker CLI tooling:

    - For Windows and MacOS, download and install Docker Desktop from [here](https://www.docker.com/products/docker-desktop).
    - For Linux, follow the instructions [here](https://docs.docker.com/engine/install/) to install Docker Engine.

2. Validate the installation by opening a terminal and executing the command `docker --version`. The Docker version should be displayed in the output.

## File Setup & Configuration

To swiftly get RPS Knights up and running on your machine, adhere to these steps:

1. Create a `docker-compose.yaml` file with the following configurations in your **root** folder (ensure to replace your Bot Token):

    ```
    version: "3.9"

    services:
    bot:
        build: 
            dockerfile: ./LeagueStatusBot/Dockerfile
            context: .
        container_name: RPSKnightsBot
        restart: unless-stopped
        environment:
        - DISCORD_BOT_TOKEN=${DISCORD_BOT_TOKEN:-**YOUR BOT TOKEN HERE**}
        volumes:
        - ./game.db:/app/game.db
    ```

2. If you are proficient with EF Core, you can create a new SQLite Database in the `LeagueStatusBot.RPGEngine.Data` directory, or you can simply use the one included in the root folder.

3. Create a `Dockerfile` file with the following configurations inside the **LeagueStatusBot** Folder:

    ```
    # Use SDK for building
    FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
    WORKDIR /source

    # Copy and restore both projects
    COPY ["./LeagueStatusBot/LeagueStatusBot.csproj", "LeagueStatusBot/"]
    COPY ["./LeagueStatusBot.RPGEngine/LeagueStatusBot.RPGEngine.csproj", "LeagueStatusBot.RPGEngine/"]
    RUN dotnet restore "LeagueStatusBot/LeagueStatusBot.csproj"
    RUN dotnet restore "LeagueStatusBot.RPGEngine/LeagueStatusBot.RPGEngine.csproj"

    # Copy all source code and build
    COPY . .
    WORKDIR "/source/LeagueStatusBot"
    RUN dotnet build "LeagueStatusBot.csproj" -c Release -o /app/build

    FROM build AS publish
    RUN dotnet publish "LeagueStatusBot.csproj" -c Release -o /app/publish /p:UseAppHost=false

    # Final image
    FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
    WORKDIR /app
    COPY --from=publish /app/publish .
    COPY ["./LeagueStatusBot.RPGEngine/Core/Assets/Maps/", "/app/Assets/Maps"]
    COPY ["./LeagueStatusBot.RPGEngine/Core/Assets/Sprites/", "/app/Assets/Sprites"]
    ENTRYPOINT ["dotnet", "LeagueStatusBot.dll"]
    ```


4. Once all files are correctly positioned, you can initiate the application by navigating to your **root** folder and executing the following CLI Command -> `docker-compose up -d`

Congratulations! You have successfully set up RPS Knights.

*This guide is intended to facilitate a swift setup of a self-hosted instance on your local machine. However, if you intend to host this application in a different environment, we strongly recommend not using Docker environment variables for security reasons.*
