using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    #region Delegates
    //public delegate void ParentChanged(EntityObject oldParent, EntityObject newParent);
    public delegate void ChildChangedEventHandler(EntityObject sender, EntityObject child);
    #endregion

    /// <summary>
    /// <para>Holds data for an entity in the game world. Entity parameters are returned from base predefined class
    /// or from an instance of the entity (EntityObject), if they are overridden.</para>
    /// <para>In runtime, data is stored using dynamic JObject, with class data being shared among all instances.</para>
    /// </summary>
    public class EntityObject
    {
        #region Data
        /// <summary>
        /// Stores a reference to class data
        /// </summary>
        private JObject ClassData { get; set; }

        /// <summary>
        /// Stores individual instance data
        /// </summary>
        private JObject ObjectData { get; set; }
        #endregion

        #region Properties
        /// <summary>
        /// Unique, bu not user friendly Id of an entity instance
        /// </summary>
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

        /// <summary>
        /// User friendly Id of an instance
        /// </summary>
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

        /// <summary>
        /// Name to be displayed to the player (later to be used as a name of a resource string)
        /// </summary>
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

        /// <summary>
        /// Name of a class that was inherited
        /// </summary>
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

        /// <summary>
        /// Name of an image file
        /// </summary>
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

        /// <summary>
        /// If an entity is a shortcut/hyperlink, this property will have the target path
        /// </summary>
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

        /// <summary>
        /// If an entity is supposed to be explored (like a directory), this property should be set to true
        /// </summary>
        public bool? CanEnter
        {
            get
            {
                return (bool?)GetProperty("CanEnter") ?? false;
            }
            set
            {
                ObjectData["CanEnter"] = value;
            }
        }

        public bool? CanTake
        {
            get
            {
                return (bool?)GetProperty("CanTake") ?? false;
            }
            set
            {
                ObjectData["CanTake"] = value;
            }
        }

        /// <summary>
        /// If a property should be hidden, this property should be set to false
        /// </summary>
        public bool? Visible
        {
            get
            {
                return (bool?)GetProperty("Visible") ?? true;
            }
            set
            {
                ObjectData["Visible"] = value;
            }
        }

        /// <summary>
        /// Space this entity takes in a container
        /// </summary>
        public Int64? Size
        {
            get
            {
                return (Int64?)GetProperty("Size") ?? 0;
            }
            set
            {
                ObjectData["Size"] = value;
            }
        }

        /// <summary>
        /// How much space this container has (if left empty, the space is infinite)
        /// </summary>
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

        public EntityObject[] ImmutableChildren
        {
            get
            {
                return Children?.ToArray();
            }
        }

        private List<EntityObject> Children { get; set; }

        #endregion

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

        public Int64 OccupiedSpace
        {
            get
            {
                return Children?.Sum(child => child.Size.Value) ?? 0;
            }
        }

        public Int64 FreeSpace
        {
            get
            {
                if (!Space.HasValue)
                {
                    return Int64.MaxValue;
                }
                return Space.Value - OccupiedSpace;
            }
        }
        #endregion

        #region Events
        public event ChildChangedEventHandler ChildAdded;
        public event ChildChangedEventHandler ChildRemoved;
        #endregion

        /// <summary>
        /// Returns property value from either base class or this instance
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        public EntityObject(EntityClassManager classManager, string className) : this (new { Class = className }, classManager)
        {
        }

        public EntityObject(object objectData, EntityClassManager classManager) : this(JObject.FromObject(objectData), classManager)
        {
        }

        public EntityObject(JObject objectData, EntityClassManager classManager)
        {
            ObjectData = (JObject)objectData.DeepClone();
            JObject classData;
            if (Class != null && classManager.EntityClassDictionary.TryGetValue(Class, out classData))
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

        /// <summary>
        /// Converts this EntityObject to JObject for further serialization
        /// </summary>
        /// <returns></returns>
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

        public EntityObject QuerySingle(string path)
        {
            return QuerySingle(path, out var prettyPath);
        }

        /// <summary>
        /// <para>Returns the first matching child or one of the grandchildren.</para>
        /// <para/>
        /// <para>Query format is as follows:</para>
        /// <para>/ChildId/GrandChildId/Etc</para>
        /// <para>Each part of the query can be a Guid or Id.</para>
        /// <para>Query will always be treated as relative, even if it starts with a slash.</para>
        /// </summary>
        /// <param name="path">Path to be queried</param>
        /// <param name="prettyPath">User friendly path to be returned</param>
        /// <returns></returns>
        public EntityObject QuerySingle(string path, out string prettyPath)
        {
            EntityObject objectToSearchIn = this;
            EntityObject result = null;

            var prettyPathParts = new List<string>();

            // remove starting slash if found
            if (path.StartsWith("/"))
            {
                path = path.Substring(1);
            }

            // Split the path into parts
            var parts = path.Split('/');

            // walk through the parts in search of matching entities
            do
            {
                result = objectToSearchIn.Children?.FirstOrDefault(child => QueryPredicate(child, parts[0]));

                // if no entity was found for current query part, we can stop the search
                if (result == null)
                {
                    prettyPath = null;
                    return null;
                }

                // add user friendly name to the prettyPath
                prettyPathParts.Add(result.DisplayName);

                // remove the found part from parts collection
                parts = parts.Skip(1).ToArray();

                // set found entity as the object to ber searched
                objectToSearchIn = result;

            } while (parts.Count() > 0); // stop the loop if we are out of parts

            // build the userFriendly path
            prettyPath = string.Join("/", prettyPathParts);

            return result;
        }

        /// <summary>
        /// Defines how entities are filtered using a quryPart
        /// </summary>
        /// <param name="entityObject"></param>
        /// <param name="queryPart"></param>
        /// <returns></returns>
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

        public void AddChild(EntityObject child)
        {
            Children.Add(child);
            ChildAdded?.Invoke(this, child);
        }

        public void RemoveChild(EntityObject child)
        {
            Children.Remove(child);
            ChildRemoved?.Invoke(this, child);
        }
    }
}
