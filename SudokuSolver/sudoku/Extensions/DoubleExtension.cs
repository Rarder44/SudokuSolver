using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Extensions
{
    public static class DoubleExtension
    {
        public static bool IsInteger(this double d)
        {
            return (d - (int)d)==0;
        }
        public static double Sqrt(this double d)
        {
            return Math.Sqrt(d);
        }
    }
}
