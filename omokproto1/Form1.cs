using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Media;
namespace omokproto1
{
    public enum BlockType {
        Empty,
        BlackStone,
        WhiteStone
    }

    public partial class Form1 : Form
    {
        const int BoardWidth = 15; //보드의 각 줄
        const int BoardMargin = 50; //보드 마진
        const int BoardCircleSize = 10; // 격자점 크기
        int BoardStoneSize = 70;

        readonly int[,] BoardDirection = new int[4, 2] { { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
        int[,] Ai_point = new int[5, 4];

        Rectangle BoardSize;
        BlockType[,] BoardGrid = new BlockType[BoardWidth, BoardWidth];

        BufferedGraphicsContext ctx;
        BufferedGraphics bg;

        Image sprite_Blackstone;
        Image sprite_Whitestone;

        SoundPlayer sound_Placestone = new SoundPlayer(@"C:/Users/user/source/repos/omokproto1/omokproto1/Resources/dolsound.wav");

        bool Game_Run = false;
        BlockType Game_Turn = BlockType.BlackStone;
        BlockType Game_Winner = BlockType.Empty;

        BlockType Ai_stone = BlockType.WhiteStone;
        Point Ai_lastPlace = new Point(-1, -1);
        BlockType User_stone = BlockType.BlackStone;

        Font Game_font = new Font("Gulim", 10, FontStyle.Bold);
        Font Overlay_font = new Font("Gulim", 50, FontStyle.Bold);
        double[,] benefit_map = new double[BoardWidth, BoardWidth];
        double[,] ai_benefit = new double[BoardWidth, BoardWidth];
        double[,] user_benefit = new double[BoardWidth, BoardWidth];
        string overlay_text = "";

        StringFormat centerFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };

        public Form1()
        {
            InitializeComponent();

            //리소스 로드
            sprite_Blackstone = omokproto1.Properties.Resources.blackdol;
            sprite_Whitestone = omokproto1.Properties.Resources.whitedol;

            //보드 사이즈 생성과 보드 길이 유효성 체크
            if (BoardWidth % 2 == 0)
                throw new System.ArgumentException("BoardWidth must be Odd number");
            if (BoardWidth < 3)
                throw new System.ArgumentException("BoardWidth must be higher than 2");
            BoardSize = new Rectangle(BoardMargin, BoardMargin, omokpan.Width - BoardMargin * 2, omokpan.Height - BoardMargin * 2);

            BoardStoneSize = BoardSize.Size.Width / BoardWidth;

            //그래픽스 생성 밑 그리기 시작
            ctx = BufferedGraphicsManager.Current;
            ctx.MaximumBuffer = new Size(this.Width + 1, this.Height + 1);

            bg = ctx.Allocate(omokpan.CreateGraphics(), new Rectangle(this.Location, this.Size));
            bg.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            Omokpan_Paint(null, null);

            //AI 평점 설정
            Ai_point[1, 0] = 1;
            Ai_point[2, 0] = 4;
            Ai_point[2, 1] = 3;
            Ai_point[2, 2] = 2;
            Ai_point[2, 3] = 1;
            Ai_point[3, 0] = 18;
            Ai_point[3, 1] = 17;
            Ai_point[3, 2] = 16;
            Ai_point[4, 0] = 73;
            Ai_point[4, 1] = 72;
        }


        private void start_btn_Click(object sender, EventArgs e)
        {
            if (Game_Run)
            {
                Game_Run = false;
                start_btn.Text = "start";
                overlay_text = "";
                BoardGrid = new BlockType[BoardWidth, BoardWidth];
                Omokpan_Paint(null, null);
            }
            else
            {
                Game_Run = true;
                start_btn.Text = "stop";
                overlay_text = "";
                benefit_map = new double[BoardWidth, BoardWidth]; 
                ai_benefit = new double[BoardWidth, BoardWidth];
                user_benefit = new double[BoardWidth, BoardWidth];


                BoardGrid = new BlockType[BoardWidth, BoardWidth];
                Game_Winner = BlockType.Empty;

                if (cmb_first.Text == "User First")
                {
                    Ai_stone = BlockType.WhiteStone;
                    User_stone = BlockType.BlackStone;

                    action_Place_stone(User_stone, BoardWidth / 2, BoardWidth / 2);
                    Game_Turn = Ai_stone;

                    timer1.Interval = 500;
                    timer1.Enabled = true;
                }
                else
                {
                    Ai_stone = BlockType.BlackStone;
                    User_stone = BlockType.WhiteStone;

                    action_Place_stone(Ai_stone, BoardWidth / 2, BoardWidth / 2);
                    Game_Turn = User_stone;
                }

            }
        }

        private void omokpan_MouseDown_1(object sender, MouseEventArgs e)
        {
            bool run = true;
            if (Game_Run && Game_Turn == User_stone)
            {
                for (int x = 0; x < BoardWidth && run; ++x)
                {
                    for (int y = 0; y < BoardWidth && run; ++y)
                    {
                        if (BoardSize.X + BoardSize.Width * x / (BoardWidth - 1) - BoardStoneSize / 2 < e.X && e.X < BoardSize.X + BoardSize.Width * x / (BoardWidth - 1) - BoardStoneSize / 2 + BoardStoneSize &&
                            BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1) - BoardStoneSize / 2 < e.Y && e.Y < BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1) - BoardStoneSize / 2 + BoardStoneSize)
                        {
                            if (BoardGrid[x, y] == BlockType.Empty)
                            {
                                action_Place_stone(User_stone, x, y);
                                Game_Turn = Ai_stone;

                                if (!action_Check_Winner())
                                {
                                    timer1.Interval = 500;
                                    timer1.Enabled = true;
                                }
                            }
                            run = false;
                        }
                    }
                }
            }
        }

        private void action_Place_stone(BlockType type, int x, int y)
        {
            BoardGrid[x, y] = type;
            Omokpan_Paint(null, null);
        }

        private bool action_Check_Winner()
        {
            BlockType winner = BlockType.Empty;
            for (int x = 0; x < BoardWidth && winner == BlockType.Empty; ++x)
            {
                for (int y = 0; y < BoardWidth && winner == BlockType.Empty; ++y)
                {
                    for (int w = 0; w < 4; ++w)
                    {
                        int end_x = x + BoardDirection[w, 0] * 5;
                        int end_y = y + BoardDirection[w, 1] * 5;
                        if (0 <= end_x && end_x < BoardWidth && 0 <= end_y && end_y < BoardWidth)
                        {
                            //승패 결정
                            {
                                Point pos = new Point(x, y);
                                BlockType owner = BlockType.Empty;

                                for (int i = 0; i < 5; ++i)
                                {
                                    if (owner == BlockType.Empty && BoardGrid[pos.X, pos.Y] != BlockType.Empty)
                                    {
                                        owner = BoardGrid[x, y];
                                    }
                                    else if (owner != BlockType.Empty && BoardGrid[pos.X, pos.Y] != owner)
                                    {
                                        owner = BlockType.Empty;
                                        break;
                                    }
                                    if (owner == BlockType.Empty)
                                    {
                                        break;
                                    }
                                    pos.X += BoardDirection[w, 0];
                                    pos.Y += BoardDirection[w, 1];
                                }
                                
                                if (owner != BlockType.Empty)
                                {
                                    winner = owner;
                                }
                            }
                        }
                    }
                }
            }

            if (winner != BlockType.Empty)
            {
                overlay_text = "Winner is " + winner.ToString();
                Game_Run = false;
                start_btn.Text = "start";
                Omokpan_Paint(null, null);
                return true;
            }
            return false;
        }

        private void Omokpan_Paint(object sender, PaintEventArgs e)
        {
            bg.Graphics.Clear(omokpan.BackColor);
            //가로선과 세로선 긋기
            for (int i = 0; i < BoardWidth; i++)
            {
                //가로선
                bg.Graphics.DrawLine(Pens.Black,
                    BoardSize.X + BoardSize.Width * i / (BoardWidth - 1),
                    BoardSize.Y,
                    BoardSize.X + BoardSize.Width * i / (BoardWidth - 1),
                    BoardSize.Y + BoardSize.Height
                );

                //세로선
                bg.Graphics.DrawLine(Pens.Black,
                    BoardSize.X,
                    BoardSize.Y + BoardSize.Height * i / (BoardWidth - 1),
                    BoardSize.X + BoardSize.Width,
                    BoardSize.Y + BoardSize.Height * i / (BoardWidth - 1)
                );
            }

            const int BoardCenter = (BoardWidth) / 2;
            
            //화점 그리기
            for (int x = 1; x < BoardWidth - 1; ++x)
            {
                for (int y = 1; y < BoardWidth - 1; ++y)
                {
                    if ((x - BoardCenter) % 4 == 0 && (y - BoardCenter) % 4 == 0 && Math.Abs((y - BoardCenter)) == Math.Abs(x - BoardCenter))
                    {
                        bg.Graphics.FillEllipse(Brushes.Black,
                            BoardSize.X + BoardSize.Width * x / (BoardWidth - 1) - BoardCircleSize / 2,
                            BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1) - BoardCircleSize / 2,
                            BoardCircleSize,
                            BoardCircleSize
                        );
                    }
                }
            }

            double max_total_benefit = 1;
            double max_ai_benefit = 1;
            double max_player_benefit = 1;
            SolidBrush debugBrush = new SolidBrush(Color.Black);
            
            {
                for (int x = 0; x < BoardWidth; ++x)
                {
                    for (int y = 0; y < BoardWidth; ++y)
                    {
                        if (max_total_benefit < benefit_map[x, y])
                            max_total_benefit = benefit_map[x, y];
                        if (max_ai_benefit < ai_benefit[x, y] && isDebugAI.Checked)
                            max_ai_benefit = ai_benefit[x, y];
                        if (max_player_benefit < user_benefit[x, y] && isDebugPlayerScore.Checked)
                            max_player_benefit = user_benefit[x, y];
                    }
                } 
            }

            //돌 그리기
            for (int x = 0; x < BoardWidth; ++x)
            {
                for (int y = 0; y < BoardWidth; ++y)
                {
                    if (isDebugTotal.Checked || isDebugPlayerScore.Checked || isDebugAI.Checked)
                    {
                        if (benefit_map[x, y] > 0 || ai_benefit[x, y] > 0 || user_benefit[x, y] > 0)
                        {
                            if (isDebugTotal.Checked)
                                debugBrush.Color = Color.FromArgb((int)Math.Floor(benefit_map[x, y] * 255 / max_total_benefit), 0, 255, 0);
                            else
                            {
                                if (isDebugAI.Checked && isDebugPlayerScore.Checked)
                                {
                                    debugBrush.Color = Color.FromArgb(
                                        (int)Math.Floor(benefit_map[x, y] * 255 / max_total_benefit),
                                        (isDebugAI.Checked && max_ai_benefit > 0) ? (int)Math.Floor(ai_benefit[x, y] * 255 / max_ai_benefit) : 0, 0,
                                        (isDebugPlayerScore.Checked && max_player_benefit > 9) ? (int)Math.Floor(user_benefit[x, y] * 255 / max_player_benefit) : 0
                                        );
                                }
                                else if (isDebugAI.Checked)
                                {
                                    debugBrush.Color = Color.FromArgb((int)Math.Floor(ai_benefit[x, y] * 255 / max_ai_benefit), 255, 0, 0);
                                }
                                else
                                {
                                    debugBrush.Color = Color.FromArgb((int)Math.Floor(user_benefit[x, y] * 255 / max_player_benefit), 0, 0, 255);
                                }
                            }
                            bg.Graphics.FillRectangle(debugBrush, 
                                BoardSize.X + BoardSize.Width * (x - 0.5f) / (BoardWidth - 1), BoardSize.Y + BoardSize.Height * (y - 0.5f) / (BoardWidth - 1),
                                 BoardSize.Width / (BoardWidth - 1), BoardSize.Height / (BoardWidth - 1));
                        }
                    }

                    switch (BoardGrid[x, y])
                    {
                        case BlockType.BlackStone:
                            bg.Graphics.DrawImage(sprite_Blackstone,
                                BoardSize.X + BoardSize.Width * x / (BoardWidth - 1) - BoardStoneSize / 2,
                                BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1) - BoardStoneSize / 2,
                                BoardStoneSize,
                                BoardStoneSize
                            );
                            break;
                        case BlockType.WhiteStone:
                            bg.Graphics.DrawImage(sprite_Whitestone,
                                BoardSize.X + BoardSize.Width * x / (BoardWidth - 1) - BoardStoneSize / 2,
                                BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1) - BoardStoneSize / 2,
                                BoardStoneSize,
                                BoardStoneSize
                            );
                            break;
                    }
                    //if (x == Ai_lastPlace.X && y == Ai_lastPlace.Y)
                    if (isDebugTotal.Checked)
                    {
                        if (benefit_map[x, y] > 0)
                        {
                            if (BoardGrid[x, y] == BlockType.BlackStone)
                                bg.Graphics.DrawString(benefit_map[x, y].ToString(), Game_font, Brushes.White, BoardSize.X + BoardSize.Width * x / (BoardWidth - 1), BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1), centerFormat);
                            if (BoardGrid[x, y] == BlockType.WhiteStone)
                                bg.Graphics.DrawString(benefit_map[x, y].ToString(), Game_font, Brushes.Black, BoardSize.X + BoardSize.Width * x / (BoardWidth - 1), BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1), centerFormat);
                            else
                            {
                                bg.Graphics.DrawString(benefit_map[x, y].ToString(), Game_font, Brushes.Black, 1 + BoardSize.X + BoardSize.Width * x / (BoardWidth - 1), 1 + BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1), centerFormat);
                                bg.Graphics.DrawString(benefit_map[x, y].ToString(), Game_font, Brushes.White, BoardSize.X + BoardSize.Width * x / (BoardWidth - 1), BoardSize.Y + BoardSize.Height * y / (BoardWidth - 1), centerFormat);
                            }
                        }
                    }
                }
            }

            if (overlay_text.Length > 0)
            {
                bg.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(128, 0, 0, 0)), 0, 0, omokpan.Width, omokpan.Height);
                bg.Graphics.DrawString(overlay_text, Overlay_font, Brushes.White, BoardSize.X + BoardSize.Width / 2, BoardSize.Y + BoardSize.Height / 2, centerFormat);
            }
            bg.Render();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            for (int x = 0; x < BoardWidth; ++x)
            {
                for (int y = 0; y < BoardWidth; ++y)
                {
                    for (int w = 0; w < 4; ++w)
                    {
                        int end_x = x + BoardDirection[w, 0] * 4;
                        int end_y = y + BoardDirection[w, 1] * 4;
                        if (0 <= end_x && end_x < BoardWidth && 0 <= end_y && end_y < BoardWidth)
                        {
                            //User Benefit 계산
                            {
                                int count = 0;
                                int start = -1;
                                int end = -1;
                                Point pos = new Point(x, y);
                                for (int i = 0; i < 5; ++i)
                                {
                                    if (BoardGrid[pos.X, pos.Y] == Ai_stone)
                                    {
                                        count = 0;
                                        //5개를 이어서 놓을수 없는 자리
                                        break;
                                    }
                                    else if (BoardGrid[pos.X, pos.Y] == User_stone)
                                    {
                                        if (start == -1) start = i;
                                        end = i;
                                        ++count;
                                    }
                                    pos.X += BoardDirection[w, 0];
                                    pos.Y += BoardDirection[w, 1];
                                }

                                if (count > 0)
                                {
                                    //돌 사이의 빈 공간
                                    int space = end - start + 1 - count;

                                    double score = Ai_point[count, space];
                                    pos = new Point(x, y);
                                    for (int i = 0; i < 5; ++i)
                                    {
                                        user_benefit[pos.X, pos.Y] += score;
                                        pos.X += BoardDirection[w, 0];
                                        pos.Y += BoardDirection[w, 1];
                                    }
                                }
                            }

                            //AI Benefit 계산
                            {
                                int count = 0;
                                int start = -1;
                                int end = -1;
                                Point pos = new Point(x, y);
                                for (int i = 0; i < 5; ++i)
                                {
                                    if (BoardGrid[pos.X, pos.Y] == User_stone)
                                    {
                                        count = 0;
                                        //5개를 이어서 놓을수 없는 자리
                                        break;
                                    }
                                    else if (BoardGrid[pos.X, pos.Y] == Ai_stone)
                                    {
                                        if (start == -1) start = i;
                                        end = i;
                                        ++count;
                                    }
                                    pos.X += BoardDirection[w, 0];
                                    pos.Y += BoardDirection[w, 1];
                                }

                                if (count > 0)
                                {
                                    //돌 사이의 빈 공간
                                    int space = end - start + 1 - count;

                                    double score = Ai_point[count, space];
                                    if (count == 4) score *= 10;
                                    pos = new Point(x, y);
                                    for (int i = 0; i < 5; ++i)
                                    {
                                        ai_benefit[pos.X, pos.Y] += score;
                                        pos.X += BoardDirection[w, 0];
                                        pos.Y += BoardDirection[w, 1];
                                    }
                                }
                            }
                        }
                    }
                }
            }

            double best_score = -1;
            Point best_pos = new Point(-1, -1);

            for (int x = 0; x < BoardWidth; ++x)
            {
                for (int y = 0; y < BoardWidth; ++y)
                {
                    if (BoardGrid[x, y] == BlockType.Empty)
                    {
                        double score = ai_benefit[x, y] + user_benefit[x, y];
                        benefit_map[x, y] = score;
                        if (score > best_score)
                        {
                            best_score = score;
                            best_pos = new Point(x, y);
                        }
                    }
                    else
                    {
                        benefit_map[x, y] = 0;
                    }
                }
            }

            Ai_lastPlace = best_pos;
            action_Place_stone(Ai_stone, best_pos.X, best_pos.Y);
            Game_Turn = User_stone;

            action_Check_Winner();

            timer1.Enabled = false;
        }

        private void isDebug_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked) isDebugTotal.Checked = false;
            Omokpan_Paint(null, null);
        }
        private void isDebugTotal_CheckedChanged(object sender, EventArgs e)
        {
            if (((CheckBox)sender).Checked)
            {
                isDebugAI.Checked = false;
                isDebugPlayerScore.Checked = false;
            }
            Omokpan_Paint(null, null);
        }
    }
}
