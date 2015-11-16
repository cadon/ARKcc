using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARKcc
{
    class Entity
    {
        public Entity() { }
        public string id { set; get; }
        public string name { set; get; }
        public string bp { set; get; }
        public int maxstack { set; get; }
        public string category { set; get; }
    }
}
