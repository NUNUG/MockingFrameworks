using System;
using System.Collections.Generic;
using System.Text;

namespace MockingModels
{
	public interface ILog
	{
		void Debug(string message);
		void Information(string message);
		void Warning(string message);
		void Error(string message);
		void Error(string message, Exception e);
	}
}
