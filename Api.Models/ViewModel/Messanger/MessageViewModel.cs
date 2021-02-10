using System;
using System.Collections.Generic;
using System.Text;

namespace Api.Models.ViewModel.Messanger
{
    public class MessageViewModel
    {
        public string CryptedText { get; set; }
        public string DialogName { get; set; }
        public DateTime Created { get; set; }
    }
}
