﻿using Known.Platform;

namespace Known.Web.ViewModels
{
    public class ViewModel
    {
        public ViewModel(Module module)
        {
            Module = module;
        }

        public Module Module { get; }

        public string JsPath
        {
            get
            {
                if (Module.FullCodes == null || Module.FullCodes.Count == 0)
                    return Module.Code.ToLower();

                return string.Join("/", Module.FullCodes).ToLower();
            }
        }
    }
}