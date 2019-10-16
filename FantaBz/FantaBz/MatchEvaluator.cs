using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class MatchEvaluator
    {
        private DateTime dueDateForBonus;
        private LineUp initialLineUpHome;
        private LineUp initialLineUpAway;
        private XmlDocument xmlDoc;

        /*static void Main(string[] args) {

            ParserForPlayerXML parser = new ParserForPlayerXML();
            VotesExcelReader voti = new VotesExcelReader();

            Match m1 = new Match();
            m1.MatchID = "C011";
            m1.Teamid_home = "T01";
            m1.Teamid_away= "T03";
            m1.CompetitionID = "C";
            m1.Day = 6;
            m1.Fantaday = 4;

            Match m2 = new Match();
            m2.MatchID = "C012";
            m2.Teamid_home = "T05";
            m2.Teamid_away = "T08";
            m2.CompetitionID = "C";
            m2.Day = 6;
            m2.Fantaday = 4;

            DateTime dueDate = new DateTime(2019,9,26,23,59,59);
            MatchEvaluator me = new MatchEvaluator(dueDate);
            XmlDocument doc = me.evaluateMatch(m1);
            doc.Save(Console.Out);
        }*/


        public MatchEvaluator(DateTime d, XmlDocument doc) {
            xmlDoc = doc;
            dueDateForBonus = d;
        }

        public XmlElement evaluateMatch(Match match) {

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
            else if (totalAway < 60 && totalHome >= 60 && totalHome-totalAway >= 3) {
                goalHome += 1;
            }
            else if (goalHome != goalAway && totalHome < totalAway && totalAway - totalHome < 3)
            {
                if (totalAway < 72)
                {
                    goalAway -= 1;
                }
                else
                {
                    goalHome += 1;
                }
            } else if (goalHome != goalAway && totalAway<totalHome && totalHome - totalAway <3) {
                if (totalHome < 72)
                {
                    goalHome -= 1;
                }
                else {
                    goalAway += 1;
                }
            }
          
            XmlElement matchNode = xmlDoc.CreateElement("match");
            matchNode.SetAttribute("id",match.MatchID);

            XmlElement homeTeamNode = xmlDoc.CreateElement("team");
            homeTeamNode.SetAttribute("id", match.Teamid_home);
            homeTeamNode.SetAttribute("name", match.Teamid_home);
            matchNode.AppendChild(homeTeamNode);

            XmlElement awayTeamNode = xmlDoc.CreateElement("team");
            awayTeamNode.SetAttribute("id", match.Teamid_away);
            awayTeamNode.SetAttribute("name", match.Teamid_away);
            matchNode.AppendChild(awayTeamNode);

            generateLineUPXML(evaluatedLineUpHome,homeTeamNode);
            generateLineUPXML(evaluatedLineUpAway,awayTeamNode);

            XmlElement homePointsNode = xmlDoc.CreateElement("points");
            XmlElement partialHomeNode = xmlDoc.CreateElement("partial");
            partialHomeNode.InnerText = parzialeHome+"";
            homePointsNode.AppendChild(partialHomeNode);

            XmlElement fattoreCampoHomeNode = xmlDoc.CreateElement("field");
            fattoreCampoHomeNode.InnerText = fattoreCampoHome + "";
            homePointsNode.AppendChild(fattoreCampoHomeNode);

            XmlElement modPerNode = xmlDoc.CreateElement("firstlineup");
            modPerNode.InnerText = modPersonaleHome + "";
            homePointsNode.AppendChild(modPerNode);

            XmlElement modPortieriNode = xmlDoc.CreateElement("goalkeeper");
            modPortieriNode.InnerText = modPortiereHome + "";
            homePointsNode.AppendChild(modPortieriNode);

            XmlElement modDifesaNode = xmlDoc.CreateElement("difense");
            modDifesaNode.InnerText = modDifesaHome + "";
            homePointsNode.AppendChild(modDifesaNode);

            XmlElement modCentrocampoNode = xmlDoc.CreateElement("midfielder");
            modCentrocampoNode.InnerText = modCentrocampoHome + "";
            homePointsNode.AppendChild(modCentrocampoNode);

            XmlElement modAttaccoNode = xmlDoc.CreateElement("striker");
            modAttaccoNode.InnerText = modAttaccoHome + "";
            homePointsNode.AppendChild(modAttaccoNode);

            XmlElement totalNode = xmlDoc.CreateElement("total");
            totalNode.InnerText = totalHome + "";
            homePointsNode.AppendChild(totalNode);

            homeTeamNode.AppendChild(homePointsNode);

            XmlElement homegoalsNode = xmlDoc.CreateElement("goals");
            homegoalsNode.InnerText = goalHome + "";
            homeTeamNode.AppendChild(homegoalsNode);



            XmlElement awayPointsNode = xmlDoc.CreateElement("points");
            XmlElement partialawayNode = xmlDoc.CreateElement("partial");
            partialawayNode.InnerText = parzialeAway + "";
            awayPointsNode.AppendChild(partialawayNode);

            XmlElement fattoreCampoawayNode = xmlDoc.CreateElement("field");
            fattoreCampoawayNode.InnerText = "0";
            awayPointsNode.AppendChild(fattoreCampoawayNode);

            XmlElement modPerAwayNode = xmlDoc.CreateElement("firstlineup");
            modPerAwayNode.InnerText = modPersonaleAway + "";
            awayPointsNode.AppendChild(modPerAwayNode);

            XmlElement modPortieriAwayNode = xmlDoc.CreateElement("goalkeeper");
            modPortieriAwayNode.InnerText = modPortiereAway + "";
            awayPointsNode.AppendChild(modPortieriAwayNode);

            XmlElement modDifesaAwayNode = xmlDoc.CreateElement("defender");
            modDifesaAwayNode.InnerText = modDifesaAway + "";
            awayPointsNode.AppendChild(modDifesaAwayNode);

            XmlElement modCentrocampoAwayNode = xmlDoc.CreateElement("midfielder");
            modCentrocampoAwayNode.InnerText = modCentrocampoAway + "";
            awayPointsNode.AppendChild(modCentrocampoAwayNode);

            XmlElement modAttaccoAwayNode = xmlDoc.CreateElement("striker");
            modAttaccoAwayNode.InnerText = modAttaccoAway + "";
            awayPointsNode.AppendChild(modAttaccoAwayNode);

            XmlElement totalAwayNode = xmlDoc.CreateElement("total");
            totalAwayNode.InnerText = totalAway + "";
            awayPointsNode.AppendChild(totalAwayNode);

            awayTeamNode.AppendChild(awayPointsNode);

            XmlElement awaygoalsNode = xmlDoc.CreateElement("goals");
            awaygoalsNode.InnerText = goalAway + "";
            awayTeamNode.AppendChild(awaygoalsNode);

            return matchNode;
        }

        private void generateLineUPXML(List<PlayerEvaluationEntry> evaluatedLineUp, XmlElement teamNode)
        {

            XmlElement lineupNode = xmlDoc.CreateElement("lineup");
            int sub = 0;

            for (int i = 0; i < 11; i++)
            {
                PlayerEvaluationEntry en = evaluatedLineUp.ElementAt(i);
                if (en.Pid.Equals("ris") || en.Pid.Equals("none"))
                {
                    sub++;
                }
                lineupNode.AppendChild(generatePlayerXML(en));
            }
            teamNode.AppendChild(lineupNode);

            XmlElement substitutesNode = xmlDoc.CreateElement("substitutes");
            for (int i = 11; i < 19+sub; i++)
            {
                substitutesNode.AppendChild(generatePlayerXML(evaluatedLineUp.ElementAt(i)));
            }
            teamNode.AppendChild(substitutesNode);

            XmlElement excludedNode = xmlDoc.CreateElement("excluded");
            for (int i = 19+sub; i < evaluatedLineUp.Count; i++)
            {
                excludedNode.AppendChild(generatePlayerXML(evaluatedLineUp.ElementAt(i)));
            }
            teamNode.AppendChild(excludedNode);
        }

        private XmlElement generatePlayerXML(PlayerEvaluationEntry en) {
            XmlElement playerNode = xmlDoc.CreateElement("footballer");
            playerNode.SetAttribute("id", en.Pid);
            string role;
            string name;
            string squad;
            string grade;
            string bonus;
            string total;
            if (en.Pid.Equals("none"))
            {
                role = en.Pos;
                name = "-----------------------";
                squad = "";
            }
            else if (en.Pid.Equals("ris"))
            {
                role = en.Pos;
                name = "Riserva d'Ufficio";
                squad = "";
            }
            else
            {
                Player p = PlayerList.getPlayer(en.Pid);
                role = p.Position;
                name = p.Name;
                squad = p.Team;
            }
            grade = en.Vote + "";
            bonus = en.BonusMalus + "";
            total = (en.Vote + en.BonusMalus) + "";
            XmlElement playerPosNode = xmlDoc.CreateElement("role");
            playerPosNode.InnerText = role;
            playerNode.AppendChild(playerPosNode);
            XmlElement playerNameNode = xmlDoc.CreateElement("name");
            playerNameNode.InnerText = name;
            playerNode.AppendChild(playerNameNode);
            XmlElement playerSqadNode = xmlDoc.CreateElement("squad");
            playerSqadNode.InnerText = squad;
            playerNode.AppendChild(playerSqadNode);
            XmlElement playerGradeNode = xmlDoc.CreateElement("grade");
            playerGradeNode.InnerText = grade;
            playerNode.AppendChild(playerGradeNode);
            XmlElement playerBonusNode = xmlDoc.CreateElement("bonus");
            playerBonusNode.InnerText = bonus;
            playerNode.AppendChild(playerBonusNode);
            XmlElement playerTotalNode = xmlDoc.CreateElement("total");
            playerTotalNode.InnerText = total;
            playerNode.AppendChild(playerTotalNode);

            return playerNode;
        }

        private int calculateGoals(double sum) {

            if (sum < 66) {
                return 0;
            } else if (sum >= 66 && sum <72) {
                return 1;
            }
            else if (sum >= 72 && sum < 77)
            {
                return 2;
            }
            else if (sum >= 77 && sum < 81)
            {
                return 3;
            }
            else if (sum >= 81 && sum < 85)
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
            DateTime deliveryAway = parseDate(initialLineUpAway.DeliveryTime);

            double[] mod = {0,0};
            int dHome = DateTime.Compare(deliveryHome, dueDateForBonus);
            if (dHome < 0) {
                mod[0] += 0.25;

                int e1 = DateTime.Compare(deliveryHome, deliveryAway);
                if (e1 < 0)
                {
                    mod[0] += 0.25;
                }
            }

            int dAway = DateTime.Compare(deliveryAway, dueDateForBonus);
            if (dAway < 0)
            {
                mod[1] += 0.25;
                int e1 = DateTime.Compare(deliveryHome, deliveryAway);
                if (e1 > 0)
                {
                    mod[1] += 0.25;
                }
            }
            return mod;
        }

        private DateTime parseDate(string date) {

            int year = Int32.Parse(date.Substring(0, 4));
            int month = Int32.Parse(date.Substring(4, 2));
            int day = Int32.Parse(date.Substring(6, 2));
            int h = Int32.Parse(date.Substring(8, 2));
            int m = Int32.Parse(date.Substring(10, 2));
            int s = Int32.Parse(date.Substring(12, 2));
            return new DateTime(year,month,day,h,m,s);
        }

        private double calculateModAttacco(List<PlayerEvaluationEntry> evaluatedLineUp)
        {
            double sum = 0;
            for (int i = 0; i < 11; i++)
            {
                PlayerEvaluationEntry en = evaluatedLineUp.ElementAt(i);
                Player p = PlayerList.getPlayer(en.Pid);
                if (!en.Pid.Equals("ris") && !en.Pid.Equals("none") && p.Position.Equals("S") && !en.ScoredGoal && en.Vote >6)
                {
                    sum += en.Vote-6;

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
                PlayerEvaluationEntry en = evaluatedLineUpHome.ElementAt(i);
                Player p = PlayerList.getPlayer(en.Pid);
                if (!en.Pid.Equals("ris") && !en.Pid.Equals("none") && p.Position.Equals("M"))
                {
                    sumHome += evaluatedLineUpHome.ElementAt(i).Vote;
                    nrPlayerHome++;
                }
                else if ((en.Pid.Equals("ris") || en.Pid.Equals("none")) && en.Pos.Equals("M"))
                {
                    nrPlayerHome++;
                }
            }

            int nrPlayerAway = 0;
            double sumAway = 0;
            for (int i = 0; i < 11; i++)
            {
                PlayerEvaluationEntry en = evaluatedLineUpAway.ElementAt(i);
                Player p = PlayerList.getPlayer(en.Pid);
                if (!en.Pid.Equals("ris") && !en.Pid.Equals("none") && p.Position.Equals("M"))
                {
                    sumAway += evaluatedLineUpAway.ElementAt(i).Vote;
                    nrPlayerAway++;
                }
                else if ((en.Pid.Equals("ris") || en.Pid.Equals("none")) && en.Pos.Equals("M"))
                {
                    nrPlayerAway++;
                }
            }

            double diff = nrPlayerHome - nrPlayerAway;
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
        
        if(d<0.5){
            return 0;
        }else if(d>=0.5 && d<1){
            return 0.25;
        }else if(d>=1 && d<1.75){
            return 0.5;
        }else if(d>=1.75 && d<2.5){
            return 0.75;
        }else if(d>=2.5 && d<3.25){
            return 1;
        }else if(d>=3.25 && d<4){
            return 1.25;
        }else if(d>=4 && d<4.75){
            return 1.5;
        }else if(d>=4.75 && d<5.5){
            return 1.75;
        }else if(d>=5.5 && d<6.25){
            return 2;
        }else if(d>=6.25 && d<7){
            return 2.25;
        }else if(d>=7 && d<7.75){
            return 2.5;
        }else if(d>=7.75 && d<8.5){
            return 2.75;
        }else if(d>=8.5 && d<9.25){
            return 3;
        }else if(d>=9.25 && d<10){
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
                if (!en.Pid.Equals("ris") && !en.Pid.Equals("none") && p.Position.Equals("D"))
                {
                    sum += en.Vote;
                    nrPlayer++;
                } else if ((en.Pid.Equals("ris") || en.Pid.Equals("none")) && en.Pos.Equals("D")) {
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
