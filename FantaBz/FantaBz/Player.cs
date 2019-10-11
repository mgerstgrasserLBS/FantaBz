using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FantaBz
{
    class Player
    {
        private String id = null;
        private String name = null;
        private String position = null;
        private String team = null;

        public string Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public string Position { get => position; set => position = value; }
        public string Team { get => team; set => team = value; }

        public string toString() {

            return id + "   " + name + "   " + position + "   " + Team;
        }
    }
}
