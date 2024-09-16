
using CustomGraph;
using UnityEngine;


namespace CustomGraph
{
    [Info("StartNode", "Process/Start", false, true)]
    public class ND_StartNode : CG_Node
    {
        public override string OnProcess(CG_AssetGraph graph)
        {
            Debug.Log("Started");
            return base.OnProcess(graph);
        }
    }

}