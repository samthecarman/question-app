using System;
using System.Collections.Generic;
using System.IO;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	public class QuestionGroupsRepositoryADO {
		QuestionGroupsDatabase db = null;
		protected static string dbLocation;		
		protected static QuestionGroupsRepositoryADO me;		

		static QuestionGroupsRepositoryADO ()
		{
			me = new QuestionGroupsRepositoryADO();
		}

		protected QuestionGroupsRepositoryADO ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new QuestionGroupsDatabase(dbLocation);
		}

		public static string DatabaseFilePath {
			get { 
				var sqliteFilename = "QuestionsDatabase.db3";
				#if NETFX_CORE
				var path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, sqliteFilename);
				#else

				#if SILVERLIGHT
				// Windows Phone expects a local path, not absolute
				var path = sqliteFilename;
				#else

				#if __ANDROID__
				// Just use whatever directory SpecialFolder.Personal returns
				string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal); ;
				#else
				// we need to put in /Library/ on iOS5.1 to meet Apple's iCloud terms
				// (they don't want non-user-generated data in Documents)
				string documentsPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal); // Documents folder
				string libraryPath = Path.Combine (documentsPath, "..", "Library"); // Library folder
				#endif
				var path = Path.Combine (libraryPath, sqliteFilename);
				#endif

				#endif
				return path;	
			}
		}

		public static QuestionGroups GetQuestionGroup(int id)
		{
			return me.db.GetQuestionGroup(id);
		}

		public static IEnumerable<QuestionGroups> GetQuestionGroups ()
		{
			return me.db.GetQuestionGroups();
		}

		public static int SaveQuestionGroups (QuestionGroups item)
		{
			return me.db.SaveQuestionGroup(item);
		}

		public static int DeleteQuestionGroup(int question_group_id)
		{
			return me.db.DeleteQuestionGroup(question_group_id);
		}
	}
}

