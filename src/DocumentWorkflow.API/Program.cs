using Microsoft.EntityFrameworkCore;
using DocumentWorkflow.Infrastructure.Data;
using DocumentWorkflow.Infrastructure.Repositories;
using DocumentWorkflow.Infrastructure.Seeders;
using DocumentWorkflow.Domain.Interfaces;
using DocumentWorkflow.Application.Interfaces;
using DocumentWorkflow.Application.Services;
using DocumentWorkflow.Application.Activities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure SQLite Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=documentworkflow.db"));

// Register repositories and Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register Activity Factory (Singleton - stateless)
builder.Services.AddSingleton<IActivityFactory, ActivityFactory>();

// Register application services
builder.Services.AddScoped<IDocumentService, DocumentService>();
builder.Services.AddScoped<IWorkflowService, WorkflowService>();
builder.Services.AddScoped<IWorkflowEngineService, WorkflowEngineService>();
builder.Services.AddScoped<IApprovalService, ApprovalService>();

var app = builder.Build();

// Initialize database and seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
    await DataSeeder.SeedAsync(context);
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Serve the workflow designer HTML
app.MapGet("/", () => Results.Redirect("/workflow-designer.html"));

app.Run();
