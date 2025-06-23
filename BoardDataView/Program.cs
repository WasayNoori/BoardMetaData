using Microsoft.EntityFrameworkCore;
using BoardDataView.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllers(); // ← This tells ASP.NET Core to look for API controllers

// Add Entity Framework with better error handling
builder.Services.AddDbContext<SPAutomationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SPAutomationDB"));
    options.EnableSensitiveDataLogging(builder.Environment.IsDevelopment());
    options.EnableDetailedErrors(builder.Environment.IsDevelopment());
});

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();
app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("X-Frame-Options");
    context.Response.Headers["Content-Security-Policy"] = "frame-ancestors 'self' https://*.monday.com";
    await next();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();
app.MapControllers(); // ← This tells ASP.NET Core to actually route requests to controllers

// Test database connection on startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<SPAutomationDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        await context.Database.CanConnectAsync();
        logger.LogInformation("Database connection successful");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database connection failed. Please check your connection string and ensure the database is accessible.");
    }
}

app.Run();
