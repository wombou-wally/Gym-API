using GymUI.Components;
using GymUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var baseUrl = builder.Configuration["Api:BaseUrl"];
if (string.IsNullOrWhiteSpace(baseUrl))
{
    throw new InvalidOperationException("Api:BaseUrl is missing from appsettings.json.");
}

builder.Services.AddHttpClient<GymApiClient>(client =>
{
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<AuthState>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();