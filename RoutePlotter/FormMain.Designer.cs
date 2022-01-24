namespace RoutePlotter
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
            this.textBoxTargetSystem = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonStartRoute = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderSystem = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderJumped = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderLeft = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderJumps = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderNeutron = new System.Windows.Forms.ColumnHeader();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxCurrentSystem = new System.Windows.Forms.TextBox();
            this.numericUpDownJumpRange = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJumpRange)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxTargetSystem
            // 
            this.textBoxTargetSystem.Location = new System.Drawing.Point(101, 70);
            this.textBoxTargetSystem.Name = "textBoxTargetSystem";
            this.textBoxTargetSystem.Size = new System.Drawing.Size(225, 23);
            this.textBoxTargetSystem.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "Target system:";
            // 
            // buttonStartRoute
            // 
            this.buttonStartRoute.Location = new System.Drawing.Point(332, 70);
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
            this.listView1.Location = new System.Drawing.Point(13, 99);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(394, 319);
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
            this.columnHeaderJumps.Width = 80;
            // 
            // columnHeaderNeutron
            // 
            this.columnHeaderNeutron.Text = "Neutron";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 15);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(66, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Current sys";
            // 
            // textBoxCurrentSystem
            // 
            this.textBoxCurrentSystem.Location = new System.Drawing.Point(101, 12);
            this.textBoxCurrentSystem.Name = "textBoxCurrentSystem";
            this.textBoxCurrentSystem.ReadOnly = true;
            this.textBoxCurrentSystem.Size = new System.Drawing.Size(225, 23);
            this.textBoxCurrentSystem.TabIndex = 5;
            // 
            // numericUpDownJumpRange
            // 
            this.numericUpDownJumpRange.DecimalPlaces = 2;
            this.numericUpDownJumpRange.Location = new System.Drawing.Point(101, 41);
            this.numericUpDownJumpRange.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
            this.numericUpDownJumpRange.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.numericUpDownJumpRange.Name = "numericUpDownJumpRange";
            this.numericUpDownJumpRange.Size = new System.Drawing.Size(225, 23);
            this.numericUpDownJumpRange.TabIndex = 8;
            this.numericUpDownJumpRange.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 43);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "Current range";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(332, 14);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(61, 19);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "OnTop";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(418, 427);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.numericUpDownJumpRange);
            this.Controls.Add(this.textBoxCurrentSystem);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.buttonStartRoute);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxTargetSystem);
            this.Name = "FormMain";
            this.Text = "Route Plotter";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownJumpRange)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxTargetSystem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonStartRoute;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderSystem;
        private System.Windows.Forms.ColumnHeader columnHeaderJumped;
        private System.Windows.Forms.ColumnHeader columnHeaderLeft;
        private System.Windows.Forms.ColumnHeader columnHeaderJumps;
        private System.Windows.Forms.ColumnHeader columnHeaderNeutron;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxCurrentSystem;
        private System.Windows.Forms.NumericUpDown numericUpDownJumpRange;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox checkBox1;
    }
}
