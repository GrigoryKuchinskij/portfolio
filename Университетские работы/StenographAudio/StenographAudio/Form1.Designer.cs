namespace StenographAudio
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.progrBarEncr = new System.Windows.Forms.ProgressBar();
            this.label6 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.DestForEncrFileBtn = new System.Windows.Forms.Button();
            this.FileForHidingSearchBtn = new System.Windows.Forms.Button();
            this.SoundFileSearchBtn = new System.Windows.Forms.Button();
            this.EncryptBtn = new System.Windows.Forms.Button();
            this.TxBxEncryptBit = new System.Windows.Forms.TextBox();
            this.DestForEncrFileTxBx = new System.Windows.Forms.TextBox();
            this.FileForHidingSearchTxBx = new System.Windows.Forms.TextBox();
            this.SoundFileSearchTxBx = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.progrBarDecr = new System.Windows.Forms.ProgressBar();
            this.label7 = new System.Windows.Forms.Label();
            this.DestForDecrFileBtn = new System.Windows.Forms.Button();
            this.EncrFileSearchBtn = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.DecryptBtn = new System.Windows.Forms.Button();
            this.TxBxDecryptBit = new System.Windows.Forms.TextBox();
            this.DestForDecrFileTxBx = new System.Windows.Forms.TextBox();
            this.EncrFileSearchTxBx = new System.Windows.Forms.TextBox();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.progrBarEncr);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.DestForEncrFileBtn);
            this.groupBox1.Controls.Add(this.FileForHidingSearchBtn);
            this.groupBox1.Controls.Add(this.SoundFileSearchBtn);
            this.groupBox1.Controls.Add(this.EncryptBtn);
            this.groupBox1.Controls.Add(this.TxBxEncryptBit);
            this.groupBox1.Controls.Add(this.DestForEncrFileTxBx);
            this.groupBox1.Controls.Add(this.FileForHidingSearchTxBx);
            this.groupBox1.Controls.Add(this.SoundFileSearchTxBx);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(981, 167);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Шифрование";
            // 
            // progrBarEncr
            // 
            this.progrBarEncr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progrBarEncr.Location = new System.Drawing.Point(107, 0);
            this.progrBarEncr.Name = "progrBarEncr";
            this.progrBarEncr.Size = new System.Drawing.Size(477, 16);
            this.progrBarEncr.TabIndex = 6;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(590, -3);
            this.label6.MaximumSize = new System.Drawing.Size(230, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(208, 30);
            this.label6.TabIndex = 5;
            this.label6.Text = "Количество занимаемых младших бит (больше->хуже качество):";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label6.UseMnemonic = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(6, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Выходной файл";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(118, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "Скрываемый файл";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(6, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "Исходный звуковой файл";
            // 
            // DestForEncrFileBtn
            // 
            this.DestForEncrFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DestForEncrFileBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DestForEncrFileBtn.Location = new System.Drawing.Point(950, 138);
            this.DestForEncrFileBtn.Name = "DestForEncrFileBtn";
            this.DestForEncrFileBtn.Size = new System.Drawing.Size(25, 24);
            this.DestForEncrFileBtn.TabIndex = 3;
            this.DestForEncrFileBtn.Text = "...";
            this.DestForEncrFileBtn.UseVisualStyleBackColor = true;
            this.DestForEncrFileBtn.Click += new System.EventHandler(this.DestForEncrFileBtn_Click);
            // 
            // FileForHidingSearchBtn
            // 
            this.FileForHidingSearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FileForHidingSearchBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FileForHidingSearchBtn.Location = new System.Drawing.Point(950, 93);
            this.FileForHidingSearchBtn.Name = "FileForHidingSearchBtn";
            this.FileForHidingSearchBtn.Size = new System.Drawing.Size(25, 24);
            this.FileForHidingSearchBtn.TabIndex = 3;
            this.FileForHidingSearchBtn.Text = "...";
            this.FileForHidingSearchBtn.UseVisualStyleBackColor = true;
            this.FileForHidingSearchBtn.Click += new System.EventHandler(this.FileForHidingSearchBtn_Click);
            // 
            // SoundFileSearchBtn
            // 
            this.SoundFileSearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SoundFileSearchBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.SoundFileSearchBtn.Location = new System.Drawing.Point(950, 48);
            this.SoundFileSearchBtn.Name = "SoundFileSearchBtn";
            this.SoundFileSearchBtn.Size = new System.Drawing.Size(25, 24);
            this.SoundFileSearchBtn.TabIndex = 3;
            this.SoundFileSearchBtn.Text = "...";
            this.SoundFileSearchBtn.UseVisualStyleBackColor = true;
            this.SoundFileSearchBtn.Click += new System.EventHandler(this.SoundFileSearchBtn_Click);
            // 
            // EncryptBtn
            // 
            this.EncryptBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EncryptBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EncryptBtn.Location = new System.Drawing.Point(857, 0);
            this.EncryptBtn.Name = "EncryptBtn";
            this.EncryptBtn.Size = new System.Drawing.Size(118, 24);
            this.EncryptBtn.TabIndex = 2;
            this.EncryptBtn.Text = "Зашифровать";
            this.EncryptBtn.UseVisualStyleBackColor = true;
            this.EncryptBtn.Click += new System.EventHandler(this.EncryptBtn_Click);
            // 
            // TxBxEncryptBit
            // 
            this.TxBxEncryptBit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBxEncryptBit.Location = new System.Drawing.Point(807, 0);
            this.TxBxEncryptBit.Name = "TxBxEncryptBit";
            this.TxBxEncryptBit.Size = new System.Drawing.Size(44, 24);
            this.TxBxEncryptBit.TabIndex = 1;
            this.TxBxEncryptBit.Text = "1";
            // 
            // DestForEncrFileTxBx
            // 
            this.DestForEncrFileTxBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DestForEncrFileTxBx.Location = new System.Drawing.Point(6, 138);
            this.DestForEncrFileTxBx.Name = "DestForEncrFileTxBx";
            this.DestForEncrFileTxBx.Size = new System.Drawing.Size(938, 24);
            this.DestForEncrFileTxBx.TabIndex = 0;
            // 
            // FileForHidingSearchTxBx
            // 
            this.FileForHidingSearchTxBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FileForHidingSearchTxBx.Location = new System.Drawing.Point(6, 93);
            this.FileForHidingSearchTxBx.Name = "FileForHidingSearchTxBx";
            this.FileForHidingSearchTxBx.Size = new System.Drawing.Size(938, 24);
            this.FileForHidingSearchTxBx.TabIndex = 0;
            // 
            // SoundFileSearchTxBx
            // 
            this.SoundFileSearchTxBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SoundFileSearchTxBx.Location = new System.Drawing.Point(6, 48);
            this.SoundFileSearchTxBx.Name = "SoundFileSearchTxBx";
            this.SoundFileSearchTxBx.Size = new System.Drawing.Size(938, 24);
            this.SoundFileSearchTxBx.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.progrBarDecr);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.DestForDecrFileBtn);
            this.groupBox2.Controls.Add(this.EncrFileSearchBtn);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.DecryptBtn);
            this.groupBox2.Controls.Add(this.TxBxDecryptBit);
            this.groupBox2.Controls.Add(this.DestForDecrFileTxBx);
            this.groupBox2.Controls.Add(this.EncrFileSearchTxBx);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.groupBox2.Location = new System.Drawing.Point(12, 185);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(981, 122);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Дешифрование";
            // 
            // progrBarDecr
            // 
            this.progrBarDecr.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progrBarDecr.Location = new System.Drawing.Point(126, 0);
            this.progrBarDecr.Name = "progrBarDecr";
            this.progrBarDecr.Size = new System.Drawing.Size(516, 16);
            this.progrBarDecr.TabIndex = 6;
            // 
            // label7
            // 
            this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label7.Location = new System.Drawing.Point(648, -3);
            this.label7.MaximumSize = new System.Drawing.Size(155, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(153, 30);
            this.label7.TabIndex = 5;
            this.label7.Text = "Количество занимаемых младших бит:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.label7.UseMnemonic = false;
            // 
            // DestForDecrFileBtn
            // 
            this.DestForDecrFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DestForDecrFileBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DestForDecrFileBtn.Location = new System.Drawing.Point(950, 92);
            this.DestForDecrFileBtn.Name = "DestForDecrFileBtn";
            this.DestForDecrFileBtn.Size = new System.Drawing.Size(25, 24);
            this.DestForDecrFileBtn.TabIndex = 3;
            this.DestForDecrFileBtn.Text = "...";
            this.DestForDecrFileBtn.UseVisualStyleBackColor = true;
            this.DestForDecrFileBtn.Click += new System.EventHandler(this.DestForDecrFileBtn_Click);
            // 
            // EncrFileSearchBtn
            // 
            this.EncrFileSearchBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.EncrFileSearchBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.EncrFileSearchBtn.Location = new System.Drawing.Point(950, 47);
            this.EncrFileSearchBtn.Name = "EncrFileSearchBtn";
            this.EncrFileSearchBtn.Size = new System.Drawing.Size(25, 24);
            this.EncrFileSearchBtn.TabIndex = 3;
            this.EncrFileSearchBtn.Text = "...";
            this.EncrFileSearchBtn.UseVisualStyleBackColor = true;
            this.EncrFileSearchBtn.Click += new System.EventHandler(this.EncrFileSearchBtn_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(6, 74);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(100, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "Выходной файл";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(6, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(248, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Звуковой файл со скрытым содержимым";
            // 
            // DecryptBtn
            // 
            this.DecryptBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DecryptBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.DecryptBtn.Location = new System.Drawing.Point(857, 0);
            this.DecryptBtn.Name = "DecryptBtn";
            this.DecryptBtn.Size = new System.Drawing.Size(118, 24);
            this.DecryptBtn.TabIndex = 2;
            this.DecryptBtn.Text = "Расшифровать";
            this.DecryptBtn.UseVisualStyleBackColor = true;
            this.DecryptBtn.Click += new System.EventHandler(this.DecryptBtn_Click);
            // 
            // TxBxDecryptBit
            // 
            this.TxBxDecryptBit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TxBxDecryptBit.Location = new System.Drawing.Point(807, 0);
            this.TxBxDecryptBit.Name = "TxBxDecryptBit";
            this.TxBxDecryptBit.Size = new System.Drawing.Size(44, 24);
            this.TxBxDecryptBit.TabIndex = 1;
            this.TxBxDecryptBit.Text = "1";
            // 
            // DestForDecrFileTxBx
            // 
            this.DestForDecrFileTxBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DestForDecrFileTxBx.Location = new System.Drawing.Point(6, 92);
            this.DestForDecrFileTxBx.Name = "DestForDecrFileTxBx";
            this.DestForDecrFileTxBx.Size = new System.Drawing.Size(938, 24);
            this.DestForDecrFileTxBx.TabIndex = 0;
            // 
            // EncrFileSearchTxBx
            // 
            this.EncrFileSearchTxBx.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.EncrFileSearchTxBx.Location = new System.Drawing.Point(6, 47);
            this.EncrFileSearchTxBx.Name = "EncrFileSearchTxBx";
            this.EncrFileSearchTxBx.Size = new System.Drawing.Size(938, 24);
            this.EncrFileSearchTxBx.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // timer1
            // 
            this.timer1.Interval = 90;
            this.timer1.Tick += new System.EventHandler(this.Timer1_Tick_1);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1005, 315);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.Text = "SteganografAudio";
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Form1_HelpButtonClicked);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button EncryptBtn;
        private System.Windows.Forms.TextBox TxBxEncryptBit;
        private System.Windows.Forms.TextBox DestForEncrFileTxBx;
        private System.Windows.Forms.TextBox FileForHidingSearchTxBx;
        private System.Windows.Forms.TextBox SoundFileSearchTxBx;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button DecryptBtn;
        private System.Windows.Forms.TextBox TxBxDecryptBit;
        private System.Windows.Forms.TextBox DestForDecrFileTxBx;
        private System.Windows.Forms.TextBox EncrFileSearchTxBx;
        private System.Windows.Forms.Button DestForEncrFileBtn;
        private System.Windows.Forms.Button FileForHidingSearchBtn;
        private System.Windows.Forms.Button SoundFileSearchBtn;
        private System.Windows.Forms.Button DestForDecrFileBtn;
        private System.Windows.Forms.Button EncrFileSearchBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ProgressBar progrBarEncr;
        private System.Windows.Forms.ProgressBar progrBarDecr;
        private System.Windows.Forms.Timer timer1;
    }
}

