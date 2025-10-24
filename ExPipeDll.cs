using ExileCore;
using ExileCore.PoEMemory.MemoryObjects;
using ExileCore.Shared.Enums;
using SharpDX;
using System.Collections.Generic;
using System.Linq;
using Vector2 = System.Numerics.Vector2;

namespace ExPipeDll;

public partial class ExPipeDll : BaseSettingsPlugin<ExPipeDllSettings>
{
    List<Entity> entitiesWorldItems = new List<Entity>();
    public override bool Initialise()
    {
        Settings.SendButton.OnPressed = SendPacket;

        return true;
    }

    public override void AreaChange(AreaInstance area)
    {
        entitiesWorldItems.Clear();
    }

    public override Job Tick()
    {
        entitiesWorldItems.Clear();
        var worldItems = GameController.EntityListWrapper.ValidEntitiesByType[EntityType.WorldItem];
        entitiesWorldItems = worldItems.Where(x => x.IsValid && x.IsTargetable).ToList();

        if (Settings.LootLoopHotKey.IsPressed())
        {
            LootLoop();
        }

        return null;
    }
}