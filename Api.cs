using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using CloudyApis;
using DiscordRPC;
using DiscordRPC.Events;
using DiscordRPC.Message;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CloudyApi;

public static class Api
{
	public static class External
	{
		public struct ClientInfo
		{
			public string version;

			public string name;

			public int id;
		}

		private static Timer time12;

		private static bool isua;

		private static bool _autoInject;

		public static bool isRegistred;

		public static string i;

		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern void Initialize();

		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
		private static extern void ExecuteAsync(byte[] scriptSource, string[] clientUsers, int numUsers);

		[DllImport("bin\\Cloudy.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr GetClients();

		static External()
		{
			//IL_0000: Unknown result type (might be due to invalid IL or missing references)
			//IL_000a: Expected O, but got Unknown
			time12 = new Timer();
			isua = false;
			isRegistred = false;
			i = "";
			AutoSetup();
		}

		public static void AutoInject(bool enable)
		{
			if (misc.isRobloxOpen() && misc.CheckRobloxVersion())
			{
				Initialize();
				_autoInject = enable;
				if (enable)
				{
					inject();
				}
			}
		}

		public static bool IsAutoInjectEnabled()
		{
			return _autoInject;
		}

		public static void inject()
		{
			if (misc.isRobloxOpen() && misc.CheckRobloxVersion() && isRegistred)
			{
				Initialize();
				Thread.Sleep(2000);
				string s = "\tgame:GetService(\"StarterGui\"):SetCore(\"SendNotification\", {\r\n\t\tTitle = \"" + i + "\",\r\n\t\tText = \"Injected!\"\r\n\t})";
				string[] array = (from c in GetClientsList()
					select c.name).ToArray();
				ExecuteAsync(Encoding.UTF8.GetBytes(s), array, array.Length);
			}
		}

		public static void execute(string scriptSource)
		{
			string[] array = (from c in GetClientsList()
				select c.name).ToArray();
			ExecuteAsync(Encoding.UTF8.GetBytes(scriptSource), array, array.Length);
		}

		public static List<ClientInfo> GetClientsList()
		{
			List<ClientInfo> list = new List<ClientInfo>();
			IntPtr clients = GetClients();
			while (true)
			{
				ClientInfo item = Marshal.PtrToStructure<ClientInfo>(clients);
				if (item.name != null)
				{
					list.Add(item);
					clients += Marshal.SizeOf<ClientInfo>();
					continue;
				}
				break;
			}
			return list;
		}

		public static void RegisterExecutor(string injectionMessage)
		{
			i = injectionMessage;
			isRegistred = true;
		}

		public static bool IsInjected()
		{
			try
			{
				return GetClientsList().Count > 0;
			}
			catch
			{
				return false;
			}
		}

		private static void AutoSetup()
		{
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			string[] array = new string[5] { "Cloudy.dll", "libcrypto-3-x64.dll", "libssl-3-x64.dll", "xxhash.dll", "zstd.dll" };
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin");
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string text3 = Path.Combine(text, text2);
				if (File.Exists(text3))
				{
					continue;
				}
				try
				{
					string address = "https://github.com/CloudyExecugor/frontend/releases/download/reareaaaa/" + text2;
					using WebClient webClient = new WebClient();
					webClient.DownloadFile(address, text3);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to Download " + text2 + ": " + ex.Message, "CloudyApi");
				}
			}
		}
	}

	public static class Internal
	{
		static Internal()
		{
			AutoSetup();
			CreateCLDYWorkspace();
			Configs.executeP = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "execute.txt");
			Configs.SetupFileWatcher();
		}

		public static async void CreateCLDYWorkspace()
		{
			string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
			string targetPath = Path.Combine(localAppData, "CLDY", "Workspace");
			if (!Directory.Exists(targetPath))
			{
				Directory.CreateDirectory(targetPath);
			}
		}

		public static void inject()
		{
			//IL_0057: Unknown result type (might be due to invalid IL or missing references)
			try
			{
				Process process = new Process
				{
					StartInfo = 
					{
						FileName = "Injector.exe",
						UseShellExecute = false,
						CreateNoWindow = true
					}
				};
				process.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error starting injector: " + ex.Message, "Error", (MessageBoxButtons)0, (MessageBoxIcon)16);
			}
		}

		public static void execute(string script)
		{
			try
			{
				string directoryName = Path.GetDirectoryName(Configs.executeP);
				if (!Directory.Exists(directoryName))
				{
					Directory.CreateDirectory(directoryName);
				}
				File.WriteAllText(Configs.executeP, script);
			}
			catch (Exception)
			{
			}
		}

		private static void AutoSetup()
		{
			//IL_00d8: Unknown result type (might be due to invalid IL or missing references)
			string[] array = new string[3] { "Injector.exe", "Module.dll", "fmt.dll" };
			string[] array2 = new string[3] { "Injector.exe", "Module.dll", "fmt.dll" };
			string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
			if (!Directory.Exists(baseDirectory))
			{
				Directory.CreateDirectory(baseDirectory);
			}
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				string path = array2[i];
				string text2 = Path.Combine(baseDirectory, path);
				if (File.Exists(text2))
				{
					continue;
				}
				try
				{
					string address = "https://github.com/CloudyExecugor/frontend/releases/download/reareaaaa/" + text;
					using WebClient webClient = new WebClient();
					webClient.DownloadFile(address, text2);
				}
				catch (Exception ex)
				{
					MessageBox.Show("An error occurred while downloading required files. ERR: " + ex.Message);
				}
			}
		}
	}

	public static class misc
	{
		[Serializable]
		[CompilerGenerated]
		private sealed class _003C_003Ec
		{
			public static readonly _003C_003Ec _003C_003E9 = new _003C_003Ec();

			public static OnReadyEvent _003C_003E9__12_0;

			public static Func<string, DateTime> _003C_003E9__21_0;

			public static Func<string, long> _003C_003E9__21_1;

			internal void _003CSetDiscordRpc_003Eb__12_0(object sender, ReadyMessage e)
			{
			}

			internal DateTime _003CcPlaceId_003Eb__21_0(string f)
			{
				return new FileInfo(f).LastWriteTime;
			}

			internal long _003CcPlaceId_003Eb__21_1(string f)
			{
				return new FileInfo(f).Length;
			}
		}

		public static string editorUri;

		private static readonly HttpClient client;

		private static DiscordRpcClient clients;

		private static DiscordRpcClient clientas;

		static misc()
		{
			//IL_000a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0014: Expected O, but got Unknown
			editorUri = "https://getcloudy.xyz/Editor";
			client = new HttpClient();
			AutoSetup();
			((HttpHeaders)client.DefaultRequestHeaders).Add("Authorization", "v1USERFREE");
		}

		public static void disableSecurity()
		{
			Configs.isSec = false;
		}

		public static void credits()
		{
			//IL_000b: Unknown result type (might be due to invalid IL or missing references)
			MessageBox.Show("This api was made by @volxphy. Thanks to @kaos for contributing to the internal module (sigma) and also thanks to @nuageux for making such an awesome UI for cloudy!", "skibidi");
		}

		private static string GetUserIdFromUsername(string username)
		{
			//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
			string address = "https://users.roblox.com/v1/usernames/users";
			string data = "{\"usernames\": [\"" + username + "\"]}";
			try
			{
				using WebClient webClient = new WebClient();
				webClient.Headers[HttpRequestHeader.ContentType] = "application/json";
				string text = webClient.UploadString(address, "POST", data);
				JObject val = JObject.Parse(text);
				if (val["data"] != null && val["data"].HasValues)
				{
					return ((object)val["data"][(object)0][(object)"id"]).ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error getting user ID for " + username + ": " + ex.Message, "CloudyApi");
			}
			return null;
		}

		public static BitmapImage GetAvatar(string username)
		{
			//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
			//IL_009e: Unknown result type (might be due to invalid IL or missing references)
			//IL_00a5: Expected O, but got Unknown
			string userIdFromUsername = GetUserIdFromUsername(username);
			if (string.IsNullOrEmpty(userIdFromUsername))
			{
				return null;
			}
			string address = "https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds=" + userIdFromUsername + "&size=420x420&format=png";
			try
			{
				using WebClient webClient = new WebClient();
				string text = webClient.DownloadString(address);
				JObject val = JObject.Parse(text);
				if (val["data"] != null && val["data"].HasValues)
				{
					string uriString = ((object)val["data"][(object)0][(object)"imageUrl"]).ToString();
					return new BitmapImage(new Uri(uriString));
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error getting avatar for " + username + ": " + ex.Message, "CloudyApi");
			}
			return null;
		}

		public static string SetScriptJS(string script)
		{
			return "setText(\"" + HttpUtility.JavaScriptStringEncode(script) + "\")";
		}

		public static string GetTextJS()
		{
			return "getText()";
		}

		public static string AddTabJS(string name, string content)
		{
			return "addTab(\"" + HttpUtility.JavaScriptStringEncode(name) + "\", \"" + HttpUtility.JavaScriptStringEncode(content) + "\")";
		}

		public static bool isRobloxOpen()
		{
			return Process.GetProcessesByName("RobloxPlayerBeta").Any();
		}

		public static void SetDiscordRpc(string title, string appid, string img, string state)
		{
			//IL_0002: Unknown result type (might be due to invalid IL or missing references)
			//IL_000c: Expected O, but got Unknown
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0053: Unknown result type (might be due to invalid IL or missing references)
			//IL_005b: Unknown result type (might be due to invalid IL or missing references)
			//IL_0067: Unknown result type (might be due to invalid IL or missing references)
			//IL_0068: Unknown result type (might be due to invalid IL or missing references)
			//IL_006d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			//IL_0082: Expected O, but got Unknown
			//IL_0088: Expected O, but got Unknown
			//IL_0025: Unknown result type (might be due to invalid IL or missing references)
			//IL_002a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0030: Expected O, but got Unknown
			clients = new DiscordRpcClient(appid);
			DiscordRpcClient obj = clients;
			object obj2 = _003C_003Ec._003C_003E9__12_0;
			if (obj2 == null)
			{
				OnReadyEvent val = delegate
				{
				};
				_003C_003Ec._003C_003E9__12_0 = val;
				obj2 = (object)val;
			}
			obj.OnReady += (OnReadyEvent)obj2;
			clients.Initialize();
			clients.SetPresence(new RichPresence
			{
				Details = title,
				State = state,
				Timestamps = Timestamps.Now,
				Assets = new Assets
				{
					LargeImageKey = img,
					LargeImageText = title
				}
			});
		}

		public static async Task<string> GetDiscordUsername(string appId)
		{
			TaskCompletionSource<string> tcs = new TaskCompletionSource<string>();
			clientas = new DiscordRpcClient(appId);
			clientas.OnReady += (OnReadyEvent)delegate(object sender, ReadyMessage e)
			{
				string result = $"{e.User.Username}#{e.User.Discriminator}";
				tcs.SetResult(result);
			};
			clientas.Initialize();
			clientas.SetPresence(new RichPresence
			{
				Details = "Username Validation...",
				State = "Please wait",
				Timestamps = Timestamps.Now
			});
			return await tcs.Task;
		}

		public static bool CheckRobloxVersion()
		{
			//IL_0020: Unknown result type (might be due to invalid IL or missing references)
			//IL_0079: Unknown result type (might be due to invalid IL or missing references)
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			if (processesByName.Length == 0)
			{
				MessageBox.Show("Roblox is not running.", "CloudyApi");
				return false;
			}
			string fileName = processesByName[0].MainModule.FileName;
			string name = new DirectoryInfo(Path.GetDirectoryName(fileName)).Name;
			if (name != Configs.SupportedRobloxVersion)
			{
				if (!Configs.isSec)
				{
					return true;
				}
				MessageBox.Show("CloudyApi is Outdated. Please Update https://getcloudy.xyz", "CloudyApi");
				return false;
			}
			return true;
		}

		public static string GetUsername()
		{
			//IL_0085: Unknown result type (might be due to invalid IL or missing references)
			string userName = Environment.UserName;
			string path = "C:\\\\Users\\\\" + userName + "\\\\AppData\\\\Local\\\\Roblox\\\\LocalStorage\\\\appStorage.json";
			if (!File.Exists(path))
			{
				return null;
			}
			try
			{
				string text = File.ReadAllText(path);
				JObject val = JObject.Parse(text);
				if (val.ContainsKey("Username"))
				{
					return ((object)val["Username"])?.ToString();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("Error getting username " + ex.Message, "CloudyApi");
			}
			return null;
		}

		public static void killRoblox()
		{
			Process[] processesByName = Process.GetProcessesByName("RobloxPlayerBeta");
			foreach (Process process in processesByName)
			{
				try
				{
					process.Kill();
					process.WaitForExit();
				}
				catch (Exception ex)
				{
					Console.WriteLine("Failed to kill process: " + ex.Message);
				}
			}
		}

		public static async Task<string> AskAi(string input)
		{
			try
			{
				StringContent reqbody = new StringContent("{\"input\":\"" + input + "\"}", Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync("http://37.114.56.124:5000/v1/cloudy/requests", (HttpContent)(object)reqbody);
				if (response.IsSuccessStatusCode)
				{
					JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync());
					return "AI is currently not available: 'AI WAS DISABLED BY ADMINISTRATOR' ";
				}
				return "Error: No valid response ";
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				return "Error: " + ex.Message;
			}
		}

		public static string Cleanres(string res)
		{
			return Regex.Replace(res, "^```lua\\s*(.*?)\\s*```$", "$1", RegexOptions.Singleline);
		}

		private static void AutoSetup()
		{
			//IL_00c8: Unknown result type (might be due to invalid IL or missing references)
			string[] array = new string[2] { "Newtonsoft.Json.dll", "DiscordRPC.dll" };
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
			if (!Directory.Exists(text))
			{
				Directory.CreateDirectory(text);
			}
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				string text3 = Path.Combine(text, text2);
				if (File.Exists(text3))
				{
					continue;
				}
				try
				{
					string address = "https://github.com/CloudyExecugor/frontend/releases/download/reareaaaa/" + text2;
					using WebClient webClient = new WebClient();
					webClient.DownloadFile(address, text3);
				}
				catch (Exception ex)
				{
					MessageBox.Show("Failed to Download " + text2 + ": " + ex.Message, "CloudyApi");
				}
			}
		}

		public static string cPlaceId()
		{
			try
			{
				string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Roblox", "logs");
				List<string> source = (from f in Directory.GetFiles(path, "*.log")
					orderby new FileInfo(f).LastWriteTime descending, new FileInfo(f).Length descending
					select f).ToList();
				Regex regex = new Regex("placeIds=(\\d+)", RegexOptions.IgnoreCase);
				Regex regex2 = new Regex("placeid:(\\d+)", RegexOptions.IgnoreCase);
				foreach (string item in source.Take(5))
				{
					using FileStream stream = new FileStream(item, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					using StreamReader streamReader = new StreamReader(stream);
					string text = null;
					string input;
					while ((input = streamReader.ReadLine()) != null)
					{
						Match match = regex.Match(input);
						if (match.Success)
						{
							text = match.Groups[1].Value;
						}
						Match match2 = regex2.Match(input);
						if (match2.Success)
						{
							text = match2.Groups[1].Value;
						}
					}
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
				}
				return null;
			}
			catch (Exception ex)
			{
				return "Error: " + ex.Message;
			}
		}

		public static async Task<string> cPlaceName(string placeId)
		{
			try
			{
				HttpClient httpClient = new HttpClient();
				try
				{
					string url = "https://www.roblox.com/games/" + placeId;
					HttpResponseMessage response = await httpClient.GetAsync(url);
					if (!response.IsSuccessStatusCode)
					{
						return $"Failed to retrieve data: {response.StatusCode}";
					}
					Match titleMatch = Regex.Match(await response.Content.ReadAsStringAsync(), "<title>(.+?) - Roblox</title>", RegexOptions.IgnoreCase);
					if (titleMatch.Success)
					{
						return titleMatch.Groups[1].Value.Trim();
					}
					return "Place name not found in page content.";
				}
				finally
				{
					((IDisposable)httpClient)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				return "Error: " + ex.Message;
			}
		}

		public static async Task<string> cPlaceImage(string placeId)
		{
			try
			{
				HttpClient httpClient = new HttpClient();
				try
				{
					string url = "https://thumbnails.roblox.com/v1/assets?assetIds=" + placeId + "&format=Png&size=768x432";
					HttpResponseMessage response = await httpClient.GetAsync(url);
					if (!response.IsSuccessStatusCode)
					{
						return $"Failed to retrieve image: {response.StatusCode}";
					}
					Match match = Regex.Match(await response.Content.ReadAsStringAsync(), "\"imageUrl\":\\s*\"(https:[^\"]+)\"");
					if (match.Success)
					{
						return match.Groups[1].Value;
					}
					return "Image URL not found.";
				}
				finally
				{
					((IDisposable)httpClient)?.Dispose();
				}
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				return "Error: " + ex.Message;
			}
		}
	}
}
