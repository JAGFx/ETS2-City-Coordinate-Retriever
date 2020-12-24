namespace ETS2_Log_to_Coordinates
{
    partial class ConflictSolver
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
            this.listCities = new System.Windows.Forms.ListView();
            this.cmGameName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmRealName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmCountry = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmX = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmY = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cmZ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lHeader = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnAbort = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listCities
            // 
            this.listCities.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listCities.CheckBoxes = true;
            this.listCities.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.cmGameName,
            this.cmRealName,
            this.cmCountry,
            this.cmX,
            this.cmY,
            this.cmZ});
            this.listCities.FullRowSelect = true;
            this.listCities.GridLines = true;
            this.listCities.Location = new System.Drawing.Point(12, 25);
            this.listCities.Name = "listCities";
            this.listCities.Size = new System.Drawing.Size(604, 440);
            this.listCities.TabIndex = 0;
            this.listCities.UseCompatibleStateImageBehavior = false;
            this.listCities.View = System.Windows.Forms.View.Details;
            // 
            // cmGameName
            // 
            this.cmGameName.Text = "gameName";
            this.cmGameName.Width = 136;
            // 
            // cmRealName
            // 
            this.cmRealName.Text = "realName";
            this.cmRealName.Width = 105;
            // 
            // cmCountry
            // 
            this.cmCountry.Text = "country";
            this.cmCountry.Width = 156;
            // 
            // cmX
            // 
            this.cmX.Text = "X";
            this.cmX.Width = 57;
            // 
            // cmY
            // 
            this.cmY.Text = "Y";
            // 
            // cmZ
            // 
            this.cmZ.Text = "Z";
            // 
            // lHeader
            // 
            this.lHeader.AutoSize = true;
            this.lHeader.Location = new System.Drawing.Point(12, 9);
            this.lHeader.Name = "lHeader";
            this.lHeader.Size = new System.Drawing.Size(373, 13);
            this.lHeader.TabIndex = 1;
            this.lHeader.Text = "Select the cities you want to include (the ones in red are detected duplicates):";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExport.Location = new System.Drawing.Point(540, 474);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnAbort
            // 
            this.btnAbort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAbort.Location = new System.Drawing.Point(459, 474);
            this.btnAbort.Name = "btnAbort";
            this.btnAbort.Size = new System.Drawing.Size(75, 23);
            this.btnAbort.TabIndex = 3;
            this.btnAbort.Text = "Abort";
            this.btnAbort.UseVisualStyleBackColor = true;
            this.btnAbort.Click += new System.EventHandler(this.btnAbort_Click);
            // 
            // ConflictSolver
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(628, 509);
            this.Controls.Add(this.btnAbort);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.lHeader);
            this.Controls.Add(this.listCities);
            this.Name = "ConflictSolver";
            this.ShowIcon = false;
            this.Text = "ConflictSolver";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConflictSolver_FormClosing);
            this.Load += new System.EventHandler(this.ConflictSolver_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.ListView listCities;
        private System.Windows.Forms.ColumnHeader cmGameName;
        private System.Windows.Forms.ColumnHeader cmRealName;
        private System.Windows.Forms.ColumnHeader cmCountry;
        private System.Windows.Forms.ColumnHeader cmX;
        private System.Windows.Forms.ColumnHeader cmY;
        private System.Windows.Forms.ColumnHeader cmZ;
        private System.Windows.Forms.Label lHeader;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnAbort;
    }
}