# PokeTeam

This API performs CRUD operations to create, read, update and delete users, while integrating with PokeApi to *add* and *delete* Pokemon from a user's team.

## PokeApi
https://pokeapi.co

## Configuration files

**appsettings.json**

![image](https://user-images.githubusercontent.com/65014987/183883962-fd0a21c7-053d-4445-9314-b3182156937d.png)

**appsettings.Development.json**

![image](https://user-images.githubusercontent.com/65014987/183884048-47ba522a-a487-4c51-a2a8-a6b3698da7ca.png)

The non-development configuration is configured to connect to a locally hosted SQL Server database, and the connection string is provided. To set up a local SQL Server, simply choose the Developer installation from [this link](https://www.microsoft.com/en-us/sql-server/sql-server-downloads).

The development logging level for "Microsoft.AspNetCore" libraries is set to "Information", in order to provide more verbose information.

![image](https://user-images.githubusercontent.com/65014987/183884154-b94cf0bd-17d9-4eda-9f08-ea0ce254527f.png)

## Dependency Injections

Configuring application middleware can simplify the set up of the Web API and its dependencies through dependency injection. 

Through using a separate 'injector' method to inject services into the components of the Web API application, we can simplify our code by removing the need to manually configure all dependencies in the application Program.cs and controller files.

For example, `builder.Services.AddControllers()`, will add the controller services into the web application and removes the need to manually configure and instantiate every individual controller. This will make the code more readable and also allow one to easily add and remove controllers.

Adding the middleware through `app.MapControllers()`, will map the identified controllers to the appropriate routes in one line.

By extending the `ControllerBase` class to define the controllers, we allow the `AddControllers()` method to pick up on this dependency.

### DI Example

![image](https://user-images.githubusercontent.com/65014987/183884182-6d172ba0-15eb-4c3c-bce8-f2bb9205b2da.png)

The program registers the `DbContext` dependency by adding the service to the `WebApplicationBuilder`'s services. 

![image](https://user-images.githubusercontent.com/65014987/183884261-eabace90-6536-43a7-b2f3-8a3ead463dd2.png)

The collection of services in the application when it is built, can then be injected into the `UserController` class. We can now interact with the database within our controllers through the `_context` field. This simplifies our code as we can access dependencies that we need straight from the dependencies defined in the application. The ASP.NET framework will be responsible for creating instances of the required dependency, and deleting it when it is no longer required.

## Testing

### Using substitutes
The NSubsitute library was used to create a **substitute** for the PokeApi return object. This is done by substituting the `HTTPClientFactory` and forcing the `CreateClient` method to return a '`fakeHttpClient`'. 

```c#
httpClientFactoryMock.CreateClient("pokeapi").Returns(fakeHttpClient);
```

This fake HttpClient will only return an OK response containing the serialised JSON object, read from `litten.json`, a file containing an example schema from PokeApi

![image](https://user-images.githubusercontent.com/65014987/183884420-b7c619c9-0a33-4e3c-a2ab-42a4c12f6bda.png)

This `litten.json`, has been set to always copy to the output directory, in `bin/Debug/net6.0` or otherwise, the test will be unable to find the file.

![image](https://user-images.githubusercontent.com/65014987/183884448-b1ca85f4-4b1a-479e-b298-b6dd52b0a089.png)

### How does middleware make code easier to test

The application middleware through dependency injection will allow the controller I defined to take any parameters. Due to the application middleware, services such as the `DbContext`, `HttpClientFactory` are injected by .NET into the constructor of the controllers for use in the API, so that the controllers do not need to manually define its dependencies. 

Therefore, when testing, we can pass in our own classes into the controller contrusctors for the purpose of testing, allowing testing to be easier to perform.

We do not want to use a live database (for testing purposes), as in the Web API, but can define a `DbContext` in the testing `Setup()` function, such that we can test the controller locally. Through this, we can also get a fresh test database for each test, so that modifications to the database in one test will not affect another. The code for that is shown below.

![image](https://user-images.githubusercontent.com/65014987/183884495-4cc6e88b-7673-4466-aa48-3676a97120c3.png)

Additionally, if we were to configure the `HttpClient` inside our controller to point to PokeApi, without having it as middleware, then during testing we would still be calling the external API which is undesirable. Instead, we can create our own substitute for HTTPClient with NSubstitute as shown in the section above.
