using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
using SudokuSolver.Util;
using System.Threading;

namespace SudokuSolver.Services
{
    
    public class SudokuSolverService
    {

        #region Variabili
        SudokuSolverTextBoxManager mngTxb =null;
        public SudokuSolverTextBoxManager TextBoxManager
        {
            get { return mngTxb; }
            set
            {
                if(mngMat!=null)
                    if(mngMat.Dim!=value.Dim) 
                        throw new SudokuSolverException("La matrice numerica non è stata inizializzata");
                mngTxb = value;

            }
        }
        SudokuSolverMatriceManager mngMat = null;
        Thread Risolutore=null;
        #endregion

        #region Eventi
        public delegate void FinishSudokuDel(int cod);
        public event FinishSudokuDel FinishSudoku;
        public delegate void NeedResizeDel(int Dim);
        public event NeedResizeDel NeedTextBoxMatriceResize;
        #endregion 

        #region Costruttori
        public SudokuSolverService(int Dim)
        {
            mngMat = new SudokuSolverMatriceManager(Dim);
        }
        public SudokuSolverService(SudokuSolverService sss)
        {
            mngMat = sss.mngMat.Clone();
            mngTxb = sss.mngTxb;
        }
        public SudokuSolverService clone()
        {
            return new SudokuSolverService(this);
        }
        #endregion

        #region Risolutore      
        public void RisolviNormale()
        {
            TextBoxToMatrice();
            mngTxb.SetFreeCellAs(NumberType.InsertByProgram);

            bool controllo = true;
            while (controllo)
            {
                controllo = false;
                for (int i = 1; i <= mngMat.Dim; i++)
                {
                    mngMat.PulisciCodErr();
                    for (int x = 0; x < mngMat.Dim; x++)
                        for (int y = 0; y < mngMat.Dim; y++)
                        {
                            if (GetMatriceNumber(x, y) == i)
                                SettaRCQ(x, y);
                        }
                    incrocio();
                    if (controlloRCQ(i))
                        controllo = true;
                }
            }
            mngMat.PulisciCodErr();
        }
        public void RisolviRicorsione()
        {
            TextBoxToMatrice();
            mngTxb.SetFreeCellAs(NumberType.InsertByProgram);

            funzione_ricorsiva(clone());
        }
        public void RisolviCombinato()
        {
            TextBoxToMatrice();
            mngTxb.SetFreeCellAs(NumberType.InsertByProgram);

            bool controllo = true;
            while (controllo)
            {
                controllo = false;
                for (int i = 1; i <= mngMat.Dim; i++)
                {
                    mngMat.PulisciCodErr();
                    for (int x = 0; x < mngMat.Dim; x++)
                        for (int y = 0; y < mngMat.Dim; y++)
                        {
                            if (GetMatriceNumber(x, y) == i)
                                SettaRCQ(x, y);
                        }
                    incrocio();
                    if (controlloRCQ(i))
                        controllo = true;
                }
            }
            mngMat.PulisciCodErr();

            if(!completo())
                funzione_ricorsiva(clone());

        }

        public void Stoppa()
        {
            try
            {
                if (Risolutore != null && Risolutore.IsAlive)
                {
                    Risolutore.Abort();
                    Risolutore.Join();
                }
            }
            catch (Exception ex)
            { }
        }

        public void RisolviNormaleAsync()
        {
            if (Risolutore != null && Risolutore.IsAlive)
            {
                return;
            }
            new Thread(() =>
            {
                RisolviNormale();
                if (FinishSudoku != null)
                    FinishSudoku(1);
            }).Start();
        }
        public void RisolviRicorsioneAsync()
        {
            if (Risolutore != null && Risolutore.IsAlive)
            {
                return;
            }
            new Thread(() =>
            {
                RisolviRicorsione();
                if (FinishSudoku != null)
                    FinishSudoku(1);
            }).Start();
        }
        public void RisolviCombinatoAsync()
        {
            if (Risolutore != null && Risolutore.IsAlive)
            {
                return;
            }
            new Thread(() =>
            {
                RisolviCombinato();
                if (FinishSudoku != null)
                    FinishSudoku(1);
            }).Start();
        }

        public void StoppaAsync()
        {
            new Thread(() =>
            {
                Stoppa();
            }).Start();
        }
        


        bool controlloRCQ(int Num)
        {
            bool c = false;
            for (int x = 0; x < mngMat.Dim; x++)
            {
                int xt = 0, yt = 0, n = 0;
                for (int y = 0; y < mngMat.Dim; y++)
                {
                    if (GetMatriceNumber(x, y) == 0)
                    {
                        n++;
                        xt = x;
                        yt = y;
                    }
                }
                if (n == 1)
                {
                    SetMatriceNumber(xt, yt, Num, NumberType.InsertByProgram);
                    SetTextBoxNumber(xt, yt, Num, NumberType.InsertByProgram);
                    SettaRCQ(xt, yt);
                    c = true;
                }
            }




            for (int x = 0; x < mngMat.Dim; x++)
            {
                int xt = 0, yt = 0, n = 0;
                for (int y = 0; y < mngMat.Dim; y++)
                {
                    if (GetMatriceNumber(y, x) == 0)
                    {
                        n++;
                        xt = x;
                        yt = y;
                    }
                }
                if (n == 1)
                {
                    SetMatriceNumber(yt, xt, Num, NumberType.InsertByProgram);
                    SetTextBoxNumber(yt, xt, Num, NumberType.InsertByProgram);
                    SettaRCQ(yt, xt);
                    c = true;
                }
            }


            int DimQ =(int) mngMat.Dim.Sqrt();
            for (int qx = 0; qx < DimQ; qx++)
                for (int qy = 0; qy < DimQ; qy++)
                {
                    int xt = 0, yt = 0, n = 0;
                    for (int xx = qx * DimQ; xx < (qx * DimQ) + DimQ; xx++)
                        for (int yy = qy * DimQ; yy < (qy * DimQ) + DimQ; yy++)
                            if (GetMatriceNumber(xx, yy) == 0)
                            {
                                n++;
                                xt = xx;
                                yt = yy;
                            }
                    if (n == 1)
                    {
                        SetMatriceNumber(xt, yt, Num, NumberType.InsertByProgram);
                        SetTextBoxNumber(xt, yt, Num, NumberType.InsertByProgram);
                        SettaRCQ(xt, yt);
                        c = true;
                    }
                }

            return c;
        }

        void SettaRCQ(int x, int y)
        {
            SettaR( x,  y);
            SettaC(x, y);
            SettaQ(x, y);
        }
        void SettaR(int x, int y)
        {
            for (int i = 0; i < mngMat.Dim; i++)
                if (GetMatriceNumber(i, y) < 1)
                    SetMatriceNumber(i, y, -2);
        }
        void SettaC(int x, int y)
        {
            for (int i = 0; i < mngMat.Dim; i++)
                if (GetMatriceNumber(x, i) < 1)
                    SetMatriceNumber(x, i, -2);
        }
        void SettaQ(int x, int y)
        {
            int DimQ = (int)mngMat.Dim.Sqrt();
            int xt = x / DimQ;
            int yt = y / DimQ;
            xt *= DimQ;
            yt *= DimQ;
            for (int xx = xt; xx < xt + DimQ; xx++)
                for (int yy = yt; yy < yt + DimQ; yy++)
                {
                    if (GetMatriceNumber(xx, yy) < 1)
                        SetMatriceNumber(xx, yy, -2);
                }


        }
        bool InFilaColonna(Dictionary<int, Tuple<int, int>> l)
        {
            int xtemp = -1;
            foreach(KeyValuePair<int, Tuple<int, int>> t in l)
            {
                if (xtemp == -1)
                    xtemp = t.Value.Item1;
                else if (xtemp != t.Value.Item1)
                    return false;
            }
            return true;
        }
        bool InFilaRiga(Dictionary<int, Tuple<int, int>> l)
        {
            int ytemp = -1;
            foreach (KeyValuePair<int, Tuple<int, int>> t in l)
            {
                if (ytemp == -1)
                    ytemp = t.Value.Item2;
                else if (ytemp != t.Value.Item2)
                    return false;
            }
            return true;
        }
        
        void incrocio()
        {
            int DimQ = (int)mngMat.Dim.Sqrt();
            for (int c = 0; c < mngMat.Dim; c = c + DimQ)
                for (int r = 0; r < mngMat.Dim; r = r + DimQ)
                {
                    Dictionary<int, Tuple<int, int>> mtemp = new Dictionary<int, Tuple<int, int>>();
                    for (int i = c; i < (c + DimQ); i++)
                        for (int j = r; j < (r + DimQ); j++)
                            if (GetMatriceNumber(i, j) == 0)
                                mtemp.Add(mtemp.Count, new Tuple<int, int>(i, j));
                            

                    if(mtemp.Count>0 && mtemp.Count<=DimQ)
                        if(InFilaRiga(mtemp))
                        {
                            SettaR(mtemp[0].Item1, mtemp[0].Item2);
                            foreach(KeyValuePair<int, Tuple<int, int>> k in mtemp)
                                SetMatriceNumber(k.Value.Item1, k.Value.Item2, 0);  
                        }
                        else if (InFilaColonna(mtemp))
                        {
                            SettaC(mtemp[0].Item1, mtemp[0].Item2);
                            foreach (KeyValuePair<int, Tuple<int, int>> k in mtemp)
                                SetMatriceNumber(k.Value.Item1, k.Value.Item2, 0);
                        } 
                }
        }

        bool funzione_ricorsiva(SudokuSolverService m)
        {
            Tuple<int,int> v = m.PosizioneMigliore();
            if (v.Item1 == -1)
                return true;
            int x = v.Item1;
            int y = v.Item2;

            for (int i = 1; i <= mngMat.Dim; i++)
                if (m.puo_starci(x, y, i))
                {
                    m.SetNumber(x, y, i);
                    SetTextBoxNumber(x, y, i, NumberType.InsertByProgram);
                    SudokuSolverService sss = m.clone();
                    if (funzione_ricorsiva(sss))
                        return true;
                }
            m.SetNumber(x, y, 0);
            SetTextBoxNumber(x, y,0, NumberType.InsertByProgram);
            return false;
        }
     
        Tuple<int,int> PosizioneMigliore()
        {
            int DimQ = (int)mngMat.Dim.Sqrt();
            int[][] mat = new int[mngMat.Dim][];
            for (int i = 0; i < mngMat.Dim; i++)
            {
                mat[i] = new int[mngMat.Dim];
            }

            for (int xt = 0; xt < mngMat.Dim; xt++)
                for (int yt = 0; yt < mngMat.Dim; yt++)
                {
                    int nm = GetMatriceNumber(xt, yt);
                    mat[xt][yt] = nm <= 0?0:-1;
                }

            for (int xt = 0; xt < mngMat.Dim; xt++)
                for (int yt = 0; yt < mngMat.Dim; yt++)
                {
                    if (mat[xt][yt] == -1)
                    {
                        for (int i = 0; i < mngMat.Dim; i++)
                        {
                            if (mat[xt][i] != -1)
                                mat[xt][i]++;

                            if (mat[i][yt] != -1)
                                mat[i][yt]++;
                        }
                        int xtt = xt / DimQ;
                        int ytt = yt / DimQ;
                        xtt *= DimQ;
                        ytt *= DimQ;
                        for (int xx = xtt; xx < xtt + DimQ; xx++)
                            for (int yy = ytt; yy < ytt + DimQ; yy++)
                            {
                                if (mat[xx][yy] != -1)
                                    mat[xx][yy]++;
                            }
                    }
                }

            int n = 0;
            Tuple<int, int> v = null;
            for (int xt = 0; xt < mngMat.Dim; xt++)
                for (int yt = 0; yt < mngMat.Dim; yt++)
                {
                    if (mat[xt][yt] >= n)
                    {
                        n = mat[xt][yt];
                        v = new Tuple<int, int>(xt,yt);
                    }
                }
            if(v==null)
               v  = new Tuple<int, int>(-1,0);

            return v;
        }
        bool puo_starci(int x, int y, int num)
        {
            for (int i = 0; i < mngMat.Dim; i++)
                if (GetMatriceNumber(x, i) == num)
                    return false;
                else if (GetMatriceNumber(i, y) == num)
                    return false;
            

            int DimQ = (int)mngMat.Dim.Sqrt();
            int xt = x / DimQ;
            int yt = y / DimQ;
            xt *= DimQ;
            yt *= DimQ;
            for (int xx = xt; xx < xt + DimQ; xx++)
                for (int yy = yt; yy < yt + DimQ; yy++)
                    if (GetMatriceNumber(xx, yy) == num)
                        return false;
            return true;
        }

        bool completo()
        {
            for (int x = 0; x < mngMat.Dim; x++)
                for (int y = 0; y < mngMat.Dim; y++)
                {
                    if (GetMatriceNumber(x, y) == 0)
                        return false;
                }
            return true;
        }
        
        
        
        #endregion

        #region Operazioni All Matrici Managers
        public void pulisci()
        {
            if(mngTxb!= null)
                mngTxb.pulisci();
            if(mngMat!=null)
                mngMat.pulisci();
        }
        public bool CheckManagersDim()
        {
            if (mngTxb != null && mngMat != null)
            {
                return mngMat.Dim == mngTxb.Dim;
            }
            else if (mngTxb == null && mngMat != null)
            {
                return true;
            }
            else
            {
                throw new SudokuSolverException("Sudoku non inizializzato correttamente");
            }

        }
        public int GetDim()
        {
            if (mngTxb==null && mngMat==null)
            {
                throw new SudokuSolverException("Sudoku non inizializzato");
            }
            else if (mngTxb == null )
            {
                return mngMat.Dim;
            }
            else if (CheckManagersDim())
            {
                return mngMat.Dim;
            }
            else
                throw new SudokuSolverException("Le dimensione dei manager non corrispondono");
        }

        public void MatriceToTextBox()
        {
            if (mngMat != null && mngTxb != null && CheckManagersDim())
            {
                for (int i = 0; i < mngMat.Dim; i++)
                    for (int j = 0; j < mngMat.Dim; j++)
                    {
                        mngTxb.SetNumber(i, j, mngMat.GetNumber(i, j), mngMat.GetSudokuNumber(i, j).Type);
                    }

            }
        }
        public void TextBoxToMatrice()
        {
            if (mngMat != null && mngTxb != null && CheckManagersDim())
            {
                for (int i = 0; i < mngMat.Dim; i++)
                    for (int j = 0; j < mngMat.Dim; j++)
                        mngMat.SetNumber(i, j, mngTxb.GetNumber(i, j), mngTxb.GetNumberType(i, j));
            }
        }
        public void LoadByFile(String FileName)
        {
            pulisci();
            int d=mngMat.Deserialize(System.IO.File.ReadAllText(FileName));
            if(d==mngTxb.Dim)
                MatriceToTextBox();
            else
            {
                if (NeedTextBoxMatriceResize != null)
                {
                    while(!CheckManagersDim())
                        NeedTextBoxMatriceResize(d);
                    MatriceToTextBox();
                }
                    
            }
        }
        public void WriteOnFile(String FileName)
        {
            TextBoxToMatrice();
            System.IO.File.WriteAllText(FileName, mngMat.Serialize());
        }

        #endregion

        #region Set Get TextBox
        public int GetTextBoxNumber(int x, int y)
        {
            return mngTxb.GetNumber(x, y);
        }
        public int GetMatriceNumber(int x, int y)
        {
            return mngMat.GetNumber(x, y);
        }

        public void SetMatriceNumber(int x, int y, int Num, NumberType Type = NumberType.InsertByUser)
        {
            mngMat.SetNumber(x, y, Num, Type);
            //SetTextBoxNumber(x, y, Num, Type);
        }
        public void SetTextBoxNumber(int x, int y, int Num, NumberType Type = NumberType.InsertByUser)
        {
            mngTxb.SetNumber(x, y, Num, Type);
        }
        public void SetNumber(int x, int y, int Num, NumberType Type = NumberType.InsertByUser)
        {
            mngMat.SetNumber(x, y, Num, Type);
        }

        #endregion


    }
}
