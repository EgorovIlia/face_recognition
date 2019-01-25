using CategoryEnumeration;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using Shared;

namespace ClientExtensions
{
    public class Category
    {

        public Category(CategoryEnum category)
        {
            Faces = new ObservableCollection<Face>();
            this.Cat = category;
            string cat = category.ToString();
            if (cat == "not_determined")
            {
                this.Gender = null;
                this.AgeFrom = -1;
                this.AgeTo = -1;
            }
            else
            {
                if (cat[0] == 'M')
                    this.Gender = "Male";
                else if (cat[0] == 'F')
                    this.Gender = "Female";
                int startIndex = cat.IndexOf("x") + ("x").Length;
                int endIndex = cat.IndexOf("_");
                this.AgeFrom = int.Parse(cat.Substring(startIndex, endIndex - startIndex));
                startIndex = cat.IndexOf("_") + ("_").Length;
                this.AgeTo = int.Parse(cat.Substring(startIndex, cat.Length - startIndex));
            }
        }

        public Category() : base()
        {
        }

        public CategoryEnum Cat { get; set; }

        public string Gender { get; set; }
        public int AgeFrom { get; set; }
        public int AgeTo { get; set; }

        public ObservableCollection<Face> Faces { get; set; }

        public override string ToString()
        {
            if (this.Cat == CategoryEnum.not_determined)
                return CategoryEnum.not_determined.ToString();
            if (this.AgeTo == -1)
                return this.Gender + " from " + this.AgeFrom;
            return this.Gender + " from " + this.AgeFrom + " to " + this.AgeTo;
        }
    }
}
