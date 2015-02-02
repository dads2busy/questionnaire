using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class QuestionnaireAppData
    {
        public Questionnaire Questionnaire { get; set; }
        public int UserId { get; set; }
        public ICollection<QuestionnaireAppRecord> AppRecords { get; set; }
        public IList<Response> Responses { get; set; }
        public IList<QCategory> QCategories { get; set; }
    }

    public class QuestionnaireAppRecord
    {
        public QCategory QCategory { get; set; }
        public QuestionnaireQuestion QuestionnaireQuestion { get; set; }
        public Question Question { get; set; }
        public SelectList Answers { get; set; }
        public QType QType { get; set; }
        public int Ordinal { get; set; }
        public int SubOrdinal { get; set; }
        public string Response { get; set; }

        public QuestionnaireAppRecord(){}

        public QuestionnaireAppRecord(QCategory QCategory, QuestionnaireQuestion QuestionnaireQuestion, Question Question, SelectList Answers, QType QType, int Ordinal, int SubOrdinal, string Response)
        {
            this.QCategory = QCategory;
            this.QuestionnaireQuestion = QuestionnaireQuestion;
            this.Question = Question;
            this.Answers = Answers;
            this.QType = QType;
            this.Ordinal = Ordinal;
            this.SubOrdinal = SubOrdinal;
            this.Response = Response;
        }
    }
}