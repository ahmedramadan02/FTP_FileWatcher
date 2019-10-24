# FTP_FileWatcher

- This Project consists of three projects
1. FTP_FileWatcherLib - This is the main project, it's a library type.
2. FTP_FileWatcherApp - This is a console app as appliaction layer for our library
3. FTP_FileWatcher.WinApp - This is a windows forms app as application layer for our library

- What do you need to run these projects ?
1. You need DOTNET framework 4.5
2. SQLite library for .NET (I provided it in the bin folder of the FTP_FileWatcherLib, so, DON'T delete it)
3. You need to access my folder on the FTPServer, downlaoding it, then modify the UpdateTime
of the files to be 1 hour lagged!. (this option if you want!. Of course the app will work without it)

Important Note: In order to deal with time in SQLite DB
	A. Change time in the following format YYYY-MM-DD HH:MM:SS
	B. the time system is 24 hour not 12 hour based system (Important)
	C. GoTO https://www.sqlite.org/lang_datefunc.html


- What will happen when you run this app !?
1. The application will set the workspace
	Creating "./SDE_Assignment", "./SDE_Assignment/Download", and "./SDE_Assignment/tmp" as default folders to work on.
	Download folder for new files on the server
	tmp is for any temporary files like sqlite DB file
2. It will connect to the server
3. Downlading the SQLite file
4. Query the DB file for any new files
5. If (newFiles) then
	Create new folder to download on i.e. ./SDE_Assignment/Download/SDE-2015614-22
	Downloading files in separate threads
6. GoTO 3 after 1 Hour


-Finally, You can find the structure of the library, as ClassDiagram @ the same project.
