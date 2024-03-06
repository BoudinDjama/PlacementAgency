using DataBaseConnection;
using DataBaseConnection.Table;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI;

namespace AgencePlacementUi.Class
{
    public class Requete 
    {

        private MySqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;

        public Requete()
        {
            Initialize();
        }
        private void Initialize()
        {

            server = "localhost";
            port = "3306";
            database = "recruitment";
            uid = "root";
            password = "123456";

            

            connection = new MySqlConnection($"Database={database};Server={server};Port={port};" +
                $"User Id={uid};Password={password}");
        }
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, The application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }

        public void InsertCandidat(Candidat c)
        {
            if (OpenConnection())
            {

            }
            MySqlCommand cmd;
            { 
            
                ///try
                
                    cmd = connection.CreateCommand();
                    cmd.CommandText = "INSERT INTO CANDIDAT (candidat_nom, candidat_prenom, candidat_titre, candidat_email, candidat_telephone, " +
                        "communication_preferee, date_modif, pass) VALUES( @nom, @prenom , @titre, @email, @telephone, " +
                        "(SELECT ID_TYPECOMMUNICATION from TYPECOMMUNICATION WHERE nom_TYPECOMMUNICATION = @com_pref), CURDATE(), @pass)";

                    cmd.Parameters.AddWithValue("@nom", c.data["candidat_nom"]);
                    cmd.Parameters.AddWithValue("@prenom", c.data["candidat_prenom"]);

                   
                    cmd.Parameters.AddWithValue("@pass", c.data["pass"]);
                    cmd.Parameters.AddWithValue("@titre", c.data["candidat_titre"]);
                    cmd.Parameters.AddWithValue("@email", c.data["candidat_email"]);
                    cmd.Parameters.AddWithValue("@telephone", c.data["candidat_telephone"]);
                    cmd.Parameters.AddWithValue("@com_pref", c.data["communication_preferee"]);


                cmd.ExecuteNonQuery();
                CloseConnection();
                


            }
            //catch
            {
                CloseConnection();
            }
        }

        public void InsertEmployeur(Employeur c)
        {
            if (OpenConnection())
            {

            }
            MySqlCommand cmd;
            {

                ///try

                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO EMPLOYEUR (employeur_nom, employeur_prenom, employeur_email, employeur_telephone, " +
                    "communication_preferee, date_modif, pass, entreprise, entreprise_descri) VALUES( @nom, @prenom , @email, @telephone, " +
                    "(SELECT ID_TYPECOMMUNICATION from TYPECOMMUNICATION WHERE nom_TYPECOMMUNICATION = @com_pref), CURDATE(), @pass, @entreprise, @entreprise_descri)";

                cmd.Parameters.AddWithValue("@nom", c.data["nom"]);
                cmd.Parameters.AddWithValue("@prenom", c.data["prenom"]);

                cmd.Parameters.AddWithValue("@descri", c.data["descri"]);
                cmd.Parameters.AddWithValue("@pass", c.data["pass"]);
                cmd.Parameters.AddWithValue("@titre", c.data["titre"]);
                cmd.Parameters.AddWithValue("@email", c.data["email"]);
                cmd.Parameters.AddWithValue("@telephone", c.data["telephone"]);
                cmd.Parameters.AddWithValue("@com_pref", c.data["com_pref"]);
                cmd.Parameters.AddWithValue("@entreprise", c.data["entreprise"]);
                cmd.Parameters.AddWithValue("@entreprise_descri", c.data["entreprise_descri"]);

                cmd.ExecuteNonQuery();
                CloseConnection();



            }
            //catch
            {
                CloseConnection();
            }
        }

        public void InsertOffre(Offer o)
        {
            if (OpenConnection())
            {

            }
            MySqlCommand cmd;
            {

                ///try

                cmd = connection.CreateCommand();
                cmd.CommandText = "INSERT INTO Offre (id_employeur, nom_poste, " +
                    "region, date_modif, salaire_min, salaire_max, experience_min, experience_max, horaire, langue) VALUES( @id_employeur, @nom , @region, CURDATE(), @salaire_min, @salaire_max, " +
                    "@experience_min, @experience_max, @horaire, @Langue)";

                cmd.Parameters.AddWithValue("@id_employeur", o.data["id_employeur"]);
                cmd.Parameters.AddWithValue("@nom", o.data["nom"]);
                cmd.Parameters.AddWithValue("@region", o.data["region"]);

                cmd.Parameters.AddWithValue("@salaire_min", o.data["salaire_min"]);
                cmd.Parameters.AddWithValue("@salaire_max", o.data["salaire_max"]);
                cmd.Parameters.AddWithValue("@experience_min", o.data["experience_min"]);
                cmd.Parameters.AddWithValue("@experience_max", o.data["experience_max"]);
                cmd.Parameters.AddWithValue("@horaire", o.data["horaire"]);
                cmd.Parameters.AddWithValue("@langue", o.data["langue"]);
                

                cmd.ExecuteNonQuery();
                CloseConnection();
            }

        }
        
        public void InsertTheQuery(string query)
        {
            //open connection
            if (this.OpenConnection() == true)
            {
                //create command and assign the query and connection from the constructor
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //Execute command
                cmd.ExecuteNonQuery();

                //close connection
                this.CloseConnection();
            }
        }
       
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //Trying to figure out the onClick for the button register.
       /* protected void Button1_OnClick(Object sender, EventArgs e)
        {
           Insert();
        }
        */
    }
    


    }

