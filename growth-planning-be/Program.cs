using base_dotnetcore.Extensions;
using base_dotnetcore.Middlewares;
using Microsoft.EntityFrameworkCore;
using growth_planning_be.Data;
using growth_planning_be.Extensions;
using growth_planning_be.Helper;

var builder = WebApplication.CreateBuilder(args);
var secretKey = builder.Configuration["Jwt:Key"]!;

builder.Services.AddDbContext<MyDbContext>(options => {
  options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddOpenApi();
builder.Services.AddRepositories();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddBaseServices();

builder.Services.AddCors(options => {
  options.AddDefaultPolicy(policy => {
    policy.AllowAnyHeader()
      .AllowAnyMethod()
      .AllowAnyOrigin();
  });
});

builder.Services
  .AddCustomSwagger();

growth_planning_be.Extensions.JwtAuthenticationServiceExtensions.AddCustomJwtAuthentication(builder.Services, secretKey);

builder.WebHost.UseSentry(o => {
  o.Dsn = builder.Configuration.GetValue<string>("Sentry:Dsn");
});

var app = builder.Build();

app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();
UserHelper.Init(app.Services, app.Configuration);
base_dotnetcore.Helper.UserHelper.Init(app.Services, app.Configuration);
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
