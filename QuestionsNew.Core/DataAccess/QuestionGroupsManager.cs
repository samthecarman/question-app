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
		
		public static int DeleteQuestionGroup(int question_group_id)
		{
			// First we delete all answer groups and answers associated with this group template
			AnswerGroupsManager.DeleteAnswerGroups (question_group_id);
			// Second we delete all questions associated with group
			QuestionsManager.DeleteQuestions (question_group_id);
			return QuestionGroupsRepositoryADO.DeleteQuestionGroup(question_group_id);
		}
	}
}