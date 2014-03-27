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

	}
}


