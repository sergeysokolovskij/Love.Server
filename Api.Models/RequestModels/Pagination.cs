using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.RequestModels
{
    public class Pagination
    {
        public int CurrentPage { get; set; }
        public int CountItemsInPage { get; set; }
    }
}
