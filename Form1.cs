using BrowseQuest.Controls;
using BrowseQuest.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BrowseQuest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            var jsonStr = File.ReadAllText("Content/Data/world.json");
            var world = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EntityInstance>>(jsonStr);
            populate(world);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var entity = new EntityControl("bat");
            //entityLayoutPanel.Controls.Add(entity);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //var entity = new EntityControl("rat");
            //entityLayoutPanel.Controls.Add(entity);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (entityLayoutPanel.Controls.Count == 0)
            {
                return;
            }
            var entity = entityLayoutPanel.Controls.Cast<Control>().Last();
            entity.Dispose();
        }

        public void populate(List<EntityInstance> entities)
        {
            entityLayoutPanel.Controls.Clear();
            foreach (var entityInstance in entities)
            {
                var entityControl = new EntityControl(entityInstance);
                entityControl.Click += entityControl_Click;
                entityLayoutPanel.Controls.Add(entityControl);
            }
        }

        private void entityControl_Click(object sender, EventArgs e)
        {
            var entityControl = (EntityControl)sender;
            if (entityControl.EntityInstance.Children?.Count > 0)
            {
                populate(entityControl.EntityInstance.Children);
            }
        }
    }
}
