namespace emwave_pulse_PC_App
{
   partial class emwave_app
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(emwave_app));
            this.start_btn = new System.Windows.Forms.Button();
            this.graph_ = new ZedGraph.ZedGraphControl();
            this.label1 = new System.Windows.Forms.Label();
            this.HR_Txt = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.session_time_txt = new System.Windows.Forms.Label();
            this.exit_btn = new System.Windows.Forms.Button();
            this.guiTimer_ = new System.Windows.Forms.Timer(this.components);
            this.feedbackTxt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.acc_Calm_Txt = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.calm_Txt = new System.Windows.Forms.TextBox();
            this.upLoadTimer_ = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // start_btn
            // 
            this.start_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.start_btn.BackColor = System.Drawing.Color.Transparent;
            this.start_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("start_btn.BackgroundImage")));
            this.start_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.start_btn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.start_btn.FlatAppearance.BorderSize = 0;
            this.start_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.start_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.start_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.start_btn.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.start_btn.Location = new System.Drawing.Point(980, 568);
            this.start_btn.Margin = new System.Windows.Forms.Padding(0);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(75, 75);
            this.start_btn.TabIndex = 1;
            this.start_btn.Text = "Start";
            this.start_btn.UseVisualStyleBackColor = false;
            this.start_btn.Click += new System.EventHandler(this.Connect_OnListener);
            // 
            // graph_
            // 
            this.graph_.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.graph_.IsEnableHPan = false;
            this.graph_.Location = new System.Drawing.Point(13, 27);
            this.graph_.Name = "graph_";
            this.graph_.ScrollGrace = 0D;
            this.graph_.ScrollMaxX = 0D;
            this.graph_.ScrollMaxY = 0D;
            this.graph_.ScrollMaxY2 = 0D;
            this.graph_.ScrollMinX = 0D;
            this.graph_.ScrollMinY = 0D;
            this.graph_.ScrollMinY2 = 0D;
            this.graph_.Size = new System.Drawing.Size(1133, 442);
            this.graph_.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 510);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Heart Rate:";
            // 
            // HR_Txt
            // 
            this.HR_Txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.HR_Txt.BackColor = System.Drawing.SystemColors.MenuText;
            this.HR_Txt.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HR_Txt.ForeColor = System.Drawing.Color.Lime;
            this.HR_Txt.Location = new System.Drawing.Point(137, 506);
            this.HR_Txt.Margin = new System.Windows.Forms.Padding(10);
            this.HR_Txt.MaximumSize = new System.Drawing.Size(172, 32);
            this.HR_Txt.Name = "HR_Txt";
            this.HR_Txt.ReadOnly = true;
            this.HR_Txt.Size = new System.Drawing.Size(172, 32);
            this.HR_Txt.TabIndex = 5;
            this.HR_Txt.Text = "60";
            this.HR_Txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Times New Roman", 10.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(828, 581);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(115, 17);
            this.label2.TabIndex = 8;
            this.label2.Text = "Remaining Time:";
            // 
            // session_time_txt
            // 
            this.session_time_txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.session_time_txt.AutoSize = true;
            this.session_time_txt.Font = new System.Drawing.Font("Times New Roman", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.session_time_txt.Location = new System.Drawing.Point(843, 611);
            this.session_time_txt.Name = "session_time_txt";
            this.session_time_txt.Size = new System.Drawing.Size(85, 32);
            this.session_time_txt.TabIndex = 9;
            this.session_time_txt.Text = "20:00";
            this.session_time_txt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // exit_btn
            // 
            this.exit_btn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.exit_btn.BackColor = System.Drawing.Color.Transparent;
            this.exit_btn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("exit_btn.BackgroundImage")));
            this.exit_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.exit_btn.FlatAppearance.BorderColor = System.Drawing.Color.Silver;
            this.exit_btn.FlatAppearance.BorderSize = 0;
            this.exit_btn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.exit_btn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.exit_btn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.exit_btn.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exit_btn.Location = new System.Drawing.Point(1071, 568);
            this.exit_btn.Margin = new System.Windows.Forms.Padding(0);
            this.exit_btn.Name = "exit_btn";
            this.exit_btn.Size = new System.Drawing.Size(75, 75);
            this.exit_btn.TabIndex = 10;
            this.exit_btn.Text = "Exit";
            this.exit_btn.UseVisualStyleBackColor = false;
            this.exit_btn.Click += new System.EventHandler(this.exitBtn);
            // 
            // guiTimer_
            // 
            this.guiTimer_.Interval = 47;
            this.guiTimer_.Tick += new System.EventHandler(this.updateGUITimer);
            // 
            // feedbackTxt
            // 
            this.feedbackTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.feedbackTxt.BackColor = System.Drawing.Color.Lime;
            this.feedbackTxt.Location = new System.Drawing.Point(16, 593);
            this.feedbackTxt.Multiline = true;
            this.feedbackTxt.Name = "feedbackTxt";
            this.feedbackTxt.Size = new System.Drawing.Size(770, 50);
            this.feedbackTxt.TabIndex = 11;
            this.feedbackTxt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.feedbackTxt.Visible = false;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(12, 557);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 23);
            this.label3.TabIndex = 12;
            this.label3.Text = "Feedback:";
            // 
            // acc_Calm_Txt
            // 
            this.acc_Calm_Txt.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.acc_Calm_Txt.BackColor = System.Drawing.SystemColors.MenuText;
            this.acc_Calm_Txt.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acc_Calm_Txt.ForeColor = System.Drawing.Color.Lime;
            this.acc_Calm_Txt.Location = new System.Drawing.Point(614, 506);
            this.acc_Calm_Txt.Margin = new System.Windows.Forms.Padding(10);
            this.acc_Calm_Txt.MaximumSize = new System.Drawing.Size(172, 32);
            this.acc_Calm_Txt.Name = "acc_Calm_Txt";
            this.acc_Calm_Txt.ReadOnly = true;
            this.acc_Calm_Txt.Size = new System.Drawing.Size(172, 32);
            this.acc_Calm_Txt.TabIndex = 13;
            this.acc_Calm_Txt.Text = "60";
            this.acc_Calm_Txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(337, 510);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(264, 23);
            this.label4.TabIndex = 14;
            this.label4.Text = "Accumulated Calmness Score:";
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(811, 510);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(150, 23);
            this.label5.TabIndex = 15;
            this.label5.Text = "Calmness Score:";
            // 
            // calm_Txt
            // 
            this.calm_Txt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.calm_Txt.BackColor = System.Drawing.SystemColors.MenuText;
            this.calm_Txt.Font = new System.Drawing.Font("Times New Roman", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.calm_Txt.ForeColor = System.Drawing.Color.Lime;
            this.calm_Txt.Location = new System.Drawing.Point(969, 506);
            this.calm_Txt.Margin = new System.Windows.Forms.Padding(10);
            this.calm_Txt.MaximumSize = new System.Drawing.Size(172, 32);
            this.calm_Txt.Name = "calm_Txt";
            this.calm_Txt.ReadOnly = true;
            this.calm_Txt.Size = new System.Drawing.Size(172, 32);
            this.calm_Txt.TabIndex = 16;
            this.calm_Txt.Text = "60";
            this.calm_Txt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // upLoadTimer_
            // 
            this.upLoadTimer_.Tick += new System.EventHandler(this.updateUploadTimer);
            // 
            // emwave_app
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1160, 672);
            this.Controls.Add(this.calm_Txt);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.acc_Calm_Txt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.feedbackTxt);
            this.Controls.Add(this.exit_btn);
            this.Controls.Add(this.session_time_txt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.HR_Txt);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.graph_);
            this.Controls.Add(this.start_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MinimizeBox = false;
            this.Name = "emwave_app";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "emwave_app";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.exit_Click);
            this.ResumeLayout(false);
            this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.Button start_btn;
      private ZedGraph.ZedGraphControl graph_;
      private System.Windows.Forms.Label label1;
      private System.Windows.Forms.TextBox HR_Txt;
      private System.Windows.Forms.Label label2;
      private System.Windows.Forms.Label session_time_txt;
      private System.Windows.Forms.Button exit_btn;
      private System.Windows.Forms.Timer guiTimer_;
      private System.Windows.Forms.TextBox feedbackTxt;
      private System.Windows.Forms.Label label3;
      private System.Windows.Forms.TextBox acc_Calm_Txt;
      private System.Windows.Forms.Label label4;
      private System.Windows.Forms.Label label5;
      private System.Windows.Forms.TextBox calm_Txt;
      private System.Windows.Forms.Timer upLoadTimer_;
   }
}

