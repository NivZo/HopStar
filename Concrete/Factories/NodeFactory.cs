using Godot;

public class NodeFactory<TNode, TConfig>
    where TNode : class, IConfigurable<TConfig>, new()
{
    private Node _stage;

    public NodeFactory(Node stage)
    {
        _stage = stage;
    }

    public void ConfigureNode(TConfig config)
    {
        var configurable = new TNode()
        {
            Configuration = config,
        };

        configurable.Configure(_stage);
    }
}