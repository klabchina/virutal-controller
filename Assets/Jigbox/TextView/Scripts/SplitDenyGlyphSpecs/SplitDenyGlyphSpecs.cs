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
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Jigbox.TextView
{
    /// <summary>
    /// 論理行を取り扱うクラス
    /// </summary>
    public class SplitDenyGlyphSpecs
    {
        private readonly List<IGlyphSpecGroup> groups;

        public virtual IEnumerable<IGlyphSpecGroup> Groups
        {
            get { return this.groups; }
        }

        public int MainsCount { get; protected set; }

        public SplitDenyGlyphSpecs(IEnumerable<IGlyphSpecGroup> groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException("groups");
            }

            this.groups = new List<IGlyphSpecGroup>(groups);
            if (!this.groups.Any())
            {
                throw new ArgumentException("groups");
            }

            foreach (var group in this.groups) {
                this.MainsCount += group.MainsCount;
            }
        }

        protected SplitDenyGlyphSpecs()
        {
        }
    }

    public sealed class MinimumSplitDenyGlyphSpecs : SplitDenyGlyphSpecs
    {
#region inner classes, enum, and structs

        public class Enumerable : IEnumerable<IGlyphSpecGroup>
        {
            public class Enumerator : IEnumerator<IGlyphSpecGroup>
            {
                static IGlyphSpecGroup[] container = new IGlyphSpecGroup[1];

                public IGlyphSpecGroup Group { get { return container[0]; } set { container[0] = value; } }

                IGlyphSpecGroup current = null;

                public IGlyphSpecGroup Current { get { return current; } }

                object IEnumerator.Current { get { return current; } }

                public bool MoveNext()
                {
                    if (current == null)
                    {
                        current = container[0];
                        return true;
                    }
                    else
                    {
                        current = null;
                        return false;
                    }
                }

                public void Reset()
                {
                    current = null;
                }

                public void Dispose()
                {
                }
            }

            Enumerator enumerator = new Enumerator();

            public IGlyphSpecGroup Group { get { return enumerator.Group; } set { enumerator.Group = value; } }

            public IEnumerator<IGlyphSpecGroup> GetEnumerator()
            {
                enumerator.Reset();
                return enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                enumerator.Reset();
                return enumerator;
            }
        }


#endregion

#region properties

        static Enumerable groupContainer = new Enumerable();

        IGlyphSpecGroup group;

        public override IEnumerable<IGlyphSpecGroup> Groups
        {
            get
            {
                groupContainer.Group = group;
                return groupContainer; 
            }
        }

#endregion

#region public methods

        public MinimumSplitDenyGlyphSpecs(IGlyphSpecGroup group) : base()
        {
            if (group == null)
            {
                throw new ArgumentNullException("group");
            }

            this.group = group;
            MainsCount = 1;
        }

#endregion
    }
}
