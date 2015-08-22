using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Extensions
{
    public static class StringExtension
    {
        public static bool IsInt(this String d)
        {
            int n;
            return int.TryParse(d, out n);
        }
        public static int ParseInt(this String d)
        {
            return int.Parse(d);
        }

    }
}
