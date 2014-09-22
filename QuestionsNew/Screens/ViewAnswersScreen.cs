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
		IList<Questions> questions;
		Adapters.ViewAnswerListAdapter answersList;
		Adapters.AnswerByQuestionAdapter questionList;
		ListView answersListView;
		ListView questionListView;
		TextView txtName;
		TextView txtDate;
		Button toggleView;

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
			questionListView = FindViewById<ListView> (Resource.Id.answersByQuestionView);
			txtName = FindViewById<TextView>(Resource.Id.textGroupName);
			txtDate = FindViewById<TextView> (Resource.Id.textDate);
			toggleView = FindViewById<Button> (Resource.Id.toggleViewButton);

			txtName.Text = group.group_name;
			txtDate.Text = "Created: " + group.date_created.ToString("d");

			toggleView.Click += viewByQuestion;

		}

		private void viewByQuestion (Object sender, EventArgs e) {
			// There will be two list views and only one of them will show at a time. The first time
			// this function is called we will create the adapter, on subsequent calls to this function
			// we will just hide the existing adapter and display this one.
			if (questionList == null) {
				// means the adapter was created yet, so we create it.
				questions = QuestionsManager.GetQuestions (groupID);
				questionList = new Adapters.AnswerByQuestionAdapter (this, questions);
				questionListView.Adapter = questionList;
			}
			// make the question list view visible
			answersListView.Visibility = ViewStates.Gone;
			questionListView.Visibility = ViewStates.Visible;
			// Chenge the text and event on toggleView to view by question
			toggleView.Text = "View By Date";
			toggleView.Click -= viewByQuestion;
			toggleView.Click += viewByDate;
			// Here is the query I came up with so far
			// select b.question_id, count(b.question_id) as answers_count from answer_groups a inner join answers b on a.answer_group_id = b.answer_group_id
			// group by b.question_id

		}

		private void viewByDate (Object sender, EventArgs e) {
			// make the date list view visible
			questionListView.Visibility = ViewStates.Invisible;
			answersListView.Visibility = ViewStates.Visible;
			// Chenge the text and event on toggleView to view by date
			toggleView.Text = "View By Question";
			toggleView.Click -= viewByDate;
			toggleView.Click += viewByQuestion;
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

