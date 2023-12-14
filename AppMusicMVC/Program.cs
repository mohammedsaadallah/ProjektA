using Services;
using Microsoft.EntityFrameworkCore.Design;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

#region Injecting a dependency service to read MusicWebApi
if (Configuration.csAppConfig.DataSource == "WebApi")
{
    builder.Services.AddHttpClient(name: "MusicWebApi", configureClient: options =>
    {
        options.BaseAddress = Configuration.csAppConfig.WebApiBaseUri;
        options.DefaultRequestHeaders.Accept.Add(
            new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue(
                mediaType: "application/json",
                quality: 1.0));
    });
    builder.Services.AddScoped<IMusicService, csMusicServiceWaco>();
}
else
{
    builder.Services.AddScoped<IMusicService, csMusicService>();

}
#endregion

var app = builder.Build();



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

