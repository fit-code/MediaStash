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
    //http://zetcode.com/gui/gtksharp/layout/
    public class MainWindow : BaseWindow<VBox>
    {

        public MainWindow(string title, int width = 800, int height = 600) : base(title, width, height)
        {
            ParentContainer = new VBox();

            CreateMenu();

            Add(ParentContainer);
        }

        private void CreateMenu()
        {
            AccelMap.AddEntry("Exit", Gdk.Keyval.FromName("e"), Gdk.ModifierType.ControlMask);

            MenuBar mb = new MenuBar();

            Menu file_menu = new Menu();

            MenuItem exit_item = new MenuItem("Exit");
            exit_item.Activated += (sender, args) => Application.Quit();
            exit_item.AccelPath = "Exit";

            file_menu.Append(exit_item);

            MenuItem file_item = new MenuItem("File");
            MenuItem help_item = new MenuItem("Help");

            file_item.Submenu = file_menu;

            mb.Append(file_item);
            mb.Append(help_item);

            ParentContainer.PackStart(mb, false, false, 0);

            Toolbar toolbar = new Toolbar();
            toolbar.ToolbarStyle = ToolbarStyle.Icons;

            ToolButton newtb = new ToolButton(Stock.New);
            ToolButton opentb = new ToolButton(Stock.Open);
            ToolButton savetb = new ToolButton(Stock.Save);
            SeparatorToolItem sep = new SeparatorToolItem();
            ToolButton quittb = new ToolButton(Stock.Quit);

            toolbar.Insert(newtb, 0);
            toolbar.Insert(opentb, 1);
            toolbar.Insert(savetb, 2);
            toolbar.Insert(sep, 3);
            toolbar.Insert(quittb, 4);

            //VBox vbox = new VBox(false, 2);
            ParentContainer.PackStart(toolbar, false, false, 0);
        }

        public void delete_cb(object o, DeleteEventArgs args)
        {
            Application.Quit();
            args.RetVal = true;
        }
    }
}
