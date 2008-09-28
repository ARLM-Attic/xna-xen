namespace Tutorials
{
	partial class TutorialChooser
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
			System.Windows.Forms.Button button1;
			System.Windows.Forms.Button button2;
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TutorialChooser));
			this.tutorialList = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			button1 = new System.Windows.Forms.Button();
			button2 = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// button1
			// 
			button1.Location = new System.Drawing.Point(246, 19);
			button1.Name = "button1";
			button1.Size = new System.Drawing.Size(75, 23);
			button1.TabIndex = 1;
			button1.Text = "&OK";
			button1.UseVisualStyleBackColor = true;
			button1.Click += new System.EventHandler(this.OK);
			// 
			// button2
			// 
			button2.Location = new System.Drawing.Point(246, 48);
			button2.Name = "button2";
			button2.Size = new System.Drawing.Size(75, 23);
			button2.TabIndex = 2;
			button2.Text = "&Cancel";
			button2.UseVisualStyleBackColor = true;
			button2.Click += new System.EventHandler(this.Cancel);
			// 
			// tutorialList
			// 
			this.tutorialList.FormattingEnabled = true;
			this.tutorialList.Location = new System.Drawing.Point(22, 19);
			this.tutorialList.Name = "tutorialList";
			this.tutorialList.Size = new System.Drawing.Size(218, 21);
			this.tutorialList.TabIndex = 0;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(button2);
			this.groupBox1.Controls.Add(button1);
			this.groupBox1.Controls.Add(this.tutorialList);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(340, 87);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Select a tutorial to run:";
			// 
			// TutorialChooser
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(359, 107);
			this.Controls.Add(this.groupBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TutorialChooser";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Tutorial Chooser";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ComboBox tutorialList;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}