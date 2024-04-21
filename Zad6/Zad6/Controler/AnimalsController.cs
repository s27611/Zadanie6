using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Zad6.Model;

namespace YourNamespace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        private readonly string _connectionString;

        public AnimalsController(string connectionString)
        {
            _connectionString = connectionString;
        }

        [HttpGet]
        public IActionResult GetAnimals(string orderBy = "name")
        {
            try
            {
                var animals = new List<AnimalDto>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"SELECT * FROM Animal ORDER BY {orderBy}";
                    
                    using (var command = new SqlCommand(query, connection))
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            animals.Add(new AnimalDto
                            {
                                IdAnimal = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Category = reader.GetString(3),
                                Area = reader.GetString(4)
                            });
                        }
                    }
                }

                return Ok(animals);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public IActionResult AddAnimal([FromBody] AnimalDto animal)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", animal.Name);
                        command.Parameters.AddWithValue("@Description", (object)animal.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Category", animal.Category);
                        command.Parameters.AddWithValue("@Area", animal.Area);

                        command.ExecuteNonQuery();
                    }
                }

                return StatusCode(201); // Created
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{idAnimal}")]
        public IActionResult UpdateAnimal(int idAnimal, [FromBody] AnimalDto animal)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"UPDATE Animal SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", animal.Name);
                        command.Parameters.AddWithValue("@Description", (object)animal.Description ?? DBNull.Value);
                        command.Parameters.AddWithValue("@Category", animal.Category);
                        command.Parameters.AddWithValue("@Area", animal.Area);
                        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(); // Success
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{idAnimal}")]
        public IActionResult DeleteAnimal(int idAnimal)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = $"DELETE FROM Animal WHERE IdAnimal = @IdAnimal";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                        command.ExecuteNonQuery();
                    }
                }

                return Ok(); // Success
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
