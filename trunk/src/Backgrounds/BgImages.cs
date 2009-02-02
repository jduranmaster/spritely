using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml;

namespace Spritely
{
	public class BgImages
	{
		private Document m_doc;
		private Dictionary<int, BgImage> m_bgimages;

		/// <summary>
		/// The next BgImage id available to assign.
		/// </summary>
		private int m_nNextId = 0;

		private BgImage m_bgiCurrent;

		private BgImageListForm m_winBgImageList;
		private BgImageForm m_winBgImage;

		public BgImages(Document doc)
		{
			m_doc = doc;
			m_bgimages = new Dictionary<int, BgImage>();
			m_bgiCurrent = null;

			if (m_doc.Owner != null)
			{
				m_winBgImageList = new BgImageListForm(m_doc.Owner, this); ;
				m_winBgImage = new BgImageForm(m_doc.Owner, null);
			}
		}

		public void UpdateDocument(Document doc)
		{
			m_doc = doc;

			foreach (BgImage bgi in m_bgimages.Values)
			{
				bgi.UpdateDocument(doc);
			}
		}

		public BgImageListForm BgImageListWindow
		{
			get { return m_winBgImageList; }
		}

		public BgImageForm BgImageWindow
		{
			get { return m_winBgImage; }
		}

		public Dictionary<int, BgImage>.ValueCollection AllBgImages
		{
			get { return m_bgimages.Values; }
		}

		public int NumImages
		{
			get { return m_bgimages.Count; }
		}

		public BgImage CurrentImage
		{
			get { return m_bgiCurrent; }
			set
			{
				m_bgiCurrent = value;
				m_winBgImage.SetBgImage(m_bgiCurrent);
			}
		}

		public void Clear()
		{
			m_bgimages.Clear();
			m_bgiCurrent = null;
		}

		public BgImage GetImage(int id)
		{
			return m_bgimages[id];
		}

		public void SelectFirstImage()
		{
			if (NumImages >= 1)
			{
				foreach (BgImage bgi in m_bgimages.Values)
				{
					CurrentImage = bgi;
					break;
				}
			}
			else
			{
				CurrentImage = null;
			}
		}

		public BgImage AddBgImage(string strName, int id, string strDesc, Bitmap bm)
		{
			// Auto-generate a new id.
			if (id == -1)
			{
				id = m_nNextId++;
				while (m_bgimages.ContainsKey(id))
					id = m_nNextId++;
			}

			if (m_bgimages.ContainsKey(id))
				return null;

			BgImage bgi = new BgImage(m_doc, strName, id, strDesc);
			if (bgi.LoadBitmap(bm))
			{
				m_bgimages.Add(id, bgi);
				CurrentImage = bgi;
				m_doc.Owner.HandleBackgroundImageListChanged(this);
			}
			else
				bgi = null;
			return bgi;
		}

		#region Load/Save

		public bool LoadXML_bgimages(XmlNode xnode)
		{
			foreach (XmlNode xn in xnode.ChildNodes)
			{
				switch (xn.Name)
				{
					case "bgimage_pal8":
						string strName = XMLUtils.GetXMLAttribute(xn, "name");
						int id = XMLUtils.GetXMLIntegerAttribute(xn, "id");
						string strDesc = XMLUtils.GetXMLAttribute(xn, "desc");
						string strSize = XMLUtils.GetXMLAttribute(xn, "size");

						string[] aSize = strSize.Split('x');
						int nWidth = XMLUtils.ParseInteger(aSize[0]);
						int nHeight = XMLUtils.ParseInteger(aSize[1]);

						if (m_bgimages.ContainsKey(id))
						{
							m_doc.ErrorString("A background image with id={0} already exists.", id);
							return false;
						}

						BgImage bgi = new BgImage(m_doc, strName, id, strDesc);
						if (!bgi.LoadXML_bgimage_pal8(xn, nWidth, nHeight))
							return false;

						m_bgimages.Add(id, bgi);
						CurrentImage = bgi;
						break;
				}
			}
			return true;
		}

		public void Save(System.IO.TextWriter tw)
		{
			if (m_bgimages.Count == 0)
				return;

			Export_AssignIDs();

			tw.WriteLine("\t<bgimages>");

			foreach (BgImage bgi in m_bgimages.Values)
			{
				bgi.Save(tw);
			}

			tw.WriteLine("\t</bgimages>");
		}

		#endregion

		#region Export

		public void Export_AssignIDs()
		{
			int nBgImageExportId = 0;
			foreach (BgImage bgi in m_bgimages.Values)
				bgi.Export_AssignIDs(nBgImageExportId++);
		}

		public void Export_BgImageInfo(System.IO.TextWriter tw)
		{
			foreach (BgImage bgi in m_bgimages.Values)
			{
				tw.WriteLine(String.Format("\t{{{0,4},{1,4}, BgImgPal_{2}, BgImgData_{3}}}, // BgImage_{4}",
					bgi.Bitmap.Width, bgi.Bitmap.Height, bgi.Name, bgi.Name, bgi.Name));
			}
		}

		public void Export_BgImageIDs(System.IO.TextWriter tw)
		{
			foreach (BgImage bgi in m_bgimages.Values)
			{
				tw.WriteLine(String.Format("const int kBgImage_{0} = {1};", bgi.Name, bgi.ExportId));
			}
		}

		public void Export_BgImageHeaders(System.IO.TextWriter tw)
		{
			foreach (BgImage bgi in m_bgimages.Values)
			{
				//tw.WriteLine(String.Format("#include \"{0}\"", bgi.HeaderFileName));
				tw.WriteLine("const unsigned short int BgImgPal_{0}[] = {{", bgi.Name);
				bgi.Export_BgImagePaletteData(tw);
				tw.WriteLine("};");

				tw.WriteLine("const unsigned char BgImgData_{0}[] = {{", bgi.Name);
				bgi.Export_BgImageData(tw);
				tw.WriteLine("};");
			}
		}

		public void Export_BgImagePaletteData(System.IO.TextWriter tw)
		{
			foreach (BgImage bgi in m_bgimages.Values)
			{
				bgi.Export_BgImagePaletteData(tw);
			}
		}

		public void Export_BgImageData(System.IO.TextWriter tw)
		{
			foreach (BgImage bgi in m_bgimages.Values)
			{
				bgi.Export_BgImageData(tw);
			}
		}

		#endregion

	}
}
