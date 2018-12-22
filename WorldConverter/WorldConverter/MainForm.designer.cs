namespace WorldConverter
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.LoadDirectory = new System.Windows.Forms.TextBox();
            this.SelectDirectory1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SelectDirectory2 = new System.Windows.Forms.Button();
            this.SaveDirectory = new System.Windows.Forms.TextBox();
            this.WorldName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Excecute = new System.Windows.Forms.Button();
            this.Console = new System.Windows.Forms.ListBox();
            this.Progress = new System.Windows.Forms.ProgressBar();
            this.Dimension = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // LoadDirectory
            // 
            this.LoadDirectory.Location = new System.Drawing.Point(45, 67);
            this.LoadDirectory.Name = "LoadDirectory";
            this.LoadDirectory.Size = new System.Drawing.Size(324, 19);
            this.LoadDirectory.TabIndex = 0;
            // 
            // SelectDirectory1
            // 
            this.SelectDirectory1.Location = new System.Drawing.Point(364, 65);
            this.SelectDirectory1.Name = "SelectDirectory1";
            this.SelectDirectory1.Size = new System.Drawing.Size(29, 23);
            this.SelectDirectory1.TabIndex = 1;
            this.SelectDirectory1.Text = "...";
            this.SelectDirectory1.UseVisualStyleBackColor = true;
            this.SelectDirectory1.Click += new System.EventHandler(this.SelectLoadDirectory);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(117, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(170, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "変換するワールドを選択してください";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(142, 138);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(125, 12);
            this.label2.TabIndex = 5;
            this.label2.Text = "保存先を選択してください";
            // 
            // SelectDirectory2
            // 
            this.SelectDirectory2.Location = new System.Drawing.Point(364, 170);
            this.SelectDirectory2.Name = "SelectDirectory2";
            this.SelectDirectory2.Size = new System.Drawing.Size(29, 23);
            this.SelectDirectory2.TabIndex = 4;
            this.SelectDirectory2.Text = "...";
            this.SelectDirectory2.UseVisualStyleBackColor = true;
            this.SelectDirectory2.Click += new System.EventHandler(this.SelectSaveDirectory);
            // 
            // SaveDirectory
            // 
            this.SaveDirectory.Location = new System.Drawing.Point(45, 172);
            this.SaveDirectory.Name = "SaveDirectory";
            this.SaveDirectory.Size = new System.Drawing.Size(323, 19);
            this.SaveDirectory.TabIndex = 3;
            // 
            // WorldName
            // 
            this.WorldName.Location = new System.Drawing.Point(45, 281);
            this.WorldName.Name = "WorldName";
            this.WorldName.Size = new System.Drawing.Size(323, 19);
            this.WorldName.TabIndex = 6;
            this.WorldName.Text = "world";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(117, 245);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(185, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "変換後のワールド名を入力してください";
            // 
            // Excecute
            // 
            this.Excecute.Location = new System.Drawing.Point(397, 397);
            this.Excecute.Name = "Excecute";
            this.Excecute.Size = new System.Drawing.Size(88, 23);
            this.Excecute.TabIndex = 8;
            this.Excecute.Text = "実行する";
            this.Excecute.UseVisualStyleBackColor = true;
            this.Excecute.Click += new System.EventHandler(this.ExcecuteConvert);
            // 
            // Console
            // 
            this.Console.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.Console.Cursor = System.Windows.Forms.Cursors.Default;
            this.Console.FormattingEnabled = true;
            this.Console.HorizontalScrollbar = true;
            this.Console.ItemHeight = 12;
            this.Console.Location = new System.Drawing.Point(521, 53);
            this.Console.Name = "Console";
            this.Console.ScrollAlwaysVisible = true;
            this.Console.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.Console.Size = new System.Drawing.Size(291, 316);
            this.Console.TabIndex = 9;
            // 
            // Progress
            // 
            this.Progress.Location = new System.Drawing.Point(521, 397);
            this.Progress.Name = "Progress";
            this.Progress.Size = new System.Drawing.Size(291, 23);
            this.Progress.TabIndex = 10;
            // 
            // Dimension
            // 
            this.Dimension.FormattingEnabled = true;
            this.Dimension.Items.AddRange(new object[] {
            "Overworld",
            "Nether",
            "End"});
            this.Dimension.Location = new System.Drawing.Point(144, 362);
            this.Dimension.Name = "Dimension";
            this.Dimension.Size = new System.Drawing.Size(121, 20);
            this.Dimension.TabIndex = 12;
            this.Dimension.Text = "Overworld";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(130, 326);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(148, 12);
            this.label4.TabIndex = 13;
            this.label4.Text = "ディメンションを選択してください";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(865, 471);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.Dimension);
            this.Controls.Add(this.Progress);
            this.Controls.Add(this.Console);
            this.Controls.Add(this.Excecute);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.WorldName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SelectDirectory2);
            this.Controls.Add(this.SaveDirectory);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.SelectDirectory1);
            this.Controls.Add(this.LoadDirectory);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox LoadDirectory;
        private System.Windows.Forms.Button SelectDirectory1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button SelectDirectory2;
        private System.Windows.Forms.TextBox SaveDirectory;
        private System.Windows.Forms.TextBox WorldName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button Excecute;
        private System.Windows.Forms.ListBox Console;
        private System.Windows.Forms.ProgressBar Progress;
        private System.Windows.Forms.ComboBox Dimension;
        private System.Windows.Forms.Label label4;
    }
}

