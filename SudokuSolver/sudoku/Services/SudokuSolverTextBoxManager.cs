﻿using System;
using System.Windows.Forms;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using SudokuSolver.Util;

namespace SudokuSolver.Services
{
    public class SudokuSolverTextBoxManager
    {
        #region Veriabili
        TextBoxPlus[] vett;
        int _Dim;
        public int Dim
        {
            get
            {
                return _Dim;
            }
        }
        #endregion

        #region Costruttori
        public SudokuSolverTextBoxManager()
        {
        }
        #endregion

        #region Get Set
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

        private Tuple<int, int> GetPos(TextBoxPlus t)
        {
            int x = -1;
            for (int i = 0; i < vett.Length; i++)
                if (vett[i] == t)
                    x = i;

            int y = x % _Dim;
            x = x / _Dim;
            return new Tuple<int, int>(x, y);
        }
        #endregion

        #region Operazioni TextBox
        public void SetArrayTextBox(TextBoxPlus[] vett)
        {
            double l = vett.Length.Sqrt();
            if (l.IsInteger() && l.Sqrt().IsInteger())
            {
                _Dim = (int)l;
                this.vett = vett;
                foreach (TextBoxPlus p in this.vett)
                {
                    p.KeyDown += (object sender, KeyEventArgs e) => {

                        if (GlobalVar.Stato != AppStatus.Waiting)
                        {
                            e.SuppressKeyPress = true;
                            return;
                        }
                        else if(!(e.KeyCode==Keys.Delete || e.KeyCode==Keys.Back))
                                e.SuppressKeyPress = true;


                        sender.Cast<TextBoxPlus>().Type = NumberType.InsertByUser;
                        try
                        {
                            if (e.KeyCode == Keys.Up)
                            {
                                Tuple<int, int> t = GetPos(sender.Cast<TextBoxPlus>());
                                if (t.Item1 > 0)
                                    GetTextBox(t.Item1 - 1, t.Item2).Select();
                                return;
                            }
                            else if (e.KeyCode == Keys.Down)
                            {
                                Tuple<int, int> t = GetPos(sender.Cast<TextBoxPlus>());
                                if (t.Item1 < _Dim - 1)
                                    GetTextBox(t.Item1 + 1, t.Item2).Select();
                                return;
                            }
                            else if (e.KeyCode == Keys.Right)
                            {
                                Tuple<int, int> t = GetPos(sender.Cast<TextBoxPlus>());
                                if (t.Item2 < _Dim - 1)
                                    GetTextBox(t.Item1, t.Item2 + 1).Select();
                                return;
                            }
                            else if (e.KeyCode == Keys.Left)
                            {
                                Tuple<int, int> t = GetPos(sender.Cast<TextBoxPlus>());
                                if (t.Item2 > 0)
                                    GetTextBox(t.Item1, t.Item2 - 1).Select();
                                return;
                            }

                            KeysConverter kc = new KeysConverter();
                            string s = kc.ConvertToString(e.KeyCode).Replace("NumPad", "");
                            string complete = sender.Cast<TextBoxPlus>().Text.Substring(0, sender.Cast<TextBoxPlus>().SelectionStart) + s + sender.Cast<TextBoxPlus>().Text.Substring(sender.Cast<TextBoxPlus>().SelectionStart+ sender.Cast<TextBoxPlus>().SelectionLength );

                            if ( IsCorrectString(s))
                            {
                                if(!IsCorrectString(complete))
                                    sender.Cast<TextBoxPlus>().SelectAll();

                                sender.Cast<TextBoxPlus>().SelectedText = GetCorrectString(s);
                            }

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
        public int SetTextBoxIndexs(int startIndex = 0)
        {
            int i = 0;
            for (; i < vett.Length; i++)
                vett[i].TabIndex = i;
            return i;
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
        #endregion

        #region Util
        public static string GetCorrectString(string s,int Dim)
        {
            s = s.Trim();
            try
            {
                int n = int.Parse(s);
                if (n < 1 || n > Dim)
                    return "";
            }
            catch (Exception e)
            {
                return "";
            }
            return s;
        }
        public string GetCorrectString(string s)
        {
            return GetCorrectString(s, _Dim);
        }
        public static bool IsCorrectString(string s, int Dim)
        {
            string ss = s.Trim();
            try
            {
                int n = int.Parse(ss);
                if (n < 1 || n > Dim)
                    return false;
                else
                    return ss == s;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public bool IsCorrectString(string s)
        {
            return IsCorrectString(s, _Dim);
        }
        public void CorrectTextBox(int x,int y)
        {
            GetTextBox(x, y).SetTextInvoke(GetCorrectString(GetTextBox(x, y).Text));
        }
        #endregion
    }
}
