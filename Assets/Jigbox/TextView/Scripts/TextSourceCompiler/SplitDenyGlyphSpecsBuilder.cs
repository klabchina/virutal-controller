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

using System.Collections.Generic;

namespace Jigbox.TextView
{
    /// <summary>
    /// SplitDenyGlyphSpecsを生成するクラス
    /// TextSourceCompilerに使用されます
    /// </summary>
    public class SplitDenyGlyphSpecsBuilder
    {
#region enclosed stuffs

        protected enum Context
        {
            Span,
            TextModifier,
            Ruby
        }

        protected class MutableSplitDenyGlyphSpecs
        {
            public readonly List<IGlyphSpecGroup> Groups = new List<IGlyphSpecGroup>();

            public virtual SplitDenyGlyphSpecs CreateImmutable()
            {
                return new SplitDenyGlyphSpecs(Groups);
            }
        }

        protected class MutableGlyphSpecGroup
        {
            public readonly List<MainGlyphPlacementSpec> Mains = new List<MainGlyphPlacementSpec>();
            public List<FontGlyphSpec> Rubies;
            public float? RubyOffset;
            public float? RubyScale;

            public virtual GlyphSpecGroup CreateImmutable()
            {
                return new GlyphSpecGroup(
                    Mains,
                    Rubies,
                    RubyOffset,
                    RubyScale
                );
            }
        }

#endregion

#region fields & properties

        protected Stack<Context> context;

        // 外側
        protected List<List<SplitDenyGlyphSpecs>> result;

        // 真ん中
        protected MutableSplitDenyGlyphSpecs currentSplitDenyGlyphSpecs;

        // 内側
        protected MutableGlyphSpecGroup currentGlyphSpecGroup;

        protected bool? isInRuby = null;

        public virtual bool IsInRuby
        {
            get
            {
                if (isInRuby == null)
                {
                    isInRuby = this.context.Contains(Context.Ruby);
                }
                return isInRuby.Value;
            }
        }

        protected bool? isInSpan = null;

        public virtual bool IsInSpan
        {
            get
            {
                if (isInSpan == null)
                {
                    isInSpan = this.context.Contains(Context.Span);
                }
                return isInSpan.Value;
            }
        }

        protected virtual List<SplitDenyGlyphSpecs> CurrentLine
        {
            get { return this.result[this.result.Count - 1]; }
        }

#endregion

#region constructor

        public SplitDenyGlyphSpecsBuilder()
        {
            this.context = new Stack<Context>();
            this.result = new List<List<SplitDenyGlyphSpecs>>();
            this.result.Add(new List<SplitDenyGlyphSpecs>());
        }

#endregion

#region TextModifier

        public virtual void PushTextModifier(TextModifier textModifier)
        {
            if (textModifier is TextCharactersSpanGroup)
            {
                if (!this.IsInSpan)
                {
                    this.currentSplitDenyGlyphSpecs = new MutableSplitDenyGlyphSpecs();
                }

                this.context.Push(Context.Span);
            }
            else
            {
                this.context.Push(Context.TextModifier);
            }
            isInSpan = null;
        }

        public virtual void PopTextModifier()
        {
            if (this.context.Count == 0)
            {
                // スタックが空なことは期待していないので、何もせず終わる
                return;
            }
            var peeked = this.context.Peek();
            if (peeked == Context.Ruby)
            {
                // Ruby が取れることは期待していないので、何もせず終わる
                return;
            }

            if (this.context.Pop() == Context.Span)
            {
                this.isInSpan = null;
            }

            if (peeked == Context.TextModifier)
            {
                // Span 以外の TextModifier は分割禁止区域に影響を与えない
                return;
            }
            // 以降は Span が取れた場合
            if (this.IsInRuby || this.IsInSpan)
            {
                // スタックに Span か Ruby が残っていたら今の分割禁止区域を閉じない
                return;
            }
            // スタックに Span も Ruby も残っていなかったら今の分割禁止区域を閉じる（当然今のグループ（本文とルビのペア）も閉じる）

            // 今のグループを閉じる
            // ただし、本文が空なら今のグループを捨てる
            if (this.currentGlyphSpecGroup != null && this.currentGlyphSpecGroup.Mains.Count > 0)
            {
                this.currentSplitDenyGlyphSpecs.Groups.Add(this.currentGlyphSpecGroup.CreateImmutable());
            }
            // 今の分割禁止区域を閉じる
            // ただし、分割禁止区域が空なら今のグループを捨てる
            if (this.currentSplitDenyGlyphSpecs != null && this.currentSplitDenyGlyphSpecs.Groups.Count > 0)
            {
                this.CurrentLine.Add(this.currentSplitDenyGlyphSpecs.CreateImmutable());
            }
            this.currentGlyphSpecGroup = null;
            this.currentSplitDenyGlyphSpecs = null;
        }

#endregion

#region Ruby

        public virtual void StartRuby(TextModifierScope modifierScope)
        {
            if (!this.IsInRuby)
            {
                if (this.IsInSpan)
                {
                    if (this.currentGlyphSpecGroup != null)
                    {
                        this.currentSplitDenyGlyphSpecs.Groups.Add(this.currentGlyphSpecGroup.CreateImmutable());
                    }
                }
                else
                {
                    this.currentSplitDenyGlyphSpecs = new MutableSplitDenyGlyphSpecs();
                }

                this.currentGlyphSpecGroup = new MutableGlyphSpecGroup();
                this.currentGlyphSpecGroup.RubyOffset = modifierScope.RubyOffset;
                this.currentGlyphSpecGroup.RubyScale = modifierScope.RubyFontScale;
            }

            this.context.Push(Context.Ruby);
            isInRuby = null;
        }

        public virtual void EndRuby()
        {
            if (this.context.Count == 0)
            {
                // スタックが空なことは期待していないので、何もせず終わる
                return;
            }
            var peeked = this.context.Peek();
            if (peeked != Context.Ruby)
            {
                // Ruby 以外が取れることは期待していないので、何もせず終わる
                return;
            }
            this.context.Pop();
            this.isInRuby = null;

            if (this.IsInRuby)
            {
                // スタックに Ruby が残っていたら今のグループ（本文とルビのペア）はまだ続く（グループが続くので分割禁止区域も当然まだ続く）
                return;
            }

            // スタックに Ruby が残っていなかっていたら、今のグループ（本文とルビのペア）を閉じる
            // ただし、本文が空の場合は今のグループを捨てる
            if (this.currentGlyphSpecGroup.Mains.Count > 0)
            {
                this.currentSplitDenyGlyphSpecs.Groups.Add(this.currentGlyphSpecGroup.CreateImmutable());
            }
            this.currentGlyphSpecGroup = null;

            if (this.IsInSpan)
            {
                // スタックに Span が残っていたら今の分割禁止区域はまだ続く
                return;
            }

            // スタックに Span も Ruby も残っていなかったら今の分割禁止区域を閉じる
            // ただし、分割禁止区域が空の場合は今の分割禁止区域を捨てる
            if (this.currentSplitDenyGlyphSpecs.Groups.Count > 0)
            {
                this.CurrentLine.Add(this.currentSplitDenyGlyphSpecs.CreateImmutable());
            }
            this.currentSplitDenyGlyphSpecs = null;
        }

#endregion

#region FontGlyphSpec

        public virtual void AddMainGlyphPlacementSpec(MainGlyphPlacementSpec mainGlyphPlacementSpec)
        {
            if (!this.IsInSpan && !this.IsInRuby)
            {
                this.CurrentLine.Add(
                    new MinimumSplitDenyGlyphSpecs(
                        new MinimumGlyphSpecGroup(mainGlyphPlacementSpec)
                    )
                );

                return;
            }

            if (this.IsInSpan && this.currentGlyphSpecGroup == null)
            {
                this.currentGlyphSpecGroup = new MutableGlyphSpecGroup();
            }

            this.currentGlyphSpecGroup.Mains.Add(mainGlyphPlacementSpec);
        }

        public virtual void AddRubyFontRequestSpec(FontGlyphSpec rubyGlyphSpec)
        {
            if (this.IsInRuby)
            {
                if (this.currentGlyphSpecGroup.Rubies == null)
                {
                    this.currentGlyphSpecGroup.Rubies = new List<FontGlyphSpec>();
                }

                this.currentGlyphSpecGroup.Rubies.Add(rubyGlyphSpec);
            }
        }

#endregion

#region LineBreak

        public virtual void AddLineBreak()
        {
            if (this.IsInSpan || this.IsInRuby)
            {
                return;
            }
            this.result.Add(new List<SplitDenyGlyphSpecs>());
        }

#endregion

#region Build

        public virtual List<List<SplitDenyGlyphSpecs>> Build()
        {
            // このメソッドの呼び出し後に追加される可能性があるため、コピーを編集する
            var tempResult = new List<List<SplitDenyGlyphSpecs>>();
            foreach (var inner in this.result)
            {
                tempResult.Add(new List<SplitDenyGlyphSpecs>(inner));
            }

            List<IGlyphSpecGroup> tempGlyphSpecGroups;
            if (this.currentSplitDenyGlyphSpecs == null)
            {
                tempGlyphSpecGroups = new List<IGlyphSpecGroup>();
            }
            else
            {
                tempGlyphSpecGroups = new List<IGlyphSpecGroup>(this.currentSplitDenyGlyphSpecs.Groups);
            }

            if (this.currentGlyphSpecGroup != null && this.currentGlyphSpecGroup.Mains.Count > 0)
            {
                tempGlyphSpecGroups.Add(this.currentGlyphSpecGroup.CreateImmutable());
                tempResult[tempResult.Count - 1].Add(new SplitDenyGlyphSpecs(tempGlyphSpecGroups));
            }

            return tempResult;
        }

#endregion

    }
}
