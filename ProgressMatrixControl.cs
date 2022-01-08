using System.ComponentModel;
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

                BackColor = Color.WhiteSmoke;
                Dock = DockStyle.Fill;
                DoubleBuffered = true;
                BorderStyle = BorderStyle.None;
                Margin = new Padding(1);
                State = 0;


            }

            public int State
            {
                set; get;
            }
            public void FadeIn(Color BaseColor)
            {
                //SolidBrush shadowBrush = new SolidBrush(Color.Red);

                for (int alpha = 0; alpha < 255; alpha += 2)
                {

                    this.BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(1);

                    //shadowBrush.Color = Color.FromArgb(alpha, BaseColor);
                    //Rectangle r = this.ClientRectangle;
                    //Graphics g =  this.CreateGraphics();
                    //g.FillRectangle(shadowBrush, r.X, r.Y, r.Width, r.Height);

                }
                State = 1;

            }
            public void FadeOut(Color BaseColor)
            {

                for (int alpha = 255; alpha > 0; alpha -= 2)
                {

                    this.BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(1);
                }
                 BackColor = Color.WhiteSmoke;
                State = 0;
            }



        }

        public enum ProgressStyle
        {
            Standard = 0, Animation = 1
        };
        CancellationTokenSource mTokenSource;
        private Task mProgressTask;
        private Color mCellColor;

        public ProgressMatrixControl()
        {

            InitializeComponent();
            InitializeMatrix();



        }

        int mValue = 0;
        [Description(""), Category("Behaviour")]
        public int Value
        {
            get
            {
                return mValue;
            }
            set
            {
                mValue = value;


            }
        }
        [Description(""), Category("Behaviour")]
        public ProgressStyle Style
        {
            get; set;

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
        public void StopProgress()
        {
            mStop = true;
            mTokenSource.Cancel();
            mProgressTask.Wait();
            ResetCells();

        }

        private void ResetCells()
        {
            for (int y = 2; y >= 0; --y)
            {
                for (int x = 0; x < 3; ++x)
                {

                    ProgressSquare cellSquare = (ProgressSquare)tableLayoutPanel1.Controls[x + 3 * y];
                    cellSquare.BackColor = Color.WhiteSmoke;

                }
            }
        }

        public void ProgressRowsSimple()
        {
            mTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = mTokenSource.Token;
            mStop = false;
            mCellColor = Color.FromArgb(0, 144, 223);

            mProgressTask = Task.Run(() =>
           {
               //AnimateCellsRowWise(ref cancelToken);
               //AnimateCellsColumnBlocks(ref cancelToken);
               AnimateCellsRandom(ref cancelToken);

           }, cancelToken);


        }

        private void AnimateCellsRowWise(ref CancellationToken token)
        {
            while (false == mStop)
            {

                for (int y = 2; y >= 0; --y)
                {
                    if (token.IsCancellationRequested) break;

                    for (int x = 0; x < 3; ++x)
                    {

                        ProgressSquare cellSquare = (ProgressSquare)tableLayoutPanel1.Controls[x + 3 * y];
                        cellSquare.FadeIn(mCellColor);


                    }


                }
                ResetCells();


            }


        }
        private void AnimateCellsColumnBlocks(ref CancellationToken token)
        {
            while (false == mStop)
            {
                for (int x = 0; x < 3; ++x)
                {
                    if (token.IsCancellationRequested) break;

                    Thread.Sleep(234);
                    ((ProgressSquare)tableLayoutPanel1.Controls[x + 3 * 0]).BackColor = mCellColor;

                    ((ProgressSquare)tableLayoutPanel1.Controls[x + 3 * 1]).BackColor = mCellColor;


                    ((ProgressSquare)tableLayoutPanel1.Controls[x + 3 * 2]).BackColor = mCellColor;


                    Thread.Sleep(234);


                }
                ResetCells();

            }
        }

        private void AnimateCellsRandom(ref CancellationToken token)
        {
            var r = new System.Random();
            while (false == mStop)
            {
                if (token.IsCancellationRequested) break;
                int x = r.Next(0,3);
                int y = r.Next(0,3);

                ProgressSquare cellSquare = (ProgressSquare)tableLayoutPanel1.Controls[x + 3 * y];
                if (cellSquare.State == 0)
                {
                    cellSquare.FadeIn(mCellColor);
                }
                else
                {
                    cellSquare.FadeOut(mCellColor);
                }

                Thread.Sleep(88);
            }
        }
    }
}
