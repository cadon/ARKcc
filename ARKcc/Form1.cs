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
        private int currentList = -1;
        private string currentCats = "";
        private int localFileVer = 0; // the version of the used entities.txt (is used for update-autocheck)

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(buttonCheckUpdate, "This only checks if a new version of the file entities.txt is available, it's not a check for a new version of the application.");
            toolTip1.SetToolTip(buttonCrap, "Primitive");
            toolTip1.SetToolTip(buttonAs, "Ascendent");
            toolTip1.SetToolTip(checkBoxForceTame, "adds the ForceTame-command after the spawn-command, so it's immediately tamed.");
            loadFile();
        }

        private void clearAll()
        {
            entities.Clear();
            treeViewCommands.Nodes.Clear();
            treeViewCreatures.Nodes.Clear();
            treeViewItems.Nodes.Clear();
        }

        private bool loadFile()
        {
            // read entities from file
            string path = "entities.txt";

            // check if file exists
            if (!File.Exists(path))
            {
                MessageBox.Show("Entities-File 'entities.txt' not found.", "Error");
                this.Close();
                return false;
            }
            else
            {
                clearAll();
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
                        if (row.Substring(0, 1) == "!")
                        {
                            if (!Int32.TryParse(row.Substring(1), out localFileVer))
                            {
                                localFileVer = 0; // file-version unknown
                            }
                        }
                        else
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
                filterList(2, "Commands");
                filterList(1, "Creatures");
                filterList(0, "Items");
                return true;
            }
        }

        private void listBoxRecent_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxRecent.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(3, index);
            }
        }

        private void listBoxCommands_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxCommands.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(2, index);
            }
        }

        private void listBoxCreatures_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxCreatures.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(1, index);
            }
        }
        private void listBoxItems_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = this.listBoxItems.IndexFromPoint(e.Location);
            if (index != System.Windows.Forms.ListBox.NoMatches)
            {
                this.addCommand(0, index);
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
                if (this.checkBoxClearCopy.Checked)
                {
                    textBoxCommand.Clear();
                }
            }
        }

        private string createCommand(int list, int index)
        {
            switch (list)
            {
                case 2:
                    return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + this.entities[index].id;
                    break;
                case 1:
                    if (this.checkBoxExact.Checked)
                    {
                        return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + "SpawnDino " + this.entities[index].bp + " " + this.numericUpDownDistance.Value.ToString() + " " + this.numericUpDownY.Value.ToString() + " " + this.numericUpDownZ.Value.ToString() + " " + this.numericUpDownLevel.Value.ToString() + (checkBoxForceTame.Checked ? "|forcetame" : "");
                    }
                    else
                    {
                        return (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + "summon " + this.entities[index].id + (checkBoxForceTame.Checked ? "|forcetame" : "");
                    }
                    break;
                case 0:
                    if (this.entities[index].id.Length > 0 || this.entities[index].bp.Length > 0)
                    {
                        string commandstring = "";
                        string commandToPlayer = "";
                        if (checkBoxToPlayer.Checked && textBoxToPlayer.Text.Length > 0)
                        {
                            commandToPlayer = "ToPlayer " + textBoxToPlayer.Text;
                        }
                        int quantityTotal = (int)numericUpDownQuantity.Value;
                        while (quantityTotal > 0)
                        {
                            int quantity = (quantityTotal > this.entities[index].maxstack ? this.entities[index].maxstack : quantityTotal);
                            quantityTotal -= quantity;
                            commandstring += "|" + (this.checkBoxAdmincheat.Checked ? "Admincheat " : "") + (this.entities[index].id.Length > 0 ? "GiveItemNum" + commandToPlayer + " " + this.entities[index].id : "GiveItem" + commandToPlayer + " " + this.entities[index].bp) + " " + quantity.ToString() + " " + this.numericUpDownQuality.Value.ToString() + (this.checkBoxBP.Checked ? " 1" : " 0");
                        }
                        return commandstring.Substring(1);
                    }
                    break;
            }
            return "";
        }

        private void addCommand(int list, int index)
        {
            //list: 3: recent, 2: commands, 1: creatures, 0: items
            int eIndex = -1;
            switch (list)
            {
                case 3:
                    eIndex = recentList[index];
                    list = recentKindList[index];
                    break;
                case 2:
                    eIndex = commandList[index];
                    break;
                case 1:
                    eIndex = creatureList[index];
                    break;
                case 0:
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
            this.groupBoxLevel.Enabled = this.checkBoxExact.Checked;
            this.groupBoxPosition.Enabled = this.checkBoxExact.Checked;
        }

        private void treeViewCommands_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(2, this.treeViewCommands.SelectedNode.FullPath);
        }
        private void treeViewCreatures_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(1, this.treeViewCreatures.SelectedNode.FullPath);
        }

        private void treeViewItems_AfterSelect(object sender, TreeViewEventArgs e)
        {
            filterList(0, this.treeViewItems.SelectedNode.FullPath);
        }

        private void filterList(int list, string cats)
        {
            ListBox pListBox = null;
            List<int> pList = null;
            currentList = list;
            currentCats = cats;
            switch (list)
            {
                case 2:
                    pListBox = this.listBoxCommands;
                    pList = commandList;
                    break;
                case 1:
                    pListBox = this.listBoxCreatures;
                    pList = creatureList;
                    break;
                case 0:
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
                    if ((this.textBoxSearch.Text.Length == 0 || entity.name.IndexOf(this.textBoxSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0) && entity.category.Length >= cats.Length && cats == entity.category.Substring(0, cats.Length))
                    {
                        pListBox.Items.Add(entity.name);
                        pList.Add(i);
                    }
                    i++;
                }
            }
        }

        private void textBoxSearch_TextChanged(object sender, EventArgs e)
        {
            filterList(currentList, currentCats);
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

        private void buttonClearSearch_Click(object sender, EventArgs e)
        {
            this.textBoxSearch.Text = "";
            this.textBoxSearch.Focus();
        }

        private void tabControlEntities_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tc = (TabControl)sender;
            if (tc != null)
            {
                currentList = tc.SelectedIndex;
                switch (currentList)
                {
                    case 0:
                        if (this.treeViewItems.SelectedNode == null)
                        {
                            currentCats = "Items";
                        }
                        else
                        {
                            currentCats = this.treeViewItems.SelectedNode.FullPath;
                        }
                        break;
                    case 1:
                        if (this.treeViewCreatures.SelectedNode == null)
                        {
                            currentCats = "Creatures";
                        }
                        else
                        {
                            currentCats = this.treeViewCreatures.SelectedNode.FullPath;
                        }
                        break;
                    case 2:
                        if (this.treeViewCommands.SelectedNode == null)
                        {
                            currentCats = "Commands";
                        }
                        else
                        {
                            currentCats = this.treeViewCommands.SelectedNode.FullPath;
                        }
                        break;
                }
                textBoxSearch.SelectAll();
                textBoxSearch.Select();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuantity.Value = 1;
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuantity.Value = 10;
        }

        private void button100_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuantity.Value = 100;
        }

        private void button1000_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuantity.Value = 1000;
        }

        private void buttonCrap_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuality.Value = 0;
        }

        private void buttonAs_Click(object sender, EventArgs e)
        {
            this.numericUpDownQuality.Value = 20;
        }

        private void linkLabelVer_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/cadon/ARKcc/");
        }

        private void buttonCheckUpdate_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you want to check for a new version of the entities.txt-file?\nYour current file will be backuped.", "Update entities-file?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string remoteUri = "https://raw.githubusercontent.com/cadon/ARKcc/master/ARKcc/";
                    // Create a new WebClient instance.
                    System.Net.WebClient myWebClient = new System.Net.WebClient();
                    string remoteVerS = myWebClient.DownloadString(remoteUri + "ver.txt");
                    int remoteFileVer = 0;
                    if (Int32.TryParse(remoteVerS, out remoteFileVer) && localFileVer < remoteFileVer)
                    {
                        string fileName = "entities.txt";
                        // backup the current version (to safe user added custom commands)
                        File.Copy(fileName, "entities_backup_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt");
                        // Download the Web resource and save it into the current filesystem folder.
                        myWebClient.DownloadFile(remoteUri + fileName, fileName);
                        // load new settings
                        if (loadFile())
                        {
                            MessageBox.Show("Download and update of entries successful", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    else
                    {
                        MessageBox.Show("You already have the newest version of the entities.txt.\n\nIf you want to add custom commands, you can easily modify the file yourself.", "No new Version", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while trying to check or download:\n\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void checkBoxToPlayer_CheckedChanged(object sender, EventArgs e)
        {
            textBoxToPlayer.Enabled = checkBoxToPlayer.Checked;
        }

        private void buttonLvl120_Click(object sender, EventArgs e)
        {
            numericUpDownLevel.Value = 120;
        }
    }
}

// TODO
//admincheat SpawnActorSpread "Blueprint'/Game/PrimalEarth/Dinos/Carno/MegaCarno_Character_BP.MegaCarno_Character_BP'" X Y Z Nr Spread