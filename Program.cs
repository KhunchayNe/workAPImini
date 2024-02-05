using System.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

using var connection = new MySqlConnection(builder.Configuration.GetConnectionString("Default"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.MapPost("/login", async (login body) =>
{
    try
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        using var cmd = new MySqlCommand($@"SELECT userid,fname,lname,username,email,studentid 
        from user WHERE (lower(username) = '{body.Username}' or lower(email) = '{body.Username}') and password = '{body.Password}';", connection);
        // cmd.CommandText = "";
        using var reader = cmd.ExecuteReader();
        RespUserInfo res = new();
        while (reader.Read())
        {
            res.Userid = reader.GetInt32(0);
            res.FName = reader.GetString(1);
            res.LName = reader.GetString(2);
            res.Username = reader.GetString(3);
            res.Email = reader.GetString(4);
            res.StudentId = reader.GetInt32(5);
        }
        await reader.CloseAsync();

        return res;
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex.Message);
        return new();
    }
    finally { await connection.CloseAsync(); }


})
.WithName("login")
.WithOpenApi();

app.MapGet("/GetUser/{Id}", async (int Id) =>
{
    try
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        using var cmd = new MySqlCommand($"SELECT userid,fname,lname,username,email,studentid from user WHERE userid = {Id}", connection);
        // cmd.CommandText = "";
        using var reader = cmd.ExecuteReader();
        RespUserInfo res = new();
        while (reader.Read())
        {
            res.Userid = reader.GetInt32(0);
            res.FName = reader.GetString(1);
            res.LName = reader.GetString(2);
            res.Username = reader.GetString(3);
            res.Email = reader.GetString(4);
            res.StudentId = reader.GetInt32(5);
        }
        await reader.CloseAsync();

        return res;
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex.Message);
        return new();
    }
    finally { await connection.CloseAsync(); }


})
.WithName("GetUser")
.WithOpenApi();


app.MapPost("/AddUser", async (AddUser body) =>
{
    try
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        using var cmd = new MySqlCommand($"INSERT INTO `user` (`userid`, `fname`, `lname`, `username`, `email`, `password`, `studentid`) VALUES (NULL, '{body.FName}', '{body.LName}', '{body.Username}', '{body.Email}', '{body.Password}', '{body.StudentId}')", connection);
        // cmd.CommandText = "";
        var res = await cmd.ExecuteNonQueryAsync();
        return res > 0 ? TypedResults.StatusCode(201) : TypedResults.StatusCode(400) ;
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex.Message);
        return TypedResults.StatusCode(500);
    }
    finally { await connection.CloseAsync(); }


})
.WithName("AddUser")
.WithOpenApi();

app.MapDelete("/DeleteUser/{userId}", async (int userId) =>
{
    try
    {
        if (connection.State != ConnectionState.Open)
            await connection.OpenAsync();
        using var cmd = new MySqlCommand($"DELETE FROM user WHERE userid = {userId}", connection);
        // cmd.CommandText = "";
        var res = await cmd.ExecuteNonQueryAsync();
        return res > 0 ? TypedResults.StatusCode(200) : TypedResults.StatusCode(400) ;
    }
    catch (System.Exception ex)
    {
        Console.WriteLine(ex.Message);
        return TypedResults.StatusCode(500);
    }
    finally { await connection.CloseAsync(); }


})
.WithName("DeleteUser")
.WithOpenApi();
app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

