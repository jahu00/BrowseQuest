using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BrowseQuest.Model;

namespace BrowseQuest.Controls
{
    public partial class EntityContainerControl : UserControl
    {
        public string PathPrefix { get; set; }

        private EntityObject RootObject { get; set; }
        private EntityObject CurrentObject { get; set; }

        public string Path { get; private set; } = "";

        public EntityContainerControl()
        {
            InitializeComponent();
        }

        public void SetRootObject(EntityObject rootObject)
        {
            RootObject = rootObject;
        }

        public void SetPath(string path)
        {
            if (!path.StartsWith("/"))
            {
                path = Path + "/" + path;
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

            Path = path;
            var currentObject = RootObject.Query(path, out var prettyPath);
            SetCurrentObject(currentObject);
            pathLabel.Text = PathPrefix + prettyPath;
        }

        private void SetCurrentObject(EntityObject currentObject)
        {
            CurrentObject = currentObject;
            Populate(CurrentObject);
            if (currentObject.Space.HasValue)
            {
                sizeLabel.Visible = true;
                UpdateSize();
            }
            else
            {
                sizeLabel.Visible = false;
            }
        }

        private void UpdateSize()
        {
            sizeLabel.Text = CurrentObject.OccupiedSpace + " / " + CurrentObject.Space;
        }

        private void Populate(EntityObject parentObject)
        {
            entityLayoutPanel.Controls.Clear();
            if (parentObject == null || parentObject.Children == null)
            {
                return;
            }
            foreach (var child in parentObject.Children)
            {
                var entityControl = new EntityControl(child, Path + "/" + child.Guid);
                entityControl.Click += entityControl_Click;
                entityLayoutPanel.Controls.Add(entityControl);
            }
        }

        private void entityControl_Click(object sender, EventArgs e)
        {
            var entityControl = (EntityControl)sender;
            var entityObject = entityControl.EntityObject;
            if (entityObject.Link != null)
            {
                SetPath(entityObject.Link);
            }
            if (entityObject.CanEnter == true)
            {
                SetPath(entityObject.Guid);
            }
        }
    }
}
