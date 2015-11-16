using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARKcc
{
    public partial class EntityEntry : UserControl
    {
        public event EventHandler useCommand;
        private string id = "";
        private string bpPath = "";
        private string category = "";

        public EntityEntry(string[] p)
        {
            InitializeComponent();
            setId(p[0]);
            setName(p[1]);
            if (p.Count() > 2)
            {
                setCategory(p[2]);
                if (p.Count() > 3)
                {
                    int s = 1;
                    if (Int32.TryParse(p[3], out s))
                    {
                        setStacksize(s);
                        if (p.Count() > 4)
                        {
                            setBpPath(p[4]);
                        }
                    }
                }
            }
        }

        public string getCommand()
        {
            return (this.id.Length > 0 ? "GiveItemNum " + this.id : "GiveItem " + this.bpPath) + " " + this.numericUpDownQuantity.Value.ToString() + " " + this.numericUpDownQuality.Value.ToString() + (this.checkBoxBP.Checked ? " 1" : " 0");
        }
        public string getCommandName()
        {
            return this.labelName.Text + " × " + this.numericUpDownQuantity.Value.ToString() + (this.numericUpDownQuality.Value>1?", Qualität " + this.numericUpDownQuality.Value.ToString():"") + (this.checkBoxBP.Checked ? " (BP)" : "");
        }
        public string getEntityName()
        {
            return this.labelName.Text;
        }
        private void setId(string id)
        {
            int x = 0;
            if (Int32.TryParse(id, out x))
            {
                this.id = id;
            }
        }
        private void setName(string name)
        {
            this.labelName.Text = name;
        }
        private void setCategory(string category)
        {
            this.category = category;
        }
        private void setStacksize(int stacksize)
        {
            if (stacksize > 0)
            {
                this.numericUpDownQuantity.Maximum = stacksize;
            }
        }
        private void setBpPath(string bpPath)
        {
            this.bpPath = bpPath;
        }
        protected virtual void OnUseCommand(EventArgs e)
        {
            if (useCommand != null)
                useCommand(this, e);
        }
        private void labelName_Click(object sender, EventArgs e)
        {
            OnUseCommand(new EventArgs());
        }
    }
}