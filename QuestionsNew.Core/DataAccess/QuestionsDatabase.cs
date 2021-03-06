using System;
using System.Linq;
using System.Collections.Generic;

using Mono.Data.Sqlite;
using System.IO;
using System.Data;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess
{
	/// <summary>
	/// QuestionsDatabase uses ADO.NET to create the [Questions] table and create,read,update,delete data
	/// </summary>
	public class QuestionsDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Questionsy.DL.QuestionsDatabase"/> QuestionsDatabase. 
		/// </summary>
		public QuestionsDatabase (string dbPath) 
		{
			var output = "";
			path = dbPath;
			// create the tables
			bool exists = File.Exists (dbPath);

			if (!exists) {
				// throw an error
			} else {
				// already exists, do nothing. 
			}
			Console.WriteLine (output);
		}

		/// <summary>Convert from DataReader to Questions object</summary>
		Questions FromReader (SqliteDataReader r) {
			var t = new Questions ();
			t.question_id = Convert.ToInt32 (r ["question_id"]);
			t.question_group_id = Convert.ToInt32 (r ["question_group_id"]);
			t.q_text = r ["q_text"].ToString ();
			t.field_name = r ["field_name"].ToString ();
			t.form_field_id = Convert.ToInt32 (r ["form_field_id"]);
			t.dlu = Convert.ToDateTime (r ["dlu"]);
			t.date_created = Convert.ToDateTime (r ["date_created"]);
			return t;
		}

		public IEnumerable<Questions> GetQuestions (int question_group_id)
		{
			var tl = new List<Questions> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT a.question_id, a.question_group_id, a.q_text, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
					                       ", a.form_field_id, b.field_name FROM questions a LEFT JOIN form_fields b ON a.form_field_id = b.form_field_id" +
						" WHERE a.question_group_id = ?";
					contents.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = question_group_id });
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
			}
			return tl;
		}

		public Questions GetQuestion (int id) 
		{
			var t = new Questions ();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT a.question_id, a.question_group_id, a.q_text, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
					                      ", a.form_field_id, b.field_name FROM questions a LEFT JOIN form_fields b ON a.form_field_id = b.form_field_id" +
										" WHERE a.question_id = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id });
					var r = command.ExecuteReader ();
					while (r.Read ()) {
						t = FromReader (r);
						break;
					}
				}
				connection.Close ();
			}
			return t;
		}

		public int SaveQuestion (Questions item) 
		{
			int r;
			lock (locker) {
				if (item.question_id != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE questions SET q_text = ?, form_field_id = ?, dlu = ? WHERE question_id = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.q_text });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.form_field_id });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.question_id });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO questions (question_group_id, q_text, form_field_id, date_created, dlu) VALUES (? , ?, ?, ?, ?)";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.question_group_id });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.q_text });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.form_field_id });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				}

			}
		}

		public int DeleteQuestion(int question_id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM questions WHERE question_id = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = question_id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}