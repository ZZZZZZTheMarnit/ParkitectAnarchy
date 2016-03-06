using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MiniJSON;

namespace Anarchy
{
    public class Main : IMod, IModSettings
    {
		private StreamWriter sw;
        private GameObject _go;
		private AnarchyObject Anar;
		private Dictionary<string, object> anarchy_settings;
		private Dictionary<string, string> settings_string = new Dictionary<string, string>();
		public Dictionary<string, bool> settings_bool = new Dictionary<string, bool>();
		private Type type;
		private bool isenabled = false;
		private string output;
		private int i;
		private int result;
		private bool hotkey_rebind = false;

		
		public void Init() {
			anarchy_settings = Json.Deserialize(File.ReadAllText(Path + @"/settings.json")) as Dictionary<string, object>;
			foreach (KeyValuePair<string, object> S in anarchy_settings) {
				type = S.Value.GetType();
				if(type==typeof(bool)) {
					settings_bool[S.Key] = bool.Parse(S.Value.ToString());
				} else {
					settings_string[S.Key] = S.Value.ToString();
				}
			}
		}
		
        public void onEnabled()
        {
			_go = new GameObject();
            Anar = _go.AddComponent<AnarchyObject>();
			Anar.settings = anarchy_settings;
			Anar.Path = Path;
			Anar.main = this;
			if((bool)anarchy_settings["anarchyEnabled"]) {
				Anar.Enable();
			}
			isenabled = true;
        }
		
		public void onDisabled()
        {
            Anar.Disable();
            UnityEngine.Object.Destroy(_go);
			isenabled = false;
        }
		
		public void onDrawSettingsUI()
		{
			foreach (KeyValuePair<string, object> S in anarchy_settings) {
				type = S.Value.GetType();
				GUILayout.BeginHorizontal();
				GUILayout.Label (S.Key, GUILayout.Width(175));
				if(S.Key == "anarchyHotkey") {
					bool hotkey = GUILayout.Button (settings_string[S.Key], GUILayout.Width(100));
					if(hotkey_rebind) {
						KeyCode key;
						if (FetchKey (out key)) {
							settings_string["anarchyHotkey"] = key.ToString();
							hotkey_rebind = false;
						}
					} else if(hotkey) {
						hotkey_rebind = true;
					}
				} else if(type==typeof(bool)) {
					settings_bool[S.Key] = GUILayout.Toggle (settings_bool[S.Key],"", GUILayout.Width(100));
				} else {
					settings_string[S.Key] = GUILayout.TextField (settings_string[S.Key], GUILayout.Width(100));
				}
				GUILayout.EndHorizontal();
			}
		}
		
		private bool FetchKey(out KeyCode outKey)
        {
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown (key)) {
                    outKey = key;
                    return true;
                }
            }
            outKey = KeyCode.A;
            return false;
        }
		
		public void onSettingsOpened()
		{

		}
		
		public void onSettingsClosed()
		{
			writeSettingsFile();
		}

		public void writeSettingsFile()
		{
			sw = File.CreateText(Path+@"/settings.json");
			sw.WriteLine("{");
			i = 0;
			foreach (KeyValuePair<string, object> S in anarchy_settings) {
				type = S.Value.GetType();
				i++;
				output = "	\""+S.Key+"\": ";
				if(type==typeof(bool)) {
					output += settings_bool[S.Key].ToString().ToLower();
				} else if(type==typeof(double)) {
					output += settings_string[S.Key];
					if(int.TryParse(settings_string[S.Key],out result)) {
						output += ".0";
					}
				} else if(type==typeof(string)) {
					output += "\""+settings_string[S.Key]+"\"";
				}
				if(i!=anarchy_settings.Count){
					output += ",";
				}
				sw.WriteLine(output);
			}
			sw.WriteLine("}");
			sw.Flush();
			sw.Close();	
			Init();
			if(isenabled==true) {
				if((bool)anarchy_settings["anarchyEnabled"]){
					Anar.settings = anarchy_settings;
					Anar.Enable();
				} else if(Anar.isenabled==true) {
					Anar.settings = anarchy_settings;
					Anar.Disable();
				}
			}
		}
		
        public string Name { get { return "Construction Anarchy"; } }
        public string Description { get { return "Lifts building restrictions for assets."; } }
        private string path;
		public string Path {
			get {
				return path;
			}
			set {
				path = value;
				Init();
			}
		}
        public string Identifier { get; set; }
		
		
		public void WriteToFile(string text) {
			sw = File.AppendText(Path+@"/mod.log");
			sw.WriteLine(text);
			sw.Flush();
			sw.Close();
		}
    }
}
