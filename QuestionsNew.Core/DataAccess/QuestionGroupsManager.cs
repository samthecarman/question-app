using System;
using System.Collections.Generic;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class QuestionGroupsManager {

		static QuestionGroupsManager ()
		{
		}
		
		public static QuestionGroups GetQuestionGroup(int id)
		{
			return QuestionGroupsRepositoryADO.GetQuestionGroup(id);
		}
		
		public static IList<QuestionGroups> GetQuestionGroups ()
		{
			return new List<QuestionGroups>(QuestionGroupsRepositoryADO.GetQuestionGroups());
		}
		
		public static int SaveQuestionGroups (QuestionGroups item)
		{
			return QuestionGroupsRepositoryADO.SaveQuestionGroups(item);
		}
		
		public static int DeleteQuestionGroup(int id)
		{
			return QuestionGroupsRepositoryADO.DeleteQuestionGroup(id);
		}
	}
}