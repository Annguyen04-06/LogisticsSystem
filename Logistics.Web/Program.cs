using Logistics.Web.Components;
using Logistics.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<AuthStateService>();
builder.Services.AddScoped<TokenStorageService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ProductApiService>();
builder.Services.AddScoped<CategoryApiService>();
builder.Services.AddScoped<CartApiService>();
builder.Services.AddScoped<OrderApiService>();
builder.Services.AddScoped<SellerDashboardApiService>();
builder.Services.AddScoped<SellerProductApiService>();
builder.Services.AddScoped<SellerOrderApiService>();
builder.Services.AddScoped<DeliveryApiService>();
builder.Services.AddScoped<AdminDashboardApiService>();
builder.Services.AddScoped<CouponApiService>();
builder.Services.AddScoped<UserApiService>();
builder.Services.AddScoped<TicketApiService>();
builder.Services.AddScoped<PaymentApiService>();
builder.Services.AddScoped<ReportApiService>();
builder.Services.AddScoped<ProfileApiService>();
builder.Services.AddScoped<UploadApiService>();
builder.Services.AddHttpClient<ApiClientService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5203/");
});

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
