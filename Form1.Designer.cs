namespace DLLToFrontend
{
    partial class Form1
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
            label1 = new Label();
            label2 = new Label();
            lblstatusplaceholder = new Label();
            btnDLLSelect = new Button();
            btnSavingLocation = new Button();
            btnGenerate = new Button();
            lbldlllocation = new TextBox();
            lblsavinglocation = new TextBox();
            label4 = new Label();
            lblStatus = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(43, 95);
            label1.Name = "label1";
            label1.Size = new Size(76, 15);
            label1.TabIndex = 0;
            label1.Text = "DLL Location";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(43, 144);
            label2.Name = "label2";
            label2.Size = new Size(91, 15);
            label2.TabIndex = 1;
            label2.Text = "Saving Location";
            // 
            // lblstatusplaceholder
            // 
            lblstatusplaceholder.AutoSize = true;
            lblstatusplaceholder.Location = new Point(43, 237);
            lblstatusplaceholder.Name = "lblstatusplaceholder";
            lblstatusplaceholder.Size = new Size(47, 15);
            lblstatusplaceholder.TabIndex = 2;
            lblstatusplaceholder.Text = "status : ";
            // 
            // btnDLLSelect
            // 
            btnDLLSelect.Location = new Point(357, 92);
            btnDLLSelect.Name = "btnDLLSelect";
            btnDLLSelect.Size = new Size(75, 23);
            btnDLLSelect.TabIndex = 3;
            btnDLLSelect.Text = "SELECT";
            btnDLLSelect.UseVisualStyleBackColor = true;
            // 
            // btnSavingLocation
            // 
            btnSavingLocation.Location = new Point(373, 141);
            btnSavingLocation.Name = "btnSavingLocation";
            btnSavingLocation.Size = new Size(75, 23);
            btnSavingLocation.TabIndex = 4;
            btnSavingLocation.Text = "SELECT";
            btnSavingLocation.UseVisualStyleBackColor = true;
            // 
            // btnGenerate
            // 
            btnGenerate.Location = new Point(43, 189);
            btnGenerate.Name = "btnGenerate";
            btnGenerate.Size = new Size(117, 23);
            btnGenerate.TabIndex = 5;
            btnGenerate.Text = "GENERATE FILES";
            btnGenerate.UseVisualStyleBackColor = true;
            // 
            // lbldlllocation
            // 
            lbldlllocation.Location = new Point(123, 92);
            lbldlllocation.Name = "lbldlllocation";
            lbldlllocation.Size = new Size(229, 23);
            lbldlllocation.TabIndex = 6;
            // 
            // lblsavinglocation
            // 
            lblsavinglocation.Location = new Point(138, 141);
            lblsavinglocation.Name = "lblsavinglocation";
            lblsavinglocation.Size = new Size(229, 23);
            lblsavinglocation.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label4.Location = new Point(324, 18);
            label4.Name = "label4";
            label4.Size = new Size(173, 30);
            label4.TabIndex = 8;
            label4.Text = "DLL TO ReactJS";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = Color.Red;
            lblStatus.Location = new Point(96, 237);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(38, 15);
            lblStatus.TabIndex = 9;
            lblStatus.Text = "status";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(lblStatus);
            Controls.Add(label4);
            Controls.Add(lblsavinglocation);
            Controls.Add(lbldlllocation);
            Controls.Add(btnGenerate);
            Controls.Add(btnSavingLocation);
            Controls.Add(btnDLLSelect);
            Controls.Add(lblstatusplaceholder);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Form1";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private Label lblstatusplaceholder;
        private Button btnDLLSelect;
        private Button btnSavingLocation;
        private Button btnGenerate;
        private TextBox lbldlllocation;
        private TextBox lblsavinglocation;
        private Label label4;
        private Label lblStatus;
    }
}
