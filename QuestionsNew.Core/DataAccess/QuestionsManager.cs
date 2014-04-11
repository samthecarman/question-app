using System;
using System.Collections.Generic;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class QuestionsManager {

		static QuestionsManager ()
		{
		}
		
		public static Questions GetQuestion(int id)
		{
			return QuestionsRepositoryADO.GetQuestion(id);
		}

		public static IList<Questions> GetQuestions (int question_group_id)
		{
			return new List<Questions>(QuestionsRepositoryADO.GetQuestions(question_group_id));
		}
		
		public static int SaveQuestions (Questions item)
		{
			return QuestionsRepositoryADO.SaveQuestions(item);
		}
		
		public static int DeleteQuestion(int question_id)
		{
			return QuestionsRepositoryADO.DeleteQuestion(question_id);
		}

		public static void DeleteQuestions(int question_group_id)
		{
			// get a list of all questions and loop over them calling DeleteQuestion
			//IList<Questions> questions = QuestionsManager.GetQuestions(question_group_id);
			foreach (var question in QuestionsManager.GetQuestions(question_group_id)) {
				QuestionsManager.DeleteQuestion (question.question_id);
			}

		}
	}
}