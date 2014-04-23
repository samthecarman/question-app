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
	public class AnswerGroupsDatabase 
	{
		static object locker = new object ();

		public SqliteConnection connection;

		public string path;

		/// <summary>
		/// Initializes a new instance of the <see cref="AnswerGroupsy.DL.AnswerGroupsDatabase"/> AnswerGroupsDatabase. 
		/// </summary>
		public AnswerGroupsDatabase (string dbPath) 
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

		/// <summary>Convert from DataReader to AnswerGroups object</summary>
		AnswerGroups FromReader (SqliteDataReader r) {
			var t = new AnswerGroups ();
			t.answer_group_id = Convert.ToInt32 (r ["answer_group_id"]);
			t.questionGroups = QuestionGroupsManager.GetQuestionGroup( Convert.ToInt32 (r ["question_group_id"]));
			t.dlu = Convert.ToDateTime (r ["dlu"]);
			t.date_created = Convert.ToDateTime (r ["date_created"]);
			return t;
		}

		public IEnumerable<AnswerGroups> GetAnswerGroups (int question_group_id)
		{
			var tl = new List<AnswerGroups> ();

			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var contents = connection.CreateCommand ()) {
					contents.CommandText = "SELECT a.answer_group_id, a.question_group_id, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
						" FROM answer_groups a " +
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

		public AnswerGroups GetAnswerGroup (int answer_group_id) 
		{
			var t = new AnswerGroups ();
			lock (locker) {
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "SELECT a.answer_group_id, a.question_group_id, datetime(a.date_created) as date_created, datetime(a.dlu) as dlu" +
						" FROM answer_groups a " +
						" WHERE a.answer_group_id = ?";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = answer_group_id });
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

		public int SaveAnswerGroup (AnswerGroups item) 
		{
			int r;
			lock (locker) {
				if (item.answer_group_id != 0) {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "UPDATE answer_groups SET dlu = ? WHERE answer_group_id = ?;";
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.answer_group_id });
						r = command.ExecuteNonQuery ();
					}
					connection.Close ();
					return r;
				} else {
					connection = new SqliteConnection ("Data Source=" + path);
					connection.Open ();
					using (var command = connection.CreateCommand ()) {
						command.CommandText = "INSERT INTO answer_groups (question_group_id, date_created, dlu) VALUES (? , ?, ?); " +
							"SELECT last_insert_rowid();";
						command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = item.questionGroups.question_group_id });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						command.Parameters.Add (new SqliteParameter (DbType.DateTime) { Value = DateTime.Now });
						//long tester = (long)command.ExecuteScalar ();
						//r = (int)tester;
						r = (int)(long)command.ExecuteScalar ();
					}
					connection.Close ();
					return r;
				}

			}
		}

		public int DeleteAnswerGroup(int answer_group_id) 
		{
			lock (locker) {
				int r;
				connection = new SqliteConnection ("Data Source=" + path);
				connection.Open ();
				using (var command = connection.CreateCommand ()) {
					command.CommandText = "DELETE FROM answer_groups WHERE answer_group_id = ?;";
					command.Parameters.Add (new SqliteParameter (DbType.Int32) { Value = answer_group_id});
					r = command.ExecuteNonQuery ();
				}
				connection.Close ();
				return r;
			}
		}
	}
}