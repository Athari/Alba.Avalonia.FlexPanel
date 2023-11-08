# Alba.Avalonia.FlexPanel

Panel implementing CSS-like flexbox for Avalonia 11.

## Features

Flexbox features:
* direction: row, column, row reverse, column reverse
* flex-wrap: nowrap, wrap, wrap-reverse
* align-items, align-self: flex-start, flex-end, center, stretch
* align-content, justify-content: flex-start, flex-end, center, space-between, space-around, space-evenly
* flex-grow, flex-shrink, flex-basis, order
* row-gap, column-gap

## Alternatives

* [jp2masa/Avalonia.Flexbox](https://github.com/jp2masa/Avalonia.Flexbox) (alternative implementation):
    * Advantages:
        * Supports: FlexLayout for ItemsRepeater
    * Disadvantages:
        * Doesn't support: flex-grow, flex-shrink, flex-basis

* [dotNevereverlie/Avalonia.FlexPanel](https://github.com/dotNevereverlie/Avalonia.FlexPanel) (alternative port):
    * Disadvantages:
        * Doesn't support: Avalonia 11, space-evenly
        * Severe bugs

## Acknowledgements

Based on:

* [HandyOrg/HandyControl](https://github.com/HandyOrg/HandyControl)

* [jp2masa/Avalonia.Flexbox](https://github.com/jp2masa/Avalonia.Flexbox)