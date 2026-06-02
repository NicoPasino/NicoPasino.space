using dotenv.net;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using NicoPasino.Core.DTO.Notas;
using NicoPasino.Core.DTO.Ventas;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Mapper;
using NicoPasino.Core.Modelos.Notas;
using NicoPasino.Core.Modelos.Ventas;
using NicoPasino.Infra.Data;
using NicoPasino.Infra.Repositorio;
using NicoPasino.Servicios.Servicios.Movies;
using NicoPasino.Servicios.Servicios.Notas;
using NicoPasino.Servicios.Servicios.Ventas;
using System.Threading.RateLimiting;

namespace NicoPasino
{
    public class Program
    {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);

            // Forzar a que escuche en todas las interfaces en el puerto 5000
            //builder.WebHost.UseUrls("https://0.0.0.0:5000"); // TODO: DESACTIVARRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRRR

            DotEnv.Load(); // leer .env

            // definir reglas CORS
            var misReglasCORS = "_misReglasCORS";
            var acceptedOrigins = Environment.GetEnvironmentVariable("ACCEPTED_ORIGINS")?.Split(',') ?? [""];
            builder.Services.AddCors(options => {
                options.AddPolicy(name: misReglasCORS, policy => { policy.WithOrigins(acceptedOrigins).AllowAnyHeader().AllowAnyMethod(); });
            });

            builder.Services.AddControllersWithViews();

            // Configuraciones para Mapster
            MappingConfig.VentasMappings();
            MappingConfig.NotasMappings();

            // conexi�n a pel�culas
            var moviesdb = Environment.GetEnvironmentVariable("movies");
            builder.Services.AddDbContext<moviesdbContext>(options =>
                options.UseMySql(moviesdb, new MySqlServerVersion(new Version(8, 0, 39)))
            );

            // conexi�n a ventas
            var ventasdb = Environment.GetEnvironmentVariable("ventas");
            builder.Services.AddDbContext<ventasdbContext>(options =>
                options.UseMySql(ventasdb, new MySqlServerVersion(new Version(8, 0, 39)))
            );

            // conexi�n a notas
            var notasdb = Environment.GetEnvironmentVariable("notas");
            builder.Services.AddDbContext<notasdbContext>(options =>
                options.UseMySql(notasdb, new MySqlServerVersion(new Version(8, 0, 39))) // version de aws?
            );


            // permitir inyecci�n (Repositorio => conexi�n con dbContext)
            builder.Services.AddScoped<IUnitOfWorkMovie, UnitOfWorkMovie>();
            builder.Services.AddScoped(typeof(IRepositorioGenerico<>), typeof(RepositorioGenericoMovies<>));
            builder.Services.AddScoped(typeof(IRepositorioGenericoVentas<>), typeof(RepositorioGenericoVentas<>));
            builder.Services.AddScoped(typeof(IRepositorioGenerico<Cards>), typeof(RepositorioGenericoNotes<Cards>));

            // Servicios
            builder.Services.AddScoped<IMovieServicio, MovieServicio>();
            builder.Services.AddScoped<IGeneroServicio, GeneroServicio>();

            builder.Services.AddScoped<IServicioGenerico<Producto, ProductoDto>, ProductoServicio>();
            builder.Services.AddScoped<IServicioGenerico<Venta, VentaDto>, VentaServicio>();
            builder.Services.AddScoped<IServicioGenerico<Cliente, ClienteDto>, ClienteServicio>();
            builder.Services.AddScoped<IServicioGenerico<Categoria, CategoriaDto>, CategoriaServicio>();

            builder.Services.AddScoped<IServicioGenerico<Cards, CardsDto>, NotasServicio>();

            // cambiar texto de validaci�n de la vista
            builder.Services.AddRazorPages()
            .AddMvcOptions(options => {
                options.ModelBindingMessageProvider.SetValueMustNotBeNullAccessor(
                    _ => "El campo es requerido.");
            });

            builder.Services.AddHsts(options => {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
            });

            builder.Services.AddRateLimiter(options => {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddPolicy("general", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new FixedWindowRateLimiterOptions {
                            PermitLimit = 100,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }
                    )
                );
            });

            var app = builder.Build();

            // crear una base de datos a trav�s de una migraci�n
            /*using (var scope = app.Services.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<moviesdbContext>();
                context.Database.Migrate();
            }*/


            app.Use(async (context, next) => {
                var h = context.Response.Headers;
                h["X-Content-Type-Options"] = "nosniff";
                h["X-Frame-Options"] = "DENY";
                h["Referrer-Policy"] = "strict-origin-when-cross-origin";
                h["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
                h["Content-Security-Policy"] = "default-src 'self'; script-src 'self' 'unsafe-inline' https://fonts.googleapis.com; style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; img-src 'self' data: https:; font-src 'self' https://fonts.gstatic.com data:; connect-src 'self' https://nicopasino.space https://*.nicopasino.space http://localhost:* http://127.0.0.1:*; frame-ancestors 'none'; base-uri 'self'; form-action 'self'; object-src 'none'";
                h["X-Robots-Tag"] = "noindex, nofollow";
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
            app.UseRateLimiter();

            // Aplicar la política de CORS
            app.UseCors(misReglasCORS);

            app.UseAuthorization();


            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}


//options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesConnectionString")) // SQL Server