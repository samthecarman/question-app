using System;

namespace QuestionsNew.Core.Model {
	/// <summary>
	/// Question business object
	/// </summary>
	public class QuestionGroups {
		public QuestionGroups () {
		}

		public int question_group_id { get; set; }
		public int account_id { get; set; }
		public string group_name { get; set;}
		public DateTime date_created { get; set;}
		public DateTime dlu { get; set;}
	}
}