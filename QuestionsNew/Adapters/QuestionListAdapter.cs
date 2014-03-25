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
	public class QuestionListAdapter : BaseAdapter<Questions> {
		Activity context = null;
		IList<Questions> questions = new List<Questions>();
		
		public IDictionary<int,Questions> questionsDictionary { get; set; }

		public QuestionListAdapter (Activity context, IList<Questions> questions) : base ()
		{
			this.context = context;
			this.questions = questions;
			this.questionsDictionary = (IDictionary<int,Questions>)questions.Select((x,i) => new {item = x, index = i}).ToDictionary(d => d.index, d => d.item);//new Dictionary<int, Questions> ();
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
				vh.Initialize (view, context);

				// push the viewholder reference into the view tag
				view.Tag = vh;
			} else {
				// get our viewholder from the tag
				vh = (ViewHolder)view.Tag;
			}
			// we remove the event while we call bind, because of the recycled views
			vh.txtQuestion.AfterTextChanged -= textChangedHandler;

			// bind our data!
			vh.Bind(item, position, questionsDictionary);			

			vh.txtQuestion.AfterTextChanged += textChangedHandler;

			//Finally return the view
			return view;
		}

		private void textChangedHandler(object sender, Android.Text.AfterTextChangedEventArgs e){
			Questions question = new Questions();
			question.q_text = ((EditText)sender).Text;
			// Check if this position has an answer object in the answers dictionary.
			if (questionsDictionary.ContainsKey((int)((EditText)sender).Tag)){
				questionsDictionary[(int)((EditText)sender).Tag].q_text = ((EditText)sender).Text;
			} else {
				questionsDictionary.Add((int)((EditText)sender).Tag, question);
			}
		}

		// extend Java.Lang.Object or you will run into all kinds of type/cast issues when trying to push/pull on the View.Tag
		private class ViewHolder : Java.Lang.Object
		{
			public TextView txtName;
			public EditText txtQuestion;

			// this method now handles getting references to our subviews
			public void Initialize(Android.Views.View view, Activity context)
			{
				txtName = view.FindViewById<TextView>(Resource.Id.editQuestion);
				txtQuestion = view.FindViewById<EditText> (Resource.Id.editQuestion);
			}

			// this method now handles binding data
			public void Bind(Questions data, int position, IDictionary<int,Questions> questionsDictionary)
			{
				txtName.Text = data.q_text;
				txtQuestion.Tag = position;
				// check if there already exists an element in questionsDictionary with the current position
				if (questionsDictionary.ContainsKey(position)) {
					// if yes then we update the text to match what is in the questions q_text field
					txtQuestion.Text = questionsDictionary [position].q_text;
				} else {
					// if not we set it to blank because we are reusing the viewHandler objects and it is possible this was used already.
					txtQuestion.Text = "";
				}
			}
		}
	}
}