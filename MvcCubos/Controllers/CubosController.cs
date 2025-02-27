using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using MvcCubos.Helpers;
using MvcCubos.Models;
using MvcCubos.Repositories;
using MvcNetCoreUtilidades.Helpers;

namespace MvcCubos.Controllers
{
    public class CubosController : Controller
    {
        private RepositoryCubos _repository;
        private HelperPathProvider helperPath;
        private IMemoryCache memoryCache;

        public CubosController(RepositoryCubos repository, HelperPathProvider helperPath, IMemoryCache memoryCache)
        {
            this._repository = repository;
            this.helperPath = helperPath;
            this.memoryCache = memoryCache;
        }

        public async Task<IActionResult> Index(int? idFavorito)
        {

            if (idFavorito != null)
            {
                List<Cubo> favoritos;

                if (!this.memoryCache.TryGetValue("favoritos", out favoritos))
                {
                    favoritos = new List<Cubo>();
                }
                else
                {
                    favoritos = this.memoryCache.Get<List<Cubo>>("favoritos");
                }

                Cubo cubo = await this._repository.FindCuboAsync(idFavorito.Value);
                favoritos.Add(cubo);
                this.memoryCache.Set("favoritos", favoritos);
            }
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
        public async Task<IActionResult> Create(int idCubo, string nombre, string modelo, string marca, IFormFile imagen, int precio)
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

        public async Task<IActionResult> Delete(int id)
        {
            await this._repository.DeleteCuboAsync(id);
            return RedirectToAction("Index");
        }

        public IActionResult Favoritos(int? idEliminar)
        {
            if (idEliminar.HasValue) { 
                List<Cubo> favoritos = this.memoryCache.Get<List<Cubo>>("favoritos");

                Cubo cubo = favoritos.Find(c => c.IdCubo == idEliminar.Value);
                favoritos.Remove(cubo);

                if (favoritos.Count == 0) {
                    this.memoryCache.Remove("favoritos");
                } else
                {
                    this.memoryCache.Set("favoritos", favoritos);
                }
            }
            return View();
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Compra>>("Carrito") ?? new List<Compra>();

            var cubo = await this._repository.FindCuboAsync(id);
            if (cubo == null)
            {
                TempData["ErrorMessage"] = "Cubo no encontrado.";
                return RedirectToAction("Index");
            }

            var compraExistente = carrito.FirstOrDefault(c => c.IdCubo == id);
            if (compraExistente != null)
            {
                compraExistente.Cantidad++;
            }
            else
            {
                carrito.Add(new Compra
                {
                    IdCubo = id,
                    Cantidad = 1,
                    Precio = cubo.Precio,
                    FechaPedido = DateTime.Now
                });
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            TempData["SuccessMessage"] = "Cubo añadido al carrito.";

            return RedirectToAction("Index");
        }
        public IActionResult Carrito()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Compra>>("Carrito") ?? new List<Compra>();
            return View(carrito);
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int id)
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<Compra>>("Carrito") ?? new List<Compra>();

            var compra = carrito.FirstOrDefault(c => c.IdCubo == id);
            if (compra != null)
            {
                carrito.Remove(compra);
            }

            HttpContext.Session.SetObjectAsJson("Carrito", carrito);

            TempData["SuccessMessage"] = "Producto eliminado del carrito.";

            return RedirectToAction("Carrito");
        }

    }
}

