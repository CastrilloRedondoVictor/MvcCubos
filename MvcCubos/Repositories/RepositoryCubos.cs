using Microsoft.EntityFrameworkCore;
using MvcCubos.Data;
using MvcCubos.Models;

namespace MvcCubos.Repositories
{
    public class RepositoryCubos
    {
        private CubosContext context;

        public RepositoryCubos(CubosContext context)
        {
            this.context = context;
        }

        public async Task<List<Cubo>> GetCubosAsync()
        {
            var consulta = from datos in this.context.Cubos
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<Cubo> FindCuboAsync(int idCubo)
        {
            var consulta = from datos in this.context.Cubos
                           where datos.IdCubo == idCubo
                           select datos;

            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<int> GetMaxIdCuboAsync()
        {
            if (!this.context.Cubos.Any())
            {
                return 1;
            }
            return await this.context.Cubos.MaxAsync(c => c.IdCubo);
        }

        public async Task CreateCuboAsync(Cubo cubo)
        {
            int newId = await GetMaxIdCuboAsync() + 1;
            cubo.IdCubo = newId;

            this.context.Cubos.Add(cubo);
            await this.context.SaveChangesAsync();
        }

        public async Task EditCuboAsync(Cubo nuevoCubo)
        {
            var cubo = await this.context.Cubos.FindAsync(nuevoCubo.IdCubo);
            if (cubo != null)
            {
                cubo.Nombre = nuevoCubo.Nombre;
                cubo.Modelo = nuevoCubo.Modelo;
                cubo.Marca = nuevoCubo.Marca;
                cubo.Precio = nuevoCubo.Precio;

                if (!string.IsNullOrEmpty(nuevoCubo.Imagen))
                {
                    cubo.Imagen = nuevoCubo.Imagen;
                }

                await this.context.SaveChangesAsync();
            }
        }

        public async Task DeleteCuboAsync(int idCubo)
        {
            var cubo = await this.context.Cubos.FindAsync(idCubo);
            if (cubo != null)
            {
                this.context.Cubos.Remove(cubo);
                await this.context.SaveChangesAsync();
            }
        }

        public async Task<List<Cubo>> GetCubosNotSessionAsync(List<Cubo> cubosSession)
        {
            var consulta = from datos in this.context.Cubos
                           where !cubosSession.Contains(datos)
                           select datos;
            return await consulta.ToListAsync();
        }
    }
}
