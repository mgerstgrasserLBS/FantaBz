using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class LineUp
    {
        private string deliveryTime;
        private string[] starting11;
        private string[] substitues;
        private List<string> excluded;
        private Dictionary<string, string> penaltyTaker;

        public string DeliveryTime { get => deliveryTime; set => deliveryTime = value; }
        public string[] Starting11 { get => starting11; set => starting11 = value; }
        public string[] Substitues { get => substitues; set => substitues = value; }
        public List<string> Excluded { get => excluded; set => excluded = value; }
        public Dictionary<string, string> PenaltyTaker { get => penaltyTaker; set => penaltyTaker = value; }


        public string toString() {

            string lineup = "\ndeliveryTime: " + deliveryTime + "\n\n Starter \n";
            for (int i = 0; i < starting11.Length; i++) {
                lineup = lineup + starting11[i] + "\n";
            }
            lineup = lineup + "\n Substitues \n";
            for (int i = 0; i < substitues.Length; i++)
            {
                lineup = lineup + substitues[i] + "\n";
            }
            lineup = lineup + "\n Excluded \n";
            foreach (string pid in excluded){
                lineup = lineup + pid + "\n";
            }
            return lineup;

        }
    }
}