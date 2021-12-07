namespace EdTools
{
    partial class FormMain
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStartRoute = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderSystem = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderJumped = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLeft = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderJumps = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNeutron = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(100, 6);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(225, 23);
            this.textBox1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target system:";
            // 
            // buttonStartRoute
            // 
            this.buttonStartRoute.Location = new System.Drawing.Point(331, 6);
            this.buttonStartRoute.Name = "buttonStartRoute";
            this.buttonStartRoute.Size = new System.Drawing.Size(75, 23);
            this.buttonStartRoute.TabIndex = 2;
            this.buttonStartRoute.Text = "Start";
            this.buttonStartRoute.UseVisualStyleBackColor = true;
            this.buttonStartRoute.Click += new System.EventHandler(this.ButtonStartRoute_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderSystem,
            this.columnHeaderJumped,
            this.columnHeaderLeft,
            this.columnHeaderJumps,
            this.columnHeaderNeutron});
            this.listView1.FullRowSelect = true;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(12, 35);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(639, 348);
            this.listView1.TabIndex = 3;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderSystem
            // 
            this.columnHeaderSystem.Text = "System";
            this.columnHeaderSystem.Width = 100;
            // 
            // columnHeaderJumped
            // 
            this.columnHeaderJumped.Text = "Jumped";
            // 
            // columnHeaderLeft
            // 
            this.columnHeaderLeft.Text = "Left";
            // 
            // columnHeaderJumps
            // 
            this.columnHeaderJumps.Text = "Jumps";
            // 
            // columnHeaderNeutron
            // 
            this.columnHeaderNeutron.Text = "Neutron";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonStartRoute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBox1);
            this.Name = "FormMain";
            this.Text = "Route Plotter";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonStartRoute;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderSystem;
        private System.Windows.Forms.ColumnHeader columnHeaderJumped;
        private System.Windows.Forms.ColumnHeader columnHeaderLeft;
        private System.Windows.Forms.ColumnHeader columnHeaderJumps;
        private System.Windows.Forms.ColumnHeader columnHeaderNeutron;
    }
}
