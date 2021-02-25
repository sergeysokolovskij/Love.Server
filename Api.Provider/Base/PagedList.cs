using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Provider.Base
{
    public interface IPagedList<T>
    {

    }
    public class PagedList<T> : IPagedList<T>
    {
        public int CountItems { get; set; }
        public int CountPages { get; set; }
        public List<T> Items { get; set; }

        public PagedList(List<T> items, 
            int countPages,
            int countItems)
        {
            CountItems = countItems;
            CountPages = countPages;
            Items = items;
        }
    }
}
