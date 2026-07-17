using GestaoGastos.Api.Data;
using GestaoGastos.Api.Middleware;
using GestaoGastos.Api.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string CorsPolicyFrontend = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(CorsPolicyFrontend, policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173")  // vite padrao
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registro de Services na injeção de dependência.
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ITransacaoService, TransacaoService>();
builder.Services.AddScoped<ITotalService, TotalService>();



var app = builder.Build();

/*
 Aplica as migrations pendentes automaticamente ao iniciar a aplicação e
 mantem o banco de dados atualizado
*/
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}


app.UseSwagger();
app.UseSwaggerUI();


// Middleware global de exceções

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseCors(CorsPolicyFrontend);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();