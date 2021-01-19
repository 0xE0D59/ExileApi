using System;
using System.Collections.Generic;
using System.Linq;
using ExileCore.PoEMemory.MemoryObjects;

namespace ExileCore.PoEMemory.Elements
{
    public class StashElement : Element
    {
        private int _indexVisibleStash;
        public long TotalStashes => StashInventoryPanel != null ? StashInventoryPanel.ChildCount : 0;
        public Element ExitButton => Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2B8)) : null;

        // Nice struct starts at 0xB80 till 0xBD0 and all are 8 byte long pointers.
        private Element StashTitlePanel => Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2C8, 0x5A8)) : null;
        private Element StashInventoryPanel => Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2C0, 0x230, 0x938)) : null;
        public Element ViewAllStashButton => Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2C0, 0x230, 0x930)) : null;
        public Element ViewAllStashPanel =>
            Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2C0, 0x230, 0x948)) : null; // going extra inside.

        //Not fixed
        public Element ButtonStashTabListPin => Address != 0 ? GetObject<Element>(M.Read<long>(Address + 0x2C0, 0x230, 0x950)) : null;
        public int IndexVisibleStash => M.Read<int>(Address + 0x2C0, 0x230, 0x9a0);
        public Inventory VisibleStash => GetVisibleStash();
        public IList<string> AllStashNames => GetAllStashNames();
        public IList<Inventory> AllInventories => GetAllInventories();

        public IList<Element> TabListButtons => GetTabListButtons();

        private Inventory GetVisibleStash()
        {
            return GetStashInventoryByIndex(IndexVisibleStash);
        }

        private List<string> GetAllStashNames()
        {
            var ret = new List<string>();

            for (var i = 0; i < TotalStashes; i++)
            {
                ret.Add(GetStashName(i));
            }

            return ret;
        }

        private IList<Inventory> GetAllInventories()
        {
            var result = new List<Inventory>();

            for (var i = 0; i < TotalStashes; i++)
            {
                result.Add(GetStashInventoryByIndex(i));
            }

            return result;
        }

        public Inventory GetStashInventoryByIndex(int index) //This one is correct
        {
            if (index >= TotalStashes) return null;
            if (index < 0) return null;
            if (StashInventoryPanel.Children[index].ChildCount == 0) return null;

            Inventory stashInventoryByIndex = null;

            try
            {
                stashInventoryByIndex = StashInventoryPanel.Children[index].Children[0].Children[0].AsObject<Inventory>();
            }
            catch
            {
                DebugWindow.LogError($"Not found inventory stash for index: {index}");
            }

            return stashInventoryByIndex;
        }

        public IList<Element> GetTabListButtons()
        {
            var listChild = ViewAllStashPanel.Children.FirstOrDefault(x => x.ChildCount == TotalStashes);
            return listChild?.Children ?? new List<Element>();
        }

        public string GetStashName(int index)
        {
            if (index >= TotalStashes || index < 0)
                return string.Empty;

            var temp = ViewAllStashPanel.Children.FirstOrDefault(x => x.ChildCount >= 4)?[index];
            return temp?.Children?.FirstOrDefault()?.Children?.LastOrDefault()?.Text ?? string.Empty;
        }
    }
}
