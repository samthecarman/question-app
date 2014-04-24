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
	public class MainActivity : Activity
	{
		Adapters.GroupListAdapter groupList;
		IList<QuestionGroups> questionGroups;
		Button addGroupButton;
		ListView groupListView;
		int selected_group_id;

		private const int DIALOG_YES_NO_MESSAGE = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

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


