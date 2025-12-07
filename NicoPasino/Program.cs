using dotenv.net;
using Microsoft.EntityFrameworkCore;
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
                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 39)))
            //options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesConnectionString")) // SQL Server
            );

            // permitir inyección
            builder.Services.AddScoped<IMovieServicio, MovieServicio>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IGeneroServicio, GeneroServicio>();
            builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));

            // cambiar texto de validación de la vista
            builder.Services.AddRazorPages()
            .AddMvcOptions(options => {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    _ => "El campo es requerido.");
            });

            var app = builder.Build();

            // crear una base de datos a través de una migración
            /*using (var scope = app.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<moviesdbContext>();
                context.Database.Migrate();
            }*/


            app.Use(async (context, next) => {
                context.Response.Headers.Append("X-Robots-Tag", "noindex, nofollow");
                await next();
            });

            // Middleware para manejar códigos de estado: (re-ejecuta la petición internamente)
            app.UseStatusCodePagesWithReExecute("/Home/NotFound", "?statusCode={0}");

            // Middleware para Error 500
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
