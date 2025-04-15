using Comments.Server.ActionFilters;
using Comments.Server.Data;
using Comments.Server.Extensions;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Comments.Server.Services;
using Comments.Server.Services.Contracts;
using Microsoft.AspNetCore.HttpOverrides;


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

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureCors();
builder.Services.ConfigureLoggerService();


builder.Services.AddScoped<ValidationFilterAttribute>();

builder.Services.AddScoped<ICommentsService, CommentsService>();
builder.Services.AddScoped<IGenerateFileNameService, GenerateFileNameService>();
builder.Services.AddScoped<IImageFileService, ImageFileService>();
builder.Services.AddScoped<ITextFileService, TextFileService>();
builder.Services.AddScoped<IGenerateCaptchaService, GenerateCaptchaService>();

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

    //await dbContext.Database.EnsureDeletedAsync();
    //await dbContext.Database.EnsureCreatedAsync();

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

app.UseSession();

app.UseStaticFiles();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.Run();
