using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace BFtools
{
    public static class Utility
    {

        public static RegistryKey GetRegistryKey()
        {
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\EA Games\Battlefield 4", true);  // 64bit
            if (key == null || key.GetValue("Install Dir") == null)
                key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\EA GAMES\Battlefield 4", true);  // 32bit
            if (key == null || key.GetValue("Install Dir") == null)
                key = null;  // N/A

            return key;
        }

        public static void KilProcesses(IEnumerable<Process> processes, bool waitForExit = false)
        {
            foreach (var process in processes)
            {
                try
                {
                    process.Kill();

                    if (waitForExit)
                        process.WaitForExit();
                }
                catch
                {
                    // ignored
                }
            }
        }

        public static float GetDirectXVersion()
        {
            string tmpFile = Environment.GetEnvironmentVariable("temp") + @"\dxdiag.tmp";

            try
            {
                Process dxdiag = new Process
                {
                    StartInfo =
                    {
                        FileName = "dxdiag.exe",
                        Arguments = "/t " + tmpFile
                    }
                };
                dxdiag.Start();
                dxdiag.WaitForExit();

                if (File.Exists(tmpFile))
                {
                    string[] lines = File.ReadAllLines(tmpFile);
                    File.Delete(tmpFile);

                    foreach (string line in lines)
                    {
                        if (line.Contains("DirectX 10")) return 10;
                        if (line.Contains("DirectX 11")) return 11;
                        if (line.Contains("DirectX 11.1")) return 11.1f;
                        if (line.Contains("DirectX 11.2")) return 11.2f;
                        if (line.Contains("DirectX 11.3")) return 11.3f;
                        if (line.Contains("DirectX 12")) return 12;
                    }
                }
            }
            catch
            {
                // ignored
            }

            return 0;
        }
    }
}
