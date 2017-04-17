using System.Collections;

namespace Flee
{
    internal class ShortCircuitInfo
    {
        public Stack Operands;

        public Stack Operators;

        public BranchManager Branches;

        public ShortCircuitInfo()
        {
            this.Operands = new Stack();
            this.Operators = new Stack();
            this.Branches = new BranchManager();
        }

        public void ClearTempState()
        {
            this.Operands.Clear();
            this.Operators.Clear();
        }
    }
}