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
	[Activity (Label = "Questions Template")]			
	public class QuestionGroupScreen : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		IList<Questions> questions;
		Adapters.QuestionListAdapter questionList;
		ListView questionListView;
		Button cancelButton;
		Button saveQuestionsButton;
		Button addQuestionButton;
		EditText groupTextEdit;
		Button saveGroupButton;

		private const int DIALOG_YES_NO_MESSAGE = 1;

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
			// we need to inflate the cancel button view to get a reference to the cancel button.
			var cancelView = (this.LayoutInflater.Inflate (
				Resource.Layout.CancelButton, 
				null));			

			// find all our controls
			cancelButton = cancelView.FindViewById<Button>(Resource.Id.cancelButton);
			questionListView = FindViewById<ListView> (Resource.Id.questionListView);
			questionListView.Focusable = false;
			questionListView.AddFooterView (cancelView);
			groupTextEdit = FindViewById<EditText>(Resource.Id.editGroupName);
			addQuestionButton = FindViewById<Button> (Resource.Id.addQuestion);
			saveGroupButton = FindViewById<Button>(Resource.Id.saveGroupName);
			// If they already have a name entered for the group, we are going to change the text of the button to say "Save Changes"
			if (group.group_name != null) {
				saveGroupButton.Text = "Save Changes";
			} else {
				// if they do not already have a group I disable the add question button.
				addQuestionButton.Enabled = false;
				//addQuestionButton.Clickable = true;
			}
			saveQuestionsButton = cancelView.FindViewById<Button> (Resource.Id.saveQuestionsButton);


			groupTextEdit.Text = group.group_name; 

			// button clicks 
			cancelButton.Click += (sender, e) => { Cancel(); };
			saveGroupButton.Click += (sender, e) => { Save(); };
			// if the group id is 0 that means we are creating a new group,
			// in that case we want to display a toast reminding the user to save the group name before proceding
			if (groupID == 0) {
				addQuestionButton.Click += DisplaySaveReminder;
			} else {
				addQuestionButton.Click += AddQuestionView;
			}
			saveQuestionsButton.Click += (sender, e) =>  { SaveQuestions(); };

			// Get any existing questions for the adapter.
			questions = QuestionsManager.GetQuestions(groupID);

			// if there are questions I enable the save questions button
			if (questions.Count > 0) {
				saveQuestionsButton.Enabled = true;
			}

			// create our adapter
			questionList = new Adapters.QuestionListAdapter(this, questions);

			//Hook up our adapter to our ListView
			questionListView.Adapter = questionList;
		}

		void SaveQuestions()
		{
			// Get a reference to the adapter
			Adapters.QuestionListAdapter localAdapter = (Adapters.QuestionListAdapter)((HeaderViewListAdapter)questionListView.Adapter).WrappedAdapter;
			// loop over the answers list that is stored in the adapter
			foreach (var currentQuestion in localAdapter.questionsDictionary)
			{
				// Add the question_group_id to the currrentQuestion object.
				currentQuestion.Value.question_group_id = group.question_group_id;
				QuestionsManager.SaveQuestions (currentQuestion.Value);
			}

			Finish();
		}

		void Save()
		{
			group.group_name = groupTextEdit.Text;
			// save the question group and save what is returned in a variable. If group_id is 0 then we 
			// set group_id to equal this variable because that means this is the first time we are saving this group.
			int question_group_id = QuestionGroupsManager.SaveQuestionGroups(group);
			if (groupID == 0) {
				groupID = question_group_id;
				group.question_group_id = question_group_id;
				// make the add question button pushable and change the text of the button
				saveGroupButton.Text = "Save Changes";
				addQuestionButton.Enabled = true;
				// change the event from DisplaySaveReminder to AddQuestionView
				addQuestionButton.Click -= DisplaySaveReminder;
				addQuestionButton.Click += AddQuestionView;
			}
			Toast.MakeText (this, "Template name saved successfully", ToastLength.Short).Show ();
		}

		void Cancel()
		{
			// Get a reference to the adapter
			Adapters.QuestionListAdapter localAdapter = (Adapters.QuestionListAdapter)((HeaderViewListAdapter)questionListView.Adapter).WrappedAdapter;

			// Check the flag if the data was modified
			if (localAdapter.modified) {
				this.ShowDialog (1);
			} else {
				Finish ();
			}
		}

		void AddQuestionView(Object sender, EventArgs e)
		{
			Questions localQuestion = new Questions ();
			localQuestion.question_group_id = groupID;
			questions.Add (localQuestion);
			Adapters.QuestionListAdapter localAdapter;
			localAdapter = (Adapters.QuestionListAdapter)((HeaderViewListAdapter)questionListView.Adapter).WrappedAdapter;
			localAdapter.NotifyDataSetChanged();
			// set the modified flag so we can confirm the cancel dialog
			localAdapter.modified = true;
			// Set the list view to scroll to the newest element
			questionListView.SetSelection ((localAdapter.Count) - 1);
			// if this is the first question added we need to enable the save questions button.
			if (questions.Count == 1) {
				saveQuestionsButton.Enabled = true;
			}
		}

		void DisplaySaveReminder (Object sender, EventArgs e)
		{
			Toast.MakeText (this, "Please save the template name", ToastLength.Short).Show();
		}

		protected override void OnResume ()
		{
			base.OnResume ();
		}

		public override void OnBackPressed ()
		{
			Cancel ();
		}

		// putting the onCreateDialog method called implicitly when using ShowDialog.
		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			switch (id)
			{
			case DIALOG_YES_NO_MESSAGE:
				var builder = new AlertDialog.Builder (this);
				builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
				builder.SetTitle (Resource.String.dialog_cancel_two_buttons_title);
				builder.SetCancelable (true);
				builder.SetPositiveButton (Resource.String.dialog_yes, yesClicked);
				builder.SetNegativeButton (Resource.String.dialog_cancel, CancelClicked);

				return builder.Create ();
			}
			return null;
		}

		protected override void OnPrepareDialog (int id, Dialog dialog, Bundle args)
		{
			base.OnPrepareDialog (id, dialog, args);
		}

		private void yesClicked (object sender, DialogClickEventArgs e)
		{
			Finish ();
		}

		private void CancelClicked (object sender, DialogClickEventArgs e)
		{
		}
	}
}

