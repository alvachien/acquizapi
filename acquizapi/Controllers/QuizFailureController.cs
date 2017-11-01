using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/quizfailure")]
    [Authorize]
    public class QuizFailureController : Controller
    {
        // GET: api/QuizFailure
        [HttpGet("{usr}")]
        public async Task<IActionResult> Get(String usr)
        {
            List<QuizFailureRetry> listRst = new List<QuizFailureRetry>();
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
                String queryString = @"SELECT [quizid]
                                  ,[quiztype]
                                  ,[submitdate]
                                  ,[failidx]
                                  ,[expected]
                                  ,[inputted]
                              FROM [dbo].[v_quizfailure]
                              WHERE [attenduser] = @user";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@user", usrName);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        QuizFailureRetry qz = new QuizFailureRetry
                        {
                            QuizID = reader.GetInt32(0),
                            QuizType = reader.GetInt16(1),
                            SubmitDate = reader.GetDateTime(2),
                            QuizFailIndex = reader.GetInt32(3),
                            Expected = reader.GetString(4),
                            Inputted = reader.GetString(5)
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
                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                    conn = null;
                }
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
            
            return new JsonResult(listRst, setting);
        }
    }
}
