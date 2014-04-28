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

		public IDictionary<int,Answers> answers { get; set; }
		public bool modified { get; set;} // This is a flag so that I know if there was any modification.

		public AnswerListAdapter (Activity context, IList<Questions> questions) : base ()
		{
			this.context = context;
			this.questions = questions;
			answers = new Dictionary<int,Answers>();
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
				vh.Initialize (view);

				// push the viewholder reference into the view tag
				view.Tag = vh;
			} else {
				// get our viewholder from the tag
				vh = (ViewHolder)view.Tag;
			}

			vh.txtAnswer.AfterTextChanged -= textChangedHandler;

			// bind our data!
			vh.Bind(item, position, answers);			

			vh.txtAnswer.AfterTextChanged += textChangedHandler;


			//Finally return the view
			return view;
		}

		private void textChangedHandler(object sender, Android.Text.AfterTextChangedEventArgs e){
			Answers answer = new Answers();
			answer.a_text = ((EditText)sender).Text;
			answer.question = questions [(int)((EditText)sender).Tag];
			// Check if this position has an answer object in the answers dictionary.
			if (answers.ContainsKey((int)((EditText)sender).Tag)){
				answers[(int)((EditText)sender).Tag].a_text = ((EditText)sender).Text;
			} else {
				answers.Add((int)((EditText)sender).Tag, answer);
			}
			// Set the modified flag to true so I know to show the confirm dialog when canceled is clicked.
			modified = true;
		}

		// extend Java.Lang.Object or you will run into all kinds of type/cast issues when trying to push/pull on the View.Tag
		private class ViewHolder : Java.Lang.Object
		{
			public TextView txtName;
			public EditText txtAnswer;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.questionText);
				txtAnswer = view.FindViewById<EditText>(Resource.Id.editAnswer);
			}

			// this method now handles binding data
			public void Bind(Questions data, int position, IDictionary<int,Answers> answers)
			{
				txtName.Text = data.q_text;
				txtAnswer.Tag = position;
				// check if there already exists an element in answers with the current position
				if (answers.ContainsKey(position)) {
					// if yes then we update the text to match what is in the answers a_text field
					txtAnswer.Text = answers [position].a_text;
				} else {
					// if not we set it to blank because we are reusing the viewHandler objects and it is possible this was used already.
					txtAnswer.Text = "";
				}
			}
		}
	}
}