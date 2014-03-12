using System;
using System.Collections.Generic;
using System.IO;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	public class AnswersRepositoryADO {
		AnswersDatabase db = null;
		protected static string dbLocation;		
		protected static AnswersRepositoryADO me;		

		static AnswersRepositoryADO ()
		{
			me = new AnswersRepositoryADO();
		}

		protected AnswersRepositoryADO ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new AnswersDatabase(dbLocation);
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

		public static Answers GetAnswer(int id)
		{
			return me.db.GetAnswer(id);
		}

		public static IEnumerable<Answers> GetAnswers (int answer_group_id)
		{
			return me.db.GetAnswers(answer_group_id);
		}

		public static int SaveAnswers (Answers item)
		{
			return me.db.SaveAnswer(item);
		}

		public static int DeleteAnswer(int id)
		{
			return me.db.DeleteAnswer(id);
		}
	}
}

