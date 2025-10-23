# Alba.Avalonia.FlexPanel

> [!IMPORTANT]
>
> FlexPanel has been merged into official [AvaloniaUI Labs](https://github.com/AvaloniaUI/Avalonia.Labs):
>
> * AvaloniaUI/Avalonia.Labs#45

[![NuGet release version](https://img.shields.io/nuget/v/Alba.Avalonia.FlexPanel.svg?label=release&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAA4AAAAOCAYAAAAfSC3RAAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAAMpJREFUeNqUkbENwjAQRY8ovZkgG5AJYALS0GaDMAGeACYIE7ABtFSZgEyAy3TegG/pR7IuToCTXuHTvTufvTo8h42InIAFTn6MDARxCwr5I3JOuYAuyhdsqCPU%2BFHslHQD9cKwI7jmKtlSsrz%2BPiGGGhOLBjSgAg/m3jO7nzMlSrSbiXLJV510A3dOi0X/TRTuNko9KMGaayyKcVSUhbtbLfoZ0SX%2BciL2CbFR5/DPXl91l5jcsjg0eHH3UouecqfyNRsIH8p9BBgAANInlRmoOxQAAAAASUVORK5CYII=)](https://www.nuget.org/packages/Alba.Avalonia.FlexPanel)
[![AppVeyor build master](https://img.shields.io/appveyor/ci/athari/alba-avalonia-flexpanel/master.svg?logo=appveyor)](https://ci.appveyor.com/project/Athari/alba-avalonia-flexpanel/branch/master)
[![GitHub release version](https://img.shields.io/github/release/Athari/Alba.Avalonia.FlexPanel.svg?logo=github)](https://github.com/Athari/Alba.Avalonia.FlexPanel/releases)

Panel implementing CSS-like flexbox for Avalonia 11.

* [**GitHub repository**](https://github.com/Athari/Alba.Avalonia.FlexPanel)
* [**NuGet package**](https://www.nuget.org/packages/Alba.Avalonia.FlexPanel)
<!-- -->
    PM> dotnet add package Alba.Avalonia.FlexPanel

## Features

Flexbox features:
* direction: row, column, row reverse, column reverse
* flex-wrap: nowrap, wrap, wrap-reverse
* align-items, align-self: flex-start, flex-end, center, stretch
* align-content, justify-content: flex-start, flex-end, center, space-between, space-around, space-evenly
* flex-basis: content, length
* flex-grow, flex-shrink, order
* row-gap, column-gap

## Alternatives

* [AvaloniaUI/Avalonia.Labs (pull 45)](https://github.com/AvaloniaUI/Avalonia.Labs/pull/45) (modification of jp2masa's implementation):
    * Advantages:
        * Supports:
            * flex-basis: percent
            * margin:auto behavior on non-stretch alignment
            * partial main axis free space distribution
        * Fork of offical AvaloniaUI labs
        * Less bugs, better compatibility with the Flexbox standard
    * Disadvantages:
        * No NuGet package

* [AvaloniaUI/Avalonia.Labs](https://github.com/AvaloniaUI/Avalonia.Labs) (migrated jp2masa's implementation):
    * Advantages:
        * Offical AvaloniaUI labs
    * Disadvantages:
        * Doesn't support: flex-grow, flex-shrink, flex-basis
        * Non-standard calculation of gaps, alignment etc.

* [jp2masa/Avalonia.Flexbox](https://github.com/jp2masa/Avalonia.Flexbox) (alternative implementation):
    * Advantages:
        * Supports: FlexLayout for ItemsRepeater
    * Disadvantages:
        * Doesn't support: flex-grow, flex-shrink, flex-basis

* [dotNevereverlie/Avalonia.FlexPanel](https://github.com/dotNevereverlie/Avalonia.FlexPanel) (alternative port):
    * Disadvantages:
        * Doesn't support: Avalonia 11, row-gap, column-gap, space-evenly
        * Severe bugs

## Acknowledgements

Based on:

* [HandyOrg/HandyControl](https://github.com/HandyOrg/HandyControl)

* [jp2masa/Avalonia.Flexbox](https://github.com/jp2masa/Avalonia.Flexbox)