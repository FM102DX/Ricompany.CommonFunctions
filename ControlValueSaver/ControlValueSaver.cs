using System;
using System.Collections.Generic;
using System.IO;

namespace Ricompany.ControlValueSaver
{
    public class ControlValueSaver
    {
        //сохраняет данные с контролов в текстовый файл, чтобы при следующем запуске программы не вводить все заново

        private List<IControl> items = new List<IControl>();
        private string WorkingDir;
        private string Prefix;

        public ControlValueSaver(string workingDir, string prefix)
        {
            this.WorkingDir = workingDir;
            this.Prefix = prefix;
        }

        public string Path { get { return WorkingDir + @"\" + Prefix + "_saved_data.rifdcfile"; } }

        public void SaveIt()
        {
            try
            {
                StreamWriter sw = new StreamWriter(Path);
                foreach (IControl c in items)
                {
                    sw.WriteLine(c.Name + "=" + c.Text);
                }
                sw.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        private void SetCtrlValue(string ctrlName, string value)
        {
            foreach (IControl c in items)
            {
                if (c.Name == ctrlName)
                {
                    c.Text = value;
                    break;
                }
            }
        }

        public void AddSaverCtrl(IControl c)
        {
            items.Add(c);
        }

        public void LoadIt()
        {
            string[] _arr = new string[2];

            try
            {
                StreamReader sr = new StreamReader(Path);
                //Read the first line of text
                string line;
                List<string> _items = new List<string>();

                do
                {
                    line = sr.ReadLine();
                    _items.Add(line);

                } while (line != null);
                sr.Close();

                foreach (string s in _items)
                {
                    _arr = s.Split('=');
                    SetCtrlValue(_arr[0], _arr[1]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        public void clearItems()
        {
            items.Clear();
        }
    }
}
