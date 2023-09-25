/**
 * Jigbox
 * Copyright(c) 2016 KLab, Inc. All Rights Reserved.
 * Proprietary and Confidential - This source code is not for redistribution
 *
 * Subject to the prior written consent of KLab, Inc(Licensor) and its terms and
 * conditions, Licensor grants to you, and you hereby accept nontransferable,
 * nonexclusive limited right to access, obtain, use, copy and/or download
 * a copy of this product only for requirement purposes. You may not rent,
 * lease, loan, time share, sublicense, transfer, make generally available,
 * license, disclose, disseminate, distribute or otherwise make accessible or
 * available this product to any third party without the prior written approval
 * of Licensor. Unauthorized copying of this product, including modifications
 * of this product or programs in which this product has been merged or included
 * with other software products is expressly forbidden.
 */

using System;
using System.Collections.Generic;

namespace Jigbox.TextView
{
    public interface ISplitDenyZone
    {
        IEnumerable<MainRenderingElement> Elements { get; }

        SplitDenyGlyphSpecs Specs { get; }

        GlyphCatalog Catalog { get; }

        LineRenderingElementsAssembly.AssembleOption Option { get; }

        int ElementsCount { get; }

        float Width(MainRenderingElementPosition position);

        MainRenderingElement ElementAt(int index);
    }

    /// <summary>
    /// 分割禁止区域を取り扱うクラス
    /// </summary>
    public class SplitDenyZone : ISplitDenyZone
    {
#region fields and propeties

        private readonly List<MainRenderingElement> elements;

        public IEnumerable<MainRenderingElement> Elements
        {
            get { return this.elements; }
        }

        private readonly int elementsCount;

        public int ElementsCount
        {
            get { return elementsCount; }
        }

        readonly SplitDenyGlyphSpecs specs;

        public SplitDenyGlyphSpecs Specs
        {
            get { return specs; }
        }

        readonly GlyphCatalog catalog;

        public GlyphCatalog Catalog
        {
            get { return catalog; }
        }

        readonly LineRenderingElementsAssembly.AssembleOption option;

        public LineRenderingElementsAssembly.AssembleOption Option
        {
            get { return option; }
        }

#endregion

#region constructors

        public SplitDenyZone(IEnumerable<MainRenderingElement> elements, SplitDenyGlyphSpecs specs = null, GlyphCatalog catalog = null, LineRenderingElementsAssembly.AssembleOption assembleOption = null)
        {
            if (elements == null)
            {
                throw new ArgumentNullException("elements");
            }

            this.elements = new List<MainRenderingElement>(elements);
            this.elementsCount = this.elements.Count;

            this.specs = specs;
            this.catalog = catalog;
            this.option = assembleOption;

            if (this.elementsCount == 0)
            {
                throw new ArgumentException("elements");
            }
        }

#endregion

#region public methods

        public virtual float Width(MainRenderingElementPosition position)
        {
            var totalWidth = 0.0f;

            for (var i = 0; i < elementsCount; i++)
            {
                totalWidth += elements[i].Width(position.PositionAtIndex(i, elementsCount));
            }

            return totalWidth;
        }

        public MainRenderingElement ElementAt(int index)
        {
            if (index < 0 || index >= ElementsCount)
            {
                throw new IndexOutOfRangeException();
            }

            return elements[index];
        }

        public void Update(IEnumerable<MainRenderingElement> renderingElements)
        {
            this.elements.Clear();
            this.elements.AddRange(renderingElements);
        }

#endregion
    }

    public sealed class MinimumSplitDenyZone : ISplitDenyZone
    {
#region properties

        private static MainRenderingElement[] elementContainer = new MainRenderingElement[1];

        private MainRenderingElement element;

        public IEnumerable<MainRenderingElement> Elements
        {
            get
            {
                elementContainer[0] = element;
                return elementContainer;
            }
        }

        readonly SplitDenyGlyphSpecs specs;

        public SplitDenyGlyphSpecs Specs
        {
            get { return specs; }
        }

        readonly GlyphCatalog catalog;

        public GlyphCatalog Catalog
        {
            get { return catalog; }
        }

        readonly LineRenderingElementsAssembly.AssembleOption option;

        public LineRenderingElementsAssembly.AssembleOption Option
        {
            get { return option; }
        }

        public int ElementsCount
        {
            get { return 1; }
        }

#endregion

#region public methods

        public MinimumSplitDenyZone(MainRenderingElement element, SplitDenyGlyphSpecs specs = null, GlyphCatalog catalog = null, LineRenderingElementsAssembly.AssembleOption assembleOption = null)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            this.element = element;
            this.specs = specs;
            this.catalog = catalog;
            this.option = assembleOption;
        }

        public float Width(MainRenderingElementPosition position)
        {
            return element.Width(position.PositionAtIndex(0, 1));
        }

        public MainRenderingElement ElementAt(int index)
        {
            if (index != 0)
            {
                throw new ArgumentOutOfRangeException("index");
            }

            return element;
        }

        public void Update(MainRenderingElement renderingElements)
        {
            this.element = renderingElements;
        }

#endregion
    }
}
