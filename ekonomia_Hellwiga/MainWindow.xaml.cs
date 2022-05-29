using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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
using MathNet.Numerics.LinearAlgebra;

namespace ekonomia_Hellwiga
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    

    public class dane
    {
        public float Y { get; set; }
        public float X1 { get; set; }
        public float X2{ get; set; }
        public float X3 { get; set; }
        public float X4{ get; set; }

    }

    public class Permutation
    {
        public List<int> Elements { get; set; }
        public double H { get; set; }
    }

    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }

    

        public double Wsp_zmi(double[] values1, double[] values2)
        {
            if (values1.Length != values2.Length)
                throw new ArgumentException("values must be the same length");

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum1 = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            var result = sum1 / Math.Sqrt(sumSqr1 * sumSqr2);

            return result;
        }
        public void label_content(TextBlock textBlock , ICollection<double> list) 
        {
            textBlock.Text = String.Empty;
            list.ToList().ForEach(x => {
                textBlock.Text +=Math.Round(x,3).ToString() + "\n";
               // MessageBox.Show(x.ToString());
                
                
                }
            ) ;
            
        }
       
        public void label_content(TextBlock textBlock, double[,] arrea)
        {
            textBlock.Text = String.Empty;
            for (int i = 0; i < arrea.GetLength(0); i++)
            {
                for (int j = 0; j < arrea.GetLength(1); j++)
                {

                

            
                //MessageBox.Show(item.ToString());
                textBlock.Text += Math.Round( arrea[i,j],3).ToString("0.00")+" ";
                }
                textBlock.Text += "\n";
            }


            

        }
        public IEnumerable<IEnumerable<T>> GetCombinations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetCombinations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }
        public void CalculateH(Permutation permutation,double[] R0,double[,] R)
        {
            List<double> h = new List<double>();
            double sum = 0;
            for (int j = 0; j < permutation.Elements.Count; j++)
            {
                sum = 0;
                for (int i = 0; i < permutation.Elements.Count; i++)
                {
                    if (j != i)
                    {
                        sum += Math.Abs(R[permutation.Elements[j] - 1,permutation.Elements[i] - 1]);
                    }
                }
                permutation.H += R0[permutation.Elements[j] - 1] * R0[permutation.Elements[j] - 1] / (1 + sum);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            List<dane> LoadCollectionData()
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                List<dane> list = new List<dane>();

                if (openFileDialog.ShowDialog() == true)
                {
                    string string_buff = string.Empty;
                    float[] t_float_buff = new float[5];
                    StreamReader streamReader = new StreamReader(openFileDialog.FileName);
                    string linia;

                    while ((linia = streamReader.ReadLine()) != null)
                    {
                        int a = 0;
                        string_buff = string.Empty;
                        foreach (char item in linia)
                        {
                            if (item == ' ')
                            {
                                t_float_buff[a] = float.Parse(string_buff);
                                a++;
                                string_buff = "";
                            }
                            else
                            {
                                string_buff += item;
                            }
                        }
                        list.Add(new dane() { Y = t_float_buff[0], X1 = t_float_buff[1], X2 = t_float_buff[2], X3 = t_float_buff[3], X4 = float.Parse(string_buff) });
                    }
                }
                if(list.Count!=0)
                    oblicz_button.IsEnabled = true;
                else
                    oblicz_button.IsEnabled = false;
                return list;
            }
            
       
            
                Grind_dane.ItemsSource = LoadCollectionData();
                
            
        }

        double[] YPrognoza=new double[1];
        List<double> Y = new List<double>();
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
            var buff = Grind_dane.ItemsSource;
            
            List<double> X1 = new List<double>();
            List<double> X2 = new List<double>();
            List<double> X3 = new List<double>();
            List<double> X4 = new List<double>();
            int rowscount = 0;
            Y.Clear();
            X1.Clear();
            X2.Clear();
            X3.Clear();
            X4.Clear();
            foreach (dane item in buff)
            {
                 Y.Add(item.Y);
                X1.Add(item.X1);
                X2.Add(item.X2);
                X3.Add(item.X3);
                X4.Add(item.X4);
                rowscount++;
            }
            //oblicznie vektora R0 
            List<double> R0 = new List<double>();
            
            List<List<double>> lists = new List<List<double>>();
            lists.Add(X1);
            lists.Add(X2);
            lists.Add(X3);
            lists.Add(X4);
            foreach (var item in lists)
            {
                R0.Add(Wsp_zmi(Y.ToArray(), item.ToArray()));
            }

            label_content(R0_text, R0);
            //obliczanie macierzy R
            double[,] R = new double[lists.Count, lists.Count];

            

            for (int i = 0; i < lists.Count; i++)
            {
                for (int j = 0; j < lists.Count; j++)
                {
                    R[i, j] = Wsp_zmi(lists[i].ToArray(), lists[j].ToArray());
                }

            }

            label_content(R_text, R);

            int S =Int32.Parse( (Math.Pow( 2, lists.Count) - 1).ToString());
            Permutation[] combinations = new Permutation[(int)Math.Pow(2, lists.Count) - 1];
            var list = new List<int>();
           for (int i = 0; i < lists.Count; i++)
           {
               list.Add(i + 1);
           }
           
           int permutationIndex = 0;
           for (int i = 0; i < lists.Count; i++)
           {
               var tmp = GetCombinations(list, i + 1);
               foreach (var item in tmp)
               {
                   combinations[permutationIndex] = new Permutation();
                   combinations[permutationIndex].Elements = item.ToList();
                   CalculateH(combinations[permutationIndex],R0.ToArray(),R);
                   permutationIndex++;
               }
           }
           var permutacja = combinations.OrderByDescending(x => x.H).FirstOrDefault();
           double[][] X = new double[permutacja.Elements.Count + 1][];

            //KMNK
          
            for (int i = 0; i < permutacja.Elements.Count; i++)
            {
                X[i] = new double[lists.Count];
                X[i] = lists[permutacja.Elements[i] - 1].ToArray();
            }
            X[permutacja.Elements.Count] = new double[rowscount];
            for (int i = 0; i < rowscount; i++)
            {
                X[permutacja.Elements.Count][i] = 1;
            }
            Matrix<double> matrixX = Matrix<double>.Build.DenseOfColumnArrays(X);
            Vector<double> vectorY = Vector<double>.Build.DenseOfArray(Y.ToArray());
            Matrix<double> matrixY = Matrix<double>.Build.DenseOfColumnArrays(Y.ToArray());
            Matrix<double> matrixXt = matrixX.Transpose();

            var vectorA = matrixX.Transpose().Multiply(matrixX).Inverse().Multiply((matrixX.Transpose().Multiply(vectorY)));
            var matrixA = Matrix<double>.Build.DenseOfColumnVectors(vectorA);
            var Su2 = (1 / (double)(rowscount - 3)) * (matrixY.Transpose().Multiply(matrixY) - matrixY.Transpose().Multiply(matrixX).Multiply(matrixA));
            var D2a = Su2[0, 0] * ((matrixX.Transpose() * matrixX).Inverse());

            var Da = new double[D2a.RowCount];
            for (int i = 0; i < D2a.RowCount; i++)
            {
                Da[i] = Math.Sqrt(D2a[i, i]);
            }

            Da_text.Text = "";
            for (int i = 0; i < Da.Length; i++)
            {
                Da_text.Text += String.Format("{0:0.0000}", Da[i]) + "\n";
            }

            var Su = Math.Sqrt(Su2[0, 0]);
            var v = Su / Y.Average() * 100;

            V_text.Text =String.Format("{0:0.00}", v) + "%";

            YPrognoza = new double[rowscount];
            for (int i = 0; i < rowscount; i++)
            {
                for (int j = 0; j < permutacja.Elements.Count; j++)
                {

                    YPrognoza[i] += vectorA[j] * lists[permutacja.Elements[j] - 1][i];
                }
                YPrognoza[i] += vectorA.Last();
            }
            double sum1 = 0;
            double sum2 = 0;
            for (int i = 0; i < rowscount; i++)
            {
                sum1 += (YPrognoza[i] - Y.Average()) * (YPrognoza[i] - Y.Average());
                sum2 += (Y[i] - Y.Average()) * (Y[i] - Y.Average());
            }
            var R2 = sum1 / sum2;
            R2_text.Text = "R^2= " + String.Format("{0:0.0000}", R2);
           Su_text.Text = "Su= " + String.Format("{0:0.0000}", Su);
           
           
          
           helldwig_text.Text = "zmienne wyznaczone metodą heldwiga { ";
           foreach (var item in permutacja.Elements)
           {
               helldwig_text.Text += "x" + item.ToString() + ", ";
           }
            helldwig_text.Text += "}";

            model_text.Text = "Równanie modelu: y = ";
           
           
           for (int i = 0; i < vectorA.Count; i++)
           {
                model_text.Text += String.Format("{0:0.0000}", vectorA[i]) + "x" + permutacja.Elements[i].ToString() + "+ ";
               if (i == vectorA.Count - 2)
               {
                   i++;
                    model_text.Text += String.Format("{0:0.0000}", vectorA[i]);
               }
           }
           zmienne_text.Text = "       ";
           for (int i = 0; i < Da.Length; i++)
           {
                zmienne_text.Text += "(";
                zmienne_text.Text += String.Format("{0:0.0000}", Da[i]);
                zmienne_text.Text += ")        ";
           
           }
            wykres_button.IsEnabled = true;
    }

        private void wykres_button_Click(object sender, RoutedEventArgs e)
        {
            Wykres win2 = new Wykres(YPrognoza,Y);
            win2.Show();
        }
    }
   
}
