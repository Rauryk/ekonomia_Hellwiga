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
using System.Windows.Shapes;
using LiveCharts;
using LiveCharts.Helpers;
using LiveCharts.Wpf;

namespace ekonomia_Hellwiga
{

    /// <summary>
    /// Interaction logic for Wykres.xaml
    /// </summary>
    public partial class Wykres : Window
    {
        public Wykres(double[] areaDouble,List<double> list)
        {
            InitializeComponent();
            RankGraph.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "Rzeczywisty",
                Values = areaDouble.AsChartValues(),
            },
            new LineSeries
            {
                Title = "Obliczone",
                Values = list.AsChartValues(),
            },
        };

        }
    }
}
