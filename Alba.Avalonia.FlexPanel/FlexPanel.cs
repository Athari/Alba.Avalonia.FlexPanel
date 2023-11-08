using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;

namespace Alba.Avalonia.FlexPanel;

[SuppressMessage("ReSharper", "RedundantArgumentDefaultValue")]
public sealed partial class FlexPanel : Panel
{
    private UVSize _uvConstraint;

    private int _lineCount;

    private readonly List<FlexItemInfo> _orderList = new();

    public static readonly AttachedProperty<int> OrderProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, int>("Order", 0);
    public static readonly AttachedProperty<double> GrowProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, double>("FlexGrow", 0.0d, validate: IsInRangeOfPosDoubleIncludeZero);
    public static readonly AttachedProperty<double> ShrinkProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, double>("FlexShrink", 1.0d, validate: IsInRangeOfPosDoubleIncludeZero);
    public static readonly AttachedProperty<double> BasisProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, double>("FlexBasis", double.NaN);
    public static readonly AttachedProperty<FlexItemAlignment> AlignSelfProperty =
        AvaloniaProperty.RegisterAttached<FlexPanel, Layoutable, FlexItemAlignment>("AlignSelf", FlexItemAlignment.Auto);
    public static readonly StyledProperty<FlexDirection> DirectionProperty =
        AvaloniaProperty.Register<FlexPanel, FlexDirection>(nameof(Direction), FlexDirection.Row);
    public static readonly StyledProperty<FlexWrap> WrapProperty =
        AvaloniaProperty.Register<FlexPanel, FlexWrap>(nameof(Wrap), FlexWrap.NoWrap);
    public static readonly StyledProperty<FlexContentJustify> JustifyContentProperty =
        AvaloniaProperty.Register<FlexPanel, FlexContentJustify>(nameof(JustifyContent), FlexContentJustify.FlexStart);
    public static readonly StyledProperty<FlexItemsAlignment> AlignItemsProperty =
        AvaloniaProperty.Register<FlexPanel, FlexItemsAlignment>(nameof(AlignItems), FlexItemsAlignment.Stretch);
    public static readonly StyledProperty<FlexContentAlignment> AlignContentProperty =
        AvaloniaProperty.Register<FlexPanel, FlexContentAlignment>(nameof(AlignContent), FlexContentAlignment.Stretch);
    public static readonly StyledProperty<double> RowGapProperty =
        AvaloniaProperty.Register<FlexPanel, double>(nameof(RowGap), 0.0d);
    public static readonly StyledProperty<double> ColumnGapProperty =
        AvaloniaProperty.Register<FlexPanel, double>(nameof(ColumnGap), 0.0d);

    static FlexPanel()
    {
        AffectsMeasure<FlexPanel>(DirectionProperty, WrapProperty, JustifyContentProperty, ColumnGapProperty, RowGapProperty);
        AffectsArrange<FlexPanel>(AlignItemsProperty, AlignContentProperty);
        AffectsParentMeasure<FlexPanel>(OrderProperty, GrowProperty, ShrinkProperty, BasisProperty);
        AffectsParentArrange<FlexPanel>(AlignSelfProperty);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        var flexDirection = Direction;
        var flexWrap = Wrap;

        var curLineSize = new UVSize(flexDirection);
        var panelSize = new UVSize(flexDirection);
        var gap = new UVSize(flexDirection, new(ColumnGap, RowGap));
        _uvConstraint = new UVSize(flexDirection, constraint);
        var childConstraint = new Size(constraint.Width, constraint.Height);
        _lineCount = 1;

        _orderList.Clear();
        for (var i = 0; i < Children.Count; i++)
            _orderList.Add(new(i, GetOrder(Children[i])));
        _orderList.Sort();

        var itemIndex = 0;

        for (var i = 0; i < Children.Count; i++) {
            var child = Children[_orderList[i].Index];
            var flexBasis = GetBasis(child);
            if (!double.IsNaN(flexBasis))
                child.SetCurrentValue(WidthProperty, flexBasis);
            child.Measure(childConstraint);
            var sz = new UVSize(flexDirection, child.DesiredSize);
            if (itemIndex > 0)
                sz.U += gap.U;

            if (flexWrap == FlexWrap.NoWrap) //continue to accumulate a line
            {
                itemIndex++;
                curLineSize.U += sz.U;
                curLineSize.V = Math.Max(sz.V, curLineSize.V);
            }
            else {
                if (MathHelper.GreaterThan(curLineSize.U + sz.U, _uvConstraint.U)) //need to switch to another line
                {
                    panelSize.U = Math.Max(curLineSize.U, panelSize.U);
                    panelSize.V += curLineSize.V;
                    itemIndex = 0;
                    curLineSize = sz;
                    _lineCount++;

                    if (MathHelper.GreaterThan(sz.U, _uvConstraint.U)) //the element is wider then the constrint - give it a separate line
                    {
                        panelSize.U = Math.Max(sz.U, panelSize.U);
                        panelSize.V += sz.V;
                        curLineSize = new UVSize(flexDirection);
                        _lineCount++;
                    }
                }
                else //continue to accumulate a line
                {
                    itemIndex++;
                    curLineSize.U += sz.U;
                    curLineSize.V = Math.Max(sz.V, curLineSize.V);
                }
            }
        }

        //the last line size, if any should be added
        panelSize.U = Math.Max(curLineSize.U, panelSize.U);
        panelSize.V += curLineSize.V + (_lineCount - 1) * gap.V;

        //go from UV space to W/H space
        return new Size(panelSize.Width, panelSize.Height);
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        if (MathHelper.IsZero(arrangeSize.Width) || MathHelper.IsZero(arrangeSize.Height))
            return arrangeSize;

        var flexDirection = Direction;
        var flexWrap = Wrap;
        var alignContent = AlignContent;
        var uvFinalSize = new UVSize(flexDirection, arrangeSize);

        // init status
        var gap = new UVSize(flexDirection, new(ColumnGap, RowGap));
        var curLineSizeArr = new UVSize[_lineCount];
        curLineSizeArr[0] = new UVSize(flexDirection);

        var lastInLineArr = new int[_lineCount];
        for (var i = 0; i < _lineCount; i++)
            lastInLineArr[i] = int.MaxValue;

        var lineIndex = 0;
        var itemIndex = 0;

        // calculate line max U space
        for (var i = 0; i < Children.Count; i++) {
            var child = Children[_orderList[i].Index];
            var sz = new UVSize(flexDirection, child.DesiredSize);
            if (itemIndex > 0)
                sz.U += gap.U;

            if (flexWrap == FlexWrap.NoWrap) {
                itemIndex++;
                curLineSizeArr[lineIndex].U += sz.U;
                curLineSizeArr[lineIndex].V = Math.Max(sz.V, curLineSizeArr[lineIndex].V);
            }
            else {
                if (MathHelper.GreaterThan(curLineSizeArr[lineIndex].U + sz.U, uvFinalSize.U)) //need to switch to another line
                {
                    lastInLineArr[lineIndex] = i;
                    itemIndex = 0;
                    lineIndex++;
                    curLineSizeArr[lineIndex] = sz;

                    if (MathHelper.GreaterThan(sz.U, uvFinalSize.U)) //the element is wider then the constraint - give it a separate line
                    {
                        //switch to next line which only contain one element
                        lastInLineArr[lineIndex] = i;
                        lineIndex++;
                        curLineSizeArr[lineIndex] = new UVSize(flexDirection);
                    }
                }
                else //continue to accumulate a line
                {
                    itemIndex++;
                    curLineSizeArr[lineIndex].U += sz.U;
                    curLineSizeArr[lineIndex].V = Math.Max(sz.V, curLineSizeArr[lineIndex].V);
                }
            }
        }

        // init status
        var scaleU = Math.Min(_uvConstraint.U / uvFinalSize.U, 1); // TODO ? The fuck is this scaling?
        var firstInLine = 0;
        var wrapReverseAdd = 0;
        var wrapReverseFlag = flexWrap == FlexWrap.WrapReverse ? -1 : 1;
        var accumulatedFlag = flexWrap == FlexWrap.WrapReverse ? 1 : 0;
        var itemsU = 0.0;
        var accumulatedV = 0.0;
        // This leads to essentially forced align-content=flex-start when overflowing.
        // Original CSS layout system makes top items disappear in some modes, which is even worse.
        // TODO Fix wrap-reverse mode in overflow state.
        var freeV = Math.Max(uvFinalSize.V - curLineSizeArr.Sum(l => l.V) - (_lineCount - 1) * gap.V, 0);
        var freeItemV = freeV;
        var lineFreeVArr = new double[_lineCount];
        for (var i = 0; i < _lineCount - 1; i++)
            lineFreeVArr[i] = gap.V;

        // calculate status
        switch (alignContent) {
            case FlexContentAlignment.Stretch:
                if (_lineCount > 1) {
                    freeItemV = freeV / _lineCount;
                    for (var i = 0; i < _lineCount; i++)
                        curLineSizeArr[i].V += freeItemV;
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V : 0;
                }
                break;
            case FlexContentAlignment.FlexStart:
                wrapReverseAdd = flexWrap == FlexWrap.WrapReverse ? -1 : 0;
                if (_lineCount > 1)
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V : 0;
                else
                    wrapReverseAdd = 0;
                break;
            case FlexContentAlignment.FlexEnd:
                wrapReverseAdd = flexWrap == FlexWrap.WrapReverse ? 1 : 0;
                if (_lineCount > 1)
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V - freeV : freeV;
                else
                    wrapReverseAdd = 0;
                break;
            case FlexContentAlignment.Center:
                if (_lineCount > 1)
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V - freeV * 0.5 : freeV * 0.5;
                break;
            case FlexContentAlignment.SpaceBetween:
                if (_lineCount > 1) {
                    freeItemV = freeV / (_lineCount - 1);
                    for (var i = 0; i < _lineCount - 1; i++)
                        lineFreeVArr[i] += freeItemV;
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V : 0;
                }
                break;
            case FlexContentAlignment.SpaceAround:
                if (_lineCount > 1) {
                    freeItemV = freeV / _lineCount * 0.5;
                    for (var i = 0; i < _lineCount - 1; i++)
                        lineFreeVArr[i] += freeItemV * 2;
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V - freeItemV : freeItemV;
                }
                break;
            case FlexContentAlignment.SpaceEvenly:
                if (_lineCount > 1) {
                    freeItemV = freeV / (_lineCount + 1);
                    for (var i = 0; i < _lineCount - 1; i++)
                        lineFreeVArr[i] += freeItemV;
                    accumulatedV = flexWrap == FlexWrap.WrapReverse ? uvFinalSize.V - curLineSizeArr[0].V - freeItemV : freeItemV;
                }
                break;
        }

        // clear status
        lineIndex = 0;

        // arrange line
        for (var i = 0; i < Children.Count; i++) {
            var child = Children[_orderList[i].Index];
            var sz = new UVSize(flexDirection, child.DesiredSize);

            if (flexWrap != FlexWrap.NoWrap) {
                if (i >= lastInLineArr[lineIndex]) //need to switch to another line
                {
                    ArrangeLine(new() {
                        ItemsU = itemsU,
                        OffsetV = accumulatedV + freeItemV * wrapReverseAdd,
                        LineV = curLineSizeArr[lineIndex].V,
                        LineFreeV = freeItemV,
                        LineU = uvFinalSize.U,
                        ItemStartIndex = firstInLine,
                        ItemEndIndex = i,
                        ScaleU = scaleU,
                    });

                    accumulatedV += (lineFreeVArr[lineIndex] + curLineSizeArr[lineIndex + accumulatedFlag].V) * wrapReverseFlag;
                    lineIndex++;
                    itemsU = 0;

                    if (i >= lastInLineArr[lineIndex]) //the element is wider then the constraint - give it a separate line
                    {
                        //switch to next line which only contain one element
                        ArrangeLine(new() {
                            ItemsU = itemsU,
                            OffsetV = accumulatedV + freeItemV * wrapReverseAdd,
                            LineV = curLineSizeArr[lineIndex].V,
                            LineFreeV = freeItemV,
                            LineU = uvFinalSize.U,
                            ItemStartIndex = i,
                            ItemEndIndex = ++i,
                            ScaleU = scaleU,
                        });

                        accumulatedV += (lineFreeVArr[lineIndex] + curLineSizeArr[lineIndex + accumulatedFlag].V) * wrapReverseFlag;
                        lineIndex++;
                        itemsU = 0;
                    }

                    firstInLine = i;
                }
            }

            itemsU += sz.U;
        }

        // arrange the last line, if any
        if (firstInLine < Children.Count) {
            ArrangeLine(new() {
                ItemsU = itemsU,
                OffsetV = accumulatedV + freeItemV * wrapReverseAdd,
                LineV = curLineSizeArr[lineIndex].V,
                LineFreeV = freeItemV,
                LineU = uvFinalSize.U,
                ItemStartIndex = firstInLine,
                ItemEndIndex = Children.Count,
                ScaleU = scaleU,
            });
        }

        return arrangeSize;
    }

    private void ArrangeLine(FlexLineInfo lineInfo)
    {
        var flexDirection = Direction;
        var flexWrap = Wrap;
        var justifyContent = JustifyContent;
        var alignItems = AlignItems;
        var gap = new UVSize(flexDirection, new(ColumnGap, RowGap));

        var isHorizontal = flexDirection is FlexDirection.Row or FlexDirection.RowReverse;
        var isReverse = flexDirection is FlexDirection.RowReverse or FlexDirection.ColumnReverse;
        var itemCount = lineInfo.ItemEndIndex - lineInfo.ItemStartIndex;
        var totalGapU = (itemCount - 1) * gap.U;
        var lineFreeU = lineInfo.LineU - lineInfo.ItemsU - totalGapU;
        var constraintFreeU = _uvConstraint.U - lineInfo.ItemsU - totalGapU;

        // calculate initial u
        var u = 0.0;
        if (isReverse) {
            u = justifyContent switch {
                FlexContentJustify.FlexStart => lineInfo.LineU,
                FlexContentJustify.SpaceBetween => lineInfo.LineU,
                FlexContentJustify.SpaceAround => lineInfo.LineU,
                FlexContentJustify.SpaceEvenly => lineInfo.LineU,
                FlexContentJustify.FlexEnd => lineInfo.ItemsU,
                FlexContentJustify.Center => (lineInfo.LineU + lineInfo.ItemsU) * 0.5,
                _ => u,
            };
        }
        else {
            u = justifyContent switch {
                FlexContentJustify.FlexEnd => lineFreeU,
                FlexContentJustify.Center => lineFreeU * 0.5,
                _ => u,
            };
        }

        u *= lineInfo.ScaleU;

        // apply FlexGrow
        var flexGrowUArr = new double[itemCount];
        if (constraintFreeU > 0) {
            var ignoreFlexGrow = true;
            var flexGrowSum = 0.0;

            for (var i = 0; i < itemCount; i++) {
                var flexGrow = GetGrow(Children[_orderList[lineInfo.ItemStartIndex + i].Index]);
                ignoreFlexGrow &= MathHelper.IsVerySmall(flexGrow);
                flexGrowUArr[i] = flexGrow;
                flexGrowSum += flexGrow;
            }

            if (!ignoreFlexGrow) {
                var flexGrowItem = 0.0;
                if (flexGrowSum > 0) {
                    flexGrowItem = constraintFreeU / flexGrowSum;
                    lineInfo.ScaleU = 1;
                    lineFreeU = 0; //line free U was used up
                }
                for (var i = 0; i < itemCount; i++)
                    flexGrowUArr[i] *= flexGrowItem;
            }
            else {
                flexGrowUArr = new double[itemCount];
            }
        }

        // apply FlexShrink
        var flexShrinkUArr = new double[itemCount];
        if (constraintFreeU < 0) {
            var ignoreFlexShrink = true;
            var flexShrinkSum = 0.0;

            for (var i = 0; i < itemCount; i++) {
                var flexShrink = GetShrink(Children[_orderList[lineInfo.ItemStartIndex + i].Index]);
                ignoreFlexShrink &= MathHelper.IsVerySmall(flexShrink - 1);
                flexShrinkUArr[i] = flexShrink;
                flexShrinkSum += flexShrink;
            }

            if (!ignoreFlexShrink) {
                var flexShrinkItem = 0.0;
                if (flexShrinkSum > 0) {
                    flexShrinkItem = constraintFreeU / flexShrinkSum;
                    lineInfo.ScaleU = 1;
                    lineFreeU = 0; //line free U was used up
                }
                for (var i = 0; i < itemCount; i++)
                    flexShrinkUArr[i] *= flexShrinkItem;
            }
            else {
                flexShrinkUArr = new double[itemCount];
            }
        }

        // calculate offset u
        var offsetUArr = new double[itemCount];
        if (lineFreeU > 0) {
            if (justifyContent == FlexContentJustify.SpaceBetween) {
                var freeItemU = lineFreeU / (itemCount - 1);
                for (var i = 1; i < itemCount; i++)
                    offsetUArr[i] = freeItemU;
            }
            else if (justifyContent == FlexContentJustify.SpaceAround) {
                var freeItemU = lineFreeU / itemCount * 0.5;
                offsetUArr[0] = freeItemU;
                for (var i = 1; i < itemCount; i++)
                    offsetUArr[i] = freeItemU * 2;
            }
            else if (justifyContent == FlexContentJustify.SpaceEvenly) {
                var freeItemU = lineFreeU / (itemCount + 1);
                offsetUArr[0] = freeItemU;
                for (var i = 1; i < itemCount; i++)
                    offsetUArr[i] = freeItemU;
            }
        }

        // arrange item
        for (int i = lineInfo.ItemStartIndex, j = 0; i < lineInfo.ItemEndIndex; i++, j++) {
            var child = Children[_orderList[i].Index];
            var childSize = new UVSize(flexDirection, isHorizontal
                ? new Size(child.DesiredSize.Width * lineInfo.ScaleU, child.DesiredSize.Height)
                : new Size(child.DesiredSize.Width, child.DesiredSize.Height * lineInfo.ScaleU));

            childSize.U += flexGrowUArr[j] + flexShrinkUArr[j];

            var gapU = j > 0 ? gap.U : 0;
            if (isReverse) {
                u -= childSize.U;
                u -= offsetUArr[j] - gapU;
            }
            else {
                u += offsetUArr[j] + gapU;
            }

            var v = lineInfo.OffsetV;
            var alignSelf = GetAlignSelf(child);
            var alignment = alignSelf == FlexItemAlignment.Auto ? alignItems : (FlexItemsAlignment)alignSelf;

            switch (alignment) {
                case FlexItemsAlignment.Stretch:
                    if (_lineCount == 1 && flexWrap == FlexWrap.NoWrap)
                        childSize.V = lineInfo.LineV + lineInfo.LineFreeV; // TODO ?
                    else
                        childSize.V = lineInfo.LineV;
                    break;
                case FlexItemsAlignment.FlexEnd:
                    v += lineInfo.LineV - childSize.V;
                    break;
                case FlexItemsAlignment.Center:
                    v += (lineInfo.LineV - childSize.V) * 0.5;
                    break;
            }

            child.Arrange(isHorizontal ? new Rect(u, v, childSize.U, childSize.V) : new Rect(v, u, childSize.V, childSize.U));

            if (!isReverse)
                u += childSize.U;
        }
    }

    private static bool IsInRangeOfPosDoubleIncludeZero(double value) =>
        !(double.IsNaN(value) || double.IsInfinity(value)) && value >= 0;

    private readonly struct FlexItemInfo : IComparable<FlexItemInfo>
    {
        public FlexItemInfo(int index, int order)
        {
            Index = index;
            Order = order;
        }

        private int Order { get; }

        public int Index { get; }

        public int CompareTo(FlexItemInfo other)
        {
            var orderCompare = Order.CompareTo(other.Order);
            if (orderCompare != 0) return orderCompare;
            return Index.CompareTo(other.Index);
        }
    }

    private struct FlexLineInfo
    {
        public double ItemsU { get; init; }
        public double OffsetV { get; init; }
        public double LineU { get; init; }
        public double LineV { get; init; }
        public double LineFreeV { get; init; }
        public int ItemStartIndex { get; init; }
        public int ItemEndIndex { get; init; }
        public double ScaleU { get; set; }
    }

    private struct UVSize
    {
        public UVSize(FlexDirection direction, Size size)
        {
            U = V = 0d;
            FlexDirection = direction;
            Width = size.Width;
            Height = size.Height;
        }

        public UVSize(FlexDirection direction)
        {
            U = V = 0d;
            FlexDirection = direction;
        }

        public double U { get; set; }

        public double V { get; set; }

        private FlexDirection FlexDirection { get; }

        public double Width
        {
            get => FlexDirection is FlexDirection.Row or FlexDirection.RowReverse ? U : V;
            private init
            {
                if (FlexDirection is FlexDirection.Row or FlexDirection.RowReverse)
                    U = value;
                else
                    V = value;
            }
        }

        public double Height
        {
            get => FlexDirection is FlexDirection.Row or FlexDirection.RowReverse ? V : U;
            private init
            {
                if (FlexDirection is FlexDirection.Row or FlexDirection.RowReverse)
                    V = value;
                else
                    U = value;
            }
        }

        public override string ToString() =>
            $"U: {U}, V: {V}";
    }
}