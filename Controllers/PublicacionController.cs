using Microsoft.AspNetCore.Mvc;
using RedSocialNetCore.Extensions;
using RedSocialNetCore.Helpers;
using RedSocialNetCore.Models;
using RedSocialNetCore.Repositories;

namespace RedSocialNetCore.Controllers
{
    public class PublicacionController : Controller
    {

        private RepositoryPublicacion repo;
        private HelperPathProvider helper;

        public PublicacionController(RepositoryPublicacion repo, HelperPathProvider helper)
        {
            this.repo = repo;
            this.helper = helper;

        }


        public IActionResult Publicar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Publicar(Publicacion publicacion, IFormFile imagen)
        {
            var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

            
            Publicacion nuevaPublicacion = new Publicacion
            {
                Texto = string.IsNullOrEmpty(publicacion.Texto) ? "" : publicacion.Texto,
                FechaPublicacion = DateTime.Now, 
                Username = currentUser.Username
            };

            if (imagen != null && imagen.Length > 0)
            {
                int nextId = repo.GetNextPublicacionId();

                string fileName = "imagen" + nextId.ToString() + ".jpeg";

                string path = this.helper.MapPath(fileName, Folders.Publicaciones);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await imagen.CopyToAsync(stream);
                }

                
                nuevaPublicacion.Imagen = fileName;
                nuevaPublicacion.TipoPublicacion = 2; 
            }
            else
            {
                nuevaPublicacion.Imagen = ""; 
                nuevaPublicacion.TipoPublicacion = 1; 
            }

            nuevaPublicacion.FotoPerfil = currentUser.FotoPerfil;

            await this.repo.AddPublicacion(nuevaPublicacion);

            return RedirectToAction("Index", "Home");
        }




    }
}
