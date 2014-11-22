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
	public class AnswerByQuestionScreen : Activity
	{
		int questionID;
		Questions questions;
		QuestionGroups group;
		IList<Answers> answers;
		Adapters.SpecificQuestionAnswersAdapter answersList;
		ListView answersListView;
		TextView txtName;
		TextView txtDate;
		TextView txtQuestion;
		TextView txtAnswerHeader2;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			questionID = Intent.GetIntExtra("question_id", 0);
			if (questionID > 0) {
				questions = QuestionsManager.GetQuestion (questionID);
				group = QuestionGroupsManager.GetQuestionGroup (questions.question_group_id);
			} else {
				//Cancel the activity if there is no groupID
				Finish ();
			}
				
			// set our layout to be the Group screen
			SetContentView(Resource.Layout.ViewSpecificAnswer);
			answersListView = FindViewById<ListView> (Resource.Id.specificAnswerList);
			txtName = FindViewById<TextView>(Resource.Id.textGroupName);
			txtDate = FindViewById<TextView>(Resource.Id.textDate);
			txtQuestion = FindViewById<TextView>(Resource.Id.answerHeader1);
			txtAnswerHeader2 = FindViewById<TextView>(Resource.Id.answerHeader2);

			txtName.Text = group.group_name; 
			txtDate.Text = "Created: " + group.date_created.ToString("d");
			txtQuestion.Text = questions.q_text;
			txtAnswerHeader2.Visibility = ViewStates.Gone;


		}



		protected override void OnResume ()
		{
			base.OnResume ();

			answers = AnswersManager.GetAnswersByQuestionId(questionID);

			// create our adapter
			answersList = new Adapters.SpecificQuestionAnswersAdapter(this, answers);

			//Hook up our adapter to our ListView
			answersListView.Adapter = answersList;
		}
	}
}

