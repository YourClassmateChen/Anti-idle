using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace AntiIdleHelper
{
    public partial class MainForm : Form
    {
        // 导入 user32.dll 的 mouse_event 函数，用于模拟鼠标相对移动
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        // 鼠标事件标志：相对移动
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        // 定时器，用于触发鼠标移动
        private Timer moveTimer;
        // 随机数生成器
        private Random random = new Random();

        // 移动概率（80% 的概率会移动，20% 的概率停顿）
        private const double MoveProbability = 0.8;
        // 最小移动间隔（毫秒）
        private const int MinInterval = 500;
        // 最大移动间隔（毫秒）
        private const int MaxInterval = 2000;
        // 固定移动距离（像素），可根据需要调整
        private const int MoveDistance = 3;

        // 记录下一次移动方向：true 表示向右，false 表示向左
        private bool moveRight = true;

        public MainForm()
        {
            InitializeComponent();
            SetupUI();
            InitializeTimer();
        }

        private void SetupUI()
        {
            // 窗体基本设置
            this.Text = "防检测挂机程序";
            this.Size = new Size(400, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(45, 45, 48); // 深色背景

            // 标题标签
            Label titleLabel = new Label
            {
                Text = "防检测挂机程序",
                Font = new Font("微软雅黑", 16, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(115, 20)
            };

            // 状态标签
            Label statusLabel = new Label
            {
                Text = "状态：已停止",
                Font = new Font("微软雅黑", 10),
                ForeColor = Color.LightGray,
                AutoSize = true,
                Location = new Point(150, 60)
            };
            statusLabel.Name = "statusLabel";

            // 开始按钮
            Button startButton = new Button
            {
                Text = "开始挂机",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(0, 122, 204),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(60, 130),
                Cursor = Cursors.Hand
            };
            startButton.FlatAppearance.BorderSize = 0;
            startButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 150, 230);
            startButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(0, 100, 180);
            startButton.Name = "startButton";
            startButton.Click += StartButton_Click;

            // 停止按钮
            Button stopButton = new Button
            {
                Text = "停止挂机",
                Font = new Font("微软雅黑", 11, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(200, 60, 60),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Location = new Point(220, 130),
                Cursor = Cursors.Hand,
                Enabled = false  // 初始禁用
            };
            stopButton.FlatAppearance.BorderSize = 0;
            stopButton.FlatAppearance.MouseOverBackColor = Color.FromArgb(220, 80, 80);
            stopButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 50, 50);
            stopButton.Name = "stopButton";
            stopButton.Click += StopButton_Click;

            // 添加控件到窗体
            this.Controls.Add(titleLabel);
            this.Controls.Add(statusLabel);
            this.Controls.Add(startButton);
            this.Controls.Add(stopButton);
        }

        private void InitializeTimer()
        {
            moveTimer = new Timer();
            moveTimer.Tick += MoveTimer_Tick;
            // 初始间隔随机设置
            moveTimer.Interval = random.Next(MinInterval, MaxInterval);
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            // 启动定时器
            moveTimer.Start();
            UpdateUI(true);
        }

        private void StopButton_Click(object sender, EventArgs e)
        {
            // 停止定时器
            moveTimer.Stop();
            UpdateUI(false);
        }

        private void MoveTimer_Tick(object sender, EventArgs e)
        {
            // 随机决定本次是否移动（模拟自然停顿）
            if (random.NextDouble() < MoveProbability)
            {
                // 根据方向标志计算水平移动量：固定距离，左右交替
                int dx = moveRight ? MoveDistance : -MoveDistance;
                // 垂直方向不移动
                int dy = 0;

                // 调用 mouse_event 模拟相对移动
                mouse_event(MOUSEEVENTF_MOVE, (uint)dx, (uint)dy, 0, 0);

                // 切换下一次移动方向
                moveRight = !moveRight;
            }

            // 随机更新下一次触发间隔
            moveTimer.Interval = random.Next(MinInterval, MaxInterval);
        }

        private void UpdateUI(bool isRunning)
        {
            Button startButton = this.Controls["startButton"] as Button;
            Button stopButton = this.Controls["stopButton"] as Button;
            Label statusLabel = this.Controls["statusLabel"] as Label;

            if (startButton != null && stopButton != null && statusLabel != null)
            {
                startButton.Enabled = !isRunning;
                stopButton.Enabled = isRunning;
                statusLabel.Text = isRunning ? "状态：挂机中..." : "状态：已停止";
                statusLabel.ForeColor = isRunning ? Color.LightGreen : Color.LightGray;
            }
        }
    }
}