using SudokuSolver.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Util
{
    class Util
    {
        public static Dictionary<NumberType, int> NumberTypeToIntDic = new Dictionary<NumberType, int>();
        public static void Init()
        {
            NumberTypeToIntDic.Add(NumberType.InsertByProgram, 1);
            NumberTypeToIntDic.Add(NumberType.InsertByUser, 2);

        }

        public static int NumberTypeToInt(NumberType n)
        {
            return NumberTypeToIntDic[n];
        }
        public static NumberType IntToNumberType(int n)
        {
            foreach(KeyValuePair<NumberType,int> k in NumberTypeToIntDic)
            {
                if (k.Value == n)
                    return k.Key;
            }

            throw new SudokuSolverException("Numero non trovato");
        }


    }
}
