using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FantaBz
{
    class ParserForPlayerXML
    {

        public ParserForPlayerXML() {
            parsePlayerXML();
        }

        private void parsePlayerXML() {
            XmlDocument doc = new XmlDocument();
            doc.Load("F2019_TEAMS.xml");

            XmlNodeList nodes = doc.DocumentElement.SelectNodes("/fantabzzz/team/active/footballer");

            foreach (XmlNode node in nodes)
            {
                Player pl = new Player();

                pl.Id = node.Attributes["id"].Value;
                pl.Name = node.SelectSingleNode("name").InnerText;
                pl.Position = node.SelectSingleNode("pos").InnerText;
                pl.Team = node.SelectSingleNode("team").InnerText;
                PlayerList.addPlayer(pl);
               // Console.Out.WriteLine(pl.toString());
            }

        }


        /*static void Main(string[] args) {
            ParserForPlayerXML p = new ParserForPlayerXML();
        }*/
    }



}
