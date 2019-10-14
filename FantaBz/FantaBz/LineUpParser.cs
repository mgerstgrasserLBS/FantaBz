using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace FantaBz
{
    class LineUpParser
    {

     /*   static void Main(string[] args)
        {

            LineUp l = LineUpParser.parseLineUPXML("");
            Console.Out.WriteLine(l.toString());
        }
       */
        public static LineUp parseLineUPXML(string file)
        {

            XmlDocument doc = new XmlDocument();
            doc.Load(file);
            //doc.Load("C:\\fantabz\\F2019_6_C_T01.xml");

            LineUp lineup = new LineUp();



            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/fantabzzz/competition/team");
            foreach (XmlNode node in nodes)
            {
                string deliveryTime = node.SelectSingleNode("delivery").InnerText;
                lineup.DeliveryTime = deliveryTime;
            }

            XmlNodeList starterNodes = doc.DocumentElement.SelectNodes("/fantabzzz/competition/team/lineup/footballer");
            string[] starter = new string[11];
            int i = 0;
            Dictionary<string, string> penaltyTaker = new Dictionary<string, string>();
            foreach (XmlNode node in starterNodes)
            {
                string pid = node.Attributes["id"].Value;
                string pen = node.SelectSingleNode("penaltyPos").InnerText;
                starter[i] = pid;
                i++;
                penaltyTaker.Add(pid, pen);
            }
            lineup.Starting11 = starter;
            lineup.PenaltyTaker = penaltyTaker;

            XmlNodeList substitutesNodes = doc.DocumentElement.SelectNodes("/fantabzzz/competition/team/substitutes/footballer");
            string[] substitutes = new string[8];
            int j = 0;
            foreach (XmlNode node in substitutesNodes)
            {
                string pid = node.Attributes["id"].Value;
                substitutes[j] = pid;
                j++;
            }
            lineup.Substitues = substitutes;

            XmlNodeList excludedNodes = doc.DocumentElement.SelectNodes("/fantabzzz/competition/team/excluded/footballer");
            List<string> excluded = new List<string>();
            foreach (XmlNode node in excludedNodes)
            {
                string pid = node.Attributes["id"].Value;
                excluded.Add(pid);
            }
            lineup.Excluded = excluded;
            return lineup;
        }
    }
}
