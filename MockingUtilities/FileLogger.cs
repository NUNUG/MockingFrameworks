using MockingModels;
using System;

namespace MockingUtilities
{
	public class FileLogger : ILog
	{
		public LogLevel MinimumLevel => throw new NotImplementedException();

		public void Debug(string message) => throw new NotImplementedException();
		public void Error(string message) => throw new NotImplementedException();
		public void Error(string message, Exception e) => throw new NotImplementedException();
		public void Information(string message) => throw new NotImplementedException();
		public void Warning(string message) => throw new NotImplementedException();
	}
}
