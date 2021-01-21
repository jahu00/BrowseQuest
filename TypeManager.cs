using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest
{
    public class TypeManager
    {
        public Dictionary<string, JObject> Types { get; set; }


        public TypeManager(string fileName)
        {
            LoadTypes(fileName);
            //Types = new Dictionary<string, dynamic>();
        }

        public void LoadTypes(string fileName)
        {
            var text = File.ReadAllText(fileName);
            var typeArray = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject[]>(text);
            Types = typeArray.ToDictionary(x => (string)x["Name"], x => x);
        }
    }
}
