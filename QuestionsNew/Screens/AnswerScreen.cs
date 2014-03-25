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
	[Activity (Label = "AnswerScreen")]			
	public class AnswerScreen : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		IList<Questions> questions;
		IList<Answers> answers;
		Adapters.AnswerListAdapter questionList;
		ListView questionListView;
		TextView groupName;
		Button cancelButton;
		Button saveAnswersButton;


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

			View titleView = Window.FindViewById(Android.Resource.Id.Title);
			if (titleView != null) {
				IViewParent parent = titleView.Parent;
				if (parent != null && (parent is View)) {
					View parentView = (View)parent;
					parentView.SetBackgroundColor(Android.Graphics.Color.Rgb(0x26, 0x75 ,0xFF)); //38, 117 ,255
				}
			}			

			// set our layout to be the Group screen
			SetContentView(Resource.Layout.Answer);
			questionListView = FindViewById<ListView> (Resource.Id.questionList);
			questionListView.Focusable = false;
			groupName = FindViewById<TextView>(Resource.Id.questionGroupName);
			saveAnswersButton = FindViewById<Button>(Resource.Id.save);

			// find all our controls
			cancelButton = FindViewById<Button>(Resource.Id.cancel);

			groupName.Text = group.group_name; 
			//notesTextEdit.Text = task.Notes;

			// button clicks 
			cancelButton.Click += (sender, e) => { Cancel(); };
			saveAnswersButton.Click += (sender, e) => { Save(); };

		}

		void Save()
		{
			//First create the answer group
			AnswerGroups answerGroup = new AnswerGroups ();
			//Set the questiongroup that is associated with this answer group
			answerGroup.questionGroups = group;
			//Save the new group and get back the id
			int newId = AnswerGroupsManager.SaveAnswerGroups (answerGroup);
			// Check if we got a valid answer_group_id back
			if (!(newId > 0)) 
			{
				// Put some message here
			}
			// if we got a valid answer_group_id put the id into our answerGroup Object
			answerGroup.answer_group_id = newId;
			// Get a reference to the adapter
			Adapters.AnswerListAdapter localAdapter = (Adapters.AnswerListAdapter)questionListView.Adapter;
			// loop over the answers list that is stored in the adapter
			foreach (var currentAnswer in localAdapter.answers)
			{
				// Add the answer_group_id to the currentAnswer object.
				currentAnswer.Value.answerGroup = answerGroup;
				AnswersManager.SaveAnswers (currentAnswer.Value);
			}

			Finish();
		}

		void Cancel()
		{
			Finish();
		}


		protected override void OnResume ()
		{
			base.OnResume ();

			questions = QuestionsManager.GetQuestions(groupID);

			// create our adapter
			questionList = new Adapters.AnswerListAdapter(this, questions);

			//Hook up our adapter to our ListView
			questionListView.Adapter = questionList;
		}
	}
}

