using Godot;

public interface IConfigurable<T>{
    T Configuration { get; set; }
    void Configure(Node parent);
}