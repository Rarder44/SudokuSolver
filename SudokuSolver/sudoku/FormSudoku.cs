using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using SudokuSolver.Services;
using SudokuSolver.Util;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using SudokuSolver.Classes;

namespace SudokuSolver
{

    public partial class FormSudoku : Form
    {
        SudokuSolverService sss;
        int DimIniziale = 16;
        TimeSpanPlus Tempo;


        public FormSudoku()
        { 
            InitializeComponent();
            GlobalVar.AppStatusChange += GlobalVar_AppStatusChange;

            GlobalVar.Stato = AppStatus.Starting;
            Util.Util.Init();
            InitGrid(DimIniziale);


            GlobalVar.Stato = AppStatus.Waiting;
        }
        public void InitGrid(int Dim, bool NewSudokuSolverService = true)
        {
            if (GlobalVar.Stato == AppStatus.Starting || GlobalVar.Stato == AppStatus.Waiting)
            {
                AppStatus old = GlobalVar.Stato;
                GlobalVar.Stato = AppStatus.ModifyingGrid;
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

                GlobalVar.Stato = old;
            }
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

        private void GlobalVar_AppStatusChange(AppStatus Old, AppStatus New)
        {
            if (New == AppStatus.Waiting)
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
            else if (New == AppStatus.Working)
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
            else if (New == AppStatus.Stopping)
            {
                button1.SetEnableInvoke(false);
                button2.SetEnableInvoke(false);
                button3.SetEnableInvoke(false);
                button4.SetEnableInvoke(false);
                button5.SetEnableInvoke(false);
                textBox1.SetEnableInvoke(false);
                comboBox1.SetEnableInvoke(false);

            }
            else if (New == AppStatus.Starting)
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
            else if (New == AppStatus.ModifyingGrid)
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
        private void Sss_NeedTextBoxMatriceResize(int Dim)
        {
            InitGrid(Dim, false);
        }
        private void Sss_FinishSudoku(int cod)
        {
            GlobalVar.Stato = AppStatus.Waiting;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (GlobalVar.Stato == AppStatus.Waiting)
            {
                GlobalVar.Stato = AppStatus.Working;
                if (comboBox1.SelectedItem is ObjTypeRisolutore)
                    ((ObjTypeRisolutore)comboBox1.SelectedItem).FuncRisolutore();

            }
            else if (GlobalVar.Stato == AppStatus.Working)
            {
                GlobalVar.Stato = AppStatus.Stopping;
                new Thread(() =>
                {
                    sss.Stoppa();
                    GlobalVar.Stato = AppStatus.Waiting;
                }).Start();
            }
         }      
        private void button2_Click(object sender, EventArgs e)
        {
            if(GlobalVar.Stato == AppStatus.Waiting)
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

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            try
            {
                if (System.IO.File.Exists(openFileDialog1.FileName))
                {
                    sss.LoadByFile(openFileDialog1.FileName);
                }
            }
            catch (SudokuSolverException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            sss.WriteOnFile(saveFileDialog1.FileName);
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

        private void timer1_Tick(object sender, EventArgs e)
        {
            Tempo.AddSeconds(1);
            label1.Text = Tempo.ToString(@"mm\:ss");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            sss.StoppaAsync();
        }

    }


}
