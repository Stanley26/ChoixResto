
using ChoixResto.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ChoixResto.Tests
{
    [TestClass]
    public class DalTests
    {
        private Dal dal;

        [TestInitialize]
        public void Init_AvantChaqueTest()
        {
            IDatabaseInitializer<BddContext> init = new DropCreateDatabaseAlways<BddContext>();
            Database.SetInitializer(init);
            init.InitializeDatabase(new BddContext());

            dal = new Dal();
        }

        [TestCleanup]
        public void ApresChaqueTest()
        {
            dal.Dispose();
        }


        [TestMethod]
        public void CreerRestaurant_AvecNouveauRestaurant_ObtientTousLesRestaurantsRenvoitBienLeRestaurant()
        {
            using (Dal dal = new Dal())
            {
                dal.CreerRestaurant("Victor", "01 02 03 04 05");
                List<Resto> restos = dal.ObtientTousLesRestaurants();

                Assert.IsNotNull(restos);
                Assert.AreEqual(1, restos.Count);
                Assert.AreEqual("Victor", restos[0].Nom);
                Assert.AreEqual("01 02 03 04 05", restos[0].Telephone);
            }

        }

        [TestMethod]
        public void ModifierRestaurant_CreationDUnNouveauRestaurantEtChangementNomEtTelephone_LaModificationEstCorrecteApresRechargement()
        {
            using (Dal dal = new Dal())
            {
                dal.CreerRestaurant("Delice", "01 02 03 04 05");
                dal.ModifierRestaurants(1, "Cosmos", null);

                List<Resto> restos = dal.ObtientTousLesRestaurants();
                Assert.IsNotNull(restos);
                Assert.AreEqual(1, restos.Count);
                Assert.AreEqual("Cosmos", restos[0].Nom);
                Assert.IsNull(restos[0].Telephone);
            }
        }


        [TestMethod]
        public void RestaurantExiste_AvecCreationDunRestaurant_RenvoiQuiExiste()
        {
            dal.CreerRestaurant("Victor", "999-999-9999");
            Assert.IsTrue(dal.RestaurantExiste("Victor"));
        }


        [TestMethod]
        public void RestaurantExiste_AvecRestaurantInexistant_RenvoiQuiExiste()
        {
            Assert.IsFalse(dal.RestaurantExiste("Victor"));
        }

        [TestMethod]
        public void ObtenirUtilisateur_UtilisateurInexistant_RetourneNull()
        {
            Assert.IsNull(dal.ObtenirUtilisateur(1));
        }

        [TestMethod]
        public void ObtenirUtilisateur_IdNonNumerique_RetourneNull()
        {
            Assert.IsNull(dal.ObtenirUtilisateur("Pascal"));
        }

        [TestMethod]
        public void AjouterUtilisateur_NouvelUtilisateurEtRecuperation_UtilisateurEstBienRecupere()
        {
            dal.CreerUtilisateur("Pascal", "12345");

            Utilisateur utilisateur = dal.ObtenirUtilisateur(1);

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Pascal", utilisateur.Prenom);

            utilisateur = dal.ObtenirUtilisateur("1");

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Pascal", utilisateur.Prenom);
        }

        [TestMethod]
        public void Authentifier_LoginMdpOk_AuthentificationOK()
        {
            dal.CreerUtilisateur("Pascal", "12345");

            Utilisateur utilisateur = dal.Authentifier("Pascal", "12345");

            Assert.IsNotNull(utilisateur);
            Assert.AreEqual("Pascal", utilisateur.Prenom);
        }

        [TestMethod]
        public void Authentifier_LoginOkMdpKo_AuthentificationKo()
        {
            dal.CreerUtilisateur("Pascal", "12345");
            Utilisateur utilisateur = dal.Authentifier("Pascal Savard", "12345");

            Assert.IsNull(utilisateur);
        }


        [TestMethod]
        public void ADejaVote_AvecIdNomNumerique_RetourneFalse()
        {
            Assert.IsFalse(dal.ADejaVote(1, "1"));
        }

        [TestMethod]
        public void ADejaVote_UtilisateurNaPasVote_RetourneFalse()
        {
            int idSondage = dal.CreerSondage();
            int idUtilisateur = dal.CreerUtilisateur("Pascal", "12345");
            bool pasVote = dal.ADejaVote(idSondage, idUtilisateur.ToString());

            Assert.IsFalse(pasVote);
        }

        [TestMethod]
        public void ADejaVote_UtilisateurAVote_RetourneTrue()
        {
            int idSondge = dal.CreerSondage();
            int idUtilisateur = dal.CreerUtilisateur("Pascal", "12345");
            dal.CreerRestaurant("Victor", "555-555-5555");
            dal.AjouterVote(idSondge, 1, idUtilisateur);

            Assert.IsTrue(dal.ADejaVote(idSondge, idUtilisateur.ToString()));
        }

        [TestMethod]
        public void ObtenirLesResultats_AvecQuelquesChoix_RetourneBienLesResultats()
        {
            int idSondage = dal.CreerSondage();
            int idUtilisateur1 = dal.CreerUtilisateur("Pascal", "12345");
            int idUtilisateur2 = dal.CreerUtilisateur("Sarah", "12345");
            int idUtilisateur3 = dal.CreerUtilisateur("Maxime", "12345");

            dal.CreerRestaurant("Victor", "555-555-5555");
            dal.CreerRestaurant("Delice", "555-555-5555");
            dal.CreerRestaurant("Cosmos", "555-555-5555");
            dal.CreerRestaurant("McDonald", "555-555-5555");

            dal.AjouterVote(idSondage, 2, idUtilisateur1);
            dal.AjouterVote(idSondage, 2, idUtilisateur2);
            dal.AjouterVote(idSondage, 3, idUtilisateur3);

            dal.AjouterVote(idSondage, 2, idUtilisateur1);
            dal.AjouterVote(idSondage, 2, idUtilisateur2);
            dal.AjouterVote(idSondage, 3, idUtilisateur3);

            dal.AjouterVote(idSondage, 1, idUtilisateur1);
            dal.AjouterVote(idSondage, 1, idUtilisateur2);
            dal.AjouterVote(idSondage, 1, idUtilisateur3);

            List<Resultats> resultats = dal.ObtenirResultats(idSondage);

            Assert.AreEqual(3, resultats[0].NombreDeVotes);
            Assert.AreEqual("Victor", resultats[0].Nom);

            Assert.AreEqual(4, resultats[1].NombreDeVotes);
            Assert.AreEqual("Delice", resultats[1].Nom);

            Assert.AreEqual(2, resultats[2].NombreDeVotes);
            Assert.AreEqual("Cosmos", resultats[2].Nom);

            Assert.AreEqual(0, resultats[3].NombreDeVotes);
            Assert.AreEqual("McDonald", resultats[3].Nom);
        }

        [TestMethod]
        public void ObtenirLesResultats_AvecDeuxSondages_RetourneBienLesResultats()
        {
            int idSondage1 = dal.CreerSondage();

            int idUtilisateur1 = dal.CreerUtilisateur("Pascal", "12345");
            int idUtilisateur2 = dal.CreerUtilisateur("Sarah", "12345");
            int idUtilisateur3 = dal.CreerUtilisateur("Maxime", "12345");

            dal.CreerRestaurant("Victor", "555-555-5555");
            dal.CreerRestaurant("Delice", "555-555-5555");
            dal.CreerRestaurant("Cosmos", "555-555-5555");
            dal.CreerRestaurant("McDonald", "555-555-5555");

            dal.AjouterVote(idSondage1, 2, idUtilisateur1);
            dal.AjouterVote(idSondage1, 2, idUtilisateur2);
            dal.AjouterVote(idSondage1, 3, idUtilisateur3);
            dal.AjouterVote(idSondage1, 2, idUtilisateur1);
            dal.AjouterVote(idSondage1, 2, idUtilisateur2);
            dal.AjouterVote(idSondage1, 3, idUtilisateur3);
            dal.AjouterVote(idSondage1, 1, idUtilisateur1);
            dal.AjouterVote(idSondage1, 1, idUtilisateur2);
            dal.AjouterVote(idSondage1, 1, idUtilisateur3);

            int idSondage2 = dal.CreerSondage();

            dal.AjouterVote(idSondage2, 4, idUtilisateur1);
            dal.AjouterVote(idSondage2, 4, idUtilisateur2);
            dal.AjouterVote(idSondage2, 4, idUtilisateur3);
            dal.AjouterVote(idSondage2, 4, idUtilisateur1);
            dal.AjouterVote(idSondage2, 4, idUtilisateur2);
            dal.AjouterVote(idSondage2, 1, idUtilisateur3);
            dal.AjouterVote(idSondage2, 2, idUtilisateur1);
            dal.AjouterVote(idSondage2, 3, idUtilisateur2);
            dal.AjouterVote(idSondage2, 4, idUtilisateur3);

            List<Resultats> resultats1 = dal.ObtenirResultats(idSondage1);
            List<Resultats> resultats2 = dal.ObtenirResultats(idSondage2);

            Assert.AreEqual(3, resultats1[0].NombreDeVotes);
            Assert.AreEqual("Victor", resultats1[0].Nom);

            Assert.AreEqual(4, resultats1[1].NombreDeVotes);
            Assert.AreEqual("Delice", resultats1[1].Nom);

            Assert.AreEqual(2, resultats1[2].NombreDeVotes);
            Assert.AreEqual("Cosmos", resultats1[2].Nom);

            Assert.AreEqual(0, resultats1[3].NombreDeVotes);
            Assert.AreEqual("McDonald", resultats1[3].Nom);


            Assert.AreEqual(1, resultats2[0].NombreDeVotes);
            Assert.AreEqual("Victor", resultats2[0].Nom);

            Assert.AreEqual(1, resultats2[1].NombreDeVotes);
            Assert.AreEqual("Delice", resultats2[1].Nom);

            Assert.AreEqual(1, resultats2[2].NombreDeVotes);
            Assert.AreEqual("Cosmos", resultats2[2].Nom);

            Assert.AreEqual(6, resultats2[3].NombreDeVotes);
            Assert.AreEqual("McDonald", resultats2[3].Nom);

        }


    }
}
