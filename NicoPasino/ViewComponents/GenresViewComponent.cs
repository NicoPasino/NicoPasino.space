using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.ViewComponents
{
    public class GenresViewComponent : ViewComponent
    {
        private readonly IGeneroServicio _servicio;

        public GenresViewComponent(IGeneroServicio servicio) {
            _servicio = servicio;
        }

        public async Task<IViewComponentResult> InvokeAsync() {
            IEnumerable<Genre> generos = await _servicio.GetAll();
            return View(generos);
        }
    }
}