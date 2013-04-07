using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATN.Export
{
    public class Graph
    {
        private List<SourceNode> _nodes;
        private List<SourceEdge> _edges;
        public Graph()
        {
            _nodes = new List<SourceNode>();
            _edges = new List<SourceEdge>();
        }

        public List<SourceNode> Nodes
        {
            get
            {
                return _nodes;
            }
            set
            {
                _nodes = value;
            }
        }

        public List<SourceEdge> Edges
        {
            get
            {
                return _edges;
            }
            set
            {
                _edges = value;
            }
        }
    }
}
