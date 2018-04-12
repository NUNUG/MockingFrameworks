using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public enum LogLevel
	{
		Debug = 1,
		Information,
		Warning,
		Error
	}

	public interface ILog
	{
		LogLevel MinimumLevel { get; }

		void Debug(string message);
		void Information(string message);
		void Warning(string message);
		void Error(string message);
		void Error(string message, Exception e);
	}
}
