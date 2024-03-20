using Microsoft.EntityFrameworkCore;
using RedSocialNetCore.Data;
using RedSocialNetCore.Models;

namespace RedSocialNetCore.Repositories
{
    public class RepositoryEditarPerfil
    {

        private ContextApp context;

        public RepositoryEditarPerfil(ContextApp context)
        {
            this.context = context;
        }

        public Usuario GetUser(string username)
        {
            var usuario = (from u in context.Usuarios
                           where u.Username == username
                           select u).FirstOrDefault();

            return usuario;
        }

        public async Task UpdateUser(Usuario usuario)
        {
            var user = context.Usuarios.FirstOrDefault(u => u.Username == usuario.Username);

            if (user != null)
            {
                user.Nombre = string.IsNullOrEmpty(usuario.Nombre) ? string.Empty : usuario.Nombre;
                user.Email = string.IsNullOrEmpty(usuario.Email) ? string.Empty : usuario.Email;
                user.Telefono = string.IsNullOrEmpty(usuario.Telefono) ? string.Empty : usuario.Telefono;
                user.Descripcion = string.IsNullOrEmpty(usuario.Descripcion) ? string.Empty : usuario.Descripcion;

                if (!string.IsNullOrEmpty(usuario.FotoPerfil))
                {
                    user.FotoPerfil = usuario.FotoPerfil;

                    var sql = $"UPDATE Publicaciones SET foto_perfil = '{usuario.FotoPerfil}' WHERE username = '{usuario.Username}'";

                    await context.Database.ExecuteSqlRawAsync(sql);

                }

                context.Entry(user).State = EntityState.Modified;

                await context.SaveChangesAsync();
            }
            else
            {
                throw new Exception("Usuario no encontrado");
            }
        }




    }
}
