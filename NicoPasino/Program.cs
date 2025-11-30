using dotenv.net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NicoPasino.Core.Interfaces;
using NicoPasino.Infra.Data;
using NicoPasino.Infra.Repositorio;
using NicoPasino.Servicios.Servicios.Movies;

namespace NicoPasino
{
    public class Program
    {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            // Conexión a db
            DotEnv.Load(); // leer .env
            var connectionString = Environment.GetEnvironmentVariable("DefaultConnection");
            builder.Services.AddDbContext<moviesdbContext>(options =>
                options.UseMySql( connectionString, new MySqlServerVersion(new Version(8, 0, 39)))
                //options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesConnectionString")) // SQL Server
            );

            // permitir inyeccion
            builder.Services.AddScoped<IMovieServicio, MovieServicio>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));

            var app = builder.Build();

            // crear una base de datos a travez de una migracion
            /*using (var scope = app.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<moviesdbContext>();
                context.Database.Migrate();
            }*/


            app.Use(async (context, next) => {
                context.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow");
                await next();
            });

            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();


            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Movies}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
