﻿using System.Linq;
using System.Threading;
using FlaUI.Core.AutomationElements.Infrastructure;
using FlaUI.Core.AutomationElements.PatternElements;
using FlaUI.Core.Definitions;

namespace FlaUI.Core.AutomationElements
{
    /// <summary>
    /// Represents a menuitem which can also contain sub-menuitems
    /// </summary>
    public class MenuItem : AutomationElement
    {
        private readonly InvokeAutomationElement _invokeAutomationElement;
        private readonly ExpandCollapseAutomationElement _expandCollapseAutomationElement;

        public MenuItem(BasicAutomationElementBase basicAutomationElement) : base(basicAutomationElement)
        {
            _invokeAutomationElement = new InvokeAutomationElement(basicAutomationElement);
            _expandCollapseAutomationElement = new ExpandCollapseAutomationElement(basicAutomationElement);
        }

        /// <summary>
        /// Flag to indicate, if the containing menu is a Win32 context menu because that one needs special handling
        /// </summary>
        internal bool IsWin32ContextMenu { get; set; }

        public string Text => Properties.Name.Value;

        public MenuItem[] SubMenuItems
        {
            get
            {
                // Special handling for Win32 context menus
                if (IsWin32ContextMenu)
                {
                    // Click the item to load the child items
                    Click();
                    // In Win32, the nested menu items are below a menu control which is below the application window
                    // So search the app window first
                    var appWindow = BasicAutomationElement.Automation.GetDesktop().FindFirstChild(cf => cf.ByControlType(ControlType.Window).And(cf.ByProcessId(Properties.ProcessId)));
                    // Then search the menu below the window
                    var menu = appWindow.FindFirstChild(cf => cf.ByControlType(ControlType.Menu).And(cf.ByName(Text))).AsMenu();
                    menu.IsWin32ContextMenu = true;
                    // Now return the menu items
                    return menu.MenuItems;
                }
                // Expand if needed, WinForms does not have the expand pattern but all children are already visible so it works as well
                if (Patterns.ExpandCollapse.IsSupported)
                {
                    ExpandCollapseState state;
                    do
                    {
                        state = _expandCollapseAutomationElement.ExpandCollapseState;
                        if (state == ExpandCollapseState.Collapsed)
                        {
                            Expand();
                        }
                        Thread.Sleep(50);
                    } while (state != ExpandCollapseState.Expanded);
                }
                return FindAllChildren(cf => cf.ByControlType(ControlType.MenuItem)).Select(e => e.AsMenuItem()).ToArray();
            }
        }

        public void Invoke()
        {
            _invokeAutomationElement.Invoke();
        }

        public void Expand()
        {
            _expandCollapseAutomationElement.Expand();
        }

        public void Collapse()
        {
            _expandCollapseAutomationElement.Collapse();
        }
    }
}
