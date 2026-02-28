using ExileCore.PoEMemory.Components;
using ImGuiNET;

namespace ExPipeDll
{
    public partial class ExPipeDll
    {
        public override void Render()
        {
            if (Settings.LootWindow.Value)
            {
                DrawLootWindow();
            }
        }

        private void DrawLootWindow()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowBgAlpha(0.6f);
            ImGui.Begin("Loot Items", ImGuiWindowFlags.NoDecoration);

            if (ImGui.BeginTable("LootTable", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersV))
            {
                ImGui.TableSetupColumn("Item Name", ImGuiTableColumnFlags.WidthFixed, 150);
                ImGui.TableSetupColumn("Action");

                foreach (var worldItem in entitiesWorldItems)
                {
                    if (!TryGetLootItem(worldItem, out var baseComponent))
                        continue;

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();
                    ImGui.Text(baseComponent.Name);

                    ImGui.TableNextColumn();
                    if (ImGui.Button($"Pick##Button_{worldItem.Id}"))
                    {
                        EnqueuePacket(PacketType.Interact, worldItem.Id);
                    }
                }

                ImGui.EndTable();
            }

            ImGui.End();
        }
    }
}
