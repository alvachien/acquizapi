using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using acquizapi.Models;
using System.Data.SqlClient;

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
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [userid], [displayas], [others] FROM [quizuser];";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        UserDetail ud = new UserDetail();
                        ud.UserID = reader.GetString(0);
                        ud.DisplayAs = reader.GetString(1);
                        if (reader.IsDBNull(2))
                            ud.Others = null;
                        else
                            ud.Others = reader.GetString(2);
                        listRst.Add(ud);
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

        // GET: api/UserDetail/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            UserDetail objRst = null;
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();

                String queryString = @"SELECT [userid], [displayas], [others] FROM [quizuser] WHERE [userid] = @id;";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        objRst = new UserDetail();
                        objRst.UserID = reader.GetString(0);
                        objRst.DisplayAs = reader.GetString(1);
                        if (reader.IsDBNull(2))
                            objRst.Others = null;
                        else
                            objRst.Others = reader.GetString(2);
                        break;
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
            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            String queryString = "";
            Boolean bError = false;
            String strErrMsg = "";

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;

            try
            {
                await conn.OpenAsync();
                queryString = @"INSERT INTO [quizuser] ([userid],[displayas],[others]) VALUES (@userid, @displayas, @others)";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@userid", ud.UserID);
                cmd.Parameters.AddWithValue("@displayas", ud.DisplayAs);
                if (String.IsNullOrEmpty(ud.Others))
                    cmd.Parameters.AddWithValue("@others", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@others", ud.Others);

                Int32 nRst = await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
                cmd = null;
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
            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            String queryString = "";
            Boolean bError = false;
            String strErrMsg = "";

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;

            try
            {
                await conn.OpenAsync();
                queryString = @"UPDATE [dbo].[quizuser] SET [displayas] = @displayas, [others] = @others WHERE [userid] = @userid;";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@displayas", ud.DisplayAs);
                if (String.IsNullOrEmpty(ud.Others))
                    cmd.Parameters.AddWithValue("@others", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@others", ud.Others);
                cmd.Parameters.AddWithValue("@userid", ud.UserID);

                Int32 nRst = await cmd.ExecuteNonQueryAsync();
                cmd.Dispose();
                cmd = null;
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

            return new JsonResult(ud);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return BadRequest();
        }
    }
}
