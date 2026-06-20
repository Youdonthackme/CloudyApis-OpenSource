using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ForlornApi;

public static class Api
{
	private static Timer time1;

	private static Forlorn forlorn;

	static Api()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Expected O, but got Unknown
		time1 = new Timer();
		CreateForlorn();
		time1.Tick += ticktimer32433;
		time1.Start();
	}

	private static void CreateForlorn()
	{
		forlorn = new Forlorn();
	}

	public static void Inject()
	{
		forlorn?.InjectForlorn();
	}

	public static void KillRoblox()
	{
		forlorn?.KillRoblox();
	}

	public static bool IsInjected()
	{
		return forlorn?.IsInjected() ?? false;
	}

	public static bool IsRobloxOpen()
	{
		return Process.GetProcessesByName("RobloxPlayerBeta").Length != 0;
	}

	public static string[] GetActiveClientNames()
	{
		return forlorn?.GetActiveClientNames();
	}

	public static void ExecuteScript(string script)
	{
		forlorn?.ExecuteScript(script);
	}

	private static void ticktimer32433(object sender, EventArgs e)
	{
		if (!IsRobloxOpen())
		{
			if (forlorn != null)
			{
				forlorn.Deject();
				forlorn = null;
			}
		}
		else if (forlorn == null)
		{
			CreateForlorn();
		}
	}

	public static void SetAutoInject(bool value)
	{
		forlorn?.AutoInject(value);
	}
}
