using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Threading;
using SudokuSolver.Services;
using SudokuSolver.Util;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using SudokuSolver.Classes;

namespace SudokuSolver
{
    
    public partial class Form1 : Form
    {
        AppStatus _stato;
        AppStatus stato
        {
            get { return _stato; }
            set
            {
                _stato = value;
                if (_stato==AppStatus.Waiting)
                {
                    button1.SetTextInvoke("Risolvi");
                    button1.SetEnableInvoke(true);
                    button2.SetEnableInvoke(true);
                    button3.SetEnableInvoke(true);
                    button4.SetEnableInvoke(true);
                    comboBox1.SetEnableInvoke(true);
                    timer1.Stop();
                    Tempo.SetTime(0);

                }
                else if (_stato == AppStatus.Working)
                {
                    button1.SetTextInvoke("Stop");
                    button1.SetEnableInvoke(true);
                    button2.SetEnableInvoke(false);
                    button3.SetEnableInvoke(false);
                    button4.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);

                    label1.SetTextInvoke(Tempo.ToString(@"mm\:ss"));
                    timer1.Start();
                }
                else if (_stato == AppStatus.Stopping)
                {
                    button1.SetEnableInvoke(false);
                    button2.SetEnableInvoke(false);
                    button3.SetEnableInvoke(false);
                    button4.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);

                }
                else if (_stato == AppStatus.Starting)
                {
                    button1.SetEnableInvoke(false);
                    button2.SetEnableInvoke(false);
                    button3.SetEnableInvoke(false);
                    button4.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);

                }
            }
        }
        SudokuSolverService sss;
        int Dim = 16;
        TimeSpanPlus Tempo;
        Thread th;


        public Form1()
        { 
            InitializeComponent();
            stato = AppStatus.Starting;
            Util.Util.Init();

            sss = new SudokuSolverService(Dim);
            SudokuSolverTextBoxManager tm = sudokuPanel1.Genera(Dim);
            tm.SetAllCellAs(NumberType.InsertByUser);
            int i=tm.SetTextBoxIndexs();

            sss.TextBoxManager=tm;
            i++;
            button1.TabIndex = i;
            i++;
            button2.TabIndex = i;
            i++;
            button3.TabIndex = i;
            i++;
            button4.TabIndex = i;

            timer1.Interval = 1000;
            Tempo = new TimeSpanPlus();

            sss.FinishSudoku += Sss_FinishSudoku;

            comboBox1.SelectedIndex=comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviNormaleAsync(); }, "Normale"));
            comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviRicorsioneAsync(); }, "Ricorsione"));
            comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviCombinatoAsync(); }, "Combinato"));
            
            stato = AppStatus.Waiting;
        }

        private void Sss_FinishSudoku(int cod)
        {
            stato = AppStatus.Waiting;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (stato == AppStatus.Waiting)
            {
                stato = AppStatus.Working;
                if (comboBox1.SelectedItem is ObjTypeRisolutore)
                    ((ObjTypeRisolutore)comboBox1.SelectedItem).FuncRisolutore();

            }
            else if (stato == AppStatus.Working)
            {
                stato = AppStatus.Stopping;
                new Thread(() =>
                {
                    sss.Stoppa();
                    stato = AppStatus.Waiting;
                }).Start();
            }
         }
        
        private void button2_Click(object sender, EventArgs e)
        {
            if(stato==AppStatus.Waiting)
            {
                sss.pulisci();
                label1.Text = "00:00";
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "file mappa sudoku | *.fms";
            saveFileDialog1.FileName = "";
            saveFileDialog1.ShowDialog(); 
        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.Multiselect = false;
            openFileDialog1.Filter = "file mappa sudoku | *.fms";
            openFileDialog1.FileName = "";
            openFileDialog1.ShowDialog();
        }

 

        private void timer1_Tick(object sender, EventArgs e)
        {
            Tempo.AddSeconds(1);
            label1.Text = Tempo.ToString(@"mm\:ss");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            new Thread(() =>
            {
                timer1.Stop();
                if (th != null)
                {
                    th.Abort();
                    th.Join();
                }
                    
            }).Start();
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(openFileDialog1.FileName))
                {
                    sss.LoadByFile(openFileDialog1.FileName);
                }
            }
            catch(SudokuSolverException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            sss.WriteOnFile(saveFileDialog1.FileName);
        }
    }


}
