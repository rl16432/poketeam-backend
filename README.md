# PokeTeam

This API performs CRUD operations to create, read, update and delete users, while integrating with PokeApi to *add* and *delete* Pokemon from a user's team.

## PokeApi
https://pokeapi.co

## Onion Architecture

Onion architecture was utilised in this project with domain, repository, services and presentation layers which are increasingly further from the domain, and closer to the 'presentation'. This architecture allows for separation of concerns, with different layers performing different functions closer or further from the domain. 

![image](https://user-images.githubusercontent.com/65014987/189576074-c3a9fd25-e5f5-453e-909e-6ede48f55338.png)

- The domain layer consists of the database models as well as the model defining the response from PokeAPI.
- The repository layer defines classes which interact with the database. This includes database migrations, database contexts and read/write operations
- The services layer contains the logic used by the controller and references operations defined in the repository layer
- The presentation layer contains the controller and uses the logic defined in services to present data to the user through an API

## Entity Framework Core

The ASP.NET minimal API makes use of EF Core to connect to a local/production SQL Server database. The default connection string is specified in appsettings.json. The database context is injected into the application and operations are performed on the specified database.

![image](https://user-images.githubusercontent.com/65014987/189569482-9ea0ccf3-39d1-4ecc-aa5f-d2c5e6b6c310.png)

## CI/CD with Github Actions

A CI/CD pipeline is setup with Github Actions on this repository. The API is deployed to Azure App Service and published to Azure API Management. 

![image](https://user-images.githubusercontent.com/65014987/189579402-97a137e0-aa59-4cc9-bb23-49ed88bda45f.png)

## Unit Testing

Unit testing are performed on the repository, service and API layers separately. Onion architecture assists with unit testing due to the low coupling between the layers, making bugs easier to identify. To generate mock data, an EF Core in memory database is constructed at the beginning of each test with sample data prepared. The data is initialised as such in: [DbTestSetup.cs](https://github.com/rl16432/msa-phase-3-backend/blob/main/msa-phase-3-backend.Testing/DbTestSetup.cs).

Mocks are also generated for the appsettings.json configuration and external HTTP client, in order to test consistently. Requests to the mock HTTP client for PokeAPI returns an object read from this [JSON](https://github.com/rl16432/msa-phase-3-backend/blob/main/msa-phase-3-backend.Testing/TestFiles/litten.json) file, simulating the API response.

Tests are performed for the main operations in the repository layer (get, create, update, delete), the services defined in the service layer and the controller methods (method returns the correct status result, duplicate usernames are prohibited...).

## FluentValidation/FluentAssertion

All test cases performed in this application utilise **FluentAssertion**. 

![image](https://user-images.githubusercontent.com/65014987/189625405-c6712221-11d2-4b6d-8c2b-088fd5565f63.png)

**FluentValidation** is also used to provide validation rules for the Trainer model. The username has to be between 3 and 20 characters, and the number of Pokemon for each trainer is capped at 6.

![image](https://user-images.githubusercontent.com/65014987/189623651-d7632750-2e7b-40de-84cf-1daa4ef1819a.png)



