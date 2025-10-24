using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;

namespace ExPipeDll;

public class ExPipeDllSettings : ISettings
{    public ToggleNode Enable { get; set; } = new ToggleNode(false);
    public ButtonNode SendButton { get; set; } = new ButtonNode();
    public TextNode TextNode { get; set; } = new TextNode("1");
    public HotkeyNode LootLoopHotKey { get; set; } = new HotkeyNode(System.Windows.Forms.Keys.A);
    public RangeNode<int> DelayAddingPacket { get; set; } = new RangeNode<int>(100, 0, 5000);
    public RangeNode<int> LootDIstance { get; set; } = new RangeNode<int>(20, 0, 2000);
}