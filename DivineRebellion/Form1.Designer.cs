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
            this.SpawnBtn = new System.Windows.Forms.Button();
            this.InitializeBattleFieldBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // SpawnBtn
            // 
            this.SpawnBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SpawnBtn.Location = new System.Drawing.Point(773, 112);
            this.SpawnBtn.Name = "SpawnBtn";
            this.SpawnBtn.Size = new System.Drawing.Size(202, 92);
            this.SpawnBtn.TabIndex = 0;
            this.SpawnBtn.Text = "Spawn";
            this.SpawnBtn.UseVisualStyleBackColor = true;
            this.SpawnBtn.Click += new System.EventHandler(this.SpawnBtn_Click);
            // 
            // InitializeBattleFieldBtn
            // 
            this.InitializeBattleFieldBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.InitializeBattleFieldBtn.Location = new System.Drawing.Point(773, 12);
            this.InitializeBattleFieldBtn.Name = "InitializeBattleFieldBtn";
            this.InitializeBattleFieldBtn.Size = new System.Drawing.Size(202, 80);
            this.InitializeBattleFieldBtn.TabIndex = 1;
            this.InitializeBattleFieldBtn.Text = "Start";
            this.InitializeBattleFieldBtn.UseVisualStyleBackColor = true;
            this.InitializeBattleFieldBtn.Click += new System.EventHandler(this.InitializeBattleFieldBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(987, 629);
            this.Controls.Add(this.InitializeBattleFieldBtn);
            this.Controls.Add(this.SpawnBtn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button SpawnBtn;
        private System.Windows.Forms.Button InitializeBattleFieldBtn;
    }
}

