﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpedizioniSPA.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.Configuration;
    using System.Data.SqlClient;

    public class Spedizione
    {
        [ScaffoldColumn(false)]
        public int IdSpedizione { get; set; }

        [Required]
        [Display(Name = "Data Spedizione")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataSpedizione { get; set; }

        [Required]
        [Display(Name = "Peso Imballaggio in kg")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Il del peso deve contenere solo numeri.")]
        public decimal Peso { get; set; }

        [Required]
        [Display(Name = "Costo Spedizione")]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Il campo costo deve contenere solo numeri.")]
        public decimal Costo { get; set; }

        [Required]
        [Display(Name = "Data Consegna")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DataConsegna { get; set; }

        [Required]
        [Display(Name = "Città di Destinazione")]
        [RegularExpression("^[a-zA-Z]*$", ErrorMessage = "Il campo Città deve contenere solo lettere.")]
        public string CittaDestinazione { get; set; }


        [Required]
        [Display(Name = "Destinatario")]
        public int IdDestinatario { get; set; }

        // Costruttore vuoto
        public Spedizione()
        {
        }

        // Costruttore con tutti i campi
        public Spedizione(int idSpedizione, DateTime dataSpedizione, decimal peso, decimal costo, DateTime dataConsegna, string cittaDestinazione, int idDestinatario)
        {
            IdSpedizione = idSpedizione;
            DataSpedizione = dataSpedizione;
            Peso = peso;
            Costo = costo;
            DataConsegna = dataConsegna;
            CittaDestinazione = cittaDestinazione;
            IdDestinatario = idDestinatario;
        }

        //Metodo per inserire una nuova Spedizione nel db
        public static void InserisciNuovaSpedizione(Spedizione S)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Spedizione2 (dataSpedizione, peso, costo, dataConsegna, cittaDestinazione, idDestinatario) VALUES (@dataSpedizione, @peso, @costo, @dataConsegna, @cittaDestinazione, @idDestinatario)", conn);
                
                cmd.Parameters.AddWithValue("@dataSpedizione", S.DataSpedizione);
                cmd.Parameters.AddWithValue("@peso", S.Peso.ToString());
                cmd.Parameters.AddWithValue("@costo", S.Costo.ToString());
                cmd.Parameters.AddWithValue("@dataConsegna", S.DataConsegna);
                cmd.Parameters.AddWithValue("@cittaDestinazione", S.CittaDestinazione);
                cmd.Parameters.AddWithValue("@idDestinatario", S.IdDestinatario.ToString());

                cmd.ExecuteNonQuery();

                Console.WriteLine("Inserimento avvenuto con Successo");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn.Close();
            }
        }


        //Metodo che seleziona e restituisce l'id della spedizione in funzione dei campi della spedizione
        //Viene usato in combinazione con la creazione di un nuovo Aggiornamento alla creazione di una spedizione per assegnare all'Aggiornamento lo stesso Id della Spedizione
        public static int GetIdSpedizione(Spedizione S)
        {
            int idSpedizione = 0;
            string connectionString = ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT idSpedizione FROM Spedizione2 WHERE dataSpedizione = @dataSpedizione AND peso = @peso AND costo = @costo AND dataConsegna = @dataConsegna AND cittaDestinazione = @cittaDestinazione AND idDestinatario = @idDestinatario", conn);

                // Aggiungi i parametri al comando
                cmd.Parameters.AddWithValue("@dataSpedizione", S.DataSpedizione);
                cmd.Parameters.AddWithValue("@peso", S.Peso);
                cmd.Parameters.AddWithValue("@costo", S.Costo);
                cmd.Parameters.AddWithValue("@dataConsegna", S.DataConsegna);
                cmd.Parameters.AddWithValue("@cittaDestinazione", S.CittaDestinazione);
                cmd.Parameters.AddWithValue("@idDestinatario", S.IdDestinatario);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    // Leggi l'ID della spedizione dal lettore
                    idSpedizione = (int)reader["idSpedizione"];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn.Close();
            }

            return idSpedizione;
        }




        //Metodo che crea ee restituisce una list di Spedizione
        public static List<Spedizione> GetListaSpedizioni()
        {
            List<Spedizione> listaSpedizioni = new List<Spedizione>();

            string connectionString = ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString();
            SqlConnection conn = new SqlConnection(connectionString);

            try
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM Spedizione2", conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    Spedizione spedizione = new Spedizione();
                    spedizione.IdSpedizione = (int)reader["idSpedizione"];
                    spedizione.DataSpedizione = (DateTime)reader["dataSpedizione"];
                    spedizione.Peso = (decimal)reader["peso"];
                    spedizione.Costo = (decimal)reader["costo"];
                    spedizione.DataConsegna = (DateTime)reader["dataConsegna"];
                    spedizione.CittaDestinazione = (string)reader["cittaDestinazione"];
                    spedizione.IdDestinatario = (int)reader["idDestinatario"];

                    listaSpedizioni.Add(spedizione);
                }

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                conn.Close();
            }

            return listaSpedizioni;
        }


        //Metodo che Restituisce il numero di spedizioni non ancora consegnate
        public static int GetNumSpedNonConsegnate()
        {
            int spedNonConsegnate = 0;
            // Utilizzo di "using" per garantire la corretta gestione delle risorse e la chiusura automatica della connessione 
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString()))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) AS NonConsegnate FROM Spedizione2 WHERE dataConsegna >= @CurrentDate";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CurrentDate", DateTime.Now.ToShortDateString());
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        spedNonConsegnate = (int)reader["NonConsegnate"];
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine("Si è verificato un errore durante il recupero del numero di spedizioni non consegnate: " + ex.Message);
                    throw;
                }
            }

            return spedNonConsegnate;
        }

        //Metodo che restituisce una List di tuple per fornire il numero di spedizioni per città
        public static List<Tuple<string, int>> GetSpedizioniPerCitta()
        {
            List<Tuple<string, int>> spedizioniPerCittaList = new List<Tuple<string, int>>();

            // Utilizzo di "using" per garantire la corretta gestione delle risorse e la chiusura automatica della connessione
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString()))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT cittaDestinazione, COUNT(cittaDestinazione) AS SpedXCitta FROM Spedizione2 GROUP BY cittaDestinazione";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        string cittaDestinazione = reader["cittaDestinazione"].ToString();
                        int spedXCitta = Convert.ToInt32(reader["SpedXCitta"]);

                        spedizioniPerCittaList.Add(new Tuple<string, int>(cittaDestinazione, spedXCitta));
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Si è verificato un errore durante il recupero delle spedizioni per città: " + ex.Message);
                    throw;
                }
            }

            return spedizioniPerCittaList;
        }

        //Metodo che restituisce una list di spedizioni che sono categorizzate come "in consegna" unendo i dati dalla lista aggiornamenti
        public static List<Spedizione> GetSpedizioniInConsegna()
        {
            List<Spedizione> spedizioniList = new List<Spedizione>();

            // Utilizzo di "using" per garantire la corretta gestione delle risorse e la chiusura automatica della connessione
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString()))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT Spedizione2.idSpedizione, dataSpedizione, peso, costo, dataConsegna, cittaDestinazione, idDestinatario  
                                 FROM Spedizione2 
                                 INNER JOIN Aggiornamenti ON Spedizione2.idSpedizione = Aggiornamenti.idSpedizione 
                                 WHERE Aggiornamenti.Stato = '3 - In Consegna'";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Spedizione spedizione = new Spedizione
                        {
                            IdSpedizione = Convert.ToInt32(reader["idSpedizione"]),
                            DataSpedizione = Convert.ToDateTime(reader["dataSpedizione"]),
                            Peso = Convert.ToDecimal(reader["peso"]),
                            Costo = Convert.ToDecimal(reader["costo"]),
                            DataConsegna = Convert.ToDateTime(reader["dataConsegna"]),
                            CittaDestinazione = reader["cittaDestinazione"].ToString(),
                            IdDestinatario = Convert.ToInt32(reader["idDestinatario"])
                        };

                        spedizioniList.Add(spedizione);
                    }
                }
                catch (Exception ex)
                {
                 
                    Console.WriteLine("Si è verificato un errore durante il recupero delle spedizioni in consegna: " + ex.Message);
                    throw;
                }
            }

            return spedizioniList;
        }


        public static bool GetSrcSpedizioni(int idSpedizione, string idCliente)
        {
            bool find = false;
            string connectionString = ConfigurationManager.ConnectionStrings["connectionStringDb"].ToString();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = @"SELECT Spedizione2.idSpedizione, " +
                                    @"CASE WHEN UAzienda.IdCliente IS NOT NULL THEN UAzienda.PIVA ELSE UPrivato.CF END AS TaxID " +
                                    @"FROM Spedizione2 " +
                                    @"LEFT JOIN UAzienda ON Spedizione2.idDestinatario = UAzienda.IdCliente " +
                                    @"LEFT JOIN UPrivato ON Spedizione2.idDestinatario = UPrivato.IdCliente " +
                                    @"WHERE Spedizione2.idSpedizione = @idSpedizione AND (UAzienda.PIVA = @idCliente OR UPrivato.CF = @idCliente)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@idSpedizione", idSpedizione);
                        cmd.Parameters.AddWithValue("@idCliente", idCliente);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                System.Diagnostics.Debug.WriteLine("Trovato match");
                                find = true;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log the exception details for troubleshooting
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            return find;
        }

    }

}