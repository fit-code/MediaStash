#region License
// Copyright (c) 2017 Fitcode.io (info@fitcode.io)
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
#endregion

using Gtk;

namespace Fitcode.MediaStash.Desktop.Views
{
    public abstract class BaseWindow<T> : Window where T : Container
    {
        public T ParentContainer { get; set; }

        public BaseWindow(string title, int width = 800, int height = 600) : base(title)
        {
            DefaultWidth = width;
            DefaultHeight = height;

            SetPosition(WindowPosition.Center);
            SetIconFromFile("icon.ico");

            DeleteEvent += new DeleteEventHandler(HandleDelete);
        }

        public void HandleDelete(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }

        public override void Dispose()
        {
            if (ParentContainer != null)
            {
                ParentContainer.Dispose();
                ParentContainer = null;
            }

            base.Dispose();
        }
    }
}
