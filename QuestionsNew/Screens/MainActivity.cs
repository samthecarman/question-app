using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;

namespace QuestionsNewAndroid.Screens
{
	[Activity (Label = "Questions", MainLauncher = true)]
	public class MainActivity : Activity, ActionMode.ICallback
	{
		Adapters.GroupListAdapter groupList;
		IList<QuestionGroups> questionGroups;
		Button addGroupButton;
		ListView groupListView;
		int selected_group_id;
		ActionMode actionMode;
		int actionModePosition = -1;


		private const int DIALOG_YES_NO_MESSAGE = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			RequestWindowFeature(WindowFeatures.ActionBar);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			//Find our controls
			groupListView = FindViewById<ListView> (Resource.Id.groupListView);
			//groupListView.Focusable = false;
			addGroupButton = FindViewById<Button> (Resource.Id.newGroupButton);

			// wire up add question group button handler
			if(addGroupButton != null) {
				addGroupButton.Click += (sender, e) => {
					StartActivity(typeof(QuestionGroupScreen));
				};
			}

			if (groupListView != null) {
				// Regular click event
				groupListView.ItemClick += (sender, e) => {
					// Depending on if there are any questions in the current option, they will get sent to the question screen or the answer screen
					if (questionGroups [e.Position].has_questions){
						// There are questions in this group, so we send them to the answer screen
						var answerQuestions = new Intent (this, typeof(QuestionsNewAndroid.Screens.AnswerScreen));
						answerQuestions.PutExtra ("question_group_id",  questionGroups [e.Position].question_group_id);
						StartActivity (answerQuestions);
					}
					else {
						// There are no questions in this group so we send them to the add question screen
						var groupDetails = new Intent (this, typeof(QuestionsNewAndroid.Screens.QuestionGroupScreen));
						groupDetails.PutExtra ("question_group_id", questionGroups [e.Position].question_group_id);
						StartActivity (groupDetails);
					}
				};
				groupListView.ItemLongClick += (sender, e) => {
					if (actionMode != null)
						return;

					actionModePosition = e.Position;

					actionMode = StartActionMode(this);
				};
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();

			// We are using async so I need to call Result so I get the correct result.
			questionGroups = QuestionGroupsManager.GetQuestionGroups().Result;

			// create our adapter
			groupList = new Adapters.GroupListAdapter(this, questionGroups);

			//Hook up our adapter to our ListView
			groupListView.Adapter = groupList;

		}

		//region ICallback implementation

		public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
		{
			// Check to make sure we have the position
			if (actionModePosition == -1)
				return false;

			switch (item.ItemId) {
			case Resource.Id.ActionModeAnswerItem: 
				// if we want to answer the questions
				var answerQuestions = new Intent (this, typeof(QuestionsNewAndroid.Screens.AnswerScreen));
				answerQuestions.PutExtra ("question_group_id", questionGroups [actionModePosition].question_group_id);
				StartActivity (answerQuestions);

				actionModePosition = -1;

				mode.Finish ();
				break;
			case Resource.Id.ActionModeEditItem:
				// if we want to edit or add questions
				var groupDetails = new Intent (this, typeof(QuestionsNewAndroid.Screens.QuestionGroupScreen));
				groupDetails.PutExtra ("question_group_id", questionGroups [actionModePosition].question_group_id);
				StartActivity (groupDetails);

				actionModePosition = -1;

				mode.Finish ();
				break;
			case Resource.Id.ActionModeViewItem:
				// if we want to view the existing answers
				var viewAnswers = new Intent (this, typeof(QuestionsNewAndroid.Screens.ViewAnswersScreen));
				viewAnswers.PutExtra ("question_group_id", questionGroups [actionModePosition].question_group_id);
				StartActivity (viewAnswers);

				actionModePosition = -1;

				mode.Finish ();
				break;
			case Resource.Id.ActionModeDeleteItem:
				// if we are deleting the group
				// create a bundle so I can pass the question_group_id to the dialog.
				Bundle forDialog = new Bundle();
				forDialog.PutInt("question_group_id", questionGroups [actionModePosition].question_group_id);
				ShowDialog(1, forDialog);

				actionModePosition = -1;

				mode.Finish ();
				break;
			default:
				break;
			}

			return true;
		}

		public bool OnCreateActionMode(ActionMode mode, IMenu menu)
		{
			mode.MenuInflater.Inflate(Resource.Menu.actionModeMain, menu);
			return true;
		}

		public void OnDestroyActionMode(ActionMode mode)
		{
			actionMode = null;
		}

		public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
		{
			return false;
		}
		// End Icallback region

		// putting the onCreateDialog method called implicitly when using ShowDialog. show dialog is called in grouplistadapter.cs
		protected override Dialog OnCreateDialog(int id, Bundle args)
		{
			// first get the question_group_id and assign it to a global variable
			selected_group_id = args.GetInt ("question_group_id");

			switch (id)
			{
			case DIALOG_YES_NO_MESSAGE:
				var builder = new AlertDialog.Builder (this);
				builder.SetIconAttribute (Android.Resource.Attribute.AlertDialogIcon);
				builder.SetTitle (Resource.String.dialog_two_buttons_title);
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
			// set the question_group_id and assign it to a global variable
			selected_group_id = args.GetInt ("question_group_id");
		}

		private void yesClicked (object sender, DialogClickEventArgs e)
		{
			int tester = selected_group_id;
			QuestionGroupsManager.DeleteQuestionGroup (selected_group_id);
			//remove the deleted group for the questionGroups list
			questionGroups.Remove (questionGroups.FirstOrDefault(localGroup => localGroup.question_group_id == selected_group_id));
			//We now need to notify the adapter to refresh the data
			Adapters.GroupListAdapter localAdapter;
			localAdapter = (Adapters.GroupListAdapter)groupListView.Adapter;
			localAdapter.NotifyDataSetChanged();

		}

		private void CancelClicked (object sender, DialogClickEventArgs e)
		{
		}

	}
}


