﻿using Nebula.Core.Data.Chunks.FrameChunks;
using Nebula.Core.Data.Chunks.ObjectChunks.ObjectCommon;
using Nebula.Core.Data.Chunks.ObjectChunks;
using Nebula.Core.Utilities;
using Spectre.Console;
using System.Drawing.Imaging;
using System.Drawing;
using Image = Nebula.Core.Data.Chunks.BankChunks.Images.Image;
using Color = System.Drawing.Color;

namespace Nebula.Plugins.GameDumper
{
    public class FrameDump : INebulaPlugin
    {
        public string Name => "Frame Dumper";

        public void Execute()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(NebulaCore.ConsoleFiglet);
            AnsiConsole.Write(NebulaCore.ConsoleRule);

            AnsiConsole.Progress().Start(ctx =>
            {
                ProgressTask? task = ctx.AddTask($"[{NebulaCore.ColorRules[4]}]Dumping frames[/]", false);

                int progress = 0;
                string path = "Dumps\\" + Utilities.ClearName(NebulaCore.PackageData.AppName) + "\\Frames";
                while (!task.IsFinished)
                {
                    if (NebulaCore.PackageData.Frames != null)
                    {
                        if (NebulaCore.PackageData.Frames.Count == 0)
                            return;

                        if (!task.IsStarted)
                            task.StartTask();

                        task.Value = progress;
                        task.MaxValue = NebulaCore.PackageData.Frames.Count;

                        foreach (Frame frm in NebulaCore.PackageData.Frames)
                        {
                            string frmPath = Path.Combine(path, Utilities.ClearName(frm.FrameName));
                            Directory.CreateDirectory(frmPath);
                            MakeFrameImg(frm).Save(frmPath + "\\Frame.png");
                            for (int i = 0; i < frm.FrameLayers.Layers.Length; i++)
                                MakeFrameImg(frm, i).Save(frmPath + "\\Layer " + i + ".png");
                            task.Value = ++progress;
                        }
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[Red]Could not find any frames.[/]");
                        Console.ReadKey();
                    }
                }
            });
        }

        public Bitmap MakeFrameImg(Frame frm, int layer = -1)
        {
            Bitmap output = new Bitmap(frm.FrameHeader.Width, frm.FrameHeader.Height);
            Rectangle destRect;
            using (Graphics graphics = Graphics.FromImage(output))
            {
                graphics.Clear(Color.FromArgb(layer < 1 ? 255 : 0, frm.FrameHeader.Background));
                foreach (FrameInstance inst in frm.FrameInstances.Instances)
                {
                    if (layer != -1 && layer != inst.Layer)
                        continue;
                    ObjectInfo oi = NebulaCore.PackageData.FrameItems.Items[(int)inst.ObjectInfo];
                    Image? img = null;
                    float alpha = 0f;
                    if (oi.Header.InkEffect != 1)
                        alpha = oi.Header.BlendCoeff / 255.0f;
                    else
                        alpha = oi.Header.InkEffectParam * 2.0f / 255.0f;
                    switch (oi.Header.Type)
                    {
                        case 0: // Quick Backdrop
                            if (((ObjectQuickBackdrop)oi.Properties).Shape.FillType == 3)
                            {
                                img = NebulaCore.PackageData.ImageBank.Images[((ObjectQuickBackdrop)oi.Properties).Shape.Image];
                                destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                         ((ObjectQuickBackdrop)oi.Properties).Width,
                                                         ((ObjectQuickBackdrop)oi.Properties).Height);
                                doDraw(graphics, img.GetBitmap(), destRect, alpha);
                            }
                            break;
                        case 1: // Backdrop
                            img = NebulaCore.PackageData.ImageBank.Images[((ObjectBackdrop)oi.Properties).Image];
                            destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                     img.Width, img.Height);
                            doDraw(graphics, img.GetBitmap(), destRect, alpha);
                            break;
                        default:
                            ObjectCommon oc = ((ObjectCommon)oi.Properties);
                            if (layer != -1 && (!oc.NewObjectFlags["VisibleAtStart"] || oc.ObjectFlags["DontCreateAtStart"]))
                                continue;
                            switch (oi.Header.Type)
                            {
                                case 2: // Active
                                    img = NebulaCore.PackageData.ImageBank.Images[oc.ObjectAnimations.Animations.First().Value.Directions.First().Frames.First()];
                                    destRect = new Rectangle(inst.PositionX - img.HotspotX,
                                                             inst.PositionY - img.HotspotY,
                                                             img.Width, img.Height);
                                    doDraw(graphics, img.GetBitmap(), destRect, alpha);
                                    break;
                                case 7: // Counter
                                    ObjectCounter cntr = oc.ObjectCounter;
                                    Bitmap cntrImg = getCounterBmp(cntr, oc.ObjectValue);

                                    if (cntr.DisplayType == 1)
                                    {
                                        destRect = new Rectangle(inst.PositionX - cntrImg.Width,
                                                                 inst.PositionY - cntrImg.Height,
                                                                 cntrImg.Width, cntrImg.Height);
                                        doDraw(graphics, cntrImg, destRect, alpha);
                                    }
                                    else if (cntr.DisplayType == 4)
                                    {
                                        destRect = new Rectangle(inst.PositionX, inst.PositionY,
                                                                 cntrImg.Width, cntrImg.Height);
                                        doDraw(graphics, cntrImg, destRect, alpha);
                                    }
                                    break;
                            }
                            break;
                    }
                }
            }
            return output;
        }

        private void doDraw(Graphics g, Bitmap sourceBitmap, Rectangle dest, float alpha)
        {
            using (ImageAttributes imageAttributes = new ImageAttributes())
            {
                ColorMatrix colorMatrix = new ColorMatrix();
                colorMatrix.Matrix33 = 1 - alpha;
                imageAttributes.SetColorMatrix(colorMatrix);
                imageAttributes.SetWrapMode(System.Drawing.Drawing2D.WrapMode.Tile);

                g.DrawImage(
                    sourceBitmap,
                    dest,
                    0, 0, dest.Width, dest.Height,
                    GraphicsUnit.Pixel,
                    imageAttributes
                );
            }
        }

        Dictionary<char, uint> counterID = new Dictionary<char, uint>()
        {
            { '0',  0 },
            { '1',  1 },
            { '2',  2 },
            { '3',  3 },
            { '4',  4 },
            { '5',  5 },
            { '6',  6 },
            { '7',  7 },
            { '8',  8 },
            { '9',  9 },
            { '-', 10 },
            { '+', 11 },
            { '.', 12 },
            { 'e', 13 },
        };

        private Bitmap getCounterBmp(ObjectCounter cntr, ObjectValue val)
        {
            Bitmap bmp = null;
            Graphics g = null;
            if (cntr.DisplayType == 1)
            {
                int width = 0;
                int height = 0;
                foreach (char c in val.Initial.ToString())
                {
                    uint id = counterID[c];
                    Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    width += img.Width;
                    height = Math.Max(height, img.Height);
                }
                bmp = new Bitmap(width, height);
                g = Graphics.FromImage(bmp);
                int? prevX = null;
                foreach (char c in val.Initial.ToString().Reverse())
                {
                    uint id = counterID[c];
                    Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[id]];
                    int xToDraw = width - img.Width;
                    if (prevX != null)
                        xToDraw = (int)prevX - img.Width;
                    g.DrawImageUnscaled(img.GetBitmap(), xToDraw, 0);
                    prevX = xToDraw;
                }
            }
            else if (cntr.DisplayType == 4)
            {
                double ratio = (double)(val.Initial - val.Minimum) / (val.Maximum - val.Minimum);
                Image img = NebulaCore.PackageData.ImageBank.Images[cntr.Frames[(int)((cntr.Frames.Length - 1) * ratio)]];
                bmp = new Bitmap(img.Width, img.Height);
                g = Graphics.FromImage(bmp);
                g.DrawImageUnscaled(img.GetBitmap(), 0, 0);
            }

            if (g != null)
                g.Dispose();

            return bmp;
        }
    }
}