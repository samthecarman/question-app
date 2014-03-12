using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Widget;
using QuestionsNew.Core.Model;
using QuestionsNew.Core.DataAccess;
using QuestionsNewAndroid;

namespace Adapters {
	/// <summary>
	/// Adapter that presents question groups in a row-view
	/// </summary>
	public class SpecificAnswerListAdapter : BaseAdapter<Answers> {
		Activity context = null;
		IList<Answers> answers = new List<Answers>();

		public SpecificAnswerListAdapter (Activity context, IList<Answers> answers) : base ()
		{
			this.context = context;
			this.answers = answers;
		}
		
		public override Answers this[int position]
		{
			get { return answers[position]; }
		}
		
		public override long GetItemId (int position)
		{
			return position;
		}
		
		public override int Count
		{
			get { return answers.Count; }
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
			var item = answers[position];			

			//Try to reuse convertView if it's not  null, otherwise inflate it from our item layout
			// gives us some performance gains by not always inflating a new view
			// will sound familiar to MonoTouch developers with UITableViewCell.DequeueReusableCell()
			var view = convertView;
			if (view == null) {
				view = (context.LayoutInflater.Inflate (
					Resource.Layout.SpecificAnswerListView, 
					parent, 
					false)) as RelativeLayout;			

				vh = new ViewHolder ();

				// here's where we get our subview references
				vh.Initialize (view, context, position, answers);

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
			TextView txtQuestion;
			TextView txtAnswer;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context, int position, IList<Answers> answers)
			{
				txtQuestion = view.FindViewById<TextView>(Resource.Id.questionText);
				txtAnswer = view.FindViewById<TextView>(Resource.Id.answerText);
				txtAnswer.Tag = position;
			}

			// this method now handles binding data
			public void Bind(Answers data)
			{
				txtQuestion.Text = data.question.q_text;
				txtAnswer.Text = data.a_text;
			}
		}
	}
}