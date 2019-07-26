using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using System;
using System.IO;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;






namespace Tetris
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    /// 





    public class Game1 : Game
    {


        const int M = 21; //wysokosc planczy
        const int N = 10; //szerokosc planszy
        int dx = 0; //przesun w lewo prawo -1, +1
        bool rotate = false; //obroc
        int colorNum = 1; //ktory kolor
        int colorNum2 = 2;
        float timer = 0f; //czas do spadania klocka
        float timer2 = 0f; //czas do przesuwania klocka
        float delay = 0.66f; //opoznenie do spadania klocka
        float delay2 = 0.1f; //opoznienie do przesowana klocka
        float skala = 1f; //skalowanie
        public int n2;
        Random rnd = new Random(); //losowanie
        int pkt = 0; 
        string punkty;
        private SpriteFont font;
        bool pauza = true;
        bool info = false;
        bool game_over = false;
        int game_over_sound = 0;
        int full_x;
        int lvl = 1;
        int exp = 0;
        string path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "tetris_score.txt");
        string path_save = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "data.txt");
        string top_pkt = "0";
        int top_score = 0;
        int []progress_bar = new int [32];
        float volume = 0.2f;

        int[,] field = new int[M, N]; //pole gry tablica
        Point[] a = new Point[4]; //wylosowany klocek
        Point[] b = new Point[4];
        Point[] c = new Point[4]; //kolejny klocek
        int[,] figures = new int [7,4] //tablica dostepnych klockow
        {
            { 1,3,5,7 }, // I
            { 2,4,5,7 }, // Z
            { 3,5,4,6 }, // S
            { 3,5,4,7 }, // T
            { 2,3,5,7 }, // L
            { 3,5,7,6 }, // J
            { 2,3,4,5 }, // O
        };





        public bool check() //kolizje
        {
            for (int i = 0; i < 4; i++)
                if (a[i].X < 0 || a[i].X >= N || a[i].Y >= M || a[i].Y<0)
                {
                    return false;
                }
                else if (field[a[i].Y, a[i].X] > 0)
                {
                    return false;

                }

            return true;
        }

        public void reset() //reset mapy , punkotow i lvl
        {
            for (int i = 0; i<M; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    field[i, j]=0;
                }
            }
            for (int i = 0; i < 32; i++)
            {
                progress_bar[i] = 0;
            }




            //zapis wyniku jesli lepszy
            if (pkt > top_score)
            {
                var writer = File.CreateText(path);
                writer.WriteLineAsync(pkt.ToString());
                writer.Close();

                top_score = pkt;
                if (top_score < 10)
                    top_pkt = "00" + top_score;
                if (top_score >= 10 && top_score < 100)
                    top_pkt = "0" + top_score;
                if (top_score >= 100)
                    top_pkt = top_score.ToString();

            }

            exp = 0;
            pkt = 0;
            lvl = 1;
            pauza = false;
            game_over = false;
            game_over_sound = 0;
            MediaPlayer.Stop();
            MediaPlayer.Volume = volume = 0.2f;

        }

        //zapis stanu gry przed jej zamknieciem
        public void save_game()
        {
            var writer = File.CreateText(path_save);
            for (int i = 0; i < M; i++)
                for (int j = 0; j < N; j++)
                {
                    writer.WriteLineAsync(field[i,j].ToString());
                }

            for (int i = 0; i < 4; i++)
            {
                writer.WriteLineAsync(a[i].X.ToString());
                writer.WriteLineAsync(a[i].Y.ToString());
            }

            for (int i = 0; i < 32; i++)
            {
                writer.WriteLineAsync(progress_bar[i].ToString());
            }
            writer.WriteLineAsync(exp.ToString());
            writer.WriteLineAsync(pkt.ToString());
            writer.WriteLineAsync(lvl.ToString());
            writer.Close();
        }

        //zaladowanie zapisanego stanu gry
        public void load_game()
        {
            if (File.Exists(path_save))
            {
                using (var streamReader = new StreamReader(path_save)) //odczyt najlepszego wyniku
                {

                    for (int i = 0; i < M; i++)
                        for (int j = 0; j < N; j++)
                        {
                            field[i, j] = Convert.ToInt32 (streamReader.ReadLine());
                        }

                    for (int i = 0; i < 4; i++)
                    {
                        a[i].X = Convert.ToInt32(streamReader.ReadLine());
                        a[i].Y = Convert.ToInt32(streamReader.ReadLine());
                    }


                    for (int i = 0; i < 32; i++)
                    {
                        progress_bar[i] = Convert.ToInt32(streamReader.ReadLine());
                    }

                    exp = Convert.ToInt32(streamReader.ReadLine());
                    pkt = Convert.ToInt32(streamReader.ReadLine());
                    lvl = Convert.ToInt32(streamReader.ReadLine());
                }
            }

        }

        public void lvl_up()
        {
            for (int i = 0; i < 32; i++)
            {
                progress_bar[i] = 0;
            }
            exp = 0;
            lvl ++;
            sound_lvl_up.Play();
        }


        void MediaPlayer_MediaStateChanged(object sender, System. EventArgs e)
        {
            MediaPlayer.Volume = volume;
            MediaPlayer.Play(song);
        }

        //tekstury
        Texture2D klocek;
        Texture2D tlo;
        Texture2D ramka;
        Texture2D bt_start;
        Texture2D bt_pauza;
        Texture2D image_game_over;
        Texture2D image_info;
        Texture2D image_pauza;
        Texture2D image_ramka_exp;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        //dzwieki
        Song song;
        SoundEffect sound_click_button;
        SoundEffect sound_lvl_up;
        SoundEffect sound_tap;
        SoundEffect sound_game_over;

        public Game1()
        {

            //ustalanie skali
            full_x = Window.ClientBounds.Width;
            int full_y = Window.ClientBounds.Height;
            if (full_x / 480f < full_y / 800f) //zmien na800
                skala = (float)full_x / 480f;
            else
                skala = (float)full_y / 800f; //zmien na 800
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
            graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
            graphics.SupportedOrientations = DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        /// 






        protected override void Initialize()
        {






            //dostepne gesty
            TouchPanel.EnabledGestures = GestureType.HorizontalDrag | GestureType.VerticalDrag | GestureType.Tap;
            //losowanie pierwszego klocka
            int n = rnd.Next(7);
            n2 = rnd.Next(7);
            for (int i = 0; i < 4; i++)
            {
                a[i].X = figures[n, i] % 2 + 2;
                a[i].Y = figures[n, i] / 2;
            }
            for (int i = 0; i < 4; i++)
            {
                c[i].X = figures[n2, i] % 2 + 12;
                c[i].Y = figures[n2, i] / 2 +7;
            }
            // TODO: Add your initialization logic here
            //jesli plik z najlepszym wunikiem NIE istnieje utworz go i zapisz 0
            if (!File.Exists(path))
            {
                var writer = File.CreateText(path);
                writer.WriteLineAsync("0");
                writer.Close();
            }
            using (var streamReader = new StreamReader(path)) //odczyt najlepszego wyniku
            {
                top_pkt = streamReader.ReadToEnd();
                top_score = Convert.ToInt32(top_pkt);
                streamReader.Close();
                if (top_score < 10)
                    top_pkt = "00" + top_score;
                if (top_score >= 10 && top_score < 100)
                    top_pkt = "0" + top_score;
                if (top_score >= 100)
                    top_pkt = top_score.ToString();
            }
            
            load_game();
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            //ladowanie tekstur
            spriteBatch = new SpriteBatch(GraphicsDevice);
            klocek = Content.Load<Texture2D>("tiles");
            tlo = Content.Load<Texture2D>("background");
            ramka = Content.Load<Texture2D>("frame");
            font = Content.Load<SpriteFont>("Font");
            bt_start = Content.Load<Texture2D>("start");
            bt_pauza = Content.Load<Texture2D>("pauza");
            image_game_over = Content.Load<Texture2D>("game_over");
            image_info = Content.Load<Texture2D>("info");
            image_pauza = Content.Load<Texture2D>("pauza_screen");
            image_ramka_exp = Content.Load<Texture2D>("ramka_exp");
            //sciezka dzwiekowa
            this.song = Content.Load<Song>("tetris_piano");
            MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
            MediaPlayer.Play(song);
            //efekty dzwiekowe
            this.sound_click_button = Content.Load<SoundEffect>("click_button");
            this.sound_lvl_up = Content.Load<SoundEffect>("lvl_up");
            this.sound_tap = Content.Load<SoundEffect>("tap");
            this.sound_game_over = Content.Load<SoundEffect>("sound_game_over");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                save_game();
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
            }
           // Exit();
            //liczniki do opadania i przesowania klocka
            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            timer2 += (float)gameTime.ElapsedGameTime.TotalSeconds;

            while (TouchPanel.IsGestureAvailable)
            {
                GestureSample gestureSample = TouchPanel.ReadGesture();
                if (gestureSample.GestureType == GestureType.Tap)
                {
                    bool click_button = false;
                    if (info)
                    {
                        info = false;
                        pauza = false;
                        click_button = true;
                        MediaPlayer.Volume = volume = 0.2f;
                        
                    }


                    if (gestureSample.Position.X > 0 && gestureSample.Position.X < full_x && gestureSample.Position.Y > 660f * skala && gestureSample.Position.Y < 730f * skala)
                    {
                        if (pauza)
                        {
                            MediaPlayer.Volume = volume = 0.2f;
                            pauza = false;
                        }
                        else
                        {
                            MediaPlayer.Volume = volume = 0f;
                            pauza = true;
                        }
                        click_button = true;
                        sound_click_button.Play();
                    }
                    if (gestureSample.Position.X > 0 && gestureSample.Position.X < full_x && gestureSample.Position.Y > 730f * skala && gestureSample.Position.Y < 800f * skala)
                    {
                        if (info)
                        {
                            pauza = false;
                            info = false;
                        }
                        else
                        {
                            pauza = true;
                            info = true;
                        }
                        click_button = true;
                        sound_click_button.Play();
                    }

                    if(!click_button)
                        rotate = true;

                    if (game_over)
                        reset();

                    



                }
                if (gestureSample.GestureType == GestureType.VerticalDrag)
                {
                    if (gestureSample.Delta.Y > 0)
                        delay = 0.05f;

                }
                if (gestureSample.GestureType == GestureType.HorizontalDrag && timer2>delay2)
                {
                    if (gestureSample.Delta.X < -5)
                        dx = -1;
                    if (gestureSample.Delta.X > 5)
                        dx = +1;
                    timer2 = 0;
                }
            }

            for (int i = 0; i < N; i++)
            {
                if (field[1, i] > 0)
                {
                    pauza = true;
                    game_over = true;
                    if(game_over_sound!=2)
                    game_over_sound = 1;

                }
            }

            if (game_over_sound==1)
            {
                game_over_sound = 2;
                sound_game_over.Play();
                MediaPlayer.Volume = volume = 0f;

            }

            if (!pauza) //Jeœli nie pauza//
            {
                //// <- Move -> ///

                for (int i = 0; i < 4; i++)
                {
                    b[i] = a[i];
                    a[i].X += dx;
                }
                if (!check())
                    for (int i = 0; i < 4; i++) a[i] = b[i];

                //////Rotate//////
                if (rotate)
                {
                    Point p = a[1]; //center of rotation
                    for (int i = 0; i < 4; i++)
                    {
                        int x = a[i].Y - p.Y;
                        int y = a[i].X - p.X;
                        a[i].X = p.X - x;
                        a[i].Y = p.Y + y;
                    }
                    if (!check()) for (int i = 0; i < 4; i++) a[i] = b[i];
                }

                ///////Tick//////
                if (timer > delay)
                {
                    for (int i = 0; i < 4; i++)
                    { b[i] = a[i]; a[i].Y += 1; }

                    if (!check())
                    {
                        for (int i = 0; i < 4; i++) field[b[i].Y, b[i].X] = colorNum;

                        colorNum = colorNum2;
                        colorNum2 = 1 + rnd.Next(7);
                        int n = n2;
                        n2 = rnd.Next(7);
                        for (int i = 0; i < 4; i++)
                        {
                            a[i].X = figures[n, i] % 2 + 2;
                            a[i].Y = figures[n, i] / 2;
                        }

                        for (int i = 0; i < 4; i++)
                        {
                            c[i].X = figures[n2, i] % 2 + 12;
                            c[i].Y = figures[n2, i] / 2 + 7;
                        }
                    }


                    timer = 0;
                }


            }





                ///////check lines//////////
                bool zdobyto_punkty = false;
                int k = M - 1;
                for (int i = M - 1; i > 0; i--)
                {
                    int count = 0;
                    for (int j = 0; j < N; j++)
                    {
                        if (field[i, j] > 0) count++;
                        field[k, j] = field[i, j];

                    }
                    if (count < N)
                    {
                        k--;
                    }
                    else
                    {
                        pkt +=lvl;
                        zdobyto_punkty = true;
                        exp++;
                        progress_bar[exp-1] = 1 + rnd.Next(4);
                    /// Lvl Up///
                    if (exp >= 32)
                    { lvl_up(); }
                    }
                }

            if (zdobyto_punkty)
                sound_tap.Play();

                if (pkt < 10)
                    punkty = "00" + pkt;
                if (pkt >= 10 && pkt < 100)
                    punkty = "0" + pkt;
                if (pkt >= 100)
                    punkty = pkt.ToString();
            
            dx = 0;
            rotate = false;
            delay = 0.7f-(float)lvl*0.04f;
            // TODO: Add your update logic here

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            spriteBatch.Draw(tlo, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            spriteBatch.Draw(ramka, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            if(pauza)
                spriteBatch.Draw(bt_start, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(bt_pauza, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);


            for (int i = 0; i < M; i++)
                for (int j = 0; j < N; j++)
                {
                    if (field[i,j] == 0) continue;
                    spriteBatch.Draw(klocek, new Vector2(j * (30f*skala), i * (30f * skala)), new Rectangle(field[i,j] * 30, 0, 30, 30), Color.White, 0f, new Vector2(-30, 0), skala, SpriteEffects.None, 0f);
                }

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(klocek, new Vector2(a[i].X * (30f * skala), a[i].Y * (30f * skala)), new Rectangle(colorNum * 30, 0, 30, 30), Color.White,0f, new Vector2(-30,0), skala, SpriteEffects.None, 0f);
            }

            for (int i = 0; i < 4; i++)
            {
                spriteBatch.Draw(klocek, new Vector2(c[i].X * (30f * skala), c[i].Y * (30f * skala)), new Rectangle(colorNum2 * 30, 0, 30, 30), Color.White, 0f, new Vector2(-30, 0), skala, SpriteEffects.None, 0f);
            }
            spriteBatch.DrawString(font, punkty, new Vector2(12.5f*30f*skala, 14f * 30f * skala), Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, top_pkt, new Vector2(12.5f * 30f * skala, 19.5f * 30f * skala), Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            for (int i = 0; i < 32; i++)
            {
                if (progress_bar[i] == 0) continue;
                spriteBatch.Draw(klocek, new Vector2(i * (15f * skala), 26.8f * (30f * skala)), new Rectangle(progress_bar[i] * 30, 0, 30, 30), Color.White, 0f, new Vector2(0, 0), skala / 2f, SpriteEffects.None, 0f);
            }
            spriteBatch.Draw(image_ramka_exp, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            spriteBatch.DrawString(font, lvl + " lvl", new Vector2(7f * 30f * skala, 26.5f * 30f * skala), Color.White, 0f, new Vector2(0, 0), skala / 2, SpriteEffects.None, 0f); ;
            if (game_over)
            spriteBatch.Draw(image_game_over , new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            if (info)
                spriteBatch.Draw(image_info, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            if (pauza && !game_over && !info)
            {
                spriteBatch.Draw(image_pauza, new Vector2(0, 0), null, Color.White, 0f, new Vector2(0, 0), skala, SpriteEffects.None, 0f);
            }



            spriteBatch.End();

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
