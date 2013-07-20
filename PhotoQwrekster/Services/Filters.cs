using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using PhotoQwrekster.ViewModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace PhotoQwrekster.Service
{
    public struct FloatPoint
    {
        public double X;
        public double Y;
    }

    public class Filters
    {
        internal static FloatPoint[,] StretchAroundPoint(int pixelWidth, int pixelHeight,
            int midX, int midY, double transformScale, int transformationRadius,
            BackgroundWorker backgroundWorker)
        {

            int nWidth = pixelWidth;
            int nHeight = pixelHeight;

            fp = new FloatPoint[nWidth, nHeight];

            int transfornationSqwere = transformationRadius * transformationRadius;

            Action<object> act = new Action<object>(RunParametrizedStretchAroundPoint);
            RunTasks(transformScale, 0, backgroundWorker, nWidth, nHeight, midX, midY, transfornationSqwere, act);


            return fp;
        }

        static void RunParametrizedStretchAroundPoint(object par)
        {
            List<object> parametr = (List<object>)par;
            MatrixRacalculateStretchAroundPoint((double)parametr[0],
                (double)parametr[1], // degree                
                (int)parametr[2],  // nWidth
                (int)parametr[3],  // nHeight
                (int)parametr[4],  // midX
                (int)parametr[5],  // midY
                (int)parametr[6],  // transfornationSqwere,
                (int)parametr[7],  // from
                (int)parametr[8],  // to
                (BackgroundWorker)parametr[9]);
        }

        private static void MatrixRacalculateStretchAroundPoint(double transformScale, double deg,
            int nWidth, int nHeight,
            int midX, int midY, int transfornationSqwere,
            int from, int to, BackgroundWorker backgroundWorker)
        {
            double radius;
            double newX, newY;

            int f5Percent = nWidth / 18;
            int nextPercentage = from + f5Percent;
            
            for (int x = from; x < to; ++x)
            {
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - midX;
                    int trueY = y - midY;
                    //  theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = (Math.Pow(2, (-Math.Pow(radius, 2) / transfornationSqwere)));

                    double newRadius = radius - transformScale * deltaRadius;

                    double transformKoef = newRadius / radius;
                    if (radius < transformScale) transformKoef = 0.1;

                    newX = midX + trueX * transformKoef;

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        //           pt[x, y].X = newX;

                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        //             pt[x, y].X = pt[x, y].Y = 0;
                    }

                    newY = midY + trueY * transformKoef;

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                        //        pt[x, y].Y = newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        //      pt[x, y].X = pt[x, y].Y = 0;
                    }
                }

                if (x > nextPercentage)
                {
                    backgroundWorker.ReportProgress(5);
                    nextPercentage += f5Percent;
                }
            }
        }


        internal static FloatPoint[,] SqueezeAroundPoint(int pixelWidth, int pixelHeight,
            int midX, int midY, double transformScale, int transformationRadius,
            BackgroundWorker backgroundWorker)
        {

            int nWidth = pixelWidth;
            int nHeight = pixelHeight;

            fp = new FloatPoint[nWidth, nHeight];
       
            int transfornationSqwere = transformationRadius * transformationRadius;

            double invertedTransformScale = 1 / transformScale;


            Action<object> act = new Action<object>(RunParametrizedSqueezeAroundPoint);
            RunTasks(invertedTransformScale, 0, backgroundWorker, nWidth, nHeight, midX, midY, transfornationSqwere, act);


            return fp;
            
        }

        static void RunParametrizedSqueezeAroundPoint(object par)
        {
            List<object> parametr = (List<object>)par;
            MatrixRacalculateSqueezeAroundPoint((double)parametr[0],
                (double)parametr[1], // degree                
                (int)parametr[2],  // nWidth
                (int)parametr[3],  // nHeight
                (int)parametr[4],  // midX
                (int)parametr[5],  // midY
                (int)parametr[6],  // transfornationSqwere,
                (int)parametr[7],  // from
                (int)parametr[8],  // to
                (BackgroundWorker)parametr[9]);
        }

        private static void MatrixRacalculateSqueezeAroundPoint(double invertedTransformScale, double deg,
            int nWidth, int nHeight,
            int midX, int midY, int transfornationSqwere,
            int from, int to, BackgroundWorker backgroundWorker)
        {
            double radius;
            double newX, newY;

            int f5Percent = nWidth / 18;
            int nextPercentage = from + f5Percent;

            for (int x = from; x < to; ++x)
            {
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - midX;
                    int trueY = y - midY;

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = (Math.Pow(2, (-Math.Pow(radius, 2) / transfornationSqwere)));

                    double newRadius = radius + invertedTransformScale * deltaRadius;

                    double transformKoef = newRadius / radius;

                    newX = midX + trueX * transformKoef;

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        //           pt[x, y].X = newX;

                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        //             pt[x, y].X = pt[x, y].Y = 0;
                    }

                    newY = midY + trueY * transformKoef;

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                        //        pt[x, y].Y = newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        //      pt[x, y].X = pt[x, y].Y = 0;
                    }
                }

                if (x > nextPercentage)
                {
                    backgroundWorker.ReportProgress(5);
                    nextPercentage += f5Percent;
                }
            }
        }

        internal static FloatPoint[,] StretchInOneDimension(int pixelWidth, int pixelHeight,
            int midX, int midY, double transformScale, int transformationRadius, double deg,
            BackgroundWorker backgroundWorker)
        {

            int nWidth = pixelWidth;
            int nHeight = pixelHeight;

            fp = new FloatPoint[nWidth, nHeight];

            int transfornationSqwere = transformationRadius * transformationRadius;

            Action<object> act = new Action<object>(RunParametrizedStretchInOneDimension);
            RunTasks(transformScale, deg, backgroundWorker, nWidth, nHeight, midX, midY, transfornationSqwere, act);

            
            return fp;

        }

        static void RunParametrizedStretchInOneDimension(object par)
        {
            List<object> parametr = (List<object>)par;
            MatrixRacalculateStretchInOneDimension((double)parametr[0],
                (double)parametr[1], // degree                
                (int)parametr[2],  // nWidth
                (int)parametr[3],  // nHeight
                (int)parametr[4],  // midX
                (int)parametr[5],  // midY
                (int)parametr[6],  // transfornationSqwere,
                (int)parametr[7],  // from
                (int)parametr[8],  // to
                (BackgroundWorker)parametr[9]);
        }

        private static void MatrixRacalculateStretchInOneDimension(double transformScale, double deg,
            int nWidth, int nHeight,
            int midX, int midY, int transfornationSqwere,
            int from, int to, BackgroundWorker backgroundWorker)
        {
            double theta, radius;
            double newX, newY;

            int f5Percent = nWidth / 18;
            int nextPercentage = from + f5Percent;

            for (int x = from; x < to; ++x)
            {
                for (int y = 1; y < nHeight; ++y)
                {
                    int trueX = (int)(x - midX);
                    int trueY = (int)(y - midY);
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = Math.Pow(2, (-Math.Pow(radius, 2) / transfornationSqwere));

                    newX = x - transformScale * deltaRadius * Math.Cos(theta + deg) * Math.Cos(deg);

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                    }

                    newY = y + transformScale * deltaRadius * Math.Cos(theta + deg) * Math.Sin(deg);

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                    }

                }
                if (x > nextPercentage)
                {
                    backgroundWorker.ReportProgress(5);
                    nextPercentage += f5Percent;
                }
            }
        }

        static FloatPoint[,] fp;

        internal static FloatPoint[,] SqueezeInOneDimension(int pixelWidth, int pixelHeight, 
            int midX, int midY, double transformScale, int transformationRadius, double deg, 
            BackgroundWorker backgroundWorker)
        {

            int nWidth = pixelWidth;
            int nHeight = pixelHeight;

            fp = new FloatPoint[nWidth, nHeight];

            int transfornationSqwere = transformationRadius * transformationRadius;
            transformScale = 1 / transformScale;

            Action<object> act = new Action<object>(RunParametrizedSqueezeInOneDimension);
            RunTasks(transformScale, deg, backgroundWorker, nWidth, nHeight, midX, midY, transfornationSqwere, act);

            return fp;

        }

        private static void RunTasks(double transformScale, double deg, BackgroundWorker backgroundWorker, int nWidth, int nHeight, int midX, int midY, int transfornationSqwere, Action<object> act)
        {
            Task[] tsk = new Task[2];
            int halfWidth = (int)nWidth / 2;
            tsk[0] = Task.Factory.StartNew(act, (object)(new List<object>() {
                transformScale, deg,  
                nWidth, nHeight, midX, midY, transfornationSqwere, 0, halfWidth, backgroundWorker }));

            tsk[1] = Task.Factory.StartNew(act, (object)(new List<object>() { 
                transformScale, deg, 
                nWidth, nHeight, midX, midY, transfornationSqwere, halfWidth, nWidth, backgroundWorker }));


            try
            {
                Task.WaitAll(tsk);
            }
            catch (Exception ex)
            {
                string str = ex.StackTrace.ToString();
            }
            return;
        }

        static void RunParametrizedSqueezeInOneDimension(object par)
        {
            List<object> parametr = (List<object>)par;
            MatrixRacalculateSqueezeInOneDimension((double)parametr[0],
                (double)parametr[1], // degree                
                (int)parametr[2],  // nWidth
                (int)parametr[3],  // nHeight
                (int)parametr[4],  // midX
                (int)parametr[5],  // midY
                (int)parametr[6],  // transfornationSqwere,
                (int)parametr[7],  // from
                (int)parametr[8],  // to
                (BackgroundWorker)parametr[9]);
        }

        private static void MatrixRacalculateSqueezeInOneDimension(double transformScale, double deg, 
            int nWidth, int nHeight, 
            int midX, int midY, int transfornationSqwere,
            int from, int to, BackgroundWorker backgroundWorker)
        {
            double theta, radius;
            double newX, newY;

            int f5Percent = nWidth / 18;
            int nextPercentage = from + f5Percent;

            for (int x = from; x < to; x++) 
            {
                for (int y = 1; y < nHeight; ++y)
                {
                    int trueX = (int)(x - midX);
                    int trueY = (int)(y - midY);
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = Math.Pow(2, (-Math.Pow(radius, 2) / transfornationSqwere));

                    newX = x + transformScale * deltaRadius * Math.Cos(theta + deg) * Math.Cos(deg);

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                    }

                    newY = y - transformScale * deltaRadius * Math.Cos(theta + deg) * Math.Sin(deg);

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                    }
                }
                if (x > nextPercentage)
                {
                    backgroundWorker.ReportProgress(5);
                    nextPercentage += f5Percent;
                }

            }
        }

        public static bool OffsetFilterAntiAlias(WriteableBitmap processed, WriteableBitmap source, FloatPoint[,] fp)
        {  
            int scanline = 1; 

            {
                int nWidth = processed.PixelWidth;
                int nHeight = processed.PixelHeight;

               

                int MaxSize = nWidth * nHeight;

                double xOffset, yOffset;

                double fraction_x, fraction_y, one_minus_x, one_minus_y;
                int ceil_x, ceil_y, floor_x, floor_y;
                double p1, p2;

                for (int y = 0; y < nHeight; y++)
                {
                    for (int x = 0; x < nWidth; x++)
                    {
                        xOffset = fp[x, y].X;
                        yOffset = fp[x, y].Y;

                        // Setup

                        floor_x = (int)Math.Floor(xOffset);
                        floor_y = (int)Math.Floor(yOffset);
                        ceil_x = floor_x + 1;
                        ceil_y = floor_y + 1;
                        fraction_x = xOffset - floor_x;
                        fraction_y = yOffset - floor_y;
                        one_minus_x = 1.0 - fraction_x;
                        one_minus_y = 1.0 - fraction_y;

                        scanline = nWidth;
                        int processedIndex = x + y * nWidth;

       
                        if (floor_y >= 0 && ceil_y < nHeight && floor_x >= 0 && ceil_x < nWidth)
                        {
                            // color component 1
                            if (processedIndex < MaxSize)
                            {

                                p1 = one_minus_x * (int)(source.Pixels[floor_y * scanline + floor_x] >> 16) +
                                    fraction_x * (int)(source.Pixels[floor_y * scanline + ceil_x] >> 16);

                                p2 = (one_minus_x * (int)(source.Pixels[ceil_y * scanline + floor_x]>>16) +
                                    fraction_x * (int)(source.Pixels[ceil_y * scanline + ceil_x]>>16));

                                int coler1 = (int)(one_minus_y * p1 + fraction_y * p2);


                                p1 = one_minus_x * (byte)(source.Pixels[floor_y * scanline + floor_x] >> 8) +
                                   fraction_x * (byte)(source.Pixels[floor_y * scanline + ceil_x] >> 8);

                                p2 = (one_minus_x * (byte)(source.Pixels[ceil_y * scanline + floor_x] >> 8) +
                                    fraction_x * (byte)(source.Pixels[ceil_y * scanline + ceil_x] >> 8));

                                int coler2 = (byte)(one_minus_y * p1 + fraction_y * p2);


                                p1 = one_minus_x * (byte)(source.Pixels[floor_y * scanline + floor_x]) +
                                   fraction_x * (byte)(source.Pixels[floor_y * scanline + ceil_x]);

                                p2 = (one_minus_x * (byte)(source.Pixels[ceil_y * scanline + floor_x]) +
                                    fraction_x * (byte)(source.Pixels[ceil_y * scanline + ceil_x]));

                                int coler3 = (byte)(one_minus_y * p1 + fraction_y * p2);

          
                                processed.Pixels[processedIndex] = (coler1 << 16) +(coler2 << 8) + coler3;
                            }
                            /////
                     
                        }
                    }
                }
            }

            return true;
        }
    }
}
