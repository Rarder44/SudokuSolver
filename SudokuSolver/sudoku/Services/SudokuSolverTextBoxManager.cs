using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using System.Drawing;
using SudokuSolver.Util;

namespace SudokuSolver.Services
{
    public class SudokuSolverTextBoxManager
    {
        TextBoxPlus[] vett;
        int _Dim;
        public int Dim
        {
            get
            {
                return _Dim;
            }
            /*set
            {
                _Dim = value;
            }*/
        }
       
        public SudokuSolverTextBoxManager()
        {
        }
        public int GetNumber(int x, int y)
        {
            try
            {
                if (GetTextBox(x,y).Text.Length == 0)
                    return 0;
                else
                    return int.Parse(GetTextBox(x, y).Text);
            }
            catch (Exception ex)
            {
                throw new SudokuSolverException("Errore durante il parse del numero");
            }
        }
        public NumberType GetNumberType(int x,int y)
        {
            return GetTextBox(x,y).Type;
        }
        private TextBoxPlus GetTextBox(int x,int y)
        {
            return x * _Dim + y < vett.Length ? vett[x * _Dim + y] : null;
        }
        public void SetNumber(int x, int y, int Num,NumberType Type)
        {
            GetTextBox(x, y).SetTextInvoke(GetCorrectString(Num.ToString()));
            GetTextBox(x, y).Type =Type;
        }
        public void SetNumber(int x, int y, int Num)
        {
            GetTextBox(x, y).Text = GetCorrectString(Num.ToString());
        }

        public int SetTextBoxIndexs(int startIndex = 0)
        {
            int i = 0;
            for (; i < vett.Length; i++)
                vett[i].TabIndex = i;
            return i;
        }
        public void SetArrayTextBox(TextBoxPlus[] vett)
        {
            double l = vett.Length.Sqrt();
            if (l.IsInteger() && l.Sqrt().IsInteger())
            {
                _Dim = (int)l;
                this.vett = vett;
                foreach(TextBoxPlus p in this.vett)
                {              
                    p.KeyDown += (object sender, KeyEventArgs e) => {
                        e.SuppressKeyPress = true;
                        try
                        {
                            if (e.KeyCode == Keys.Up) 
                            {
                                Tuple<int,int> t= GetPos((TextBoxPlus)sender);
                                if(t.Item1>0)
                                    GetTextBox(t.Item1-1, t.Item2).Select();
                                return;
                            }
                            else if(e.KeyCode == Keys.Down)
                            {
                                Tuple<int, int> t = GetPos((TextBoxPlus)sender);
                                if (t.Item1 < _Dim-1)
                                    GetTextBox(t.Item1+1, t.Item2).Select();
                                return;
                            }
                            else if( e.KeyCode == Keys.Right)
                            {
                                Tuple<int, int> t = GetPos((TextBoxPlus)sender);
                                if (t.Item2 < _Dim-1)
                                    GetTextBox(t.Item1, t.Item2+1 ).Select();
                                return;
                            }
                            else if(e.KeyCode == Keys.Left)
                            {
                                Tuple<int, int> t = GetPos((TextBoxPlus)sender);
                                if (t.Item2 >0)
                                    GetTextBox(t.Item1, t.Item2-1).Select();
                                return;
                            }

                            KeysConverter kc = new KeysConverter();
                            string s= kc.ConvertToString(e.KeyCode);
                            ((TextBox)sender).Text=GetCorrectString(((TextBox)sender).Text + s);
                            ((TextBox)sender).Select(((TextBox)sender).Text.Length, ((TextBox)sender).Text.Length); 
                        }
                        catch (Exception ex)
                        {
                            ((TextBox)sender).Text = "";
                        }
                    };
                }
            }
            else
                throw new SudokuSolverException("dimesione dell vettore/matrice di TextBox non corretta (deve corrispondere al numero di campi di un sudoku valido)");
        }
        public void pulisci()
        {
            for (int i = 0; i < vett.Length; i++)
                vett[i].SetTextInvoke("");
        }
        public void SetFreeCellAs(NumberType Type)
        {
            for(int i=0;i<vett.Length;i++)
            {
                string ss = vett[i].Text.Trim();
                if (vett[i].Text.Trim() == "")
                    vett[i].Type = Type;
            }
        }
        public void SetAllCellAs(NumberType Type)
        {
            for (int i = 0; i < vett.Length; i++)
            {
                vett[i].Type = Type;
            }
        }
        
        private Tuple<int,int> GetPos(TextBoxPlus t)
        {
            int x = -1;
            for (int i = 0; i < vett.Length; i++)
                if (vett[i] == t)
                    x = i;

            int y = x % _Dim;
            x = x / _Dim;
            return new Tuple<int, int>(x, y);
        }
        public string GetCorrectString(string s)
        {
            s = s.Trim();
            try
            {
                int n = int.Parse(s);
                if (n < 1 || n > _Dim)
                    return "";
            }
            catch(Exception e)
            {
                return "";
            }
            return s;
        }

        public void CheckTextBox(int x,int y)
        {
            GetTextBox(x, y).SetTextInvoke(GetCorrectString(GetTextBox(x, y).Text));
        }
    }
}
