using RedSocialNetCore.Data;
using RedSocialNetCore.Models;
using Microsoft.EntityFrameworkCore;

namespace RedSocialNetCore.Repositories
{
    public class RepositoryPublicacion
    {
        private ContextApp context;

        public RepositoryPublicacion(ContextApp context)
        {
            this.context = context;
        }

        public async Task AddPublicacion(Publicacion publicacion)
        {
            
            if (publicacion == null || publicacion.Texto == null || publicacion.FechaPublicacion == null || publicacion.Username == null)
            {
                throw new ArgumentNullException("Alguno de los campos de la publicación es nulo.");
            }

            
            context.Publicaciones.Add(publicacion);
            await context.SaveChangesAsync();
        }
        public int GetNextPublicacionId()
        {
            
            int maxId = context.Publicaciones.Max(p => (int?)p.IdPublicacion) ?? 0;
            return maxId + 1;
        }


    }
}
