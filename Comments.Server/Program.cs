﻿using Comments.Server.ActionFilters;
using Comments.Server.Data;
using Comments.Server.Extensions;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

builder.Services.AddDbContext<CommentsDbContext>(optionsAction =>
{
    string cs = builder.Configuration.GetConnectionString("sqlConnection")!;
    optionsAction.UseSqlServer(cs);
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureCors();
builder.Services.AddAuthentication();

builder.Services.AddScoped<ValidationFilterAttribute>();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILogger<Program>>();
app.ConfigureExceptionHandler(logger);

if (app.Environment.IsProduction())
    app.UseHsts();

using (var score = app.Services.CreateAsyncScope())
{
    var sp = score.ServiceProvider;
    var webHostEnvironment = sp.GetRequiredService<IWebHostEnvironment>();
    var dbContext = sp.GetRequiredService<CommentsDbContext>();

    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();

    await DbInitializer.InitializeAsync(dbContext, webHostEnvironment);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Simple Comments API";
        options.Theme = ScalarTheme.Kepler;
    });
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
