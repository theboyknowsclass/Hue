using TheBoyKnowsClass.Common.Models;
using TheBoyKnowsClass.Common.UI.WPF.Models;
using TheBoyKnowsClass.Common.UI.WPF.Modern.ViewModels;
using TheBoyKnowsClass.Hue.UI.Common.Models;
using TheBoyKnowsClass.Hue.UI.Common.ViewModels;
using TheBoyKnowsClass.Hue.UI.Desktop.Models;

namespace TheBoyKnowsClass.Hue.UI.Desktop.ViewModels
{
    public class HueDesktopAppViewModel : HueAppViewModel
    {
        public HueDesktopAppViewModel() : base(SettingsInitialiser.Initialise("Hue.UI.Desktop", LocalSettings.Default), new DelegateCommandFactory(), MessageHandlerViewModel.Instance)
        {
        }

        public static HueDesktopAppViewModel Instance
        {
            get
            {
                return InstanceCreator<HueDesktopAppViewModel>.Instance; 
            }
        }
    }
}
