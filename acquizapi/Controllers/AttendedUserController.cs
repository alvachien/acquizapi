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
using System.Net;

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
            String strErrMsg = "";

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;
            else
                return BadRequest("User cannot reconginze!");

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            HttpStatusCode errorCode = HttpStatusCode.OK;

            try
            {
                String queryString = @"SELECT c.[userid], b.displayas 
                        FROM [v_permuser] c JOIN [quizuser] b ON c.[userid] = b.[userid] 
                        UNION SELECT [userid], [displayas] FROM [quizuser] WHERE [userid] = @usr;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@usr", usrName);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            QuizAttendUser au = new QuizAttendUser
                            {
                                AttendUser = reader.GetString(0)
                            };
                            if (reader.IsDBNull(1))
                                au.DisplayAs = String.Empty;
                            else
                                au.DisplayAs = reader.GetString(1);
                            listRst.Add(au);
                        }
                    }

                    if (listRst.Count == 0)
                    {
                        QuizAttendUser au = new QuizAttendUser
                        {
                            AttendUser = usrName,
                            DisplayAs = usrName
                        };
                        listRst.Add(au);
                    }
                }
            }
            catch(Exception exp)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(exp.Message);
#endif
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

            return new JsonResult(listRst);
        }
    }
}
