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
        public async Task<IActionResult> Get([FromQuery]String usrid, DateTime? dtBegin = null, DateTime? dtEnd = null)
        {
            List<Quiz> listRst = new List<Quiz>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();
                String queryString = String.Empty;
                SqlDataReader reader = null;
                if (String.IsNullOrEmpty(usrid))
                {
                    queryString = @"SELECT [quizid],[quiztype],[basicinfo],[attenduser],[submitdate] FROM [dbo].[quiz] ORDER BY [quizid] ASC;
                                SELECT [quizid],[failidx],[expected],[inputted] FROM [dbo].[quizfaillog] ORDER BY [quizid] ASC;
                                SELECT [quizid],[section],[timespent],[totalitems],[faileditems] FROM [dbo].[quizsection] ORDER BY [quizid] ASC;";

                    SqlCommand cmd = new SqlCommand(queryString, conn);
                    reader = await cmd.ExecuteReaderAsync();
                }
                else
                {
                    if (dtBegin.HasValue && dtEnd.HasValue)
                    {
                        queryString = @"SELECT [quizid],[quiztype],[basicinfo],[attenduser],[submitdate] FROM [quiz] WHERE [attenduser] = @usrid AND [submitdate] <= @enddate AND [submitdate] >= @begindate ORDER BY [quizid] ASC;"
                                    + @"SELECT [quizfaillog].[quizid],[failidx],[expected],[inputted] FROM [quizfaillog] INNER JOIN [quiz] ON [quizfaillog].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid AND [quiz].[submitdate] <= @enddate AND [quiz].[submitdate] >= @begindate ORDER BY [quizfaillog].[quizid] ASC;"
                                    + @"SELECT [quizsection].[quizid],[section],[timespent],[totalitems],[faileditems] FROM [quizsection] INNER JOIN [quiz] ON [quizsection].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid AND [quiz].[submitdate] <= @enddate AND [quiz].[submitdate] >= @begindate ORDER BY [quizsection].[quizid] ASC;";
                        SqlCommand cmd = new SqlCommand(queryString, conn);
                        cmd.Parameters.AddWithValue("@usrid", usrid);
                        cmd.Parameters.AddWithValue("@begindate", dtBegin.Value);
                        cmd.Parameters.AddWithValue("@enddate", dtEnd.Value);
                        reader = await cmd.ExecuteReaderAsync();
                    }
                    else if (dtBegin.HasValue && !dtEnd.HasValue)
                    {
                        queryString = @"SELECT [quizid],[quiztype],[basicinfo],[attenduser],[submitdate] FROM [quiz] WHERE [attenduser] = @usrid AND [submitdate] >= @begindate ORDER BY [quizid] ASC;"
                                    + @"SELECT [quizfaillog].[quizid],[failidx],[expected],[inputted] FROM [quizfaillog] INNER JOIN [quiz] ON [quizfaillog].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid AND [quiz].[submitdate] >= @begindate ORDER BY [quizfaillog].[quizid] ASC;"
                                    + @"SELECT [quizsection].[quizid],[section],[timespent],[totalitems],[faileditems] FROM [quizsection] INNER JOIN [quiz] ON [quizsection].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid AND [quiz].[submitdate] >= @begindate ORDER BY [quizsection].[quizid] ASC;";
                        SqlCommand cmd = new SqlCommand(queryString, conn);
                        cmd.Parameters.AddWithValue("@usrid", usrid);
                        cmd.Parameters.AddWithValue("@begindate", dtBegin.Value);
                        reader = await cmd.ExecuteReaderAsync();
                    }
                    else
                    {
                        queryString = @"SELECT [quizid],[quiztype],[basicinfo],[attenduser],[submitdate] FROM [quiz] WHERE [attenduser] = @usrid ORDER BY [quizid] ASC;"
                                    + @"SELECT [quizfaillog].[quizid],[failidx],[expected],[inputted] FROM [quizfaillog] INNER JOIN [quiz] ON [quizfaillog].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid ORDER BY [quizfaillog].[quizid] ASC;"
                                    + @"SELECT [quizsection].[quizid],[section],[timespent],[totalitems],[faileditems] FROM [quizsection] INNER JOIN [quiz] ON [quizsection].[quizid] = [quiz].[quizid] WHERE [quiz].[attenduser] = @usrid ORDER BY [quizsection].[quizid] ASC;";
                        SqlCommand cmd = new SqlCommand(queryString, conn);
                        cmd.Parameters.AddWithValue("@usrid", usrid);
                        reader = await cmd.ExecuteReaderAsync();
                    }
                }


                this.GetDBResult(reader, listRst);
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

        // GET: api/quiz/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            List<Quiz> listRst = new List<Quiz>();
            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);

            try
            {
                await conn.OpenAsync();
                String queryString = @"SELECT [quizid],[quiztype],[basicinfo],[attenduser],[submitdate] FROM [dbo].[quiz] WHERE [quizid] = @qid;
                                SELECT [quizid],[failidx],[expected],[inputted] FROM [dbo].[quizfaillog] WHERE [quizid] = @qid;
                                SELECT [quizid],[section],[timespent],[totalitems],[faileditems] FROM [dbo].[quizsection] WHERE [quizid] = @qid;";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@qid", id);
                SqlDataReader reader = cmd.ExecuteReader();

                this.GetDBResult(reader, listRst);
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
            if (listRst.Count <= 0)
            {
                return NotFound();
            }

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            
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
            else
                return BadRequest();

            List<AwardPlan> listAPlans = new List<AwardPlan>();
            QuizCreateResult qcr = new QuizCreateResult();
            SqlCommand cmd = null;
            SqlTransaction tran = null;

            try
            {
                await conn.OpenAsync();

                // Read the award plan as the first setp
                queryString = @"SELECT [planid],[tgtuser],[createdby],[validfrom],[validto],[quiztype],[quizcontrol], [minscore],[minavgtime],[award] FROM [dbo].[awardplan] 
                    WHERE [tgtuser] = @tgtuser AND [quiztype] = @qtype AND @qdate >= [validfrom] AND @qdate <= [validto] ";
                cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@tgtuser", usrName);
                cmd.Parameters.AddWithValue("@qtype", qz.QuizType);
                cmd.Parameters.AddWithValue("@qdate", qz.SubmitDate);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();
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
                        listAPlans.Add(ap);
                    }
                }
                reader.Dispose();
                reader = null;
                cmd.Dispose();
                cmd = null;

                tran = conn.BeginTransaction();
                queryString = @"INSERT INTO [dbo].[quiz] ([quiztype],[basicinfo],[attenduser],[submitdate]) VALUES (@quiztype, @basicinfo, @attenduser, @submitdate); SELECT @Identity = SCOPE_IDENTITY();";

                cmd = new SqlCommand(queryString, conn)
                {
                    Transaction = tran
                };
                cmd.Parameters.AddWithValue("@quiztype", qz.QuizType);
                cmd.Parameters.AddWithValue("@basicinfo", qz.BasicInfo);
                cmd.Parameters.AddWithValue("@attenduser", usrName);
                cmd.Parameters.AddWithValue("@submitdate", qz.SubmitDate);
                SqlParameter idparam = cmd.Parameters.AddWithValue("@Identity", SqlDbType.Int);
                idparam.Direction = ParameterDirection.Output;

                Int32 nRst = await cmd.ExecuteNonQueryAsync();
                nNewID = (Int32)idparam.Value;
                qcr.QuizID = nNewID;
                cmd.Dispose();
                cmd = null;

                // Section
                foreach (QuizSection sect in qz.Sections)
                {
                    queryString = @"INSERT INTO [dbo].[quizsection]([quizid],[section],[timespent],[totalitems],[faileditems]) VALUES(@quizid, @section, @timespent,@totalitems,@faileditems);";
                    cmd = new SqlCommand(queryString, conn)
                    {
                        Transaction = tran
                    };
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
                    cmd = new SqlCommand(queryString, conn)
                    {
                        Transaction = tran
                    };
                    cmd.Parameters.AddWithValue("@quizid", nNewID);
                    cmd.Parameters.AddWithValue("@failidx", fl.QuizFailIndex);
                    cmd.Parameters.AddWithValue("@expected", fl.Expected);
                    cmd.Parameters.AddWithValue("@inputted", fl.Inputted);

                    await cmd.ExecuteNonQueryAsync();
                    cmd.Dispose();
                    cmd = null;
                }

                // Now, work for the award
                foreach(AwardPlan ap in listAPlans)
                {
                    if (!String.IsNullOrEmpty(ap.QuizControl))
                    {
                        if (String.IsNullOrEmpty(qz.BasicInfo) || String.CompareOrdinal(qz.BasicInfo, ap.QuizControl) != 0)
                        {
                            continue;
                        }
                    }

                    if (ap.MinQuizScore.HasValue)
                    {
                        if (qz.TotalScore < ap.MinQuizScore.Value)
                            continue;
                    }
                    if (ap.MaxQuizAvgTime.HasValue)
                    {
                        if (qz.TotalAverageTime > ap.MaxQuizAvgTime.Value)
                            continue;
                    }

                    queryString = @"INSERT INTO [dbo].[useraward] ([userid],[adate],[award],[planid],[qid],[used])
                                VALUES(@userid,@adate,@award,@planid,@qid, @used);
                                SELECT @Identity = SCOPE_IDENTITY();";

                    cmd = new SqlCommand(queryString, conn)
                    {
                        Transaction = tran
                    };
                    cmd.Parameters.AddWithValue("@userid", usrName);
                    cmd.Parameters.AddWithValue("@adate", qz.SubmitDate);
                    cmd.Parameters.AddWithValue("@award", ap.Award);
                    cmd.Parameters.AddWithValue("@planid", ap.PlanID);
                    cmd.Parameters.AddWithValue("@qid", nNewID);
                    cmd.Parameters.AddWithValue("@used", DBNull.Value);
                    SqlParameter idparam2 = cmd.Parameters.AddWithValue("@Identity", SqlDbType.Int);
                    idparam2.Direction = ParameterDirection.Output;

                    qcr.TotalAwardPoint += ap.Award;

                    nRst = await cmd.ExecuteNonQueryAsync();
                    qcr.AwardIDList.Add((Int32)idparam2.Value);
                    cmd.Dispose();
                    cmd = null;
                }

                // No errors!
                tran.Commit();
            }
            catch (Exception exp)
            {
                System.Diagnostics.Debug.WriteLine(exp.Message);
                bError = true;
                strErrMsg = exp.Message;

                if (tran != null)
                {
                    tran.Rollback();
                    tran.Dispose();
                    tran = null;
                }
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

            qz.QuizID = nNewID;

            var setting = new Newtonsoft.Json.JsonSerializerSettings
            {
                DateFormatString = "yyyy-MM-dd",
                ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver()
            };
            
            return new JsonResult(qcr, setting);
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
        private void GetDBResult(SqlDataReader reader, List<Quiz> listRst)
        {
            Int32 nRstBatch = 0;
            if (nRstBatch == 0)
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Quiz qz = new Quiz
                        {
                            QuizID = reader.GetInt32(0),
                            QuizType = (QuizTypeEnum)reader.GetInt16(1)
                        };
                        if (!reader.IsDBNull(2))
                        {
                            qz.BasicInfo = reader.GetString(2);
                        }
                        qz.AttendUser = reader.GetString(3);
                        qz.SubmitDate = reader.GetDateTime(4);
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
                        QuizFailLog fl = new QuizFailLog
                        {
                            QuizID = reader.GetInt32(0),
                            QuizFailIndex = reader.GetInt32(1),
                            Expected = reader.GetString(2),
                            Inputted = reader.GetString(3)
                        };
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
                        QuizSection qs = new QuizSection
                        {
                            QuizID = reader.GetInt32(0),
                            SectionID = reader.GetInt32(1),
                            TimeSpent = reader.GetInt32(2),
                            TotalItems = reader.GetInt32(3),
                            FailedItems = reader.GetInt32(4)
                        };
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
