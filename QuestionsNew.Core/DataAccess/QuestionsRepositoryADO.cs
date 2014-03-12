using System;
using System.Collections.Generic;
using System.IO;
using QuestionsNew.Core.Model;

namespace QuestionsNew.Core.DataAccess {
	public class QuestionsRepositoryADO {
		QuestionsDatabase db = null;
		protected static string dbLocation;		
		protected static QuestionsRepositoryADO me;		

		static QuestionsRepositoryADO ()
		{
			me = new QuestionsRepositoryADO();
		}

		protected QuestionsRepositoryADO ()
		{
			// set the db location
			dbLocation = DatabaseFilePath;

			// instantiate the database	
			db = new QuestionsDatabase(dbLocation);
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

		public static Questions GetQuestion(int id)
		{
			return me.db.GetQuestion(id);
		}

		public static IEnumerable<Questions> GetQuestions (int id)
		{
			return me.db.GetQuestions(id);
		}

		public static int SaveQuestions (Questions item)
		{
			return me.db.SaveQuestion(item);
		}

		public static int DeleteQuestion(int id)
		{
			return me.db.DeleteQuestion(id);
		}
	}
}

