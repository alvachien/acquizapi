using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace acquizapi.Models
{
    public enum QuizTypeEnum: Int16
    {
        add         = 1,
        sub         = 2,
        multi       = 3,
        div         = 4,
        formula     = 5,
        cal24       = 6,
        sudou       = 7,
        typing      = 8,
        mixedop     = 9,
        minesweep   = 10
    }

    public sealed class QuizSection
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
        [StringLength(250)]
        public String Expected { get; set; }
        [StringLength(250)]
        public String Inputted { get; set; }
    }

    public sealed class Quiz
    {
        public Quiz()
        {
            this.FailLogs = new List<QuizFailLog>();
            this.Sections = new List<QuizSection>();
        }

        public Int32 QuizID { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        [StringLength(250)]
        public String BasicInfo { get; set; }
        [StringLength(50)]
        public String AttendUser { get; set; }
        public DateTime SubmitDate { get; set; }

        public List<QuizFailLog> FailLogs { get; private set; }
        public List<QuizSection> Sections { get; private set; }

        // Runtime information
        public Int32 TotalScore
        {
            get
            {
                Int32 totalItems = 0;
                Int32 totalSucc = 0;
                foreach(QuizSection qs in this.Sections)
                {
                    totalItems += qs.TotalItems;
                    totalSucc += (qs.TotalItems - qs.FailedItems);
                }

                return (Int32)(100 * totalSucc / totalItems);
            }
        }

        public Int32 TotalAverageTime
        {
            get
            {
                Int32 totalTimespt = 0;
                Int32 totalItems = 0;
                foreach (QuizSection qs in this.Sections)
                {
                    totalItems += qs.TotalItems;
                    totalTimespt += qs.TimeSpent;                    
                }

                return (Int32)(totalTimespt / totalItems);
            }
        }
    }

    public sealed class QuizCreateResult
    {
        public Int32 QuizID { get; set; }
        public Int32 TotalAwardPoint { get; set; }
        public List<Int32> AwardIDList { get; private set; }

        public QuizCreateResult()
        {
            this.AwardIDList = new List<int>();
        }
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
        public QuizTypeEnum QuizType { get; set; }
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
        public QuizTypeEnum QuizType { get; set; }
    }

    public sealed class QuizSucceedRateStatistics
    {
        public Int32 QuizID { get; set; }
        public String AttendUser { get; set; }
        public String AttendUserDisplayAs { get; set; }
        //public DateTime SubmitDate { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        public float SucceedRate { get; set; }
    }

    public sealed class QuizTimeStatistics
    {
        public Int32 QuizID { get; set; }
        public String AttendUser { get; set; }
        public String AttendUserDisplayAs { get; set; }
        //public DateTime SubmitDate { get; set; }
        public QuizTypeEnum QuizType { get; set; }
        public float TimeSpent { get; set; }
    }

    public sealed class QuizAttendUser
    {
        [StringLength(50)]
        public String AttendUser { get; set; }
        [StringLength(50)]
        public String DisplayAs { get; set; }
    }

    public sealed class UserDetail
    {
        [StringLength(50)]
        public String UserID { get; set; }
        [StringLength(50)]
        public String DisplayAs { get; set; }
        [StringLength(50)]
        public String Others { get; set; }
        [StringLength(5)]
        public String AwardControl { get; set; }
        [StringLength(5)]
        public String AwardPlanControl { get; set; }
    }

    public sealed class AwardPlan
    {
        public Int32 PlanID { get; set; }
        [StringLength(50)]
        public String TargetUser { get; set; }
        [StringLength(50)]
        public String CreatedBy { get; set; }
        [Required]
        public DateTime ValidFrom { get; set; }
        [Required]
        public DateTime ValidTo { get; set; }
        [Required]
        public QuizTypeEnum QuizType { get; set; }
        [StringLength(250)]
        public String QuizControl { get; set; }
        public Int32? MinQuizScore { get; set; }
        public Int32? MaxQuizAvgTime { get; set; }
        [Required]
        public Int32 Award { get; set; }
    }

    public sealed class UserAward
    {
        public Int32 AwardID { get; set; }
        [Required]
        [StringLength(50)]
        public String UserID { get; set; }
        [Required]
        public DateTime AwardDate { get; set; }
        [Required]
        public Int32 Award { get; set; }
        public Int32? AwardPlanID { get; set; }
        public QuizTypeEnum? QuizType { get; set; }
        public Int32? QuizID { get; set; }
        [StringLength(50)]
        public String UsedReason { get; set; }
        public Boolean? Punish { get; set; }
    }
}
