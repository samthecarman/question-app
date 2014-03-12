using System;

namespace QuestionsNew.Core.Model
{
	public class AnswerGroups
	{
		public AnswerGroups () {
		}

		public int answer_group_id { get; set; }
		public QuestionGroups questionGroups { get; set; }
		public DateTime date_created { get; set;}
		public DateTime dlu { get; set;}
	}
}

