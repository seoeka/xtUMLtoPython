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
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textGeneratePython = new System.Windows.Forms.Label();
            this.btnGenerate = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnUpload = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.bt_copyPy = new System.Windows.Forms.Button();
            this.bt_copyJSON = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(330, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Generate JSON to Python";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.textGeneratePython);
            this.panel1.Location = new System.Drawing.Point(416, 56);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(358, 347);
            this.panel1.TabIndex = 1;
            // 
            // textGeneratePython
            // 
            this.textGeneratePython.AutoSize = true;
            this.textGeneratePython.Location = new System.Drawing.Point(3, 9);
            this.textGeneratePython.Name = "textGeneratePython";
            this.textGeneratePython.Size = new System.Drawing.Size(35, 13);
            this.textGeneratePython.TabIndex = 0;
            this.textGeneratePython.Text = "label2";
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(218, 419);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(178, 33);
            this.btnGenerate.TabIndex = 4;
            this.btnGenerate.Text = "Generate to Python";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(608, 419);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(166, 33);
            this.btnClear.TabIndex = 6;
            this.btnClear.Text = "Clear All Data";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnUpload
            // 
            this.btnUpload.Location = new System.Drawing.Point(26, 12);
            this.btnUpload.Name = "btnUpload";
            this.btnUpload.Size = new System.Drawing.Size(123, 33);
            this.btnUpload.TabIndex = 1;
            this.btnUpload.Text = "Upload File";
            this.btnUpload.UseVisualStyleBackColor = true;
            this.btnUpload.Click += new System.EventHandler(this.btnUpload_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(26, 56);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(370, 346);
            this.textBox1.TabIndex = 2;
            // 
            // bt_copyPy
            // 
            this.bt_copyPy.Location = new System.Drawing.Point(416, 419);
            this.bt_copyPy.Name = "bt_copyPy";
            this.bt_copyPy.Size = new System.Drawing.Size(186, 33);
            this.bt_copyPy.TabIndex = 5;
            this.bt_copyPy.Text = "Copy Code";
            this.bt_copyPy.UseVisualStyleBackColor = true;
            this.bt_copyPy.Click += new System.EventHandler(this.bt_copyPy_Click);
            // 
            // bt_copyJSON
            // 
            this.bt_copyJSON.Location = new System.Drawing.Point(26, 419);
            this.bt_copyJSON.Name = "bt_copyJSON";
            this.bt_copyJSON.Size = new System.Drawing.Size(186, 33);
            this.bt_copyJSON.TabIndex = 3;
            this.bt_copyJSON.Text = "Copy JSON";
            this.bt_copyJSON.UseVisualStyleBackColor = true;
            this.bt_copyJSON.Click += new System.EventHandler(this.bt_copyJSON_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 475);
            this.Controls.Add(this.bt_copyJSON);
            this.Controls.Add(this.bt_copyPy);
            this.Controls.Add(this.btnUpload);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.btnGenerate);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "UML to Python";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label textGeneratePython;
        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnUpload;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button bt_copyPy;
        private System.Windows.Forms.Button bt_copyJSON;
    }
}

