using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using acquizapi.Models;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/AttendedUser")]
    [Authorize]
    public class AttendedUserController : Controller
    {
        // GET: api/AttendedUser
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<QuizAttendUser> listRst = new List<QuizAttendUser>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT a.[attenduser], b.displayas FROM 
                        (SELECT DISTINCT [attenduser] FROM [quiz]) a LEFT OUTER JOIN [quizuser] b 
                        ON a.[attenduser] = b.[userid]";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        QuizAttendUser au = new QuizAttendUser();
                        au.AttendUser = reader.GetString(0);
                        if (reader.IsDBNull(1))
                            au.DisplayAs = String.Empty;
                        else 
                            au.DisplayAs = reader.GetString(1);
                        listRst.Add(au);
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

            return new JsonResult(listRst);
        }
    }
}
