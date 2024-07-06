using EnumExtendLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoggerLibrary.Enumeration
{
	public enum LogLevel
	{
		[EnumDisplayName("Unknown")]Unknown = -1,
		[EnumDisplayName("Error  ")]Error = 0,
		[EnumDisplayName("Warning")]Warning,
		[EnumDisplayName("Operate")]Operate,
		[EnumDisplayName("Action ")]Action,
		[EnumDisplayName("Trace  ")]Trace,
		[EnumDisplayName("Debug  ")]Debug,

		End
	}
}
