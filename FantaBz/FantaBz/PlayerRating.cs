using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class PlayerRating
    {
        private string id;

        private double votoGazzetta;
        private double golFattiGazzetta;
        private double golSubitiGazzetta;
        private double autoRetiGazzetta;
        private double assistGazzetta;

        private double votoCorriere;
        private double golFattiCorriere;
        private double golSubitiCorriere;
        private double autoRetiCorriere;
        private double assistCorriere;

        private double ammonizione;
        private double esplusione;
        private double golVittoria;
        private double golPareggio;

        private double rigoreSbagliato;
        private double rigoreParato;
        private double rigoreTrasformato;

        public string Id { get => id; set => id = value; }
        public double VotoGazzetta { get => votoGazzetta; set => votoGazzetta = value; }
        public double GolFattiGazzetta { get => golFattiGazzetta; set => golFattiGazzetta = value; }
        public double GolSubitiGazzetta { get => golSubitiGazzetta; set => golSubitiGazzetta = value; }
        public double AutoRetiGazzetta { get => autoRetiGazzetta; set => autoRetiGazzetta = value; }
        public double AssistGazzetta { get => assistGazzetta; set => assistGazzetta = value; }
        public double VotoCorriere { get => votoCorriere; set => votoCorriere = value; }
        public double GolFattiCorriere { get => golFattiCorriere; set => golFattiCorriere = value; }
        public double GolSubitiCorriere { get => golSubitiCorriere; set => golSubitiCorriere = value; }
        public double AutoRetiCorriere { get => autoRetiCorriere; set => autoRetiCorriere = value; }
        public double AssistCorriere { get => assistCorriere; set => assistCorriere = value; }
        public double Ammonizione { get => ammonizione; set => ammonizione = value; }
        public double Esplusione { get => esplusione; set => esplusione = value; }
        public double GolVittoria { get => golVittoria; set => golVittoria = value; }
        public double RigoreSbagliato { get => rigoreSbagliato; set => rigoreSbagliato = value; }
        public double RigoreParato { get => rigoreParato; set => rigoreParato = value; }
        public double RigoreTrasformato { get => rigoreTrasformato; set => rigoreTrasformato = value; }
        public double GolPareggio { get => golPareggio; set => golPareggio = value; }

        public String toString()
        {

            return id + "\t" + votoGazzetta + "\t" + golFattiGazzetta + "\t" + golSubitiGazzetta + "\t" + autoRetiGazzetta + "\t" + assistGazzetta + "\t" +
                votoCorriere + "\t" + golFattiCorriere + "\t" + golSubitiCorriere + "\t" + autoRetiCorriere + "\t" + assistCorriere + "\t" + ammonizione + "\t" +
                esplusione + "\t" + golVittoria + "\t" +golPareggio + "\t" + rigoreSbagliato + "\t" + rigoreParato + "\t" + rigoreTrasformato;
        }
    }
    
}
