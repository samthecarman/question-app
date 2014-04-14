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
	public class QuestionGroupsDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="Questionsy.DL.QuestionsDatabase"/> QuestionsDatabase. 
		/// if the database doesn't exist, it will create the database and all the tables.
		/// </summary>
		public QuestionGroupsDatabase (string dbPath) 
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

		/// <summary>Convert from DataReader to Questions object</summary>
		QuestionGroups FromReader (SqliteDataReader r) {
			var t = new QuestionGroups ();
			t.question_group_id = Convert.ToInt32 (r ["question_group_id"]);
			t.group_name = r ["group_name"].ToString ();
			t.account_id = Convert.ToInt32 (r ["question_group_id"]);
			var a = r ["date_created"];
			t.date_created = Convert.ToDateTime (r ["date_created"]);
			t.dlu = Convert.ToDateTime (r ["dlu"]);
			return t;
		}

		public IEnumerable<QuestionGroups> GetQuestionGroups ()
		{
			var tl = new List<QuestionGroups> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT a.question_group_id, a.group_name, a.account_id" +
					                       ", datetime(a.date_created) as date_created, datetime(a.dlu) as dlu FROM question_groups a";
					var r = contents.ExecuteReader ();
					while (r.Read ()) {
						tl.Add (FromReader(r));
					}
				}
				connection.Close ();
			}
			return tl;
		}

		public QuestionGroups GetQuestionGroup (int id) 
		{
			var t = new QuestionGroups ();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT a.question_group_id, a.group_name, a.account_id" +
						", datetime(a.date_created) as date_created, datetime(a.dlu) as dlu FROM question_groups a WHERE a.question_group_id = ?";
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

		public int SaveQuestionGroup (QuestionGroups item) 
		{
			int r;
			lock (locker) {
				if (item.question_group_id != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE question_groups SET group_name = ?, account_id = ?, dlu = ? WHERE question_group_id = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.group_name });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.account_id });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.question_group_id });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO question_groups (group_name, account_id, date_created, dlu) VALUES (? ,?, ?,?); " +
							"SELECT last_insert_rowid();";
						command.Parameters.Add (new SqliteParameter (DbType.String) { Value = item.group_name });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.account_id });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						r = (int)(long)command.ExecuteScalar ();
					}
					connection.Close ();
					return r;
				}

			}
		}

		public int DeleteQuestionGroup(int question_group_id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM question_groups WHERE question_group_id = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = question_group_id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}