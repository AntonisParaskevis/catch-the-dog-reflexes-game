using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        // game_is_being_played: if it's set to "true", the game is running
        // game_can_start: if it's set to "true", all the conditions for the game to be able to start are fulfilled
        // clicked: if it's set to "false" and the game is running, the player can score a point when they click on the image.
        // After clicking on the image, it will be set to "true", and the player won't be able to score more points until the image has moved
        public Boolean game_is_being_played, game_can_start, clicked = false;

        // The game time is stored in this variable
        byte time = 60; 

        // The score and the coordinates of the image are stored in these variables, respectively
        short score = 0, x_coordinate, y_coordinate;

        //Create and initialize an object of the Random class, in order to randomly generate new coordinates for the image
        Random random = new Random();

        // The game difficulty is stored in this variable. It will eventually be stored in the previous attempts file, along with the player's score, name and the time when the game was played 
        string difficulty = "Medium"; 

        private void pictureBox1_Click(object sender, EventArgs e) 
        {
            if (game_is_being_played & clicked == false)
            {
                // Increase the score by 1
                score++;

                // Update the GUI element that displays the score
                label1.Text = "Score: " + score;

                // Once the user clicks on the image, they can't score points by clicking on it again before it changes place
                clicked = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // The location of the label4 element, which contains the text "Game Paused", is controlled through code, in order to make sure that it appears exactly in the center of the form
            label4.Left = (ClientSize.Width - label4.Width) / 2;
            label4.Top = (ClientSize.Height - label4.Height) / 2;
        }

        private void newgameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (game_can_start)
            {
                //Disable the name field, so that the player cannot change their name in mid-game
                textBox1.ReadOnly = true;

                // Start the game
                game_is_being_played = true;

                // Start the game timer
                timer1.Enabled = true;

                // The image starts changing places
                timer2.Enabled = true;

                // Disable the toolstrip menu items so the player won't be able to change the difficulty, view the previous attempts or start a new game while the game is running
                gameToolStripMenuItem.Enabled = false;
                difficultyToolStripMenuItem.Enabled = false;
                previousAttemptsToolStripMenuItem.Enabled = false;

                // Enable the pause button, so the player will be able to pause the game, if they need to
                button1.Enabled = true;

                // If the file containing the previous attempts doesn't exist, create it
                if (!File.Exists(@"c:\Previous_Attempts.txt")) 
                { 
                    using (File.Create(@"c:\Previous_Attempts.txt")) { }
                }
            }
            else
            {
                // If the user hasn't entered their name in the name field, display a message prompting them to do so
                MessageBox.Show("Enter your name in the field on the top side of the screen and try again.", "You didn't enter your name"); 
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (game_is_being_played)
            {
                // Every passing second, reduce the game time by 1
                time--;

                // Update the GUI element that displays the game time
                label2.Text = "Time: " + time;
            }
            if (time == 0)
            {
                // Re-enable the name field, so a different name can be entered, if desired
                textBox1.ReadOnly = false;

                // Finish the game
                game_is_being_played = false;

                // The game time stops running
                timer1.Enabled = false;

                // The image stops changing places
                timer2.Enabled = false;

                // Disable the pause button, it's not needed since the game is not running
                button1.Enabled = false;

                // Display a message saying that the time is up, along with the player's score
                MessageBox.Show("Time is up!\n\nYour score: " + score, "Time is up!");

                // Open the file that contains the previous attempts, in order to record information about the game
                TextWriter tsw = new StreamWriter(@"C:\Previous_Attempts.txt", true);

                // Enter a line containing the player's name, score, current date and time, and game diffuculty into the file
                tsw.WriteLine(textBox1.Text + "    " + score + "    " + DateTime.Now + "    " + difficulty);

                // Close the file, we don't need it open anymore
                tsw.Close();

                // Reset the score and the time, so the game will be ready for the next playthrough
                score = 0;
                time = 60;

                // Update the GUI elements that display the score and the time
                label1.Text = "Score: 0"; 
                label2.Text = "Time: 60";

                // Re-enable the toolstrip menu items
                gameToolStripMenuItem.Enabled = true;
                difficultyToolStripMenuItem.Enabled = true;
                previousAttemptsToolStripMenuItem.Enabled = true;
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Exit the game
            Application.Exit();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (game_is_being_played)
            {
                // Randomly generate the new x coordinate of the image
                x_coordinate = (short)random.Next(0, ClientSize.Width - pictureBox1.Width);

                // Randomly generate the new y coordinate of the image
                y_coordinate = (short)random.Next(0, ClientSize.Height - pictureBox1.Height);

                // Place the image in the new coordinates
                pictureBox1.Location = new Point(x_coordinate, y_coordinate);

                // Re-enable the ability to score points when clicking on the image
                clicked = false;
            }
        }

        private void previousAttemptsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(@"c:\Previous_Attempts.txt"))
            {
                // If the previous attempts file exists, display all of its contents, the previous attempts, into a messagebox element
                string readText = File.ReadAllText(@"C:\Previous_Attempts.txt");
                MessageBox.Show(readText, "Previous Attempts");
            }
            else 
            {
                // If the previous attempts file doesn't exist, display a message saying that there are no previous attempts
                MessageBox.Show("There are no previous attempts yet.", "No previous attempts yet");
            }
        }

        private void easyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Easy", uncheck the rest
            easyToolStripMenuItem.Checked = true;
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = false;

            // Make the image change places every 1,4 seconds
            timer2.Interval = 1400;

            // Change the difficulty string to "Easy" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Easy";
        }

        private void button1_Click(object sender, EventArgs e) // pause/resume button
        {
            if (game_is_being_played) // If the game is running and the player pauses it
            {
                // Disable both timers, in order to freeze the game's functions
                timer1.Enabled = false;
                timer2.Enabled = false;

                // Change the word "Pause" in the pause/unpause button to "Resume", in order to make its function clear to the player
                button1.Text = "Resume";

                // Show the text "Game Paused" in the middle of the form, in order to notify the user that the game is paused
                label4.Visible = true;

                // The game isn't running, so set game_is_being_played to false
                game_is_being_played = false;
            }
            else // If the game is paused and the player unpauses it
            {
                // Re-enable both timers, in order to unfreeze the game's functions
                timer1.Enabled = true;
                timer2.Enabled = true;

                // Change the word "Resume" in the pause/unpause button to "Pause", in order to make its function clear to the player
                button1.Text = "Pause";

                // Hide the text "Game Paused" from the middle of the form, as it's unneeded and obstructive for the player
                label4.Visible = false;

                // The game is running once again, so set game_is_being_played to true
                game_is_being_played = true;
            }
        }

        private void mediumToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Medium", uncheck the rest
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = true;
            hardToolStripMenuItem.Checked = false;

            // Make the image change places every 1 second
            timer2.Interval = 1000;

            // Change the difficulty string to "Medium" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Medium"; 
        }

        private void hardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Check "Hard", uncheck the rest
            easyToolStripMenuItem.Checked = false;
            mediumToolStripMenuItem.Checked = false;
            hardToolStripMenuItem.Checked = true;

            // Make the image change places every 0,7 seconds
            timer2.Interval = 700;

            // Change the difficulty string to "Hard" so that if a game with this difficulty enabled is played, this difficulty will be stored in the previous attempts file, along with the rest of the game's information
            difficulty = "Hard"; 
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox1.Text)) 
            {
                // If the name field contains no characters or contains only blank characters (spaces or tabs), the game cannot start 
                game_can_start = false;
            }
            else 
            {
                // Otherwise, the game can start
                game_can_start = true;
            }
        }
    }
}
