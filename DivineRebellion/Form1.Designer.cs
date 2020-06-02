namespace DivineRebellion
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.FullClearBtn = new System.Windows.Forms.Button();
            this.InitializeBattleFieldBtn = new System.Windows.Forms.Button();
            this.StartFightBtn = new System.Windows.Forms.Button();
            this.UnitChoiceBox = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // FullClearBtn
            // 
            this.FullClearBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FullClearBtn.Location = new System.Drawing.Point(773, 112);
            this.FullClearBtn.Name = "FullClearBtn";
            this.FullClearBtn.Size = new System.Drawing.Size(202, 92);
            this.FullClearBtn.TabIndex = 0;
            this.FullClearBtn.Text = "Очистить поле";
            this.FullClearBtn.UseVisualStyleBackColor = true;
            this.FullClearBtn.Click += new System.EventHandler(this.FullClearBtn_Click);
            // 
            // InitializeBattleFieldBtn
            // 
            this.InitializeBattleFieldBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InitializeBattleFieldBtn.Location = new System.Drawing.Point(355, 181);
            this.InitializeBattleFieldBtn.Name = "InitializeBattleFieldBtn";
            this.InitializeBattleFieldBtn.Size = new System.Drawing.Size(202, 80);
            this.InitializeBattleFieldBtn.TabIndex = 1;
            this.InitializeBattleFieldBtn.Text = "Start";
            this.InitializeBattleFieldBtn.UseVisualStyleBackColor = true;
            this.InitializeBattleFieldBtn.Click += new System.EventHandler(this.InitializeBattleFieldBtn_Click);
            // 
            // StartFightBtn
            // 
            this.StartFightBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartFightBtn.Location = new System.Drawing.Point(773, 222);
            this.StartFightBtn.Name = "StartFightBtn";
            this.StartFightBtn.Size = new System.Drawing.Size(202, 82);
            this.StartFightBtn.TabIndex = 2;
            this.StartFightBtn.Text = "Начать бой";
            this.StartFightBtn.UseVisualStyleBackColor = true;
            this.StartFightBtn.Click += new System.EventHandler(this.StartFightBtn_Click);
            // 
            // UnitChoiceBox
            // 
            this.UnitChoiceBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.UnitChoiceBox.FormattingEnabled = true;
            this.UnitChoiceBox.Location = new System.Drawing.Point(773, 329);
            this.UnitChoiceBox.Name = "UnitChoiceBox";
            this.UnitChoiceBox.Size = new System.Drawing.Size(202, 21);
            this.UnitChoiceBox.TabIndex = 3;
            this.UnitChoiceBox.SelectedIndexChanged += new System.EventHandler(this.UnitChoiceBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.ClientSize = new System.Drawing.Size(987, 629);
            this.Controls.Add(this.UnitChoiceBox);
            this.Controls.Add(this.StartFightBtn);
            this.Controls.Add(this.InitializeBattleFieldBtn);
            this.Controls.Add(this.FullClearBtn);
            this.Name = "Form1";
            this.Text = "DivineRebellion";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button FullClearBtn;
        private System.Windows.Forms.Button InitializeBattleFieldBtn;
        private System.Windows.Forms.Button StartFightBtn;
        public System.Windows.Forms.ComboBox UnitChoiceBox;
    }
}

