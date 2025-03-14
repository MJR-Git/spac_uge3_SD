using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using CerealApi.CsvParser;
using CerealApi.Models;
using System.Linq.Dynamic.Core;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Cereals") ?? "Data Source=Cereals.db";

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSqlite<CerealDb>(connectionString);
builder.Services.AddSwaggerGen(c =>
{
     c.SwaggerDoc("v1", new OpenApiInfo {
       Title = "Cereal API", 
       Description = "Cereal database", 
       Version = "v1" });
});

builder.Services.AddAuthorizationBuilder()
  .AddPolicy("admin_greetings", policy =>
        policy
            .RequireRole("admin")
            .RequireClaim("scope", "greetings_api"));

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
});;
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
});
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
});
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
});
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
});
app.MapPost("/cereal/{csvPath}", async (CerealDb db, ICsvParser csvParser, string csvPath) =>
{
    try
    {
        await csvParser.ParseCsvAsync(db, csvPath);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.Problem("An error occurred while parsing the CSV.");
    };
});
app.Run();