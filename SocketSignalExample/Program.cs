using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using SocketSignalExample.Hub;
using SocketSignalRExample.Hub;
using System.Text;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder =>
    {
        builder
            .WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Bu satırı eklediniz mi?
    });
});



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
 .AddJwtBearer(options =>
 {
     options.TokenValidationParameters = new TokenValidationParameters
     {
         ValidateIssuer = true,
         ValidateAudience = true,
         ValidateLifetime = true,
         ValidateIssuerSigningKey = true,
         ValidIssuer = builder.Configuration["Jwt:Issuer"],
         ValidAudience = builder.Configuration["Jwt:Issuer"],
         IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
     };
     options.Events = new JwtBearerEvents
     {
         OnMessageReceived = context =>
         {
             var accessToken = context.Request.Query["access_token"];

             // If the request is for our hub...
             var path = context.HttpContext.Request.Path;
             if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
             {
                 // Read the token out of the query string
                 context.Token = accessToken;
             }
             return Task.CompletedTask;
         }
     };
     /* options.Events = new JwtBearerEvents
      {
          OnMessageReceived = context =>
          {
              var accessToken = context.Request.Query["access_token"];
              var path = context.HttpContext.Request.Path;
              if (!string.IsNullOrEmpty(accessToken) &&
                  (path.StartsWithSegments("/chat")))
              {
                  context.Token = accessToken;
              }
              return Task.CompletedTask;
          },
      };*/
 });
builder.Services.AddScoped<IAuthService, AuthService>();
// Add services to the container.
builder.Services.AddSignalR(e => {
    e.MaximumReceiveMessageSize = 102400000;
});
builder.Services.AddControllers();
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
app.UseRouting(); // Bu middleware, UseEndpoints'den önce gelmelidir.

// UseAuthentication ve UseAuthorization varsa buraya:
app.UseAuthentication(); // Opsiyonel
app.UseAuthorization();
app.UseCors("AllowOrigin");
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<ChatHub>("/chat");
});

app.MapControllers();

app.Run();
