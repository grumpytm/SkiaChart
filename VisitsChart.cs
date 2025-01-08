using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace SkiaChart;

public class VisitsChart
{
    private SKControl _skControl;

    private int[] _visits;
    private int[] _unique;

    public VisitsChart(SKControl skControl)
    {
        // Initialize the SKControl
        _skControl = skControl;
        //_skControl.Dock = DockStyle.Top;

        _skControl.PaintSurface += OnCanvasPaint;
    }

    public void SetRanges(int[] visits, int[] unique)
    {
        // Sample data points for "Visits" and "Unique Visits"
        _visits = visits;
        _unique = unique;
    }

    // Add 10% to the number and round up to the nearest 10
    private int RoundUp(int number)
    {
        int increasedValue = (int)(number * 1.1);
        return ((increasedValue + 9) / 10) * 10;
    }

    private SKColor[] lineColors = new SKColor[] {
        SKColor.Parse("#88BBC8"),
        SKColor.Parse("#F29575")
    };

    private void OnCanvasPaint(object? sender, SKPaintSurfaceEventArgs e)
    {
        var chartWidth = e.Info.Width;
        var chartHeight = e.Info.Height;

        var canvas = e.Surface.Canvas;
        canvas.Clear(SKColor.Parse("#F6F7F7"));

        // Inner rectangle margin and padding
        int margin = 20;
        int cornerRadius = 5;

        // Define the inner rectangle's position and size
        float left = margin * 2;
        float top = margin * 2.5f;
        float right = canvas.LocalClipBounds.Width - margin * 2;
        float bottom = canvas.LocalClipBounds.Height - margin * 1.5f;
        var innerRect = new SKRect(left, top, right, bottom);

        // Draw the inner rectangle
        DrawMainRoundedRectangle(canvas, innerRect, cornerRadius);

        // Draw legend
        DrawOutsideLegend(canvas, innerRect);

        // Draw Y-axis labels (legend)
        int maxValue = RoundUp(_visits.Max());
        DrawYAxisLegend(canvas, innerRect, maxValue, cornerRadius);

        // Draw the line graphs for each dataset
        DrawLineGraphWithFill(canvas, innerRect, cornerRadius, lineColors[0], SKColor.Parse("#F5F9FA"), _visits, maxValue);
        DrawLineGraphWithFill(canvas, innerRect, cornerRadius, lineColors[1], SKColor.Parse("#FFFBF3"), _unique, maxValue);

        // Drawing the grid and axes
        DrawAxesAndGrid(canvas, innerRect);
    }

    private void DrawMainRoundedRectangle(SKCanvas canvas, SKRect innerRect, int radius)
    {
        var rectPaint = new SKPaint
        {
            Color = SKColors.White,
            Style = SKPaintStyle.Fill,
            IsAntialias = true
        };

        var borderPaint = new SKPaint
        {
            Color = SKColor.Parse("#DCDCDC"),
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            IsAntialias = true
        };

        // Draw the rectangle itself it's rounded border
        canvas.DrawRoundRect(innerRect, radius, radius, rectPaint);
        canvas.DrawRoundRect(innerRect, radius, radius, borderPaint);
    }

    private void DrawOutsideLegend(SKCanvas canvas, SKRect innerRect)
    {
        string[] legend = { "Sample chart", "Element 1", "Element 2" };

        var left = innerRect.Left + 5;
        var top = innerRect.Top - 15;
        var right = innerRect.Right - 5;

        // Chart title
        var titlePaint = new SKPaint
        {
            Color = SKColor.Parse("#666666"),
            TextSize = 12,
            Typeface = SKTypeface.FromFamilyName(default, SKFontStyle.Bold), // 
            IsAntialias = true
        };

        canvas.DrawText(legend[0], left, top, titlePaint);

        // Text & line paint
        var textPaint = new SKPaint
        {
            Color = SKColors.Black,
            TextSize = 12,
            IsAntialias = true
        };

        var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 5
        };

        // Last element is drawn first
        var size = textPaint.MeasureText(legend[2]);

        linePaint.Color = lineColors[0];
        canvas.DrawText(legend[2], right - size, top, textPaint);
        canvas.DrawLine(right - size - 25, top - 5, right - size - 5, top - 5, linePaint);

        // First element is drawn last
        size += textPaint.MeasureText(legend[1]) + 35;
        canvas.DrawText(legend[1], right - size, top, textPaint);
        linePaint.Color = lineColors[1];
        canvas.DrawLine(right - size - 25, top - 5, right - size - 5, top - 5, linePaint);
    }

    private void DrawYAxisLegend(SKCanvas canvas, SKRect innerRect, float maxValue, int radius)
    {
        var textPaint = new SKPaint { Color = SKColor.Parse("#999999"), TextSize = 12, IsAntialias = true };

        int numberOfLabels = 5;
        float stepY = innerRect.Height / numberOfLabels;
        float valueStep = maxValue / numberOfLabels;
        float bottom = innerRect.Bottom + radius;

        for (int i = 0; i <= numberOfLabels; i++)
        {
            float yPos = bottom - i * stepY;
            float value = i * valueStep;
            string valueText = value.ToString("0");
            float textWidth = textPaint.MeasureText(valueText);
            canvas.DrawText(valueText, innerRect.Left - (textWidth + 10), yPos, textPaint);
        }
    }

    private void DrawAxesAndGrid(SKCanvas canvas, SKRect innerRect)
    {
        var gridPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColors.LightGray,
            StrokeWidth = 1,
            IsAntialias = true
        };

        var dashedPaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            Color = SKColor.Parse("#EAEBEB"),
            StrokeWidth = 1,
            IsAntialias = true
        };

        dashedPaint.PathEffect = SKPathEffect.CreateDash(new float[] { 4, 3 }, 0);

        var left = innerRect.Left;
        var top = innerRect.Top;
        var right = innerRect.Right;
        var bottom = innerRect.Bottom;
        float width = innerRect.Width;
        float height = innerRect.Height;

        // Draw horizontal grid lines (Y-axis)
        float stepY = height / 5;
        for (int i = 1; i < 5; i++)
        {
            float y = bottom - (i * stepY);
            canvas.DrawLine(left, y, right, y, dashedPaint);
        }

        // Draw vertical grid lines (X-axis)
        float stepX = width / 4;
        for (int i = 1; i < 4; i++)
        {
            float x = left + (i * stepX);
            canvas.DrawLine(x, top, x, bottom, dashedPaint);
        }
    }

    private void DrawLineGraphWithFill(SKCanvas canvas, SKRect innerRect, float cornerRadius, SKColor color, SKColor fillColor, int[] dataPoints, float maxValue)
    {
        var linePaint = new SKPaint
        {
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            Color = color,
            IsAntialias = true
        };

        linePaint.PathEffect = SKPathEffect.CreateDash(new float[] { 5, 3 }, 0);

        // Define fill paint with a transparent color
        var fillPaint = new SKPaint
        {
            Style = SKPaintStyle.Fill,
            Color = fillColor.WithAlpha(150),
            IsAntialias = true
        };

        // Define the clipping path around the inner rectangular with rounded corners
        var clipPath = new SKPath();
        clipPath.AddRoundRect(innerRect, cornerRadius, cornerRadius);

        // Save the canvas state and apply the clipping path
        canvas.Save();
        canvas.ClipPath(clipPath, SKClipOperation.Intersect);

        // Proceed to draw the graph within the clipped area
        var left = innerRect.Left;
        var bottom = innerRect.Bottom;
        float width = innerRect.Width;
        float height = innerRect.Height;

        int count = dataPoints.Length;

        float stepX = width / (count - 1);
        float scaleY = height / maxValue;

        for (int i = 0; i < count; i++)
        {
            float x1 = left + i * stepX;
            float y1 = bottom - (dataPoints[i] * scaleY);

            if (i < count - 1)
            {
                // Draw line between the current point and the next point
                float x2 = left + (i + 1) * stepX;
                float y2 = bottom - (dataPoints[i + 1] * scaleY);
                canvas.DrawLine(x1, y1, x2, y2, linePaint);

                // Draw the filled area below the line segment
                var path = new SKPath();
                path.MoveTo(x1, bottom);
                path.LineTo(x1, y1);
                path.LineTo(x2, y2);
                path.LineTo(x2, bottom);
                path.Close();
                canvas.DrawPath(path, fillPaint);
            }

            canvas.DrawCircle(x1, y1, 4, new SKPaint { Color = color, Style = SKPaintStyle.Fill });
        }

        canvas.Restore();
    }
}