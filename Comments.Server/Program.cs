using Comments.Server.ActionFilters;
using Comments.Server.Extensions;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.HttpOverrides;
using Repository;
using Microsoft.Extensions.Options;
using Shared.Options;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory(),
    EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"
});

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(c =>
    {
        //c.JsonSerializerOptions.MaxDepth = 64;
    });

builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});


//builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.ConfigureCors(builder.Environment);

builder.Services.ConfigureIOptions(builder.Environment);

builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();


builder.Services.AddScoped<ValidationFilterAttribute>();

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
    var dbContext = sp.GetRequiredService<RepositoryContext>();

    //await dbContext.Database.MigrateAsync();

    //await dbContext.Database.EnsureDeletedAsync();
    bool isDbCreated = await dbContext.Database.EnsureCreatedAsync();

    var options = sp.GetRequiredService<IOptions<FileStorageOptions>>();
    FileStorageOptions fileStorageOptions = options.Value;

    if (isDbCreated)
    {
        await DbInitializer.InitializeAsync(dbContext, fileStorageOptions);
    }
}

Console.WriteLine($"--- ENV_NAME: {app.Environment.EnvironmentName} ---");

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Docker")
//{
//    app.MapOpenApi();
//    app.MapScalarApiReference(options =>
//    {
//        options.Title = "Simple Comments API";
//        options.Theme = ScalarTheme.Kepler;
//    });
//}

// Just for testing in Production
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.Title = "Simple Comments API";
    options.Theme = ScalarTheme.Kepler;
});

app.UseHttpsRedirection();

app.UseSession();

app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "http://localhost:5173");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Origin", "https://comments-client.netlify.app");
        ctx.Context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
    }
});

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.All
});

app.UseCors("CorsPolicy");

app.UseAuthorization();

app.MapControllers();

app.UseWelcomePage("/welcome");

app.Run();
