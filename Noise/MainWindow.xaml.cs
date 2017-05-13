using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Noise
{
   /// <summary>
   /// Логика взаимодействия для MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      List<double[,]> rawtile;
      int size;
      int pow;
      private const int plainCnt = 6;

      public MainWindow()
      {
         InitializeComponent();
      }

      private void DrawAll()
      {
         int newY = 0;
         Color cl = new Color();

         DrawingVisual dv = new DrawingVisual();

         using (DrawingContext dc = dv.RenderOpen())
         {
            for (int x = 0; x < size; ++x)
            {
               for (int y = 0; y < size; ++y)
               {
                  newY = size - y;

                  for (int a = 0; a < plainCnt; ++a)
                  {
                     cl = Color.FromRgb((byte)(rawtile[a][y, x] * 255),
                                      (byte)(rawtile[a][y, x] * 255),
                                      (byte)(rawtile[a][y, x] * 255));

                     Brush br = new SolidColorBrush(cl);

                     switch (a)
                     {
                        case 0:
                           dc.DrawRectangle(br, null, new Rect(x, newY + size, 1, 1));
                           break;
                        case 1:
                           dc.DrawRectangle(br, null, new Rect(x + size, newY + size, 1, 1));
                           break;
                        case 2:
                           dc.DrawRectangle(br, null, new Rect(x + size * 2, newY + size, 1, 1));
                           break;
                        case 3:
                           dc.DrawRectangle(br, null, new Rect(x + size * 3, newY + size, 1, 1));
                           break;
                        case 4:
                           dc.DrawRectangle(br, null, new Rect(x + size, newY, 1, 1));
                           break;
                        default:
                           dc.DrawRectangle(br, null, new Rect(x + size, newY + size * 2, 1, 1));
                           break;
                     }
                  }
               }
            }

            dc.Close();
         }

         RenderTargetBitmap rtb = new RenderTargetBitmap((int)size * 4, (int)size * 3, 96, 96, PixelFormats.Pbgra32);
         rtb.Render(dv);

         noiseImage.Source = rtb;
      }

      private double minNoise()
      {
         double min = rawtile[0][0, 0];

         for (int a = 0; a < plainCnt; ++a)
         {
            for (int i = 0; i < size; ++i)
            {
               for (int j = 0; j < size; ++j)
               {
                  if (min > rawtile[a][i, j])
                  {
                     min = rawtile[a][i, j];
                  }
               }
            }
         }

         return min;
      }

      private double maxNoise()
      {
         double max = rawtile[0][0, 0];

         for (int a = 0; a < plainCnt; ++a)
         {
            for (int i = 0; i < size; ++i)
            {
               for (int j = 0; j < size; ++j)
               {
                  if (max < rawtile[a][i, j])
                  {
                     max = rawtile[a][i, j];
                  }
               }
            }
         }

         return max;
      }

      private void normalizeArray()
      {
         double min = minNoise();
         double max = maxNoise();

         for (int a = 0; a < plainCnt; ++a)
         {
            for (int i = 0; i < size; ++i)
            {
               for (int j = 0; j < size; ++j)
               {
                  rawtile[a][i, j] = ((rawtile[a][i, j] - min) / (max - min));
               }
            }
         }
      }

      private void generateButton_Click(object sender, RoutedEventArgs e)
      {
         rawtile = new List<double[,]>();
         pow = Convert.ToInt32(powOf2TextBox.Text);
         size = (int)Math.Pow(2, pow) + 1;
         int half_size = (size - 1) / 2;

         sizeLabel.Content = size.ToString();

         for (int i = 0; i < plainCnt; ++i)
         {
            rawtile.Add(new double[size, size]);
         }

         var watch = System.Diagnostics.Stopwatch.StartNew();

         Perlin3D.setSeed(Convert.ToInt32(seedTextBox.Text));

         Parallel.For(0, plainCnt, index => { CalculateNoise(half_size, index); });

         watch.Stop();
         var elapsedMs = watch.ElapsedMilliseconds;

         labelMs.Content = elapsedMs.ToString();

         normalizeArray();

         DrawAll();
      }

      private void CalculateNoise(int halfSize, int index)
      {
            halfSize = halfSize / 2;

         for (int i = 0; i < size; ++i)
         {
            for (int j = 0; j < size; ++j)
            {
               // center of cube is moved to the center of coordinates and its coords transformed to sphere coords
               /*
               *    5
               * 1  2  3  4
               *    6
               */
               switch (index)
               {
                  case 0:
                     rawtile[0][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(0 - halfSize, halfSize - j, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
                  case 1:
                     rawtile[1][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(j - halfSize, 0 - halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
                  case 2:
                     rawtile[2][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(halfSize, j - halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
                  case 3:
                     rawtile[3][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(halfSize - j, halfSize, i - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
                  case 4:
                     rawtile[4][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(j - halfSize, i - halfSize, halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
                  case 5:
                     rawtile[5][i, j] = Perlin3D.getMultioctave3DNoiseValueFromSphere(j - halfSize, halfSize - i, 0 - halfSize, 1, (uint)pow - 1, (uint)halfSize);
                     break;
               }
            }
         }
      }
   }
}
