using System.Collections.Generic;
using Android.App;
using Android.Widget;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using QuestionsNewAndroid;

namespace Adapters {
	/// <summary>
	/// Adapter that presents question groups in a row-view
	/// </summary>
	public class ViewAnswerListAdapter : BaseAdapter<AnswerGroups> {
		Activity context = null;
		IList<AnswerGroups> answerGroups = new List<AnswerGroups>();
		
		public ViewAnswerListAdapter (Activity context, IList<AnswerGroups> answerGroups) : base ()
		{
			this.context = context;
			this.answerGroups = answerGroups;
		}
		
		public override AnswerGroups this[int position]
		{
			get { return answerGroups[position]; }
		}
		
		public override long GetItemId (int position)
		{
			return position;
		}
		
		public override int Count
		{
			get { return answerGroups.Count; }
		}
		
		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// A variable which will contain a reference to the ViewHolder object
			ViewHolder vh;

			// Get our object for position
			AnswerGroups item = answerGroups[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (view == null) {
				view = (context.LayoutInflater.Inflate (
					Resource.Layout.ViewAnswerListView, 
					parent, 
					false)) as RelativeLayout;			

				vh = new ViewHolder ();

				// here's where we get our subview references
				vh.Initialize (view, context, position, answerGroups);

				// push the viewholder reference into the view tag
				view.Tag = vh;
			}
			// get our viewholder from the tag
			vh = (ViewHolder)view.Tag;

			// bind our data!
			vh.Bind(item);			

			//Finally return the view
			return view;
		}

		// extend Java.Lang.Object or you will run into all kinds of type/cast issues when trying to push/pull on the View.Tag
		private class ViewHolder : Java.Lang.Object
		{
			TextView txtName;
			ImageButton viewAnswerButton;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context, int position, IList<AnswerGroups> answerGroups)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.dateAnswered);
				viewAnswerButton = view.FindViewById<ImageButton> (Resource.Id.imageInfo);
				viewAnswerButton.Tag = position;
				viewAnswerButton.Focusable = true;
				// wire up view answers click handler
				if (viewAnswerButton != null) {
					viewAnswerButton.Click += (sender, e) => {
						var viewAnswer = new Intent (context, typeof(QuestionsNewAndroid.Screens.ViewSpecificAnswerScreen));
						viewAnswer.PutExtra ("answer_group_id", answerGroups [(int)((ImageButton)sender).Tag].answer_group_id);
						context.StartActivity (viewAnswer);

						// When the View Answer button is clicked the answers will drop down.
						// I am accomplishing that by setting the adapter in the click view.

						// First check if the Adapter was attached already.
						/*if (specificAnswerList.Adapter == null) {
							// if it wasn't created, create it now.
							// First we need to get the answer objects to this group.
							IList<Answers> answers = AnswersManager.GetAnswers((int)viewAnswerButton.Tag);
							specificAnswerList.Adapter = new SpecificAnswerListAdapter(context, answers);
						}
						else {
							// if it was created check if it's showing, if it is then hide it, if it's not then show it.
							if (specificAnswerList.Visibility == ViewStates.Visible) {
								specificAnswerList.Visibility = ViewStates.Gone;
							}
							else {
								specificAnswerList.Visibility = ViewStates.Visible;
							}
						}*/
					};
				}
			}

			// this method now handles binding data
			public void Bind(AnswerGroups data)
			{
				txtName.Text = data.date_created.ToString();
			}
		}
	}
}