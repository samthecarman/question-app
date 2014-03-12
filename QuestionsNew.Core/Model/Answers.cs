using System;

namespace QuestionsNew.Core.Model
{
	public class Answers
	{
		public Answers () {
		}

		public int answer_id { get; set; }
		public AnswerGroups answerGroup { get; set; }
		public Questions question { get; set; }
		public string a_text { get; set; }
		public DateTime date_created { get; set;}
		public DateTime dlu { get; set;}
	}
}

