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
    public partial class EntityControl : UserControl
    {
        public EntityObject EntityObject { get; private set; }

        public string ImageName { get; set; }

        public string Path { get; set; }

        public EntityControl(EntityObject entityObject, string path)
        {
            InitializeComponent();
            SetEntity(entityObject);
            Path = path;
        }

        public void SetEntity(EntityObject entityInstance)
        {
            EntityObject = entityInstance;
            UpdateEntity();   
        }

        public void UpdateEntity()
        {
            if (nameLabel.Text != EntityObject.DisplayName)
            {
                nameLabel.Text = EntityObject.DisplayName;
            }
            SetImage(EntityObject.ImageName);
            if (EntityObject.Visible.HasValue && EntityObject.Visible != Visible)
            {
                Visible = EntityObject.Visible.Value;
            }
        }

        private void SetImage(string name)
        {
            if (ImageName == name)
            {
                return;
            }
            if (DesignMode)
            {
                return;
            }
            ImageName = name;
            var image = ImageManager.Instance.GetImage(name);
            entityPictureBox.Image = image;
        }

        private void entityPictureBox_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
        }

        private void nameLabel_Click(object sender, EventArgs e)
        {
            InvokeOnClick(this, e);
        }
    }
}
