using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using CategoryEnumeration;

namespace ClientExtensions
{
    public class CategoriesObservable : ObservableCollection<Category>, INotifyPropertyChanged//, IDeepCopy
    {
        public void AddCategory(CategoryEnum category)
        {
            Add(new Category(category));
        }

        public void AddCategoryFace(Face face)
        {
            bool found = false;
            foreach (var item in this)
            {
                if (item.Cat == face.Cat)
                {
                    lock (item)
                    {
                        lock (item.Faces)
                        {
                            item.Faces.Add(face);
                        }
                    }
                    found = true;
                }
            }
            if (!found)
            {
                AddCategory(face.Cat);
                lock ((Category)this.Last())
                {
                    lock (((Category)this.Last()).Faces)
                    {
                        ((Category)this.Last()).Faces.Add(face);
                    }
                }
            }
        }
    }
}
