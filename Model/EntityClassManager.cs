using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    /// <summary>
    /// <para>Builds and stores data for base classes in the game world.</para>
    /// <para>Classes can inherit each other (hence the build part).</para>
    /// <para>Built classes are returned by their name.</para>
    /// </summary>
    public class EntityClassManager
    {
        /// <summary>
        /// Stores the built classes' data.
        /// </summary>
        public Dictionary<string,JObject> EntityClassDictionary { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="json">JSON string containing an array of objects</param>
        public EntityClassManager(string json) : this(JsonConvert.DeserializeObject<JArray>(json))
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityClasses">preparsed JSON array</param>
        public EntityClassManager(JArray entityClasses)
        {
            EntityClassDictionary = BuildClasses(entityClasses);
        }

        /// <summary>
        /// Builds classes from a provided JArray object
        /// </summary>
        /// <param name="entityClasses"></param>
        /// <returns></returns>
        private Dictionary<string, JObject> BuildClasses(JArray entityClasses)
        {
            // Copy unbuilt classes to a dictionary
            var tempDictionary = entityClasses.Cast<JObject>().ToDictionary(x => (string)x["Class"], x => x);

            // Dictionary for storing the built classes
            var result = new Dictionary<string, JObject>();

            // Iterate over the unbuilt classes
            foreach (var className in tempDictionary.Keys)
            {
                BuildClass(className, tempDictionary, result);
            }
            return result;
        }

        /// <summary>
        /// <para>Builds a single class and adds it to targetDictionary.</para>
        /// <para>If class to be built inherits from another that still needs to be built
        /// It will also be built (and its ancestors).</para>
        /// </summary>
        /// <param name="className">Name of the class to be built</param>
        /// <param name="sourceDictionary">Dictionary of unbuilt classes</param>
        /// <param name="targetDictionary">Dictionary of built classes</param>
        /// <param name="watchdog">Watchdog for preventing infinite recursion</param>
        /// <returns></returns>
        private JObject BuildClass(string className, Dictionary<string, JObject> sourceDictionary, Dictionary<string, JObject> targetDictionary, int watchdog = 255)
        {
            if (watchdog == 0)
            {
                throw new Exception("Too much recursion");
            }
            watchdog--;

            // get the unbuilt class from the sourceDictionary
            var sourceClass = sourceDictionary[className];

            // if a built version already exists in targetDictionary, we can skipt the building process
            if (targetDictionary.ContainsKey(className))
            {
                return targetDictionary[className];
            }

            // prepare a brand new JObject to which we will copy the data (including ancestors if they exist)
            var targetClass = new JObject();

            // check if class has ancestors and build them if necessary
            var parentClassName = (string)sourceClass["Inherit"];
            if (parentClassName != null)
            {
                var parentClass = BuildClass(parentClassName, sourceDictionary, targetDictionary, watchdog);

                // inherit data from ancestor
                targetClass.Merge(parentClass);
            }

            // copy data from unbuilt version of the class
            targetClass.Merge(sourceClass);

            // add class to targetDictionary
            targetDictionary[className] = targetClass;

            return targetClass;
        }

        public EntityObject InstanceClass(string className)
        {
            var entityObject = new EntityObject(this, className);
            return entityObject;
        }
    }
}
