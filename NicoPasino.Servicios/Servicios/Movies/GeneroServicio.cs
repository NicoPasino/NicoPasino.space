using NicoPasino.Core.DTO.Movies;
using NicoPasino.Core.Interfaces;
using NicoPasino.Core.Modelos.MoviesMySql;

namespace NicoPasino.Servicios.Servicios.Movies
{
    public class GeneroServicio : IGeneroServicio
    {
        private readonly IUnitOfWork _uow;
        private readonly IRepositorioGenerico<Genre> _repo;

        public GeneroServicio(IUnitOfWork uow) {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _repo = _uow.Repositorio<Genre>() ?? throw new ArgumentNullException(nameof(_repo));
        }

        public async Task<IEnumerable<Genre>> GetAll() {
            try {
                var generos = await _repo.GetAll();
                return generos ?? Enumerable.Empty<Genre>();
            } catch (Exception) {
                return Enumerable.Empty<Genre>();
            }
        }

        public async Task<Genre> GetById(int id) {
            if (id <= 0) return null;
            try {
                var genero = await _repo.GetAsync(filtro: g => g.Id == id);
                return genero;
            } catch (Exception) {
                return null;
            }
        }

        public async Task<bool> Create(GeneroDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var nombre = dto.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre)) return false;
            if (nombre.Length > 200) nombre = nombre.Substring(0, 200);

            try {
                var existentes = await _repo.ListarAsync(filtro: g => g.Name == nombre);
                if (existentes != null && existentes.Any()) return false;

                var genre = new Genre { Name = nombre };
                await _repo.Add(genre);
                await _uow.SaveChangesAsync();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public async Task<bool> Update(GeneroDto dto) {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Id <= 0) return false;

            var nombre = dto.Nombre?.Trim();
            if (string.IsNullOrEmpty(nombre)) return false;
            if (nombre.Length > 200) nombre = nombre.Substring(0, 200);

            try {
                var existing = await _repo.GetAsync(filtro: g => g.Id == dto.Id);
                if (existing == null) return false;

                // evitar duplicados con otro id
                var dup = await _repo.ListarAsync(filtro: g => g.Name == nombre && g.Id != dto.Id);
                if (dup != null && dup.Any()) return false;

                existing.Name = nombre;
                await _repo.Update(existing);
                await _uow.SaveChangesAsync();
                return true;
            } catch (Exception) {
                return false;
            }
        }

        public async Task<bool> Delete(int id) {
            if (id <= 0) return false;
            try {
                var existing = await _repo.GetAsync(filtro: g => g.Id == id, incluir: "Moviegenres");
                if (existing == null) return false;

                // No eliminar si tiene relaciones con películas
                if (existing.Moviegenres != null && existing.Moviegenres.Any()) return false;

                await _repo.Delete(existing);
                await _uow.SaveChangesAsync();
                return true;
            } catch (Exception) {
                return false;
            }
        }
    }
}