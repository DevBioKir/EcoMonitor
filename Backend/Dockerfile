FROM mcr.microsoft.com/dotnet/sdk:9.0 as build

#working directory inside the container
WORKDIR /app

#copy project files to the container
COPY . .

#restore NuGet-packages
RUN dotnet restore

#build and publishing the application to the /app/out folder in Release mode
RUN dotnet publish -c Release -o out

#runtime for run
FROM mcr.microsoft.com/dotnet/aspnet:9.0

#setting the working directory for startup
WORKDIR /app

#copying published files from the previous build
COPY --from=build /app/out .

#open port 80 for external connections to the application
EXPOSE 80

#Startup application
ENTRYPOINT ["dotnet", "EcoMonitor.dll"] 
