using ExileCore.Shared.Attributes;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace ExPipeDll;

public class ExPipeDllSettings : ISettings
{
    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    public ToggleNode LootWindow { get; set; } = new ToggleNode(false);
    public LootClassSettings LootClassSettings { get; set; } = new ();
    public ButtonNode SendButton { get; set; } = new ButtonNode();
    public TextNode DebugEntityId { get; set; } = new TextNode("1");
    public HotkeyNode LootLoopHotKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.A);
    public RangeNode<int> DelayAddingPacket { get; set; } = new RangeNode<int>(100, 0, 500);
    public RangeNode<int> LootDIstance { get; set; } = new RangeNode<int>(20, 0, 500);
}

[Submenu(CollapsedByDefault = true)]
public class LootClassSettings
{
    public ToggleNode QuestItem { get; set; } = new ToggleNode(true);
    public ToggleNode StackableCurrency { get; set; } = new ToggleNode(true);
    public ToggleNode MapFragments { get; set; } = new ToggleNode(true); 

    public ButtonNode UpdateSettings { get; set; } = new ButtonNode();
}