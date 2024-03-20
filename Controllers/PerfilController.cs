using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RedSocialNetCore.Extensions;
using RedSocialNetCore.Helpers;
using RedSocialNetCore.Models;
using RedSocialNetCore.Repositories;

public class PerfilController : Controller
{
    private RepositoryPerfil repo;
    private HelperCryptography helper;

    public PerfilController(RepositoryPerfil repo)
    {
        this.repo = repo;
    }

    public IActionResult VerPerfil(string otherUser)
    {
        Usuario user = repo.GetUser(otherUser);
        HttpContext.Session.SetObject("OtherUser", user);


        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");
        var otherUserr = HttpContext.Session.GetObject<Usuario>("OtherUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        var publicaciones = repo.GetPublicacionesUsuario(otherUserr.Username);

        int publicacionesCount = publicaciones.Count();

        var seguidosCount = repo.GetSeguidosCount(otherUserr.Username);
        var seguidoresCount = repo.GetSeguidoresCount(otherUserr.Username);

        
        ViewBag.PublicacionesCount = publicacionesCount;
        ViewBag.SeguidosCount = seguidosCount;
        ViewBag.SeguidoresCount = seguidoresCount;


        
        var likesPorPublicacion = new Dictionary<int, int>();
        foreach (var publicacion in publicaciones)
        {
            var likesCount = repo.GetLikes(publicacion.IdPublicacion);
            likesPorPublicacion.Add(publicacion.IdPublicacion, likesCount);
            publicacion.Likeado = repo.IsLiked(publicacion.IdPublicacion, currentUser.Username);
        }

        ViewBag.LikesPorPublicacion = likesPorPublicacion;

        var isFollowing = repo.IsFollowing(currentUser.Username, otherUserr.Username);

        ViewBag.IsFollowing = isFollowing;



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


    [HttpPost]
    public IActionResult Follow(string otherUser)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        repo.Follow(currentUser.Username, otherUser);

        return RedirectToAction("VerPerfil", new { otherUser });
    }

    [HttpPost]
    public IActionResult Unfollow(string otherUser)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        repo.Unfollow(currentUser.Username, otherUser);

        return RedirectToAction("VerPerfil", new { otherUser });
    }


    [HttpPost]
    public IActionResult EliminarPublicacion(int idPublicacion)
    {
        var currentUser = HttpContext.Session.GetObject<Usuario>("CurrentUser");

        if (currentUser == null)
        {
            return RedirectToAction("Login", "Inicio");
        }

        repo.EliminarPublicacion(idPublicacion, currentUser.Username);

        return RedirectToAction("VerPerfil", new { otherUser = currentUser.Username });
    }


}






