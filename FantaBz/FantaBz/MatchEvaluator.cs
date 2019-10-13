using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class MatchEvaluator
    {
        private DateTime dueDateForBonus;
        private LineUp initialLineUpHome;
        private LineUp initialLineUpAway;

        public MatchEvaluator(DateTime d) {
            dueDateForBonus = d;
        }

        public void evaluateMatch(Match match) {

            LineUpEvaluator le = new LineUpEvaluator();
            List<PlayerEvaluationEntry> evaluatedLineUpHome = le.evaluateLinuep(match.Teamid_home, match.Day, match.CompetitionID);
            initialLineUpHome = le.InitialLinup;
            List<PlayerEvaluationEntry> evaluatedLineUpAway = le.evaluateLinuep(match.Teamid_away, match.Day, match.CompetitionID);
            initialLineUpAway = le.InitialLinup;

            double parzialeHome = calculateParzialeSquadra(evaluatedLineUpHome);
            double parzialeAway = calculateParzialeSquadra(evaluatedLineUpAway);

            double fattoreCampoHome = calculateFattoreCampo(match.CompetitionID);

            double modPortiereHome = calculateModPortiere(evaluatedLineUpAway);
            double modPortiereAway = calculateModPortiere(evaluatedLineUpHome);

            double modDifesaHome = calculateModDifesa(evaluatedLineUpAway);
            double modDifesaAway = calculateModDifesa(evaluatedLineUpHome);

            double[] modCentrocampo = calculateModCentrocampo(evaluatedLineUpHome, evaluatedLineUpAway);
            double modCentrocampoHome = modCentrocampo[0];
            double modCentrocampoAway = modCentrocampo[1];

            double modAttaccoHome = calculateModAttacco(evaluatedLineUpHome);
            double modAttaccoAway = calculateModAttacco(evaluatedLineUpAway);

            double[] modPersonale = calculateModPersonale();
            double modPersonaleHome = modPersonale[0];
            double modPersonaleAway = modPersonale[1];

            double totalHome = parzialeHome + fattoreCampoHome + modPortiereHome + modDifesaHome + modCentrocampoHome + modAttaccoHome + modPersonaleHome;
            double totalAway = parzialeAway + modPortiereAway + modDifesaAway + modCentrocampoAway + modAttaccoAway + modPersonaleAway;

            int goalHome = calculateGoals(totalHome);
            int goalAway = calculateGoals(totalAway);

            if (totalHome < 60 && totalAway >= 60 && totalAway - totalHome >= 3)
            {
                goalAway += 1;
            }
            else if (totalAway < 60 && totalHome >= 60 && totalHome - totalAway >= 3)
            {
                goalHome += 1;
            }
            else if (totalHome - totalAway < 3 && totalHome < 72)
            {
                goalHome -= 1;
            }
            else if (totalAway - totalHome < 3 && totalAway < 72) {
                goalAway -= 1;
            }
            else if (totalHome - totalAway < 3 && totalHome >= 72) {
                goalAway += 1;
            }
            else if (totalAway - totalHome < 3 && totalAway >= 72)
            {
                goalHome += 1;
            }


        }

        public int calculateGoals(double sum) {

            if (sum < 66) {
                return 0;
            } else if (sum >= 66 && sum <72) {
                return 1;
            }
            else if (sum >= 72 && sum < 77)
            {
                return 2;
            }
            else if (sum >= 77 && sum < 82)
            {
                return 3;
            }
            else if (sum >= 82 && sum < 85)
            {
                return 4;
            }
            else if (sum >= 85 && sum < 89)
            {
                return 5;
            }
            else if (sum >= 89 && sum < 93)
            {
                return 6;
            }
            else if (sum >= 93 && sum < 97)
            {
                return 7;
            }
            else if (sum >= 97 && sum < 101)
            {
                return 8;
            }
            else if (sum >= 101 && sum < 105)
            {
                return 9;
            }
            else
            {
                return 10;
            }
        }

        public double[] calculateModPersonale() {

            DateTime deliveryHome = parseDate(initialLineUpHome.DeliveryTime);
            DateTime deliveryAway = parseDate(initialLineUpHome.DeliveryTime);

            double[] mod = {0,0};
            int dHome = DateTime.Compare(deliveryHome, dueDateForBonus);
            if (dHome < 0) {
                mod[0] += 0.25;
            }

            int dAway = DateTime.Compare(deliveryAway, dueDateForBonus);
            if (dAway < 0)
            {
                mod[1] += 0.25;
            }

            int e = DateTime.Compare(deliveryHome, deliveryAway);
            if (e > 0)
            {
                mod[1] += 0.25;
            }
            else if (e < 0) {
                mod[0] += 0.25;
            }
            return mod;
        }

        private DateTime parseDate(string date) {

            int year = Int32.Parse(date.Substring(0, 4));
            int month = Int32.Parse(date.Substring(4, 2));
            int day = Int32.Parse(date.Substring(6, 2));
            int h = Int32.Parse(date.Substring(8, 2));
            int m = Int32.Parse(date.Substring(10, 2));
            int s = Int32.Parse(date.Substring(10, 2));
            return new DateTime(year,month,day,h,m,s);
        }

        private double calculateModAttacco(List<PlayerEvaluationEntry> evaluatedLineUp)
        {
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                PlayerEvaluationEntry en = evaluatedLineUp.ElementAt(i);
                Player p = PlayerList.getPlayer(en.Pid);
                if (p.Position.Equals("A") && !en.ScoredGoal && en.Vote >6)
                {
                    sum += sum + en.Vote-6;

                }
            }
            return sum;
        }

        private double[] calculateModCentrocampo(List<PlayerEvaluationEntry> evaluatedLineUpHome, List<PlayerEvaluationEntry> evaluatedLineUpAway)
        {
            int nrPlayerHome = 0;
            double sumHome = 0;
            for (int i = 0; i < 11; i++)
            {
                Player p = PlayerList.getPlayer(evaluatedLineUpHome.ElementAt(i).Pid);
                if (p.Position.Equals("C"))
                {
                    sumHome += evaluatedLineUpHome.ElementAt(i).Vote;
                    nrPlayerHome++;
                }
            }

            int nrPlayerAway = 0;
            double sumAway = 0;
            for (int i = 0; i < 11; i++)
            {
                Player p = PlayerList.getPlayer(evaluatedLineUpAway.ElementAt(i).Pid);
                if (p.Position.Equals("C"))
                {
                    sumAway += evaluatedLineUpAway.ElementAt(i).Vote;
                    nrPlayerAway++;
                }
            }

            int diff = nrPlayerHome - nrPlayerAway;
            if (diff == -2)
            {
                sumHome += 10;
            }
            else if (diff == -1)
            {
                sumHome += 5;
            }
            else if (diff == 1)
            {
                sumAway += 5;
            }
            else if (diff == 2)
            {
                sumAway += 10;
            }
            double[] mod = new double[2];
            if (sumHome < sumAway)
            {
                double erg = getModCentrocampo(sumAway - sumHome);
                mod[0] = erg * -1;
                mod[1] = erg;
            }
            else
            {
                double erg = getModCentrocampo(sumHome - sumAway);
                mod[0] = erg;
                mod[1] = erg * -1;
                
            }

            return mod;
        }

        private double getModCentrocampo(double d){
        
        if(d<=0.5){
            return 0;
        }else if(d>0.5 && d<=1){
            return 0.25;
        }else if(d>1 && d<=1.75){
            return 0.5;
        }else if(d>1.75 && d<=2.5){
            return 0.75;
        }else if(d>2.5 && d<=3.25){
            return 1;
        }else if(d>3.25 && d<=4){
            return 1.25;
        }else if(d>4 && d<=4.75){
            return 1.5;
        }else if(d>4.75 && d<=5.5){
            return 1.75;
        }else if(d>5.5 && d<=6.25){
            return 2;
        }else if(d>6.25 && d<7){
            return 2.25;
        }else if(d>7 && d<=7.75){
            return 2.5;
        }else if(d>7.75 && d<=8.5){
            return 2.75;
        }else if(d>8.5 && d<=9.25){
            return 3;
        }else if(d>9.25 && d<=10){
            return 3.25;
        }else{
            return 3.5;
        }
    }

        private double calculateModDifesa(List<PlayerEvaluationEntry> evaluatedLineUpHome)
        {

            int nrPlayer = 0;
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                PlayerEvaluationEntry en = evaluatedLineUpHome.ElementAt(i);
                Player p = PlayerList.getPlayer(en.Pid);
                if (p.Position.Equals("D"))
                {
                    sum += sum + en.Vote;
                    nrPlayer++;
                }
            }
            if (nrPlayer == 3)
            {
                sum += -1;
            }
            else if (nrPlayer == 5)
            {
                sum += +1;
            }
            return getModDifesa(sum / nrPlayer);
        }

        private double getModDifesa(double d)
        {

            if (d < 5.125)
            {
                return 2;
            }
            else if (d >= 5.125 && d < 5.25)
            {
                return 1.75;
            }
            else if (d >= 5.25 && d < 5.375)
            {
                return 1.5;
            }
            else if (d >= 5.375 && d < 5.5)
            {
                return 1.25;
            }
            else if (d >= 5.5 && d < 5.625)
            {
                return 1;
            }
            else if (d >= 5.625 && d < 5.75)
            {
                return 0.75;
            }
            else if (d >= 5.75 && d < 5.875)
            {
                return 0.5;
            }
            else if (d >= 5.875 && d < 6)
            {
                return 0.25;
            }
            else if (d >= 6 && d < 6.125)
            {
                return 0;
            }
            else if (d >= 6.125 && d < 6.25)
            {
                return -0.25;
            }
            else if (d >= 6.25 && d < 6.375)
            {
                return -0.5;
            }
            else if (d >= 6.375 && d < 6.5)
            {
                return -0.75;
            }
            else if (d >= 6.5 && d < 6.625)
            {
                return -1;
            }
            else if (d >= 6.625 && d < 6.75)
            {
                return -1.25;
            }
            else if (d >= 6.75 && d < 6.875)
            {
                return -1.5;
            }
            else if (d >= 6.875 && d < 7)
            {
                return -1.75;
            }
            else
            {
                return -2;
            }

        }


        private double calculateModPortiere(List<PlayerEvaluationEntry> evaluatedLineUp) {
            double mod = 0;
            if (!(evaluatedLineUp.ElementAt(0).Pid.Equals("ris") || evaluatedLineUp.ElementAt(0).Pid.Equals("none"))) {
                mod = (evaluatedLineUp.ElementAt(0).Vote - 6) * -1;
            }
            return mod;
        }

        private double calculateFattoreCampo(string compid) {
            double fattorecampo = 0;

            if (compid.Equals("C") || compid.Equals("LCS1") || compid.Equals("LCS2") || compid.Equals("WCQ"))
                fattorecampo = 2;

            return fattorecampo;

        }

        private double calculateParzialeSquadra(List<PlayerEvaluationEntry> evaluatedLinueUp) {

            double parziale = 0;
            for (int i = 0; i < 11;i++) 
            {
                PlayerEvaluationEntry e = evaluatedLinueUp.ElementAt(i);
                parziale += e.Vote + e.BonusMalus;
            }
            return parziale;

        }
    }
}
