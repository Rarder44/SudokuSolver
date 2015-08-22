using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SudokuSolver.Util;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
namespace SudokuSolver.Services
{
    public class SudokuSolverMatriceManager
    {
        SudokuNumber[,] matrice = null;
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

        public SudokuSolverMatriceManager(SudokuSolverMatriceManager mng)
        {
            _Dim = mng.Dim;
            matrice = new SudokuNumber[Dim, Dim];
            for(int i=0;i<mng._Dim;i++)
                for (int j = 0; j < mng._Dim; j++)
                    SetNumber(i, j, mng.GetNumber(i, j));
        }
        public SudokuSolverMatriceManager(int Dim)
        {
            _Dim = Dim;
            matrice = new SudokuNumber[Dim, Dim];
            pulisci();
        }
        public int GetNumber(int x, int y)
        {
            return matrice[x, y].n;
        }
        public SudokuNumber GetSudokuNumber(int x,int y)
        {
            return matrice[x, y];
        }
        public void SetNumber(int x, int y, int Num,NumberType Type=NumberType.InsertByUser)
        {
            if (matrice[x, y] == null)
                matrice[x, y] = new SudokuNumber(Num,Type);
            else
            {
                matrice[x, y].n = Num;
                matrice[x, y].Type = Type;
            }
            
        }
        public SudokuSolverMatriceManager Clone()
        {
            return new SudokuSolverMatriceManager(this);
        }

        public void pulisci()
        {
            for (int i = 0; i < Dim; i++)
                for (int j = 0; j < Dim; j++)
                    SetNumber(i, j, 0);
        }

        public String Serialize()
        {
            String s = "";
            for (int i = 0; i < this._Dim ; i++)
            {
                for (int j = 0; j < this._Dim; j++)
                {    
                    s += GetNumber(i,j) + " " + Util.Util.NumberTypeToInt(GetSudokuNumber(i, j).Type) + " ";
                }
            }
            return s;
        }
        public void Deserialize(String s)
        {
            pulisci();
            string[] vet = s.Split(' ');
            double l = ((vet.Length - 1)/2).Sqrt();

            if (l.IsInteger() && l.Sqrt().IsInteger())
            {
                if(l==_Dim)
                {
                    int c = 0;
                    for (int i = 0; i < this._Dim; i++)
                    {
                        for (int j = 0; j < this._Dim; j++)
                        {
                            SetNumber(i, j, int.Parse(vet[c++]), Util.Util.IntToNumberType(int.Parse(vet[c++])));
                        }
                    }
                }
                else
                    throw new SudokuSolverException("Dimensione sudoku dei dati non corrisponde alla dimensione della matrice");
            }
            else
                throw new SudokuSolverException("Formato dati invalido");
        }

        public void PulisciCodErr()
        {
            for (int i = 0; i < Dim; i++)
                for (int j = 0; j < Dim; j++)
                    if (GetNumber(i, j) < 1 || GetNumber(i, j) > _Dim)
                        SetNumber(i, j, 0);
        }
    }
}
