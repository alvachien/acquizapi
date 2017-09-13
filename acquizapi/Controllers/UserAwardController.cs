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
    [Route("api/UserAward")]
    [Authorize]
    public class UserAwardController : Controller
    {
        // GET: api/UserAward
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] String userid = null)
        {
            List<UserAward> listRst = new List<UserAward>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [aid],[userid],[adate],[award],[planid],[quiztype],[qid],[used],[publish] FROM [dbo].[v_useraward] ";
                if (!String.IsNullOrEmpty(userid))
                {
                    queryString += " WHERE [userid] = N'" + userid + "'";
                }

                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        listRst.Add(ConvertDB2VM(reader));
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

        private UserAward ConvertDB2VM(SqlDataReader reader)
        {
            UserAward ua = new UserAward
            {
                AwardID = reader.GetInt32(0),
                UserID = reader.GetString(1),
                AwardDate = reader.GetDateTime(2),
                Award = reader.GetInt32(3)
            };

            if (!reader.IsDBNull(4))
                ua.AwardPlanID = reader.GetInt32(4);
            else
                ua.AwardPlanID = null;
            if (!reader.IsDBNull(5))
                ua.QuizType = reader.GetInt16(5);
            else
                ua.QuizType = null;
            if (!reader.IsDBNull(6))
                ua.QuizID = reader.GetInt32(6);
            else
                ua.QuizID = null;
            if (!reader.IsDBNull(7))
                ua.UsedReason = reader.GetString(7);
            else
                ua.UsedReason = String.Empty;
            if (!reader.IsDBNull(8))
                ua.Publish = reader.GetBoolean(8);
            else
                ua.Publish = null;
            return ua;
        }

        // GET: api/UserAward/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            UserAward objRst = null;
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [aid],[userid],[adate],[award],[planid],[quiztype],[qid],[used],[publish] FROM [dbo].[v_useraward] WHERE [aid] = @aid;";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@aid", id);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        objRst = ConvertDB2VM(reader);
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

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            ;
            return new JsonResult(objRst, setting);
        }

        // POST: api/UserAward
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]UserAward vm)
        {
            if (vm == null)
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
            else
                return BadRequest("No user info found");

            try
            {
                await conn.OpenAsync();

                // Check the authority
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [award] LIKE '%C%'";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) > 0)
                        {
                            bAllow = true;
                            break;
                        }
                    }
                }

                reader.Dispose();
                reader = null;
                cmd.Dispose();
                cmd = null;

                if (!bAllow)
                {
                    return BadRequest("No authority to delete plan");
                }

                queryString = @"INSERT INTO [dbo].[useraward] ([userid],[adate],[award],[planid],[qid],[used], [publish])
                    VALUES(@userid,@adate,@award,@planid,@qid, @used, @publish);
                    SELECT @Identity = SCOPE_IDENTITY();";

                cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@userid", vm.UserID);
                cmd.Parameters.AddWithValue("@adate", vm.AwardDate);
                cmd.Parameters.AddWithValue("@award", vm.Award);
                if (vm.AwardPlanID.HasValue)
                    cmd.Parameters.AddWithValue("@planid", vm.AwardPlanID);
                else
                    cmd.Parameters.AddWithValue("@planid", DBNull.Value);
                if (vm.QuizID.HasValue)
                    cmd.Parameters.AddWithValue("@qid", vm.QuizID);
                else
                    cmd.Parameters.AddWithValue("@qid", DBNull.Value);
                if (String.IsNullOrEmpty(vm.UsedReason))
                    cmd.Parameters.AddWithValue("@used", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@used", vm.UsedReason);
                if (vm.Publish.HasValue)
                    cmd.Parameters.AddWithValue("@publish", vm.Publish.Value);
                else
                    cmd.Parameters.AddWithValue("@publish", DBNull.Value);
                SqlParameter idparam = cmd.Parameters.AddWithValue("@Identity", SqlDbType.Int);
                idparam.Direction = ParameterDirection.Output;

                Int32 nRst = await cmd.ExecuteNonQueryAsync();
                vm.AwardID = (Int32)idparam.Value;
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

            return new JsonResult(vm);
        }

        // PUT: api/UserAward/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]UserAward vm)
        {
            if (vm == null || vm.AwardID != id)
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
            else
                return BadRequest("No user info found");

            try
            {
                await conn.OpenAsync();

                // Check the authority
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [award] LIKE '%U%'";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) > 0)
                        {
                            bAllow = true;
                            break;
                        }
                    }
                }

                reader.Dispose();
                reader = null;
                cmd.Dispose();
                cmd = null;

                if (!bAllow)
                {
                    return BadRequest("No authority to delete plan");
                }

                queryString = @"UPDATE [dbo].[useraward]
                    SET [userid] = @userid
                        ,[adate] = @adate
                        ,[award] = @award
                        ,[planid] = @planid
                        ,[qid] = @qid
                        ,[used] = @used
                        ,[publish] = @publish
                    WHERE [aid] = @aid;";

                cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@userid", vm.UserID);
                cmd.Parameters.AddWithValue("@adate", vm.AwardDate);
                cmd.Parameters.AddWithValue("@award", vm.Award);
                if (vm.AwardPlanID.HasValue)
                    cmd.Parameters.AddWithValue("@planid", vm.AwardPlanID);
                else
                    cmd.Parameters.AddWithValue("@planid", DBNull.Value);
                if (vm.QuizID.HasValue)
                    cmd.Parameters.AddWithValue("@qid", vm.QuizID);
                else
                    cmd.Parameters.AddWithValue("@planid", DBNull.Value);
                if (String.IsNullOrEmpty(vm.UsedReason))
                    cmd.Parameters.AddWithValue("@used", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@used", vm.UsedReason);
                if (vm.Publish.HasValue)
                    cmd.Parameters.AddWithValue("@publish", vm.Publish.Value);
                else
                    cmd.Parameters.AddWithValue("@publish", DBNull.Value);
                cmd.Parameters.AddWithValue("@aid", id);

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

            return new JsonResult(vm);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
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
            else
                return BadRequest("No user info found");

            try
            {
                await conn.OpenAsync();

                // Check the authority
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [award] LIKE '%D%'";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32(0) > 0)
                        {
                            bAllow = true;
                            break;
                        }
                    }
                }

                reader.Dispose();
                reader = null;
                cmd.Dispose();
                cmd = null;

                if (!bAllow)
                {
                    return BadRequest("No authority to delete plan");
                }

                queryString = @"DELETE FROM [dbo].[useraward] WHERE [aid] = @aid;";

                cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@aid", id);

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

            return Ok();
        }
    }
}
