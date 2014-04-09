using System;
using System.Collections.Generic;
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

			questionGroups = QuestionGroupsManager.GetQuestionGroups();

			// create our adapter
			groupList = new Adapters.GroupListAdapter(this, questionGroups);

			//Hook up our adapter to our ListView
			groupListView.Adapter = groupList;

		}

		// putting the onCreateDialog method called implicitly when using ShowDialog. show dialog is called in grouplistadapter.cs
		protected override Dialog OnCreateDialog(int id)
		{
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

		private void yesClicked (object sender, DialogClickEventArgs e)
		{
		}

		private void CancelClicked (object sender, DialogClickEventArgs e)
		{
		}

	}
}


