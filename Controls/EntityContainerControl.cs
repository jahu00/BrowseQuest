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
    public delegate void EntityControlClickedEventHandler(EntityContainerControl sender, EntityControl entityControl);

    public partial class EntityContainerControl : UserControl
    {
        #region Properties
        public string PathPrefix { get; set; }
        private Game Game { get; set; }
        private EntityObject CurrentObject { get; set; }

        public string PathText
        {
            get
            {
                return pathLabel.Text;
            }
            set
            {
                pathLabel.Text = value;
            }
        }
        #endregion

        #region Events
        public event EntityControlClickedEventHandler EntityControlClicked;
        #endregion

        public EntityContainerControl()
        {
            InitializeComponent();
        }

        public void SetGame(Game game)
        {
            Game = game;
        }

        public void OnPathChanged(Game sender, EntityObject entityObject, string path, string prettyPath)
        {
            PathText = PathPrefix + prettyPath;
            SetCurrentObject(entityObject);
        }

        public void SetCurrentObject(EntityObject currentObject)
        {
            if (CurrentObject != null)
            {
                CurrentObject.ChildAdded -= OnChildAdded;
                CurrentObject.ChildRemoved -= OnChildRemoved;
            }
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
            CurrentObject.ChildAdded += OnChildAdded;
            CurrentObject.ChildRemoved += OnChildRemoved;
        }

        private void OnChildAdded(EntityObject sender, EntityObject child)
        {
            AddChildControl(child);
            UpdateSize();
        }

        private void OnChildRemoved(EntityObject sender, EntityObject child)
        {
            RemoveChildControl(child);
            UpdateSize();
        }

        private void UpdateSize()
        {
            sizeLabel.Text = CurrentObject.OccupiedSpace + " / " + CurrentObject.Space;
        }

        private void Populate(EntityObject parentObject)
        {
            entityLayoutPanel.Controls.Clear();
            if (parentObject == null || parentObject.ImmutableChildren == null)
            {
                return;
            }
            foreach (var child in parentObject.ImmutableChildren)
            {
                AddChildControl(child);
            }
        }

        private void AddChildControl(EntityObject child)
        {
            var entityControl = new EntityControl(child);
            entityControl.Click += entityControl_Click;
            entityLayoutPanel.Controls.Add(entityControl);
        }

        private void RemoveChildControl(EntityObject child)
        {
            var entityControl = entityLayoutPanel.Controls.Cast<EntityControl>().Single(x => x.EntityObject == child);
            entityLayoutPanel.Controls.Remove(entityControl);
        }

        private void entityControl_Click(object sender, EventArgs e)
        {
            var entityControl = (EntityControl)sender;
            EntityControlClicked(this, entityControl);
            /*var entityControl = (EntityControl)sender;
            var entityObject = entityControl.EntityObject;
            if (entityObject.Link != null)
            {
                Game.SetPath(entityObject.Link);
            }
            if (entityObject.CanEnter == true)
            {
                Game.SetPath(entityObject.Guid);
            }

            var backpack = Game.Backpack;

            if (entityObject.CanTake == true )
            {
                if (CurrentObject == backpack)
                {
                    Game.MoveEntity(entityObject, backpack, CurrentObject);
                }
                else
                {
                    Game.MoveEntity(entityObject, CurrentObject, backpack);
                }
            }*/
        }
    }
}
