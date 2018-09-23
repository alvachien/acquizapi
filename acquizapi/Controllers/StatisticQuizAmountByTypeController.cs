using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Net;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/StatisticQuizAmountByType")]
    [Authorize]
    public class StatisticQuizAmountByTypeController : Controller
    {
        // GET: api/StatisticQuizAmountByType
        [HttpGet]
        public IActionResult Get()
        {
            return BadRequest();
        }

        // GET: api/StatisticQuizAmountByType/5
        [HttpGet("{usr}")]
        public async Task<IActionResult> Get(string usr)
        {
            List<QuizAmountByTypeStatistics> listRst = new List<QuizAmountByTypeStatistics>();
            String strErrMsg = "";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            HttpStatusCode errorCode = HttpStatusCode.OK;

            // Get user name
            String usrName = String.Empty;
            if (String.IsNullOrEmpty(usr))
            {
                var usro = User.FindFirst(c => c.Type == "sub");
                if (usr != null)
                    usrName = usro.Value;
                else
                    return BadRequest("User cannot recognize");
            }
            else
                usrName = usr;

            try
            {
                String queryString = @"SELECT [quiztype]
                                  ,[attenduser]
                                  ,[quizamount]
                              FROM [v_quizamountbytype]
                              WHERE [attenduser] = @user";
                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@user", usrName);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            QuizAmountByTypeStatistics qz = new QuizAmountByTypeStatistics
                            {
                                QuizType = (QuizTypeEnum)reader.GetInt16(0),
                                AttendUser = reader.GetString(1),
                                Amount = reader.GetInt32(2)
                            };
                            listRst.Add(qz);
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                strErrMsg = exp.Message;
                if (errorCode == HttpStatusCode.OK)
                    errorCode = HttpStatusCode.InternalServerError;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Dispose();
                    reader = null;
                }
                if (cmd != null)
                {
                    cmd.Dispose();
                    cmd = null;
                }
                if (conn != null)
                {
                    conn.Dispose();
                    conn = null;
                }
            }

            if (errorCode != HttpStatusCode.OK)
            {
                switch (errorCode)
                {
                    case HttpStatusCode.Unauthorized:
                        return Unauthorized();
                    case HttpStatusCode.NotFound:
                        return NotFound();
                    case HttpStatusCode.BadRequest:
                        return BadRequest();
                    default:
                        return StatusCode(500, strErrMsg);
                }
            }

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            
            return new JsonResult(listRst, setting);
        }
    }
}
