using Comments.Server.Data;
using Comments.Server.Data.Entities;
using Comments.Server.Extensions;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureCors();
builder.Services.AddAuthentication();
builder.Services.ConfigureIdentity();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var score = app.Services.CreateAsyncScope())
{
    var sp = score.ServiceProvider;
    var dbContext = sp.GetRequiredService<CommentsDbContext>();
    
    await dbContext.Database.EnsureDeletedAsync();
    await dbContext.Database.EnsureCreatedAsync();

    var userManager = sp.GetRequiredService<UserManager<User>>();
    var signInManager = sp.GetRequiredService<SignInManager<User>>();

    await DbInitializer.InitializeAsync(dbContext, userManager, signInManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Theme = ScalarTheme.Kepler;
    });
}

app.UseHttpsRedirection();

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
