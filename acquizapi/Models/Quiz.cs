using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace acquizapi.Models
{
    public class QuizSection
    {
        public Int32 QuizID { get; set; }
        public Int32 SectionID { get; set; }
        public Int32 TimeSpent { get; set; }
        public Int32 TotalItems { get; set; }
        public Int32 FailedItems { get; set; }
    }

    public class QuizFailLog
    {
        public Int32 QuizID { get; set; }
        public Int32 QuizFailIndex { get; set; }
        [StringLength(50)]
        public String Expected { get; set; }
        [StringLength(50)]
        public String Inputted { get; set; }
    }

    public class Quiz
    {
        public Quiz()
        {
            this.FailLogs = new List<QuizFailLog>();
            this.Sections = new List<QuizSection>();
        }

        public Int32 QuizID { get; set; }
        public Int16 QuizType { get; set; }
        [StringLength(50)]
        public String BasicInfo { get; set; }
        [StringLength(50)]
        public String AttendUser { get; set; }
        public DateTime SubmitDate { get; set; }

        public List<QuizFailLog> FailLogs { get; private set; }
        public List<QuizSection> Sections { get; private set; }
    }

    public sealed class QuizFailureRetry: QuizFailLog
    {
        public Int16 QuizType { get; set; }
        [StringLength(50)]
        public String AttendUser { get; set; }
        public DateTime SubmitDate { get; set; }
    }

    public abstract class QuizAmountStatistics
    {
        [StringLength(50)]
        public String AttendUser { get; set; }
        public Int32 Amount { get; set; }
    }

    public sealed class QuizAmountByDateStatistics: QuizAmountStatistics
    {
        public DateTime QuizDate { get; set; }
    }

    public sealed class QuizAmountByTypeStatistics: QuizAmountStatistics
    {
        public Int16 QuizType { get; set; }
    }

    public abstract class QuizItemAmountStatistics
    {
        [StringLength(50)]
        public String AttendUser { get; set; }
        public Int32 TotalAmount { get; set; }
        public Int32 FailedAmount { get; set; }
    }

    public sealed class QuizItemAmountByDateStatistics: QuizItemAmountStatistics
    {
        public DateTime QuizDate { get; set; }
    }

    public sealed class QuizItemAmountByTypeStatistics: QuizItemAmountStatistics
    {
        public Int16 QuizType { get; set; }
    }
}
