using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SmartX.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<SmartX.Client.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");


builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7170/") });
builder.Services.AddScoped<ApiClient>();
builder.Services.AddScoped<AuthState>();
builder.Services.AddScoped<FirebaseAuthService>();

await builder.Build().RunAsync();