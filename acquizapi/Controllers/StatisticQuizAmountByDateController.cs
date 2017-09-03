using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/StatisticQuizAmountByDate")]
    [Authorize]
    public class StatisticQuizAmountByDateController : Controller
    {
        // GET: api/StatisticQuizAmountByDate
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return BadRequest();
        }

        // GET: api/StatisticQuizAmountByDate/5
        [HttpGet("{usr}")]
        public async Task<IActionResult> Get(string usr)
        {
            List<QuizAmountByDateStatistics> listRst = new List<QuizAmountByDateStatistics>();
            Boolean bError = false;
            String strErrMsg = "";

            // Get user name
            String usrName = String.Empty;
            if (String.IsNullOrEmpty(usr))
            {
                var usro = User.FindFirst(c => c.Type == "sub");
                if (usr != null)
                    usrName = usro.Value;
            }
            else
            {
                usrName = usr;
            }

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [submitdate]
                                  ,[attenduser]
                                  ,[quizamount]
                              FROM [v_quizamountbydate]
                              WHERE [attenduser] = @user";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@user", usrName);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        QuizAmountByDateStatistics qz = new QuizAmountByDateStatistics
                        {
                            QuizDate = reader.GetDateTime(0),
                            AttendUser = reader.GetString(1),
                            Amount = reader.GetInt32(2)
                        };
                        listRst.Add(qz);
                    }
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                bError = true;
                strErrMsg = exp.Message;
            }
            finally
            {
                conn.Close();
                conn.Dispose();
            }

            if (bError)
            {
                return StatusCode(500, strErrMsg);
            }

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            ;
            return new JsonResult(listRst, setting);
        }
    }
}
