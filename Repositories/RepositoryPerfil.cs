using Microsoft.AspNetCore.Mvc;
using RedSocialNetCore.Data;
using RedSocialNetCore.Models;

namespace RedSocialNetCore.Repositories
{
    public class RepositoryPerfil
    {
        private ContextApp context;

        public RepositoryPerfil(ContextApp context)
        {
            this.context = context;
        }


        public List<Publicacion> GetPublicacionesUsuario(string username)
        {
            return context.Publicaciones
                .Where(p => p.Username == username)
                .OrderByDescending(p => p.FechaPublicacion)
                .ToList();
        }

        public Usuario GetUser(string username)
        {
            var usuario = (from u in context.Usuarios
                           where u.Username == username
                           select u).FirstOrDefault();

            return usuario;
        }

        public bool IsLiked(int idPublicacion, string username)
        {
            return context.Likes.Any(l => l.IdPublicacion == idPublicacion && l.Username == username);
        }

        public void Like(int idPublicacion, string username)
        {
            if (!IsLiked(idPublicacion, username))
            {
                Like like = new Like { IdPublicacion = idPublicacion, Username = username };
                context.Likes.Add(like);
                context.SaveChanges();
            }
        }

        public void Dislike(int idPublicacion, string username)
        {
            Like like = context.Likes.FirstOrDefault(l => l.IdPublicacion == idPublicacion && l.Username == username);
            if (like != null)
            {
                context.Likes.Remove(like);
                context.SaveChanges();
            }
        }

        public int GetLikes(int idPublicacion)
        {
            return context.Likes.Count(l => l.IdPublicacion == idPublicacion);
        }


        public int GetSeguidosCount(string username)
        {
            return context.Seguidores.Count(s => s.SeguidorUsername == username);
        }

        public int GetSeguidoresCount(string username)
        {
            return context.Seguidores.Count(s => s.SeguidoUsername == username);
        }


        public bool IsFollowing(string seguidorUsername, string seguidoUsername)
        {
            return context.Seguidores.Any(s => s.SeguidorUsername == seguidorUsername && s.SeguidoUsername == seguidoUsername);
        }


        public void Follow(string seguidorUsername, string seguidoUsername)
        { 
            if (!IsFollowing(seguidorUsername, seguidoUsername))
            {
               
                Seguidores seguimiento = new Seguidores
                {
                    SeguidorUsername = seguidorUsername,
                    SeguidoUsername = seguidoUsername
                };

                
                context.Seguidores.Add(seguimiento);
                context.SaveChanges();
            }
        }

        public void Unfollow(string seguidorUsername, string seguidoUsername)
        {
            
            Seguidores seguimiento = context.Seguidores.FirstOrDefault(s => s.SeguidorUsername == seguidorUsername && s.SeguidoUsername == seguidoUsername);

            if (seguimiento != null)
            {
               
                context.Seguidores.Remove(seguimiento);
                context.SaveChanges();
            }
        }

        [HttpPost]
        public void EliminarPublicacion(int idPublicacion, string username)
        {
            
            Publicacion publicacion = context.Publicaciones.FirstOrDefault(p => p.IdPublicacion == idPublicacion && p.Username == username);

            if (publicacion != null)
            {
                
                var likes = context.Likes.Where(l => l.IdPublicacion == idPublicacion);
                context.Likes.RemoveRange(likes);

                
                context.Publicaciones.Remove(publicacion);
                context.SaveChanges();
            }
        }





    }
}
