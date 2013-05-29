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

namespace PhotoQwrekster.Service
{
    public struct FloatPoint
    {
        public double X;
        public double Y;
    }

    public class Filters
    {
        internal static bool StretchAroundPoint(WriteableBitmap b, int cx, int cy, double transformScale, int transformationRadius)
        {

            int nWidth = b.PixelWidth;
            int nHeight = b.PixelHeight;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
    //        Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = cx;
            mid.Y = cy;

            int midX = cx;
            int midY = cy;

            double radius;
            double newX, newY;
            int transfornationSqwere = transformationRadius * transformationRadius;


            for (int x = 0; x < nWidth; ++x)
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

                    newX = mid.X + trueX * transformKoef;

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

                    newY = mid.Y + trueY * transformKoef;

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

              OffsetFilterAntiAlias(b, fp);
      
            return true;


        }

        internal static bool SqueezeAroundPoint(WriteableBitmap b, int cx, int cy, double transformScale, int transformationRadius)
        {

            int nWidth = b.PixelWidth;
            int nHeight = b.PixelHeight;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
       
            Point mid = new Point();
            mid.X = cx;
            mid.Y = cy;

            int midX = cx;
            int midY = cy;

            double radius;
            double newX, newY;
            int transfornationSqwere = transformationRadius * transformationRadius;

            double invertedTransformScale = 1 / transformScale;


            for (int x = 0; x < nWidth; ++x)
                for (int y = 0; y < nHeight; ++y)
                {
                    int trueX = x - midX;
                    int trueY = y - midY;
         
                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = (Math.Pow(2, (-Math.Pow(radius, 2) / transfornationSqwere)));

                    double newRadius = radius + invertedTransformScale * deltaRadius;

                    double transformKoef = newRadius / radius;
                
                    newX = mid.X + trueX * transformKoef;

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

                    newY = mid.Y + trueY * transformKoef;

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

            OffsetFilterAntiAlias(b, fp);

            return true;


        }


        internal static bool SqueezeInOneDimansion(WriteableBitmap b, int cx, int cy, bool bSmoothing)
        {

            int nWidth = b.PixelWidth;
            int nHeight = b.PixelHeight;

            FloatPoint[,] fp = new FloatPoint[nWidth, nHeight];
            Point[,] pt = new Point[nWidth, nHeight];

            Point mid = new Point();
            mid.X = cx;// nWidth / 2;
            mid.Y = cy;// nHeight / 2;

            double theta, radius;
            double newX, newY;
            double deg = 30 * Math.PI / 180;

            for (int x = 1; x < nWidth; ++x)
                for (int y = 1; y < nHeight; ++y)
                {
                    int trueX = (int)(x - mid.X);
                    int trueY = (int)(y - mid.Y);
                    theta = Math.Atan2((trueY), (trueX));

                    radius = Math.Sqrt(trueX * trueX + trueY * trueY);

                    double deltaRadius = Math.Pow(2, (-Math.Pow(radius, 2) / 90000));

                    double newRadius = radius + 3 * deltaRadius;
                    double transformKoef = newRadius / radius;
                    if (radius == 0) transformKoef = 0;
                    double a = 1 + (trueX / x) * (transformKoef - 1);
                    double d = 1 + (trueY / y) * (transformKoef - 1);

                    double nx = (x - mid.X) * transformKoef;
                    double ny = (y - mid.Y) * 1;

                    //  newX = mid.X + (newRadius * Math.Cos(theta + deg));// *Math.Cos(deg);
                    // newX = mid.X + trueX * transformKoef ;
                    // newX = a * x * Math.Cos(deg) - a* y * Math.Sin(deg);
                    // newX = mid.X + Math.Cos(deg) * nx - Math.Sin(deg) * ny;
                    newX = x + 5 * deltaRadius * Math.Cos(theta) * Math.Cos(deg);

                    if (newX > 0 && newX < nWidth)
                    {
                        fp[x, y].X = newX;
                        pt[x, y].X = (int)newX;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        pt[x, y].X = pt[x, y].Y = 0;
                    }

                    //newY = mid.Y + (radius * Math.Sin(theta ));// *Math.Cos(deg);
                    //newY = mid.Y + trueY ; 
                    //newY = d * x * Math.Sin(deg) + d* y * Math.Cos(deg);
                    //newY = mid.Y +  Math.Sin(deg) * nx + Math.Cos(deg) * ny;

                    newY = y + 5 * deltaRadius * Math.Cos(theta) * Math.Sin(deg);

                    if (newY > 0 && newY < nHeight && newX > 0 && newX < nWidth)
                    {
                        fp[x, y].Y = newY;
                        pt[x, y].Y = (int)newY;
                    }
                    else
                    {
                        fp[x, y].X = fp[x, y].Y = 0.0;
                        pt[x, y].X = pt[x, y].Y = 0;
                    }
                }

            if (bSmoothing)
                OffsetFilterAbs(b, pt);
            else
                OffsetFilterAntiAlias(b, fp);

            return true;

        }


        public static bool OffsetFilterAntiAlias(WriteableBitmap processed, FloatPoint[,] fp)
        {
            WriteableBitmap source = new  WriteableBitmap (processed);

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


        public static bool OffsetFilterAbs(WriteableBitmap processed, Point[,] offset)
        {
            WriteableBitmap source = new WriteableBitmap(processed);

            // GDI+ still lies to us - the return format is BGR, NOT RGB.
      
            //unsafe
            {
            //    byte* processedPixels = (byte*)(void*)Scan0;
              //  byte* sourcePixels = (byte*)(void*)SrcScan0;

          //      int nOffset = bmData.Stride - processed.Width * 3;
                int nWidth = processed.PixelWidth;
                int nHeight = processed.PixelHeight;

                int xOffset, yOffset;

                for (int y = 0; y < nHeight; ++y)
                {
                    for (int x = 0; x < nWidth; ++x)
                    {
                        int processedIndex = x + y * processed.PixelWidth;
                        xOffset = (int)offset[x, y].X;
                        yOffset = (int)offset[x, y].Y;
                        int sourceIndex = xOffset + yOffset * source.PixelWidth;

                        if (yOffset >= 0 && yOffset < nHeight && xOffset >= 0 && xOffset < nWidth)
                        {
                            processed.Pixels[processedIndex] = source.Pixels[sourceIndex];
                           
                        }

                       
                    }
                    
                }
            }

       
            return true;
        }


    

    }
}
