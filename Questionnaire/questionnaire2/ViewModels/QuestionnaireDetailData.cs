using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Questionnaire2.Models;

namespace Questionnaire2.ViewModels
{
    public class QuestionnaireDetailData
    {
        public Questionnaire Questionnaire { get; set; }
        public ICollection<QuestionnaireDetailRecord> DetailRecords { get; set; }
    }

    public class QuestionnaireDetailRecord
    {
        public QCategory QCategory { get; set; }
        public Question Question { get; set; }
        public SelectList Answers { get; set; }
        public QType QType { get; set; }
        public int Ordinal { get; set; }

        public QuestionnaireDetailRecord(){}

        public QuestionnaireDetailRecord(QCategory QCategory, Question Question, SelectList Answers, QType QType, int Ordinal)
        {
            this.QCategory = QCategory;
            this.Question = Question;
            this.Answers = Answers;
            this.QType = QType;
            this.Ordinal = Ordinal;
        }
    }
}