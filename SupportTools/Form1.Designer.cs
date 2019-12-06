namespace SupportTools
{
    partial class Form1
    {
        /// <summary>
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Windows Form Designer

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.NumeroSerieAtual = new System.Windows.Forms.ComboBox();
            this.btAtualizaNum_Serie = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.Numero_Serie = new System.Windows.Forms.TextBox();
            this.btAjustaChave = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ChaveTxt = new System.Windows.Forms.RichTextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button14 = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button12 = new System.Windows.Forms.Button();
            this.btApagarDuplicados = new System.Windows.Forms.Button();
            this.btCo19Date = new System.Windows.Forms.Button();
            this.btDeleteResumo = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.btResetNuvem = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.btUpdateTuss = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btDat005 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbNaoApagar = new System.Windows.Forms.RadioButton();
            this.rbApagar = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.btApagaMedico = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.label4 = new System.Windows.Forms.Label();
            this.Emails = new System.Windows.Forms.ComboBox();
            this.tabPage5.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.panel1);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(807, 644);
            this.tabPage5.TabIndex = 5;
            this.tabPage5.Text = "12.1.16 SCRIPTS SINCRONISMO";
            this.tabPage5.UseVisualStyleBackColor = true;
            this.tabPage5.Click += new System.EventHandler(this.tabPage5_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.Emails);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.NumeroSerieAtual);
            this.panel1.Controls.Add(this.btAtualizaNum_Serie);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.Numero_Serie);
            this.panel1.Controls.Add(this.btAjustaChave);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.ChaveTxt);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.button14);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.btApagarDuplicados);
            this.panel1.Controls.Add(this.btCo19Date);
            this.panel1.Controls.Add(this.btDeleteResumo);
            this.panel1.Controls.Add(this.button4);
            this.panel1.Controls.Add(this.btResetNuvem);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.btUpdateTuss);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.btDat005);
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.button10);
            this.panel1.Controls.Add(this.button7);
            this.panel1.Controls.Add(this.button9);
            this.panel1.Controls.Add(this.button8);
            this.panel1.Location = new System.Drawing.Point(6, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(792, 634);
            this.panel1.TabIndex = 31;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(520, 250);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 60;
            this.label3.Text = "NUMERO DE SERIE";
            // 
            // NumeroSerieAtual
            // 
            this.NumeroSerieAtual.FormattingEnabled = true;
            this.NumeroSerieAtual.Location = new System.Drawing.Point(523, 269);
            this.NumeroSerieAtual.Name = "NumeroSerieAtual";
            this.NumeroSerieAtual.Size = new System.Drawing.Size(242, 21);
            this.NumeroSerieAtual.TabIndex = 59;
            // 
            // btAtualizaNum_Serie
            // 
            this.btAtualizaNum_Serie.Location = new System.Drawing.Point(523, 350);
            this.btAtualizaNum_Serie.Name = "btAtualizaNum_Serie";
            this.btAtualizaNum_Serie.Size = new System.Drawing.Size(128, 23);
            this.btAtualizaNum_Serie.TabIndex = 58;
            this.btAtualizaNum_Serie.Text = "ATUALIZAR";
            this.btAtualizaNum_Serie.UseVisualStyleBackColor = true;
            this.btAtualizaNum_Serie.Click += new System.EventHandler(this.btAtualizaNum_Serie_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(520, 307);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(170, 13);
            this.label2.TabIndex = 57;
            this.label2.Text = "ATUALIZAÇÃO NUMERO_SERIE";
            // 
            // Numero_Serie
            // 
            this.Numero_Serie.Location = new System.Drawing.Point(523, 323);
            this.Numero_Serie.Name = "Numero_Serie";
            this.Numero_Serie.Size = new System.Drawing.Size(242, 20);
            this.Numero_Serie.TabIndex = 56;
            // 
            // btAjustaChave
            // 
            this.btAjustaChave.Location = new System.Drawing.Point(16, 528);
            this.btAjustaChave.Name = "btAjustaChave";
            this.btAjustaChave.Size = new System.Drawing.Size(128, 23);
            this.btAjustaChave.TabIndex = 55;
            this.btAjustaChave.Text = "AJUSTA CHAVE";
            this.btAjustaChave.UseVisualStyleBackColor = true;
            this.btAjustaChave.Click += new System.EventHandler(this.btAjustaChave_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 231);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 13);
            this.label1.TabIndex = 54;
            this.label1.Text = "ATUALIZAÇÃO CHAVE";
            // 
            // ChaveTxt
            // 
            this.ChaveTxt.Location = new System.Drawing.Point(16, 250);
            this.ChaveTxt.Name = "ChaveTxt";
            this.ChaveTxt.Size = new System.Drawing.Size(501, 272);
            this.ChaveTxt.TabIndex = 53;
            this.ChaveTxt.Text = "";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(529, 16);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(242, 27);
            this.button1.TabIndex = 47;
            this.button1.Text = "CORRIGIR CPF";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.CorrigiCPF);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(529, 42);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(242, 36);
            this.button14.TabIndex = 44;
            this.button14.Text = "Delete Servico";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.DeleteServico);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.progressBar1);
            this.groupBox2.Controls.Add(this.button12);
            this.groupBox2.Location = new System.Drawing.Point(523, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(261, 83);
            this.groupBox2.TabIndex = 43;
            this.groupBox2.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(3, 46);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 3;
            this.label6.Text = "Progresso";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(6, 62);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(251, 15);
            this.progressBar1.TabIndex = 2;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(6, 10);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(242, 33);
            this.button12.TabIndex = 0;
            this.button12.Text = "EXECUTE ALL";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // btApagarDuplicados
            // 
            this.btApagarDuplicados.Location = new System.Drawing.Point(275, 185);
            this.btApagarDuplicados.Name = "btApagarDuplicados";
            this.btApagarDuplicados.Size = new System.Drawing.Size(242, 30);
            this.btApagarDuplicados.TabIndex = 31;
            this.btApagarDuplicados.Text = "DELETAR AGENDAMENTO DUPLICADO";
            this.btApagarDuplicados.UseVisualStyleBackColor = true;
            this.btApagarDuplicados.Click += new System.EventHandler(this.btApagarDuplicados_Click);
            // 
            // btCo19Date
            // 
            this.btCo19Date.Location = new System.Drawing.Point(16, 16);
            this.btCo19Date.Name = "btCo19Date";
            this.btCo19Date.Size = new System.Drawing.Size(242, 31);
            this.btCo19Date.TabIndex = 8;
            this.btCo19Date.Text = "ADD COLUMN CO19 CLOUD SYNC DATE";
            this.btCo19Date.UseVisualStyleBackColor = true;
            this.btCo19Date.Click += new System.EventHandler(this.ADD_COLUMN_CO19_CLOUD_SYNC_DATE);
            // 
            // btDeleteResumo
            // 
            this.btDeleteResumo.Location = new System.Drawing.Point(275, 93);
            this.btDeleteResumo.Name = "btDeleteResumo";
            this.btDeleteResumo.Size = new System.Drawing.Size(242, 45);
            this.btDeleteResumo.TabIndex = 30;
            this.btDeleteResumo.Text = "DELETE RESUMO\\LOCK";
            this.btDeleteResumo.UseVisualStyleBackColor = true;
            this.btDeleteResumo.Click += new System.EventHandler(this.DELETE_RESUMO_LOCK);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(16, 45);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(242, 31);
            this.button4.TabIndex = 9;
            this.button4.Text = "ADD COLUMN CO19 CLOUD SYNC ID";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.ADD_COLUMN_CO19_CLOUD_SYNC_ID);
            // 
            // btResetNuvem
            // 
            this.btResetNuvem.Location = new System.Drawing.Point(275, 65);
            this.btResetNuvem.Name = "btResetNuvem";
            this.btResetNuvem.Size = new System.Drawing.Size(242, 30);
            this.btResetNuvem.TabIndex = 29;
            this.btResetNuvem.Text = "RESET NUVEM";
            this.btResetNuvem.UseVisualStyleBackColor = true;
            this.btResetNuvem.Click += new System.EventHandler(this.RESET_NUVEM);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(16, 74);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(242, 31);
            this.button3.TabIndex = 10;
            this.button3.Text = "ADD COLUMN CO50 CLOUD SYNC DATE";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.ADD_COLUMN_CO50_CLOUD_SYNC_DATE);
            // 
            // btUpdateTuss
            // 
            this.btUpdateTuss.Location = new System.Drawing.Point(275, 45);
            this.btUpdateTuss.Name = "btUpdateTuss";
            this.btUpdateTuss.Size = new System.Drawing.Size(242, 29);
            this.btUpdateTuss.TabIndex = 28;
            this.btUpdateTuss.Text = "UPDATE TUSS";
            this.btUpdateTuss.UseVisualStyleBackColor = true;
            this.btUpdateTuss.Click += new System.EventHandler(this.UPDATE_TUSS);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(16, 103);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(242, 31);
            this.button2.TabIndex = 11;
            this.button2.Text = "ADD COLUMN CO50 CLOUD SYNC ID";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.ADD_COLUMN_CO50_CLOUD_SYNC_ID);
            // 
            // btDat005
            // 
            this.btDat005.Location = new System.Drawing.Point(275, 16);
            this.btDat005.Name = "btDat005";
            this.btDat005.Size = new System.Drawing.Size(242, 33);
            this.btDat005.TabIndex = 27;
            this.btDat005.Text = "DAT005";
            this.btDat005.UseVisualStyleBackColor = true;
            this.btDat005.Click += new System.EventHandler(this.DAT005);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(16, 131);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(242, 31);
            this.button6.TabIndex = 12;
            this.button6.Text = "ADD COLUMN CO51 CLOUD SYNC DATE";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.ADD_COLUMN_CO51_CLOUD_SYNC_DATE);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(16, 184);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(242, 31);
            this.button10.TabIndex = 16;
            this.button10.Text = "REPAIR DAT005";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.REPAIR_DAT005);
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(16, 159);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(242, 31);
            this.button7.TabIndex = 13;
            this.button7.Text = "ADD COLUMN CO51 CLOUD SYNC ID";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.ADD_COLUMN_CO51_CLOUD_SYNC_ID);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(275, 159);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(242, 31);
            this.button9.TabIndex = 15;
            this.button9.Text = "INSERT NEW_DAT005";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.INSERT_NEW_DAT005);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(275, 131);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(242, 31);
            this.button8.TabIndex = 14;
            this.button8.Text = "DROP AND CREATE NEW_DAT005";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.DROP_AND_CREATE_NEW_DAT005);
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox1);
            this.tabPage2.Controls.Add(this.comboBox1);
            this.tabPage2.Controls.Add(this.btApagaMedico);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(807, 589);
            this.tabPage2.TabIndex = 3;
            this.tabPage2.Text = "Deletar Médico";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbNaoApagar);
            this.groupBox1.Controls.Add(this.rbApagar);
            this.groupBox1.Location = new System.Drawing.Point(194, 154);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(229, 69);
            this.groupBox1.TabIndex = 5;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "APAGAR MÉDICO";
            // 
            // rbNaoApagar
            // 
            this.rbNaoApagar.AutoSize = true;
            this.rbNaoApagar.Checked = true;
            this.rbNaoApagar.Location = new System.Drawing.Point(120, 33);
            this.rbNaoApagar.Name = "rbNaoApagar";
            this.rbNaoApagar.Size = new System.Drawing.Size(95, 17);
            this.rbNaoApagar.TabIndex = 1;
            this.rbNaoApagar.TabStop = true;
            this.rbNaoApagar.Text = "NÃO APAGAR";
            this.rbNaoApagar.UseVisualStyleBackColor = true;
            // 
            // rbApagar
            // 
            this.rbApagar.AutoSize = true;
            this.rbApagar.Location = new System.Drawing.Point(0, 33);
            this.rbApagar.Name = "rbApagar";
            this.rbApagar.Size = new System.Drawing.Size(114, 17);
            this.rbApagar.TabIndex = 0;
            this.rbApagar.Text = "DESEJO APAGAR";
            this.rbApagar.UseVisualStyleBackColor = true;
            this.rbApagar.CheckedChanged += new System.EventHandler(this.rbApagar_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(194, 46);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(229, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // btApagaMedico
            // 
            this.btApagaMedico.Enabled = false;
            this.btApagaMedico.Location = new System.Drawing.Point(194, 96);
            this.btApagaMedico.Name = "btApagaMedico";
            this.btApagaMedico.Size = new System.Drawing.Size(229, 23);
            this.btApagaMedico.TabIndex = 2;
            this.btApagaMedico.Text = "APAGAR MÉDICO";
            this.btApagaMedico.UseVisualStyleBackColor = true;
            this.btApagaMedico.Click += new System.EventHandler(this.DeletarMedico);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage5);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(815, 670);
            this.tabControl1.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(523, 389);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 61;
            this.label4.Text = "EMAIL DOS MÉDICOS";
            // 
            // Emails
            // 
            this.Emails.FormattingEnabled = true;
            this.Emails.Location = new System.Drawing.Point(523, 405);
            this.Emails.Name = "Emails";
            this.Emails.Size = new System.Drawing.Size(242, 21);
            this.Emails.TabIndex = 62;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(831, 684);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Tools";
            this.tabPage5.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button btApagarDuplicados;
        private System.Windows.Forms.Button btCo19Date;
        private System.Windows.Forms.Button btDeleteResumo;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btResetNuvem;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btUpdateTuss;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btDat005;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNaoApagar;
        private System.Windows.Forms.RadioButton rbApagar;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Button btApagaMedico;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RichTextBox ChaveTxt;
        private System.Windows.Forms.Button btAjustaChave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btAtualizaNum_Serie;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox Numero_Serie;
        private System.Windows.Forms.ComboBox NumeroSerieAtual;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox Emails;
        private System.Windows.Forms.Label label4;
    }
}

