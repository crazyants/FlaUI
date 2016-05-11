﻿using FlaUI.Core.Definitions;
using FlaUI.Core.Patterns;
using System;
using UIA = interop.UIAutomationCore;

namespace FlaUI.Core.Elements
{
    public class CheckBox : AutomationElement
    {
        public CheckBox(Automation automation, UIA.IUIAutomationElement nativeElement) : base(automation, nativeElement) { }

        public TogglePattern TogglePattern
        {
            get { return PatternFactory.GetTogglePattern(); }
        }

        public ToggleState State
        {
            get { return GetPropertyValue<ToggleState>(TogglePattern.ToggleStateProperty, false); }
            set
            {
                // Loop for all states
                for (int i = 0; i < Enum.GetNames(typeof(ToggleState)).Length; i++)
                {
                    // Break if we're in the correct state
                    if (State == value) break;
                    // Toggle to the next state
                    Toggle();
                }
            }
        }

        public virtual void Toggle()
        {
            var togglePattern = TogglePattern;
            if (togglePattern != null)
            {
                togglePattern.Toggle();
            }
        }
    }
}
