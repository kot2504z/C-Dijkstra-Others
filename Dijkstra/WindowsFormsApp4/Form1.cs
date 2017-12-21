/*
 * 20.12
Jest tak: pozycja x,y kazdego okregu odpowiednio 1,2,3,4...

Polaczenia okregow w postaci listy punktow (zaleznie od kolejnosci laczenia czyli mozemy zaczac od 66 z 44 itd)
tzn: okrag 1 z 4, 2 z 4 1 z 3, 5 z 1, (trzeba to posortowac, zamienic dla 5 z 1, 1 z 5, 
albo jakos inaczej to obejsc zeby latwiej zaimplementowac djikstre.

Wartosci polaczenia odpowiednio dla kolejnosci polaczenia okregow liniami tzn 66 z 44 na miejscu pierwszym i odpowiadajaca mu wartosc

Ja bym to posortowal tzn, zeby najpierw byly poleczenia 1 z reszta - czyli trzeba zamienic np. 5 z 1 na 1 z 5 i wtedy posortowac,
policzyc ile polaczen ma 1 z reszta i zapisac wartosci tych polaczen do jakiejs tablicy oraz do drugiej tablicy ile jest polaczen dla jedynki..
a dalej to sie pomysli (moze warto skonsultowac z prowadzacym?)

  * update 21.12
  * no to sie pokielbasilo, powstal burdel ktory wydaje sie byc coraz blizszy dzialania
  * pozniej trzeba bedzie polowe wywalic
  * 
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp4
{
    public partial class Form1 : Form
    {

        //dunno why i did that, but its fine to make global objects/variables
        Pen pen = new Pen(Color.Black, 3);
        List<Point> mouse_position = new List<Point>();
        List<Point> connections = new List<Point>();
        List<int> values = new List<int>();
        List<int> how_many_connections = new List<int>();
        List<int> values_summary = new List<int>();
        //List<int> control_sum = new List<int>();

        //anti-terrorist events
        private bool combo1 = false, combo2 = false, was_pressed = false;

        private float radius;
            public float Radius { get => radius; set => radius = value; }        

        private int numberOf_circles;
            public int NumberOf_circles { get => numberOf_circles; set => numberOf_circles = value; }        

        //private int value;
        //    public int Value { get => value; set => this.value = value; }

        private bool has_been_assign;
            public bool Has_been_assign { get => has_been_assign; set => has_been_assign = value; }

        private void is_disabled(bool disable)
        {
            if (combo1 == true && combo2 == true)
            {
                if (disable || (comboBox1.SelectedIndex == comboBox2.SelectedIndex))
                    button2.Enabled = false;
                else
                    button2.Enabled = true;
            }
        }

        private void fill_controles ()
        {
            listBox1.Items.Add("Circle " + numberOf_circles + " X:" + mouse_position[numberOf_circles-1].X + " Y:" + mouse_position[numberOf_circles-1].Y);
            comboBox1.Items.Add("Circle " + numberOf_circles);
            comboBox2.Items.Add("Circle " + numberOf_circles);
        }

        private void check_sum()
        {
            for (int check = 0; check < connections.Count; check++)
            {
                if ((comboBox1.SelectedIndex == connections[check].X && comboBox2.SelectedIndex == connections[check].Y) 
                    || (comboBox1.SelectedIndex == connections[check].Y && comboBox2.SelectedIndex == connections[check].X))
                {
                    has_been_assign = true;
                    break;
                }
                else
                    has_been_assign = false;

            }
        }

        private void sort()
        {
            //sort each x & y for every point
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].X > connections[i].Y)
                {
                    connections[i] = new Point(connections[i].Y, connections[i].X);
                }
            }

            //sort every point
            int index = 0, swap_values = 0;
            Point minimum = new Point(connections[0].X, connections[0].Y);
            for (int i = 0; i < connections.Count; i++)
            {
                minimum = connections[i];
                swap_values = values[i];
                index = i;

                for (int j = i; j < connections.Count; j++)
                {
                    if (connections[j].X < minimum.X)
                    {
                        minimum = connections[j];
                        swap_values = values[j];
                        index = j;
                    }
                }
                connections[index] = connections[i];
                connections[i] = minimum;

                values[index] = values[i];
                values[i] = swap_values;
            }

            int number_of_connection = 0, iterate = 0;
            for (int i = 1; i < numberOf_circles; i++)
            {

                while (iterate < connections.Count)
                {
                    if (connections[iterate].Y == i)
                        number_of_connection++;
    
                    iterate++;
                }
                how_many_connections.Add(number_of_connection);
                number_of_connection = 0;
                iterate = 0;
                //MessageBox.Show(Convert.ToString(connections[i].X + 1) + " polaczone z " + Convert.ToString(connections[i].Y + 1));
            }
        }

        //HUGE funciont for drawing circle (1)
        private void draw_circle(Graphics circle, Pen pen, float x, float y, float r)
        {
            circle.DrawEllipse(pen, x - r - 12, y - r - 32, r * 2, r * 2);
        }

        //draw number in circle, cosmetics
        public void draw_string(Graphics line)
        {
            string number = Convert.ToString(numberOf_circles);
            float x, y;
            if (numberOf_circles < 10)
            {
                x = mouse_position[numberOf_circles - 1].X - 12 - radius / 2;
                y = mouse_position[numberOf_circles - 1].Y - 32 - radius / 2;
            }
            else
            {
                x = mouse_position[numberOf_circles - 1].X - 12 - (radius * 2) / 2;
                y = mouse_position[numberOf_circles - 1].Y - 32 - (radius * 2) / 2;
            }

            Font drawFont = new Font("Arial", radius);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            StringFormat drawFormat = new StringFormat();

            line.DrawString(number, drawFont, drawBrush, x, y, drawFormat);
            drawFont.Dispose();
            drawBrush.Dispose();
            line.Dispose();
        }

        //draw line copied from microsoft dot com
        public void draw_line(Graphics line, Pen pen, Point from, Point to)
        {
            string value = Convert.ToString(numericUpDown3.Value);

            Point draw_value = new Point(((from.X + to.X) - 12 * 2) / 2, ((from.Y + to.Y) - 32 * 2) / 2);
            Font drawFont = new Font("Arial", radius);
            SolidBrush drawBrush = new SolidBrush(Color.Black);
            StringFormat drawFormat = new StringFormat();

            values.Add(Convert.ToInt32(numericUpDown3.Value));
            connections.Add(new Point(comboBox1.SelectedIndex, comboBox2.SelectedIndex));

            from.X -= 12;
            from.Y -= 32;

            to.X -= 12;
            to.Y -= 32;

            line.DrawLine(pen, from, to);

            line.DrawString(value, drawFont, drawBrush, draw_value.X, draw_value.Y, drawFormat);
            drawFont.Dispose();
            drawBrush.Dispose();
            line.Dispose();
        }

        public Form1()
        {
            InitializeComponent();
        }      

        //better draw your own graph!
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            numberOf_circles++;

            mouse_position.Add(this.PointToClient(new Point(MousePosition.X, MousePosition.Y)));
            Graphics circle = pictureBox1.CreateGraphics();
            
            //(1)
            draw_circle(circle, pen, mouse_position[numberOf_circles-1].X, mouse_position[numberOf_circles-1].Y, radius);            
            fill_controles();
            draw_string(circle);

                        
            //MessageBox.Show(Convert.ToString("x: " + mouse_position.X + " y " + mouse_position.Y));
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            radius = (float)numericUpDown1.Value;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            radius = (float)numericUpDown1.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            {
                if (colorDialog1.ShowDialog() == DialogResult.OK)
                {
                    pen = new Pen(colorDialog1.Color, (float)numericUpDown2.Value);
                }
            }
        }

        //draw connections from point to point
        private void button2_Click(object sender, EventArgs e)
        {
            Graphics line = pictureBox1.CreateGraphics();
            draw_line(line, pen, mouse_position[comboBox1.SelectedIndex], mouse_position[comboBox2.SelectedIndex]);

            //control_sum.Add(comboBox1.SelectedIndex + comboBox2.SelectedIndex);
            check_sum();

            is_disabled(has_been_assign);

            if (connections.Count > 2 && !was_pressed)
                button4.Enabled = true;
        }

        //anti-terrorist events
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label5.Text = "Circle " + (comboBox1.SelectedIndex + 1);

            combo1 = true;
            check_sum();                        
            is_disabled(has_been_assign);
        }

        //anti-terrorist events
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            label6.Text = "Circle " + (comboBox2.SelectedIndex + 1);

            combo2 = true;
            check_sum();
            is_disabled(has_been_assign);
        }

        //resets the app
        private void button3_Click(object sender, EventArgs e)
        {
            Application.Restart();
        }
        
        //start the algorithm
        private void button4_Click(object sender, EventArgs e)
        {
            was_pressed = true;
            button4.Enabled = false;

            sort();

            int iterate = 0;

            List<Point> dijkstra_connections = new List<Point>();
            int i = 0, value = 0;

            for (i = 0; i < connections.Count; i++)
            {
                if (connections[i].X != 0)
                {
                    value += dijkstra_connections[i].X;
                 //   j--;
                }
                else
                {
                    value += values[i];
                    dijkstra_connections.Add(new Point(value, connections[i].Y));
                }
            }
 
            //for (int i = 0; i < how_many_connections.Count; i++)
            //    MessageBox.Show(Convert.ToString(how_many_connections[i]));
            for (i = 0; i < values_summary.Count; i++)
            {
                MessageBox.Show("Wartosc najkrotszego polaczenia to: " + Convert.ToString(values_summary[i]));
            }

            for (i = 0; i < how_many_connections.Count; i++)
            {
                MessageBox.Show("Wartosc najkrotszego polaczenia to: " + Convert.ToString(how_many_connections[i]));
            }
            // MessageBox.Show(Convert.ToString(values_summary[0]));
        }
    }
}
