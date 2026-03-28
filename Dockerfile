# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY *.csproj ./
COPY . .

# Restore dependencies
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o out --self-contained true --runtime linux-x64

# Use the official .NET runtime image for the final stage
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# Copy the published application from the build stage
COPY --from=build /app/out ./

# Create logs directory
RUN mkdir -p Logs

# Expose the default server port (can be overridden at runtime)
EXPOSE 22110

# Set the entry point
ENTRYPOINT ["./LiteNetLibDebugApp"]