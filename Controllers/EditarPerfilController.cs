﻿using Microsoft.AspNetCore.Mvc;
using RedSocialNetCore.Extensions;
using RedSocialNetCore.Helpers;
using RedSocialNetCore.Models;
using RedSocialNetCore.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RedSocialNetCore.Controllers
{
    public class EditarPerfilController : Controller
    {
        private RepositoryEditarPerfil repo;
        private HelperPathProvider helper;

        public EditarPerfilController(RepositoryEditarPerfil repo, HelperPathProvider helper)
        {
            this.repo = repo;
            this.helper = helper;

        }



        public IActionResult EditarPerfil()
        {

            var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

            HttpContext.Session.Remove("OtherUser");

            if (currentUser == null)
            {
                return RedirectToAction("Login", "Inicio");
            }

            Usuario user = repo.GetUser(currentUser.Username);


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EditarPerfil(Usuario usuario, IFormFile FotoPerfil)
        {

            var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

            string nuevaFotoPerfil = null;

            
            if (FotoPerfil != null && FotoPerfil.Length > 0)
            {
                
                string fileName = "img" + currentUser.Username + ".jpeg";

                string path = this.helper.MapPath(fileName, Folders.Usuarios);
                using (Stream stream = new FileStream(path, FileMode.Create))
                {
                    await FotoPerfil.CopyToAsync(stream);
                }

                nuevaFotoPerfil = fileName;
            }

            
            Usuario perfilActualizado = new Usuario
            {
                Username = currentUser.Username,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Descripcion = usuario.Descripcion,
                Telefono = usuario.Telefono,
                // Si el usuario no cambia la foto se queda la actual
                FotoPerfil = !string.IsNullOrEmpty(nuevaFotoPerfil) ? nuevaFotoPerfil : currentUser.FotoPerfil
            };

            await this.repo.UpdateUser(perfilActualizado);


            var username = currentUser.Username;

            HttpContext.Session.Remove("CurrentUser");
            Usuario user = repo.GetUser(username);
            HttpContext.Session.SetObject("CurrentUser", user);

            return RedirectToAction("Index", "Home");
        }

    }
}
