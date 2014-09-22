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
	[Activity (Label = "View Specific Answer")]			
	public class ViewSpecificAnswerScreen : Activity
	{
		int answerGroupID;
		AnswerGroups group;
		IList<Answers> answers;
		Adapters.SpecificAnswerListAdapter answersList;
		ListView answersListView;
		TextView txtName;
		TextView txtDate;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			answerGroupID = Intent.GetIntExtra("answer_group_id", 0);
			if (answerGroupID > 0) {
				group = AnswerGroupsManager.GetAnswerGroup (answerGroupID);
			} else {
				//Cancel the activity if there is no groupID
				Finish ();
			}
				
			// set our layout to be the Group screen
			SetContentView(Resource.Layout.ViewSpecificAnswer);
			answersListView = FindViewById<ListView> (Resource.Id.specificAnswerList);
			txtName = FindViewById<TextView>(Resource.Id.textGroupName);
			txtDate = FindViewById<TextView>(Resource.Id.textDate);

			txtName.Text = group.questionGroups.group_name; 
			txtDate.Text = group.date_created.ToString();


		}



		protected override void OnResume ()
		{
			base.OnResume ();

			answers = AnswersManager.GetAnswers(answerGroupID);

			// create our adapter
			answersList = new Adapters.SpecificAnswerListAdapter(this, answers);

			//Hook up our adapter to our ListView
			answersListView.Adapter = answersList;
		}
	}
}

