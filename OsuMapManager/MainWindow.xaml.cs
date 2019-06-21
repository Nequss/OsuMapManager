using System;
using System.Windows;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;

namespace OsuMapManager
{
    public partial class MainWindow : Window
    {
        public string[] files;
        public string[] dirs;
        public FolderBrowserDialog dialog;
        public string selected;

        public bool hp_flag = false;
        public bool ar_flag = false;
        public bool od_flag = false;
        public bool cs_flag = false;

        public List<string> maplist = new List<string>();

        public MainWindow()
        {
            ResizeMode = ResizeMode.NoResize;
            InitializeComponent();
        }

        private void select_path(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Listofmaps.Items.Clear();

            dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = false;
            DialogResult result = dialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                status_txt.Text = "Loading maps...";

                dirs = Directory.GetDirectories(dialog.SelectedPath);

                foreach (string dir in dirs)
                {
                    files = Directory.GetFiles(dir, "*.osu");
                    foreach (string file in files)
                    {
                        maplist.Add(Path.GetFileName(file));
                        Listofmaps.Items.Add(Path.GetFileName(file));
                    }
                }
            }
        }

        private void Listofmaps_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            selected = Listofmaps.SelectedItem as String;

            foreach (string dir in dirs)
            {
                files = Directory.GetFiles(dir, "*.osu");
                foreach (string file in files)
                {
                    if (selected == Path.GetFileName(file))
                    {
                        string[] lines = File.ReadAllLines(Path.GetFullPath(file));
                        hp_flag = false;
                        ar_flag = false;
                        od_flag = false;
                        cs_flag = false;

                        bool title_flag = false;
                        bool artist_flag = false;
                        bool creator_flag = false;

                        foreach (string line in lines)
                        {
                            Regex check = new Regex("Title:");

                            if (title_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapTitle.Text = "Title: " + Regex.Replace(line, "Title:", "");
                                    title_flag = true;
                                }

                            check = new Regex("Artist:");

                            if (artist_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapArtist.Text = "Artist: " + Regex.Replace(line, "Artist:", "");
                                    artist_flag = true;
                                }

                            check = new Regex("Creator:");

                            if (creator_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapCreator.Text = "Creator: " + Regex.Replace(line, "Creator:", "");
                                    creator_flag = true;
                                }

                            check = new Regex("HPDrainRate:.");

                            if (hp_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapHP.Text = Regex.Replace(line, "HPDrainRate:", "");
                                    hp_flag = true;
                                }

                            check = new Regex("CircleSize:.");

                            if (cs_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapCS.Text = Regex.Replace(line, "CircleSize:", "");
                                    cs_flag = true;
                                }

                            check = new Regex("OverallDifficulty:.");

                            if (od_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapOD.Text = Regex.Replace(line, "OverallDifficulty:", "");
                                    od_flag = true;
                                }

                            check = new Regex("ApproachRate:.");

                            if (ar_flag == false)
                                if (check.Match(line).Success)
                                {
                                    mapAR.Text = Regex.Replace(line, "ApproachRate:", "");
                                    ar_flag = true;
                                }
                        }
                    }
                }
            }
        }

        private void Save_map_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (mapAR.Text != "0")
                ar_flag = true;

            foreach (string dir in dirs)
            {
                files = Directory.GetFiles(dir);

                foreach (string file in files)
                {
                    if (selected == Path.GetFileName(file))
                    {
                        string path_to_folder = Path.GetDirectoryName(file) + " Edited";

                        Directory.CreateDirectory(path_to_folder);

                        foreach (var filee in files)
                            if (selected == Path.GetFileName(filee) & Path.GetExtension(filee) == ".osu")
                            {
                                string[] lines = File.ReadAllLines(Path.GetFullPath(filee));
                                List<String> newLines = new List<string>();

                                foreach (string line in lines)
                                {

                                    Regex checkHP = new Regex("HPDrainRate:.");
                                    Regex checkOD = new Regex("OverallDifficulty:.");
                                    Regex checkCS = new Regex("CircleSize:.");
                                    Regex checkAR = new Regex("ApproachRate:.");

                                    if (checkHP.Match(line).Success)
                                    {
                                        newLines.Add("HPDrainRate:" + mapHP.Text);
                                        hp_flag = false;
                                    }
                                    else if (checkOD.Match(line).Success)
                                    {
                                        newLines.Add("OverallDifficulty:" + mapOD.Text);
                                        od_flag = false;
                                    }
                                    else if (checkCS.Match(line).Success)
                                    {
                                        newLines.Add("CircleSize:" + mapCS.Text);
                                        cs_flag = false;
                                    }
                                    else if ((!cs_flag & !od_flag) & (!hp_flag & ar_flag))
                                    {
                                        newLines.Add("ApproachRate:" + mapAR.Text);
                                        ar_flag = false;
                                    }
                                    else
                                        newLines.Add(line);
                                }

                                using (var tw = new StreamWriter(path_to_folder + @"\" + Path.GetFileNameWithoutExtension(filee) + " Edited.osu", true))
                                {
                                    foreach (string newline in newLines)
                                        tw.WriteLine(newline);

                                    tw.Close();
                                }
                            }
                            else if (Path.GetExtension(filee) == ".osu")
                                continue;
                            else
                                File.Copy(Path.GetFullPath(filee), path_to_folder + @"\" + Path.GetFileName(filee), true);
                    }
                }
            }
        }

        private void Search_btn_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (search_text.Text != "")
            {
                foreach (string item in maplist)
                    if (!item.ToLower().Contains(search_text.Text.ToLower()))
                        Listofmaps.Items.Remove(item);
            }

        }

        private void Refresh_btn_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Listofmaps.Items.Clear();

            foreach (string item in maplist)
                Listofmaps.Items.Add(item);
        }
    }
}