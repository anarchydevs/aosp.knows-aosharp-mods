﻿using System;
using System.Collections.Generic;
using System.Linq;
using AOSharp.Common.GameData;
using AOSharp.Core.GameData;
using AOSharp.Core.Imports;

namespace AOSharp.Core.UI.Options
{
    public static class OptionsPanel
    {
        private static List<Menu> _menus = new List<Menu>();

        public static Menu AddMenu(Menu menu)
        {
            _menus.Add(menu);
            return menu;
        }

        private unsafe static void OnOptionPanelActivatedInternal(IntPtr pOptionPanelModule, bool unk)
        {
            IntPtr pOptionWindow = OptionPanelModule_c.GetOptionWindow(pOptionPanelModule + 0xB8);

            if (pOptionWindow == IntPtr.Zero)
                return;

            IntPtr pViewSelector = *(IntPtr*)(pOptionWindow + 0x78);

            if (pViewSelector == IntPtr.Zero)
                return;

            ViewSelector viewSelector = ViewSelector.FromPointer(pViewSelector, false);

            foreach (Menu menu in _menus)
            {
                CreateMenu(menu, viewSelector);
            }
        }

        private unsafe static View CreateMenu(Menu menu, ViewSelector viewSelector)
        {
            View view = menu.CreateView();

            viewSelector.GetListView().AppendItem(StringListViewItem.Create(Variant.Create(view.Handle), menu.Name, 216, 0));
            viewSelector.AppendView(view);

            return view;
        }
    }
}
