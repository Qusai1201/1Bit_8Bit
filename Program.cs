using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

#pragma warning disable CA1416

class Converter
{
    public unsafe Bitmap To_8Bpp(Bitmap BinImage)
    {

        if (BinImage.PixelFormat != PixelFormat.Format1bppIndexed)
        {
            throw new ArgumentException("the image should be 1bpp.");
        }

        int Width = BinImage.Width, Height = BinImage.Height;

        BitmapData BinData = BinImage.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly, PixelFormat.Format1bppIndexed);

        Bitmap GrayImage = new Bitmap(Width, Height, PixelFormat.Format8bppIndexed);
        BitmapData GrayData = GrayImage.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);


        byte[] BitBytes = new byte[BinData.Stride * Height];
        byte[] GrayBytes = new byte[GrayData.Stride * Height];

        Marshal.Copy(BinData.Scan0, BitBytes, 0, BitBytes.Length);

        int BinStride = BinData.Stride;
        int GrayStride = GrayData.Stride;

        for (int y = 0; y < Height; y++)
        {

            int Binrow = BinStride * y;
            int Grayrow = GrayStride * y;

            for (int x = 0; x < Width; x++)
            {
                int Index = Binrow + (x >> 3);
                int mask = 0x80 >> (x % 8);
                int bit = (BitBytes[Index] & mask) == 0 ? 0 : 1;
                GrayBytes[Grayrow + x] = (byte)(bit * 255);
            }
        }

        Marshal.Copy(GrayBytes, 0, GrayData.Scan0, GrayBytes.Length);


        GrayImage.UnlockBits(GrayData);
        BinImage.UnlockBits(BinData);

        return GrayImage;
    }
    public void Run()
    {
        string dir = @"results";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Console.Write("enter the image path : ");
        string path;
        path = Console.ReadLine();

        Bitmap BinImage = new Bitmap(path);

        var watch = System.Diagnostics.Stopwatch.StartNew();

        To_8Bpp(BinImage).Save("results/result.bmp"); ;

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        Console.WriteLine("Time taken: {0}ms", elapsedMs);
    }
    public static void Main(string[] args)
    {
        Converter Conv = new Converter();
        Conv.Run();
    }
}
