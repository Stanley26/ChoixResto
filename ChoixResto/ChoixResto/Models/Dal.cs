using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Security.Cryptography;
using System.Text;

namespace ChoixResto.Models
{
    public class Dal : IDal
    {
        private BddContext bdd;

        public Dal()
        {
            bdd = new BddContext();
        }

        public List<Resto> ObtientTousLesRestaurants()
        {
            return bdd.Restos.ToList();
        }

        public void Dispose()
        {
            bdd.Dispose();
        }

        #region Restaurant

        public void CreerRestaurant(string nom, string telephone)
        {
            bdd.Restos.Add(new Resto { Nom = nom, Telephone = telephone });
            bdd.SaveChanges();
        }

        public void ModifierRestaurants(int id, string nom, string telephone)
        {
            Resto restoTrouve = bdd.Restos.FirstOrDefault(resto => resto.Id == id);
            if (restoTrouve != null)
            {
                restoTrouve.Nom = nom;
                restoTrouve.Telephone = telephone;
                bdd.SaveChanges();
            }
        }

        public bool RestaurantExiste(string nom)
        {
            Resto restoExist = bdd.Restos.FirstOrDefault(resto => resto.Nom == nom);
            if (restoExist != null)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region Utilisateur

        public int CreerUtilisateur(string prenom, string motDePasse)
        {
            string motDePasseCode = EncodeMD5(motDePasse);
            Utilisateur utilisateur = new Utilisateur { Prenom = prenom, MotDePasse = motDePasseCode };
            bdd.Utilisateurs.Add(utilisateur);
            bdd.SaveChanges();

            return utilisateur.Id;
        }



        public Utilisateur ObtenirUtilisateur(int id)
        {
            Utilisateur utilisateurTrouve = bdd.Utilisateurs.FirstOrDefault(utilisateur => utilisateur.Id == id);
            if (utilisateurTrouve != null)
            {
                return utilisateurTrouve;
            }
            return null;
        }

        public Utilisateur ObtenirUtilisateur(string idString)
        {
            if (!ConvertirId(idString))
            {
                return null;
            }
            else
                return ObtenirUtilisateur(Int32.Parse(idString));
        }

        public Utilisateur Authentifier(string prenom, string motDePasse)
        {
            string motDePasseCode = EncodeMD5(motDePasse);
            Utilisateur utilisateurAuthentifier = bdd.Utilisateurs.FirstOrDefault(utilisateur => utilisateur.Prenom == prenom && utilisateur.MotDePasse == motDePasseCode);

            return utilisateurAuthentifier;
        }

        private string EncodeMD5(string motDePasse)
        {
            string motDePasseSel = "ChoixResto" + motDePasse + "ASP.NET MVC";
            return BitConverter.ToString(new MD5CryptoServiceProvider().ComputeHash(ASCIIEncoding.Default.GetBytes(motDePasse)));
        }
        #endregion

        #region Sondage

        public int CreerSondage()
        {
            Sondage sondage = new Sondage { Date = DateTime.Now, Votes = new List<Vote> { } };
            bdd.Sondages.Add(sondage);
            bdd.SaveChanges();

            return sondage.Id;
        }

        public Sondage ObtenirSondage(string idString)
        {
            if (!ConvertirId(idString))
            {
                return null;
            }
            else
            {
                return bdd.Sondages.FirstOrDefault(sondage => sondage.Id == int.Parse(idString));
            }
        }

        public Sondage ObtenirSondage(int id)
        {
            return bdd.Sondages.FirstOrDefault(sondage => sondage.Id == id);
        }

        #endregion

        #region Vote

        public void AjouterVote(int idSondage, int idResto, int idUtilisateur)
        {
            Sondage sondage = bdd.Sondages.FirstOrDefault(sondageList => sondageList.Id == idSondage);
            Resto resto = bdd.Restos.FirstOrDefault(restoList => restoList.Id == idResto);
            Utilisateur utilisateur = bdd.Utilisateurs.FirstOrDefault(utilisateurList => utilisateurList.Id == idUtilisateur);

            sondage.Votes.Add(new Vote { Resto = resto, Utilisateur = utilisateur });
            bdd.SaveChanges();
        }


        public bool ADejaVote(int idSondage, string idUtilisateur)
        {
            bool aDejaVote = false;
            Sondage sondageEnCour = bdd.Sondages.FirstOrDefault(sondageList => sondageList.Id == idSondage);

            if (sondageEnCour == null)
            {
                return false;
            }

            foreach (Vote vote in sondageEnCour.Votes)
            {
                if (vote.Utilisateur.Id == int.Parse(idUtilisateur))
                {
                    aDejaVote = true;
                }
            }

            return aDejaVote;
        }

        #endregion


        public List<Resultats> ObtenirResultats(int idSondage)
        {
            List<Resultats> listResultat = new List<Resultats>();

            Sondage sondage = bdd.Sondages.FirstOrDefault(sondageList => sondageList.Id == idSondage);

            foreach (Resto resto in bdd.Restos)
            {
                listResultat.Add(new Resultats { Nom = resto.Nom, Telephone = resto.Telephone, NombreDeVotes = 0 });
            }

            foreach (Vote vote in sondage.Votes)
            {
                foreach (Resultats resultat in listResultat)
                {
                    if (vote.Resto.Nom == resultat.Nom)
                    {
                        resultat.NombreDeVotes++;
                    }
                }
            }
            return listResultat;
        }




        private bool ConvertirId(string idString)
        {
            int id;
            if (int.TryParse(idString, out id))
                return true;
            return false;
        }


    }

}