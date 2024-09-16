using System.Collections;
using System.Collections.Generic;
using CustomGraph;
using UnityEngine;

namespace CustomGraph
{
    [Info("DebugLog", "Debug/Log")]
    public class ND_DebugLogNode : CG_Node
    {
        [ExposedProperty()]
        public string message;

        public override string OnProcess(CG_AssetGraph graph)
        {
            Debug.Log(message);
            return base.OnProcess(graph);
        }
    }
}
