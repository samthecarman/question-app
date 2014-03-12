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

		public static IList<Questions> GetQuestions (int id)
		{
			return new List<Questions>(QuestionsRepositoryADO.GetQuestions(id));
		}
		
		public static int SaveQuestions (Questions item)
		{
			return QuestionsRepositoryADO.SaveQuestions(item);
		}
		
		public static int DeleteQuestion(int id)
		{
			return QuestionsRepositoryADO.DeleteQuestion(id);
		}
	}
}