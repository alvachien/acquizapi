using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using acquizapi.Models;
using System.Data.SqlClient;
using System.Net;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/UserDetail")]
    [Authorize]
    public class UserDetailController : Controller
    {
        // GET: api/UserDetail
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<UserDetail> listRst = new List<UserDetail>();
            String strErrMsg = "";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            HttpStatusCode errorCode = HttpStatusCode.OK;

            try
            {
                String queryString = @"SELECT [userid], [displayas], [others],[award],[awardplan] FROM [quizuser] WHERE [deletionflag] != 1;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();
                    cmd = new SqlCommand(queryString, conn);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            UserDetail ud = new UserDetail
                            {
                                UserID = reader.GetString(0),
                                DisplayAs = reader.GetString(1)
                            };
                            if (reader.IsDBNull(2))
                                ud.Others = null;
                            else
                                ud.Others = reader.GetString(2);
                            if (reader.IsDBNull(3))
                                ud.AwardControl = null;
                            else
                                ud.AwardControl = reader.GetString(3);
                            if (reader.IsDBNull(4))
                                ud.AwardPlanControl = null;
                            else
                                ud.AwardPlanControl = reader.GetString(4);
                            listRst.Add(ud);
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

            return new JsonResult(listRst);
        }

        // GET: api/UserDetail/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            UserDetail objRst = null;
            String strErrMsg = "";
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            HttpStatusCode errorCode = HttpStatusCode.OK;

            try
            {
                String queryString = @"SELECT [userid], [displayas], [others],[award],[awardplan] FROM [quizuser] WHERE [userid] = @id;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@id", id);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objRst = new UserDetail
                            {
                                UserID = reader.GetString(0),
                                DisplayAs = reader.GetString(1)
                            };
                            if (reader.IsDBNull(2))
                                objRst.Others = null;
                            else
                                objRst.Others = reader.GetString(2);
                            if (reader.IsDBNull(3))
                                objRst.AwardControl = null;
                            else
                                objRst.AwardControl = reader.GetString(3);
                            if (reader.IsDBNull(4))
                                objRst.AwardPlanControl = null;
                            else
                                objRst.AwardPlanControl = reader.GetString(4);
                            break;
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

            if (objRst == null)
                return NotFound();

            return new JsonResult(objRst);
        }

        // POST: api/UserDetail
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserDetail ud)
        {
            if (ud == null)
            {
                return BadRequest();
            }

            // Update the database
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            String queryString = "";
            String strErrMsg = "";
            HttpStatusCode errorCode = HttpStatusCode.OK;

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;

            try
            {
                queryString = @"INSERT INTO [quizuser] ([userid],[displayas],[others],[award],[awardplan]) VALUES (@userid, @displayas, @others, @award, @awardplan)";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();                    

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@userid", ud.UserID);
                    cmd.Parameters.AddWithValue("@displayas", ud.DisplayAs);
                    if (String.IsNullOrEmpty(ud.Others))
                        cmd.Parameters.AddWithValue("@others", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@others", ud.Others);
                    if (String.IsNullOrEmpty(ud.AwardControl))
                        cmd.Parameters.AddWithValue("@award", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@award", ud.AwardControl);
                    if (String.IsNullOrEmpty(ud.AwardPlanControl))
                        cmd.Parameters.AddWithValue("@awardplan", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@awardplan", ud.AwardControl);

                    Int32 nRst = await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                    cmd = null;
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

            return new JsonResult(ud);
        }

        // PUT: api/UserDetail/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id, [FromBody]UserDetail ud)
        {
            if (ud == null)
            {
                return BadRequest();
            }

            // Update the database
            SqlConnection conn = null;
            SqlCommand cmd = null;
            String queryString = "";
            String strErrMsg = "";
            HttpStatusCode errorCode = HttpStatusCode.OK;

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;

            try
            {
                queryString = @"UPDATE [dbo].[quizuser] SET [displayas] = @displayas, [others] = @others, 
                                        [award] = @award, [awardplan] = @awardplan WHERE [userid] = @userid;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@displayas", ud.DisplayAs);
                    if (String.IsNullOrEmpty(ud.Others))
                        cmd.Parameters.AddWithValue("@others", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@others", ud.Others);
                    if (String.IsNullOrEmpty(ud.AwardControl))
                        cmd.Parameters.AddWithValue("@award", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@award", ud.AwardControl);
                    if (String.IsNullOrEmpty(ud.AwardPlanControl))
                        cmd.Parameters.AddWithValue("@awardplan", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@awardplan", ud.AwardControl);
                    cmd.Parameters.AddWithValue("@userid", ud.UserID);

                    Int32 nRst = await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                    cmd = null;
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

            return new JsonResult(ud);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            // Todo
            return BadRequest();
        }
    }
}
