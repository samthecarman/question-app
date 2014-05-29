using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;

namespace QuestionsNewAndroid.Screens
{
	[Activity (Label = "View Answers")]			
	public class ViewAnswersScreen : Activity
	{
		int groupID;
		QuestionGroups group;
		IList<AnswerGroups> answerGroups;
		Adapters.ViewAnswerListAdapter answersList;
		ListView answersListView;
		TextView groupName;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			groupID = Intent.GetIntExtra("question_group_id", 0);
			if (groupID > 0) {
				group = QuestionGroupsManager.GetQuestionGroup (groupID);
			} else {
				//Cancel the activity if there is no groupID
				Finish ();
			}
				
			// set our layout to be the Group screen
			SetContentView(Resource.Layout.ViewAnswers);
			answersListView = FindViewById<ListView> (Resource.Id.answersView);
			groupName = FindViewById<TextView>(Resource.Id.groupName);

			groupName.Text = group.group_name; 
			//notesTextEdit.Text = task.Notes;


		}



		protected override void OnResume ()
		{
			base.OnResume ();

			answerGroups = AnswerGroupsManager.GetAnswerGroups(groupID);

			// create our adapter
			answersList = new Adapters.ViewAnswerListAdapter(this, answerGroups);

			//Hook up our adapter to our ListView
			answersListView.Adapter = answersList;
		}
	}
}

