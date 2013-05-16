using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Media;
using System.Windows.Media;
using System.Net;
using System.Globalization;


using Styx;
using Styx.Common;
using Styx.CommonBot;
using Styx.Helpers;
using Styx.WoWInternals;
using Styx.WoWInternals.WoWObjects;
using Styx.Plugins;
using Styx.Pathing;

namespace ProfileChanger
{
    partial class ProfileChangerUI
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.TBProfile1 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.TBProfile2 = new System.Windows.Forms.TextBox();
            this.TBProfile3 = new System.Windows.Forms.TextBox();
            this.TBProfile4 = new System.Windows.Forms.TextBox();
            this.TBProfile5 = new System.Windows.Forms.TextBox();
            this.TBProfile6 = new System.Windows.Forms.TextBox();
            this.BSearchFile1 = new System.Windows.Forms.Button();
            this.BSearchFile2 = new System.Windows.Forms.Button();
            this.BSearchFile3 = new System.Windows.Forms.Button();
            this.BSearchFile4 = new System.Windows.Forms.Button();
            this.BSearchFile5 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.TBMinutes2 = new System.Windows.Forms.TextBox();
            this.TBMinutes3 = new System.Windows.Forms.TextBox();
            this.TBMinutes4 = new System.Windows.Forms.TextBox();
            this.TBMinutes5 = new System.Windows.Forms.TextBox();
            this.TBMinutes6 = new System.Windows.Forms.TextBox();
            this.BSave = new System.Windows.Forms.Button();
            this.CB2 = new System.Windows.Forms.CheckBox();
            this.CB3 = new System.Windows.Forms.CheckBox();
            this.CB4 = new System.Windows.Forms.CheckBox();
            this.CB5 = new System.Windows.Forms.CheckBox();
            this.CB6 = new System.Windows.Forms.CheckBox();
            this.CBStopBot = new System.Windows.Forms.CheckBox();
            this.CB1 = new System.Windows.Forms.CheckBox();
            this.BSearchFile6 = new System.Windows.Forms.Button();
            this.TBProfile7 = new System.Windows.Forms.TextBox();
            this.TBProfile8 = new System.Windows.Forms.TextBox();
            this.TBProfile9 = new System.Windows.Forms.TextBox();
            this.TBProfile10 = new System.Windows.Forms.TextBox();
            this.TBProfile11 = new System.Windows.Forms.TextBox();
            this.TBProfile12 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.TBMinutes7 = new System.Windows.Forms.TextBox();
            this.TBMinutes8 = new System.Windows.Forms.TextBox();
            this.TBMinutes9 = new System.Windows.Forms.TextBox();
            this.TBMinutes10 = new System.Windows.Forms.TextBox();
            this.TBMinutes11 = new System.Windows.Forms.TextBox();
            this.TBMinutes12 = new System.Windows.Forms.TextBox();
            this.BSearchFile7 = new System.Windows.Forms.Button();
            this.BSearchFile8 = new System.Windows.Forms.Button();
            this.BSearchFile9 = new System.Windows.Forms.Button();
            this.BSearchFile10 = new System.Windows.Forms.Button();
            this.BSearchFile11 = new System.Windows.Forms.Button();
            this.BSearchFile12 = new System.Windows.Forms.Button();
            this.CB7 = new System.Windows.Forms.CheckBox();
            this.CB8 = new System.Windows.Forms.CheckBox();
            this.CB9 = new System.Windows.Forms.CheckBox();
            this.CB10 = new System.Windows.Forms.CheckBox();
            this.CB11 = new System.Windows.Forms.CheckBox();
            this.CB12 = new System.Windows.Forms.CheckBox();
            this.TBMinutes1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // TBProfile1
            // 
            this.TBProfile1.Enabled = false;
            this.TBProfile1.Location = new System.Drawing.Point(99, 25);
            this.TBProfile1.Name = "TBProfile1";
            this.TBProfile1.Size = new System.Drawing.Size(276, 20);
            this.TBProfile1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Profile 1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(35, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Profile 2";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(35, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(54, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Profile 3";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(35, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(54, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Profile 4";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(35, 132);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Profile 5";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(35, 158);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Profile 6";
            // 
            // TBProfile2
            // 
            this.TBProfile2.Enabled = false;
            this.TBProfile2.Location = new System.Drawing.Point(99, 51);
            this.TBProfile2.Name = "TBProfile2";
            this.TBProfile2.Size = new System.Drawing.Size(276, 20);
            this.TBProfile2.TabIndex = 0;
            // 
            // TBProfile3
            // 
            this.TBProfile3.Enabled = false;
            this.TBProfile3.Location = new System.Drawing.Point(99, 77);
            this.TBProfile3.Name = "TBProfile3";
            this.TBProfile3.Size = new System.Drawing.Size(276, 20);
            this.TBProfile3.TabIndex = 0;
            // 
            // TBProfile4
            // 
            this.TBProfile4.Enabled = false;
            this.TBProfile4.Location = new System.Drawing.Point(99, 103);
            this.TBProfile4.Name = "TBProfile4";
            this.TBProfile4.Size = new System.Drawing.Size(276, 20);
            this.TBProfile4.TabIndex = 0;
            // 
            // TBProfile5
            // 
            this.TBProfile5.Enabled = false;
            this.TBProfile5.Location = new System.Drawing.Point(99, 129);
            this.TBProfile5.Name = "TBProfile5";
            this.TBProfile5.Size = new System.Drawing.Size(276, 20);
            this.TBProfile5.TabIndex = 0;
            // 
            // TBProfile6
            // 
            this.TBProfile6.Enabled = false;
            this.TBProfile6.Location = new System.Drawing.Point(99, 155);
            this.TBProfile6.Name = "TBProfile6";
            this.TBProfile6.Size = new System.Drawing.Size(276, 20);
            this.TBProfile6.TabIndex = 0;
            // 
            // BSearchFile1
            // 
            this.BSearchFile1.Location = new System.Drawing.Point(381, 23);
            this.BSearchFile1.Name = "BSearchFile1";
            this.BSearchFile1.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile1.TabIndex = 2;
            this.BSearchFile1.Text = "Search";
            this.BSearchFile1.UseVisualStyleBackColor = true;
            this.BSearchFile1.Click += new System.EventHandler(this.BSearchFile1_Click);
            // 
            // BSearchFile2
            // 
            this.BSearchFile2.Location = new System.Drawing.Point(381, 49);
            this.BSearchFile2.Name = "BSearchFile2";
            this.BSearchFile2.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile2.TabIndex = 2;
            this.BSearchFile2.Text = "Search";
            this.BSearchFile2.UseVisualStyleBackColor = true;
            this.BSearchFile2.Click += new System.EventHandler(this.BSearchFile2_Click);
            // 
            // BSearchFile3
            // 
            this.BSearchFile3.Location = new System.Drawing.Point(381, 75);
            this.BSearchFile3.Name = "BSearchFile3";
            this.BSearchFile3.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile3.TabIndex = 2;
            this.BSearchFile3.Text = "Search";
            this.BSearchFile3.UseVisualStyleBackColor = true;
            this.BSearchFile3.Click += new System.EventHandler(this.BSearchFile3_Click);
            // 
            // BSearchFile4
            // 
            this.BSearchFile4.Location = new System.Drawing.Point(381, 101);
            this.BSearchFile4.Name = "BSearchFile4";
            this.BSearchFile4.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile4.TabIndex = 2;
            this.BSearchFile4.Text = "Search";
            this.BSearchFile4.UseVisualStyleBackColor = true;
            this.BSearchFile4.Click += new System.EventHandler(this.BSearchFile4_Click);
            // 
            // BSearchFile5
            // 
            this.BSearchFile5.Location = new System.Drawing.Point(381, 127);
            this.BSearchFile5.Name = "BSearchFile5";
            this.BSearchFile5.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile5.TabIndex = 2;
            this.BSearchFile5.Text = "Search";
            this.BSearchFile5.UseVisualStyleBackColor = true;
            this.BSearchFile5.Click += new System.EventHandler(this.BSearchFile5_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(210, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Path";
            // 
            // TBMinutes2
            // 
            this.TBMinutes2.Enabled = false;
            this.TBMinutes2.Location = new System.Drawing.Point(438, 51);
            this.TBMinutes2.Name = "TBMinutes2";
            this.TBMinutes2.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes2.TabIndex = 4;
            // 
            // TBMinutes3
            // 
            this.TBMinutes3.Enabled = false;
            this.TBMinutes3.Location = new System.Drawing.Point(438, 77);
            this.TBMinutes3.Name = "TBMinutes3";
            this.TBMinutes3.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes3.TabIndex = 4;
            // 
            // TBMinutes4
            // 
            this.TBMinutes4.Enabled = false;
            this.TBMinutes4.Location = new System.Drawing.Point(438, 103);
            this.TBMinutes4.Name = "TBMinutes4";
            this.TBMinutes4.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes4.TabIndex = 4;
            // 
            // TBMinutes5
            // 
            this.TBMinutes5.Enabled = false;
            this.TBMinutes5.Location = new System.Drawing.Point(438, 129);
            this.TBMinutes5.Name = "TBMinutes5";
            this.TBMinutes5.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes5.TabIndex = 4;
            // 
            // TBMinutes6
            // 
            this.TBMinutes6.Enabled = false;
            this.TBMinutes6.Location = new System.Drawing.Point(438, 155);
            this.TBMinutes6.Name = "TBMinutes6";
            this.TBMinutes6.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes6.TabIndex = 4;
            // 
            // BSave
            // 
            this.BSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.BSave.Location = new System.Drawing.Point(381, 350);
            this.BSave.Name = "BSave";
            this.BSave.Size = new System.Drawing.Size(149, 34);
            this.BSave.TabIndex = 5;
            this.BSave.Text = "Save Settings";
            this.BSave.UseVisualStyleBackColor = true;
            this.BSave.Click += new System.EventHandler(this.BSave_Click);
            // 
            // CB2
            // 
            this.CB2.AutoSize = true;
            this.CB2.Enabled = false;
            this.CB2.Location = new System.Drawing.Point(12, 54);
            this.CB2.Name = "CB2";
            this.CB2.Size = new System.Drawing.Size(15, 14);
            this.CB2.TabIndex = 6;
            this.CB2.UseVisualStyleBackColor = true;
            this.CB2.CheckedChanged += new System.EventHandler(this.CB2_CheckedChanged);
            // 
            // CB3
            // 
            this.CB3.AutoSize = true;
            this.CB3.Enabled = false;
            this.CB3.Location = new System.Drawing.Point(12, 80);
            this.CB3.Name = "CB3";
            this.CB3.Size = new System.Drawing.Size(15, 14);
            this.CB3.TabIndex = 6;
            this.CB3.UseVisualStyleBackColor = true;
            this.CB3.CheckedChanged += new System.EventHandler(this.CB3_CheckedChanged_1);
            // 
            // CB4
            // 
            this.CB4.AutoSize = true;
            this.CB4.Enabled = false;
            this.CB4.Location = new System.Drawing.Point(12, 106);
            this.CB4.Name = "CB4";
            this.CB4.Size = new System.Drawing.Size(15, 14);
            this.CB4.TabIndex = 6;
            this.CB4.UseVisualStyleBackColor = true;
            this.CB4.CheckedChanged += new System.EventHandler(this.CB4_CheckedChanged_1);
            // 
            // CB5
            // 
            this.CB5.AutoSize = true;
            this.CB5.Enabled = false;
            this.CB5.Location = new System.Drawing.Point(12, 132);
            this.CB5.Name = "CB5";
            this.CB5.Size = new System.Drawing.Size(15, 14);
            this.CB5.TabIndex = 6;
            this.CB5.UseVisualStyleBackColor = true;
            this.CB5.CheckedChanged += new System.EventHandler(this.CB5_CheckedChanged_1);
            // 
            // CB6
            // 
            this.CB6.AutoSize = true;
            this.CB6.Enabled = false;
            this.CB6.Location = new System.Drawing.Point(12, 158);
            this.CB6.Name = "CB6";
            this.CB6.Size = new System.Drawing.Size(15, 14);
            this.CB6.TabIndex = 6;
            this.CB6.UseVisualStyleBackColor = true;
            this.CB6.CheckedChanged += new System.EventHandler(this.CB6_CheckedChanged_1);
            // 
            // CBStopBot
            // 
            this.CBStopBot.AutoSize = true;
            this.CBStopBot.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CBStopBot.Location = new System.Drawing.Point(12, 350);
            this.CBStopBot.Name = "CBStopBot";
            this.CBStopBot.Size = new System.Drawing.Size(169, 17);
            this.CBStopBot.TabIndex = 7;
            this.CBStopBot.Text = "Stop Bot after last Profile";
            this.CBStopBot.UseVisualStyleBackColor = true;
            // 
            // CB1
            // 
            this.CB1.AutoSize = true;
            this.CB1.Location = new System.Drawing.Point(12, 28);
            this.CB1.Name = "CB1";
            this.CB1.Size = new System.Drawing.Size(15, 14);
            this.CB1.TabIndex = 8;
            this.CB1.UseVisualStyleBackColor = true;
            this.CB1.CheckedChanged += new System.EventHandler(this.CB1_CheckedChanged_1);
            // 
            // BSearchFile6
            // 
            this.BSearchFile6.Location = new System.Drawing.Point(381, 153);
            this.BSearchFile6.Name = "BSearchFile6";
            this.BSearchFile6.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile6.TabIndex = 9;
            this.BSearchFile6.Text = "Search";
            this.BSearchFile6.UseVisualStyleBackColor = true;
            this.BSearchFile6.Click += new System.EventHandler(this.BSearchFile6_Click_1);
            // 
            // TBProfile7
            // 
            this.TBProfile7.Enabled = false;
            this.TBProfile7.Location = new System.Drawing.Point(99, 181);
            this.TBProfile7.Name = "TBProfile7";
            this.TBProfile7.Size = new System.Drawing.Size(276, 20);
            this.TBProfile7.TabIndex = 0;
            // 
            // TBProfile8
            // 
            this.TBProfile8.Enabled = false;
            this.TBProfile8.Location = new System.Drawing.Point(99, 207);
            this.TBProfile8.Name = "TBProfile8";
            this.TBProfile8.Size = new System.Drawing.Size(276, 20);
            this.TBProfile8.TabIndex = 0;
            // 
            // TBProfile9
            // 
            this.TBProfile9.Enabled = false;
            this.TBProfile9.Location = new System.Drawing.Point(99, 233);
            this.TBProfile9.Name = "TBProfile9";
            this.TBProfile9.Size = new System.Drawing.Size(276, 20);
            this.TBProfile9.TabIndex = 0;
            // 
            // TBProfile10
            // 
            this.TBProfile10.Enabled = false;
            this.TBProfile10.Location = new System.Drawing.Point(99, 259);
            this.TBProfile10.Name = "TBProfile10";
            this.TBProfile10.Size = new System.Drawing.Size(276, 20);
            this.TBProfile10.TabIndex = 0;
            // 
            // TBProfile11
            // 
            this.TBProfile11.Enabled = false;
            this.TBProfile11.Location = new System.Drawing.Point(99, 285);
            this.TBProfile11.Name = "TBProfile11";
            this.TBProfile11.Size = new System.Drawing.Size(276, 20);
            this.TBProfile11.TabIndex = 0;
            // 
            // TBProfile12
            // 
            this.TBProfile12.Enabled = false;
            this.TBProfile12.Location = new System.Drawing.Point(99, 311);
            this.TBProfile12.Name = "TBProfile12";
            this.TBProfile12.Size = new System.Drawing.Size(276, 20);
            this.TBProfile12.TabIndex = 0;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(35, 184);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(54, 13);
            this.label10.TabIndex = 1;
            this.label10.Text = "Profile 7";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(35, 210);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(54, 13);
            this.label11.TabIndex = 1;
            this.label11.Text = "Profile 8";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(35, 236);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(54, 13);
            this.label12.TabIndex = 1;
            this.label12.Text = "Profile 9";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(35, 262);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(61, 13);
            this.label13.TabIndex = 1;
            this.label13.Text = "Profile 10";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(35, 288);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(61, 13);
            this.label14.TabIndex = 1;
            this.label14.Text = "Profile 11";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(35, 314);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(61, 13);
            this.label15.TabIndex = 1;
            this.label15.Text = "Profile 12";
            // 
            // TBMinutes7
            // 
            this.TBMinutes7.Enabled = false;
            this.TBMinutes7.Location = new System.Drawing.Point(438, 181);
            this.TBMinutes7.Name = "TBMinutes7";
            this.TBMinutes7.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes7.TabIndex = 4;
            // 
            // TBMinutes8
            // 
            this.TBMinutes8.Enabled = false;
            this.TBMinutes8.Location = new System.Drawing.Point(438, 207);
            this.TBMinutes8.Name = "TBMinutes8";
            this.TBMinutes8.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes8.TabIndex = 4;
            // 
            // TBMinutes9
            // 
            this.TBMinutes9.Enabled = false;
            this.TBMinutes9.Location = new System.Drawing.Point(438, 233);
            this.TBMinutes9.Name = "TBMinutes9";
            this.TBMinutes9.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes9.TabIndex = 4;
            // 
            // TBMinutes10
            // 
            this.TBMinutes10.Enabled = false;
            this.TBMinutes10.Location = new System.Drawing.Point(438, 259);
            this.TBMinutes10.Name = "TBMinutes10";
            this.TBMinutes10.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes10.TabIndex = 4;
            // 
            // TBMinutes11
            // 
            this.TBMinutes11.Enabled = false;
            this.TBMinutes11.Location = new System.Drawing.Point(438, 285);
            this.TBMinutes11.Name = "TBMinutes11";
            this.TBMinutes11.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes11.TabIndex = 4;
            // 
            // TBMinutes12
            // 
            this.TBMinutes12.Enabled = false;
            this.TBMinutes12.Location = new System.Drawing.Point(438, 311);
            this.TBMinutes12.Name = "TBMinutes12";
            this.TBMinutes12.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes12.TabIndex = 4;
            // 
            // BSearchFile7
            // 
            this.BSearchFile7.Location = new System.Drawing.Point(381, 179);
            this.BSearchFile7.Name = "BSearchFile7";
            this.BSearchFile7.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile7.TabIndex = 15;
            this.BSearchFile7.Text = "Search";
            this.BSearchFile7.UseVisualStyleBackColor = true;
            this.BSearchFile7.Click += new System.EventHandler(this.BSearchFile7_Click);
            // 
            // BSearchFile8
            // 
            this.BSearchFile8.Location = new System.Drawing.Point(381, 205);
            this.BSearchFile8.Name = "BSearchFile8";
            this.BSearchFile8.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile8.TabIndex = 16;
            this.BSearchFile8.Text = "Search";
            this.BSearchFile8.UseVisualStyleBackColor = true;
            this.BSearchFile8.Click += new System.EventHandler(this.BSearchFile8_Click);
            // 
            // BSearchFile9
            // 
            this.BSearchFile9.Location = new System.Drawing.Point(381, 231);
            this.BSearchFile9.Name = "BSearchFile9";
            this.BSearchFile9.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile9.TabIndex = 17;
            this.BSearchFile9.Text = "Search";
            this.BSearchFile9.UseVisualStyleBackColor = true;
            this.BSearchFile9.Click += new System.EventHandler(this.BSearchFile9_Click);
            // 
            // BSearchFile10
            // 
            this.BSearchFile10.Location = new System.Drawing.Point(381, 257);
            this.BSearchFile10.Name = "BSearchFile10";
            this.BSearchFile10.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile10.TabIndex = 18;
            this.BSearchFile10.Text = "Search";
            this.BSearchFile10.UseVisualStyleBackColor = true;
            this.BSearchFile10.Click += new System.EventHandler(this.BSearchFile10_Click);
            // 
            // BSearchFile11
            // 
            this.BSearchFile11.Location = new System.Drawing.Point(381, 283);
            this.BSearchFile11.Name = "BSearchFile11";
            this.BSearchFile11.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile11.TabIndex = 19;
            this.BSearchFile11.Text = "Search";
            this.BSearchFile11.UseVisualStyleBackColor = true;
            this.BSearchFile11.Click += new System.EventHandler(this.BSearchFile11_Click);
            // 
            // BSearchFile12
            // 
            this.BSearchFile12.Location = new System.Drawing.Point(381, 309);
            this.BSearchFile12.Name = "BSearchFile12";
            this.BSearchFile12.Size = new System.Drawing.Size(51, 23);
            this.BSearchFile12.TabIndex = 20;
            this.BSearchFile12.Text = "Search";
            this.BSearchFile12.UseVisualStyleBackColor = true;
            this.BSearchFile12.Click += new System.EventHandler(this.BSearchFile12_Click);
            // 
            // CB7
            // 
            this.CB7.AutoSize = true;
            this.CB7.Enabled = false;
            this.CB7.Location = new System.Drawing.Point(12, 184);
            this.CB7.Name = "CB7";
            this.CB7.Size = new System.Drawing.Size(15, 14);
            this.CB7.TabIndex = 21;
            this.CB7.UseVisualStyleBackColor = true;
            this.CB7.CheckedChanged += new System.EventHandler(this.CB7_CheckedChanged);
            // 
            // CB8
            // 
            this.CB8.AutoSize = true;
            this.CB8.Enabled = false;
            this.CB8.Location = new System.Drawing.Point(12, 210);
            this.CB8.Name = "CB8";
            this.CB8.Size = new System.Drawing.Size(15, 14);
            this.CB8.TabIndex = 22;
            this.CB8.UseVisualStyleBackColor = true;
            this.CB8.CheckedChanged += new System.EventHandler(this.CB8_CheckedChanged);
            // 
            // CB9
            // 
            this.CB9.AutoSize = true;
            this.CB9.Enabled = false;
            this.CB9.Location = new System.Drawing.Point(12, 236);
            this.CB9.Name = "CB9";
            this.CB9.Size = new System.Drawing.Size(15, 14);
            this.CB9.TabIndex = 23;
            this.CB9.UseVisualStyleBackColor = true;
            this.CB9.CheckedChanged += new System.EventHandler(this.CB9_CheckedChanged);
            // 
            // CB10
            // 
            this.CB10.AutoSize = true;
            this.CB10.Enabled = false;
            this.CB10.Location = new System.Drawing.Point(12, 262);
            this.CB10.Name = "CB10";
            this.CB10.Size = new System.Drawing.Size(15, 14);
            this.CB10.TabIndex = 24;
            this.CB10.UseVisualStyleBackColor = true;
            this.CB10.CheckedChanged += new System.EventHandler(this.CB10_CheckedChanged);
            // 
            // CB11
            // 
            this.CB11.AutoSize = true;
            this.CB11.Enabled = false;
            this.CB11.Location = new System.Drawing.Point(12, 288);
            this.CB11.Name = "CB11";
            this.CB11.Size = new System.Drawing.Size(15, 14);
            this.CB11.TabIndex = 25;
            this.CB11.UseVisualStyleBackColor = true;
            this.CB11.CheckedChanged += new System.EventHandler(this.CB11_CheckedChanged);
            // 
            // CB12
            // 
            this.CB12.AutoSize = true;
            this.CB12.Enabled = false;
            this.CB12.Location = new System.Drawing.Point(12, 314);
            this.CB12.Name = "CB12";
            this.CB12.Size = new System.Drawing.Size(15, 14);
            this.CB12.TabIndex = 26;
            this.CB12.UseVisualStyleBackColor = true;
            this.CB12.CheckedChanged += new System.EventHandler(this.CB12_CheckedChanged);
            // 
            // TBMinutes1
            // 
            this.TBMinutes1.Enabled = false;
            this.TBMinutes1.Location = new System.Drawing.Point(438, 25);
            this.TBMinutes1.Name = "TBMinutes1";
            this.TBMinutes1.Size = new System.Drawing.Size(110, 20);
            this.TBMinutes1.TabIndex = 4;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(465, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 3;
            this.label8.Text = "Minutes";
            // 
            // ProfileChangerUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(567, 396);
            this.Controls.Add(this.CB12);
            this.Controls.Add(this.CB11);
            this.Controls.Add(this.CB10);
            this.Controls.Add(this.CB9);
            this.Controls.Add(this.CB8);
            this.Controls.Add(this.CB7);
            this.Controls.Add(this.BSearchFile12);
            this.Controls.Add(this.BSearchFile11);
            this.Controls.Add(this.BSearchFile10);
            this.Controls.Add(this.BSearchFile9);
            this.Controls.Add(this.BSearchFile8);
            this.Controls.Add(this.BSearchFile7);
            this.Controls.Add(this.BSearchFile6);
            this.Controls.Add(this.CB1);
            this.Controls.Add(this.CBStopBot);
            this.Controls.Add(this.CB6);
            this.Controls.Add(this.CB5);
            this.Controls.Add(this.CB4);
            this.Controls.Add(this.CB3);
            this.Controls.Add(this.CB2);
            this.Controls.Add(this.BSave);
            this.Controls.Add(this.TBMinutes12);
            this.Controls.Add(this.TBMinutes6);
            this.Controls.Add(this.TBMinutes11);
            this.Controls.Add(this.TBMinutes5);
            this.Controls.Add(this.TBMinutes10);
            this.Controls.Add(this.TBMinutes4);
            this.Controls.Add(this.TBMinutes9);
            this.Controls.Add(this.TBMinutes3);
            this.Controls.Add(this.TBMinutes8);
            this.Controls.Add(this.TBMinutes2);
            this.Controls.Add(this.TBMinutes7);
            this.Controls.Add(this.TBMinutes1);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.BSearchFile5);
            this.Controls.Add(this.BSearchFile4);
            this.Controls.Add(this.BSearchFile3);
            this.Controls.Add(this.BSearchFile2);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.BSearchFile1);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.TBProfile12);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TBProfile11);
            this.Controls.Add(this.TBProfile6);
            this.Controls.Add(this.TBProfile10);
            this.Controls.Add(this.TBProfile5);
            this.Controls.Add(this.TBProfile9);
            this.Controls.Add(this.TBProfile4);
            this.Controls.Add(this.TBProfile8);
            this.Controls.Add(this.TBProfile3);
            this.Controls.Add(this.TBProfile7);
            this.Controls.Add(this.TBProfile2);
            this.Controls.Add(this.TBProfile1);
            this.Name = "ProfileChangerUI";
            this.Text = "Profile Changer - Config Version 2.0";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TBProfile1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox TBProfile2;
        private System.Windows.Forms.TextBox TBProfile3;
        private System.Windows.Forms.TextBox TBProfile4;
        private System.Windows.Forms.TextBox TBProfile5;
        private System.Windows.Forms.TextBox TBProfile6;
        private System.Windows.Forms.Button BSearchFile1;
        private System.Windows.Forms.Button BSearchFile2;
        private System.Windows.Forms.Button BSearchFile3;
        private System.Windows.Forms.Button BSearchFile4;
        private System.Windows.Forms.Button BSearchFile5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox TBMinutes2;
        private System.Windows.Forms.TextBox TBMinutes3;
        private System.Windows.Forms.TextBox TBMinutes4;
        private System.Windows.Forms.TextBox TBMinutes5;
        private System.Windows.Forms.TextBox TBMinutes6;
        private System.Windows.Forms.Button BSave;
        private System.Windows.Forms.CheckBox CB1; 
        private System.Windows.Forms.CheckBox CB2;
        private System.Windows.Forms.CheckBox CB3;
        private System.Windows.Forms.CheckBox CB4;
        private System.Windows.Forms.CheckBox CB5;
        private System.Windows.Forms.CheckBox CB6;
        private System.Windows.Forms.CheckBox CBStopBot;
        private Button BSearchFile6;
        private TextBox TBProfile7;
        private TextBox TBProfile8;
        private TextBox TBProfile9;
        private TextBox TBProfile10;
        private TextBox TBProfile11;
        private TextBox TBProfile12;
        private Label label10;
        private Label label11;
        private Label label12;
        private Label label13;
        private Label label14;
        private Label label15;
        private TextBox TBMinutes7;
        private TextBox TBMinutes8;
        private TextBox TBMinutes9;
        private TextBox TBMinutes10;
        private TextBox TBMinutes11;
        private TextBox TBMinutes12;
        private Button BSearchFile7;
        private Button BSearchFile8;
        private Button BSearchFile9;
        private Button BSearchFile10;
        private Button BSearchFile11;
        private Button BSearchFile12;
        private CheckBox CB7;
        private CheckBox CB8;
        private CheckBox CB9;
        private CheckBox CB10;
        private CheckBox CB11;
        private CheckBox CB12;
        private TextBox TBMinutes1;
        private Label label8;
    }
}

