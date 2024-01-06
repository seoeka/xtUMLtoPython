using System.Drawing;

namespace pppl_uml_python
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textGeneratePython = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.btnParse = new System.Windows.Forms.Button();
            this.btnVisualize = new System.Windows.Forms.Button();
            this.btnSimulate = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bt_copyPy = new System.Windows.Forms.Button();
            this.bt_copyJSON = new System.Windows.Forms.Button();
            this.btExportPython = new System.Windows.Forms.Button();
            this.btHelp = new System.Windows.Forms.Button();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.howToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.documentationMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(690, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "xtUML Model Compiler";
            this.label1.Font = new System.Drawing.Font(this.label1.Font.FontFamily, 20);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DarkTurquoise;
            this.label2.Font = new System.Drawing.Font("Arial", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(180, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(640, 30);
            this.label2.TabIndex = 9;
            this.label2.Text = "JSON";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.DarkSalmon;
            this.label3.Font = new System.Drawing.Font("Arial", 10.125F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(850, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(640, 30);
            this.label3.TabIndex = 10;
            this.label3.Text = "Python";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(710, 50);
            this.label4.Name = "label1";
            this.label4.Size = new System.Drawing.Size(130, 25);
            this.label4.TabIndex = 0;
            this.label4.Text = "from xtUML JSON Model to Python";
            this.label4.Font = new System.Drawing.Font(this.label1.Font.FontFamily, 11);
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textGeneratePython);
            this.panel1.Location = new System.Drawing.Point(850, 120);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 590);
            this.panel1.TabIndex = 1;
            // 
            // textGeneratePython
            // 
            this.textGeneratePython.AutoSize = true;
            this.textGeneratePython.Location = new System.Drawing.Point(3, 9);
            this.textGeneratePython.Name = "textGeneratePython";
            this.textGeneratePython.Size = new System.Drawing.Size(35, 13);
            this.textGeneratePython.TabIndex = 0;
            this.textGeneratePython.ForeColor = System.Drawing.Color.Gray;
            this.textGeneratePython.Text = "translated python appears here...";
            this.textGeneratePython.Font = new System.Drawing.Font(this.textGeneratePython.Font.FontFamily, 10);
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(30, 150);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(125, 40);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Translate";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            this.btnGenerate.Font = new System.Drawing.Font(this.btnGenerate.Font.FontFamily, 9);
            this.btnGenerate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.btnGenerate.BackColor = System.Drawing.Color.DarkSalmon;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(30, 610);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(125, 40);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Reset";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            this.btnClear.Font = new System.Drawing.Font(this.btnClear.Font.FontFamily, 9);
            this.btnClear.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(246)))));
            this.btnClear.BackColor = System.Drawing.Color.DarkSalmon;
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(30, 90);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(125, 40);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Select File";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            this.btnUpload.Font = new System.Drawing.Font(this.btnUpload.Font.FontFamily, 9);
            // 
            // btnParse
            // 
            this.btnParse.Location = new System.Drawing.Point(30, 210);
            this.btnParse.Name = "btnParse";
            this.btnParse.Size = new System.Drawing.Size(125, 40);
            this.btnParse.TabIndex = 1;
            this.btnParse.Text = "Parse";
            this.btnParse.UseVisualStyleBackColor = true;
            this.btnParse.Font = new System.Drawing.Font(this.btnParse.Font.FontFamily, 9);
            // 
            // btnVisualize
            // 
            this.btnVisualize.Location = new System.Drawing.Point(30, 270);
            this.btnVisualize.Name = "btnVisualize";
            this.btnVisualize.Size = new System.Drawing.Size(125, 40);
            this.btnVisualize.TabIndex = 1;
            this.btnVisualize.Text = "Visualize";
            this.btnVisualize.UseVisualStyleBackColor = true;
            this.btnVisualize.Font = new System.Drawing.Font(this.btnVisualize.Font.FontFamily, 9);
            this.btnVisualize.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.btnVisualize.BackColor = System.Drawing.Color.DarkTurquoise;
            // 
            // btnSimulate
            // 
            this.btnSimulate.Location = new System.Drawing.Point(30, 330);
            this.btnSimulate.Name = "btnSimulate";
            this.btnSimulate.Size = new System.Drawing.Size(125, 40);
            this.btnSimulate.TabIndex = 1;
            this.btnSimulate.Text = "Simulate";
            this.btnSimulate.UseVisualStyleBackColor = true;
            this.btnSimulate.Font = new System.Drawing.Font(this.btnSimulate.Font.FontFamily, 9);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(180, 120);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBox1.Size = new System.Drawing.Size(640, 590);
            this.textBox1.TabIndex = 2;
            this.textBox1.WordWrap = false;
            this.textBox1.Font = new System.Drawing.Font(this.textBox1.Font.FontFamily, 10);
            // 
            // bt_copyPy
            //
            this.bt_copyPy.Enabled = false;
            this.bt_copyPy.Location = new System.Drawing.Point(850, 725);
            this.bt_copyPy.Name = "bt_copyPy";
            this.bt_copyPy.Size = new System.Drawing.Size(150, 40);
            this.bt_copyPy.TabIndex = 5;
            this.bt_copyPy.Text = "Copy Python";
            this.bt_copyPy.UseVisualStyleBackColor = true;
            this.bt_copyPy.Click += new System.EventHandler(this.bt_copyPy_Click);
            this.bt_copyPy.Font = new System.Drawing.Font(this.bt_copyPy.Font.FontFamily, 9);
            // 
            // bt_copyJSON
            //
            this.bt_copyJSON.Enabled = false;
            this.bt_copyJSON.Location = new System.Drawing.Point(180, 725);
            this.bt_copyJSON.Name = "bt_copyJSON";
            this.bt_copyJSON.Size = new System.Drawing.Size(150, 40);
            this.bt_copyJSON.TabIndex = 3;
            this.bt_copyJSON.Text = "Copy JSON";
            this.bt_copyJSON.UseVisualStyleBackColor = true;
            this.bt_copyJSON.Click += new System.EventHandler(this.bt_copyJSON_Click);
            this.bt_copyJSON.Font = new System.Drawing.Font(this.bt_copyJSON.Font.FontFamily, 9);
            // 
            // btExportPython
            //
            this.btExportPython.Enabled = false;
            this.btExportPython.Location = new System.Drawing.Point(30, 670);
            this.btExportPython.Name = "btExportPython";
            this.btExportPython.Size = new System.Drawing.Size(125, 40);
            this.btExportPython.TabIndex = 8;
            this.btExportPython.Text = "Save";
            this.btExportPython.UseVisualStyleBackColor = true;
            this.btExportPython.Click += new System.EventHandler(this.btExportPython_Click);
            this.btExportPython.Font = new System.Drawing.Font(this.btExportPython.Font.FontFamily, 9);
            // 
            // btHelp
            // 
            this.btHelp.Location = new System.Drawing.Point(1390, 40);
            this.btHelp.Name = "btHelp";
            this.btHelp.Size = new System.Drawing.Size(100, 33);
            this.btHelp.TabIndex = 8;
            this.btHelp.Text = "Help";
            this.btHelp.UseVisualStyleBackColor = true;
            this.btHelp.Click += new System.EventHandler(this.btHelp_Click);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.howToMenuItem,
            this.documentationMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(167, 48);
            // 
            // howToMenuItem
            // 
            this.howToMenuItem.Name = "howToMenuItem";
            this.howToMenuItem.Size = new System.Drawing.Size(180, 22);
            this.howToMenuItem.Text = "How to Use";
            this.howToMenuItem.Click += new System.EventHandler(this.howToMenuItem_Click);
            // 
            // documentationMenuItem
            // 
            this.documentationMenuItem.Name = "documentationMenuItem";
            this.documentationMenuItem.Size = new System.Drawing.Size(166, 22);
            this.documentationMenuItem.Text = "Documentation...";
            this.documentationMenuItem.Click += new System.EventHandler(this.documentationMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 475);
            this.Controls.Add(this.btHelp);
            this.Controls.Add(this.btExportPython);
            this.Controls.Add(this.bt_copyJSON);
            this.Controls.Add(this.bt_copyPy);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnParse);
            this.Controls.Add(this.btnVisualize);
            this.Controls.Add(this.btnSimulate);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label4);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UML to Python";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label textGeneratePython;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnVisualize;
        private System.Windows.Forms.Button btnSimulate;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bt_copyPy;
        private System.Windows.Forms.Button bt_copyJSON;
        private System.Windows.Forms.Button btExportPython;
        private System.Windows.Forms.Button btHelp;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem howToMenuItem;
        private System.Windows.Forms.ToolStripMenuItem documentationMenuItem;
    }
}
