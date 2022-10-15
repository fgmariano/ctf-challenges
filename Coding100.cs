using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reply.CyberSecurityChallenge2022
{
    public class Coding100
    {
        private List<GridItem> Grid = new List<GridItem>();
        private List<string> Words = new List<string>();

        public Coding100()
        {
            var rawGrid = File.ReadAllLines(@"C:\Users\f.mariano\source\repos\Reply.CyberSecurityChallenge2022\grid_v1.txt");
            int rowIndex = 0;
            foreach (var row in rawGrid)
            {
                var cols = row.Split(' ');
                int colIndex = 0;
                foreach (var col in cols)
                {
                    if (string.IsNullOrEmpty(col))
                        continue;

                    Grid.Add(new GridItem()
                    {
                        Id = Guid.NewGuid(),
                        rowPos = rowIndex,
                        colPos = colIndex,
                        Value = col.Trim(),
                        IsUsed = col.Trim() == "-" ? true : false
                    });
                    colIndex++;
                }
                rowIndex++;
            }
            Words = File.ReadAllLines(@"C:\Users\f.mariano\source\repos\Reply.CyberSecurityChallenge2022\words.txt").ToList();
        }

        private GridItem? HasSequentialItem(GridItem item, bool isHorizontal = false, bool isBackwards = false, bool isDiagonal = false, bool leftToRight = false)
        {
            GridItem? itemFound = null;
            if (!isBackwards)
            {
                if (isHorizontal)
                {
                    var col = item.colPos + 1;
                    itemFound = Grid.FirstOrDefault(_ => _.rowPos == item.rowPos && _.colPos == col && !_.IsUsed);
                }
                if (isDiagonal)
                {
                    if (leftToRight)
                    {
                        var col = item.colPos + 1;
                        var row = item.rowPos + 1;
                        itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == col && !_.IsUsed);
                    }
                    else
                    {
                        var col = item.colPos - 1;
                        var row = item.rowPos + 1;
                        itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == col && !_.IsUsed);
                    }
                }
                else
                {
                    var row = item.rowPos + 1;
                    itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == item.colPos && !_.IsUsed);
                }
            }
            else
            {
                if (isHorizontal)
                {
                    var col = item.colPos - 1;
                    itemFound = Grid.FirstOrDefault(_ => _.rowPos == item.rowPos && _.colPos == col && !_.IsUsed);
                }
                if (isDiagonal)
                {
                    if (!leftToRight)
                    {
                        var col = item.colPos - 1;
                        var row = item.rowPos - 1;
                        itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == col && !_.IsUsed);
                    }
                    else
                    {
                        var col = item.colPos + 1;
                        var row = item.rowPos - 1;
                        itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == col && !_.IsUsed);
                    }
                }
                else
                {
                    var row = item.rowPos - 1;
                    itemFound = Grid.FirstOrDefault(_ => _.rowPos == row && _.colPos == item.colPos && !_.IsUsed);
                }
            }

            return itemFound;
        }

        private string GetWordFromGridList(List<GridItem> value)
        {
            return string.Join("", value.Select(_ => _.Value));
        }

        private bool CheckIfItsAWord(List<GridItem> PossibleWord)
        {
            var strPossibleWord = GetWordFromGridList(PossibleWord);
            return Words.Contains(strPossibleWord);
        }

        private void SaveCurrentGridToFile(List<GridItem> grid)
        {
            var sb = new StringBuilder();
            for (int rowIndex = 0; rowIndex <= grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = 0; colIndex <= grid.Select(_ => _.colPos).Max(); colIndex++)
                {
                    var item = grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (item.IsUsed)
                        sb.Append("- ");
                    else
                        sb.Append($"{item.Value} ");
                }
                sb.Append("\n");
            }

            File.WriteAllText(@"C:\Users\f.mariano\source\repos\Reply.CyberSecurityChallenge2022\grid_v1.txt", sb.ToString());
        }

        public void Run()
        {
            L_shaped_non_backwards_top_to_bottom_left_to_right();
        }

        public void L_shaped_non_backwards_top_to_bottom_left_to_right()
        {
            for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    Search_horizontal_non_backwards(possibleWord, pivotalItem);
                }
            }
        }

        private void Search_horizontal_non_backwards(List<GridItem> possibleWord, GridItem pivotalItem)
        {
            while (true)
            {
                int newRow = pivotalItem.rowPos + 1;
                pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == newRow && _.colPos == pivotalItem.colPos);
                if (pivotalItem == null || pivotalItem.IsUsed)
                    break;

                pivotalItem = HasSequentialItem(pivotalItem, true, false, false, true);
                if (pivotalItem == null || pivotalItem.IsUsed)
                    break;

                possibleWord.Add(pivotalItem);
                var teste = GetWordFromGridList(possibleWord);
                if (CheckIfItsAWord(possibleWord))
                {
                    Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                    //Console.ReadLine();
                    foreach (var item in possibleWord)
                    {
                        Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                    }
                    SaveCurrentGridToFile(Grid);
                    possibleWord.Clear();
                }
            }
        }

        public void Diagonal_Non_Backwards_Right_to_Left()
        {
            for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, false, true, false);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        var teste = GetWordFromGridList(possibleWord);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Diagonal_Backwards_Left_to_Right()
        {
            for (int rowIndex = Grid.Select(_ => _.rowPos).Max(); rowIndex >= 0; rowIndex--)
            {
                for (int colIndex = Grid.Select(_ => _.colPos).Max(); colIndex >= 0; colIndex--)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, true, true, true);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        var teste = GetWordFromGridList(possibleWord);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Diagonal_Backwards_Right_to_Left()
        {
            for (int rowIndex = Grid.Select(_ => _.rowPos).Max(); rowIndex >= 0; rowIndex--)
            {
                for (int colIndex = Grid.Select(_ => _.colPos).Max(); colIndex >= 0; colIndex--)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, true, true, false);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        var teste = GetWordFromGridList(possibleWord);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Diagonal_Non_Backwards_Left_to_Right()
        {
            for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, false, true);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        var teste = GetWordFromGridList(possibleWord);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Vertical_Backwards()
        {
            for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
            {
                for (int rowIndex = Grid.Select(_ => _.rowPos).Max(); rowIndex >= 0; rowIndex--)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, true);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Vertical_Non_Backwards()
        {
            for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
            {
                for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, false, false);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Horizontal_Backwards()
        {
            for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = Grid.Select(_ => _.colPos).Max(); colIndex >= 0; colIndex--)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, true, true);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            //Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }

        public void Horizontal_Non_Backwards()
        {
            for (int rowIndex = 0; rowIndex <= Grid.Select(_ => _.rowPos).Max(); rowIndex++)
            {
                for (int colIndex = 0; colIndex <= Grid.Select(_ => _.colPos).Max(); colIndex++)
                {
                    var pivotalItem = Grid.FirstOrDefault(_ => _.rowPos == rowIndex && _.colPos == colIndex);
                    if (pivotalItem.IsUsed)
                        continue;

                    var possibleWord = new List<GridItem>();
                    possibleWord.Add(pivotalItem);
                    while (true)
                    {
                        pivotalItem = HasSequentialItem(pivotalItem, true, false);
                        if (pivotalItem == null || pivotalItem.IsUsed)
                            break;

                        possibleWord.Add(pivotalItem);
                        if (CheckIfItsAWord(possibleWord))
                        {
                            Console.WriteLine("Word found : " + GetWordFromGridList(possibleWord));
                            Console.ReadLine();
                            foreach (var item in possibleWord)
                            {
                                Grid.FirstOrDefault(_ => _.Id == item.Id).IsUsed = true;
                            }
                            SaveCurrentGridToFile(Grid);
                            possibleWord.Clear();
                        }
                    }
                }
            }
        }
    }

    internal class GridItem
    {
        public Guid Id { get; set; }
        public int rowPos { get; set; }
        public int colPos { get; set; }
        public string Value { get; set; }
        public bool IsUsed { get; set; }
    }
}
