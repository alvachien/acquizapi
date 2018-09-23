﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using acquizapi.Models;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Authorization;
using System.Net;

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
            SqlConnection conn = null;
            SqlCommand cmd = null;
            SqlDataReader reader = null;
            HttpStatusCode errorCode = HttpStatusCode.OK;
            String strErrMsg = "";

            try
            {
                String queryString = @"SELECT quiz.quizid, quiz.quiztype, quiz.attenduser, 
                    SUM(quizsection.timespent) as timespent
	                FROM quiz LEFT OUTER JOIN quizsection
	                ON quiz.quizid = quizsection.quizid WHERE quiz.attenduser = @user ";
                if (dtBegin.HasValue)
                    queryString += " AND [quiz].[submitdate] >= @begindate ";
                if (dtEnd.HasValue)
                    queryString += " AND [quiz].[submitdate] <= @enddate ";
                queryString += @" GROUP BY quiz.quizid, quiz.quiztype, quiz.attenduser;";

                using (conn = new SqlConnection(Startup.DBConnectionString))
                {
                    await conn.OpenAsync();

                    cmd = new SqlCommand(queryString, conn);
                    cmd.Parameters.AddWithValue("@user", usrid);
                    if (dtBegin.HasValue)
                        cmd.Parameters.AddWithValue("@begindate", dtBegin.Value);
                    if (dtEnd.HasValue)
                        cmd.Parameters.AddWithValue("@enddate", dtEnd.Value);
                    reader = await cmd.ExecuteReaderAsync();

                    this.GetDBResult(reader, listRst);
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

            return new JsonResult(listRst, setting);
        }

        // GET: api/StatisticQuizTime/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return BadRequest("Unsupported");
        }
        
        // POST: api/StatisticQuizTime
        [HttpPost]
        public IActionResult Post([FromBody]string value)
        {
            return BadRequest("UnSupported");
        }
        
        // PUT: api/StatisticQuizTime/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody]string value)
        {
            return BadRequest("UnSupported");
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            return BadRequest("UnSupported");
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
    }
}
