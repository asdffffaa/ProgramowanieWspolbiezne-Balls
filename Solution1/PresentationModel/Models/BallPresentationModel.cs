using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace TP.ConcurrentProgramming.PresentationModel.Models
{
    public class BallPresentationModel : INotifyPropertyChanged
    {
        private double _x;
        private double _y;
        private double _diameter;

        public double X
        {
            get => _x; // double currentX = ball.X;
            set
            {
                _x = value;
                OnPropertyChanged();
            } // ball.X = 100;
        }

        public double Y
        {
            get => _y; 

            set
            {
                _y = value;
                OnPropertyChanged( );
            }
        }

        public double Diameter
        {
            get => _diameter;

            set
            {
                _diameter = value;
                OnPropertyChanged();
            }
        }

    

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName ] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



    }
}