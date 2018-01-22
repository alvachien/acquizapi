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
    [Route("api/StatisticQuizRate")]
    [Authorize]
    public class StatisticQuizRateController : Controller
    {
        // GET: api/StatisticQuizRate
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]String usrid, DateTime? dtBegin = null, DateTime? dtEnd = null)
        {
            if (String.IsNullOrEmpty(usrid))
                return BadRequest("User is must!");

            List<QuizSucceedRateStatistics> listRst = new List<QuizSucceedRateStatistics>();

            Boolean bError = false;
            String strErrMsg = "";

            SqlConnection conn = new SqlConnection(Startup.DBConnectionString);
            try
            {
                await conn.OpenAsync();

                String queryString = @"SELECT quiz.quizid, quiz.quiztype, quiz.submitdate, quiz.attenduser, 
                    SUM(quizsection.timespent) as timespent, 
                    SUM(quizsection.totalitems) as totalitems,
	                SUM(quizsection.faileditems) as faileditems
	                FROM quiz LEFT OUTER JOIN quizsection
	                ON quiz.quizid = quizsection.quizid
	                GROUP BY quiz.quizid, quiz.quiztype, quiz.submitdate, quiz.attenduser WHERE quiz.attenduser = @user ";
                if (dtBegin.HasValue)
                    queryString += " AND [quiz].[submitdate] >= @begindate ";
                if (dtEnd.HasValue)
                    queryString += " AND [quiz].[submitdate] <= @enddate ";

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

        private void GetDBResult(SqlDataReader reader, List<QuizSucceedRateStatistics> listRst)
        {
            if (reader.HasRows)
            {
                while(reader.Read())
                {
                    QuizSucceedRateStatistics row = new QuizSucceedRateStatistics();

                }
            }
        }

        // GET: api/StatisticQuizRate/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            return BadRequest("UnSupported");
        }

        // POST: api/StatisticQuizRate
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]string value)
        {
            return BadRequest("UnSupported");
        }

        // PUT: api/StatisticQuizRate/5
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
