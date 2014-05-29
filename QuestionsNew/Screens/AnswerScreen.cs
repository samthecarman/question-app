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
	[Activity (Label = "Answer")]			
	public class AnswerScreen : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		IList<Questions> questions;
		Adapters.AnswerListAdapter questionList;
		ListView questionListView;
		TextView groupName;
		Button cancelButton;
		Button saveAnswersButton;

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
			SetContentView(Resource.Layout.Answer);
			// we need to inflate the cancel button view to get a reference to the cancel button.
			var cancelView = (this.LayoutInflater.Inflate (
				Resource.Layout.AnswerButtons, 
				null));			

			// find all our controls
			cancelButton = cancelView.FindViewById<Button>(Resource.Id.cancelAnswersButton);
			questionListView = FindViewById<ListView> (Resource.Id.questionList);
			questionListView.AddFooterView (cancelView);
			questionListView.Focusable = false;
			groupName = FindViewById<TextView>(Resource.Id.questionGroupName);
			saveAnswersButton = cancelView.FindViewById<Button>(Resource.Id.saveAnswersButton);


			groupName.Text = group.group_name; 

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
			Adapters.AnswerListAdapter localAdapter =  (Adapters.AnswerListAdapter)((HeaderViewListAdapter)questionListView.Adapter).WrappedAdapter;
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
			// Get a reference to the adapter
			Adapters.AnswerListAdapter localAdapter =  (Adapters.AnswerListAdapter)((HeaderViewListAdapter)questionListView.Adapter).WrappedAdapter;

			// Check the flag if the data was modified
			if (localAdapter.modified) {
				this.ShowDialog (1);
			} else {
				Finish ();
			}
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

