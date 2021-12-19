using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;

namespace BootChanger
{
    public partial class Form1 : Form
    {
        string s,g;
        public Form1()
        {
            InitializeComponent();
        }                      
        ///
        ///Mount EFI partition
        ///
        private void ESPMount()
        {
            richTextBox1.AppendText("Secure Boot Must Be Disabled.", Color.Red);
            richTextBox1.AppendText(Environment.NewLine+"Mounting EFI System Partition : ", Color.Yellow);
            try
            {
                ProcessStartInfo esp = new ProcessStartInfo()
                {
                    FileName = "cmd.exe",
                    Arguments = "/c mountvol Z: /S",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                };
                Process.Start(esp).WaitForExit();
                richTextBox1.AppendText("Done!", Color.LimeGreen);
            }
            catch (Exception ex)
            {
                richTextBox1.AppendText("Error!", Color.Red);
                MessageBox.Show(ex.ToString());
            }
        }
        ///
        /// Unmount EFI System Partition
        /// 
        private void ESPUnmount()
        {
            ProcessStartInfo esp_u = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = "/c mountvol Z: /D",
                UseShellExecute = false,
                CreateNoWindow = true,
            };
            Process.Start(esp_u).WaitForExit();
        }
        ///
        /// Execute bcdedit command
        ///
        private void Set_bootloader()
        {
            var p = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = g,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                }
            };
            p.Start();
            p.WaitForExit();
            string o= p.StandardOutput.ReadToEnd();
            richTextBox1.AppendText(Environment.NewLine+o,Color.LimeGreen);
        }
        ///
        /// List Current Bootloader folder
        ///
        public void List_Bootloader()
        {
            richTextBox1.AppendText(Environment.NewLine+"Scanning Bootloaders : ", Color.Yellow);
            if (Directory.Exists(@"Z:\EFI\Microsoft"))
            {
                if (File.Exists(@"Z:\EFI\Microsoft\Boot\bootmgfw.efi"))
                {
                    richTextBox1.AppendText(Environment.NewLine +"Found Windows Boot Manager", Color.LimeGreen);
                    comboBox1.Items.Add("Windows Boot Manager");
                }
            }
            if (Directory.Exists(@"Z:\EFI\CLOVER"))
            {
                if (File.Exists(@"Z:\EFI\CLOVER\CLOVERX64.efi"))
                {
                    richTextBox1.AppendText(Environment.NewLine +"Found Clover Bootloader", Color.LimeGreen);
                    comboBox1.Items.Add("Clover Bootloader");
                }
            }
            if (Directory.Exists(@"Z:\EFI\OC"))
            {
                if (File.Exists(@"Z:\EFI\OC\Opencore.efi"))
                {
                    richTextBox1.AppendText(Environment.NewLine+"Found Opencore Bootloader", Color.LimeGreen);
                    comboBox1.Items.Add("Opencore Bootloader");
                }
            }
            var gd = Directory.GetDirectories(@"Z:\EFI");
            foreach(string dir in gd)
            {
                if(dir!="Boot" && dir!="CLOVER" && dir!="OC" && dir != "Microsoft")
                {
                    if (File.Exists(dir + "\\grubx64.efi"))
                    {
                        richTextBox1.AppendText(Environment.NewLine+"Found Grub Bootloader", Color.LimeGreen);
                        comboBox1.Items.Add("Grub Bootloader");
                        s = Path.GetFullPath(dir + "\\grubx64.efi");                        
                    }
                }
            }
            comboBox1.SelectedIndex = 0;
         }
        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Current BootLoader List";
            richTextBox1.ReadOnly = true;
            ESPMount();
            List_Bootloader();
        }

        private void AppClose(object sender, FormClosingEventArgs e)
        {
            ESPUnmount();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.facebook.com/hackintoshmm");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            richTextBox1.AppendText(Environment.NewLine+"Setting Default Bootloader : ", Color.Yellow);
            if (comboBox1.SelectedItem.ToString().Contains("CLOVER"))
            {
                richTextBox1.AppendText("Clover", Color.LightGreen);
                g = "/c bcdedit /set {bootmgr} path EFI\\CLOVER\\CLOVERX64.efi";
            }
            else if (comboBox1.SelectedItem.ToString().Contains("Opencore"))
            {
                richTextBox1.AppendText("Opencore", Color.LightGreen);
                g = "/c bcdedit /set {bootmgr} path EFI\\OC\\Opencore.efi";
            }
            else if (comboBox1.SelectedItem.ToString().Contains("Windows"))
            {
                richTextBox1.AppendText("Windows Boot Manager", Color.LightGreen);
                g = "/c bcdedit /set {bootmgr} path EFI\\Microsoft\\Boot\\bootmgfw.efi";
            }
            else if (comboBox1.SelectedItem.ToString().Contains("Grub"))
            {
                richTextBox1.AppendText("Grub", Color.LightGreen);
                string pt=s.Replace("Z:\\","");
                g="/c bcdedit /set {bootmgr} path "+pt;
            }
            else
            {
                richTextBox1.AppendText("Select a Bootloader",Color.Red);               
            }
            Set_bootloader();
        }
    }
    public static class RichTextBoxExtension
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;
            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }
}
