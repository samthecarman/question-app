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
using Android.Provider;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;
using QuestionsNewAndroid;
using Java.Util;

namespace QuestionsNewAndroid.Screens
{
	[Activity (Label = "AddEditQuestions")]			
	public class AddEditQuestions : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		IList<Questions> questions;
		Adapters.QuestionListAdapter questionList;
		ListView questionListView;
		Button cancelButton;
		Button saveQuestionsButton;
		Button addQuestionButton;
		TextView txtName;
		TextView txtDate;
		ImageButton editGroupButton;

		private const int DIALOG_YES_NO_MESSAGE = 1;

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
			SetContentView(Resource.Layout.AddEditQuestion);
			// we need to inflate the cancel button view to get a reference to the cancel button.
			var cancelView = (this.LayoutInflater.Inflate (
				Resource.Layout.CancelButton, 
				null));			

			// find all our controls
			cancelButton = cancelView.FindViewById<Button>(Resource.Id.cancelButton);
			questionListView = FindViewById<ListView> (Resource.Id.questionListView);
			questionListView.Focusable = false;
			questionListView.AddFooterView (cancelView, null, false);
			addQuestionButton = FindViewById<Button> (Resource.Id.addQuestion);
			saveQuestionsButton = cancelView.FindViewById<Button> (Resource.Id.saveQuestionsButton);
			txtName = FindViewById<TextView>(Resource.Id.textGroupName);
			txtDate = FindViewById<TextView> (Resource.Id.textDate);
			editGroupButton = FindViewById<ImageButton> (Resource.Id.editGroupButton);

			txtName.Text = group.group_name;
			txtDate.Text = "Created: " + group.date_created.ToString("d");
			editGroupButton.Visibility = ViewStates.Visible;

			// button clicks 
			addQuestionButton.Click += AddQuestionView;
			saveQuestionsButton.Click += (sender, e) =>  { SaveQuestions(); };
			cancelButton.Click += (sender, e) => { Cancel(); };
			editGroupButton.Click += EditGroupView;
		
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
			// Send them back to the main screen
			StartActivity(typeof(MainActivity));
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

		void EditGroupView(Object sender, EventArgs e) {
			// Send them back to the edit group screen;
			var groupDetails = new Intent (this, typeof(QuestionsNewAndroid.Screens.QuestionGroupScreen));
			groupDetails.PutExtra ("question_group_id", groupID);
			StartActivity (groupDetails);
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

