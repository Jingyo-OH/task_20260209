using EmployeeManagement.Api.Commands.ImportEmployees;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Infrastructure;
using EmployeeManagement.Api.Middlewares;
using EmployeeManagement.Api.Queries.GetEmployeeByName;
using EmployeeManagement.Api.Queries.GetEmployees;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

#region[inMemoryDB Setting]
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("EmployeeDb"));
#endregion

builder.Services.AddControllers();

#region[DI]
builder.Services.AddScoped<IEmployeeCsvReader, EmployeeCsvReader>();
#endregion

#region[Handlers Setting]
builder.Services.AddScoped<EmployeePagingQueryHandler>();
builder.Services.AddScoped<GetEmployeeByNameQueryHandler>();
builder.Services.AddScoped<ImportEmployeeCommandHandler>();
#endregion

#region[Serilog Setting]
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateBootstrapLogger();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});
#endregion

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(); // Swagger 미들웨어 활성화
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "My API V1");
    });
}

#region[Middleware Setting]
app.UseMiddleware<ExceptionHandlingMiddleware>();
#endregion

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
