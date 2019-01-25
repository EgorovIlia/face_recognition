using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoryEnumeration;

namespace Database
{
    public class Face
    {
        [Key]
        public int FaceId { get; set; }

        public double Age { get; set; }
        public string Gender { get; set; }
        public CategoryEnum Cat { get; set; }

        //image
        //public BitmapImage Bitmap { get; set; }
        public byte[] Bitmap { get; set; }
    }

    public class Catalog : DbContext
    {
        //public Catalog() : base("DefaultConnection") { }
        public DbSet<Face> Faces { get; set; }
    }
}