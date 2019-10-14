using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class PlayerEvaluationEntry
    {
        private string pid;
        private double vote;
        private double bonusMalus;
        private Boolean scoredGoal;
        private string pos;

        public string Pid { get => pid; set => pid = value; }
        public double Vote { get => vote; set => vote = value; }
        public double BonusMalus { get => bonusMalus; set => bonusMalus = value; }
        public bool ScoredGoal { get => scoredGoal; set => scoredGoal = value; }
        public string Pos { get => pos; set => pos = value; }
    }
}
