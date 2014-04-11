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
		
		public static Answers GetAnswer(int answer_id)
		{
			return AnswersRepositoryADO.GetAnswer(answer_id);
		}

		public static IList<Answers> GetAnswers (int answer_group_id)
		{
			return new List<Answers>(AnswersRepositoryADO.GetAnswers(answer_group_id));
		}
		
		public static int SaveAnswers (Answers item)
		{
			return AnswersRepositoryADO.SaveAnswers(item);
		}
		
		public static int DeleteAnswer(int answer_id)
		{
			return AnswersRepositoryADO.DeleteAnswer(answer_id);
		}

		public static void DeleteAnswers(int answer_group_id)
		{
			// get a list of all answers and loop over them calling DeleteAnswer
			foreach (var answer in AnswersManager.GetAnswers(answer_group_id)) {
				AnswersManager.DeleteAnswer(answer.answer_id);
			}
		}
	}
}