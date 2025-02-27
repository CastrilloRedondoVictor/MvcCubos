using Microsoft.AspNetCore.Mvc;
using MvcCubos.Models;
using MvcCubos.Repositories;
using MvcNetCoreUtilidades.Helpers;

namespace MvcCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos _repository;
        private HelperPathProvider helperPath;

        public CubosController(RepositoryCubos repository, HelperPathProvider helperPath)
        {
            this._repository = repository;
            this.helperPath = helperPath;
        }

        public async Task<IActionResult> Index()
        {
            List<Cubo> cubos = await this._repository.GetCubosAsync();
            return View(cubos);
        }

        public async Task<IActionResult> Details(int id)
        {
            Cubo cubo = await this._repository.FindCuboAsync(id);
            return View(cubo);
        }

        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(int idCubo, string nombre, string modelo, string marca, IFormFile imagen, int precio )
        {
            string fileName = imagen.FileName;
            string path = this.helperPath.MapPath(fileName, Folders.Images);

            using (Stream stream = new FileStream(path, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            Cubo cubo = new Cubo()
            {
                Nombre = nombre,
                Modelo = modelo,
                Marca = marca,
                Imagen = fileName,
                Precio = precio
            };

            await this._repository.CreateCuboAsync(cubo);
            return RedirectToAction("Index");
        }

        
        public async Task<IActionResult> Edit(int id)
        {
            
            Cubo cubo = await this._repository.FindCuboAsync(id);            
            return View(cubo);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int idCubo, string nombre, string modelo, string marca, IFormFile nuevaImagen, int precio)
        {
            Cubo cubo = await this._repository.FindCuboAsync(idCubo);
            if (cubo == null)
            {
                return NotFound();
            }

            string fileName = cubo.Imagen;

            if (nuevaImagen != null && nuevaImagen.Length > 0)
            {
                fileName = nuevaImagen.FileName;
                string path = this.helperPath.MapPath(fileName, Folders.Images);

                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await nuevaImagen.CopyToAsync(stream);
                }
            }

            Cubo cuboNuevo = new Cubo()
            {
                IdCubo = idCubo,
                Nombre = nombre,
                Modelo = modelo,
                Marca = marca,
                Imagen = fileName,
                Precio = precio
            };

            await this._repository.EditCuboAsync(cuboNuevo);

            return RedirectToAction("Index");
        }
    }
}
