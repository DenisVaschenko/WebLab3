using WebLab3;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(7275, listenOptions =>
    {
        listenOptions.UseHttps("localhost.p12", "changeit");
    });
});
var openIdProps = new OpenIdProperties
{
    Authority = "https://localhost:8001",
    ClientId = "a34a25ce2d6ad721085b",
    ClientSecret = "a9f9832c832af66b7e0366b802d4ad2feedc542f",
    RedirectUri = "https://localhost:7275/auth/callback"
};
builder.Services.AddSingleton(openIdProps);
builder.Services.AddSingleton<TokenValidator>();
var app = builder.Build();


// Дозволяє обслуговувати статичні файли
app.UseStaticFiles();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
