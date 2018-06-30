using System;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace AutoUpdater
{
    class Program
    {
        static void Main(string[] args)
        {
            string url;
            if (args.Length == 0)
            {
                using (WebClient client = new WebClient())
                {
                    string v = client.DownloadString("http://blog.detolly.no/version.txt");
                    url = "http://blog.detolly.no/TwitchChat-" + v + ".zip";
                }
            }
            else {
                int start = args[0].IndexOf("--url=") + "--url=".Length;
                url = args[0].Substring(start);
            }
            try
            {
                using (WebClient client = new WebClient())
                {
                    Console.WriteLine("Downloading update..");
                    client.DownloadFile(url, "update.zip");
                    Console.WriteLine("Download successful.");
                }
                bool e = true;
                while (e)
                    if (System.Diagnostics.Process.GetProcessesByName("TwitchChatCoroutines").Length == 0)
                        e = false;
                    else
                    {
                        Console.WriteLine("Close TwitchChatCoroutines to continue");
                        System.Threading.Thread.Sleep(1000);
                    }

                if (File.Exists("TwitchChatCoroutines.exe"))
                {
                    File.Copy("TwitchChatCoroutines.exe", "TwitchChatCoroutines.exe.old");
                    Console.WriteLine("Deleting " + "TwitchChatCoroutines.exe");
                    System.Threading.Thread.Sleep(100);
                    File.Delete("TwitchChatCoroutines.exe");
                }
                if (File.Exists("Newtonsoft.Json.dll"))
                {
                    File.Copy("Newtonsoft.Json.dll", "Newtonsoft.Json.dll.old");
                    Console.WriteLine("Deleting " + "Newtonsoft.Json.dll");
                    System.Threading.Thread.Sleep(100);
                    File.Delete("Newtonsoft.Json.dll");
                }
                ZipFile.ExtractToDirectory("./update.zip", "./update");
                System.Threading.Thread.Sleep(100);
                var arr = Directory.GetFiles("./update");
                foreach (string a in arr)
                {
                    int u = a.IndexOf("./update\\") + "./update\\".Length;
                    string x = a.Substring(u);
                    if (x == "AutoUpdater.exe")
                    {
                        Directory.CreateDirectory("./.AutoUpdater");
                        File.Copy(a, "./.AutoUpdater/AutoUpdater.exe");
                        System.Threading.Thread.Sleep(100);
                        File.Delete(a);
                        continue;
                    }
                    File.Copy(a, "./" + x);
                    System.Threading.Thread.Sleep(100);
                    File.Delete(a);
                }
                Directory.Delete("./update");
                Console.WriteLine("Deleting update.zip");
                File.Delete("update.zip");
                System.Threading.Thread.Sleep(100);
                System.Diagnostics.Process.Start("TwitchChatCoroutines.exe");
                if (File.Exists("TwitchChatCoroutines.exe.old"))
                    File.Delete("TwitchChatCoroutines.exe.old");
                if (File.Exists("Newtonsoft.Json.dll.old"))
                    File.Delete("Newtonsoft.Json.dll.old");
            }
            catch (Exception e)
            {
                Console.WriteLine("Update Failed lol");
                Console.WriteLine(e.Message);
                if (File.Exists("update.zip"))
                    File.Delete("update.zip");
                if (Directory.Exists("./update"))
                {
                    foreach (string s in Directory.EnumerateFiles("./update"))
                    {
                        File.Delete(s);
                    }
                    Directory.Delete("./update");
                }
                Console.ReadKey();
            }
        }
    }
}
