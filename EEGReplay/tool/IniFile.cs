using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Ini
{
    /// <summary>
    /// ini文件操作类
    /// </summary>
    public class IniFile
    {
         public Dictionary<string, string> configData;

         private string fullFileName;

         public string FullFileName
         {
             get { return this.fullFileName; }
         }

         public IniFile(string _fileName)
         {
             configData = new Dictionary<string, string> ();
             fullFileName = Application.StartupPath + @"\" + _fileName;

             bool hasCfgFile = File.Exists(fullFileName);
             if (hasCfgFile == false)
             {
                 StreamWriter writer = new StreamWriter(File.Create(fullFileName), Encoding.Default);
                 writer.Close();
             }
             StreamReader reader = new StreamReader(fullFileName, Encoding.Default);
             string line;
 
             int indx = 0;
             while ((line = reader.ReadLine()) != null)
             {
                 if (line.StartsWith(";") || string.IsNullOrEmpty(line))
                     configData.Add(";" + indx++, line);
                 else
                 {
                     string[] key_value = line.Split('=');
                     if (key_value.Length >= 2)
                         configData.Add(key_value[0], key_value[1]);
                     else
                         configData.Add(";" + indx++, line);
                 }
             }
             reader.Close();
         }
 
         public string get(string key)
         {
             if (configData.Count <= 0)
                 return null;
             else if(configData.ContainsKey(key))
                 return configData[key].ToString();
             else
                 return null;
         }
 
         public void set(string key, string value)
         {
             if (configData.ContainsKey(key))
                 configData[key] = value;
             else
                 configData.Add(key, value);

             // save();
         }
 
         public void save()
         {
             lock (this)
             {
                 // MessageBox.Show("fullFileName:" + fullFileName);
                 StreamWriter writer = new StreamWriter(fullFileName, false, Encoding.Default);
                 IDictionaryEnumerator enu = configData.GetEnumerator();
                 while (enu.MoveNext())
                 {
                     if (enu.Key.ToString().StartsWith(";"))
                         writer.WriteLine(enu.Value);
                     else
                         writer.WriteLine(enu.Key + "=" + enu.Value);
                 }
                 writer.Close();
             }
         }
    }
}
