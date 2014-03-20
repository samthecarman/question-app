using System.Collections.Generic;
using Android.App;
using Android.Widget;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;
using QuestionsNewAndroid;

namespace Adapters {
	/// <summary>
	/// Adapter that presents question groups in a row-view
	/// </summary>
	public class QuestionListAdapter : BaseAdapter<Questions> {
		Activity context = null;
		IList<Questions> questions = new List<Questions>();
		
		public QuestionListAdapter (Activity context, IList<Questions> questions) : base ()
		{
			this.context = context;
			this.questions = questions;
		}
		
		public override Questions this[int position]
		{
			get { return questions[position]; }
		}
		
		public override long GetItemId (int position)
		{
			return position;
		}
		
		public override int Count
		{
			get { return questions.Count; }
		}

		public override void NotifyDataSetChanged ()
		{
			base.NotifyDataSetChanged ();
		}

		public override Android.Views.View GetView (int position, Android.Views.View convertView, Android.Views.ViewGroup parent)
		{
			// A variable which will contain a reference to the ViewHolder object
			ViewHolder vh;

			// Get our object for position
			var item = questions[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (view == null) {
				view = (context.LayoutInflater.Inflate (
					Resource.Layout.QuestionView, 
					parent, 
					false)) as RelativeLayout;			

				vh = new ViewHolder ();

				// here's where we get our subview references
				vh.Initialize (view, context, position, questions);

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
			Button saveButton;
			Questions localQuestions;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context, int position, IList<Questions> questions)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.editQuestion);
				txtName.RequestFocus();
				saveButton = view.FindViewById<Button> (Resource.Id.saveQuestion);
				saveButton.Tag = questions[position].question_group_id;
				saveButton.Focusable = true;
				// wire up task click handler
				if (saveButton != null) {
					saveButton.Click += (sender, e) => {
						localQuestions = new Questions();
						localQuestions.q_text = txtName.Text;
						localQuestions.question_group_id = (int)((Button)sender).Tag;
						QuestionsManager.SaveQuestions(localQuestions);
					};
				}
			}

			// this method now handles binding data
			public void Bind(Questions data)
			{
				txtName.Text = data.q_text;
			}
		}
	}
}