# ------------------------------------------------------------
# Using the DotNetCore 3.1 SDK image to build the applications
# ------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/core/sdk:3.1.101-buster AS build
RUN curl -sL https://deb.nodesource.com/setup_10.x | bash - \ 
    && apt update \
    && apt-get install -y --no-install-recommends \
        nodejs 
        # add other dpenedencies here

# Install the angular CLI
RUN npm install -g @angular/cli

# Now copy all the source code so that we can build the Web API and Angular apps
WORKDIR /build/
COPY . .
WORKDIR /build/MagicBus.AdminPortal/
RUN dotnet restore

# --------------------------------------
# Build Stage for WebAPI and Angular SPA
# --------------------------------------
FROM build AS publish
WORKDIR /build/MagicBus.AdminPortal/ClientApp/

# install all the node dependencies
RUN npm i

# publish the SPA webapp - this will publish both the WebAPI and the Angular SPA
WORKDIR /build/MagicBus.AdminPortal/
RUN dotnet publish "MagicBus.AdminPortal.csproj" --no-restore -c Release -o /app 

# -----------------------------------------------------------------------------------
# Final stage - copy the published apps to the asp.net container and start the webapp
# -----------------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.1-buster-slim AS final

# Expose our http/https ports
EXPOSE 80
EXPOSE 443

# Set up some ASP.NET Environment variables
ENV ASPNETCORE_URLS http://+:80
ENV ASPNETCORE_ENVIRONMENT Production

# Copy the published app from the previous stage
WORKDIR /app
COPY --from=publish /app .
RUN ls
RUN ls -l /app/dll

# Start the webapp
ENTRYPOINT ["dotnet", "MagicBus.AdminPortal.dll"]