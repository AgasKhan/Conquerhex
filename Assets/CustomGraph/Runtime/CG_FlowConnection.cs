using System;

namespace CustomGraph
{
    [Serializable]
    public struct CG_FlowConnection
    {
        public CG_FlowConnectionPort Input, Output;

        public CG_FlowConnection(CG_FlowConnectionPort input, CG_FlowConnectionPort output)
        {
            Input = input;
            Output = output;
        }

        public CG_FlowConnection(string inputID, int inputIndex, string outputID, int outputIndex)
        {
            Input = new(inputID, inputIndex);
            Output = new(outputID, outputIndex);
        }
    }

    [Serializable]
    public struct CG_FlowConnectionPort
    {
        public string ID;
        public int PortIndex;

        public CG_FlowConnectionPort(string id, int i)
        {
            ID = id;
            PortIndex = i;
        }
    }
}
