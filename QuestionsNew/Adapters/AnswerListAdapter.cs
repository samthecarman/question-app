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
	public class AnswerListAdapter : BaseAdapter<Questions> {
		Activity context = null;
		IList<Questions> questions = new List<Questions>();

		public IList<Answers> answers { get; set; }

		public AnswerListAdapter (Activity context, IList<Questions> questions) : base ()
		{
			this.context = context;
			this.questions = questions;
			answers = new List<Answers>();
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
					Resource.Layout.AnswerView, 
					parent, 
					false)) as RelativeLayout;			

				vh = new ViewHolder ();

				// here's where we get our subview references
				vh.Initialize (view, context, position, questions, answers);

				// push the viewholder reference into the view tag
				view.Tag = vh;
			}
			// get our viewholder from the tag
			vh = (ViewHolder)view.Tag;

			// bind our data!
			vh.Bind(item, position, answers);			

			//Finally return the view
			return view;
		}

		// extend Java.Lang.Object or you will run into all kinds of type/cast issues when trying to push/pull on the View.Tag
		private class ViewHolder : Java.Lang.Object
		{
			TextView txtName;
			EditText txtAnswer;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context, int position, IList<Questions> questions, IList<Answers> answers)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.questionText);
				txtAnswer = view.FindViewById<EditText>(Resource.Id.editAnswer);
				txtAnswer.FocusChange += (object sender, Android.Views.View.FocusChangeEventArgs e) => {
					// We only add it to the list when they leave so we check that it doesn't currently have focus.
					if (!e.HasFocus) {
						Answers answer = new Answers();
						answer.a_text = txtAnswer.Text;
						answer.question = questions [(int)((EditText)sender).Tag];
						// Try to get a reference to existing Answer object in answers, will return null if it doesn't find it.
						Answers answerInList = answers.FirstOrDefault(localAnswer => localAnswer.question.question_id == answer.question.question_id);

						// Check if answerInList is null

						if (answerInList == null) {
							// if it doesn't exist we add it to answers list.
							answers.Add(answer);
						} 
						else {
							// it does exist so we just update the a_text field
							answerInList.a_text = txtAnswer.Text;
						}
					}
				};
			}

			// this method now handles binding data
			public void Bind(Questions data, int position, IList<Answers> answers)
			{
				txtName.Text = data.q_text;
				// check if there already exists an element in answers with the current position
				if (answers.Count > position) {
					// if yes then we update the text to match what is in the answers a_text field
					txtAnswer.Text = answers [position].a_text;
				} else {
					// if not we set it to blank because we are reusing the viewHandler objects and it is possible this was used already.
					txtAnswer.Text = "";
				}
				txtAnswer.Tag = position;
			}
		}
	}
}