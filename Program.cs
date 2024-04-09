
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Https;



////builder.Services.AddAuthentication
////                        (CertificateAuthenticationDefaults.
////                         AuthenticationScheme)
////        .AddCertificate(options =>
////{
////    options.RevocationMode = X509RevocationMode.NoCheck;
////    options.AllowedCertificateTypes = CertificateTypes.All;
////    options.Events = new CertificateAuthenticationEvents
////    {
////        OnCertificateValidated = context =>
////        {
////            var validationService =
////                context.HttpContext.RequestServices.
////            GetService<CertificateValidationService>();

////            if (validationService != null &&
////                validationService.ValidateCertifcate
////                (context.ClientCertificate))
////            {
////                Console.WriteLine("Success");
////                context.Success();
////            }
////            else
////            {
////                Console.WriteLine("invalid cert");
////                context.Fail("invalid cert");
////            }
////            return Task.CompletedTask;
////        }
////    };
////});




//// Add services to the container.
//builder.Services.AddRazorPages(options =>
//{
//    options.Conventions.AddPageRoute("/login/login","");
//});
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//        builder =>
//        {

//            //you can configure your custom policy
//            builder.AllowAnyOrigin()
//                                .AllowAnyHeader()
//                                .AllowAnyMethod();
//        });
//});

//builder.Services.AddSession(options =>
//{
//    options.Cookie.Name = ".IBCSCoreWebPortal.Session";
//    options.IdleTimeout = TimeSpan.FromSeconds(600);
//    //options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//});

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}


//app.UseCors();
////app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthorization();
//app.UseSession();
//app.MapRazorPages();
//app.MapFallbackToPage("/login");
//app.Run();


var builder = WebApplication.CreateBuilder(args);

//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.ConfigureHttpsDefaults(options =>
//            options.ClientCertificateMode = ClientCertificateMode.RequireCertificate);
//});
//Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(webBuilder =>
//{
//    webBuilder.ConfigureKestrel(options =>
//    {
//        options.ConfigureHttpsDefaults(o =>
//        {
//            o.ClientCertificateMode = ClientCertificateMode.AllowCertificate;
//        });
//    });
//});
// Add services to the container.
builder.Services.AddRazorPages(options =>
{
    options.Conventions.AddPageRoute("/login/login", "");
});
//builder.Services.AddCors(options =>
//{
//    options.AddDefaultPolicy(
//        builder =>
//        {

//            //you can configure your custom policy
//            builder.AllowAnyOrigin()
//                                .AllowAnyHeader()
//                                .AllowAnyMethod();
//        });
//});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
	options.MinimumSameSitePolicy = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
	options.HttpOnly = Microsoft.AspNetCore.CookiePolicy.HttpOnlyPolicy.Always;
	options.Secure = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.Cookie.SameSite = SameSiteMode.Strict;
		options.Cookie.HttpOnly = true;
		options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
	});
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".IBCSCoreWebPortal.Session";
    options.IdleTimeout = TimeSpan.FromSeconds(600);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

});

//builder.Services.AddAntiforgery(options =>
//{
//	options.Cookie.Name = ".IBCSCoreWebPortal.Session";
//	options.Cookie.HttpOnly = true;
//	options.Cookie.IsEssential = true;
//	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//});

var app = builder.Build();
app.UseExceptionHandler("/IBCSHome");
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	//app.UseExceptionHandler("/IBCSHome");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();
app.UseSession();
app.MapRazorPages();
app.MapFallbackToPage("/login");
app.Run();
