using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Desafio.Application.Services;
using Desafio.Domain.Repositories;
using Desafio.Domain.Services;
using Desafio.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAmazonDynamoDB>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var region = configuration.GetValue<string>("Aws:Region") ?? "us-east-1";
    return new AmazonDynamoDBClient(new AmazonDynamoDBConfig
    {
        RegionEndpoint = RegionEndpoint.GetBySystemName(region)
    });
});

builder.Services.AddSingleton<IDynamoDBContext>(sp =>
    new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => sp.GetRequiredService<IAmazonDynamoDB>())
        .Build());

builder.Services.AddScoped<IAccountRepository, DynamoDbAccountRepository>();
builder.Services.AddScoped<IAnalystRepository, DynamoDbAnalystRepository>();
builder.Services.AddScoped<PixLimitEvaluator>();
builder.Services.AddScoped<PixLimitService>();
builder.Services.AddScoped<AnalystService>();
builder.Services.AddScoped<AccountBalanceService>();

var app = builder.Build();

// Configure Swagger middleware.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
