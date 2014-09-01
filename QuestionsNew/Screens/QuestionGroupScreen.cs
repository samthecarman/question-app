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
using Java.Util;

namespace QuestionsNewAndroid.Screens
{
	[Activity (Label = "Questions Template")]			
	public class QuestionGroupScreen : Activity
	{
		int groupID;
		QuestionGroups group = new QuestionGroups();
		Button cancelButton;
		Button saveAddQuestions;
		EditText groupTextEdit;
		Button saveGroupButton;
		Button remindButton;
		TextView requiredTextView;
		bool groupModified;

		private const int DIALOG_YES_NO_MESSAGE = 1;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			groupID = Intent.GetIntExtra("question_group_id", 0);
			if(groupID > 0) {
				group = QuestionGroupsManager.GetQuestionGroup(groupID);
			}
				
			// set our layout to be the Group screen
			SetContentView(Resource.Layout.Group);

			// find all our controls
			cancelButton = FindViewById<Button>(Resource.Id.cancelButton);
			saveAddQuestions = FindViewById<Button> (Resource.Id.saveAddButton);
			groupTextEdit = FindViewById<EditText>(Resource.Id.editGroupName);
			saveGroupButton = FindViewById<Button>(Resource.Id.saveGroupName);
			remindButton = FindViewById<Button> (Resource.Id.setReminder);
			requiredTextView = FindViewById<TextView> (Resource.Id.textViewRequired);
			// If they already have a name entered for the group, we are going to change the text of the button to say "Save Changes"
			if (group.group_name != null) {
				saveGroupButton.Text = "Save Template Changes";
				// Hide required if they have a name already
				requiredTextView.Visibility = ViewStates.Gone;
			} else {
				// if they do not already have a group I disable the add question button.
				remindButton.Alpha = .45F;
			}


			groupTextEdit.Text = group.group_name; 

			// button clicks 
			cancelButton.Click += (sender, e) => { Cancel(); };
			saveGroupButton.Click += (sender, e) => { Save(); };
			saveAddQuestions.Click += (sender, e) => {
				// This button calles the save function and then sends them to the screen to create the questions.
				Save();
				var groupDetails = new Intent (this, typeof(QuestionsNewAndroid.Screens.AddEditQuestions));
				groupDetails.PutExtra ("question_group_id", groupID);
				StartActivity (groupDetails);
			};

			// if the group id is 0 that means we are creating a new group,
			// in that case we want to display a toast reminding the user to save the group name before proceding
			if (groupID == 0) {
				remindButton.Click += DisplaySaveReminder;
			} else {
				remindButton.Click += SetReminder;
			}
			// Add an event to groupTextEdit to catch when modified so I can warn when cancled.
			groupTextEdit.AfterTextChanged += (sender, e) => {
				groupModified = true;
			};

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
				remindButton.Alpha = 1.0F;
				// change the event for the remindButton from DisplaySaveReminder to SetReminder
				remindButton.Click -= DisplaySaveReminder;
				remindButton.Click += SetReminder;
				// Set the modified flag back to false
				groupModified = false;
			}
			Toast.MakeText (this, "Template name saved successfully", ToastLength.Short).Show ();
		}

		void Cancel()
		{
			// Check the flag if the data was modified
			if (groupModified) {
				this.ShowDialog (1);
			} else {
				Finish ();
			}
		}
			
		void DisplaySaveReminder (Object sender, EventArgs e)
		{
			Toast.MakeText (this, "Please save the template name", ToastLength.Short).Show();
		}

		// method called when set reminder is clicked, that will send them to the calendar app to set a reminder
		void SetReminder(Object sender, EventArgs e){
			//var test = CalendarContract.ContentUri;
			//Intent calendarIntent = new Intent (Intent.ActionEdit, CalendarContract.Calendars.ContentUri);
			//StartActivity (calendarIntent);
			Intent intent = new Intent (Intent.ActionEdit,CalendarContract.Events.ContentUri);
			intent.PutExtra("title", "Time to answer " + group.group_name);
			StartActivity(intent);		
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

