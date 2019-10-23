using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Xml;

namespace FantaBz
{
    public partial class Form1 : Form
    {

        private List<Match> matchList = new List<Match>();
        XmlDocument xmlDoc = new XmlDocument();

        public static void Main(string[] args) {

            ParserForPlayerXML parser = new ParserForPlayerXML();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        public Form1()
        {
            InitializeComponent();
            
        }

        private void fillMatchList()
        {
            string connStr = "Server=127.0.0.1;Port=5555;Database=fantabz;Uid=root;Pwd=Steelers0;";
            MySqlConnection conn = new MySqlConnection(connStr);
            conn.Open();

            string sql = "SELECT * FROM t_calendar where ca_day=" + comboBox1.Text;
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            MySqlDataReader rdr = cmd.ExecuteReader();
            
            while (rdr.Read())
            {
                Match m = new Match();
                m.Year = rdr.GetInt32(1);
                m.Day = rdr.GetInt32(2);
                m.CompetitionID = rdr.GetString(3);
                m.MatchID = rdr.GetString(4);
                m.Fantaday = rdr.GetInt32(5);
                m.Teamid_home = rdr.GetString(6);
                m.Teamid_away = rdr.GetString(7);
                matchList.Add(m);
            }
            rdr.Close();
            conn.Close();
        }

        private void createMatchDayXML()
        {
            DateTime d = dateTimePicker1.Value;
            DateTime dDate = new DateTime(d.Year, d.Month, d.Day, 23, 59, 59);
            xmlDoc.AppendChild(xmlDoc.CreateXmlDeclaration("1.0","utf-8",""));
            xmlDoc.AppendChild(xmlDoc.CreateProcessingInstruction("xml-stylesheet","type='text/xsl' href='results.xsl'"));
            XmlElement rootNode = xmlDoc.CreateElement("fantabzzz");
            xmlDoc.AppendChild(rootNode);
            XmlElement infoNode = xmlDoc.CreateElement("info");
            XmlElement infoYearNode = xmlDoc.CreateElement("year");
            infoYearNode.InnerText = dDate.Year + "";
            infoNode.AppendChild(infoYearNode);
            XmlElement infodayNode = xmlDoc.CreateElement("day");
            infodayNode.InnerText = comboBox1.Text;
            infoNode.AppendChild(infodayNode);
            rootNode.AppendChild(infoNode);

            XmlElement champNode = xmlDoc.CreateElement("competition");
            champNode.SetAttribute("id", "C");
            champNode.SetAttribute("name", "Campionato");
            rootNode.AppendChild(champNode);

            XmlElement cupNode = xmlDoc.CreateElement("competition");
            cupNode.SetAttribute("id", "ACQ");
            cupNode.SetAttribute("name", "Coppa");

            foreach (Match m in matchList) {
                MatchEvaluator me = new MatchEvaluator(dDate,xmlDoc);
                XmlElement matchNode = me.evaluateMatch(m);
                if (m.CompetitionID.Equals("C")) {
                    champNode.AppendChild(matchNode);
                }
                else
                {
                    cupNode.AppendChild(matchNode);
                }
            }
            rootNode.AppendChild(champNode);
            rootNode.AppendChild(cupNode);
            string fileName = "C:\\fantabz\\F2019_" + comboBox1.Text + "_RESULTS.xml";
            xmlDoc.Save(fileName);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Console.Out.WriteLine("Giornata: " + comboBox1.Text);
            Console.Out.WriteLine("due date bonus: " + dateTimePicker1.Value);
            VotesExcelReader voti = new VotesExcelReader(Int32.Parse(comboBox1.Text));
            fillMatchList();
            createMatchDayXML();
            //TableManager tm = new TableManager(matchList, comboBox1.Text);
            Application.Exit();
        }
    }
}
