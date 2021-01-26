using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    public class EntityObject
    {
        private JObject ClassData { get; set; }

        private JObject ObjectData { get; set; }

        public string Guid
        {
            get
            {
                return (string)GetProperty("Guid");
            }
            private set
            {
                ObjectData["Guid"] = value;
            }
        }

        public string Id
        {
            get
            {
                return (string)GetProperty("Id");
            }
            set
            {
                ObjectData["Id"] = value;
            }
        }

        public string Name
        {
            get
            {
                return (string)GetProperty("Name");
            }
            set
            {
                ObjectData["Name"] = value;
            }
        }

        public string Class
        {
            get
            {
                return (string)GetProperty("Class");
            }
            set
            {
                ObjectData["Class"] = value;
            }
        }

        public string Image
        {
            get
            {
                return (string)GetProperty("Image");
            }
            set
            {
                ObjectData["Image"] = value;
            }
        }

        public string Link
        {
            get
            {
                return (string)GetProperty("Link");
            }
            set
            {
                ObjectData["Link"] = value;
            }
        }

        public bool? CanEnter
        {
            get
            {
                return (bool?)GetProperty("CanEnter");
            }
            set
            {
                ObjectData["CanEnter"] = value;
            }
        }

        public bool? Visible
        {
            get
            {
                return (bool?)GetProperty("Visible");
            }
            set
            {
                ObjectData["Visible"] = value;
            }
        }

        public Int64? Size
        {
            get
            {
                return (Int64?)GetProperty("Size");
            }
            set
            {
                ObjectData["Size"] = value;
            }
        }

        public Int64? Space
        {
            get
            {
                return (Int64?)GetProperty("Space");
            }
            set
            {
                ObjectData["Space"] = value;
            }
        }

        public Int64 OccupiedSpace
        {
            get
            {
                return Children?.Sum(child => child.Size ?? 0) ?? 0;
            }
        }

        /*public int? Age
        {
            get
            {
                return (int?)GetProperty("Age");
            }
            set
            {
                ObjectData["Age"] = value;
            }
        }*/

        public List<EntityObject> Children { get; set; }

        #region Computed properties
        public string DisplayName
        {
            get
            {
                return Name ?? Class ?? "???";
            }
        }

        public string ImageName
        {
            get
            {
                return Image ?? Class;
            }
        }
        #endregion

        public object GetProperty(string name)
        {
            object value = null;
            if (ObjectData?.ContainsKey(name) == true)
            {
                value = ((JValue)ObjectData[name]).Value;
            }

            if (value == null && ClassData?.ContainsKey(name) == true)
            {
                value = ((JValue)ClassData[name]).Value;
            }

            return value;
        }

        /*public T GetProperty<T>(string name)
        {
            var value = GetProperty(name);
        }*/

        public EntityObject(string json, EntityClassManager classManager) : this(JsonConvert.DeserializeObject<JObject>(json), classManager)
        {
        }

        public EntityObject(JObject objectData, EntityClassManager classManager)
        {
            ObjectData = (JObject)objectData.DeepClone();
            JObject classData;
            if (classManager.EntityClassDictionary.TryGetValue(Class, out classData))
            {
                ClassData = classData;
            }
            if (Guid == null)
            {
                Guid = System.Guid.NewGuid().ToString("N");
            }

            if (ObjectData.ContainsKey("Children"))
            {
                Children = new List<EntityObject>();
                foreach(JObject child in (JArray)ObjectData["Children"])
                {
                    Children.Add(new EntityObject(child, classManager));
                }
                ObjectData["Children"] = null;
            }
        }

        public JObject Serialize()
        {
            var result = (JObject)ObjectData.DeepClone();
            if (Children != null)
            {
                var children = new JArray();
                foreach(var child in Children)
                {
                    children.Add(child.Serialize());
                }
                result["Children"] = children;
            }
            return result;
        }

        public EntityObject Query(string path, out string prettyPath)
        {
            EntityObject objectToSearchIn = this;
            EntityObject result = null;

            var prettyPathParts = new List<string>();

            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }
            var parts = path.Split('/');

            do
            {
                result = objectToSearchIn.Children?.FirstOrDefault(child => QueryPredicate(child, parts[0]));

                if (result == null)
                {
                    prettyPath = null;
                    return null;
                }

                prettyPathParts.Add(result.DisplayName);

                parts = parts.Skip(1).ToArray();
                objectToSearchIn = result;

            } while (parts.Count() > 0);

            prettyPath = string.Join("/", prettyPathParts);
            return result;


            /*if (parts[0] != Guid && parts[0] != Id)
            {
                return null;
            }
            if (parts.Count() == 1)
            {
                prettyPath = DisplayName;
                return this;
            }
            if (Children == null)
            {
                return null;
            }
            var relativePath = string.Join("/", parts.Skip(1));
            foreach(var child in Children)
            {
                var queryResult = child.Query(relativePath, out var childPrettyPath);
                if (queryResult != null)
                {
                    prettyPath = DisplayName + "/" + childPrettyPath;
                    return queryResult;
                }
            }
            return null;*/
        }

        private bool QueryPredicate(EntityObject entityObject, string queryPart)
        {
            if (entityObject.Guid == queryPart)
            {
                return true;
            }
            if (entityObject.Id == queryPart)
            {
                return true;
            }
            if (queryPart.StartsWith("."))
            {
                var className = queryPart.Substring(1);
                if (entityObject.Class == className)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
