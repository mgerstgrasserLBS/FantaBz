using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Xml;

namespace FantaBz
{
    class TableManager
    { 

        private List<Match> matchList;
        private string matchday;
        public TableManager(List<Match> list, string m) {
            matchList = list;
            matchday = m;
            updateDB();
        }

        private void updateDB() {
            string connStr = "Server=127.0.0.1;Port=5555;Database=fantabz;Uid=root;Pwd=Steelers0;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            foreach (Match m in matchList) {
                updateTable(conn, m,m.CompetitionID);
            }
            updateRanksChampionshipTable(conn);
            updateTopPoints(conn);

            conn.Close();
        }

        private void updateTopPoints(MySqlConnection conn) {

            string sqltsQuerry = "select ts_teamid, ts_totalscore, ts_place from t_totalscore where ts_matchday=" + matchday + " order by ts_totalscore desc, ts_place asc";
            MySqlCommand cmd = new MySqlCommand(sqltsQuerry, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            List<string> sqls = new List<string>();
            int pos = 9;
            double last_TS_points = 0;
            int last_TS_Place = 0;
            int points = pos;
            int mostPoints = 0;
            while (rdr.Read())
            {
                if (pos == 9) {
                    mostPoints = 1;
                }
                else
                {
                    mostPoints = 0;
                }
                string lastgrade = (rdr.GetDouble(1) + "").Replace(',', '.');
                string sql1 = "update t_toppoints set tp_toppoints=tp_toppoints+" + points +
                                                        ", tp_nrtopranks=tp_nrtopranks+" + mostPoints +
                                                        ", tp_lastgrade=" + lastgrade +
                                                        ", tp_lastpoints=" + points +
                                                        " where tp_teamid ='" + rdr.GetString(0) +"'";
                sqls.Add(sql1);
                if (last_TS_points == rdr.GetDouble(1) && last_TS_Place.Equals(rdr.GetString(2)))
                {
                    pos--;
                }
                else {
                    pos--;
                    points = pos;
                }
                
            }
            rdr.Close();

            foreach (string s in sqls)
            {
                cmd = new MySqlCommand(s, conn);
                cmd.ExecuteNonQuery();
            }

            string sql = "select tp_teamid,tp_toppoints,tp_nrtopranks,tp_lastgrade, tp_lastpoints from t_toppoints order by tp_toppoints DESC, tp_nrtopranks DESC";
            cmd = new MySqlCommand(sql, conn);
            rdr = cmd.ExecuteReader();
            int rank = 1;
            sqls = new List<string>();
            while (rdr.Read())
            {
                string teamid = rdr.GetString(0);
                string updateSQL = "update t_toppoints set tp_rank = " + rank + " where tp_teamid='" + teamid + "'";
                sqls.Add(updateSQL);
                rank++;
            }
            rdr.Close();

            foreach (string s in sqls)
            {
                cmd = new MySqlCommand(s, conn);
                cmd.ExecuteNonQuery();
            }


        }

        private void updateRanksChampionshipTable(MySqlConnection conn)
        {

            string sql = "select tc_teamid,tc_points,tc_goaldiff,tc_goalsScored from t_championship order by tc_points DESC,tc_goaldiff DESC,tc_goalsScored DESC";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            int rank = 1;
            List<string> sqls = new List<string>();
            while (rdr.Read())
            {
                string teamid = rdr.GetString(0);
                string updateSQL = "update t_championship set tc_rank = " + rank + " where tc_teamid='" + teamid + "'";
                sqls.Add(updateSQL);
                rank++;
            }
            rdr.Close();

            foreach (string s in sqls) {
                cmd = new MySqlCommand(s, conn);
                cmd.ExecuteNonQuery();
            }
        }

        private void updateTable(MySqlConnection conn, Match m, string competition) {

            int winHome = 0;
            int winAway = 0;
            int drawHome = 0;
            int drawAway = 0;
            int lossHome = 0;
            int lossAway = 0;
            int pointsHome = 0;
            int pointsAway = 0;
            int goalsScoredHome = 0;
            int goalsScoredAway = 0;
            int goalsAgainstHome = 0;
            int goalsAgainstAway = 0;
            int goalDiffHome = 0;
            int goalDiffAway = 0;
            int matchesPlayedHome = 0;
            int matchesPlayedAway = 0;

            string table = "";
            string teamid = "";
            string prefix = "";
            if (competition.Equals("C"))
            {
                table = "t_championship";
                teamid = "tc_teamid";
                prefix = "tc";

            } else if (competition.Equals("ACQ")) {
                table = "t_autumcupqualification";
                teamid = "taq_teamid";
                prefix = "taq";
            }


            string sqlcurrentValues = "Select * from "+ table +" where "+ teamid +"='" +m.Teamid_home + "'";
            MySqlCommand cmd = new MySqlCommand(sqlcurrentValues, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                matchesPlayedHome = rdr.GetInt32(1);
                winHome = rdr.GetInt32(2);
                drawHome = rdr.GetInt32(3);
                lossHome = rdr.GetInt32(4);
                pointsHome = rdr.GetInt32(5);
                goalsScoredHome = rdr.GetInt32(6);
                goalsAgainstHome = rdr.GetInt32(7);
                goalDiffHome = rdr.GetInt32(8);
            }
            rdr.Close();
            sqlcurrentValues = "Select * from " + table + " where " + teamid + "='" + m.Teamid_away + "'";
            cmd = new MySqlCommand(sqlcurrentValues, conn);
            rdr = cmd.ExecuteReader();
            if (rdr.Read())
            {
                matchesPlayedAway = rdr.GetInt32(1);
                winAway = rdr.GetInt32(2);
                drawAway = rdr.GetInt32(3);
                lossAway = rdr.GetInt32(4);
                pointsAway = rdr.GetInt32(5);
                goalsScoredAway = rdr.GetInt32(6);
                goalsAgainstAway = rdr.GetInt32(7);
                goalDiffAway = rdr.GetInt32(8);
            }
            rdr.Close();
            if (m.GoalsAwayTeam == m.GoalsHomeTeam)
            {
                drawHome += 1;
                drawAway += 1;
                pointsAway += 1;
                pointsHome += 1;
            }
            else if (m.GoalsAwayTeam < m.GoalsHomeTeam)
            {
                winHome += 1;
                lossAway += 1;
                pointsHome += 3;
            }
            else
            {
                lossHome += 1;
                winAway += 1;
                pointsAway += 3;
            }

            string sqlHome = "Update "+ table + " set "+prefix +"_matchesplayed=" +(matchesPlayedHome+1) +
                            ", "+prefix+"_nrvictories=" + winHome +
                            ", " + prefix + "_nrdraws=" + drawHome +
                            ", " + prefix + "_nrlosts=" + lossHome +
                            ", " + prefix + "_points=" + pointsHome +
                            ", " + prefix + "_goalsscored=" + (goalsScoredHome +  m.GoalsHomeTeam) +
                            ", " + prefix + "_goalsagainst=" + (goalsAgainstHome + m.GoalsAwayTeam) +
                            ", " + prefix + "_goaldiff=" + (goalDiffHome + m.GoalsHomeTeam - m.GoalsAwayTeam) +
                            " where " + prefix +"_teamid = '" + m.Teamid_home + "'";
            Console.WriteLine(sqlHome);

            cmd = new MySqlCommand(sqlHome, conn);
            int r = cmd.ExecuteNonQuery();

            string sqlAway = "Update " + table + " set " + prefix + "_matchesplayed=" + (matchesPlayedAway+1) +
                           ", " + prefix + "_nrvictories=" + winAway +
                           ", " + prefix + "_nrdraws=" + drawAway +
                           ", " + prefix + "_nrlosts=" + lossAway +
                           ", " + prefix + "_points=" + pointsAway +
                           ", " + prefix + "_goalsscored=" + (goalsScoredAway+ m.GoalsAwayTeam) +
                           ", " + prefix + "_goalsagainst=" + (goalsAgainstAway+ m.GoalsHomeTeam) +
                           ", " + prefix + "_goaldiff=" + (goalDiffAway + m.GoalsAwayTeam - m.GoalsHomeTeam) +
                           " where " + prefix + "_teamid = '" + m.Teamid_away + "'";

            Console.WriteLine(sqlAway);
            Console.WriteLine("");
            cmd = new MySqlCommand(sqlAway, conn);
            r = cmd.ExecuteNonQuery();
        }

       /* public static void Main(string[] args) {

            List<Match> matchList = new List<Match>();

            Match m1 = new Match();
            m1.MatchID = "C011";
            m1.Teamid_home = "T01";
            m1.Teamid_away = "T02";
            m1.CompetitionID = "C";
            m1.Day = 6;
            m1.Fantaday = 4;
            m1.GoalsAwayTeam = 0;
            m1.GoalsHomeTeam = 2;
            matchList.Add(m1);

            Match m2 = new Match();
            m2.MatchID = "C012";
            m2.Teamid_home = "T03";
            m2.Teamid_away = "T04";
            m2.CompetitionID = "C";
            m2.Day = 6;
            m2.Fantaday = 4;
            m2.GoalsAwayTeam = 2;
            m2.GoalsHomeTeam = 2;
            matchList.Add(m2);

            Match m3 = new Match();
            m3.MatchID = "C013";
            m3.Teamid_home = "T05";
            m3.Teamid_away = "T06";
            m3.CompetitionID = "C";
            m3.Day = 6;
            m3.Fantaday = 4;
            m3.GoalsAwayTeam = 2;
            m3.GoalsHomeTeam = 0;
            matchList.Add(m3);

            TableManager tm = new TableManager(matchList);
        }*/
    }
}
