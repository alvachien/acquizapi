using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;

namespace acquizapi.Controllers
{
    [Produces("application/json")]
    [Route("api/StatisticQuizTime")]
    [Authorize]
    public class StatisticQuizTimeController : Controller
    {
        // GET: api/StatisticQuizTime
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]String usrid, DateTime? dtBegin = null, DateTime? dtEnd = null)
        {
            if (String.IsNullOrEmpty(usrid))
                return BadRequest("User is must!");

            List<QuizTimeStatistics> listRst = new List<QuizTimeStatistics>();

            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();

                String queryString = @"SELECT quiz.quizid, quiz.quiztype, quiz.attenduser, 
                    SUM(quizsection.timespent) as timespent
	                FROM quiz LEFT OUTER JOIN quizsection
	                ON quiz.quizid = quizsection.quizid WHERE quiz.attenduser = @user ";
                if (dtBegin.HasValue)
                    queryString += " AND [quiz].[submitdate] >= @begindate ";
                if (dtEnd.HasValue)
                    queryString += " AND [quiz].[submitdate] <= @enddate ";
                queryString += @" GROUP BY quiz.quizid, quiz.quiztype, quiz.attenduser;";

                SqlCommand cmd = new SqlCommand(queryString, conn);
                cmd.Parameters.AddWithValue("@user", usrid);
                if (dtBegin.HasValue)
                    cmd.Parameters.AddWithValue("@begindate", dtBegin.Value);
                if (dtEnd.HasValue)
                    cmd.Parameters.AddWithValue("@enddate", dtEnd.Value);
                SqlDataReader reader = await cmd.ExecuteReaderAsync();

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

        private void GetDBResult(SqlDataReader reader, List<QuizTimeStatistics> listRst)
        {
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    QuizTimeStatistics row = new QuizTimeStatistics();
                    row.QuizID = reader.GetInt32(0);
                    row.QuizType = (QuizTypeEnum)reader.GetInt16(1);
                    //row.SubmitDate = reader.GetDateTime(2);
                    row.AttendUser = reader.GetString(2);
                    row.TimeSpent = reader.GetInt32(3);
                    listRst.Add(row);

                }
            }
        }

        // GET: api/StatisticQuizTime/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return BadRequest("Unsupported");
        }
        
        // POST: api/StatisticQuizTime
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            return BadRequest("UnSupported");
        }
        
        // PUT: api/StatisticQuizTime/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody]string value)
        {
            return BadRequest("UnSupported");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            return BadRequest("UnSupported");
        }
    }
}
