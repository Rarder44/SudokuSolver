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
                    button5.SetEnableInvoke(true);
                    textBox1.SetEnableInvoke(true);
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
                    button5.SetEnableInvoke(false);
                    textBox1.SetEnableInvoke(false);
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
                    button5.SetEnableInvoke(false);
                    textBox1.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);

                }
                else if (_stato == AppStatus.Starting)
                {
                    button1.SetEnableInvoke(false);
                    button2.SetEnableInvoke(false);
                    button3.SetEnableInvoke(false);
                    button4.SetEnableInvoke(false);
                    button5.SetEnableInvoke(false);
                    textBox1.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);
                    label1.SetTextInvoke("00:00");
                }
                else if (_stato == AppStatus.ModifyingGrid)
                {
                    button1.SetEnableInvoke(false);
                    button2.SetEnableInvoke(false);
                    button3.SetEnableInvoke(false);
                    button4.SetEnableInvoke(false);
                    button5.SetEnableInvoke(false);
                    textBox1.SetEnableInvoke(false);
                    comboBox1.SetEnableInvoke(false);
                }
            }
        }
        SudokuSolverService sss;
        int DimIniziale = 16;
        TimeSpanPlus Tempo;


        public Form1()
        { 
            InitializeComponent();
            stato = AppStatus.Starting;
            Util.Util.Init();

            InitGrid(DimIniziale);


            stato = AppStatus.Waiting;
        }
        public void InitGrid(int Dim,bool NewSudokuSolverService=true)
        {
            if (stato == AppStatus.Starting || stato == AppStatus.Waiting)
            {
                AppStatus old = stato;
                stato = AppStatus.ModifyingGrid;
                if (NewSudokuSolverService)
                {
                    sss = new SudokuSolverService(Dim);
                    sss.FinishSudoku += Sss_FinishSudoku;
                    sss.NeedTextBoxMatriceResize += Sss_NeedTextBoxMatriceResize;
                }

                try
                {
                    SudokuSolverTextBoxManager tm = sudokuPanel1.Genera(Dim);
                    ResizeForm();

                    tm.SetAllCellAs(NumberType.InsertByUser);
                    int i = tm.SetTextBoxIndexs();

                    sss.TextBoxManager = tm;
                    i++;
                    button1.TabIndex = i;
                    i++;
                    comboBox1.TabIndex = i;
                    comboBox1.Items.Clear();
                    comboBox1.SelectedIndex = comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviNormaleAsync(); }, "Normale"));
                    comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviRicorsioneAsync(); }, "Ricorsione"));
                    comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviRicorsioneAsync(false); }, "Ricorsione no step"));
                    comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviCombinatoAsync(); }, "Combinato"));
                    comboBox1.Items.Add(new ObjTypeRisolutore(() => { sss.RisolviCombinatoAsync(false); }, "Combinato no step"));
                    i++;
                    button2.TabIndex = i;
                    i++;
                    button3.TabIndex = i;
                    i++;
                    button4.TabIndex = i;

                    timer1.Interval = 1000;
                    Tempo = new TimeSpanPlus();
                }
                catch (SudokuSolverException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (SudokuPanelException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }             
                
                stato = old;
            }
        }

        private void Sss_NeedTextBoxMatriceResize(int Dim)
        {
            InitGrid(Dim, false);
        }

        public void ResizeForm()
        {
            Size t = sudokuPanel1.GetTheoreticalSize();
            int Width = sudokuPanel1.Location.X + t.Width + panel1.Location.X - (sudokuPanel1.Location.X + sudokuPanel1.Size.Width) + panel1.Width + Size.Width - (panel1.Location.X + panel1.Size.Width);
            int Height = sudokuPanel1.Location.Y + t.Height + Size.Height - (sudokuPanel1.Location.Y + sudokuPanel1.Size.Height);
            int HeightPanel = panel1.Location.Y + panel1.Size.Height;

            MinimumSize = new Size(Width, Height > HeightPanel ? Height : HeightPanel);
            Size = MinimumSize;
            

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
            sss.StoppaAsync();
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

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            int n;
            if (!int.TryParse(sender.Cast<TextBox>().Text, out n))
                sender.Cast<TextBox>().Text = "";
            
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                e.SuppressKeyPress = false;
            else
            {
                e.SuppressKeyPress = true;
                try
                {
                    KeysConverter kc = new KeysConverter();
                    string s = kc.ConvertToString(e.KeyCode).Replace("NumPad", "");
                    if ((sender.Cast<TextBox>().Text + s).IsInt())          
                        sender.Cast<TextBox>().SelectedText = s;  
                }
                catch (Exception ex)
                {
                    sender.Cast<TextBox>().Text = "";
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            
            if (textBox1.Text.IsInt())
                InitGrid(textBox1.Text.ParseInt());

            else
                MessageBox.Show("Dimensione non valida");
        }
    }


}
