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

namespace Jigbox.TextView
{
    public interface IGlyphSpecGroup
    {
        int MainsCount { get; }

        int RubiesCount { get; }

        float? RubyOffset { get; }

        float? RubyScale { get; }

        IEnumerable<MainGlyphPlacementSpec> Mains { get; }

        IEnumerable<FontGlyphSpec> Rubies { get; }
    }

    public class GlyphSpecGroup : IGlyphSpecGroup
    {
#region fields and propeties

        private readonly List<MainGlyphPlacementSpec> mains;
        private readonly List<FontGlyphSpec> rubies;

        protected readonly int mainsCount;
        protected readonly int rubiesCount;

        protected readonly float? rubyOffset;
        protected readonly float? rubyScale;

        public int MainsCount
        {
            get { return mainsCount; }
        }

        public int RubiesCount
        {
            get { return rubiesCount; }
        }

        public float? RubyOffset
        {
            get
            {
                return rubyOffset;
            }
        }

        public float? RubyScale
        {
            get { return rubyScale; }
        }

        public IEnumerable<MainGlyphPlacementSpec> Mains
        {
            get { return this.mains; }
        }

        public IEnumerable<FontGlyphSpec> Rubies
        {
            get { return this.rubies; }
        }

#endregion

#region constructors

        public GlyphSpecGroup(
            IEnumerable<MainGlyphPlacementSpec> mains,
            IEnumerable<FontGlyphSpec> rubies,
            float? rubyOffset = null,
            float? rubyScale = null
        )
        {
            if (mains == null)
            {
                throw new ArgumentNullException("mains");
            }

            this.mains = new List<MainGlyphPlacementSpec>(mains);
            this.mainsCount = this.mains.Count;
            if (this.MainsCount == 0)
            {
                throw new ArgumentException("mains");
            }

            if (rubies == null)
            {
                this.rubies = new List<FontGlyphSpec>();
                this.rubiesCount = 0;
            }
            else
            {
                this.rubies = new List<FontGlyphSpec>(rubies);
                this.rubiesCount = this.rubies.Count;
            }

            this.rubyOffset = rubyOffset;
            this.rubyScale = rubyScale;
        }

#endregion
    }

    public sealed class MinimumGlyphSpecGroup : IGlyphSpecGroup
    {
#region inner classes, enum, and structs

        public class Enumerable : IEnumerable<MainGlyphPlacementSpec>
        {
            public class Enumerator : IEnumerator<MainGlyphPlacementSpec>
            {
                static MainGlyphPlacementSpec[] container = new MainGlyphPlacementSpec[1];

                public MainGlyphPlacementSpec Main { get { return container[0]; } set { container[0] = value; } }

                MainGlyphPlacementSpec current = null;

                public MainGlyphPlacementSpec Current { get { return current; } }

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

            public MainGlyphPlacementSpec Main { get { return enumerator.Main; } set { enumerator.Main = value; } }

            public IEnumerator<MainGlyphPlacementSpec> GetEnumerator()
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

        public class RubyEnumerable : IEnumerable<FontGlyphSpec>
        {
            public class Enumerator : IEnumerator<FontGlyphSpec>
            {
                public FontGlyphSpec Current { get { return null; } }

                object IEnumerator.Current { get { return null; } }

                public bool MoveNext()
                {
                    return false;
                }

                public void Reset()
                {
                }

                public void Dispose()
                {
                }
            }

            Enumerator enumerator = new Enumerator();

            public IEnumerator<FontGlyphSpec> GetEnumerator()
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

#region constants

        static readonly RubyEnumerable rubiesContainer = new RubyEnumerable();

#endregion

#region properties

        static Enumerable mainContainer = new Enumerable();

        MainGlyphPlacementSpec mainGlyphSpec;

        public int MainsCount { get { return 1; } }

        public int RubiesCount { get { return 0; } }

        public float? RubyOffset { get { return null; } }

        public float? RubyScale { get { return null; } }

        public IEnumerable<MainGlyphPlacementSpec> Mains
        {
            get
            {
                mainContainer.Main = mainGlyphSpec;
                return mainContainer; 
            }
        }

        public IEnumerable<FontGlyphSpec> Rubies { get { return rubiesContainer; } }

#endregion

#region public methods

        public MinimumGlyphSpecGroup(MainGlyphPlacementSpec mainGlyphSpec)
        {
            if (mainGlyphSpec == null)
            {
                throw new ArgumentNullException("mainGlyphSpec");
            }

            this.mainGlyphSpec = mainGlyphSpec;
        }

#endregion
    }
}
