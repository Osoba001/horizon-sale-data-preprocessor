using HorizonPreProcessor.Api.Services;
using HorizonPreProcessor.Api;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddScoped<ISalesDataTransformer, SalesDataTransformer>();
        builder.Services.AddScoped<IAddressFormatter, AddressFormatter>();
        builder.Services.AddScoped<IDataParser, DataParser>();
        builder.Services.AddScoped<IValidator, Validator>();

        var app = builder.Build();

        app.MapSaleDataTransformApis();

        app.Run();
    }
}