using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel;
using System.Diagnostics;

namespace WindowsFormsApplication4
{
    public partial class Form : System.Windows.Forms.Form
    {
        string pathToWatch = null;
        string pathToRaidHeroes = null;
        FolderBrowserDialog raidheroes;
        FolderBrowserDialog dialog;
        string speicherort;
        FileSystemWatcher watcher;
        delegate void SetTextCallback(string text);


        public Form()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)       //Find Raidheroes
        {
            
            speicherort = Application.StartupPath + "/verzeichnisspeicher.txt";

            if (File.Exists(speicherort))
            {
                string[] array = File.ReadAllLines(speicherort);
                pathToRaidHeroes = array[0];
                pathToWatch = array[1];
                Directory.CreateDirectory(pathToWatch + "/Export");
                Directory.SetCurrentDirectory(pathToWatch + "/Export");
                
            }
            else
            {
                pathToRaidHeroes = pathToWatch = null;
            }
        }

        private void Reset(object sender, EventArgs e)
        {
            pathToRaidHeroes = pathToWatch = null;
            watcher = null;
            File.Delete(speicherort);

        }

        private void SetText(string s)
        {

            if (this.textBox1.InvokeRequired)
            {
                SetTextCallback d = new SetTextCallback(SetText);
                this.Invoke(d, new object[] { s });
            }
            else
            {
                this.textBox1.Text = "Das letzte Element ist " + s;
            }

        }

        private void selectDirectorys(object sender, EventArgs e)
        {
            if (pathToRaidHeroes == null)
            {
                raidheroes = new FolderBrowserDialog();
                raidheroes.Description = "Bitte Raidheroes Ordner auswählen";
                frage:
                DialogResult dr = raidheroes.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    if (MessageBox.Show("Es wurde kein Ordner ausgewählt!", "erneut auswählen?", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        goto frage;
                    }
                    else { dialog.Dispose(); }

                }
                pathToRaidHeroes = raidheroes.SelectedPath;

                Console.WriteLine(pathToRaidHeroes);
                                                              
            }

             secondpath:
            if (pathToWatch == null)
            {
                dialog = new FolderBrowserDialog();
                dialog.Description = "Bitte Ordner auswählen";

                dialog:
                DialogResult dr = dialog.ShowDialog();
                if (dr == DialogResult.Cancel)
                {
                    if (MessageBox.Show("Es wurde kein Ordner ausgewählt!", "erneut auswählen?", MessageBoxButtons.YesNo) == DialogResult.Yes) { 
                        goto dialog;
                    }
                    else
                    {
                        dialog.Dispose();
                    }

                }
                pathToWatch = dialog.SelectedPath;
                Console.WriteLine(pathToWatch);
                Directory.CreateDirectory(pathToWatch + "/Export");
                Directory.SetCurrentDirectory(pathToWatch + "/Export");

            }
            if (!File.Exists(speicherort))
            {
                StreamWriter sw = File.CreateText(speicherort);
                sw.WriteLine(pathToRaidHeroes);
                sw.WriteLine(pathToWatch);
                sw.Close();
            }

            watcher = new FileSystemWatcher(pathToWatch, "*.zip") { EnableRaisingEvents = true, IncludeSubdirectories = true };

            watcher.NotifyFilter = NotifyFilters.FileName;

            watcher.Created += (sender1, e1) =>
            {
                Console.WriteLine(e1.Name);
                Console.WriteLine(e1.FullPath);
                try
                {
                    string strCmdText = string.Format("/C {0}/raid_heroes {1}", pathToRaidHeroes, e1.FullPath);
                    Console.Write(strCmdText);
                    System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                    SetText(e1.Name);      
                    

                }
                catch (Exception)
                {

                    throw;
                }

            };

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", Application.StartupPath);  
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ActiveForm.Close();
        }
    }
}
