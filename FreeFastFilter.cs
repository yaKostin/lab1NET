using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FreeFilter
{
    public class FreeFastFilter : IImageHandler
    {
        private const string HANDLER_NAME = "Fast free filter";

        private Bitmap bitmap;

        private string fName; // имя файла

        /*private Int16[,] RandomFilter = {
                                    { 4, 3, 2, 1, 3},
                                    { 3, 1, -5, 4, 2}, 
                                    { 2, 2, -3, 2, 1},
                                    { 5, 3, 2, 1, 4},
                                    { 4, 1, 4, 1, 6} };  */

        //private Int16[,] RandomFilter = { { -1, -1, -1 }, { -1, 16, -1 }, { -1, -1, -1 } };
        private int[,] RandomFilter;
               
        public string HandlerName
        {
            get { return HANDLER_NAME; }
        }
                  

        public void init(SortedList<string, object> parameters)
        {
            if (parameters.Keys[0] == "filter")
            {
                RandomFilter = (int[,])parameters.Values[0]; 
            }
                        
        }


        public Bitmap Source
        {
            set { bitmap = value; }
        }


        public Bitmap Result
        {
            get { return bitmap; }
        }


        public void startHandle(ProgressDelegate progressDelegate)
        {            
            Bitmap result = new Bitmap(bitmap);

            Rectangle rect = new Rectangle(0, 0, result.Width, result.Height);

            BitmapData bitmapData = result.LockBits(rect, ImageLockMode.ReadWrite, result.PixelFormat);

            IntPtr ptr = bitmapData.Scan0;

            int bytes = bitmapData.Stride * bitmapData.Height;

            byte[] rgbValues = new byte[bytes];

            
            int kernelWidth = RandomFilter.GetLength(0);
            int kernelHeight = RandomFilter.GetLength(1);
                        
            Marshal.Copy(ptr, rgbValues, 0, bytes);
                
            
            for (int x = 0; x < bitmapData.Stride; x++)
            {
                for (int y = 0; y < bitmapData.Height; y++)
                {

                    int sum = 0;

                    for (int i = -kernelWidth / 2; i < kernelWidth / 2; i++)
                    {
                        for (int j = -kernelHeight / 2; j < kernelHeight / 2; j++)
                        {
                            
                            if ((int)(((y - j) * bitmapData.Stride) + (x - i) ) >= rgbValues.Length || (int)(((y - j) * bitmapData.Stride) + (x - i) ) < 0)
                            {
                               continue;
                            } 
                                                        
                            sum += rgbValues[(int)((y - j) * bitmapData.Stride) + (x - i)] * RandomFilter[i + kernelWidth / 2, j + kernelHeight / 2];
 
                        }
                    }
                                                            
                    rgbValues[y * bitmapData.Stride + x] = (byte)(sum / (kernelHeight * kernelWidth));
                }

                    progressDelegate((double)((x+1) * bitmapData.Height) / rgbValues.Length * 100);
                
            }


            Marshal.Copy(rgbValues, 0, ptr, bytes);

            result.UnlockBits(bitmapData);

            bitmap = result;
            
        }
    }
}
