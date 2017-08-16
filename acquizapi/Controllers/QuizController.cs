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
    [Route("api/quiz")]
    [Authorize]
    public class QuizController : Controller
    {
        // GET: api/quiz
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            List<Quiz> listRst = new List<Quiz>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [quizid],[quiztype],[attenduser],[submitdate] FROM [dbo].[quiz] ORDER BY [quizid] ASC;
                                SELECT [quizid],[failidx],[expected],[inputted] FROM [dbo].[quizfaillog] ORDER BY [quizid] ASC;
                                SELECT [quizid],[section],[timespent],[totalitems],[faileditems] FROM [dbo].[quizsection] ORDER BY [quizid] ASC;";

                this.GetDBResult(queryString, conn, null, listRst);
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

        // GET: api/quiz/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<IActionResult> Get(int id)
        {
            List<Quiz> listRst = new List<Quiz>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);

            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [quizid],[quiztype],[attenduser],[submitdate] FROM [dbo].[quiz] WHERE [quizid] = @qid;
                                SELECT [quizid],[failidx],[expected],[inputted] FROM [dbo].[quizfaillog] WHERE [quizid] = @qid;
                                SELECT [quizid],[section],[timespent],[totalitems],[faileditems] FROM [dbo].[quizsection] WHERE [quizid] = @qid;";

                this.GetDBResult(queryString, conn, id, listRst);
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
            if (listRst.Count <= 0)
            {
                return NotFound();
            }

            var setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.DateFormatString = "yyyy-MM-dd";
            setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(); ;
            return new JsonResult(listRst[0], setting);
        }

        // POST: api/quiz
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]Quiz qz)
        {
            if (qz == null)
            {
                return BadRequest();
            }

            // Update the database
            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            String queryString = "";
            Int32 nNewID = -1;
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
                queryString = @"INSERT INTO [dbo].[quiz] ([quiztype],[attenduser],[submitdate]) VALUES (@quiztype, @attenduser, @submitdate); SELECT @Identity = SCOPE_IDENTITY();";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@quiztype", qz.QuizType);
                cmd.Parameters.AddWithValue("@attenduser", usrName);
                cmd.Parameters.AddWithValue("@submitdate", qz.SubmitDate);
                SqlParameter idparam = cmd.Parameters.AddWithValue("@Identity", SqlDbType.Int);
                idparam.Direction = ParameterDirection.Output;

                Int32 nRst = await cmd.ExecuteNonQueryAsync();
                nNewID = (Int32)idparam.Value;
                cmd.Dispose();
                cmd = null;

                // Section
                foreach (QuizSection sect in qz.Sections)
                {
                    queryString = @"INSERT INTO [dbo].[quizsection]([quizid],[section],[timespent],[totalitems],[faileditems]) VALUES(@quizid, @section, @timespent,@totalitems,@faileditems);";
                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@quizid", nNewID);
                    cmd.Parameters.AddWithValue("@section", sect.SectionID);
                    cmd.Parameters.AddWithValue("@timespent", sect.TimeSpent);
                    cmd.Parameters.AddWithValue("@totalitems", sect.TotalItems);
                    cmd.Parameters.AddWithValue("@faileditems", sect.FailedItems);

                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                    cmd = null;
                }

                // Failed log
                foreach (QuizFailLog fl in qz.FailLogs)
                {
                    queryString = @"INSERT INTO [dbo].[quizfaillog]([quizid],[failidx],[expected],[inputted]) VALUES(@quizid,@failidx,@expected,@inputted);";
                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@quizid", nNewID);
                    cmd.Parameters.AddWithValue("@failidx", fl.QuizFailIndex);
                    cmd.Parameters.AddWithValue("@expected", fl.Expected);
                    cmd.Parameters.AddWithValue("@inputted", fl.Inputted);

                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                    cmd = null;
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

            qz.QuizID = nNewID;
            var setting = new Newtonsoft.Json.JsonSerializerSettings();
            setting.DateFormatString = "yyyy-MM-dd";
            setting.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver(); ;
            return new JsonResult(qz, setting);
        }

        // PUT: api/quiz/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
            return BadRequest();
        }

        // DELETE: api/quiz/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return BadRequest();
        }

        // Get DB result via SQL
        private void GetDBResult(String queryString, SqlConnection conn, Int32? id, List<Quiz> listRst)
        {
            SqlCommand cmd = new SqlCommand(queryString, conn);
            if (id.HasValue)
            {
                cmd.Parameters.AddWithValue("@qid", id);
            }
            SqlDataReader reader = cmd.ExecuteReader();

            Int32 nRstBatch = 0;
            if (nRstBatch == 0)
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Quiz qz = new Quiz();
                        qz.QuizID = reader.GetInt32(0);
                        qz.QuizType = reader.GetInt16(1);
                        qz.AttendUser = reader.GetString(2);
                        qz.SubmitDate = reader.GetDateTime(3);
                        listRst.Add(qz);
                    }
                }
                ++nRstBatch;

                reader.NextResult();
            }

            if (nRstBatch == 1)
            {
                if (reader.HasRows)
                {
                    List<QuizFailLog> listLogs = new List<QuizFailLog>();

                    while (reader.Read())
                    {
                        QuizFailLog fl = new QuizFailLog();
                        fl.QuizID = reader.GetInt32(0);
                        fl.QuizFailIndex = reader.GetInt32(1);
                        fl.Expected = reader.GetString(2);
                        fl.Inputted = reader.GetString(3);
                        listLogs.Add(fl);
                    }

                    foreach (Quiz qz in listRst)
                    {
                        foreach (QuizFailLog fl in listLogs)
                        {
                            if (qz.QuizID == fl.QuizID)
                            {
                                qz.FailLogs.Add(fl);
                            }
                        }
                    }
                    listLogs.Clear();
                    listLogs = null;
                }
                ++nRstBatch;

                reader.NextResult();
            }

            if (nRstBatch == 2)
            {
                if (reader.HasRows)
                {
                    List<QuizSection> listSect = new List<QuizSection>();
                    while (reader.Read())
                    {
                        QuizSection qs = new QuizSection();
                        qs.QuizID = reader.GetInt32(0);
                        qs.SectionID = reader.GetInt32(1);
                        qs.TimeSpent = reader.GetInt32(2);
                        qs.TotalItems = reader.GetInt32(3);
                        qs.FailedItems = reader.GetInt32(4);
                        listSect.Add(qs);
                    }

                    foreach (Quiz qz in listRst)
                    {
                        foreach (QuizSection qs in listSect)
                        {
                            if (qz.QuizID == qs.QuizID)
                            {
                                qz.Sections.Add(qs);
                            }
                        }
                    }
                    listSect.Clear();
                    listSect = null;
                }
            }
        }
    }
}
