using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CerealApi.CsvParser;
using CerealApi.Models;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Cereals") ?? "Data Source=Cereals.db";
var configuration = builder.Configuration;
var jwtSettings = configuration.GetSection("Jwt");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<CerealDb>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
       Title = "Cereal API", 
       Description = "Cereal database", 
       Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 1;    
})
    .AddEntityFrameworkStores<CerealDb>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings["Key"])),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddScoped<ICsvParser, CsvParser>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
   app.UseSwagger();
   app.UseSwaggerUI(c =>
   {
      c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cereal API V1");
   });
}

// Here I activate authentication and authorization.
app.UseAuthentication();
app.UseAuthorization();

/* The MapGet functions allows the client to retrieve data from the server, based on id, query, 
or just a list of all the cereals. The client can also retrieve  an image based on the id.*/
app.MapGet("/", () => "Cereal database");
app.MapGet("/cereal", async (CerealDb db) =>
{
    try
    {
        return Results.Ok(await db.Cereals.ToListAsync());
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while retrieving cereals: {ex.Message}");
    }
}).RequireAuthorization();
app.MapGet("/cereal/{id}", async (CerealDb db, int id) =>
{
    try
    {
        var cereal = await db.Cereals.FindAsync(id);
        return cereal != null ? Results.Ok(cereal) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while retrieving cereal: {ex.Message}");
    }
});
app.MapGet("/cereal/query/{query}", async (CerealDb db, string query) =>
{
    try
    {
        var cereals = await db.Cereals.Where(query).ToListAsync();
        return cereals.Count != 0 ? Results.Ok(cereals) : Results.NotFound();
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Invalid query: {ex.Message}");
    }
});
app.MapGet("/cereal/image/{id}", async (CerealDb db, int id) =>
{
    try
    {
        var cereal = await db.Cereals.FindAsync(id);
        if (cereal == null || !File.Exists(cereal.Image))
        {
            return Results.NotFound();
        }

        var image = File.OpenRead(cereal.Image);
        return Results.File(image, "image/jpeg");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while retrieving image: {ex.Message}");
    }
});

/* The MapPost functions allows the client to add a new cereal to the database, 
or to parse a CSV file and add the cereals to the database. They both require authentication*/
app.MapPost("/cereal", async (CerealDb db, Cereal cereal) =>
{
    try
    {
        await db.Cereals.AddAsync(cereal);
        await db.SaveChangesAsync();
        return Results.Created($"/cereal/{cereal.Id}", cereal);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while adding cereal: {ex.Message}");
    }
}).RequireAuthorization();
app.MapPost("/cereal/{csvPath}", async (CerealDb db, ICsvParser csvParser, string csvPath) =>
{
    try
    {
        await csvParser.ParseCsvAsync(db, csvPath);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.Problem($"An error occurred while parsing the CSV: {ex.Message}");
    };
}).RequireAuthorization();

// MapPut functions allow the client to update a cereal based on id. Requires authentication.
app.MapPut("/cereal/{id}", async (CerealDb db, Cereal updatecereal, int id) => 
{
    try
    {
        var cereal = await db.Cereals.FindAsync(id);
        if (cereal is null) 
            return Results.NotFound();

        cereal.Name = updatecereal.Name;
        cereal.Mfr = updatecereal.Mfr;
        cereal.Type = updatecereal.Type;
        cereal.Calories = updatecereal.Calories;
        cereal.Protein = updatecereal.Protein;
        cereal.Fat = updatecereal.Fat;
        cereal.Sodium = updatecereal.Sodium;
        cereal.Fiber = updatecereal.Fiber;
        cereal.Carbo = updatecereal.Carbo;
        cereal.Sugar = updatecereal.Sugar;
        cereal.Potass = updatecereal.Potass;
        cereal.Vitamins = updatecereal.Vitamins;
        cereal.Shelf = updatecereal.Shelf;
        cereal.Weight = updatecereal.Weight;
        cereal.Cups = updatecereal.Cups;
        cereal.Rating = updatecereal.Rating;
        cereal.Image = updatecereal.Image;

        await db.SaveChangesAsync();
        return Results.Ok(cereal);
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while updating cereal: {ex.Message}");
    }
}).RequireAuthorization();

// MapDelete functions allow the client to delete all cereals or a cereal based on id. Requires authentication
app.MapDelete("/cereal/{id}", async (CerealDb db, int id) => 
{
    try
    {
        var cereal = await db.Cereals.FindAsync(id);
        if (cereal is null)
            return Results.NotFound();

        db.Cereals.Remove(cereal);
        await db.SaveChangesAsync();
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while deleting cereal: {ex.Message}");
    }
}).RequireAuthorization();
app.MapDelete("/cereal", async (CerealDb db) =>
{
    try
    {
        var allCereals = await db.Cereals.ToListAsync();
        db.Cereals.RemoveRange(allCereals);
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.Problem($"Error ocurred while deleting cereals: {ex.Message}");
    }
}).RequireAuthorization();

// These endpoints are used for user registration and login.
app.MapPost("/register", async (UserManager<IdentityUser> userManager, string username, string password) =>
{
    var user = new IdentityUser { UserName = username };
    var result = await userManager.CreateAsync(user, password);
    if (result.Succeeded)
    {
        return Results.Ok();
    }
    return Results.BadRequest(result.Errors);
});
app.MapPost("/login", async (UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, string username, string password) =>
{
    var result = await signInManager.PasswordSignInAsync(username, password, false, false);
    if (result.Succeeded)
    {
        var user = await userManager.FindByNameAsync(username);
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(jwtSettings["key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id) }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        return Results.Ok(new { Token = tokenString });
    }
    return Results.Unauthorized();
});

app.Run();