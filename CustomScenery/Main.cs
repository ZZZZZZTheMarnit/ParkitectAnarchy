using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MiniJSON;

namespace Anarchy
{
    public class Main : IMod
    {
        private GameObject _go;
		private AnarchyObject Anar;
		private Dictionary<string, object> anarchy_settings;

        public void onEnabled()
        {
			anarchy_settings = Json.Deserialize(File.ReadAllText(Path + @"/settings.json")) as Dictionary<string, object>;
            _go = new GameObject();
            Anar = _go.AddComponent<AnarchyObject>();
			Anar.settings = anarchy_settings;
			Anar.Path = Path;
            Anar.Enable();
        }

        public void onDisabled()
        {
            Anar.Disable();
            UnityEngine.Object.Destroy(_go);
        }

        public string Name { get { return "Construction Anarchy"; } }
        public string Description { get { return "Lifts building restrictions for assets."; } }
        public string Path { get; set; }
        public string Identifier { get; set; {} }
    }
}
