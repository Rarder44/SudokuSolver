using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using SudokuSolver.Services;

namespace SudokuSolver.Controls
{
    public partial class SudokuPanel : UserControl
    {
        public SudokuPanel()
        {
            InitializeComponent();
        }
        public void AddOrizzontalControl(Control c, Control RelativeTo = null,int PaddingLeft=0,int PaddingTop=0)
        {
            int x=0, y = 0;
            if(RelativeTo==null)
            {
                foreach(Control tc in Controls)
                    if (tc.Location.X > x)
                    {
                        x = tc.Location.X+tc.Size.Width;
                        y = tc.Location.Y;
                    }
                x += PaddingLeft;
                y += PaddingTop;
            }
            else
            {
                x = RelativeTo.Location.X+RelativeTo.Size.Width+PaddingLeft;
                y = RelativeTo.Location.Y+PaddingTop;
            }
            c.Location = new Point(x, y);
            Controls.Add(c);
        }
        public void AddVerticalControl(Control c,  Control RelativeTo = null,int PaddingLeft = 0, int PaddingTop = 0)
        {
            int x = 0, y = 0;
            if (RelativeTo == null)
            {
                foreach (Control tc in Controls)
                    if (tc.Location.Y > y)
                    {
                        x = tc.Location.X ;
                        y = tc.Location.Y + tc.Size.Height;
                    }
                x += PaddingLeft;
                y += PaddingTop;
            }
            else
            {
                x = RelativeTo.Location.X + PaddingLeft;
                y = RelativeTo.Location.Y + RelativeTo.Size.Height +PaddingTop;
            }
            c.Location = new Point(x, y);
            Controls.Add(c);
        }

        int _NormalSpace = 0;
        int _PlusSpace = 0;
        int _CellWidth = 22;
        int _CellHeight = 20;
        int NumRow = 3;
        public int NormalSpace
        {
            get { return _NormalSpace; }
            set { _NormalSpace = value; }
        }
        public int PlusSpace
        {
            get { return _PlusSpace; }
            set { _PlusSpace = value; }
        }
        public int CellWidth
        {
            get { return _CellWidth; }
            set { _CellWidth = value; }
        }
        public int CellHeight
        {
            get { return _CellHeight; }
            set { _CellHeight = value; }
        }



        public SudokuSolverTextBoxManager Genera9x9()
        {
            return Genera(9);
        }
        public SudokuSolverTextBoxManager Genera16x16()
        {
            return Genera(16);
        }
        public SudokuSolverTextBoxManager Genera(int Dimensione)
        {
            
            SudokuSolverTextBoxManager sss = new SudokuSolverTextBoxManager();
            TextBoxPlus[] ArrTemp = new TextBoxPlus[Dimensione * Dimensione];
            int ArrTempI = 0;

            double ris = ((double)Dimensione).Sqrt();
            if(!ris.IsInteger())         
                throw new SudokuPanelException("Valore non accettato per la generazione dello schema");

            Controls.Clear();
            NumRow = (int)ris;
            TextBoxPlus preV = null;
            TextBoxPlus preO = null;
            for (int i = 0; i < Dimensione; i++)
            {
                TextBoxPlus t = (TextBoxPlus)new TextBoxPlus().SetSize(_CellWidth, _CellHeight);
                if (preV == null)
                    AddVerticalControl(t, null, 0, 0);
                else
                    AddVerticalControl(t, preV, 0, i % NumRow == 0 ? _PlusSpace : _NormalSpace);

                preO = t;
                preV = t;
                ArrTemp[ArrTempI++] = t;

                for (int j = 1; j < Dimensione; j++)
                {
                    TextBoxPlus tt = (TextBoxPlus)new TextBoxPlus().SetSize(_CellWidth, _CellHeight);
                    AddOrizzontalControl(tt, preO, j % NumRow == 0 ? _PlusSpace : _NormalSpace, 0);
                    preO = tt;
                    ArrTemp[ArrTempI++] = tt;
                }
            }

          
            sss.SetArrayTextBox(ArrTemp);
            return sss;
        }

        public Size GetTheoreticalSize()
        {
            int MaxY = 0, MaxX = 0, cc = 0;
            foreach (Control c in Controls)
            {
                cc++;
                if (c.Location.X + c.Size.Width > MaxX)
                    MaxX = c.Location.X + c.Size.Width;
                if (c.Location.Y + c.Size.Height > MaxY)
                    MaxY = c.Location.Y + c.Size.Height;
            }
            return new Size(MaxX, MaxY);
        }

        public void ResizePanelFromControls()
        {
            Size = GetTheoreticalSize();
        }
    }
}
