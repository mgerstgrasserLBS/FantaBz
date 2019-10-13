using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class LineUpEvaluator
    {
        private LineUp initialLinup;

        internal LineUp InitialLinup { get => initialLinup; set => initialLinup = value; }

        public List<PlayerEvaluationEntry> evaluateLinuep(String tid, int mday, string comp) {

            List<PlayerEvaluationEntry> evaluatedLinup = new List<PlayerEvaluationEntry>();
            string lineUpFile = "F2019_" + mday + "_" + comp + "_"+tid + ".xml";

            InitialLinup = LineUpParser.parseLineUPXML(lineUpFile);
            PlayerEvaluationEntry[] starting11 = new PlayerEvaluationEntry[InitialLinup.Starting11.Length];
            for (int i = 0; i < InitialLinup.Starting11.Length; i++) {
                string pid = InitialLinup.Starting11[i];
                starting11[i] = evaluatePlayer(pid);
            }

            PlayerEvaluationEntry[] substitutes = new PlayerEvaluationEntry[InitialLinup.Substitues.Length];
            for (int i=0; i< InitialLinup.Substitues.Length;i++) {
                string pid = InitialLinup.Substitues[i];
                substitutes[i] = evaluatePlayer(pid);
            }

            List<PlayerEvaluationEntry> excluded = new List<PlayerEvaluationEntry>();
            foreach (string pid in InitialLinup.Excluded) {
                excluded.Add(evaluatePlayer(pid));
            }

            evaluatedLinup.AddRange(substitutePlayersWithNoVote(starting11,substitutes));
            evaluatedLinup.AddRange(excluded);

            return evaluatedLinup;
        }

        private List<PlayerEvaluationEntry> substitutePlayersWithNoVote(PlayerEvaluationEntry[] starting11, PlayerEvaluationEntry[] substitutes) {

            List<PlayerEvaluationEntry> finalLinup = new List<PlayerEvaluationEntry>();
            List<PlayerEvaluationEntry> substitutedPlayers = new List<PlayerEvaluationEntry>();
            int nrSubstitutions = 0;
            int lastRiservaDUffico = -1;
            for (int i = 0; i < starting11.Length; i++) {
                if (starting11[i].Vote == 0) {
                    substitutedPlayers.Add(starting11[i]);
                    int sub = findSubstitute(starting11[i].Pid,substitutes);
                    if (sub != -1 && nrSubstitutions < 3)
                    {
                        starting11[i] = substitutes[sub];
                        substitutes[sub] = null;
                        nrSubstitutions++;
                    }
                    else if (sub == -1 && nrSubstitutions < 3)
                    {
                        PlayerEvaluationEntry entry = new PlayerEvaluationEntry();
                        entry.Pid = "ris";
                        entry.Vote = 0;
                        Player p = PlayerList.getPlayer(starting11[i].Pid);
                        if (p.Position.Equals("P"))
                        {
                            entry.BonusMalus = 3;
                        }
                        else
                        {
                            entry.BonusMalus = 4;
                        }
                        entry.ScoredGoal = false;
                        starting11[i] = entry;
                        lastRiservaDUffico = i;
                        nrSubstitutions++;
                    }
                    else if (sub != -1 && nrSubstitutions >= 3 && lastRiservaDUffico != -1)
                    {
                        starting11[i] = substitutes[sub];
                        substitutes[sub] = null;
                        PlayerEvaluationEntry entry = new PlayerEvaluationEntry();
                        entry.Pid = "none";
                        entry.Vote = 0;
                        entry.BonusMalus = 0;
                        entry.ScoredGoal = false;
                        starting11[lastRiservaDUffico] = entry;
                        lastRiservaDUffico = -1;
                        for (int j = 0; j <= i; j++)
                        {
                            if (starting11[j].Pid.Equals("ris"))
                            {
                                lastRiservaDUffico = j;
                            }
                        }
                    }
                    else {
                        PlayerEvaluationEntry entry = new PlayerEvaluationEntry();
                        entry.Pid = "none";
                        entry.Vote = 0;
                        entry.BonusMalus = 0;
                        entry.ScoredGoal = false;
                        starting11[i] = entry;
                    }
                }
            }

            for (int i = 0; i < starting11.Length; i++)
            {
                finalLinup.Add(starting11[i]);
            }
            for (int i = 0; i < substitutes.Length; i++)
            {
                if (substitutes[i] != null)
                {
                    finalLinup.Add(starting11[i]);
                }
            }
            finalLinup.AddRange(substitutedPlayers);
            return finalLinup;
        }

        private int findSubstitute(string pid, PlayerEvaluationEntry[] substitutes) {

            for (int i = 0; i < substitutes.Length; i++)
            {
                if (substitutes[i].Vote != 0)
                {
                    Player toSub = PlayerList.getPlayer(pid);
                    Player possibleSub = PlayerList.getPlayer(substitutes[i].Pid);
                    if (possibleSub.Position.Equals(toSub.Position))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        private PlayerEvaluationEntry evaluatePlayer(string pid) {
            PlayerRating ratingForPid = PlayerRatingsList.getPlayerRating(pid);
            PlayerEvaluationEntry entry = new PlayerEvaluationEntry();
            entry.Pid = pid;
            if (ratingForPid != null)
            {
                entry.Vote = calculateVote(ratingForPid);
                if (ratingForPid.GolFattiGazzetta != 0 || ratingForPid.GolFattiCorriere != 0)
                {
                    entry.ScoredGoal = true;
                }
                else
                {
                    entry.ScoredGoal = false;
                }
                entry.BonusMalus = calculateBonusMalus(ratingForPid);
            }
            else
            {
                entry.Vote = 0;
                entry.BonusMalus = 0;
                entry.ScoredGoal = false;
            }
            return entry;
        }


        private double calculateVote(PlayerRating pr)
        {
            double vote = 0;
            if (pr.VotoCorriere != 0 && pr.VotoGazzetta != 0)
            {
                vote = (pr.VotoGazzetta + pr.VotoCorriere) / 2;
            }
            else if (pr.VotoCorriere == 0)
            {
                vote = pr.VotoGazzetta;
            }
            else if (pr.VotoGazzetta == 0)
            {
                vote = pr.VotoCorriere;
            }
            else
            {
                vote = 0;
            }
            return vote;
        }

        private double calculateBonusMalus(PlayerRating pr) {
            double bm = 0;
            Player p = PlayerList.getPlayer(pr.Id);

            if (pr.GolFattiGazzetta > 0) {
                if (p.Position.Equals("A"))
                {
                    bm += (pr.GolFattiGazzetta - pr.RigoreTrasformato) * 3;
                }
                else {
                    bm += (pr.GolFattiGazzetta - pr.RigoreTrasformato) * 2.5;
                }
            }

            bm += pr.GolVittoria * 0.25;
            bm += pr.AssistGazzetta * 1;
            bm += pr.RigoreParato * 2;
            bm += pr.RigoreTrasformato * 2;
            bm -= pr.Ammonizione * 0.5;
            bm -= pr.AutoRetiGazzetta * 2;
            bm -= pr.Esplusione * 1;
            bm -= pr.GolSubitiGazzetta * 0.75;
            bm -= pr.RigoreSbagliato * 2;

            return bm;
        }

    }
}
