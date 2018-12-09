using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoixResto.Models
{
    interface IDal : IDisposable
    {
        void CreerRestaurant(string nom, string telephone);
        void ModifierRestaurants(int id, string nom, string telephone);
        bool RestaurantExiste(string nom);

        int CreerUtilisateur(string prenom, string motDePasse);
        Utilisateur ObtenirUtilisateur(string id);
        Utilisateur ObtenirUtilisateur(int id);
        Utilisateur Authentifier(string prenom, string motDePasse);

        int CreerSondage();
        Sondage ObtenirSondage(string idString);
        Sondage ObtenirSondage(int id);

        void AjouterVote(int idSondage, int idResto, int idUtilisateur);
        bool ADejaVote(int idSondage, string idUtilisateur);

        List<Resto> ObtientTousLesRestaurants();
        List<Resultats> ObtenirResultats(int idSondage);
    }
}
