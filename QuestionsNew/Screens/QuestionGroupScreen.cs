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
	[Activity (Label = "QuestionGroupScreen")]			
	public class QuestionGroupScreen : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		IList<Questions> questions;
		Adapters.QuestionListAdapter questionList;
		ListView questionListView;
		Button cancelButton;
		Button addQuestionButton;
		EditText groupTextEdit;
		Button saveGroupButton;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			groupID = Intent.GetIntExtra("question_group_id", 0);
			if(groupID > 0) {
				group = QuestionGroupsManager.GetQuestionGroup(groupID);
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
			SetContentView(Resource.Layout.Group);
			questionListView = FindViewById<ListView> (Resource.Id.questionListView);
			questionListView.Focusable = false;
			groupTextEdit = FindViewById<EditText>(Resource.Id.editGroupName);
			//questionTextEdit = FindViewById<EditText>(Resource.Id.editQuestion);
			saveGroupButton = FindViewById<Button>(Resource.Id.saveGroupName);

			// find all our controls
			cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
			addQuestionButton = FindViewById<Button> (Resource.Id.addQuestion);


			groupTextEdit.Text = group.group_name; 
			//notesTextEdit.Text = task.Notes;

			// button clicks 
			cancelButton.Click += (sender, e) => { Cancel(); };
			saveGroupButton.Click += (sender, e) => { Save(); };
			addQuestionButton.Click += (sender, e) => { AddQuestionView(); };

		}

		void Save()
		{
			group.group_name = groupTextEdit.Text;
			QuestionGroupsManager.SaveQuestionGroups(group);
			Finish();
		}

		void Cancel()
		{
			Finish();
		}

		void AddQuestionView()
		{
			Questions localQuestion = new Questions ();
			localQuestion.question_group_id = groupID;
			questions.Add (localQuestion);
			Adapters.QuestionListAdapter localAdapter = (Adapters.QuestionListAdapter)questionListView.Adapter;
			localAdapter.NotifyDataSetChanged();
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			questions = QuestionsManager.GetQuestions(groupID);

			// create our adapter
			questionList = new Adapters.QuestionListAdapter(this, questions);

			//Hook up our adapter to our ListView
			questionListView.Adapter = questionList;
		}
	}
}

