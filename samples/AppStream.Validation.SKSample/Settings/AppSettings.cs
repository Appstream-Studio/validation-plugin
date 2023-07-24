﻿namespace AppStream.Validation.SKSample.Settings;

internal sealed class AppSettings
{
    public KernelSettings Kernel { get; set; } = null!;
    public AIPluginSettings AIPlugin { get; set; } = null!;
}
