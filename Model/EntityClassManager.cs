using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    public class EntityClassManager
    {
        public Dictionary<string,JObject> EntityClassDictionary { get; set; }

        public EntityClassManager(string json) : this(JsonConvert.DeserializeObject<JArray>(json))
        {
        }

        public EntityClassManager(JArray entityClasses)
        {
            EntityClassDictionary = BuildClasses(entityClasses);
        }

        private Dictionary<string, JObject> BuildClasses(JArray entityClasses)
        {
            var tempDictionary = entityClasses.Cast<JObject>().ToDictionary(x => (string)x["Class"], x => x);
            var result = new Dictionary<string, JObject>();
            foreach (var className in tempDictionary.Keys)
            {
                BuildClass(className, tempDictionary, result);
            }
            return result;
        }

        private JObject BuildClass(string className, Dictionary<string, JObject> sourceDictionary, Dictionary<string, JObject> targetDictionary, int watchdog = 255)
        {
            if (watchdog == 0)
            {
                throw new Exception("Too much recursion");
            }
            watchdog--;

            var sourceClass = sourceDictionary[className];

            if (targetDictionary.ContainsKey(className))
            {
                return targetDictionary[className];
            }
            var targetClass = new JObject();
            var parentClassName = (string)sourceClass["Inherit"];
            if (parentClassName != null)
            {
                var parentClass = BuildClass(parentClassName, sourceDictionary, targetDictionary, watchdog);
                targetClass.Merge(parentClass);
            }
            targetClass.Merge(sourceClass);
            targetDictionary[className] = targetClass;
            return targetClass;
        }
    }
}
