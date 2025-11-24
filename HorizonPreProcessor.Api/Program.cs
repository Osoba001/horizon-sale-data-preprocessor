using HorizonPreProcessor.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<ISalesDataTransformer, SalesDataTransformer>();
builder.Services.AddScoped<IAddressFormatter, AddressFormatter>();
builder.Services.AddScoped<IDataParser, DataParser>();
builder.Services.AddScoped<IValidator, Validator>();
var app = builder.Build();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
