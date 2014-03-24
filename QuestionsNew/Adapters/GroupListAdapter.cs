using System.Collections.Generic;
using Android.App;
using Android.Widget;
using QuestionsNew.Core.Model;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using QuestionsNewAndroid;

namespace Adapters {
	/// <summary>
	/// Adapter that presents question groups in a row-view
	/// </summary>
	public class GroupListAdapter : BaseAdapter<QuestionGroups> {
		Activity context = null;
		IList<QuestionGroups> questionGroups = new List<QuestionGroups>();
		
		public GroupListAdapter (Activity context, IList<QuestionGroups> questionGroups) : base ()
		{
			this.context = context;
			this.questionGroups = questionGroups;
		}
		
		public override QuestionGroups this[int position]
		{
			get { return questionGroups[position]; }
		}
		
		public override long GetItemId (int position)
		{
			return position;
		}
		
		public override int Count
		{
			get { return questionGroups.Count; }
		}
		
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// A variable which will contain a reference to the ViewHolder object
			ViewHolder vh;

			// Get our object for position
			QuestionGroups item = questionGroups[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (view == null) {
				view = (context.LayoutInflater.Inflate (
					Resource.Layout.GroupView, 
					parent, 
					false)) as RelativeLayout;			

				vh = new ViewHolder ();

				// here's where we get our subview references
				vh.Initialize (view, context, position, questionGroups);

				// push the viewholder reference into the view tag
				view.Tag = vh;
			}
			// get our viewholder from the tag
			vh = (ViewHolder)view.Tag;

			// bind our data!
			vh.Bind(item, position);			

			//Finally return the view
			return view;
		}

		// extend Java.Lang.Object or you will run into all kinds of type/cast issues when trying to push/pull on the View.Tag
		private class ViewHolder : Java.Lang.Object
		{
			TextView txtName;
			LinearLayout clickCatcher;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context, int position, IList<QuestionGroups> questionGroups)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.textGroupName);
				clickCatcher = view.FindViewById<LinearLayout> (Resource.Id.linearClick);

				clickCatcher.Click += (sender, e) => {
					PopupMenu groupOptions = new PopupMenu(context, view);

					groupOptions.Inflate(Resource.Menu.questionGroupMenu);

					groupOptions.MenuItemClick += (menuSender, menuEvent) => {
						switch (menuEvent.Item.ItemId)
						{
						case Resource.Id.answerQuestions:
							var answerQuestions = new Intent (context, typeof(QuestionsNewAndroid.Screens.AnswerScreen));
							answerQuestions.PutExtra ("question_group_id", questionGroups [(int)((LinearLayout)sender).Tag].question_group_id);
							context.StartActivity (answerQuestions);
							break;
						case Resource.Id.editAddQuestions:
							var groupDetails = new Intent (context, typeof(QuestionsNewAndroid.Screens.QuestionGroupScreen));
							groupDetails.PutExtra ("question_group_id", questionGroups [(int)((LinearLayout)sender).Tag].question_group_id);
							context.StartActivity (groupDetails);
							break;
						case Resource.Id.viewAnswers:
							var viewAnswers = new Intent (context, typeof(QuestionsNewAndroid.Screens.ViewAnswersScreen));
							viewAnswers.PutExtra ("question_group_id", questionGroups [(int)((LinearLayout)sender).Tag].question_group_id);
							context.StartActivity (viewAnswers);
							break;
						case Resource.Id.deleteGroup:
							break;
						default:
							break;
						}
					};

					groupOptions.Show();
				};
			}

			// this method now handles binding data
			public void Bind(QuestionGroups data, int position)
			{
				txtName.Text = data.group_name;
				clickCatcher.Tag = position;
			}
		}
	}
}