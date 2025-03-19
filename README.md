# spac_uge3_SD

# spac_uge3_SD

### Overview

The Api can be tested with Swagger UI. Run the server with "{port}/swagger" and a interface with all the possible http calls to my api will be visible. In the the top right corner there should be a button called "Authorize". This can be used to test the authorization functionality-- in it the tester should write "Bearer {token}". The token can be retrieved by using the MapPost("register) endpoint and creating a username and password--then using MapPost("login") and using the token it gives you. 

The Api consists of the following programs:

- Program.cs
- Cereal.cs
- CsvParser.cs

They are all written in C# in dotnet 9.0. The api have been written as a minimal api using lambda functions. 

### Program.cs
Program.cs runs the server and defines the endpoints and http methods.
It defines the following endpoints:

- **app.MapGet("/")**: This simply retrieves the string "Cereal Database"

- **app.MapGet("/cereal")**: This retrieves a list of all cereals in the database

- **app.MapGet("/cereal/query/{query}")**: This retrieves a list of cereals that fullfill the query condition. For example, "/cereal/query/Vitamins > 20", retrieves all cereals where the vitamin key has a value above 20.

- **app.MapGet("/cereal/image/{id}"**): This retrieves an image based on the Cereal.Image property, which consist in a path to the image if one exists. 

- **app.MapPost("/cereal")**: This adds a new cereal to the database.

- **app.MapPost("/cereal/{csvPath}"**) This adds one or more cereals to the database, by parsing a csv file. The file has to have the same structure and fields as the "Cereal.csv" file.

- **app.MapPut("/cereal/{id}")**: This updates the cereal with the given id.

- **app.MapDelete("/cereal/{id}"**): This deletes the cereal with the given id.

- **app.MapDelete("/cereal")**: This deletes all cereals in the database

- **app.MapPost("/register")**: This allows a user to register with username and password.

- **app.MapPost("/login")**: This logs in the user and allows them to post, put, and delete.

### Cereal.cs
This is where the Cereal class is defined, as well as the database context.

### CvsParser.cs
This contains the CvsParser which is used in the app.MapPost("/cereal/{csvPath}") call via dependency injection to parse a csv file and return a list of cereals. It also contains the static method GetCerealImage, which is used to find the value for the Cereal.Image value by looking in the "CerealPictures" Directory.
