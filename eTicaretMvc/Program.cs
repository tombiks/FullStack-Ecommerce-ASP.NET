using eTicaretMvc.Helpers;
using AppBusiness.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient("data-api", client =>
{
    client.BaseAddress = new Uri("https://localhost:7201");
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => 
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true, //token üreten olsun
        ValidIssuer = "eTicaretMVC", //kim olsun
        ValidateAudience = true, //token kullanan olsun
        ValidAudience = "MVC", //kim kullansın
        ValidateLifetime = true, // token süresini kontrol et
        ValidateIssuerSigningKey = true, //jwtnin son kısmının (signature) kontrol edilip edilmediği kısım

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret configuration is missing"))),

        RoleClaimType = System.Security.Claims.ClaimTypes.Role
    };

    
    options.Events = new JwtBearerEvents
    {

        //validasyon yapıyoruz - her istek geldiğinde
        OnMessageReceived = context =>
        {
            var accesToken = context.Request.Cookies["access_token"];
            if (!string.IsNullOrEmpty(accesToken))
            {
                context.Token = accesToken;
            }

            return Task.CompletedTask;
        },

        //loginpatch
        OnChallenge = async context =>
        {
            //varsayılan zorlama durumunu engelle
            context.HandleResponse();

            context.Response.Redirect("/Auth/Login");

            await Task.CompletedTask;
        }


    };

    //claimler içerisine kendi belirlediğimiz bir claim ismi eklenirse, program bunu soap mimarisine uygun bir claim ismine çevirme davranışı gösterebilir. Burada bunu engellemış oluruz.
    options.MapInboundClaims = false; 

});

//dependency container
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ClaimHelper>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<CategoryService>();
builder.Services.AddScoped<CartService>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<ContactService>();
builder.Services.AddScoped<CommentService>();


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
