using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Azure;
using ModernRecrut.Documents.API.Helpers;
using ModernRecrut.Documents.API.Interfaces;
using ModernRecrut.Documents.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddAzureClients
    (configure =>
    {
        configure.AddBlobServiceClient(builder.Configuration.GetConnectionString("StorageConnectionString"));

    });

builder.Services.AddScoped<IGenererNom, GenererNom>();
builder.Services.AddScoped<IStorageServiceHelper, StorageServiceHelper>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
