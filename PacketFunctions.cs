using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.MemoryObjects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ExPipeDll
{
    public partial class ExPipeDll
    {
        // Constants
        private const int MaxZDifference = 100;
        private const int PacketTypeBitShift = 24;
        private const uint IdMask = 0xFFFFFF;

        public enum PacketType : byte
        {
            Interact = 1,
            GrabBeast = 7,
            DeleteBeast = 8,
            AcceptTrade = 20,
        }

        public Dictionary<uint, DateTime> LootSendTimestamps { get; private set; } = new();

        private bool CanSendLoot(uint id, int delayMs)
            => !LootSendTimestamps.TryGetValue(id, out var last) ||
               DateTime.Now - last > TimeSpan.FromMilliseconds(delayMs);

        private bool TryGetLootItem(Entity entity, out Base baseComponent)
        {
            baseComponent = null;

            if (!entity.TryGetComponent(out WorldItem worldComponent))
                return false;

            if (!worldComponent.ItemEntity.TryGetComponent(out baseComponent))
                return false;

            return true;
        }

        private bool IsLootAllowed(Base baseComponent)
            => lootClasses.TryGetValue(baseComponent.Info.BaseItemTypeDat.ClassName, out bool allowed) && allowed;

        private bool IsLootInRange(Entity entity)
            => entity.DistancePlayer <= Settings.LootDistance.Value;

        private bool IsLootAtSameHeight(Entity entity)
            => Math.Abs(GameController.Player.PosNum.Z - entity.PosNum.Z) <= MaxZDifference;

        public void LootLoop()
        {
            foreach (var entity in entitiesWorldItems)
            {
                if (!Settings.LootLoopHotKey.IsPressed())
                    return;

                if (!TryGetLootItem(entity, out var baseComponent))
                    continue;

                if (!IsLootInRange(entity))
                    continue;

                if (!IsLootAllowed(baseComponent))
                    continue;

                if (!IsLootAtSameHeight(entity))
                    continue;

                if (!CanSendLoot(entity.Id, Settings.DelayAddingPacket.Value))
                    continue;

                EnqueuePacket(PacketType.Interact, entity.Id);
            }
        }

        public void DebugSendEntityId()
        {
            if (!int.TryParse(Settings.DebugEntityId.Value, out int entityId))
                return;

            var isLoading = GameController.Game.LoadingState.IsLoading;
            var isConnected = GameController.IngameState.ServerData.NetworkState == ExileCore.Shared.Enums.NetworkStateE.Connected;
            
            if (!isConnected)
            {
                LogMessage("[DebugSendEntityId] Not connected to server");
                return;
            }

            if (isLoading)
            {
                LogMessage("[DebugSendEntityId] Game is loading");
                return;
            }

            EnqueuePacket(PacketType.Interact, (uint)entityId);
        }

        public static void SendEntityId(uint entityId)
        {
            var msg = PackMessage(PacketType.Interact, entityId);
            PipeConnector.SendMessage(msg);
        }

        public void SendPacket(PacketType type, uint id)
        {
            var msg = PackMessage(type, id);
            PipeConnector.SendMessage(msg);
        }

        public static uint PackMessage(PacketType type, uint id)
        {
            return ((uint)type << PacketTypeBitShift) | (id & IdMask);
        }

        private void EnqueuePacket(PacketType type, uint id)
            => LootPacketQueue.Enqueue((id, type));

        private void HandleOutgoingPacket()
        {
            if (LootPacketQueue.Count == 0)
                return;

            // Preserve queue order while searching for the first sendable packet.
            var items = new List<(uint id, PacketType type)>();
            while (LootPacketQueue.Count > 0)
                items.Add(LootPacketQueue.Dequeue());

            int sendIndex = -1;
            for (int i = 0; i < items.Count; i++)
            {
                if (CanSendLoot(items[i].id, Settings.DelayAddingPacket.Value))
                {
                    sendIndex = i;
                    break;
                }
            }

            if (sendIndex == -1)
            {
                // No packet is ready to be sent — restore queue in original order.
                foreach (var it in items)
                    LootPacketQueue.Enqueue(it);
                return;
            }

            // Send the ready packet and record timestamp.
            var toSend = items[sendIndex];
            SendPacket(toSend.type, toSend.id);
            LootSendTimestamps[toSend.id] = DateTime.Now;

            // Re-enqueue remaining items in original order (excluding the sent one).
            for (int i = 0; i < items.Count; i++)
            {
                if (i == sendIndex) continue;
                LootPacketQueue.Enqueue(items[i]);
            }
        }
    }
}
