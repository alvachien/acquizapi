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
    [Route("api/AwardPlan")]
    [Authorize]
    public class AwardPlanController : Controller
    {
        // GET: api/AwardPlan
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] String tgtuser = null, [FromQuery]String crtedby = null)
        {
            List<AwardPlan> listRst = new List<AwardPlan>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [planid],[tgtuser],[createdby],[validfrom],[validto],[quiztype],[minscore],[minavgtime],[award] FROM [dbo].[awardplan] ";
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


                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = cmd.ExecuteReader();

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
                        ap.QuizType = reader.GetInt16(5);
                        if (!reader.IsDBNull(6))
                            ap.MinQuizScore = reader.GetInt32(6);
                        if (!reader.IsDBNull(7))
                            ap.MinQuizAvgTime = reader.GetInt32(7);
                        ap.Award = reader.GetInt32(8);
                        listRst.Add(ap);
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

        // GET: api/AwardPlan/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            AwardPlan objRst = new AwardPlan();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [planid],[tgtuser],[createdby],[validfrom],[validto],[quiztype],[minscore],[minavgtime],[award] FROM [dbo].[awardplan] WHERE [planid] = @pid;";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@pid", id);
                SqlDataReader reader = cmd.ExecuteReader();

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
                        objRst.QuizType = reader.GetInt16(5);
                        if (!reader.IsDBNull(6))
                            objRst.MinQuizScore = reader.GetInt32(6);
                        if (!reader.IsDBNull(7))
                            objRst.MinQuizAvgTime = reader.GetInt32(7);
                        objRst.Award = reader.GetInt32(8);
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

        // POST: api/AwardPlan
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]AwardPlan ap)
        {
            if (ap == null)
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
                await conn.OpenAsync();

                // Check the authority
                Boolean bAllow = false;
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%C%'";
                SqlCommand cmd = new SqlCommand(queryString, conn);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if(reader.HasRows)
                {
                    while(reader.Read())
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

                queryString = @"INSERT INTO [dbo].[awardplan] ([tgtuser],[createdby],[validfrom],[validto],[quiztype],[minscore],[minavgtime],[award])
                    VALUES(@tgtuser, @createdby, @validfrom, @validto, @quiztype, @minscore, @minavgtime, @award);
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
                if (ap.MinQuizScore.HasValue)
                    cmd.Parameters.AddWithValue("@minscore", ap.MinQuizScore.Value);
                else
                    cmd.Parameters.AddWithValue("@minscore", DBNull.Value);
                if (ap.MinQuizAvgTime.HasValue)
                    cmd.Parameters.AddWithValue("@minavgtime", ap.MinQuizAvgTime.Value);
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
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%U%'";
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
                    return BadRequest("No authority to create plan");
                }

                queryString = @"UPDATE [dbo].[awardplan]
                    SET [tgtuser] = @tgtuser
                      ,[createdby] = @createdby
                      ,[validfrom] = @validfrom
                      ,[validto] = @validto
                      ,[quiztype] = @quiztype
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
                if (ap.MinQuizScore.HasValue)
                    cmd.Parameters.AddWithValue("@minscore", ap.MinQuizScore.Value);
                else
                    cmd.Parameters.AddWithValue("@minscore", DBNull.Value);
                if (ap.MinQuizAvgTime.HasValue)
                    cmd.Parameters.AddWithValue("@minavgtime", ap.MinQuizAvgTime.Value);
                else
                    cmd.Parameters.AddWithValue("@minavgtime", DBNull.Value);
                cmd.Parameters.AddWithValue("@award", ap.Award);
                cmd.Parameters.AddWithValue("@planid", id);

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

            return new JsonResult(ap);
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
                queryString = @"SELECT COUNT(*) AS COUNT FROM [quizuser] WHERE [userid] = N'" + usrName + "' AND [awardplan] LIKE '%C%'";
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

                queryString = @"DELETE FROM[dbo].[awardplan] WHERE [planid] = @planid;";

                cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@planid", id);

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
