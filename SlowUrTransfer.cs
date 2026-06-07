using System.Collections.Generic;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
namespace SLowUrTransfer
{
    public class SLowUrTransferSystem : ModSystem
    {
        private ICoreClientAPI capi;
        private const int CheckIntervalMs = 50;
        private const float SlowdownValue = -0.75f;
        private const string StatCode = "inventoryslowdown";
        private bool slowdownActive;
        private static readonly HashSet<string> InventoryDialogTypeNames = new HashSet<string>
        {
            "GuiDialogInventory",
            // "GuiDialogCharacter",
        };
        public override void StartClientSide(ICoreClientAPI api)
        {
            capi = api;
            capi.Event.RegisterGameTickListener(OnClientTick, CheckIntervalMs);
        }
        private void OnClientTick(float dt)
        {
            EntityPlayer player = capi.World?.Player?.Entity;
            if (player == null) return;
            bool inventoryOpen = false;
            foreach (object gui in capi.OpenedGuis)
            {
                if (gui is GuiDialog dlg && InventoryDialogTypeNames.Contains(dlg.GetType().Name))
                {
                    inventoryOpen = true;
                    break;
                }
            }
            if (inventoryOpen)
            {
                player.Stats.Set("walkspeed", StatCode, SlowdownValue, persistent: false);
                slowdownActive = true;
            }
            else if (slowdownActive)
            {
                player.Stats.Remove("walkspeed", StatCode);
                slowdownActive = false;
            }
        }
    }
}
