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
    [Route("api/StatisticQuizItemAmountByType")]
    [Authorize]
    public class StatisticQuizItemAmountByTypeController : Controller
    {
        // GET: api/StatisticQuizItemAmountByType
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return BadRequest();
        }

        // GET: api/StatisticQuizItemAmountByType/5
        [HttpGet("{usr}")]
        public async Task<IActionResult> Get(string usr)
        {
            List<QuizItemAmountByTypeStatistics> listRst = new List<QuizItemAmountByTypeStatistics>();
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
                String queryString = @"SELECT [quiztype]
                                  ,[attenduser]
                                  ,[totalitems]
                                  ,[faileditems]
                              FROM [v_quizitemamountbytype]
                              WHERE [attenduser] = @user";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@user", usrName);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        QuizItemAmountByTypeStatistics qz = new QuizItemAmountByTypeStatistics
                        {
                            QuizType = reader.GetInt16(0),
                            AttendUser = reader.GetString(1),
                            TotalAmount = reader.GetInt32(2),
                            FailedAmount = reader.GetInt32(3)
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
