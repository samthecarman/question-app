using System;
using System.Collections.Generic;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class AnswersManager {

		static AnswersManager ()
		{
		}
		
		public static Answers GetAnswer(int id)
		{
			return AnswersRepositoryADO.GetAnswer(id);
		}

		public static IList<Answers> GetAnswers (int answer_group_id)
		{
			return new List<Answers>(AnswersRepositoryADO.GetAnswers(answer_group_id));
		}
		
		public static int SaveAnswers (Answers item)
		{
			return AnswersRepositoryADO.SaveAnswers(item);
		}
		
		public static int DeleteAnswer(int id)
		{
			return AnswersRepositoryADO.DeleteAnswer(id);
		}
	}
}