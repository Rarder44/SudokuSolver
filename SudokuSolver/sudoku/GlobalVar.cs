using SudokuSolver.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver
{
    static class GlobalVar
    {
        public delegate void AppStatusChangeDel(AppStatus Old, AppStatus New);
        public static event AppStatusChangeDel AppStatusChange;

        static AppStatus _Stato;
        public static AppStatus Stato
        {
            get
            {
                return _Stato;
            }
            set
            {
                AppStatus old = _Stato;
                _Stato = value;
                if (AppStatusChange != null)
                    AppStatusChange(old, _Stato);
            }
        }
    }
}
