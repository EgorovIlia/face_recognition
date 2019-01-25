using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Service.Controllers
{
    public class FacesController : ApiController
    {
        // GET api/books
        [Route("api/faces")]
        public IEnumerable<Shared.Face> Get()
        {
            var catalog = new Database.Catalog();
            return catalog.Faces.Select(Utils.ToShared).ToArray();
        }

        [HttpPost]
        [Route("api/faces")]
        public void Post(Shared.Face face)
        {
            var catalog = new Database.Catalog();
            catalog.Faces.Add(Utils.ToDatabase(face));
            catalog.SaveChanges();
        }

        [Route("api/faces")]
        public void Delete()
        {
            var catalog = new Database.Catalog();
            catalog.Faces.RemoveRange(catalog.Faces);
            catalog.SaveChanges();
        }
    }

    public static class Utils
    {
        public static Shared.Face ToShared(Database.Face f)
        {
            return new Shared.Face()
            {
                FaceId = f.FaceId,
                Age = f.Age,
                Gender = f.Gender,
                Cat = f.Cat,
                Bitmap = f.Bitmap
            };
        }

        public static Database.Face ToDatabase(Shared.Face f)
        {
            return new Database.Face()
            {
                FaceId = f.FaceId,
                Age = f.Age,
                Gender = f.Gender,
                Cat = f.Cat,
                Bitmap = f.Bitmap
            };
        }
    }
}