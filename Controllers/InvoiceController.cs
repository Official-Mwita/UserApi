using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using UserApi.Models;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : ControllerBase
    {
        private readonly SqlConnection _connection;
        public InvoiceController(IConfiguration config)
        {
            //Creating a database instance when controller is initialized
            _connection = new SqlConnection(config.GetConnectionString("StandardStr"));

        }
        // Return a Json Array of all available invoice from the database
        [HttpGet]
        public async Task<JsonResult> Get()
        {
            List<Invoice> list = new List<Invoice>(); //Create a list to hold


            using(SqlConnection connection = _connection)
            {
                connection.Open();
                string query = "SELECT * FROM Invoice";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                   SqlDataReader reader = await command.ExecuteReaderAsync();
                     while (reader.Read())
                    {
                        Invoice invoice = new Invoice() {DateCreated = reader.GetDateTime(2),
                                                         InvoiceID = reader.GetString(1),
                                                         ServerdBy = reader.GetString(5),
                                                         TotalBillable = reader.GetDecimal(3),
                                                         TotalTaxable = reader.GetDecimal(4)};
                        list.Add(invoice);
                    }

                }
            }
            JsonResult result = new JsonResult(list);
            return result;
        }

        // Returns an invoice by its invoice id
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // Saves a new invoice into the database. Using stored procedure
        [HttpPost]
        public void Post([FromBody] Invoice value)
        {
            using(SqlConnection connection = _connection)
            {
                connection.Open();

                string query = "InsterttoInvoice";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.CommandType = System.Data.CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@Invoice_ID", SqlDbType.NVarChar).Value = value.InvoiceID;
                    command.Parameters.AddWithValue("@Total_Billable", SqlDbType.Money).Value = value.TotalBillable;
                    command.Parameters.AddWithValue("@Total_Taxable", SqlDbType.Money).Value = value.TotalTaxable;
                    command.Parameters.AddWithValue("@Served_By", SqlDbType.NVarChar).Value = value.ServerdBy;

                    command.ExecuteReader();
                }


            }
        }

        // PUT api/<InvoiceController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<InvoiceController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
