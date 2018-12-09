using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ChoixResto.ViewModels
{
    public class AccueilViewModel
    {
        [Display(Name = "Le message")]
        public string Message { get; set; }
        public DateTime Date { get; set; }
        public Models.Resto Resto { get; set; }
        public string Login { get; set; }
    }
}