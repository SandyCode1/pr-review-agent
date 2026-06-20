using Azure;
using Azure.AI.OpenAI;
using PRReviewAgent.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddScoped<GitHubService>();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ReviewService>();


builder.Services.AddSingleton(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();

    Console.WriteLine($"Endpoint = {config["AzureOpenAI:Endpoint"]}");

    return new AzureOpenAIClient(
        new Uri(config["AzureOpenAI:Endpoint"]!),
        new AzureKeyCredential(config["AzureOpenAI:ApiKey"]!)
    );
});

Console.WriteLine(
    builder.Configuration["AzureOpenAI:Endpoint"]);

Console.WriteLine(
    builder.Configuration["AzureOpenAI:DeploymentName"]);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
