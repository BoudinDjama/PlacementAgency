using AgencePlacementUi.Class;
using DataBaseConnection.Table;
using Microsoft.Ajax.Utilities;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Helpers;

namespace DataBaseConnection
{
    public class RefTableConnection
    {
        private MySqlConnection databaseConnection;
        private string server;
        private string database;
        private string port;
        private string uid;
        private string password;
        public RefTableConnection()
        {
            server = "localhost";
            port = "3306";
            database = "recruitment";
            uid = "root";
            password = "123456";

            string connectionString;
            connectionString = "SERVER=" + server + ";"  + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";


             databaseConnection = new MySqlConnection($"Database={database};Server={server};Port={port};" +
                 $"User Id={uid};Password={password}");
           


           // databaseConnection = new MySqlConnection(connectionString);
           
        }
        public Dictionary<string, string> TestConnection()
        {
            var myDictionary = new Dictionary<string, string>();
            myDictionary.Add("Key1", "Value1");
            myDictionary.Add("Key2", "Value2");
            return myDictionary;
        }
        public int GetAccountId(string tableName, string email)
        {
            int result;
            if (OpenConnection())
            {
               
             
            }
            


            string query = $"SELECT id_{tableName} FROM {tableName} where {tableName}_email = @email";
            





            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                cmd.Parameters.AddWithValue("@email", email);

                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                if (dataReader.HasRows)
                {
                    result = int.Parse(dataReader[$"id_{tableName}"].ToString());
                }
                else
                {
                    result = 0;
                }
               


            }
            catch
            {
                //Console.Write("error");
                CloseConnection();

                return 0;
            }
            CloseConnection();
            return result;
        }
        public bool AccountExists(string tableName, string email)
        {
            
            bool result;
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }






            try
            {

                MySqlCommand cmd = databaseConnection.CreateCommand();
                cmd.CommandText = $"SELECT id_{tableName} FROM {tableName} where {tableName}_email = @email";
            cmd.Parameters.AddWithValue("@email", email);

                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                if (dataReader.HasRows)
                {
                    result = true;
                }
                else
                {
                    result = false;
                }


            }
            catch
            {
                //Console.Write("error");
                CloseConnection();

                return false;
            }
            CloseConnection();
            return result;
        }
        public string GetDefaultValue(string tableName)
        {
            string result;
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $" SELECT {tableName}_nom FROM {tableName} ORDER BY id_{tableName} limit 1;";
           




            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                result = dataReader["nom"].ToString();



            }
            catch
            {
                //Console.Write("error");
                CloseConnection();

                return "aucun";
            }
            CloseConnection();
            return result;
        }
        private List<string> GetTable(string tableName)
        {
            if (OpenConnection())
            {
                //
                Console.WriteLine("connected");
            }
            else
            {
                Console.WriteLine("failed");
            }

            List<string> result = new List<string>();

            string query = $"SELECT {tableName}_nom FROM {tableName}";




            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    result.Add(dataReader[$"{tableName}_nom"].ToString());
                    
                }
            }
            catch
            {
                
                CloseConnection();
                return new List<string> { "no result" };
            }
            CloseConnection();
            return result;
        }
        public List<MatchCandidat> GetMatchCandidats(int id)
        {
            List<MatchCandidat> m = new List<MatchCandidat>();
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $"SELECT o.id_offre, o.nom_poste " +
                $"FROM offre o WHERE o.salaire_min <= (SELECT CAST(REPLACE(REPLACE(salaire_nom, ',', ''), '$', '') AS DECIMAL(10, 2)) AS decimal_value FROM salaire s " +
                $"WHERE s.id_salaire = (select salaire from Candidat where id_Candidat = {id}))" +
                $"AND o.experience_min <= (SELECT c.candidat_experience FROM Candidat c WHERE c.id_candidat = {id})" +
                $"AND o.nom_poste = (SELECT p.poste_nom FROM poste p WHERE p.id_poste = (select poste from Candidat where id_Candidat = {id}))";




            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list

                while (dataReader.Read())
                {
                
                    MatchCandidat c = new MatchCandidat();
                    c.data.Add("id", dataReader["id_offre"].ToString());
                    c.data.Add("nom", dataReader["nom_poste"].ToString());
                    m.Add(c);

                }

            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                CloseConnection();

                return null;
            }

            CloseConnection();
            return m;
        }
        public void UpdateCandidat(Candidat c)
        {
            if (OpenConnection())
            {

            }
            MySqlCommand cmd;
            
            try
            {
                cmd = databaseConnection.CreateCommand();
                cmd.CommandText = @"
                UPDATE candidat SET 
                candidat_nom = @nom,
                candidat_prenom = @prenom,
                candidat_titre = @titre,
                pass = @pass,
                candidat_email = @email,
                candidat_telephone = @telephone,
                communication_preferee = (SELECT ID_TYPECOMMUNICATION FROM TYPECOMMUNICATION WHERE TYPECOMMUNICATION_nom = @com_pref),
                poste = (SELECT ID_poste FROM poste WHERE poste_nom = @poste),
                region = (SELECT ID_region FROM region WHERE region_nom = @region),
                langue = (SELECT ID_langue FROM langue WHERE langue_nom = @langue),
                candidat_experience = (SELECT ID_experience FROM experience WHERE experience_nom = @experience),
                salaire = (SELECT ID_salaire FROM salaire WHERE salaire_nom = @salaire),
                horaire = (SELECT ID_horaire FROM horaire WHERE horaire_nom = @horaire)
                WHERE id_candidat = @id";






                cmd.Parameters.AddWithValue("@nom", c.data["nom"]);
                cmd.Parameters.AddWithValue("@prenom", c.data["prenom"]);
                cmd.Parameters.AddWithValue("@pass", c.data["pass"]);
                cmd.Parameters.AddWithValue("@titre", c.data["titre"]);
                cmd.Parameters.AddWithValue("@email", c.data["email"]);
                cmd.Parameters.AddWithValue("@telephone", c.data["telephone"]);
                cmd.Parameters.AddWithValue("@com_pref", c.data["com_pref"]);

                cmd.Parameters.AddWithValue("@poste", c.data["poste"]);
                cmd.Parameters.AddWithValue("@region", c.data["region"]);
                cmd.Parameters.AddWithValue("@langue", c.data["langue"]);
                cmd.Parameters.AddWithValue("@experience", c.data["experience"]);
                cmd.Parameters.AddWithValue("@salaire", c.data["salaire"]);
                cmd.Parameters.AddWithValue("@horaire", c.data["horaire"]);
                cmd.Parameters.AddWithValue("@id", c.data["id"]);
                string id = c.data["id"];
                int.Parse(id);

                cmd.ExecuteNonQuery();
                CloseConnection();
            }
            catch(Exception e)
            {
                Debug.Write(e.ToString());
                CloseConnection();
            }

        }
        public List<Offer> GetOffers(int id)
        {
            List<Offer> o = new List<Offer>();
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $"SELECT o.id_OFFRE, COUNT(c.id_OFFRE) AS application, o.nom_poste FROM offre o LEFT JOIN correspondance c ON c.id_offre = o.id_offre WHERE o.id_employeur = {id} GROUP BY o.id_OFFRE;";




            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list

                while (dataReader.Read() )
                {
                    Offer c = new Offer();
                    c.data.Add("id", dataReader["id_offre"].ToString());
                    c.data.Add("nom", dataReader["nom_poste"].ToString());
                    c.data.Add("application", dataReader["application"].ToString());
                    /*
                    // c.data.Add("id_employeur", dataReader["id_employeur"].ToString());
                    c.data.Add("descri", dataReader["description_poste"].ToString());
                    //c.data.Add("domaine", dataReader["domaine"].ToString());
                    c.data.Add("region", dataReader["region"].ToString());
                   // c.data.Add("diplome", dataReader["diplome"].ToString());
                    c.data.Add("langue", dataReader["Langue"].ToString());
                    c.data.Add("experiencemin", dataReader["experience_min"].ToString());
                    c.data.Add("experiencemax", dataReader["experience_max"].ToString());
                    c.data.Add("salairemin", dataReader["salaire_min"].ToString());
                    c.data.Add("salairemax", dataReader["salaire_max"].ToString());
                    c.data.Add("horaire", dataReader["horaire"].ToString());
                  //  c.data.Add("application", dataReader["application"].ToString());
                    */
                    o.Add(c);
                }

            }
            catch
            {
                //Console.Write("error");
                CloseConnection();

                return null;
            }

            CloseConnection();
            return o;
        }
        public Offer GetOffer(int id)
        {
            Offer c = new Offer();
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $"SELECT o.*, COALESCE(COUNT(c.id_offre), 0) AS application FROM offre o LEFT JOIN correspondance c ON c.id_offre = o.id_offre WHERE o.id_offre = {id} GROUP BY o.id_offre";




           try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list

                bool readed = false;
                while (dataReader.Read() && readed == false)
                {

                    readed = true;
                    c.data.Add("nom", dataReader["nom_poste"].ToString());
                    c.data.Add("id_employeur", dataReader["id_employeur"].ToString());
                    c.data.Add("id", dataReader["id_offre"].ToString());
                    c.data.Add("descri", dataReader["description_poste"].ToString());
                   // c.data.Add("poste", dataReader["poste"].ToString());
                   // c.data.Add("domaine", dataReader["domaine"].ToString());
                    c.data.Add("region", dataReader["region"].ToString());
                  //  c.data.Add("diplome", dataReader["diplome"].ToString());
                   // c.data.Add("permis_conduire", dataReader["permis"].ToString());
                    c.data.Add("langue", dataReader["Langue"].ToString());
                    c.data.Add("experience_min", dataReader["experience_min"].ToString());
                    c.data.Add("experience_max", dataReader["experience_max"].ToString());
                    c.data.Add("salaire_min", dataReader["salaire_min"].ToString());
                    c.data.Add("salaire_max", dataReader["salaire_max"].ToString());
                    c.data.Add("horaire", dataReader["horaire"].ToString());
                    c.data.Add("application", dataReader["application"].ToString());

                }

            }
            catch
            {
                CloseConnection();

                return null;
            }

            CloseConnection();
            return c;
        }
        public Employeur GetEmployeur(int id)
        {
            Employeur e = new Employeur();
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $"SELECT * FROM EMPLOYEUR where id_employeur = {id}";




            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list

                bool readed = false;
                while (dataReader.Read() && readed == false)
                {

                    readed = true;
                    e.data.Add("nom", dataReader["employeur_nom"].ToString());
                    e.data.Add("prenom", dataReader["employeur_prenom"].ToString());
                    e.data.Add("pass", dataReader["pass"].ToString());
                    e.data.Add("id", dataReader["id_employeur"].ToString());
                    e.data.Add("email", dataReader["employeur_email"].ToString());
                    e.data.Add("telephone", dataReader["employeur_telephone"].ToString());
                    e.data.Add("com_pref", dataReader["communication_preferee"].ToString());
                    e.data.Add("entreprise", dataReader["entreprise"].ToString());
                    e.data.Add("entreprise_descri", dataReader["entreprise_descri"].ToString());
                    

                }

            }
            catch
            {
                //Console.Write("error");
                CloseConnection();

                return null;
            }

            CloseConnection();
            return e;
        }
        public Candidat GetCandidat(int id)
        {
            Candidat c = new Candidat();
            if (OpenConnection())
            {
                //Console.WriteLine("connected");
            }


            string query = $"SELECT * FROM CANDIDAT where id_candidat = {id}";
           

            try
            {
                //first query
                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();


                //second query
                string poste = "", region = "", salaire = "", horaire= "", experience = "", langue = "";


                //Read the data and store them in the list

                bool readed = false;
                while (dataReader.Read() && readed == false)
                {

                    readed = true;
                    c.data.Add("nom", dataReader["candidat_nom"].ToString());
                    c.data.Add("prenom", dataReader["candidat_prenom"].ToString());
                    c.data.Add("pass", dataReader["pass"].ToString());
                    c.data.Add("id", dataReader["id_candidat"].ToString());
                    c.data.Add("titre", dataReader["candidat_titre"].ToString());
                    c.data.Add("email", dataReader["candidat_email"].ToString());
                    c.data.Add("telephone", dataReader["candidat_telephone"].ToString());
                    c.data.Add("com_pref", dataReader["communication_preferee"].ToString());            
                //    c.data.Add("domaine", dataReader["domaine"].ToString());
                    

                    poste   = dataReader["poste"].ToString();
                    region  = dataReader["region"].ToString();
                    salaire = dataReader["salaire"].ToString();
                    horaire = dataReader["horaire"].ToString();
                    experience = dataReader["candidat_experience"].ToString();
                    langue  = dataReader["langue"].ToString();


                }
                c.data.Add("poste", GetForeignKeyValue("poste", poste));
                c.data.Add("region", GetForeignKeyValue("region", region));
                c.data.Add("salaire", GetForeignKeyValue("salaire", salaire));
                c.data.Add("horaire", GetForeignKeyValue("horaire", horaire));
                c.data.Add("experience", GetForeignKeyValue("experience", experience));
                c.data.Add("langue", GetForeignKeyValue("langue", langue));


            }
            catch (Exception ex)
            {
                Debug.WriteLine("the following is the error:" + ex);
                CloseConnection();
                
                return null;
            }
          
            CloseConnection();
            Dictionary<string, string> clone = c.data;
            foreach (KeyValuePair<string, string> pair in c.data.ToList())
            {
                if (pair.Value.Equals(null))
                {
                    clone[pair.Key] = GetDefaultValue(pair.Key);
                }
                else if(pair.Key != "id")
                {
                    string s = pair.Value;

                    try
                    {
                        int i = int.Parse(pair.Value);

                        try
                        {

                            clone[pair.Key] = GetFieldById(i, pair.Key);
                        }
                        catch
                        {
                            clone[pair.Key] = s;
                        }
                    }
                    catch
                    {
                        clone[pair.Key] = s;
                        continue;
                    }
                }
            }
            c.data = clone;
            return c;
        }


        public string GetForeignKeyValue(string tableName, string key)
        {
            CloseConnection();

            if (OpenConnection()) { }

            try
            {
                

                string query = $"SELECT {tableName}_nom FROM {tableName} where id_{tableName} = @keyValue";

                using (MySqlCommand cmd = new MySqlCommand(query, databaseConnection))
                {
                    cmd.Parameters.AddWithValue("@keyValue", key);

                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            return dataReader.GetString(0);
                        }
                        else
                        {
                            // Handle no results (e.g., log or return a default value)
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle specific exceptions (e.g., log or throw)
                Debug.WriteLine($"Error: {ex.Message}");
                return null;
            }
            finally
            {
                CloseConnection();
            }
           
        }


        public string GetFieldById(int id, string tableName)
        {
           
                if (OpenConnection())
                {
                    //Console.WriteLine("connected");
                }
            
            

            string result ;

            string query = $"SELECT {tableName}_nom FROM {tableName} where id_{tableName} = {id}";
            



            try
            {

                MySqlCommand cmd = new MySqlCommand(query, databaseConnection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                dataReader.Read();
                    result = dataReader[$"{tableName}_nom"].ToString();

                
            }
            catch (Exception e)
            {
                Debug.WriteLine("issue is: " +  e);
                CloseConnection();
                return "Error";
            }
            CloseConnection();
            return result;
        }
    
        public List<string> GetExperience()
        {
            return GetTable("EXPERIENCE");
        }
        public List<string> GetLangue()
        {
            return GetTable("LANGUE");
        }
        public List<string> GetDomaine()
        {
            return GetTable("DOMAINE");
        }
        public List<string> GetRegion()
        {
            return GetTable("REGION");
        }
        public List<string> GetDiplome()
        {
            return GetTable("DIPLOME");
        }
        public List<string> GetTypeCommunication()
        {
            return GetTable("typecommunication");
        }
        public List<string> GetSalaire()
        {
            return GetTable("SALAIRE");
        }
        public List<string> GetHoraire()
        {
            return GetTable("HORAIRE");
        }
        public List<string> GetPermisConduire()
        {
            return GetTable("PERMIS_CONDUIRE");
        }
        public List<string> GetPoste()
        {
            return GetTable("POSTE");
        }
        private bool OpenConnection()
        {
            try
            {
                databaseConnection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
              
                switch (ex.Number)
                {
                    case 0:
                   //     Console.WriteLine("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                     //   Console.WriteLine("Invalid username/password, please try again");
                        break;
                }
                return false;
            }
        }
       /* public bool AlreadyApplied(int candidatId, int offerId)
        {
            if(OpenConnection()) { }
            string query = $"SELECT {tableName}_nom FROM {tableName} where id_{tableName} = {id}";

        }
        */

        private bool CloseConnection()
        {
            try
            {
                databaseConnection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
              //  Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
