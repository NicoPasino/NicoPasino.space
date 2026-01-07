using dotenv.net;
using Microsoft.EntityFrameworkCore;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Interfaces.Ventas;
using NicoPasino.Core.Mapper;
using NicoPasino.Core.Modelos.Ventas;
using NicoPasino.Infra.Data;
using NicoPasino.Infra.Repositorio;
using NicoPasino.Servicios.Servicios.Movies;
using NicoPasino.Servicios.Servicios.Ventas;

namespace NicoPasino
{
    public class Program
    {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // definir reglas CORS
            var misReglasCORS = "_misReglasCORS";
            builder.Services.AddCors(options => {
                options.AddPolicy(name: misReglasCORS,
                                  policy => {
                                      policy.WithOrigins("http://localhost:3000", "https://nicopasino.space", "https://sistema-ventas.nicopasino.space", "http://localhost:5173", "https://localhost:5173")
                                            .AllowAnyHeader()
                                            .AllowAnyMethod();
                                  });
            });

            builder.Services.AddControllersWithViews();

            DotEnv.Load(); // leer .env

            MappingConfig.VentasMappings();

            // conexión a películas
            var moviesdb = Environment.GetEnvironmentVariable("movies2");
            builder.Services.AddDbContext<moviesdbContext>(options =>
                options.UseMySql(moviesdb, new MySqlServerVersion(new Version(8, 0, 39)))
            );

            // conexión a ventas
            var ventasdb = Environment.GetEnvironmentVariable("ventas2");
            builder.Services.AddDbContext<ventasdbContext>(options =>
                options.UseMySql(ventasdb, new MySqlServerVersion(new Version(8, 0, 39)))
            );


            // permitir inyección (Repositorio => conexión con dbContext)
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenerico<>));
            builder.Services.AddScoped(typeof(IRepositorioGenericoVentas<>), typeof(RepositorioGenericoVentas<>));


            // Servicios
            builder.Services.AddScoped<IMovieServicio, MovieServicio>();
            builder.Services.AddScoped<IGeneroServicio, GeneroServicio>();

            builder.Services.AddScoped<IServicioGenerico<Producto, ProductoDto>, ProductoServicio>();
            builder.Services.AddScoped<IServicioGenerico<Venta, VentaDto>, VentaServicio>();
            builder.Services.AddScoped<IServicioGenerico<Cliente, ClienteDto>, ClienteServicio>();

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

            // Aplicar la política de CORS
            app.UseCors(misReglasCORS);

            app.UseAuthorization();


            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Ventas}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}


//options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesConnectionString")) // SQL Server