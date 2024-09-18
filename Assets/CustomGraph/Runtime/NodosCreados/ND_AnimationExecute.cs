
using CustomGraph;
using UnityEngine;
using UnityEngine.Events;

[Info("Animation", "Animations/New Animation")]
public class ND_AnimationExecute : CG_Node
{
    [ExposedProperty()]
    public UnityEvent s;
    public override string OnProcess(CG_AssetGraph graph)
    {
        //Ejecutar animacion
        return base.GetNextNode(graph);
    }
}
