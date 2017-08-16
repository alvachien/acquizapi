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
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<QuizFailureRetry> listRst = new List<QuizFailureRetry>();
            Boolean bError = false;
            String strErrMsg = "";

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;

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
                        QuizFailureRetry qz = new QuizFailureRetry();
                        qz.QuizID = reader.GetInt32(0);
                        qz.QuizType = reader.GetInt16(1);
                        qz.SubmitDate = reader.GetDateTime(2);
                        qz.QuizFailIndex = reader.GetInt32(3);
                        qz.Expected = reader.GetString(4);
                        qz.Inputted = reader.GetString(5);
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

            var setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.DateFormatString = "yyyy-MM-dd";
            setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(); ;
            return new JsonResult(listRst, setting);
        }
    }
}
