namespace ARKcc
{
    partial class EntityEntry
    {
        /// <summary> 
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Komponenten-Designer generierter Code

        /// <summary> 
        /// Erforderliche Methode für die Designerunterstützung. 
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.labelName = new System.Windows.Forms.Label();
            this.numericUpDownQuantity = new System.Windows.Forms.NumericUpDown();
            this.numericUpDownQuality = new System.Windows.Forms.NumericUpDown();
            this.checkBoxBP = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuantity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // labelName
            // 
            this.labelName.Location = new System.Drawing.Point(3, 2);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(206, 15);
            this.labelName.TabIndex = 0;
            this.labelName.Text = "labelEntity";
            this.labelName.Click += new System.EventHandler(this.labelName_Click);
            // 
            // numericUpDownQuantity
            // 
            this.numericUpDownQuantity.Location = new System.Drawing.Point(215, 0);
            this.numericUpDownQuantity.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDownQuantity.Name = "numericUpDownQuantity";
            this.numericUpDownQuantity.Size = new System.Drawing.Size(38, 20);
            this.numericUpDownQuantity.TabIndex = 1;
            this.numericUpDownQuantity.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // numericUpDownQuality
            // 
            this.numericUpDownQuality.Location = new System.Drawing.Point(259, 0);
            this.numericUpDownQuality.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericUpDownQuality.Name = "numericUpDownQuality";
            this.numericUpDownQuality.Size = new System.Drawing.Size(33, 20);
            this.numericUpDownQuality.TabIndex = 2;
            // 
            // checkBoxBP
            // 
            this.checkBoxBP.AutoSize = true;
            this.checkBoxBP.Location = new System.Drawing.Point(298, 1);
            this.checkBoxBP.Name = "checkBoxBP";
            this.checkBoxBP.Size = new System.Drawing.Size(40, 17);
            this.checkBoxBP.TabIndex = 3;
            this.checkBoxBP.Text = "BP";
            this.checkBoxBP.UseVisualStyleBackColor = true;
            // 
            // EntityEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.checkBoxBP);
            this.Controls.Add(this.numericUpDownQuality);
            this.Controls.Add(this.numericUpDownQuantity);
            this.Controls.Add(this.labelName);
            this.Name = "EntityEntry";
            this.Size = new System.Drawing.Size(341, 20);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuantity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.NumericUpDown numericUpDownQuantity;
        private System.Windows.Forms.NumericUpDown numericUpDownQuality;
        private System.Windows.Forms.CheckBox checkBoxBP;
    }
}
