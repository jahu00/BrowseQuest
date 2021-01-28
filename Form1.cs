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
        private Game Game { get; set; }

        public Form1()
        {
            InitializeComponent();
            var classJson = File.ReadAllText("Content/Data/classes.json");
            var worldJson = File.ReadAllText("Content/Data/world.json");
            var playerJson = File.ReadAllText("Content/Data/player.json");
            var classManager = new EntityClassManager(classJson);
            var world = new EntityObject(worldJson, classManager);
            var player = new EntityObject(playerJson, classManager);

            var game = new Game(classManager, world, player);
            Game = game;

            worldEntityContainer.SetGame(game);
            worldEntityContainer.EntityControlClicked += OnWorldEntityClicked;
            game.PathChanged += worldEntityContainer.OnPathChanged;
            game.SetPath("Farm");

            backpackEntityContainer.SetGame(game);
            backpackEntityContainer.EntityControlClicked += OnBackpackEntityClicked;
            backpackEntityContainer.SetCurrentObject(game.Backpack);
            backpackEntityContainer.PathText = "B:/Backpack";
            
        }

        private void OnWorldEntityClicked(EntityContainerControl sender, EntityControl entityControl)
        {
            if (Game.TryGoToEntity(entityControl.EntityObject))
            {
                return;
            }
            Game.TryPickEntity(entityControl.EntityObject);
        }

        private void OnBackpackEntityClicked(EntityContainerControl sender, EntityControl entityControl)
        {
            Game.TryDropEntity(entityControl.EntityObject);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Game.Spawn("Bat");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Game.Spawn("Rat");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var lastChild = Game.CurrentObject.ImmutableChildren.LastOrDefault();
            if (lastChild != null)
            {
                Game.CurrentObject.RemoveChild(lastChild);
            }

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
