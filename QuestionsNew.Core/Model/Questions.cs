using System;

namespace QuestionsNew.Core.Model
{
	public class Questions
	{
		public Questions () {
		}

		public int question_id { get; set; }
		public int question_group_id { get; set; }
		public string q_text { get; set; }
		public string field_name { get; set; }
		public int form_field_id { get; set; }
		public DateTime date_created { get; set;}
		public DateTime dlu { get; set;}
	}
}

