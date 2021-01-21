using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest
{
    public class EntityManager
    {
        public Dictionary<string, dynamic> Entities { get; set; }

        public TypeManager TypeManager { get; set; }

        public EntityManager(TypeManager typeManager, string fileName)
        {
            TypeManager = typeManager;
            LoadTypes(fileName);
            //Types = new Dictionary<string, dynamic>();
        }

        public void LoadTypes(string fileName)
        {
            var text = File.ReadAllText(fileName);
            var entityArray = Newtonsoft.Json.JsonConvert.DeserializeObject<JObject[]>(text);
            Entities = entityArray.ToDictionary
            (
                entity => (string)entity["Name"],
                entity =>
                {
                    var mergedEntity = new JObject();
                    var typeName = (string)entity["Type"];
                    if (typeName != null)
                    {
                        var type = TypeManager.Types[typeName];
                        mergedEntity.Merge(type);
                    }
                    mergedEntity.Merge(entity);
                    return (dynamic)mergedEntity;
                }
            );
        }
    }
}
