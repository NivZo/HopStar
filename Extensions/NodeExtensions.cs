using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class NodeExtensions
{
    public static IEnumerable<Node> GetChildrenRecursive(this Node node)
    {
        var cands = node.GetChildren();
        
        if (cands.Count() == 0)
        {
            return Enumerable.Empty<Node>();
        }

        return cands.Concat(cands.SelectMany(cand => cand.GetChildrenRecursive()));
    }

    public static IEnumerable<Node> GetChildrenByName(this Node node, string name)
    {
        return node.GetChildrenRecursive()
            .Where(child => child.Name == name);
    }

    public static Node GetChildByName(this Node node, IConvertible name)
    {
        var cands = node.GetChildrenByName(name.ToString());
        if (cands.Count() != 1)
        {
            throw new ArgumentException($"Found {cands.Count()} != 1 children with the name {name}, under {node.Name}.");
        }

        return cands.First();
    }

    public static IEnumerable<T> GetChildrenByType<T>(this Node node)
        where T : Node
    {
        return node.GetChildrenRecursive()
            .Where(child => child is T)
            .Select(child => child as T);
    }

    public static T GetChildByType<T>(this Node node)
        where T : Node
    {
        var cands = node.GetChildrenByType<T>();
        if (cands.Count() != 1)
        {
            throw new ArgumentException($"Found {cands.Count()} != 1 children with the type {typeof(T)}, under {node.Name}.");
        }

        return cands.First();
    }

    public static IEnumerable<TNode> GetTypedChildrenByName<TNode>(this Node node, IConvertible name = null)
        where TNode : Node
    {
        var sName = name?.ToString() ?? typeof(TNode).FullName;
        var cands = node.GetChildrenByName(sName)
            ?.Where(child => child is TNode)
            .Select(child => child as TNode);

        return cands;
    }

    public static TNode GetTypedChildByName<TNode>(this Node node, IConvertible name = null)
        where TNode : Node
    {
        var cands = GetTypedChildrenByName<TNode>(node, name);

        if (cands.Count() != 1)
        {
            throw new ArgumentException($"Found {cands.Count()} != 1 children with the name {typeof(TNode).FullName}, under {node.Name}.");
        }

        return cands.First();
    }

    public static TNode GetTypedSiblingByName<TNode>(this Node node, IConvertible name = null)
        where TNode : Node
        => node.GetParent().GetTypedChildByName<TNode>(name);

    public static bool IsValid(this Node node) => GodotObject.IsInstanceValid(node) && !node.IsQueuedForDeletion();


    public static Explosion AddExplosion(this Node2D node, ParticleConstants.ExplosionType explosionType, Vector2? overridePosition = null, float? overrideRotation = null, float scale = 1)
    {
        var explosion = GD.Load<PackedScene>(Explosion.ScenePath).Instantiate<Explosion>();
        explosion.Animation = explosionType.ToString();
        explosion.Scale = Vector2.One.Mult(scale);
        explosion.GlobalPosition = overridePosition ?? node.GlobalPosition;
        explosion.GlobalRotation = overrideRotation ?? node.GlobalRotation;

        node.GetTree().Root.AddChild(explosion);
        return explosion;
    }

    public static void AddScrap(this Node2D node, Vector2? overridePosition = null)
    {
        var scrap = GD.Load<PackedScene>(Scrap.ScenePath).Instantiate<Scrap>();
        scrap.GlobalPosition = overridePosition ?? node.GlobalPosition;

        node.GetStageNode().AddChild(scrap);
    }

    public static TNode GetAutoload<TNode>(this Node node, IConvertible name = null)
        where TNode : Node, IAutoLoad
    {
        return node.GetTree().Root.GetTypedChildByName<TNode>(name);
    }

    private static Node GetAutoloadInternal(this Node node, IConvertible name)
    {
        return node.GetTree().Root.GetChildByName(name);
    }

    public static Game GetGame(this Node node) => node.GetTree().Root.GetNode<Game>("/root/Game");

    public static Stage GetStage(this Node node) => node.GetAutoload<SceneService>().GetCurrentStage();
    
    public static Stage GetStageNode(this Node node) => node.GetAutoload<SceneService>().GetCurrentStage();

    public static PlayerController GetPlayerController(this Node node) =>
        node.GetGame()
            .GetTypedChildByName<PlayerController>();

    public static CombatantBody GetPlayerShipBody(this Node node) => 
        node.GetGame()
            .GetTypedChildByName<Player>()
            .GetTypedChildByName<CombatantBody>();
    
    public static Player GetPlayer(this Node node) => 
        node.GetGame()
            .GetTypedChildByName<Player>();

    public static bool IsChildOf(this Node node, Node parent)
    {
        var cands = parent?.GetChildrenByName(node.Name)?.Where(child => child == node);

        if (cands != null)
        {
            return cands.Count() > 0;
        }

        return false;
    }

    public static void InitOnReady<T>(this T node, Node overrideParent = null)
        where T : Node
    {
        var fields = typeof(T).GetFields();
        foreach (var field in fields)
        {
            var attributes = (OnReadyAttribute[])field.GetCustomAttributes(typeof(OnReadyAttribute), true);
            if (attributes.Length > 0)
            {
                var attr = attributes[0];
                if (attr.IsAutoload && field.FieldType.GetType().IsInstanceOfType(typeof(IAutoLoad)))
                {
                    var autoloadRawName = field.ToString().Split(' ')[0];
                    var autoloadName = field.FieldType.IsInterface ? autoloadRawName.Substring(1, autoloadRawName.Length-1) : autoloadRawName;
                    field.SetValue(node, node.GetAutoloadInternal(autoloadName));
                }
                else
                {
                    var childName = attr?.Name?.ToString() ?? field.Name;
                    var parent = overrideParent ?? node;
                    var child = parent.GetChildByName(childName);
                    field.SetValue(node, child);
                }
            }
        }
    }
}