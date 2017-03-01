﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace Chart
{
    public sealed class Core: IDisposable
    {
        private static Core _core;
        private Model _model;
        private Form _wnd;

        private Matrix _rotation =
            Matrix.CreateIdentity()*
            Matrix.CreateRotateX(90);

        private Core() { }

        public static Core Instance
        {
            get { return _core ?? (_core = new Core()); }
        }

        public void Run()
        {
            Application.EnableVisualStyles();
            Application.Run(Window);
        }

        public void Dispose()
        {
            if (_wnd != null)
            {
                _wnd.Dispose();
                _wnd = null;
            }
        }

        [STAThread]
        public static void Main()
        {
            using (Core core = Instance)
                core.Run();
        }

        public void Draw()
        {
            using (var bmp = new Bitmap(Window.ClientSize.Width, Window.ClientSize.Height))
            {
                using (var gfx = Graphics.FromImage(bmp))
                {
                    gfx.Clear(Color.WhiteSmoke);
                    gfx.SmoothingMode = SmoothingMode.AntiAlias;
                    Model.Draw(gfx, bmp.Width, bmp.Height, _rotation);
                }

                using (var gfx = Window.CreateGraphics())
                    gfx.DrawImageUnscaled(bmp, 0, 0);
            }
        }

        public Form Window
        {
            get
            {
                if (_wnd == null)
                {
                    _wnd = new Form
                    {
                        StartPosition = FormStartPosition.CenterScreen,
                        Width = 1024,
                        Height = 768
                    };

                    _wnd.Paint += (sender, e) => Draw();
                    _wnd.Resize += (sender, e) => Draw();
                    _wnd.Cursor = Cursors.SizeAll;

                    bool isMouseDown = false;
                    int prevMouseX = 0, prevMouseY = 0;

                    _wnd.MouseDown += (sender, e) =>
                    {
                        if (!isMouseDown)
                        {
                            isMouseDown = true;
                            prevMouseX = e.X;
                            prevMouseY = e.Y;
                        }
                    };

                    _wnd.MouseUp += (sender, e) =>
                    {
                        isMouseDown = false;
                    };

                    _wnd.MouseMove += (sender, e) =>
                    {
                        if (isMouseDown)
                        {
                            int dx = e.X - prevMouseX;
                            int dy = e.Y - prevMouseY;

                            prevMouseX = e.X;
                            prevMouseY = e.Y;

                            _rotation *= Matrix.CreateRotateY(dx) * Matrix.CreateRotateX(-dy);
                            Draw();
                        }
                    };
                }

                return _wnd;
            }
        }

        public IEnumerable<string> ModelFiles
        {
            get
            {
                for (int i = 1;; ++i)
                {
                    string fileName = Path.Combine(Application.StartupPath, i + ".txt");
                    if (File.Exists(fileName))
                        yield return fileName;
                    else
                        yield break;
                }
            }
        }

        public Model Model
        {
            get { return _model ?? (_model = new Model(ModelFiles)); }
        }
    }
}
