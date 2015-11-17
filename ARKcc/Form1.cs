using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ARKcc
{
    public partial class Form1 : Form
    {
        private List<int> creatureList = new List<int>();
        private List<int> itemList = new List<int>();
        private List<int> commandList = new List<int>();
        private List<int> recentList = new List<int>();
        private List<int> recentKindList = new List<int>();
        private List<Entity> entities = new List<Entity>();
        private int maxRecently = 100;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // read entities from file
            string path = "entities.txt";

            // check if file exists
            if (!File.Exists(path))
            {
                MessageBox.Show("Entities-File 'entities.txt' not found.", "Error");
                this.Close();
            }
            else
            {
                string[] rows;
                rows = File.ReadAllLines(path);
                string[] parameters;
                List<string> categories = new List<string>();
                // default category
                categories.Add("Items");
                int i = 0;
                foreach (string row in rows)
                {
                    if (row.Length > 1 && row.Substring(0, 2) != "//")
                    {
                        // if new category
                        if (row.Substring(0, 1) == "-")
                        {
                            int level = 1;
                            while (row.Length > level && row.Substring(level, 1) == "-") { level++; }
                            int currentLevel = categories.Count();
                            if (currentLevel >= level)
                            {
                                categories.RemoveRange(level - 1, 1 + currentLevel - level);
                            }
                            categories.Add(row.Substring(level).Trim());

                            // check if category is already in treeview, else add
                            TreeNode node;
                            TreeNodeCollection nodes;
                            switch (categories[0].ToLower())
                            {
                                case "commands":
                                    nodes = this.treeViewCommands.Nodes;
                                    break;
                                case "creatures":
                                    nodes = this.treeViewCreatures.Nodes;
                                    break;
                                case "items":
                                    nodes = this.treeViewItems.Nodes;
                                    break;
                                default:
                                    nodes = null;
                                    break;
                            }
                            bool nodeExists;
                            if (nodes != null)
                            {
                                for (int n = 0; n < categories.Count; n++)
                                {
                                    node = null;
                                    nodeExists = false;
                                    if (nodes.Count > 0)
                                    {
                                        node = nodes[0];
                                    }
                                    while (node != null)
                                    {
                                        if (node.Text == categories[n])
                                        {
                                            if (n == categories.Count - 1)
                                            {
                                                nodeExists = true;
                                                break;
                                            }
                                            nodes = node.Nodes;
                                            nodeExists = true;
                                            break;
                                        }
                                        node = node.NextNode;
                                    }
                                    if (!nodeExists)
                                    {
                                        // add new node alphabetically
                                        int nn = 0;
                                        while (nn < nodes.Count && String.Compare(nodes[nn].Text, categories[n], true) < 0) { nn++; }
                                        nodes.Insert(nn, categories[n]);
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            parameters = row.Split(',');
                            if (parameters.Count() > 1)
                            {
                                int n = 1;
                                this.entities.Add(new Entity() { name = parameters[0].Trim() + (parameters[1].Trim().Length + (parameters.Count() > 2 ? parameters[2].Trim().Length : 0) == 0 ? " (NO ID OR BP GIVEN!)" : ""), id = parameters[1].Trim(), bp = (parameters.Count() > 2 ? parameters[2].Trim() : ""), category = string.Join("\\", categories), maxstack = (parameters.Count() > 3 ? (int.TryParse(parameters[3].Trim(), out n) ? n : 1) : 1) });
                                i++;
                            }
                        }
                    }
                }
                // sort entities
                entities.Sort(delegate (Entity x, Entity y)
                {
                    if (x.name == null && y.name == null) return 0;
                    else if (x.name == null) return -1;
                    else if (y.name == null) return 1;
                    else return x.name.CompareTo(y.name);
                });

                this.treeViewCommands.ExpandAll();
                this.treeViewCreatures.ExpandAll();
                this.treeViewItems.ExpandAll();
                filterList(1, "Commands");
                filterList(2, "Creatures");
                filterList(3, "Items");
            }
        }

        private void listBoxRecent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxRecent.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(0, index);
            }
        }

        private void listBoxCommands_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxCommands.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(1, index);
            }
        }

        private void listBoxCreatures_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxCreatures.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(2, index);
            }
        }
        private void listBoxItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxItems.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(3, index);
            }
        }

        private void buttonClearCommand_Click(object sender, EventArgs e)
        {
            textBoxCommand.Clear();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (textBoxCommand.Text.Length > 0)
            {
                Clipboard.SetText(textBoxCommand.Text);
            }
        }

        private string createCommand(int list, int index)
        {
            switch (list)
            {
                case 1:
                    return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + this.entities[index].id;
                    break;
                case 2:
                    if (this.checkBoxExact.Checked)
                    {
                        return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + "SpawnDino " + this.entities[index].bp + " " + this.numericUpDownDistance.Value.ToString() + " " + this.numericUpDownY.Value.ToString() + " " + this.numericUpDownZ.Value.ToString() + " " + this.numericUpDownLevel.Value.ToString();
                    }
                    else
                    {
                        return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + "summon " + this.entities[index].id;
                    }
                    break;
                case 3:
                    if (this.entities[index].id.Length > 0 || this.entities[index].bp.Length > 0)
                    {
                        string quantity = (this.numericUpDownQuantity.Value > this.entities[index].maxstack ? this.entities[index].maxstack : this.numericUpDownQuantity.Value).ToString();
                        return (this.entities[index].id.Length > 0 ? "GiveItemNum " + this.entities[index].id : "GiveItem " + this.entities[index].bp) + " " + quantity + " " + this.numericUpDownQuality.Value.ToString() + (this.checkBoxBP.Checked ? " 1" : " 0");
                    }
                    break;
            }
            return "";
        }

        private void addCommand(int list, int index)
        {
            //list: 0: recent, 1: commands, 2: creatures, 3: items
            int eIndex = -1;
            switch (list)
            {
                case 0:
                    eIndex = recentList[index];
                    list = recentKindList[index];
                    break;
                case 1:
                    eIndex = commandList[index];
                    break;
                case 2:
                    eIndex = creatureList[index];
                    break;
                case 3:
                    eIndex = itemList[index];
                    break;
                default:
                    break;
            }
            if (eIndex >= 0)
            {
                string command = createCommand(list, eIndex);
                if (command.Length > 0)
                {
                    string name = this.entities[eIndex].name;
                    this.textBoxCommand.Text += (this.textBoxCommand.Text.Length > 0 ? "|" : "") + command;
                    int pos = this.listBoxRecent.Items.IndexOf(name);
                    if (pos == -1)
                    {
                        this.recentList.Insert(0, eIndex);
                        this.recentKindList.Insert(0, list);
                        this.listBoxRecent.Items.Insert(0, name);
                        updateRecentlyList();
                    }
                    else if (pos > 0)
                    {
                        this.recentList.RemoveAt(pos);
                        this.recentKindList.RemoveAt(pos);
                        this.listBoxRecent.Items.RemoveAt(pos);
                        this.recentList.Insert(0, eIndex);
                        this.recentKindList.Insert(0, list);
                        this.listBoxRecent.Items.Insert(0, name);
                        updateRecentlyList();
                    }
                }
            }
        }

        private void updateRecentlyList()
        {
            if (this.recentList.Count() > maxRecently)
            {
                this.recentList.RemoveRange(maxRecently, this.recentList.Count() - maxRecently);
            }
            if (this.recentKindList.Count() > maxRecently)
            {
                this.recentKindList.RemoveRange(maxRecently, this.recentKindList.Count() - maxRecently);
            }
            while (this.listBoxRecent.Items.Count > maxRecently)
            {
                this.listBoxRecent.Items.RemoveAt(this.listBoxRecent.Items.Count - 1);
            }

        }

        private void checkBoxExact_CheckedChanged(object sender, EventArgs e)
        {
            this.groupBoxExact.Enabled = this.checkBoxExact.Checked;
        }

        private void treeViewCommands_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(1, this.treeViewCommands.SelectedNode.FullPath);
        }
        private void treeViewCreatures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(2, this.treeViewCreatures.SelectedNode.FullPath);
        }

        private void treeViewItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(3, this.treeViewItems.SelectedNode.FullPath);
        }

        private void filterList(int list, string cats)
        {
            ListBox pListBox = null;
            List<int> pList = null;
            switch (list)
            {
                case 1:
                    pListBox = this.listBoxCommands;
                    pList = commandList;
                    break;
                case 2:
                    pListBox = this.listBoxCreatures;
                    pList = creatureList;
                    break;
                case 3:
                    pListBox = this.listBoxItems;
                    pList = itemList;
                    break;
            }
            if (pListBox != null)
            {
                pListBox.Items.Clear();
                pList.Clear();
                int i = 0;
                foreach (Entity entity in entities)
                {
                    if (entity.category.Length >= cats.Length && cats == entity.category.Substring(0, cats.Length))
                    {
                        pListBox.Items.Add(entity.name);
                        pList.Add(i);
                    }
                    i++;
                }
            }
        }

        private void listBox_MouseUp(object sender, MouseEventArgs e)
        {
            ListBox lb = sender as ListBox;
            if (lb != null)
            {
                if (e.Button == System.Windows.Forms.MouseButtons.Right)
                {
                    int index = lb.IndexFromPoint(e.Location);
                    if (index != System.Windows.Forms.ListBox.NoMatches)
                    {
                        System.Diagnostics.Process.Start("http://ark.gamepedia.com/" + lb.Items[index].ToString().Replace(' ', '_'));
                    }
                }
            }
        }
    }
}
