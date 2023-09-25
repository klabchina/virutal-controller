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

namespace Jigbox.TextView
{
    public class TextRunPointer
    {
        public int Position { get; set; }

        public int TextRunIndex { get; set; }

        public TextSource TextSource { get; set; }

        public TextRun Current { get; private set; }

        public TextRun Previous
        {
            get
            { 
                return this.TextRunIndex != 0 ?
                    this.TextSource.TextRuns[this.TextRunIndex - 1]
                        : this.TextSource.TextRuns[this.TextRunIndex];
            }
        }

        public bool CanRead
        {
            get { return !this.EndOfSource && (this.Position < this.Current.Length); }
        }

        public bool EndOfSource
        {
            get { return this.TextRunIndex == this.TextSource.TextRuns.Length; }
        }

        public TextRunPointer(TextSource source)
        {
            this.TextRunIndex = 0;
            this.Position = -1;
            this.TextSource = source;

            // Instead UpdateCurrent();
            if (this.TextRunIndex > this.TextSource.TextRuns.Length - 1)
            {
                this.Current = null;
            }
            else
            {
                this.Current = this.TextSource.TextRuns[this.TextRunIndex];
            }
        }

        public void Initialize()
        {
            this.UpdateCurrent();
        }

        void UpdateCurrent()
        {
            if (this.TextRunIndex > this.TextSource.TextRuns.Length - 1)
            {
                this.Current = null;
            }
            else
            {
                this.Current = this.TextSource.TextRuns[this.TextRunIndex];
            }
        }

        public void BackChunk()
        {
            this.TextRunIndex--;
            this.UpdateCurrent();
            this.Position = this.Current.Length - 1;
        }

        public void NextChunk()
        {
            this.TextRunIndex++;
            this.UpdateCurrent();
            this.Position = -1;
        }

        public bool Back()
        {
            this.Position--;
            if (this.Position < -1)
            {
                this.BackChunk();
                return true;
            }
            return false;
        }

        public bool Next()
        {
            if (this.EndOfSource)
            {
                throw new InvalidOperationException();
            }
            this.Position++;
            if (this.Position > this.Current.Length - 1)
            {
                this.NextChunk();
                return true;
            }
            return false;
        }

        public string PreviousTextCharacters(int prevCount)
        {
            int count = 0;
            for (int i = this.TextRunIndex - 1; i >= 0; i--)
            {
                if (this.TextSource.TextRuns[i] is TextCharacters)
                {
                    count++;
                    if (prevCount <= count)
                    {
                        return ((TextCharacters) TextSource.TextRuns[i]).RawCharacters;
                    }
                }
            }
                
            return null;
        }
    }
}
