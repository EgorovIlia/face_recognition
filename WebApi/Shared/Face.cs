using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CategoryEnumeration;

namespace Shared
{
    public class Face
    {
        public int FaceId { get; set; }

        public double Age { get; set; }
        public string Gender { get; set; }
        public CategoryEnum Cat { get; set; }

        //image
        //public BitmapImage Bitmap { get; set; }
        public byte[] Bitmap { get; set; }
    }
}
