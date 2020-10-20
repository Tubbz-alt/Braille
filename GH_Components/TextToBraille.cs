using Braille.GH_Goos;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using Rhino;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Braille.GH_Components
{
    public class TextToBraille : GH_Component, IGH_VariableParameterComponent
    {
        private Kernal.brailleType m_brailleType = 0;
        private List<GH_brailleStr> m_brailleStrs = new List<GH_brailleStr>();

        private double m_spacingCells = 6;
        private double m_spacingPoints = 2.5;
        private double m_spacingLines = 10;

        public TextToBraille()
          : base("Text to Braille", "tB",
              "This will convert text to braille charecters",
              "Vector", "Braille")
        {
        }

        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                return Properties.Icons.BrailleDot;
            }
        }

        public override Guid ComponentGuid
        {
            get { return new Guid("50F073DE-58D4-4E3E-B7B7-908A13623710"); }
        }

        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Text", "T", "This will return text as Braille charedters", GH_ParamAccess.list);
            pManager.AddCurveParameter("Region", "r", "The region where braille text should go", GH_ParamAccess.list);

            pManager[1].Optional = true;
        }

        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddGenericParameter("Braille", "B", "Text as Braille charecters", GH_ParamAccess.tree);
            pManager.AddGenericParameter("Points", "P", "Text as Braille points", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            m_brailleStrs.Clear();
            List<string> strs = new List<string>();
            List<Curve> regions = new List<Curve>();

            DA.GetDataList("Text", strs);
            bool region = DA.GetDataList("Region", regions);

            if (region)
            {
                /// <summary>
                /// Extends the shortest of two lists to match the other one
                /// </summary>
                /// <typeparam name="T">Generic type A</typeparam>
                /// <typeparam name="Q">Generic type B</typeparam>
                /// <param name="A">First list</param>
                /// <param name="B">Second List</param>
                void ExtendShortestList<T, Q>(List<T> A, List<Q> B)
                {
                    var difference = Math.Abs(A.Count - B.Count);
                    if (difference == 0) return;

                    if (A.Count < B.Count) A.AddRange(Enumerable.Repeat(A.Last(), difference));
                    else B.AddRange(Enumerable.Repeat(B.Last(), difference));
                }

                ExtendShortestList(strs, regions);

                m_brailleStrs.AddRange(
                    strs.Zip(regions, (s, r) => brailleWithRegion(s, r))
                    );
            }
            else
            {
                foreach (string s in strs)
                {
                    GH_brailleStr bS = new GH_brailleStr(s, type: m_brailleType, spacing: m_spacingPoints, cell: m_spacingCells, lineSpacing: m_spacingLines);
                    m_brailleStrs.Add(bS);
                }
            }

            GH_Structure<GH_brailleStr> tree = new GH_Structure<GH_brailleStr>();
            for (int i = 0; i < m_brailleStrs.Count; ++i)
            {
                GH_brailleStr b = m_brailleStrs[i];
                tree.Append(b, new GH_Path(i)); //i for seperat trees
            }

            //Conversion to points
            GH_Structure<GH_Point> points = new GH_Structure<GH_Point>();

            for (int i = 0; i < m_brailleStrs.Count; ++i)
            {
                var b = m_brailleStrs[i].ToPointsGH(path: new GH_Path(i));

                points.MergeStructure(b);
            }

            DA.SetDataTree(0, tree); ;
            DA.SetDataTree(1, points);
        }

        private GH_brailleStr brailleWithRegion(string str, Curve region)
        {
            GH_brailleStr bS = new GH_brailleStr(str, type: m_brailleType, spacing: m_spacingPoints, cell: m_spacingCells, lineSpacing: m_spacingLines);
            bS.SetRegion(region);
            return bS;
        }

        protected override void AppendAdditionalComponentMenuItems(ToolStripDropDown menu)
        {
            base.AppendAdditionalComponentMenuItems(menu);

            ToolStripSeparator seperator = Menu_AppendSeparator(menu);

            ToolStripMenuItem robotManufacturers = Menu_AppendItem(menu, "Braille Type");
            robotManufacturers.ToolTipText = "Select the Braille type";
            foreach (string name in typeof(Kernal.brailleType).GetEnumNames())
            {
                ToolStripMenuItem item = new ToolStripMenuItem(name, null, brailleTypeClick);

                if (name == this.m_brailleType.ToString()) item.Checked = true;
                robotManufacturers.DropDownItems.Add(item);
            }

            ToolStripSeparator seperator2 = Menu_AppendSeparator(menu);

            ToolStripTextBox spacingPoints = new ToolStripTextBox(); //;  Menu_AppendTextItem(menu, m_spacingPoints.ToString(), null, OnTextCanged, false);
            spacingPoints.Text = m_spacingPoints.ToString();
            spacingPoints.Name = "Point Spacing";
            spacingPoints.ToolTipText = "";
            spacingPoints.TextChanged += OnTextCanged;
            ToolStripTextBox spacingCells = new ToolStripTextBox(); //Menu_AppendTextItem(menu, m_spacingCells.ToString(), null, OnTextCanged, false);
            spacingCells.Text = m_spacingCells.ToString();
            spacingCells.Name = "Cell Spacing";
            spacingCells.ToolTipText = "";
            spacingCells.TextChanged += OnTextCanged;
            ToolStripTextBox spacingLines = new ToolStripTextBox(); //Menu_AppendTextItem(menu, m_spacingLines.ToString(), null, OnTextCanged, false);
            spacingLines.Text = m_spacingLines.ToString();
            spacingLines.Name = "Line Spacing";
            spacingLines.ToolTipText = "";
            spacingLines.TextChanged += OnTextCanged;

            ToolStripMenuItem spacingPointsConatiner = Menu_AppendItem(menu, "Point Spacing");
            spacingPointsConatiner.DropDownItems.Add(spacingPoints);

            ToolStripMenuItem spacingCellsConatiner = Menu_AppendItem(menu, "Cell Spacing");
            spacingCellsConatiner.DropDownItems.Add(spacingCells);

            ToolStripMenuItem spacingLineConatiner = Menu_AppendItem(menu, "Line Spacing");
            spacingLineConatiner.DropDownItems.Add(spacingLines);
        }

        private void OnTextCanged(object sender, EventArgs e)
        {
            if (typeof(ToolStripTextBox).IsAssignableFrom(sender.GetType()))
            {
                var tB = sender as ToolStripTextBox;

                if (tB.Name == "Point Spacing") this.m_spacingPoints = Convert.ToDouble(tB.Text);
                if (tB.Name == "Cell Spacing") this.m_spacingCells = Convert.ToDouble(tB.Text);
                if (tB.Name == "Line Spacing") this.m_spacingLines = Convert.ToDouble(tB.Text);
            }

            ExpireSolution(true);
        }

        private void ToolStripItem1_TextChanged(Object sender, EventArgs e)
        {
            MessageBox.Show("You are in the ToolStripItem.TextChanged event.");
        }

        private void brailleTypeClick(object sender, EventArgs e)
        {
            RecordUndoEvent("Manufacturer");
            ToolStripMenuItem currentItem = (ToolStripMenuItem)sender;
            Menu.UncheckOtherMenuItems(currentItem);
            this.m_brailleType = (Kernal.brailleType)currentItem.Owner.Items.IndexOf(currentItem);
            ExpireSolution(true);
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetInt32("BrailleType", (int)this.m_brailleType);
            writer.SetDouble("CellSpacing", this.m_spacingCells);
            writer.SetDouble("PointSpacing", this.m_spacingPoints);
            writer.SetDouble("LineSpacing", this.m_spacingLines);
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            this.m_brailleType = (Kernal.brailleType)reader.GetInt32("BrailleType");
            this.m_spacingCells = reader.GetDouble("CellSpacing");
            this.m_spacingPoints = reader.GetDouble("PointSpacing");
            this.m_spacingLines = reader.GetDouble("LineSpacing");
            return base.Read(reader);
        }

        public override BoundingBox ClippingBox
        {
            get
            {
                var points = new List<Point3d>();
                foreach (var bStrin in m_brailleStrs)
                {
                    points.AddRange(bStrin.ToPoints());
                }
                return new BoundingBox(points);
            }
        }

        public override void DrawViewportWires(IGH_PreviewArgs args)
        {
            base.DrawViewportWires(args);

            if (m_brailleStrs.Count == 0) return;

            foreach (var bStrin in m_brailleStrs)
            {
                args.Display.DrawPointCloud(new PointCloud(bStrin.ToPoints()), 10);
            }
        }

        public override void DrawViewportMeshes(IGH_PreviewArgs args)
        {
            base.DrawViewportMeshes(args);

            if (m_brailleStrs.Count == 0) return;

            foreach (var bStrin in m_brailleStrs)
            {
                args.Display.DrawPointCloud(new PointCloud(bStrin.ToPoints()), 10);
            }
        }

        public override void ClearData()
        {
            base.ClearData();
            this.m_brailleStrs.Clear();
        }

        public override void BakeGeometry(RhinoDoc doc, List<Guid> obj_ids)
        {
            base.BakeGeometry(doc, obj_ids);
            foreach (GH_brailleStr bStr in this.m_brailleStrs)
            {
                var attributes = doc.CreateDefaultAttributes();
                
                attributes.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                //attributes.ObjectColor = ;
                obj_ids.Add(doc.Objects.AddPointCloud(bStr.ToPoints(), attributes));
            }
        }

        public override void BakeGeometry(RhinoDoc doc, Rhino.DocObjects.ObjectAttributes att, List<Guid> obj_ids)
        {
            base.BakeGeometry(doc, att, obj_ids);

            foreach (GH_brailleStr bStr in this.m_brailleStrs)
            {
                var attributes = doc.CreateDefaultAttributes();
                if (att != null) attributes = att;
                attributes.ColorSource = Rhino.DocObjects.ObjectColorSource.ColorFromObject;
                //attributes.ObjectColor = ;
                obj_ids.Add(doc.Objects.AddPointCloud(bStr.ToPoints(), attributes));
            }
        }

        bool IGH_VariableParameterComponent.CanInsertParameter(GH_ParameterSide side, int index) => false;

        bool IGH_VariableParameterComponent.CanRemoveParameter(GH_ParameterSide side, int index) => false;

        IGH_Param IGH_VariableParameterComponent.CreateParameter(GH_ParameterSide side, int index) => null;

        bool IGH_VariableParameterComponent.DestroyParameter(GH_ParameterSide side, int index) => false;

        void IGH_VariableParameterComponent.VariableParameterMaintenance()
        {
        }
    }
}