using BrowseQuest.Controls;
using BrowseQuest.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
            var classJson = File.ReadAllText("Content/Data/classes.json");
            var worldJson = File.ReadAllText("Content/Data/world.json");
            var classManager = new EntityClassManager(classJson);
            var world = new EntityObject(worldJson, classManager); //Newtonsoft.Json.JsonConvert.DeserializeObject<List<EntityInstance>>(jsonStr);
            //populate(world.Children);
            worldEntityContainer.SetRootObject(world);
            backpackEntityContainer.SetRootObject(world);
            worldEntityContainer.SetPath("Farm");
            backpackEntityContainer.SetPath("Backpack");
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
            /*if (entityLayoutPanel.Controls.Count == 0)
            {
                return;
            }
            var entity = entityLayoutPanel.Controls.Cast<Control>().Last();
            entity.Dispose();*/
        }

        private void entityControl_Click(object sender, EventArgs e)
        {
            /*var entityControl = (EntityControl)sender;
            if (entityControl.EntityObject.Children?.Count > 0)
            {
                populate(entityControl.EntityObject.Children);
            }*/
        }
    }
}
