using System;
using System.Collections.Generic;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	/// <summary>
	/// Manager classes are an abstraction on the data access layers
	/// </summary>
	public static class AnswerGroupsManager {

		static AnswerGroupsManager ()
		{
		}
		
		public static AnswerGroups GetAnswerGroup(int answer_group_id)
		{
			return AnswerGroupsRepositoryADO.GetAnswerGroup(answer_group_id);
		}

		public static IList<AnswerGroups> GetAnswerGroups (int question_group_id)
		{
			return new List<AnswerGroups>(AnswerGroupsRepositoryADO.GetAnswerGroups(question_group_id));
		}
		
		public static int SaveAnswerGroups (AnswerGroups item)
		{
			return AnswerGroupsRepositoryADO.SaveAnswerGroups(item);
		}
		
		public static int DeleteAnswerGroup(int answer_group_id)
		{
			// First delete all answers associated with this group
			AnswersManager.DeleteAnswers (answer_group_id);
			return AnswerGroupsRepositoryADO.DeleteAnswerGroup(answer_group_id);
		}

		public static void DeleteAnswerGroups(int question_group_id)
		{
			// get a list of all answer groups and loop over them calling DeleteAnswerGroup
			IList<AnswerGroups> answerGroups = AnswerGroupsManager.GetAnswerGroups (question_group_id);
			foreach (var answerGroup in answerGroups) {
				AnswerGroupsManager.DeleteAnswerGroup (answerGroup.answer_group_id);
			}
		}
	}
}