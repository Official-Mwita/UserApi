using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using UserApi.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        // Returns an invoice by its invoice inv_no
        [HttpGet("{inv_no}")]
        public async Task<JsonResult> Get(string? inv_no)
        {
            JsonResult result;
            try
            {
                using (SqlConnection connection = _connection)
                {
                    connection.Open();
                    string procedure = "InvoiceByID";

                    using (SqlCommand command = new SqlCommand(procedure, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@InvoiceID", SqlDbType.NVarChar).Value = inv_no;
                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        reader.Read();
                        Invoice invoice = new Invoice()
                        {
                            DateCreated = reader.GetDateTime(2),
                            InvoiceID = reader.GetString(1),
                            ServerdBy = reader.GetString(5),
                            TotalBillable = reader.GetDecimal(3),
                            TotalTaxable = reader.GetDecimal(4)
                        };
                        result = new JsonResult(invoice);

                    }
                }
            }
            catch (Exception)
            {
                result = new JsonResult("An error occured processing your result");
            }
           
                return result;
        }

        // Saves a new invoice into the database. Using stored procedure
        [HttpPost]
        public async Task<string> Post([FromBody] Invoice value)
        {
            JObject jsonObject = new JObject();
            try
            {
                using (SqlConnection connection = _connection)
                {
                    connection.Open();

                    string query = "InsterttoInvoice";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Invoice_ID", SqlDbType.NVarChar).Value = value.InvoiceID;
                        command.Parameters.AddWithValue("@Total_Billable", SqlDbType.Money).Value = value.TotalBillable;
                        command.Parameters.AddWithValue("@Total_Taxable", SqlDbType.Money).Value = value.TotalTaxable;
                        command.Parameters.AddWithValue("@Served_By", SqlDbType.NVarChar).Value = value.ServerdBy;

                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        reader.Read();
                        jsonObject.Add("Redirect", "api/Invoice");
                        jsonObject.Add("Message", reader.GetString(0));
                        jsonObject.Add("Success", true);
                        jsonObject.Add("Date", DateTime.Now);
                    }


                }
            }
            catch (Exception)
            {
                jsonObject.Add("Message", "An error occured processing you request. Try again");
                jsonObject.Add("Success", false);
                jsonObject.Add("Date", DateTime.Now);

            }
            string message = jsonObject.ToString();
            return message;
           
        }


        // DELETE api/<InvoiceController>/5
        [HttpDelete("{inv_no}")]
        public async Task<string> Delete(string inv_no)
        {
            JObject jsonObject = new JObject();
            try
            {
                using (SqlConnection connection = _connection)
                {
                    connection.Open();


                    string query = "Delete_Invoice";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Invoice_ID", SqlDbType.NVarChar).Value = inv_no;

                        SqlDataReader reader = await command.ExecuteReaderAsync();
                        reader.Read();

                        jsonObject.Add("Redirect", "api/Invoice");
                        jsonObject.Add("Message", reader.GetString(0));
                        jsonObject.Add("Success", true);
                        jsonObject.Add("Date", DateTime.Now);
                    }
                }

            }
            catch (Exception)
            {
                jsonObject.Add("Message", "An error occured processing you request. Try again");
                jsonObject.Add("Success", false);
                jsonObject.Add("Date", DateTime.Now);


            }

            string message = jsonObject.ToString();
            return message;

        }
    }
}
