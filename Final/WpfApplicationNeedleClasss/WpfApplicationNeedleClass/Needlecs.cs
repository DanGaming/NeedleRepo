
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
namespace WpfApplicationNeedleClass
{
    class Needle : StackPanel
    {
        public Needle()
        {
            LayoutTransform = new ScaleTransform(1, -1);
        }
        public bool IsEmpty
        {
            get
            {
                return (this.Children.Count == 0);
            }
        }
        
        /// <summary>
        /// grabs top disk only
        /// </summary> 
        public Rectangle TopDisk
        {
            get
            {
                if (!this.IsEmpty)
                    return (Rectangle)this.Children[Children.Count - 1];
                else
                    return null;
            }
        }
       
        /// <summary>
        ///add disk from numdisc textbox user input and scale incrementally
        /// </summary> 
        public void AddDisk(Rectangle disk, double scale) {
            if (this.Children.Count < 10) {
                if (this.IsEmpty)
                    disk.Width = this.Width;

                else {
                    if (scale > 1)
                        scale = 1; // always less than or equal to 1
                    disk.Width = TopDisk.Width * scale;
                }

                this.Children.Add(disk);
            }

            else
                MessageBox.Show("You already have the maximum 10 discs!");
        }
       
        /// <summary>
        ///code no longer used as buttons to remove and add disk during game are redundant.
        /// </summary> 
        public void AddDisk(Rectangle disk)
        {
            AddDisk(disk, 0.8);
        }

        public void RemoveTopDisk() {
            if (!IsEmpty)
                this.Children.RemoveAt(Children.Count - 1);
            else
                MessageBox.Show("There are no discs to remove!");
        }



    }
}