using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrowseQuest.Model
{
    public class EntityInstance
    {
        public string Name { get; set; }

        public string Id { get; set; }

        public Guid guid { get; set; }

        public Guid Guid
        {
            get
            {
                if (guid == null)
                {
                    guid = Guid.NewGuid();
                }
                return guid;
            }
            set
            {
                guid = value;
            }
        }

        public dynamic Entity { get; set; }

        private Dictionary<string, dynamic> variables { get; set; }



        public List<EntityInstance> Children { get; set; }

        public EntityInstance()
        {
        }
    }
}
