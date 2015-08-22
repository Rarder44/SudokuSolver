using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Util
{
    public class SudokuNumber
    {
        int _n=0;
        public int n
        {
            get { return _n; }
            set { _n = value; }
        }
        NumberType _Type = NumberType.InsertByUser;
        public NumberType Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        public SudokuNumber(int n=0,NumberType Type=NumberType.InsertByUser)
        {
            this.n = n;
            this.Type = Type;
        }
    }
}
