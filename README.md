# PokeTeam back-end

This API performs CRUD operations to create, read, update and delete users, while integrating with PokeApi to *add* and *delete* Pokemon from a user's team

## Front-end Repo
https://github.com/rl16432/poketeam-frontend

## Contents

- [Getting started](#getting-started)
   - [Migration](#migration)
- [Dependencies](#dependencies)
   - [Dev Dependencies](#dev-dependencies)
- [Deployment](#deployment)
- [PokeApi](#pokeapi)
  
## Getting started

1. Open solution in Visual Studio

2. Remove "DefaultConnection" connection string in appsettings.json if using in-memory database

   ![image](https://user-images.githubusercontent.com/65014987/217098284-7ce15250-27a9-4cc6-886d-e953fe5cd9ec.png)
   
   Or connect your own SQL Server instance by editing the connection string and applying the existing [**migration**](#migration)
3. Start API
   
   ![image](https://user-images.githubusercontent.com/65014987/192089158-929e52af-0c34-4bac-8762-b83473c2fd62.png)

### Migration

1. There is an existing migration set up [here](https://github.com/rl16432/poketeam-backend/tree/main/poketeam-backend.Repository/Migrations)  
2. Apply migration by selecting the `poketeam-backend.Repository` project and executing `Update-Database`
   
   ![image](https://user-images.githubusercontent.com/65014987/217102548-a578cd37-9403-4e22-8046-10e265dbed01.png)

## Dependencies

- ASP.NET Core 6.0
- Entity Framework Core 6.0
- [Swashbuckle/Swagger](https://github.com/domaindrivendev/Swashbuckle.AspNetCore)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)

### Dev Dependencies
- [FluentAssertions](https://github.com/fluentassertions/fluentassertions)
- [NUnit](https://github.com/nunit/nunit)
- [NSubstitute](https://github.com/nsubstitute/NSubstitute)
- [Moq 4](https://github.com/moq/moq4)
- [Coverlet](https://github.com/coverlet-coverage/coverlet)

## Deployment

**NO LONGER ACTIVE** :moneybag::moneybag::moneybag:

## PokeApi
https://pokeapi.co
