using System.Collections.Generic;

namespace Core_Scripts.UtilityScripts
{
    public class EquipmentUtil
    {
        public static Dictionary<OnDropType, GameItem> InitEquip()
        {
            return new Dictionary<OnDropType, GameItem>()
            {
                { OnDropType.HeadSlot, null },
                { OnDropType.ChestSlot, null },
                { OnDropType.LegSlot, null },
                { OnDropType.BootSlot, null },
                { OnDropType.LeftHandSlot, null },
                { OnDropType.RightHandSlot, null },
                { OnDropType.AmuletSlot, null },
                { OnDropType.RingSlot, null },
            };
        }
    }
}