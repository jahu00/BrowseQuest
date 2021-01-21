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
        public EntityInstance EntityInstance { get; set; }

        public EntityControl(EntityInstance entityInstance)
        {
            InitializeComponent();
            SetEntity(entityInstance);
        }

        public void SetEntity(EntityInstance entityInstance)
        {
            EntityInstance = entityInstance;
            nameLabel.Text = EntityInstance.Name;
            SetImage(EntityInstance.Name);
        }

        private void SetImage(string name)
        {
            if (!DesignMode)
            {
                var image = ImageManager.Instance.GetImage(name);
                entityPictureBox.Image = image;
            }
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
