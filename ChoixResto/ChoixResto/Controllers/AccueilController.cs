using ChoixResto.Models;
using ChoixResto.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChoixResto.Controllers
{
    public class AccueilController : Controller
    {
        // GET: Accueil
        public ActionResult Index()
        {
            List<Models.Resto> listeDesRestos = new List<Resto>
            {
                new Resto { Id = 1, Nom = "Victor", Telephone = "111-111-1111"},
                new Resto { Id = 2, Nom = "Delice", Telephone = "111-111-1111"},
                new Resto { Id = 3, Nom = "Cosmos", Telephone = "111-111-1111"},
                new Resto { Id = 4, Nom = "McDonald", Telephone = "111-111-1111"},
                new Resto { Id = 5, Nom = "St-Hubert", Telephone = "111-111-1111"}
            };

            ViewBag.ListeDesRestos = new SelectList(listeDesRestos, "Id", "Nom", 2);

            AccueilViewModel vm = new AccueilViewModel
            {
                Message = "Bonjour !",
                Date = DateTime.Now,
                Resto = new Resto { Nom = "Victor", Telephone = "555-555-5555" }
            };

            return View(vm);
        }

        [ChildActionOnly]
        public ActionResult AfficherListeRestaurant()
        {
            List<Models.Resto> listeDesRestos = new List<Resto>
            {
                new Resto { Id = 1, Nom = "Victor", Telephone = "111-111-1111"},
                new Resto { Id = 2, Nom = "Delice", Telephone = "111-111-1111"},
                new Resto { Id = 3, Nom = "Cosmos", Telephone = "111-111-1111"},
                new Resto { Id = 4, Nom = "McDonald", Telephone = "111-111-1111"},
                new Resto { Id = 5, Nom = "St-Hubert", Telephone = "111-111-1111"}
            };
            return PartialView(listeDesRestos);
        }
    }
}