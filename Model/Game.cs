using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    public delegate void PathChangedEventHandler(Game sender, EntityObject entityObject, string path, string prettyPath);

    public class Game
    {
        #region Properties
        public EntityClassManager EntityClassManager { get; set; }

        public EntityObject World { get; private set; }
        public EntityObject Player { get; private set; }

        public EntityObject Backpack { get; private set; }

        public string CurrentPath { get; private set; }

        public string CurrentPrettyPath { get; private set; }

        public EntityObject CurrentObject { get; set; }
        #endregion

        #region Events
        public event PathChangedEventHandler PathChanged;
        #endregion

        public Game(EntityClassManager entityClassManager, EntityObject world, EntityObject player)
        {
            EntityClassManager = entityClassManager;
            World = world;
            Player = player;
            Backpack = player.QuerySingle("Backpack");
        }

        #region Moving entities
        public bool TryPickEntity(EntityObject entityObject)
        {
            if (entityObject.CanTake == true)
            {
                MoveEntity(entityObject, CurrentObject, Backpack);
                return true;
            }
            return false;
        }

        public bool TryDropEntity(EntityObject entityObject)
        {
            if (entityObject.CanTake == true)
            {
                MoveEntity(entityObject, Backpack, CurrentObject);
                return true;
            }
            return false;
        }

        public void MoveEntity(EntityObject entityObject, EntityObject oldParent, EntityObject newParent)
        {
            oldParent.RemoveChild(entityObject);
            newParent.AddChild(entityObject);
        }
        #endregion

        #region Changing current path
        public bool TryGoToEntity(EntityObject entityObject)
        {
            if (entityObject.Link != null)
            {
                SetPath(entityObject.Link);
                return true;
            }
            if (entityObject.CanEnter == true)
            {
                SetPath(entityObject.Guid);
                return true;
            }
            return false;
        }

        public void SetPath(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = CurrentPath + "/" + path;
            }

            var parts = new List<string>();
            foreach (var part in path.Split('/'))
            {
                if (part == "..")
                {
                    if (parts.Count() > 0)
                    {
                        parts.RemoveAt(parts.Count - 1);
                    }
                    continue;
                }
                parts.Add(part);
            }

            path = string.Join("/", parts);

            CurrentPath = path;
            CurrentObject = World.QuerySingle(path, out var prettyPath);
            CurrentPrettyPath = prettyPath;
            PathChanged?.Invoke(this, CurrentObject, CurrentPath, prettyPath);
        }
        #endregion

        #region Spawning entities
        public void Spawn(string className)
        {
            var entityObject = EntityClassManager.InstanceClass(className);
            CurrentObject.AddChild(entityObject);
        }
        #endregion
    }
}
