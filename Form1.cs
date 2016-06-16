using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace BFtools
{
    public partial class Form1 : Form
    {
		private readonly string userProfilePath = Environment.GetEnvironmentVariable("userprofile") + @"\Documents\Battlefield 4\settings\PROF_SAVE_profile";
		private readonly string screenshotsPath = Environment.GetEnvironmentVariable("userprofile") + @"\Documents\Battlefield 4\screenshots";
		private readonly string tempSettings = Environment.GetEnvironmentVariable("temp") + @"\bf.ini";

		private readonly RegistryKey key;

		private string BFdirectory = "";

        public Form1()
        {
			InitializeComponent();

			key = Utility.GetRegistryKey();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            MaximumSize = Size;

			comboBox2.SelectedIndex = 1;
			comboBox3.SelectedIndex = 0;
            comboBox4.SelectedIndex = 0;
            comboBox5.SelectedIndex = 0;
            comboBox6.SelectedIndex = 0;
			
            if (File.Exists(userProfilePath))
            {
                string[] lines = File.ReadAllLines(userProfilePath);

                foreach (string line in lines)
                {
                    if (line.Contains("GstRender.VSyncEnabled 0"))
                        checkBox7.CheckState = CheckState.Unchecked;

                    if (line.Contains("GstRender.VSyncEnabled 1"))
                        checkBox7.CheckState = CheckState.Checked;
                }
            }

			if (File.Exists(tempSettings))
            {
				string[] lines = File.ReadAllLines(tempSettings);
                BFdirectory = lines[0];
                if (File.Exists(BFdirectory + @"\user.cfg"))
					ReadCFG();
            }

            if (key != null)
            {
                if (BFdirectory.Length == 0)
                    BFdirectory = (string)key.GetValue("Install Dir");

                if (File.Exists(BFdirectory + @"\user.cfg"))
                    ReadCFG();

                switch (key.GetValue("Locale").ToString())
                {
                    case "cs_CZ": comboBox1.SelectedIndex = 0; break;
                    case "de_DE": comboBox1.SelectedIndex = 1; break;
                    case "en_US": comboBox1.SelectedIndex = 2; break;
                    case "es_Es": comboBox1.SelectedIndex = 3; break;
                    case "fr_FR": comboBox1.SelectedIndex = 4; break;
                    case "it_IT": comboBox1.SelectedIndex = 5; break;
                    case "pl_PL": comboBox1.SelectedIndex = 6; break;
                    case "ko_KO": comboBox1.SelectedIndex = 7; break;
                    case "ja_JA": comboBox1.SelectedIndex = 8; break;
                    case "zh_ZH": comboBox1.SelectedIndex = 9; break;
                }
            }
            else
            {
                if (LoadPath())
                    ReadCFG();
            }

            if (comboBox3.SelectedIndex == 0)
            {
                float DXversion = Utility.GetDirectXVersion();

                if (DXversion < 11)
                {
                    comboBox3.SelectedIndex = 2;
                    comboBox5.SelectedIndex = 2;
                    comboBox6.SelectedIndex = 2;
                }
                else if (DXversion > 11)
                {
                    comboBox3.SelectedIndex = 1;
                    comboBox5.SelectedIndex = 1;
                    comboBox6.SelectedIndex = 1;
                }
                else // DXversion == 11
                {
                    comboBox3.SelectedIndex = 1;
                    comboBox5.SelectedIndex = 2;
                    comboBox6.SelectedIndex = 2;
                }
            }
        }

        private bool LoadPath()
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                BFdirectory = folderBrowserDialog1.SelectedPath;

                using (StreamWriter file = new StreamWriter(Environment.GetEnvironmentVariable("temp") + @"\bf4.ini", false))
                {
                    file.Write(BFdirectory);
                }
                return true;
            }

            return false;
        }

        private void SaveCFG()
        {
            using (StreamWriter file = new StreamWriter(BFdirectory + @"\user.cfg", false))
            {
				file.WriteLine("GameTime.MaxVariableFps " + numericUpDown1.Value);
				file.WriteLine("UI.DrawEnable " + checkBox4.Checked.ToInt());
				file.WriteLine("PerfOverlay.DrawFPS " + checkBox1.Checked.ToInt());
				file.WriteLine("PerfOverlay.DrawGraph " + checkBox8.Checked.ToInt());

                switch (checkBox7.CheckState)
                {
                    case CheckState.Checked:
                        file.WriteLine("RenderDevice.TripleBufferingEnable 1");
                        break;
                    case CheckState.Unchecked:
                        file.WriteLine("RenderDevice.TripleBufferingEnable 0");
                        break;
                }

                switch (comboBox3.SelectedIndex)
                {
                    case 1:
                        file.WriteLine("RenderDevice.Dx11Enable 1");
                        break;
                    case 2:
                        file.WriteLine("RenderDevice.Dx11Enable 0");
                        break;
                }

                switch (comboBox5.SelectedIndex)
                {
                    case 1:
                        file.WriteLine("RenderDevice.Dx11Dot1Enable 1");
                        break;
                    case 2:
                        file.WriteLine("RenderDevice.Dx11Dot1Enable 0");
                        break;
                }

                switch (comboBox6.SelectedIndex)
                {
                    case 1:
                        file.WriteLine("RenderDevice.Dx11Dot1RuntimeEnable 1");
                        break;
                    case 2:
                        file.WriteLine("RenderDevice.Dx11Dot1RuntimeEnable 0");
                        break;
                }

				if (comboBox4.SelectedIndex != 0)
					file.WriteLine("RenderDevice.ForceRenderAheadLimit " + (comboBox4.SelectedIndex - 1));

				file.WriteLine("WorldRender.DxDeferredCsPathEnable " + checkBox5.Checked.ToInt());
				file.WriteLine("WorldRender.TransparencyShadowmapsEnable " + checkBox9.Checked.ToInt());
				file.WriteLine("WorldRender.SpotlightShadowmapEnable " + checkBox2.Checked.ToInt());

                switch (comboBox2.SelectedIndex)
                {
                    case 0: file.WriteLine("WorldRender.SpotLightShadowmapResolution 64"); break;
                    case 1: file.WriteLine("WorldRender.SpotLightShadowmapResolution 256"); break;
                    case 2: file.WriteLine("WorldRender.SpotLightShadowmapResolution 1024"); break;
                    case 3: file.WriteLine("WorldRender.SpotLightShadowmapResolution 2048"); break;
                    case 4: file.WriteLine("WorldRender.SpotLightShadowmapResolution 4096"); break;
                    case 5: file.WriteLine("WorldRender.SpotLightShadowmapResolution 8192"); break;
                }
            }

            if (key != null)
            {
                switch (comboBox1.SelectedIndex)
                {
                    case 0: key.SetValue("Locale", "cs_CZ", RegistryValueKind.String); break;
                    case 1: key.SetValue("Locale", "de_DE", RegistryValueKind.String); break;
                    case 2: key.SetValue("Locale", "en_US", RegistryValueKind.String); break;
                    case 3: key.SetValue("Locale", "es_ES", RegistryValueKind.String); break;
                    case 4: key.SetValue("Locale", "fr_FR", RegistryValueKind.String); break;
                    case 5: key.SetValue("Locale", "it_IT", RegistryValueKind.String); break;
                    case 6: key.SetValue("Locale", "pl_PL", RegistryValueKind.String); break;
                    case 7: key.SetValue("Locale", "ko_KO", RegistryValueKind.String); break;
                    case 8: key.SetValue("Locale", "ja_JA", RegistryValueKind.String); break;
                    case 9: key.SetValue("Locale", "zh_ZH", RegistryValueKind.String); break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (BFdirectory.Length == 0 && !LoadPath())
                return;

            SaveCFG();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (File.Exists(BFdirectory + @"\user.cfg"))
                File.Delete(BFdirectory + @"\user.cfg");

			if (File.Exists(tempSettings))
				File.Delete(tempSettings);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            // kill Origin
            Process[] Origins = Process.GetProcessesByName("Origin");
            Utility.KillProcesses(Origins);

            // restart BF4
            Process BFstart = new Process();
            Process[] Bf4 = Process.GetProcessesByName("bf4");

            if (Bf4.Length > 0)
            {
                BFstart.StartInfo = Bf4[0].StartInfo;
                Utility.KillProcesses(Bf4, true);
            }
            else
            {
                BFstart.StartInfo.FileName = BFdirectory + @"\bf4.exe";
            }

            BFstart.Start();
        }

        private void ReadCFG()
        {
            string[] lines = File.ReadAllLines(@BFdirectory + @"\user.cfg");

            foreach (string line in lines)
            {
                if (line.Contains("GameTime.MaxVariableFps"))
                {
					string value = line.Replace("GameTime.MaxVariableFps ", "");

					string fps = value.Contains(".")
						? value.Remove(value.LastIndexOf('.'), value.Length - value.LastIndexOf('.'))
						: value;

					numericUpDown1.Value = Convert.ToDecimal(fps);
                }
                else if (line.Contains("RenderDevice.ForceRenderAheadLimit"))
				{
					string value = line.Replace("RenderDevice.ForceRenderAheadLimit ", "");
                    comboBox4.SelectedIndex = Convert.ToInt32(value) + 1;
                }

                else if (line.Contains("PerfOverlay.DrawFPS 1")) checkBox1.Checked = true;
                else if (line.Contains("PerfOverlay.DrawGraph 1")) checkBox8.Checked = true;

                else if (line.Contains("UI.DrawEnable 0")) checkBox4.Checked = false;

                else if (line.Contains("RenderDevice.Dx11Enable 1")) comboBox3.SelectedIndex = 1;
                else if (line.Contains("RenderDevice.Dx11Enable 0")) comboBox3.SelectedIndex = 2;

                else if (line.Contains("RenderDevice.Dx11Dot1Enable 1")) comboBox5.SelectedIndex = 1;
                else if (line.Contains("RenderDevice.Dx11Dot1Enable 0")) comboBox5.SelectedIndex = 2;

                else if (line.Contains("RenderDevice.Dx11Dot1RuntimeEnable 1")) comboBox6.SelectedIndex = 1;
                else if (line.Contains("RenderDevice.Dx11Dot1RuntimeEnable 0")) comboBox6.SelectedIndex = 2;

                else if (line.Contains("RenderDevice.TripleBufferingEnable 1")) checkBox7.CheckState = CheckState.Checked;
                else if (line.Contains("RenderDevice.TripleBufferingEnable 0")) checkBox7.CheckState = CheckState.Unchecked;

                else if (line.Contains("WorldRender.TransparencyShadowmapsEnable 0")) checkBox9.Checked = false;
                else if (line.Contains("WorldRender.SpotlightShadowmapEnable 0")) checkBox2.Checked = false;
                else if (line.Contains("WorldRender.DxDeferredCsPathEnable 0")) checkBox5.Checked = false;

                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 64")) comboBox2.SelectedIndex = 0;
                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 256")) comboBox2.SelectedIndex = 1;
                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 1024")) comboBox2.SelectedIndex = 2;
                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 2048")) comboBox2.SelectedIndex = 3;
                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 4096")) comboBox2.SelectedIndex = 4;
                else if (line.Contains("WorldRender.SpotLightShadowmapResolution 8192")) comboBox2.SelectedIndex = 5;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (!checkBox2.Checked)
				comboBox2.SelectedIndex = 0;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (BFdirectory.Length == 0)
            {
                if (LoadPath())
                    ReadCFG();
                else
                    return;
            }

            if (!File.Exists(BFdirectory + @"\user.cfg"))
            {
                File.Create(BFdirectory + @"\user.cfg");
            }

            Process.Start("notepad.exe", BFdirectory + @"\user.cfg");
        }

        private void button7_Click(object sender, EventArgs e)
        {
			Process.Start("explorer.exe", screenshotsPath);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = false;
            checkBox5.Checked = false;
            checkBox7.CheckState = CheckState.Unchecked;
            checkBox9.Checked = false;
            comboBox2.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            checkBox2.Checked = true;
            checkBox5.Checked = true;
            checkBox7.CheckState = CheckState.Checked;
            checkBox9.Checked = true;
            comboBox2.SelectedIndex = 5;
        }
    }
}
