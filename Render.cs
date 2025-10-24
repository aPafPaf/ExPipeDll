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
                DrawWindow();
            }
        }
        private void DrawWindow()
        {
            ImGui.SetNextWindowSize(new System.Numerics.Vector2(0, 0));
            ImGui.SetNextWindowBgAlpha(0.6f);
            ImGui.Begin("Window", ImGuiWindowFlags.NoDecoration);

            if (ImGui.BeginTable("Table", 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersOuter | ImGuiTableFlags.BordersV))
            {
                ImGui.TableSetupColumn("Item", ImGuiTableColumnFlags.WidthFixed, 100);
                ImGui.TableSetupColumn("Button");

                foreach (var worldItem in entitiesWorldItems)
                {
                    if (!worldItem.TryGetComponent(out ExileCore.PoEMemory.Components.WorldItem componentWorldItem)) continue;
                    if (!componentWorldItem.ItemEntity.TryGetComponent(out Base componentBase)) continue;

                    ImGui.TableNextRow();

                    ImGui.TableNextColumn();

                    ImGui.Text(componentBase.Name);

                    ImGui.TableNextColumn();

                    if (ImGui.Button("Button"))
                    {
                        SendEntityId(worldItem.Id);
                    }
                }

                ImGui.EndTable();
            }

            ImGui.End();
        }
    }
}
