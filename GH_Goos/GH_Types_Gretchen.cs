using Braille.Kernal;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Braille.GH_Goos
{
    internal class GH_brailleStr : braillFont, IGH_Goo
    {
        private Curve region = null;

        public GH_brailleStr(string str, brailleType type = brailleType.sixDot, double spacing = 2.5, double cell = 6, double lineSpacing = 10)
            : base(str: str, type: type, spacing: spacing, cell: cell, lineSpacing: lineSpacing) { }

        #region private methods

        private GH_Structure<GH_Point> toPointsGH(List<brailleChar> bStr, Point3d startPoint = default, GH_Path path = default)
        {
            GH_Structure<GH_Point> structuredPoints = new GH_Structure<GH_Point>();

            if (region != null)
            {
                var words = new Queue<brailleWord>(extractWords(Braille));
                var line = new Queue<Curve>(getLinesFromRegion(region));

                var usedLines = new Queue<Curve>();

                List<List<brailleChar>> rows = splitToLines(line, words, out usedLines).Select(row => wordsToCharecters(row)).ToList();

                structuredPoints = formatLines(rows, usedLines, mainPath: path);
            }
            else structuredPoints = formatLine(Braille, linePath: path);

            return structuredPoints;
        }

        private GH_Structure<GH_Point> charToPoints(brailleChar c, Point3d location = default, GH_Path path = default)
        {
            GH_Structure<GH_Point> points = new GH_Structure<GH_Point>();
            Plane plane = Plane.WorldXY;
            plane.Origin = location;

            points.EnsurePath(path);

            int fix = 0;
            if (c.type == brailleType.sixDot) { fix = -1; }

            if (c.dot1) points.Append(new GH_Point(plane.PointAt(0, Spacing * (3 + fix), 0)), path);
            if (c.dot2) points.Append(new GH_Point(plane.PointAt(0, Spacing * (2 + fix), 0)), path);
            if (c.dot3) points.Append(new GH_Point(plane.PointAt(0, Spacing * (1 + fix), 0)), path);
            if (c.dot7) points.Append(new GH_Point(plane.PointAt(0, 0, 0)), path);

            if (c.dot4) points.Append(new GH_Point(plane.PointAt(Spacing, Spacing * (3 + fix), 0)), path);
            if (c.dot5) points.Append(new GH_Point(plane.PointAt(Spacing, Spacing * (2 + fix), 0)), path);
            if (c.dot6) points.Append(new GH_Point(plane.PointAt(Spacing, Spacing * (1 + fix), 0)), path);
            if (c.dot8) points.Append(new GH_Point(plane.PointAt(Spacing, 0, 0)), path);

            return points;
        }

        private GH_Structure<GH_Point> formatLine(List<brailleChar> row, Point3d startPoint = default, GH_Path linePath = default)
        {
            GH_Structure<GH_Point> points = new GH_Structure<GH_Point>();

            if (linePath == default)
                linePath = new GH_Path(0);

            var start = startPoint;
            int i = 0;
            foreach (brailleChar brailleChar in row)
            {
                var p = linePath.AppendElement(i);
                points.MergeStructure(charToPoints(brailleChar, start, p));
                start.Transform(Transform.Translation(Cell, 0, 0));
                ++i;
            }
            return points;
        }

        private GH_Structure<GH_Point> formatLines(List<List<brailleChar>> rows, Queue<Curve> curve, GH_Path mainPath = default)
        {
            GH_Structure<GH_Point> structurePoints = new GH_Structure<GH_Point>();
            int lineCount = 0;

            if (mainPath == default)
                mainPath = new GH_Path(0);

            foreach (List<brailleChar> row in rows)
            {
                var l = curve.Dequeue();

                var p = mainPath.AppendElement(lineCount);

                structurePoints.MergeStructure(formatLine(row, l.PointAtEnd, linePath: p));

                ++lineCount;
            }

            return structurePoints;
        }

        private Curve[] getLinesFromRegion(Curve curve, Vector3d direction = default)
        {
            if (direction == default)
            {
                direction = Vector3d.YAxis;
                direction.Reverse();
            }
            var region = new Brep();

            if (!GH_Convert.ToBrep(curve, ref region, GH_Conversion.Both))
                new GH_RuntimeMessage("Canot use this curve as region", GH_RuntimeMessageLevel.Error);

            var lines = Brep.CreateContourCurves(region, curve.GetBoundingBox(false).Corner(true, false, true), curve.GetBoundingBox(false).Corner(true, true, true), LineSpacing);

            return lines;
        }

        private List<List<brailleWord>> splitToLines(Queue<Curve> lineQueue, Queue<brailleWord> wordQueue, out Queue<Curve> usedLines)
        {
            var rowes = new List<List<brailleWord>>();
            var lines = new Queue<Curve>();

            if (lineQueue.Count == 0) throw new Exception("Shape was not contured");
            lineQueue.Dequeue(); //Skip the first line

            brailleWord overSet = new brailleWord();

            while (wordQueue.Count() > 0 & lineQueue.Count() > 0)
            {
                var row = new List<brailleWord>();
                var line = lineQueue.Dequeue();

                var lineLength = line.GetLength();
                double textLength = 0;

                //Add overset
                if (overSet.Count != 0)
                {
                    row.Add(overSet);
                    textLength += wordLength(overSet);
                }

                while (lineLength > textLength)
                {
                    if (wordQueue.Count == 0) break;
                    var word = wordQueue.Dequeue();
                    row.Add(word);
                    textLength += wordLength(word);
                    textLength += Cell; //aacound for blank spaces between words
                }

                //Move last word to the next row
                if (textLength > lineLength)
                {
                    var word = new brailleWord(row.Last());
                    row.Remove(row.Last());
                    rowes.Add(new List<brailleWord>(row));

                    overSet = word;
                }
                //If liens are exactly the same length
                else
                {
                    rowes.Add(new List<brailleWord>(row));
                    row.Clear();
                    textLength = 0;
                }

                lines.Enqueue(line);
            }

            usedLines = lines;
            return rowes;
        }

        #endregion private methods

        #region public methods

        /// <summary>
        /// Retrun braille as point list
        /// </summary>
        /// <returns>List of type Point3d</returns>
        public List<Point3d> ToPoints()
        {
            return (this.toPointsGH(this.Braille)).Select(p => p.Value).ToList();
        }

        /// <summary>
        /// Return the braille as structures
        /// </summary>
        /// <param name="path">The path the structure should be offset at</param>
        /// <returns>GH_Structure of type GH_Point</returns>
        public GH_Structure<GH_Point> ToPointsGH(GH_Path path = default)
        {
            return this.toPointsGH(this.Braille, path: path);
        }

        /// <summary>
        /// Set the region for the braille to be in
        /// </summary>
        /// <param name="c">The curve that discribes the region</param>
        /// <returns>Returns true if the operation has been sucsessful</returns>
        public bool SetRegion(Curve c)
        {
            if (c == null)
                return false;
            else if (!c.IsClosed)
                return false;
            else
                this.region = c;
            return true;
        }

        /// <summary>
        /// Attempt a cast from generic object
        /// </summary>
        /// <param name="source">Reference to source of cast.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <remarks>If False, the contents of this instance are not to be trusted.</remarks>
        public bool CastFrom(object source)
        {
            if (source is null)
                return false;
            if (source.GetType().IsAssignableFrom(typeof(string)))
            {
                string str = source as string;
                //Value = new GH_brailleStr(str);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempt a cast to type T
        /// </summary>
        /// <typeparam name="Q">Type to cast to</typeparam>
        /// <param name="target">Pointer to target of cast.</param>
        /// <returns>True on success, false on failure.</returns>
        /// <remarks>If false, the target instance contents are not guaranteed to be valid.</remarks>
        public bool CastTo<T>(out T target)
        {
            if (typeof(T).IsAssignableFrom(typeof(PointCloud)))
            {
                List<Point3d> pts = new List<Point3d>();
                foreach (GH_Point pt in this.ToPointsGH())
                {
                    Point3d p = new Point3d();
                    GH_Convert.ToPoint3d(pt, ref p, GH_Conversion.Both);
                    pts.Add(p);
                }

                object pc = new PointCloud(pts);
                target = (T)pc;
                return true;
            }
            if (typeof(T).IsAssignableFrom(typeof(GH_String)))
            {
                object text = new GH_String(this.Str);
                target = (T)text;
                return true;
            }

            target = default(T);
            return false;
        }

        /// <summary>
        /// Make a complete duplicate of this instance. No shallow copies.
        /// </summary>
        /// <returns></returns>
        /// <reamrks>Classes which implement this interface should also provide type specific Duplicate methods</reamrks>
        public IGH_Goo Duplicate()
        {
            return new GH_brailleStr(this.Str, this.Type, this.Spacing, this.Cell, this.LineSpacing);
        }

        /// <summary>
        /// Create a new proxy for this instance. Return Null if this class does not support proxies.
        /// </summary>
        /// <returns></returns>
        public IGH_GooProxy EmitProxy() => null;

        /// <summary>
        /// This function will be called when the local IGH_Goo instance disapears into a
        /// user Script. This would be an excellent place to cast your IGH_Goo type to a
        /// simple data type.
        /// </summary>
        /// <returns>The object that represents this IGH_Goo instance in a script.</returns>
        public object ScriptVariable() => null;

        /// <summary>
        /// This method is called whenever the instance is required to deserialize itself.
        /// </summary>
        /// <param name="reader">Reader object to deserialize from.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool Read(GH_IO.Serialization.GH_IReader reader) => false;

        /// <summary>
        /// This method is called whenever the instance is required to serialize itself.
        /// </summary>
        /// <param name="writer">Writer object to serialize with.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool Write(GH_IO.Serialization.GH_IWriter writer) => false;

        #endregion public methods
    }
}

internal class Menu
{
    /// <summary>
    /// Uncheck other dropdown menu items
    /// </summary>
    /// <param name="selectedMenuItem"></param>
    static public void UncheckOtherMenuItems(ToolStripMenuItem selectedMenuItem)
    {
        selectedMenuItem.Checked = true;

        // Select the other MenuItens from the ParentMenu(OwnerItens) and unchecked this,
        // The current Linq Expression verify if the item is a real ToolStripMenuItem
        // and if the item is a another ToolStripMenuItem to uncheck this.
        foreach (var ltoolStripMenuItem in (from object
                                                item in selectedMenuItem.Owner.Items
                                            let ltoolStripMenuItem = item as ToolStripMenuItem
                                            where ltoolStripMenuItem != null
                                            where !item.Equals(selectedMenuItem)
                                            select ltoolStripMenuItem))
            (ltoolStripMenuItem).Checked = false;

        // This line is optional, for show the mainMenu after click
        //selectedMenuItem.Owner.Show();
    }
}