using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressMatrixLibrary
{


    public partial class ProgressMatrixControl : UserControl
    {

        public class ProgressSquare : Panel
        {

            public ProgressSquare()
            {

                BackColor = Color.White;
                Dock = DockStyle.Fill;
                DoubleBuffered = true;
                BorderStyle = BorderStyle.FixedSingle;
                Margin = new Padding(1);
                int i = 0;


            }
           

            public void FadeIn(Color BaseColor)
            {

                for(int alpha=0; alpha<255;++alpha)
                {
                                        
                    this.BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(2);
                }

            }
            public void FadeOut(Color BaseColor)
            {

                for (int alpha = 255; alpha> 0; --alpha)
                {

                    this.BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(2);
                }

            }

        }
      

        CancellationTokenSource mTokenSource;
        private Task mProgressTask;

        public ProgressMatrixControl()
        {

            InitializeComponent();
            InitializeMatrix();
            
            mTokenSource = new CancellationTokenSource();

            

        }

        private void InitializeMatrix()
        {
            for (int y = 0; y < 3; ++y)
            {
                for (int x = 0; x < 3; ++x)
                {

                    tableLayoutPanel1.Controls.Add(new ProgressSquare(), x, y);

                }
            }
        }

        bool mStop = false;
        public void Stop()
        {
            mStop = true;
            mTokenSource.Cancel();
        }
       
        public void ProgressSimple()
        { 
            var token = mTokenSource.Token;
            mStop = false;

          
            
            mProgressTask = Task.Run(() =>
            {
                FadeCells(ref token);

            }, token);

            
        }

        private void FadeCells(ref CancellationToken token)
        {
            while (false == mStop)
            {

                for (int y = 2; y >= 0; --y)
                {
                    for (int x = 0; x < 3; ++x)
                    {

                        ProgressSquare cellSquare = (ProgressSquare)tableLayoutPanel1.Controls[x + 3 * y];
                        cellSquare.FadeIn(Color.FromArgb(123, 123, 234));
                        //sq.FadeOut(Color.FromArgb(123, 123, 234));
                        if (token.IsCancellationRequested) break;
                    }
                    Thread.Sleep(2);
                    if (token.IsCancellationRequested) break;
                }
               
            }

            if (token.IsCancellationRequested) return;
        }
    }
   
}
