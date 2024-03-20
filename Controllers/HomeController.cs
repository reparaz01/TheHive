using Microsoft.AspNetCore.Mvc;
using RedSocialNetCore.Extensions;
using RedSocialNetCore.Models;
using RedSocialNetCore.Repositories;
using System.Collections.Generic;

public class HomeController : Controller
{
    private readonly RepositoryHome repo;

    public HomeController(RepositoryHome repo)
    {
        this.repo = repo;
    }

    public IActionResult Index()
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        HttpContext.Session.Remove("OtherUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        var publicaciones = repo.GetAllPublicacionesExceptoUsuario(currentUser.Username);

        
        var likesPorPublicacion = new Dictionary<int, int>();
        foreach (var publicacion in publicaciones)
        {
            
            var likesCount = repo.GetLikes(publicacion.IdPublicacion);
            likesPorPublicacion.Add(publicacion.IdPublicacion, likesCount);
            
            publicacion.Likeado = repo.IsLiked(publicacion.IdPublicacion, currentUser.Username);
        }

        
        ViewBag.LikesPorPublicacion = likesPorPublicacion;

        return View(publicaciones);
    }

    public IActionResult Siguiendo()
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        HttpContext.Session.Remove("OtherUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        var publicaciones = repo.GetAllPublicacionesSeguidosExceptoUsuario(currentUser.Username);

        
        var likesPorPublicacion = new Dictionary<int, int>();
        foreach (var publicacion in publicaciones)
        {
            
            var likesCount = repo.GetLikes(publicacion.IdPublicacion);
            likesPorPublicacion.Add(publicacion.IdPublicacion, likesCount);
            
            publicacion.Likeado = repo.IsLiked(publicacion.IdPublicacion, currentUser.Username);
        }

        
        ViewBag.LikesPorPublicacion = likesPorPublicacion;

        return View(publicaciones);
    }



    [HttpPost]
    public IActionResult Like(int idPublicacion)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        repo.Like(idPublicacion, currentUser.Username);

        ViewBag.LikesPorPublicacion = ViewBag.LikesPorPublicacion + 1;

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult Dislike(int idPublicacion)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        repo.Dislike(idPublicacion, currentUser.Username);

        return RedirectToAction("Index");
    }

    [HttpPost]
    public IActionResult ToggleLike(int idPublicacion, bool isLiked)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return Unauthorized(); 
        }

        if (isLiked)
        {
            repo.Dislike(idPublicacion, currentUser.Username);
        }
        else
        {
            repo.Like(idPublicacion, currentUser.Username);
        }

        return Ok();
    }




}
