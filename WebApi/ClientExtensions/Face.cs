using CategoryEnumeration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientExtensions
{
    public class Face
    {
        public Face(double age, string gender, byte[] bmp, CategoryEnum cat) //BitmapImage
        {
            Age = age;
            Gender = gender;
            Cat = cat;
            //Bitmap = bmp;
            //Bitmap.Freeze();
            Bitmap = bmp;
        }

        public Face() : base()
        {
        }

        public int FaceId { get; set; }

        public double Age { get; set; }
        public string Gender { get; set; }
        public CategoryEnum Cat { get; set; }

        //image
        //public BitmapImage Bitmap { get; set; }
        public byte[] Bitmap { get; set; }
    }

    public static class ConvertTools
    {
        public static Shared.Face ToShared(Face f)
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

        public static Face ToClient(Shared.Face f)
        {
            return new Face()
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
