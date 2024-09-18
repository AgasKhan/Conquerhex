using CustomGraph;

public class ND_Dialogue : CG_Node
{
    [ExposedProperty()]
    public string Dialogue;

    UI.TextCompleto dialogText;

    public override string GetNextNode(CG_AssetGraph graph)
    {
        dialogText = UI.Interfaz.SearchTitle("Subtitulo");
        dialogText.ClearMsg();
        dialogText.AddMsg(Dialogue);
        
        return base.GetNextNode(graph);
    }

    
}

