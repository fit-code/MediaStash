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
using System;

namespace Fitcode.MediaStash.Desktop.Views
{
    public class MainWindow : BaseWindow
    {
        public MainWindow(string title, int width = 800, int height = 600) : base(title, width, height)
        {
            DeleteEvent += new DeleteEventHandler(delete_cb);


            VBox box = new VBox(false, 2);

            MenuBar mb = new MenuBar();
            Menu file_menu = new Menu();
            MenuItem exit_item = new MenuItem("Exit");
            exit_item.Activated += (sender, args) => Application.Quit();
            file_menu.Append(exit_item);
            MenuItem file_item = new MenuItem("File");
            file_item.Submenu = file_menu;
            mb.Append(file_item);
            box.PackStart(mb, false, false, 0);

            Button btn = new Button("Yep, that's a menu");
            box.PackStart(btn, false, false, 0);

            Add(box);
        }

        public void delete_cb(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }
    }
}
