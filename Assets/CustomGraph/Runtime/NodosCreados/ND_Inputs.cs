
using CustomGraph;
using UnityEngine;


namespace CustomGraph
{
    [Info("Start", "Process/On Start", false)]
    public class ND_OnStart : CG_Node
    {
        public override string OnProcess(CG_AssetGraph graph)
        {
            return base.OnProcess(graph);
        }
    }

    [Info("Update", "Process/On Update", false)]
    public class ND_OnUpdate : CG_Node
    {
        public override string OnProcess(CG_AssetGraph graph)
        {
            return base.OnProcess(graph);
        }
    }

    [Info("Exit", "Process/On Exit", false)]
    public class ND_OnExit : CG_Node
    {
        public override string OnProcess(CG_AssetGraph graph)
        {
            return base.OnProcess(graph);
        }
    }
}