using System;

public static class ActionExtension
{
	public static void SafeCall(this Action a)
	{
		if (a != null)
		{
			a();
		}
	}

	public static void SafeCall<T>(this Action<T> a, T variable)
	{
		if (a != null)
		{
			a(variable);
		}
	}
}