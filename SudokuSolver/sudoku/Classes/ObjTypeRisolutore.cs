using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokuSolver.Classes
{
    class ObjTypeRisolutore
    {
        public delegate void DelegatoTypeRisolutore();
        private DelegatoTypeRisolutore _FuncRisolutore;
        public DelegatoTypeRisolutore FuncRisolutore
        {
            get { return _FuncRisolutore; }
            set { _FuncRisolutore = value; }
        }



        string _Nome;
        public string Nome
        {
            get { return _Nome; }
            set { _Nome = value; }
        }


        public ObjTypeRisolutore(DelegatoTypeRisolutore FuncRisolutore,string Nome)
        {
            this.FuncRisolutore = FuncRisolutore;
            this.Nome = Nome;
        }

        public override string ToString()
        {
            return Nome;
        }
        
    }
}
