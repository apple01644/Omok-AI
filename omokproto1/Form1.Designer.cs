namespace omokproto1
{
    partial class Form1
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.start_btn = new System.Windows.Forms.Button();
            this.omokpan = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.cmb_first = new System.Windows.Forms.ComboBox();
            this.isDebugTotal = new System.Windows.Forms.CheckBox();
            this.isDebugAI = new System.Windows.Forms.CheckBox();
            this.isDebugPlayerScore = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // start_btn
            // 
            this.start_btn.Location = new System.Drawing.Point(816, 520);
            this.start_btn.Name = "start_btn";
            this.start_btn.Size = new System.Drawing.Size(155, 50);
            this.start_btn.TabIndex = 2;
            this.start_btn.Text = "start";
            this.start_btn.UseVisualStyleBackColor = true;
            this.start_btn.Click += new System.EventHandler(this.start_btn_Click);
            // 
            // omokpan
            // 
            this.omokpan.BackColor = System.Drawing.Color.Orange;
            this.omokpan.Location = new System.Drawing.Point(10, 10);
            this.omokpan.Name = "omokpan";
            this.omokpan.Size = new System.Drawing.Size(800, 800);
            this.omokpan.TabIndex = 3;
            this.omokpan.Paint += new System.Windows.Forms.PaintEventHandler(this.Omokpan_Paint);
            this.omokpan.MouseDown += new System.Windows.Forms.MouseEventHandler(this.omokpan_MouseDown_1);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
            // 
            // cmb_first
            // 
            this.cmb_first.Font = new System.Drawing.Font("Gulim", 14F);
            this.cmb_first.FormattingEnabled = true;
            this.cmb_first.Items.AddRange(new object[] {
            "User First",
            "AI First"});
            this.cmb_first.Location = new System.Drawing.Point(816, 487);
            this.cmb_first.Name = "cmb_first";
            this.cmb_first.Size = new System.Drawing.Size(155, 27);
            this.cmb_first.TabIndex = 5;
            this.cmb_first.Text = "User First";
            // 
            // isDebugTotal
            // 
            this.isDebugTotal.Location = new System.Drawing.Point(816, 433);
            this.isDebugTotal.Name = "isDebugTotal";
            this.isDebugTotal.Size = new System.Drawing.Size(155, 16);
            this.isDebugTotal.TabIndex = 6;
            this.isDebugTotal.Text = "Debug TotalScore";
            this.isDebugTotal.UseVisualStyleBackColor = true;
            this.isDebugTotal.CheckedChanged += new System.EventHandler(this.isDebugTotal_CheckedChanged);
            // 
            // isDebugAI
            // 
            this.isDebugAI.Location = new System.Drawing.Point(816, 450);
            this.isDebugAI.Name = "isDebugAI";
            this.isDebugAI.Size = new System.Drawing.Size(155, 16);
            this.isDebugAI.TabIndex = 7;
            this.isDebugAI.Text = "Debug AIScore";
            this.isDebugAI.UseVisualStyleBackColor = true;
            this.isDebugAI.CheckedChanged += new System.EventHandler(this.isDebug_CheckedChanged);
            // 
            // isDebugPlayerScore
            // 
            this.isDebugPlayerScore.Location = new System.Drawing.Point(816, 468);
            this.isDebugPlayerScore.Name = "isDebugPlayerScore";
            this.isDebugPlayerScore.Size = new System.Drawing.Size(155, 16);
            this.isDebugPlayerScore.TabIndex = 8;
            this.isDebugPlayerScore.Text = "Debug PlayerScore";
            this.isDebugPlayerScore.UseVisualStyleBackColor = true;
            this.isDebugPlayerScore.CheckedChanged += new System.EventHandler(this.isDebug_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(976, 822);
            this.Controls.Add(this.isDebugPlayerScore);
            this.Controls.Add(this.isDebugAI);
            this.Controls.Add(this.isDebugTotal);
            this.Controls.Add(this.cmb_first);
            this.Controls.Add(this.omokpan);
            this.Controls.Add(this.start_btn);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Text = "omok";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button start_btn;
        private System.Windows.Forms.Panel omokpan;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ComboBox cmb_first;
        private System.Windows.Forms.CheckBox isDebugTotal;
        private System.Windows.Forms.CheckBox isDebugAI;
        private System.Windows.Forms.CheckBox isDebugPlayerScore;
    }
}

