using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProgressMatrixLibrary
{


    public partial class ProgressMatrixControl : UserControl
    {
        [ToolboxItem(true)]
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

                for (int alpha = 0; alpha < 255; alpha += 2)
                {
                    BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(1);
                }
                State = 1;

            }
            public void FadeOut(Color BaseColor)
            {

                for (int alpha = 255; alpha > 0; alpha -= 2)
                {

                    BackColor = Color.FromArgb(alpha, BaseColor);
                    Thread.Sleep(1);
                }
                BackColor = Color.WhiteSmoke;
                State = 0;
            }

        }

        public enum ProgressStyle
        {
            Standard = 0, Classic = 1, ColumnBLocks = 2, Random = 3
        };
        public enum ProgressState
        {
            Idle = 0, Animate = 1, ColumnBLocks = 2, Random = 3
        };
        CancellationTokenSource mTokenSource;
        private Task mProgressTask;
        private Color mCellColor;

        public ProgressMatrixControl()
        {

            InitializeComponent();
            InitializeMatrix();

            Style = ProgressStyle.Classic;
            State = ProgressState.Idle;
        }

        private ProgressState State
        {
            set; get;
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
            State = ProgressState.Idle;
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

        public void ShowProgress(Form form)
        {
            if (Parent == null)
            {
                form.Controls.Add(this);
            }
            BringToFront();
            Left = (form.ClientSize.Width - Width) / 2;
            Top = (form.ClientSize.Height - Height) / 2;
            Show();
        }
        public void ProgressAnimation()
        {
            if (State == ProgressState.Animate) return;

            mTokenSource = new CancellationTokenSource();
            CancellationToken cancelToken = mTokenSource.Token;
            mStop = false;
            mCellColor = Color.FromArgb(0, 144, 223);

            State = ProgressState.Animate;
            mProgressTask = Task.Run(() =>
           {
               switch (Style)
               {
                   case ProgressStyle.Classic:
                       AnimateCellsRowWise(ref cancelToken);
                       break;
                   case ProgressStyle.Random:
                       AnimateCellsRandom(ref cancelToken);
                       break;
                   case ProgressStyle.ColumnBLocks:
                       AnimateCellsColumnBlocks(ref cancelToken);
                       break;
                   default:
                       break;
               }

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
            var randomNumber = new System.Random();
            while (false == mStop)
            {
                if (token.IsCancellationRequested) break;

                int x = randomNumber.Next(0, 3);
                int y = randomNumber.Next(0, 3);

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
