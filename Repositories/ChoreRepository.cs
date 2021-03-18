using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    /// <summary>
    /// This class is responsible for interacting with the Room data.
    /// It inherits from the BaseRepository class (the tunnel!) so that it can use the BaseRepository's Connection property
    /// </summary>
    /// Friendly reminder: the ':' says you get to inherit things from the BaseRepository
    public class ChoreRepository : BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }
        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using SqlCommand cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT Id, Name FROM Chore";

                SqlDataReader reader = cmd.ExecuteReader();
                List<Chore> chores = new List<Chore>();

                while (reader.Read())
                {
                    int idColumnPosition = reader.GetOrdinal("Id");
                    int idValue = reader.GetInt32(idColumnPosition);

                    int nameColumnPosition = reader.GetOrdinal("Name");
                    string nameValue = reader.GetString(nameColumnPosition);

                    Chore chore = new Chore
                    {
                        Id = idValue,
                        Name = nameValue,
                    };

                    chores.Add(chore);
                }
                reader.Close();
                return chores;
            }
        }

        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    SqlDataReader reader = cmd.ExecuteReader();

                    Chore chore = null;

                    if (reader.Read())
                    {
                        chore = new Chore
                        {
                            Id = id,
                            Name = reader.GetString(reader.GetOrdinal("Name"))
                        };
                    }
                    reader.Close();
                    return chore;
                }
            }
        }

        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                    OUTPUT INSERTED.Id
                                    VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }
    }
}
