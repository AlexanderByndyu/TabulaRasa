﻿using System.Collections.Generic;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;

namespace Foxby.Core.DocumentBuilder.Anchors
{
	/// <summary>
	/// Contains metadata for anchors such as tags, placeholders etc.
	/// </summary>
	/// <typeparam name="TElement"><see cref="OpenXmlElement"/> representing anchor</typeparam>
	internal abstract class AnchorElement<TElement>
		where TElement : OpenXmlElement
	{
		/// <summary>
		/// Anchor name
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Fully qualified name of opening anchor
		/// </summary>
		public string OpeningName { get; private set; }

		/// <summary>
		/// Fully qualified name of closing anchor
		/// </summary>
		public string ClosingName { get; private set; }

		/// <summary>
		/// <see cref="OpenXmlElement"/> corresponding to opening anchor
		/// </summary>
		public TElement Opening { get; protected set; }

		/// <summary>
		/// <see cref="OpenXmlElement"/> corresponding to closing anchor
		/// </summary>
		public TElement Closing { get; protected set; }

		protected AnchorElement(string elementName, string openingNameFormat = null, string closingNameFormat = null)
		{
			Name = elementName;
			OpeningName = openingNameFormat == null
			              	? elementName
			              	: string.Format(openingNameFormat, elementName);
			ClosingName = closingNameFormat == null
			              	? elementName
			              	: string.Format(closingNameFormat, elementName);
		}

		protected static IEnumerable<TElement> GetElements(WordprocessingDocument document)
		{
		    var ofType = document.MainDocumentPart.Document
		        .Descendants()
		        .OfType<TElement>().ToList();

		    var elementsInHeaders = new List<TElement>();
		    foreach (var headerPart in document.MainDocumentPart.HeaderParts)
		        elementsInHeaders.AddRange(headerPart.RootElement.Descendants().OfType<TElement>());

		    ofType.AddRange(elementsInHeaders);

            var elementsInFooters = new List<TElement>();
            foreach (var footerPart in document.MainDocumentPart.FooterParts)
                elementsInFooters.AddRange(footerPart.RootElement.Descendants().OfType<TElement>());

            ofType.AddRange(elementsInFooters);

            return ofType;
		}

	    ///<summary>
	    /// Removes anchor from OpenXML document preserving its content
	    ///</summary>
	    public void Remove()
		{
			Opening.Remove();
			if (Opening != Closing)
				Closing.Remove();
		}
	}
}