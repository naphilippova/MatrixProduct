using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace MatrixProduct
{
    public partial class Form1 : Form
    {
        // Текущая размерность матриц
        int p, q;
        // m1, m2 - матрицы чисел, m3 - матрица результат умножения m1 на m2
        double[,] m1, m2, m3;
        //
        DataGridView dataGridView1;
        // Объект класса формы Form2
        Form Form2 = null;
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox1.Text, "[^0-9]"))
            {
                MessageBox.Show("Введите целое число от 0 до 99...");
                textBox1.Text = textBox1.Text.Remove(textBox1.Text.Length - 1);
            }
            if ((textBox1.Text == "") || (Int16.Parse(textBox1.Text) >= 100))
            {
                MessageBox.Show("Введите целое число от 2 до 99...");
                textBox1.Text = "2";
            }
            textBox1.SelectionStart = textBox1.Text.Length;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(textBox2.Text, "[^0-9]"))
            {
                MessageBox.Show("Введите целое число от 0 до 99...");
                textBox2.Text = textBox2.Text.Remove(textBox2.Text.Length - 1);
            }
            if ((textBox2.Text == "") || (Int16.Parse(textBox2.Text) >= 100))
            {
                MessageBox.Show("Введите целое число от 2 до 99...");
                textBox2.Text = "2";
            }
            textBox2.SelectionStart = textBox2.Text.Length;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            // Чтение размерности матрицы
            if ((textBox1.Text == "") && (textBox2.Text == ""))
                return;
            p = int.Parse(textBox1.Text);
            q = int.Parse(textBox2.Text);
            m1 = new double[p, q];
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Init(123456789, m1);
            sw.Stop();
            // Данные в матрицу m1 внесены
            button1.Text = "Матрица 1 инициализирована";
            // Запись в файл
            Save("m1.txt", p, q, m1, sw.ElapsedMilliseconds,"Матрица 1");
        }
        private void button2_Click(object sender, EventArgs e)
        {
            // Чтение размерности матрицы
            if ((textBox1.Text == "") && (textBox2.Text == ""))
                return;
            p = int.Parse(textBox1.Text);
            q = int.Parse(textBox2.Text);
            m2= new double[q, p];
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Init(987654321, m2);
            sw.Stop();
            // Данные в матрицу m2 внесены
            button2.Text = "Матрица 2 инициализирована";
            // Запись в файл
            Save("m2.txt", p, q, m2, sw.ElapsedMilliseconds, "Матрица 2");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            m3 = new double[m1.GetLength(0), m2.GetLength(1)];
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Вычисление произведения матриц последовательно. Результат в m3
            for (int r = 0; r < m1.GetLength(0); r++)
                for (int c = 0; c < m2.GetLength(1); c++)
                {
                    m3[r, c] = 0;
                    for (int k = 0; k < m1.GetLength(1); k++)
                        m3[r, c] += m1[r, k] * m2[k, c];
                }
            sw.Stop();
            button3.Text = "Последовательное умножение, время счёта: " + sw.ElapsedMilliseconds + " мc";
            // Запись в файл
            Save("m3_sequentially.txt", p, q, m3, sw.ElapsedMilliseconds, "Результат уножения матрицы 1 на матрицу 2");
        }
        private void button4_Click(object sender, EventArgs e)
        {
            m3 = new double[m1.GetLength(0), m2.GetLength(1)];
            Stopwatch sw = new Stopwatch();
            sw.Start();
            // Вычисление произведения матриц параллельно. Результат в m3
            Parallel.For(0, m1.GetLength(0), r =>
            {
                Parallel.For(0, m2.GetLength(1), c =>
                {
                    m3[r, c] = 0;
                    for (int k = 0; k < m1.GetLength(1); k++)
                        m3[r, c] += m1[r, k] * m2[k, c];
                });
            });
            sw.Stop();
            button4.Text = "Параллельное умножение, время счёта: " + sw.ElapsedMilliseconds + " мc";
            // Запись в файл
            Save("m3_parallel.txt", p, q, m3, sw.ElapsedMilliseconds, "Результат уножения матрицы 1 на матрицу 2");
        }
        private void Init(int seed, double[,] m)
        {
            var rnd = new Random(seed);
            for (int r = 0; r < m.GetLength(0); r++)
                for (int c = 0; c < m.GetLength(1); c++)
                    m[r, c] = rnd.Next(0, m.GetLength(0) * m.GetLength(1));
        }
        private void Save(string fn, int p, int q, double[,] m, double t, string title)
        {
            // Запись в файл
            FileStream fw = null;
            // Строковая переменная
            string msg;
            // Байтовый массив
            byte[] msgByte = null;
            // Открыть файл с именем fn для записи
            fw = new FileStream(fn, FileMode.Create);
            // Запись матрицы результата в файл, сначала записать число элементов матрицы
            msg = "Размерность матрицы: " + p.ToString() + " на " + q.ToString() + "\r\n";
            // Перевод строки msg в байтовый массив msgByte
            msgByte = Encoding.Default.GetBytes(msg);
            // запись массива msgByte в файл
            fw.Write(msgByte, 0, msgByte.Length);
            // Время расчёта
            msg = "Время инициализации матрицы: " + t.ToString() + " мс\r\n";
            // Перевод строки msg в байтовый массив msgByte
            msgByte = Encoding.Default.GetBytes(msg);
            // Запись массива msgByte в файл
            fw.Write(msgByte, 0, msgByte.Length);
            // Теперь записать саму матрицу
            msg = "";
            for (int r = 0; r < m.GetLength(0); r++)
            {
                // Формируем строку msg из элементов матрицы
                for (int c = 0; c < m.GetLength(1); c++)
                    msg += m[r, c].ToString() + "  ";
                // Добавить перевод строки
                msg += "\r\n";
            }
            // Перевод строки msg в байтовый массив msgByte
            msgByte = Encoding.Default.GetBytes(msg);
            // Запись строк матрицы в файл
            fw.Write(msgByte, 0, msgByte.Length);
            // Закрыть файл
            if (fw != null)
                fw.Close();
            // Представление марицы m в виде DataGridView
            dataGridView1 = new DataGridView();
            // Задаём кол-во колонок в dataGridView1
            dataGridView1.ColumnCount = m.GetLength(1);
            for (int r = 0; r < m.GetLength(0); r++)
            {
                // Создаём строки в dataGridView1
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridView1);
                // Заполняем dataGridView1
                for (int c = 0; c < m.GetLength(1); c++)
                    row.Cells[c].Value = m[r, c];
                // Добавлем строку в dataGridView1
                dataGridView1.Rows.Add(row);
            }
            // Пусть dataGridView1 будет во весь размер формы
            dataGridView1.Dock = DockStyle.Fill;
            // Пусть не будет верт. и гориз. полос прокрутки и ячейки нельзя редактировать
            dataGridView1.ColumnHeadersVisible = dataGridView1.RowHeadersVisible = dataGridView1.Enabled = false;
            // Сздаём новое окно Form2
            Form2 = new Form();
            // Задаём размеры клиенской области Form2 равнами размерм dataGridView1
            Form2.ClientSize = new System.Drawing.Size(dataGridView1.Columns.GetColumnsWidth(DataGridViewElementStates.None), dataGridView1.Rows.GetRowsHeight(DataGridViewElementStates.None));
            // Меняе заголовок окна Form2
            Form2.Text = title;
            // Form2 нельзя свернуть или развернуть
            Form2.MaximizeBox = Form2.MinimizeBox = false;
            // Добавляем dataGridView1 на Form2
            Form2.Controls.Add(dataGridView1);
            // Скрываем основную форму Form1
            this.Hide();
            if (Form2.ShowDialog() == DialogResult.Cancel)
            {
                // Если Form2 закрыли покзываем основную форму Form1
                this.Show();
            }
        }
    }
}
