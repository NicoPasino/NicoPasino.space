using Microsoft.EntityFrameworkCore;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Repositorio.Movies;
using NicoPasino.Infra.Data;
using NicoPasino.Servicios.Servicios.Movies;

namespace NicoPasino
{
    public class Program
    {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddControllersWithViews();

            // conexion
            builder.Services.AddDbContext<moviesdbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("MoviesConnectionString"))
            );

            // permitir inyeccion
            builder.Services.AddScoped<IMovieServicio, MovieServicio>();
            builder.Services.AddScoped<IMovieRepo, MoviesRepo>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment()) {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
