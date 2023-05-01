using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime;

namespace CourseWork
{
    public partial class SimplexMethodSolution : Form
    {
        public SimplexMethodSolution()
        {
            InitializeComponent();
        }

        List<List<double>> solution = new List<List<double>>();

        bool IsOptimum(List<double> functionValues, int cols)
        {
            for (int i = 0; i < cols; i++)
            {
                double num = functionValues[i];
                if (Math.Round(num, 6) > 0) 
                {
                    return false;
                }
            }
            return true;
        }


        int FindRowIndex(List<List<double>> limitValues, List<double> limitFreeValues, int rows, int colIndex)
        {
            int rowIndex = -1;
            for(int i = 0; i < rows; i++)
            {
                if(limitValues[i][colIndex] > 0)
                {
                    rowIndex = i;
                    break;
                }
            }

            if (rowIndex != rows - 1 && rowIndex != -1)
            {
                for (int i = rowIndex + 1; i < rows; i++)
                {
                    if (limitValues[i][colIndex] > 0 && ((limitFreeValues[i] / limitValues[i][colIndex]) < (limitFreeValues[rowIndex] / limitValues[rowIndex][colIndex])))
                    {
                        rowIndex = i;
                    }
                }
            }

            return rowIndex;
        }


        List<List<double>> MakeSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> basicValuesIndexes, double ResultFunctionF, bool min)
        {
            List<double> col = new List<double>();
            for (int i = 0; i < rows; i++)
            {
                col.Add(basicValuesIndexes[i] + 1);
            }
            col.Add(0);
            solution.Add(col);

            List<double> col1 = new List<double>();

            col1.AddRange(limitFreeValues);
            if (min)
                col1.Add(-ResultFunctionF);
            else
                col1.Add(ResultFunctionF);
            solution.Add(col1);

            for (int i = 0; i < cols; i++)
            {
                List<double> col2 = new List<double>();
                for (int j = 0; j < rows; j++)
                {
                    col2.Add(limitValues[j][i]);
                }
                if (min)
                    col2.Add(-functionValues[i]);
                else
                    col2.Add(functionValues[i]);
                solution.Add(col2);
            }

            return solution;
        }


        List<List<double>> SimplexSolveF(int cols, int rows,
            List<double> functionValues, List<List<double>> limitValues, List<double> limitFreeValues, List<int> basicValuesIndexes,
            double ResultFunctionF, int colsBeforeAdding, bool min)
        {
            double resolvingElement;
            int colIndex;
            int rowIndex;
            bool optimum = IsOptimum(functionValues, colsBeforeAdding);

            while (!optimum)
            {
                colIndex = functionValues.IndexOf(functionValues.Max());

                rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);
                if (rowIndex == -1)
                    return solution;

                resolvingElement = limitValues[rowIndex][colIndex];
                basicValuesIndexes[rowIndex] = colIndex;

                //пересчитываем значение функции f
                ResultFunctionF -= limitFreeValues[rowIndex] * functionValues[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++)
                {
                    if (i != colIndex)
                        functionValues[i] -= limitValues[rowIndex][i] * functionValues[colIndex] / resolvingElement;
                }
                functionValues[colIndex] = 0;


                //пересчитываем значения свободных членов
                for (int i = 0; i < rows; i++)
                {
                    if (i != rowIndex)
                        limitFreeValues[i] -= limitValues[i][colIndex] * limitFreeValues[rowIndex] / resolvingElement;
                }
                limitFreeValues[rowIndex] /= resolvingElement;


                //пересчитываем симплекс-таблицу
                List<List<double>> solvingMatrix = new List<List<double>>();

                for (int i = 0; i < cols; i++)
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }
                for (int i = 0; i < rows; i++)
                {
                    List<double> rowSolution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex)
                        {
                            rowSolution.Add(limitValues[i][j]);
                        }
                        else
                            rowSolution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j]);
                    }
                    solvingMatrix.Add(rowSolution);
                }

                limitValues.Clear();
                for (int i = 0; i < rows; i++)
                {
                    List<double> iteration = new List<double>();
                    iteration.AddRange(solvingMatrix[i]);
                    limitValues.Add(iteration);
                }
                solvingMatrix.Clear();

                //проверяем план на оптимальность
                optimum = IsOptimum(functionValues, colsBeforeAdding);
            }

            return MakeSolution(colsBeforeAdding, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, min);
        }

        List<List<double>> NoIOR(int cols, int rows)
        {
            for(int i = 0; i < rows; i++)
            {
                List<double> iteration = new List<double>();
                for (int j = 0; j < cols; j++)
                {
                    iteration.Add(0);
                }
                solution.Add(iteration);
            }
            return solution;
        }


        List<List<double>> SimplexFindIOR(int cols, int rows, 
            List<double> functionValues, List<List<double>> limitValues, List<double> limitFreeValues, List<int> basicValuesIndexes, List<double> functionT,
            double ResultFunctionT, int colsBeforeAdding, bool min)
        {
            double ResultFunctionF = 0;
            double resolvingElement;
            int colIndex;
            int rowIndex;
            bool optimum = false;

            while (!optimum)
            {
                colIndex = functionT.IndexOf(functionT.Max());

                rowIndex = FindRowIndex(limitValues, limitFreeValues, rows, colIndex);
                if (rowIndex == -1)
                    return solution = NoIOR(cols, rows);

                resolvingElement = limitValues[rowIndex][colIndex];
                basicValuesIndexes[rowIndex] = colIndex;

                //пересчитываем значение функции T
                ResultFunctionT -= limitFreeValues[rowIndex] * functionT[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++)
                {
                    if (i != colIndex)
                        functionT[i] -= limitValues[rowIndex][i] * functionT[colIndex] / resolvingElement;
                }
                functionT[colIndex] = 0;


                //пересчитываем значение функции f
                ResultFunctionF -= limitFreeValues[rowIndex] * functionValues[colIndex] / resolvingElement;
                for (int i = 0; i < cols; i++)
                {
                    if (i != colIndex)
                        functionValues[i] -= limitValues[rowIndex][i] * functionValues[colIndex] / resolvingElement;
                }
                functionValues[colIndex] = 0;


                //пересчитываем значения свободных членов
                for (int i = 0; i < rows; i++)
                {
                    if (i != rowIndex)
                        limitFreeValues[i] -= limitValues[i][colIndex] * limitFreeValues[rowIndex] / resolvingElement;
                }
                limitFreeValues[rowIndex] /= resolvingElement;


                //пересчитываем симплекс-таблицу
                List<List<double>> solvingMatrix = new List<List<double>>();

                for (int i = 0; i < cols; i++)
                {
                    limitValues[rowIndex][i] /= resolvingElement;
                }
                for (int i = 0; i < rows; i++)
                {
                    List<double> rowSolution = new List<double>();
                    for (int j = 0; j < cols; j++)
                    {
                        if (i == rowIndex)
                        {
                            rowSolution.Add(limitValues[i][j]);
                        }
                        else
                            rowSolution.Add(limitValues[i][j] - limitValues[i][colIndex] * limitValues[rowIndex][j]);
                    }
                    solvingMatrix.Add(rowSolution);
                }

                limitValues.Clear();
                for (int i = 0; i < rows; i++)
                {
                    List<double> iteration = new List<double>();
                    iteration.AddRange(solvingMatrix[i]);
                    limitValues.Add(iteration);
                }
                solvingMatrix.Clear();

                //проверяем план на оптимальность
                optimum = IsOptimum(functionT, colsBeforeAdding);
            }

            for(int i = 0; i < rows; i++)
            {
                if (basicValuesIndexes[i] >= colsBeforeAdding)
                    return solution = NoIOR(cols, rows);
            }

            solution = SimplexSolveF(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, ResultFunctionF, colsBeforeAdding, min);

            return solution;
        }


        List<List<double>> SimplexMethod(int cols, int rows, List<double> functionValues, List<List<double>> limitValues,
            List<double> limitFreeValues, List<int> sign, bool min)
        {
            List<int> basicValuesIndexes = new List<int>();
            List<double> functionT = new List<double>();

            double ResultFunctionT = 0;
            int colsBeforeAdding;

            //если есть отрицательные b
            for (int i = 0; i < rows; i++)
            {
                if (limitFreeValues[i] < 0)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        limitValues[i][j] *= (-1);
                    }
                    limitFreeValues[i] *= (-1);
                    if (sign[i] == 0)
                        sign[i] = 2;
                    else if (sign[i] == 2)
                        sign[i] = 0;
                }
            }

            //добавление доп переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                if (sign[i] == 0) // ">="
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (j == i)
                            limitValues[j].Add(-1);
                        else
                            limitValues[j].Add(0);
                    }
                    functionValues.Add(0);
                    cols++;
                }
            }

            //добавление доп переменных, если есть неравенства
            for (int i = 0; i < rows; i++)
            {
                if (sign[i] == 2) // "<="
                {
                    for (int j = 0; j < rows; j++)
                    {
                        if (j == i)
                            limitValues[j].Add(1);
                        else
                            limitValues[j].Add(0);
                    }
                    functionValues.Add(0);
                    cols++;
                }
            }

            colsBeforeAdding = cols;

            //добавление доп переменных для двухэтапного метода
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j == i)
                        limitValues[i].Add(1);
                    else
                        limitValues[i].Add(0);
                }
                basicValuesIndexes.Add(cols);  //индексы базисных переменных
                functionValues.Add(0);
                cols++;
            }


            for (int j = 0; j < colsBeforeAdding; j++) //функция T
            {
                double sum = 0;
                for (int i = 0; i < rows; i++)
                {
                    sum += limitValues[i][j];
                }
                functionT.Add(sum);
            }
            for (int j = colsBeforeAdding; j < cols; j++)
            {
                functionT.Add(0);
            }
            for (int i = 0; i < rows; i++)
            {
                ResultFunctionT += limitFreeValues[i];
            }

            if (!min)
            {
                for (int i = 0; i < cols; i++) 
                {
                    functionValues[i] *= -1;
                }
            }

            return SimplexFindIOR(cols, rows, functionValues, limitValues, limitFreeValues, basicValuesIndexes, functionT, ResultFunctionT, colsBeforeAdding, min);
        }



        internal SimplexMethodSolution(int cols, int rows, List<double> functionValues, List<List<double>> limitValues, 
            List<double> limitFreeValues, List<int> sign, bool min) 
        {
            this.AutoSize = true;
            this.Text = "Решение";
            this.StartPosition = FormStartPosition.CenterScreen;

            int num = 0;

            RichTextBox richTextBox1 = new RichTextBox
            {
                Location = new Point(10, 10),
                Font = new Font("Microsoft Sans Serif", 11),
                ReadOnly = true
            };
            richTextBox1.ContentsResized += (object sender, ContentsResizedEventArgs e) =>
            {
                var richTextBox = (RichTextBox)sender;
                richTextBox.Height = e.NewRectangle.Height;
            };
            richTextBox1.WordWrap = false;
            richTextBox1.ScrollBars = RichTextBoxScrollBars.None;


            richTextBox1.Text += "Целевая функция: \r\n";
            for (int j = 0; j < cols; j++)
            {
                num = j;
                if (functionValues[j] == 0)
                {
                    if (j != cols - 1)
                    {
                        if (functionValues[j + 1] > 0)
                            richTextBox1.Text += "+ ";
                    }
                }
                else
                {
                    if (functionValues[j] == 1)
                        richTextBox1.Text += "x" + ++num + " ";
                    else if (functionValues[j] == -1)
                        richTextBox1.Text += "-x" + ++num + " ";
                    else
                        richTextBox1.Text += functionValues[j].ToString() + "x" + ++num + " ";
                    if (j != cols - 1)
                        if (functionValues[j + 1] > 0)
                            richTextBox1.Text += "+ ";
                }
            }
            if (min) 
                richTextBox1.Text += " --> max \r\n";
            else 
                richTextBox1.Text += " --> min \r\n";


            richTextBox1.Text += "\r\n";
            richTextBox1.Text += "Ограничения: \r\n";
            num = 0;
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    num = j;
                    if (limitValues[i][j] == 0)
                    {
                        if(j != cols - 1)
                        {
                            if (limitValues[i][j + 1] > 0)
                                richTextBox1.Text += "+ ";
                        }
                    }
                    else
                    {
                        if(limitValues[i][j] == 1)
                            richTextBox1.Text += "x" + ++num + " ";
                        else if (limitValues[i][j] == -1)
                            richTextBox1.Text += "-x" + ++num + " ";
                        else
                            richTextBox1.Text += limitValues[i][j].ToString() + "x" + ++num + " ";
                        if (j != cols - 1)
                            if(limitValues[i][j + 1] > 0)
                                richTextBox1.Text += "+ ";
                    }
                }
                if(sign[i] == 0)
                    richTextBox1.Text += " >= ";
                else if(sign[i] == 1)
                    richTextBox1.Text += " = ";
                else richTextBox1.Text += " <= ";
                
                richTextBox1.Text += limitFreeValues[i].ToString() + "\r\n";
            }
            
            richTextBox1.Text += "\r\n";
            richTextBox1.Text += "РЕШЕНИЕ: \r\n";
            solution = SimplexMethod(cols, rows, functionValues, limitValues, limitFreeValues, sign, min);

            if (!solution.Any())
            {
                richTextBox1.Text += "\r\n";
                richTextBox1.Text += "Задача не имеет оптимальных решений. \r\n";
                richTextBox1.Width = 450;
                this.Controls.Add(richTextBox1);
            }
            else
            {
                cols = solution[0].Count();
                rows = solution.Count();
                bool flag = true;

                for(int i = 0; i < rows; i++)
                {
                    for(int j = 0; j < cols; j++)
                    {
                        if (solution[i][j] != 0)
                        {
                            flag = false;
                            break;
                        }                            
                    }
                }

                if (flag)
                {
                    richTextBox1.Text += "\r\n";
                    richTextBox1.Text += "Задача не имеет ИОР. Решений нет. \r\n";
                    richTextBox1.Width = 450;
                    this.Controls.Add(richTextBox1);
                }
                else
                {
                    DataGridView DGV = new DataGridView
                    {
                        Font = new Font("Microsoft Sans Serif", 11),
                        Location = new Point(10, richTextBox1.Height + 140),
                        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                        AutoSize = true,
                        ReadOnly = true,

                        ColumnCount = rows,
                        RowCount = cols
                    };

                    DGV.Columns[0].HeaderText = "Базис";
                    DGV.Columns[1].HeaderText = "Св. чл.";
                    int k = 1;
                    for (int i = 2; i < rows; i++)
                    {
                        DGV.Columns[i].HeaderText = "X" + k.ToString();
                        k++;

                    }

                    for (int i = 0; i < cols; i++)
                    {
                        for (int j = 0; j < rows; j++)
                        {
                            if (i < cols - 1 && j == 0)
                            {
                                DGV.Rows[i].Cells[j].Value = "X" + Math.Round(solution[j][i], 3).ToString();
                            }
                            else if (i == cols - 1 && j == 0)
                            {
                                DGV.Rows[i].Cells[j].Value = "F";
                            }
                            else
                            {
                                DGV.Rows[i].Cells[j].Value = Math.Round(solution[j][i], 2).ToString();
                            }
                        }
                    }

                    int dgv_width = DGV.Columns.GetColumnsWidth(DataGridViewElementStates.Visible);
                    int dgv_height = DGV.Rows.GetRowsHeight(DataGridViewElementStates.Visible);
                    richTextBox1.Width = dgv_width / 2 + 100;

                    Button intSolutionButton = new Button
                    {
                        Location = new Point(10, richTextBox1.Height + dgv_height + 250),
                        Size = new Size(190, 50),
                        Name = "ButtonCount",
                        Font = new Font("Microsoft Sans Serif", 11),
                        Text = "Найти целочисленное решение"
                    };
                    this.Controls.Add(intSolutionButton);


                    intSolutionButton.Click += (sender, eventArgs) =>
                    { 
                    
                    };

                    this.Controls.Add(richTextBox1);
                    this.Controls.Add(DGV);
                    this.Controls.Add(intSolutionButton);

                }                
            }
        }

    }
}
