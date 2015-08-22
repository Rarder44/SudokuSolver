using System;
using SudokuSolver.Util;
using SudokuSolver.Extensions;
using SudokuSolver.Exceptions;
namespace SudokuSolver.Services
{
    public class SudokuSolverMatriceManager
    {
        #region Variabili
        SudokuNumber[,] matrice = null;
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
        public SudokuSolverMatriceManager(SudokuSolverMatriceManager mng)
        {
            SetDimMatrice(mng._Dim);
            for (int i = 0; i < mng._Dim; i++)
                for (int j = 0; j < mng._Dim; j++)
                    SetNumber(i, j, mng.GetNumber(i, j), mng.GetSudokuNumber(i, j).Type);
        }
        public SudokuSolverMatriceManager(int Dim)
        {
            SetDimMatrice(Dim);
            pulisci();
        }
        public SudokuSolverMatriceManager Clone()
        {
            return new SudokuSolverMatriceManager(this);
        }
        #endregion

        #region Get Set
        public int GetNumber(int x, int y)
        {
            return matrice[x, y].n;
        }
        public SudokuNumber GetSudokuNumber(int x, int y)
        {
            return matrice[x, y];
        }
        public void SetNumber(int x, int y, int Num, NumberType Type = NumberType.InsertByUser)
        {
            if (matrice[x, y] == null)
                matrice[x, y] = new SudokuNumber(Num, Type);
            else
            {
                matrice[x, y].n = Num;
                matrice[x, y].Type = Type;
            }

        }
        #endregion

        #region Operazioni Tutta Matrice
        private void SetDimMatrice(int Dim)
        {
            _Dim = Dim;
            matrice = new SudokuNumber[Dim, Dim];
        }

        public void PulisciCodErr()
        {
            for (int i = 0; i < Dim; i++)
                for (int j = 0; j < Dim; j++)
                    if (GetNumber(i, j) < 1 || GetNumber(i, j) > _Dim)
                        SetNumber(i, j, 0);
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
            for (int i = 0; i < this._Dim; i++)
            {
                for (int j = 0; j < this._Dim; j++)
                {
                    s += GetNumber(i, j) + " " + Util.Util.NumberTypeToInt(GetSudokuNumber(i, j).Type) + " ";
                }
            }
            return s;
        }

        /// <summary>
        /// Deserializza una stringa
        /// </summary>
        /// <param name="s">Stringa da deserealizzare</param>
        /// <returns>Dimensione del sudoku</returns>
        public int Deserialize(String s)
        {
            string[] vet = s.Split(' ');
            double l = ((vet.Length - 1) / 2).Sqrt();

            if (CheckValidDim(l))
            {
                SetDimMatrice((int)l);
                int c = 0;
                for (int i = 0; i < this._Dim; i++)
                {
                    for (int j = 0; j < this._Dim; j++)
                    {
                        SetNumber(i, j, int.Parse(vet[c++]), Util.Util.IntToNumberType(int.Parse(vet[c++])));
                    }
                }
                return (int)l;
            }
            else
                throw new SudokuSolverException("Formato dati invalido");
        }
        #endregion

        #region Util

        public static bool CheckValidDim(int Dim)
        {
            return Dim.Sqrt().IsInteger();
        }
        public static bool CheckValidDim(double Dim)
        {
            return Dim.Sqrt().IsInteger();
        }


        #endregion
    }
}
