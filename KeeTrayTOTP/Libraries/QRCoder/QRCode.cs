/*
This file was taken from https://github.com/codebude/QRCoder and is licensed under the MIT license.
---------------------
The MIT License (MIT)

Copyright (c) 2013-2015 Raffael Herrmann

Permission is hereby granted, free of charge, to any person obtaining a copy of
this software and associated documentation files (the "Software"), to deal in
the Software without restriction, including without limitation the rights to
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
the Software, and to permit persons to whom the Software is furnished to do so,
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
namespace QRCoder
{
    using System;
    using System.Drawing;

    public sealed class QRCode : IDisposable
    {
        private QRCodeData QrCodeData;
        private static readonly SolidBrush darkBrush = new SolidBrush(Color.Black);
        private static readonly SolidBrush lightBrush = new SolidBrush(Color.White);

        public QRCode(QRCodeData data)
        {
            this.QrCodeData = data;
        }

        public Bitmap GetGraphic(int pixelsPerModule, bool drawQuietZones = true)
        {
            var size = (this.QrCodeData.ModuleMatrix.Count - (drawQuietZones ? 0 : 8)) * pixelsPerModule;
            var offset = drawQuietZones ? 0 : 4 * pixelsPerModule;

            var bmp = new Bitmap(size, size);
            using (var gfx = Graphics.FromImage(bmp))
            {
                for (var x = 0; x < size + offset; x += pixelsPerModule)
                {
                    for (var y = 0; y < size + offset; y += pixelsPerModule)
                    {
                        var module = this.QrCodeData.ModuleMatrix[(y + pixelsPerModule) / pixelsPerModule - 1][(x + pixelsPerModule) / pixelsPerModule - 1];
                        var rect = new Rectangle(x - offset, y - offset, pixelsPerModule, pixelsPerModule);
                        var brush = module ? darkBrush : lightBrush;

                        gfx.FillRectangle(brush, rect);
                    }
                }

                gfx.Save();
            }

            return bmp;
        }

        public void Dispose()
        {
            if (this.QrCodeData != null)
            {
                QrCodeData.Dispose();
            }

            this.QrCodeData = null;
        }
    }
}
