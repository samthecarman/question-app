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
	public class AnswersDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Answersy.DL.AnswersDatabase"/> AnswersDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		public AnswersDatabase (string dbPath) 
		{
			var output = "";
			path = dbPath;
			// create the tables
			bool exists = File.Exists (dbPath);

			if (!exists) {
				connection = new SqliteConnection ("Data Source=" + dbPath);

				connection.Open ();
				var commands = new[] {
					"CREATE TABLE [QUESTION_GROUPS] (question_group_id INTEGER PRIMARY KEY AUTOINCREMENT, group_name NTEXT, account_id INTEGER, date_created INTEGER, dlu INTEGER);"
					,"CREATE TABLE [QUESTIONS] (question_id INTEGER PRIMARY KEY AUTOINCREMENT, question_group_id INTEGER, form_field_id INTEGER, q_text NTEXT, date_created INTEGER, dlu INTEGER);"
					,"CREATE TABLE [FORM_FIELDS] (form_field_id INTEGER PRIMARY KEY AUTOINCREMENT, field_name NTEXT);"
					,"INSERT INTO FORM_FIELDS (field_name) VALUES ('textfield');"
					,"INSERT INTO FORM_FIELDS (field_name) VALUES ('checkbox');"
					,"CREATE TABLE [ANSWER_GROUPS] (answer_group_id INTEGER PRIMARY KEY AUTOINCREMENT, question_group_id INTEGER, date_created INTEGER, dlu INTEGER);"
					,"CREATE TABLE [ANSWERS] (answer_id INTEGER PRIMARY KEY AUTOINCREMENT, answer_group_id INTEGER, question_id INTEGER, a_text NTEXT, date_created INTEGER, dlu INTEGER);"
				};
				foreach (var command in commands) {
					using (var c = connection.CreateCommand ()) {
						c.CommandText = command;
						var i = c.ExecuteNonQuery ();
					}
				}
			} else {
				// already exists, do nothing. 
			}
			Console.WriteLine (output);
		}

		/// <summary>Convert from DataReader to Answers object</summary>
		Answers FromReader (SqliteDataReader r) {
			var t = new Answers ();
			t.answer_id = Convert.ToInt32 (r ["answer_id"]);
			t.answerGroup = AnswerGroupsManager.GetAnswerGroup( Convert.ToInt32 (r ["answer_group_id"]));
			t.question = QuestionsManager.GetQuestion( Convert.ToInt32 (r ["question_id"]));
			t.a_text = r ["a_text"].ToString ();
			t.dlu = Convert.ToDateTime (r ["dlu"]);
			t.date_created = Convert.ToDateTime (r ["date_created"]);
			return t;
		}

		public Answers GetAnswer (int answer_id) 
		{
			var t = new Answers ();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT a.answer_id, a.answer_group_id, a.question_id, a.a_text, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
						", c.field_name FROM answers a INNER JOIN questions b ON a.question_id = b.question_id " +
						"LEFT JOIN form_fields c ON b.form_field_id = c.form_field_id" +
						" WHERE a.answer_id = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = answer_id });
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

		public IEnumerable<Answers> GetAnswers (int answer_group_id)
		{
			var tl = new List<Answers> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT a.answer_id, a.answer_group_id, a.question_id, a.a_text, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
						" FROM answers a" +
						" WHERE a.answer_group_id = ?";
					contents.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = answer_group_id });
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
			}
			return tl;
		}

		/*public IEnumerable<Answers> GetAnswers (int answer_group_id)
		{
			var tl = new List<Answers> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT a.answer_id, a.answer_group_id, a.question_id, a.a_text, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
						", c.field_name FROM answers a INNER JOIN questions b ON a.question_id = b.question_id " +
						"LEFT JOIN form_fields c ON b.form_field_id = c.form_field_id" +
						" WHERE a.question_id = ?";
					contents.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = id });
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
			}
			return tl;
		}*/

		public int SaveAnswer (Answers item) 
		{
			int r;
			lock (locker) {
				if (item.answer_id != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE answers SET a_text = ?, dlu = ? WHERE answer_id = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.a_text });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.answer_id });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO answers (answer_group_id, question_id, a_text,  date_created, dlu) VALUES (? , ?, ?, ?, ?)";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.answerGroup.answer_group_id });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.question.question_id });
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.a_text });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				}

			}
		}

		public int DeleteAnswer(int answer_id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM answers WHERE answer_id = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = answer_id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}