﻿// The MIT License(MIT)
//
// Copyright(c) 2021 Alberto Rodriguez Orozco & LiveCharts Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.VisualElements;

namespace LiveChartsCore.SkiaSharpView.VisualElements;

/// <summary>
/// Defines a visual element in a chart that draws a rectangle geometry in the user interface.
/// </summary>
public class GeometryVisual<TGeometry> : BaseGeometryVisual
    where TGeometry : ISizedGeometry<SkiaSharpDrawingContext>, new()
{
    private TGeometry? _geometry;
    private LvcSize _actualSize = new();

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Measure"/>
    public override LvcSize Measure(Chart<SkiaSharpDrawingContext> chart, Scaler primaryAxisScale, Scaler secondaryAxisScale)
    {
        var w = (float)Width;
        var h = (float)Height;

        if (SizeUnit == MeasureUnit.ChartValues)
        {
            w = secondaryAxisScale.MeasureInPixels(w);
            h = primaryAxisScale.MeasureInPixels(h);
        }

        return _actualSize = new LvcSize(w, h);
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.GetActualSize"/>
    public override LvcSize GetActualSize()
    {
        return _actualSize;
    }

    /// <inheritdoc cref="VisualElement{TDrawingContext}.Draw"/>
    protected override void Draw(Chart<SkiaSharpDrawingContext> chart, Scaler primaryScaler, Scaler secondaryScaler)
    {
        var x = (float)X;
        var y = (float)Y;

        if (LocationUnit == MeasureUnit.ChartValues)
        {
            x = secondaryScaler.ToPixels(x);
            y = primaryScaler.ToPixels(y);
        }

        _ = Measure(chart, primaryScaler, secondaryScaler);

        if (_geometry is null)
        {
            _geometry = new TGeometry { X = x, Y = y, Width = _actualSize.Width, Height = _actualSize.Height };

            _ = _geometry
                .TransitionateProperties()
                .WithAnimation(chart)
                .CompleteCurrentTransitions();
        }

        _geometry.X = x;
        _geometry.Y = y;
        _geometry.Width = _actualSize.Width;
        _geometry.Height = _actualSize.Height;

        var drawing = chart.Canvas.Draw();
        if (Fill is not null) _ = drawing.SelectPaint(Fill).Draw(_geometry);
        if (Stroke is not null) _ = drawing.SelectPaint(Stroke).Draw(_geometry);
    }
}
