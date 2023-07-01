using MoviesApp.Interfaces;
using MoviesApp.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.File("logs/movies.txt"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISearchMovieService, SearchMovieService>();
builder.Services.AddScoped<ISearchSimilarMoviesService, SearchSimilarMoviesService>();

var configuration = builder.Configuration;
var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Películas");
    options.RoutePrefix = string.Empty;
});


// Configure the HTTP request pipeline.
app.MapControllers();
app.UseHttpsRedirection();

app.Run();
