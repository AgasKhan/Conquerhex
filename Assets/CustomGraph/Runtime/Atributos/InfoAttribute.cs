using System;

namespace CustomGraph
{
    
    /// <summary>
    /// Atributo para asignar o setear un nodo. Lleva titulo y si tiene o no input o output.
    /// </summary>
    public class InfoAttribute : Attribute
    {
        string _title;
        string _item;
        bool _input;
        bool _output;
        
        public string Title => _title;
        public string MenuItem => _item;

        public bool hasFlowInput => _input;
        public bool hasFlowOutput => _output;

        /// <summary>
        /// Atributo para setear un nodo.
        /// </summary>
        /// <param name="title">Titulo del nodo</param>
        /// <param name="item"></param>
        /// <param name="hasFlowInput">Si el nodo recibe un input</param>
        /// <param name="hasFlowOutput">Si el nodo recibe un output</param>
        public InfoAttribute(string title, string item = "", bool hasFlowInput = true, bool hasFlowOutput = true)
        {
            _title = title;
            _item = item;
            _input = hasFlowInput;
            _output = hasFlowOutput;
        }
    }
}
