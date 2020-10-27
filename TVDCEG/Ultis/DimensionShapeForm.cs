using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Brushes = System.Windows.Media.Brushes;
using Point = System.Drawing.Point;

namespace TVDCEG.Ultis
{
    class DimensionShapeForm
    {
        private int ExtensionLineLength;
        private int ExtensionLineExtent;
        private HorizontalAlignment allign;
        private Line dimensionLine;
        private SolidColorBrush lineColor = Brushes.DarkSlateBlue;

        private List<PointF> PointList = new List<PointF>();
        private List<Line> ExtensionLines = new List<Line>();
        private List<UIElement> interfaceElements = new List<UIElement>();
        private WpfCoordinates tool = new WpfCoordinates();

        public DimensionShapeForm()
        {
            ExtensionLineLength = 60;
            ExtensionLineExtent = 7;
            allign = HorizontalAlignment.Right;
        }

        public DimensionShapeForm(int ExtensionlineLength, int ExtensionlineExtent, HorizontalAlignment allign)
        {
            ExtensionLineLength = ExtensionlineLength;
            ExtensionLineExtent = ExtensionlineExtent;
            this.allign = allign;
        }
        public void DrawWallDimension(Line wall/*, GridSetup grid*/)
        {
            AddExtensionLines(wall);
            //AddDimLine(wall);
            //AddDimensionTick(dimensionLine);
            //AddAnnotation(wall, grid);

            //DrawDimension(grid);
        }
        private void AddExtensionLines(Line line)
        {
            GetWallStartEnd(line);
            foreach (Line item in tool.GetPerpendiculars(line, PointList))
            {
                Line extensionLine = new Line();
                PointF point = new PointF();

                if (tool.GetSlope(item) > 0)
                {
                    point = tool.GetSecondCoord(item, tool.GetLength(item) - ExtensionLineLength);
                }
                else
                {
                    Line temp = new Line();
                    temp.X1 = item.X2;
                    temp.Y1 = item.Y2;
                    temp.X2 = 2 * item.X2 - item.X1;
                    temp.Y2 = 2 * item.Y2 - item.Y1;
                    point = tool.GetSecondCoord(temp, -ExtensionLineLength);
                }

                extensionLine.X1 = point.X;
                extensionLine.Y1 = point.Y;
                extensionLine.X2 = item.X2;
                extensionLine.Y2 = item.Y2;
                extensionLine.Uid = "DimensionExtent";
                extensionLine.Stroke = lineColor;

                ExtensionLines.Add(extensionLine);
                interfaceElements.Add(extensionLine);
            }
        }
        private void GetWallStartEnd(Line line)
        {
            PointF start = new PointF()
            {
                X = (float)line.X1,
                Y = (float)line.Y1
            };
            PointList.Add(start);
            PointF end = new PointF
            {
                X = (float)line.X2,
                Y = (float)line.Y2
            };
            PointList.Add(end);
        }
    }
}
