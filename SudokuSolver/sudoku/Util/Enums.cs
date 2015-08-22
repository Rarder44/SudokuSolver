using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Util
{
    public enum AppStatus
    {
        Starting,
        Waiting,
        Working,
        Stopping
    }

    public enum NumberType
    {
        InsertByUser,
        InsertByProgram
    }
    
}
