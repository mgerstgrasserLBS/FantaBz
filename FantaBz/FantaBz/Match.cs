using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class Match
    {
        private int year;
        private int day;
        private string matchID;
        private string competitionID;
        private int fantaday;
        private string teamid_away;
        private string teamid_home;
        private int goalsHomeTeam;
        private int goalsAwayTeam;

        public int Year { get => year; set => year = value; }
        public int Day { get => day; set => day = value; }
        public string MatchID { get => matchID; set => matchID = value; }
        public string CompetitionID { get => competitionID; set => competitionID = value; }
        public int Fantaday { get => fantaday; set => fantaday = value; }
        public string Teamid_away { get => teamid_away; set => teamid_away = value; }
        public string Teamid_home { get => teamid_home; set => teamid_home = value; }
        public int GoalsHomeTeam { get => goalsHomeTeam; set => goalsHomeTeam = value; }
        public int GoalsAwayTeam { get => goalsAwayTeam; set => goalsAwayTeam = value; }

        public String toString() {
            return year + " | " + day + " |" + matchID + " | " + competitionID + " | " +
                    fantaday + " | " + teamid_home + " |" + teamid_away + " | ";
        }
    }
}
