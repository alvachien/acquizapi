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
    [Route("api/AwardPlan")]
    [Authorize]
    public class AwardPlanController : Controller
    {
        // GET: api/AwardPlan
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] String tgtuser = null, [FromQuery]String crtedby = null, [FromQuery]Boolean incInvalid = false)
        {
            List<AwardPlan> listRst = new List<AwardPlan>();
            String strErrMsg = "";
            HttpStatusCode errorCode = HttpStatusCode.OK;
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                String queryString = @"SELECT [planid],[tgtuser],[createdby],[validfrom],[validto],[quiztype],[quizcontrol],[minscore],[minavgtime],[award] FROM [dbo].[awardplan] ";
                if (!String.IsNullOrEmpty(crtedby) && String.IsNullOrEmpty(tgtuser))
                {
                    queryString += " WHERE [createdby] = N'" + crtedby + "'";
                }
                else if (String.IsNullOrEmpty(crtedby) && !String.IsNullOrEmpty(tgtuser))
                {
                    queryString += " WHERE [tgtuser] = N'" + tgtuser + "'";
                }
                else if (!String.IsNullOrEmpty(crtedby) && !String.IsNullOrEmpty(tgtuser))
                {
                    queryString += " WHERE [tgtuser] = N'" + tgtuser + "' AND [createdby] = N'" + crtedby + "'";
                }
                if (!incInvalid)
                {
                    if (!String.IsNullOrEmpty(crtedby) || !String.IsNullOrEmpty(tgtuser))
                    {
                        queryString += " AND ";
                    }
                    else
                    {
                        queryString += " WHERE ";
                    }

                    queryString += " [validfrom] <= GETDATE() AND [validto] >= GETDATE() ";
                }

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            AwardPlan ap = new AwardPlan
                            {
                                PlanID = reader.GetInt32(0),
                                TargetUser = reader.GetString(1)
                            };
                            if (!reader.IsDBNull(2))
                                ap.CreatedBy = reader.GetString(2);
                            else
                                ap.CreatedBy = String.Empty;
                            ap.ValidFrom = reader.GetDateTime(3);
                            ap.ValidTo = reader.GetDateTime(4);
                            ap.QuizType = (QuizTypeEnum)reader.GetInt16(5);
                            if (!reader.IsDBNull(6))
                                ap.QuizControl = reader.GetString(6);
                            if (!reader.IsDBNull(7))
                                ap.MinQuizScore = reader.GetInt32(7);
                            if (!reader.IsDBNull(8))
                                ap.MaxQuizAvgTime = reader.GetInt32(8);
                            ap.Award = reader.GetInt32(9);
                            listRst.Add(ap);
                        }
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

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            
            return new JsonResult(listRst, setting);
        }

        // GET: api/AwardPlan/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            AwardPlan objRst = new AwardPlan();
            String strErrMsg = "";
            HttpStatusCode errorCode = HttpStatusCode.OK;

            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;

            try
            {
                String queryString = @"SELECT [planid],[tgtuser],[createdby],[validfrom],[validto],[quiztype],[quizcontrol],[minscore],[minavgtime],[award] FROM [dbo].[awardplan] WHERE [planid] = @pid;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@pid", id);
                    reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            objRst.PlanID = reader.GetInt32(0);
                            objRst.TargetUser = reader.GetString(1);
                            if (!reader.IsDBNull(2))
                                objRst.CreatedBy = reader.GetString(2);
                            else
                                objRst.CreatedBy = String.Empty;
                            objRst.ValidFrom = reader.GetDateTime(3);
                            objRst.ValidTo = reader.GetDateTime(4);
                            objRst.QuizType = (QuizTypeEnum)reader.GetInt16(5);
                            if (!reader.IsDBNull(6))
                                objRst.QuizControl = reader.GetString(6);
                            if (!reader.IsDBNull(7))
                                objRst.MinQuizScore = reader.GetInt32(7);
                            if (!reader.IsDBNull(8))
                                objRst.MaxQuizAvgTime = reader.GetInt32(8);
                            objRst.Award = reader.GetInt32(9);
                            break;
                        }
                    }
                    else
                    {
                        errorCode = HttpStatusCode.NotFound;
                        throw new Exception();
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

            return new JsonResult(objRst, setting);
        }

        // POST: api/AwardPlan
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AwardPlan ap)
        {
            if (ap == null)
            {
                return BadRequest();
            }

            // Update the database
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            String queryString = "";
            Boolean bError = false;
            String strErrMsg = "";
            HttpStatusCode errorCode = HttpStatusCode.OK;

            // Get user name
            var usr = User.FindFirst(c => c.Type == "sub");
            String usrName = String.Empty;
            if (usr != null)
                usrName = usr.Value;
            else
                return BadRequest();

#if DEBUG
            // Just skip this check in debug mode
#else
            if (String.IsNullOrEmpty(ap.CreatedBy))
            {
                if (String.CompareOrdinal(usrName, ap.TargetUser) == 0)
                {
                    return BadRequest("Cannot create an plan for yourself");
                }
            }
            else
            {
                if (String.CompareOrdinal(ap.CreatedBy, ap.TargetUser) == 0)
                {
                    return BadRequest("Cannot create an plan for yourself");
                }
            }
#endif

            try
            {
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%C%'";
                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    // Check the authority
                    Boolean bAllow = false;
                    cmd = new SqlCommand(queryString, conn);
                    reader = await cmd.ExecuteReaderAsync();

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
                        return BadRequest("No authority to create plan");
                    }

                    queryString = @"INSERT INTO [dbo].[awardplan] ([tgtuser],[createdby],[validfrom],[validto],[quiztype],[quizcontrol],[minscore],[minavgtime],[award])
                    VALUES(@tgtuser, @createdby, @validfrom, @validto, @quiztype, @quizcontrol, @minscore, @minavgtime, @award);
                    SELECT @Identity = SCOPE_IDENTITY();";

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@tgtuser", ap.TargetUser);
                    if (String.IsNullOrEmpty(ap.CreatedBy))
                    {
                        cmd.Parameters.AddWithValue("@createdby", usrName);
                    }
                    else
                        cmd.Parameters.AddWithValue("@createdby", ap.CreatedBy);
                    cmd.Parameters.AddWithValue("@validfrom", ap.ValidFrom);
                    cmd.Parameters.AddWithValue("@validto", ap.ValidTo);
                    cmd.Parameters.AddWithValue("@quiztype", ap.QuizType);
                    if (!String.IsNullOrEmpty(ap.QuizControl))
                        cmd.Parameters.AddWithValue("@quizcontrol", ap.QuizControl);
                    else
                        cmd.Parameters.AddWithValue("@quizcontrol", DBNull.Value);
                    if (ap.MinQuizScore.HasValue)
                        cmd.Parameters.AddWithValue("@minscore", ap.MinQuizScore.Value);
                    else
                        cmd.Parameters.AddWithValue("@minscore", DBNull.Value);
                    if (ap.MaxQuizAvgTime.HasValue)
                        cmd.Parameters.AddWithValue("@minavgtime", ap.MaxQuizAvgTime.Value);
                    else
                        cmd.Parameters.AddWithValue("@minavgtime", DBNull.Value);
                    cmd.Parameters.AddWithValue("@award", ap.Award);
                    SqlParameter idparam = cmd.Parameters.AddWithValue("@Identity", SqlDbType.Int);
                    idparam.Direction = ParameterDirection.Output;

                    Int32 nRst = await cmd.ExecuteNonQueryAsync();
                    ap.PlanID = (Int32)idparam.Value;
                    cmd.Dispose();
                    cmd = null;
                }
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                bError = true;
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

            if (bError)
            {
                return StatusCode(500, strErrMsg);
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

            return new JsonResult(ap);
        }

        // PUT: api/AwardPlan/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]AwardPlan ap)
        {
            if (ap == null || ap.PlanID != id)
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
            else
                return BadRequest("No user info found");

            try
            {
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%U%'";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    // Check the authority
                    cmd = new SqlCommand(queryString, conn);
                    reader = await cmd.ExecuteReaderAsync();
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
                        errorCode = HttpStatusCode.BadRequest;
                        throw new Exception("No authority to create plan");
                    }

                    queryString = @"UPDATE [dbo].[awardplan]
                                    SET [tgtuser] = @tgtuser
                                      ,[createdby] = @createdby
                                      ,[validfrom] = @validfrom
                                      ,[validto] = @validto
                                      ,[quiztype] = @quiztype
                                      ,[quizcontrol] = @quizcontrol
                                      ,[minscore] = @minscore
                                      ,[minavgtime] = @minavgtime
                                      ,[award] = @award
                                    WHERE [planid] = @planid;";

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@tgtuser", ap.TargetUser);
                    if (String.IsNullOrEmpty(ap.CreatedBy))
                        cmd.Parameters.AddWithValue("@createdby", DBNull.Value);
                    else
                        cmd.Parameters.AddWithValue("@createdby", ap.CreatedBy);
                    cmd.Parameters.AddWithValue("@validfrom", ap.ValidFrom);
                    cmd.Parameters.AddWithValue("@validto", ap.ValidTo);
                    cmd.Parameters.AddWithValue("@quiztype", ap.QuizType);
                    if (!String.IsNullOrEmpty(ap.QuizControl))
                        cmd.Parameters.AddWithValue("@quizcontrol", ap.QuizControl);
                    else
                        cmd.Parameters.AddWithValue("@quizcontrol", DBNull.Value);
                    if (ap.MinQuizScore.HasValue)
                        cmd.Parameters.AddWithValue("@minscore", ap.MinQuizScore.Value);
                    else
                        cmd.Parameters.AddWithValue("@minscore", DBNull.Value);
                    if (ap.MaxQuizAvgTime.HasValue)
                        cmd.Parameters.AddWithValue("@minavgtime", ap.MaxQuizAvgTime.Value);
                    else
                        cmd.Parameters.AddWithValue("@minavgtime", DBNull.Value);
                    cmd.Parameters.AddWithValue("@award", ap.Award);
                    cmd.Parameters.AddWithValue("@planid", id);

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

            return new JsonResult(ap);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
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
            else
                return BadRequest("No user info found");

            try
            {
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%C%'";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    // Check the authority
                    cmd = new SqlCommand(queryString, conn);
                    reader = await cmd.ExecuteReaderAsync();
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
                        errorCode = HttpStatusCode.BadRequest;
                        throw new Exception("No authority to delete plan");
                    }

                    queryString = @"DELETE FROM[dbo].[awardplan] WHERE [planid] = @planid;";

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@planid", id);

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

            return Ok();
        }
    }
}
