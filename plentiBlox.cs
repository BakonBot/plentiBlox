using System;
using System.Drawing;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RobloxFiles;
using RobloxFiles.DataTypes;

namespace plentiBlox
{
    class plentiBlox
    {

        static Color getAverageColor(Color color1, Color color2) // The name explains everything
        {
            int r = (color1.R + color2.R)/2;
            int g = (color1.G + color2.G)/2;
            int b = (color1.B + color2.B)/2;
            int a = (color1.A + color2.A)/2;
            Color newColor = Color.FromArgb(a, r, g, b);
            return newColor;
        }
        static bool isCloseColor(Color color1, Color color2, double tolerance) // Checks if color difference is less or equal to the tolerance
        {
            double r = color1.R - color2.R;
            double g = color1.G - color2.G;
            double b = color1.B - color2.B;
            double a = color1.A - color2.A;
            double difference = Math.Sqrt((r * r) + (g * g) + (b * b) + (a * a));
            return difference <= tolerance;
        }

        static void Main(string[] args)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Clear();
            Console.WriteLine("> Welcome to plentiBlox");
            bool success = true;
            // Choose image path
            string imagePath = "";
            while (imagePath == "")
            {
                Console.WriteLine("> Locate the image you want to port over to Roblox");
                Console.WriteLine("> Example: C:\\Users\\Username\\Pictures\\image.png");
                string tempPath = Console.ReadLine();
                if (File.Exists(tempPath) == true)
                {
                    if (Path.GetExtension(tempPath) == ".png" || Path.GetExtension(tempPath) == ".jpg" || Path.GetExtension(tempPath) == ".jpeg" || Path.GetExtension(tempPath) == ".bmp")
                    {
                        imagePath = tempPath;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("! The file you located isn't an image (or it is but doesn't use a supported format)");
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("! The image you located doesn't exist");
                }
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            //Choose path where to save the Roblox place file

            string rbxlPath = "";
            while (rbxlPath == "")
            {
                Console.WriteLine("> Locate where you would like the Roblox place file to be saved");
                Console.WriteLine("> Example: C:\\Users\\Username\\Documents\\place.rbxl");
                string tempPath = Console.ReadLine();
                if (File.Exists(tempPath) == false)
                {
                    if (Path.GetExtension(tempPath) == ".rbxl")
                    {
                        rbxlPath = tempPath;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.WriteLine("! You must locate a file with the .rbxl extension");
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("! The file located already exists");
                }
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Choose compression level 

            int compressionLevel = -1;
            int colorTolerance = 0;
            while (compressionLevel == -1)
            {
                Console.WriteLine("> Choose compression level");
                Console.WriteLine("  > 0 - Uncompressed (Not recommended, use only if you have to edit individual pixels in Studio)");
                Console.WriteLine("  > 1 - Lossless compression (Compresses groups of pixels into bigger frames)");
                Console.WriteLine("  > 2 - Low lossy compression (Makes 2 similar colors next to eachother turn into 1 color) [Color Tolerance Level 5]");
                Console.WriteLine("  > 3 - Medium lossy compression (Low lossy compression but more tolerant) [Color Tolerance Level 20]");
                Console.WriteLine("  > 4 - High lossy compression (Medium lossy compression but more tolerant) [Color Tolerance Level 50]");
                Console.WriteLine("  > 5 - Custom lossy compression (Set color tolerance level to whatever you want)");
                int tempLevel = Convert.ToInt16(Console.ReadLine());
                if (tempLevel >= 0 && tempLevel <= 5)
                {
                    compressionLevel = tempLevel;
                    if (compressionLevel == 2)
                    {
                        colorTolerance = 5;
                    }
                    else if (compressionLevel == 3)
                    {
                        colorTolerance = 20;
                    }
                    else if(compressionLevel == 4)
                    {
                        colorTolerance = 50;
                    }
                    else if (compressionLevel == 5)
                    {
                        Console.WriteLine("> Choose your color tolerance level:");
                        tempLevel = Convert.ToInt32(Console.ReadLine());
                        colorTolerance = tempLevel;
                    }
                }
                else
                {
                    Console.BackgroundColor = ConsoleColor.DarkRed;
                    Console.ForegroundColor = ConsoleColor.Black;
                    Console.WriteLine("! You must choose a number from 0 to 5");
                }
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.WriteLine("> Reading image file...");

            Bitmap bitmap = new Bitmap(imagePath);

            Console.WriteLine("> Creating Roblox place file...");

            // Create Roblox place file and such

            BinaryRobloxFile testFile = new BinaryRobloxFile();

            StarterGui starterGui = new StarterGui();
            starterGui.Parent = testFile;

            ScreenGui screenGui = new ScreenGui();
            screenGui.Name = "plentiBlox";
            screenGui.Parent = starterGui;

            Frame imageFrame = new Frame();
            imageFrame.Name = "Image";
            imageFrame.Size = new UDim2(0, bitmap.Width, 0, bitmap.Height);
            imageFrame.BorderSizePixel = 0;
            imageFrame.BackgroundTransparency = 1;
            imageFrame.Parent = screenGui;

            Console.WriteLine("> Porting over image...");

            // Create frames for each pixel/group of pixels depending on the compression level chosen

            Frame[,] pixelsContainer = new Frame[bitmap.Width, bitmap.Height];

            for (int i = 0; i < bitmap.Width; ++i)
            {
                for (int j = 0; j < bitmap.Height; ++j)
                {
                    Color pixelColor = bitmap.GetPixel(i, j);

                    if (pixelColor.A == 0) continue;

                    if (i > 0 && compressionLevel >= 1)
                    {
                        Color previousPixelColor = bitmap.GetPixel(i-1, j);
                        if (previousPixelColor == pixelColor || isCloseColor(previousPixelColor, pixelColor, colorTolerance) == true)
                        {
                            if (colorTolerance > 0)
                            {
                                Color avgColor = getAverageColor(pixelColor, previousPixelColor);
                                pixelsContainer[i-1, j].BackgroundColor3 = Color3.FromRGB(avgColor.R, avgColor.G, avgColor.B);
                                pixelsContainer[i-1, j].BackgroundTransparency = 1f - (float)avgColor.A / 255f;
                            }
                            pixelsContainer[i - 1, j].Size = new UDim2(0, pixelsContainer[i - 1, j].Size.X.Offset + 1, 0, pixelsContainer[i - 1, j].Size.Y.Offset);
                            pixelsContainer[i, j] = pixelsContainer[i - 1, j];
                            continue;
                        }
                    }

                    Frame pixel = new Frame();
                    pixel.Name = "Pixel(" + i + "," + j + ")";
                    pixel.BorderSizePixel = 0;
                    pixel.Size = new UDim2(0, 1, 0, 1);
                    pixel.Position = new UDim2(0, i, 0, j);
                    pixel.BackgroundColor3 = Color3.FromRGB(pixelColor.R, pixelColor.G, pixelColor.B);
                    pixel.BackgroundTransparency = 1f - (float)pixelColor.A / 255f;
                    pixel.Parent = imageFrame;

                    pixelsContainer[i, j] = pixel;
                }
            }

            if (compressionLevel >= 1)
            {
                for (int i = 0; i < bitmap.Width; ++i)
                {
                    for (int j = 0; j < bitmap.Height - 1; ++j)
                    {
                        if (pixelsContainer[i, j] != null && pixelsContainer[i, j + 1] != null && i == pixelsContainer[i, j].Position.X.Offset && pixelsContainer[i, j].Position.X.Offset == pixelsContainer[i, j + 1].Position.X.Offset && pixelsContainer[i, j].Size.X.Offset == pixelsContainer[i, j + 1].Size.X.Offset)
                        {
                            Color pixelColor = bitmap.GetPixel(i, j);
                            Color nextPixelColor = bitmap.GetPixel(i, j + 1);
                            if (pixelColor != nextPixelColor || isCloseColor(nextPixelColor, pixelColor, colorTolerance) == false) continue;
                            if (colorTolerance > 0)
                            {
                                Color avgColor = getAverageColor(pixelColor, nextPixelColor);
                                pixelsContainer[i, j].BackgroundColor3 = Color3.FromRGB(avgColor.R, avgColor.G, avgColor.B);
                                pixelsContainer[i, j].BackgroundTransparency = 1f - (float)avgColor.A / 255f;
                            }
                            pixelsContainer[i, j].Size = new UDim2(0, pixelsContainer[i, j].Size.X.Offset, 0, pixelsContainer[i, j].Size.Y.Offset + 1);
                            pixelsContainer[i, j + 1].Parent = null;
                            pixelsContainer[i, j + 1] = pixelsContainer[i, j];
                        }
                    }
                }
            }
            
            Console.WriteLine("> Saving Roblox place file...");

            //Catching exception just in case saving isn't allowed

            try
            {
                FileStream stream = File.OpenWrite(rbxlPath);
                testFile.Save(stream);
            }
            catch (Exception e)
            {
                success = false;
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("! " + e.Message);
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
            }

            // End of the program

            if (success == true)
            {
                Console.BackgroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("> Done! Press any key to open the Roblox place file");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ReadKey();
                Process.Start(rbxlPath);
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.DarkRed;
                Console.ForegroundColor = ConsoleColor.Black;
                Console.WriteLine("! Porting failed! Please try again.");
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("> Press any key to exit");
                Console.ReadKey();
            }
        }
    }
}
