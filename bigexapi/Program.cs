using bigexapi.Storage;
using bigexapi.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = AuthOptions.ISSUER,

        ValidateAudience = true,
        ValidAudience = AuthOptions.AUDIENCE,

        ValidateLifetime = true,

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(
            builder.Configuration["JwtSettings:Key"])
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();

            Console.WriteLine($"METHOD: {context.Request.Method}");
            Console.WriteLine($"AUTH: {authHeader}");

            if (string.IsNullOrWhiteSpace(authHeader))
            {
                context.NoResult();
            }

            return Task.CompletedTask;
        }
    };
});*/

builder.Services.AddCors(opt =>
opt.AddPolicy("CorsPolicy", policy =>
{
    policy.AllowAnyOrigin()
      .AllowAnyMethod()
      .AllowAnyHeader();
}));

string connectionString = builder.Configuration.GetConnectionString("MySqlConnectionString");
builder.Services.AddSingleton<IStorage>(new MySqlStorage(connectionString));

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//app.UseRouting();

app.UseCors("CorsPolicy");
//app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();