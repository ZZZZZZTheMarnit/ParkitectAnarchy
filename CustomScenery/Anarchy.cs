using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using UnityEngine;

public class AnarchyObject : MonoBehaviour
{
	private Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
	private StreamWriter sw;
	private Type type;
	public string Path { get; set; }
	public Dictionary<string, object> settings { get; set; }
    public void Enable()
    {
        List<Deco> list =  AssetManager.Instance.getDecoObjects().ToList();
        foreach (Deco D in list) {
			data[D.ToString()] = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> S in settings)
			{
				try{
					data[D.ToString()].Add(S.Key, D.GetType().GetField(S.Key).GetValue(D).ToString());
					type = D.GetType().GetField(S.Key).GetValue(D).GetType();
					
					if(type==typeof(float)) {
						D.GetType().GetField(S.Key).SetValue(D,(float)(double)S.Value);
					} else if(type==typeof(bool)) {
						D.GetType().GetField(S.Key).SetValue(D,(bool)S.Value);
					} else {
						WriteToFile("Enable: "+type+": Type unsupported ("+S.Key+")");
					}
				} catch {
					WriteToFile("Enable: "+S.Key+": Field unsupported");
				}
			}
		}
    }
	public void Disable()
    {
        List<Deco> list =  AssetManager.Instance.getDecoObjects().ToList();
        foreach (Deco D in list) {
			foreach (KeyValuePair<string, string> S in data[D.ToString()])
			{
				try {
					type = D.GetType().GetField(S.Key).GetValue(D).GetType();
					if(type==typeof(float)) {
						D.GetType().GetField(S.Key).SetValue(D,float.Parse(S.Value));
					} else if(type==typeof(bool)) {
						D.GetType().GetField(S.Key).SetValue(D,bool.Parse(S.Value));
					} else {
						WriteToFile("Disable: "+type+": Type unsupported ("+S.Key+")");
					}
				} catch {
					WriteToFile("Disable: "+S.Key+": Field unsupported");
				}
			}
        }
    }
	public void WriteToFile(string text) {
		sw = File.AppendText(Path+@"/mod.log");
        sw.WriteLine(text);
        sw.Flush();
        sw.Close();
	}
}