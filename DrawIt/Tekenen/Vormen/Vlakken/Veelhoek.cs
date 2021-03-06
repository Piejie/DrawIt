﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace DrawIt.Tekenen
{
	public class Veelhoek : Vlak
	{
		public Veelhoek(Layer currentlayer)
			: base(Vorm_type.Veelhoek, currentlayer)
		{
		}

		#region Punten
		private Collectie<Punt> punten = new Collectie<Punt>();
		public Collectie<Punt> Punten
		{
			get { return punten; }
		}

		public override Vorm[] Dep_Vormen
		{
			get
			{
				return punten.ToArray();
			}
		}
		#endregion

		public override void Draw(Tekening tek, Graphics gr, bool widepen, bool fill)
		{
			Point[] pt = punten.Select(T => tek.co_pt(T.Coordinaat, gr.DpiX, gr.DpiY)).ToArray();
			if(pt.Length < 2) return;
			if (fill) gr.FillPolygon(GetBrush(gr, tek.Schaal, tek.Offset, false), pt);
			gr.DrawPolygon(GetPen(false), pt);
		}

		public override void Draw(Graphics gr, float schaal, RectangleF window)
		{
			Pen pen = (Pen)GetPen(true).Clone();
			pen.Width = pen.Width / 2.54f;

			IEnumerable<PointF> ptn = punten.Select(T => new PointF(T.Coordinaat.X - window.Left, T.Coordinaat.Y - window.Top));
			ptn = ptn.Select(T => new PointF(T.X * schaal * 10, T.Y * schaal * 10));

			gr.FillPolygon(GetBrush(gr, schaal, new PointF(-window.Left, -window.Top), true), ptn.ToArray());
			gr.DrawPolygon(pen, ptn.ToArray());
		}

		public override string ToString()
		{
			return "veelhoek;" + Zichtbaarheid + ";" + Niveau + ";" + Layer.Naam + ";"  + ColorTranslator.ToOle(LijnKleur) + ";" + LijnDikte + ";" + (int)LijnStijl + ";" + ColorTranslator.ToOle(VulKleur1) + ";" + ColorTranslator.ToOle(VulKleur2) + ";" + (int)OpvulType + ";" + (int)VulStijl + ";" + LoopHoek + ";" + string.Join(";", punten.Select(T => T.ID.ToString()).ToArray());
		}

		public override RectangleF Bounds(Graphics gr)
		{
			float l = punten.Min(T => T.X);
			float r = punten.Max(T => T.X);
			float t = punten.Min(T => T.Y);
			float b = punten.Max(T => T.Y);

			return new RectangleF(l, t, r - l, b - t);
		}
		
        public override void Draw(Tekening tek, Graphics gr, PointF loc_co, Vorm[] ref_vormen)
        {
            Point[] pt = punten.Select(T => T.Coordinaat).Combine(loc_co).Select(T=> tek.co_pt(T, gr.DpiX, gr.DpiY)).ToArray();
            if (pt.Length < 2) return;
            gr.FillPolygon(GetBrush(gr, tek.Schaal, tek.Offset, false), pt);
            gr.DrawPolygon(GetPen(false), pt);
        }

		public override int AddPunt(Punt p)
		{
			punten.Add(p);
			return punten.Count;
		}

		public override Region GetRegion(Tekening tek)
		{
			return new Region();
		}
	}
}
