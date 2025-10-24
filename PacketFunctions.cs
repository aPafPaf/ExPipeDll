using ExileCore.PoEMemory.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExPipeDll
{
    public partial class ExPipeDll
    {
        public void LootLoop()
        {
            foreach (var entity in entitiesWorldItems)
            {
                if (!Settings.LootLoopHotKey.IsPressed()) return;

                if (!entity.TryGetComponent(out ExileCore.PoEMemory.Components.WorldItem componentWorldItem)) continue;
                if (!componentWorldItem.ItemEntity.TryGetComponent(out Base componentBase)) continue;
                if (entity.DistancePlayer > Settings.LootDIstance.Value) continue;
                if (componentBase.Info.BaseItemTypeDat.ClassName != "StackableCurrency") continue;

                SendEntityId(entity.Id);

                Thread.Sleep(Settings.DelayAddingPacket.Value);
            }
        }

        public void SendPacket()
        {
            if (!int.TryParse(Settings.TextNode.Value, out int entityId)) return;
            SendEntityId((uint)entityId);
        }
        public static void SendEntityId(uint entityId)
        {
            Task.Run(() =>
            {
                try
                {
                    using var client = new NamedPipeClientStream(".", "PoE_Pipe", PipeDirection.Out);
                    client.Connect(1000); // Wait Connect Pipe
                    using var writer = new BinaryWriter(client);
                    writer.Write(entityId);
                }
                catch
                {
                }
            });
        }
    }
}
