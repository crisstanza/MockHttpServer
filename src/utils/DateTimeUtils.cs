using constants;
using System;

namespace utils
{
	class DateTimeUtils
	{
		internal string Now()
		{
			return DateTime.Now.ToString(DateFormatConstants.DDMMYYYY_HHMMSSMICROS);
		}
	}
}
