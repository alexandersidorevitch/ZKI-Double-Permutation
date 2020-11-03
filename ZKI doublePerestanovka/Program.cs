using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ZKI_doublePerestanovka
{

    /// <summary>
    /// Класс который содержит символ и его порядковый номер в строке, зависящий от алфавита.
    /// </summary>
    class CharNum
    {
        #region Fields
        /// <summary>
        /// Символ.
        /// </summary>
        private char _ch;
        /// <summary>
        /// Порядковый номер зависящий от алфавита.
        /// </summary>
        private int _numberInWord;
        #endregion Fieds

        #region Properties
        /// <summary>
        /// Символ.
        /// </summary>
        public char Ch
        {
            get { return _ch; }
            set
            {
                if (_ch == value)
                    return;
                _ch = value;
            }
        }
        /// <summary>
        /// Порядковый номер в строке, зависящий от алфавита.
        /// </summary>
        public int NumberInWord
        {
            get { return _numberInWord; }
            set
            {
                if (_numberInWord == value)
                    return;
                _numberInWord = value;
            }
        }
        #endregion Properties
    }

    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("1. Зашифровать");
                Console.WriteLine("2. Расшифровать");
                var selected = Console.ReadLine();
                switch (selected)
                {
                    case "1":
                        {
                            // Первый ключ, количество столбцов
                            Console.WriteLine("Введите первый ключ");
                            string firstKey = Console.ReadLine();

                            // Второй ключ, количество строк
                            Console.WriteLine("Введите второй ключ");
                            string secondKey = Console.ReadLine();

                            // Предложение которое шифруем


                            Console.WriteLine("Введите текст который вы хотите зашифровать");
                            string decoded = MatrixToString(Encode(Console.ReadLine(), firstKey, secondKey));
                            Console.WriteLine(decoded);
                            Console.WriteLine("Сохранить в файл?");
                            selected = Console.ReadLine().ToLower();
                            switch (selected)
                            {
                                case "да":
                                    {
                                        Console.WriteLine("Введите название");
                                        WriteToFile(decoded, $"{Console.ReadLine()}.txt");
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    case "2":
                        {
                            Console.WriteLine("1. Из файла");
                            Console.WriteLine("2. Из консольки");
                            selected = Console.ReadLine().ToLower();
                            switch (selected)
                            {
                                case "1":
                                    {
                                        Console.WriteLine("Введите название файла");
                                        var path = $"{Console.ReadLine()}.txt";

                                        // Первый ключ, количество столбцов
                                        Console.WriteLine("Введите первый ключ");
                                        string firstKey = Console.ReadLine();

                                        // Второй ключ, количество строк
                                        Console.WriteLine("Введите второй ключ");
                                        string secondKey = Console.ReadLine();

                                        Console.WriteLine(MatrixToString(Decode(ReadFromFile(path), firstKey, secondKey)));
                                        break;
                                    }
                                case "2":
                                    {
                                        Console.WriteLine("Введите зашифрованный текст");
                                        var text = Console.ReadLine();

                                        // Первый ключ, количество столбцов
                                        Console.WriteLine("Введите первый ключ");
                                        string firstKey = Console.ReadLine();

                                        // Второй ключ, количество строк
                                        Console.WriteLine("Введите второй ключ");
                                        string secondKey = Console.ReadLine();

                                        Console.WriteLine(MatrixToString(Decode(text, firstKey, secondKey)));
                                        break;
                                    }
                                default:
                                    break;
                            }
                            break;
                        }
                    default:
                        break;
                }
            }



            Console.ReadKey();
        }

        #region Methods
        /// <summary>
        /// Возвращает порядковый номер символа по алфавиту.
        /// </summary>
        /// <param name="s">Символ, чей порядковый номер, необходимо узнать.</param>
        /// <returns></returns>
        public static char[,] Decode(string text, string xkeyText, string ykeyText)
        {
            var xkey = FillListKey(xkeyText.ToCharArray());
            var ykey = FillListKey(ykeyText.ToCharArray());



            xkey = FillingSerialsNumber(xkey);
            ykey = FillingSerialsNumber(ykey);


            var matrix = new char[xkey.Count, ykey.Count];
            var countSymbols = 0;

            var textMatrix = StringToMatrix(text, xkey.Count, ykey.Count);
            for (int i = 0; i < xkey.Count; i++)
            {
                for (int j = 0; j < ykey.Count && countSymbols < text.Length; j++)
                {
                    matrix[i, j] = textMatrix[xkey[i].NumberInWord, ykey[j].NumberInWord];
                }
            }

            return matrix;



        }
        public static char[,] Encode(string text, string xkeyText, string ykeyText)
        {
            var xkey = FillListKey(xkeyText.ToCharArray());
            var ykey = FillListKey(ykeyText.ToCharArray());



            xkey = FillingSerialsNumber(xkey);
            ykey = FillingSerialsNumber(ykey);

            ShowKey(xkey, "Первый ключ: ");
            ShowKey(ykey, "Второй ключ: ");

            var matrixText = new char[xkey.Count, ykey.Count];
            var countSymbols = 0;
            for (int i = 0; i < xkey.Count; i++)
            {
                for (int j = 0; j < ykey.Count && countSymbols < text.Length; j++)
                {
                    matrixText[xkey[i].NumberInWord, ykey[j].NumberInWord] = text[countSymbols++];
                }
            }
            return matrixText;



        }
        public static char[,] StringToMatrix(string text, int x, int y)
        {
            var matrix = new char[x, y];
            var countSymbols = 0;

            for (int i = 0; i < x; i++)
            {
                for (int j = 0; j < y && countSymbols < text.Length; j++)
                {
                    matrix[i, j] = text[countSymbols++];
                }
            }
            return matrix;
        }
        public static string MatrixToString(char[,] matrix)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    s.Append(matrix[i, j]);
                }
            }
            return s.ToString();
        }
        public static string ReadFromFile(string path)
        {
            using (StreamReader reader = new StreamReader(path))
            {
                return reader.ReadLine();
            }
        }
        public static void WriteToFile(string text, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(text);
            }
        }
        /// <summary>
        /// Заполнение символами списка с ключом.
        /// </summary>
        /// <param name="chars">массив символов.</param>
        /// <returns>Список символов.</returns>
        public static List<CharNum> FillListKey(char[] chars)
        {
            List<CharNum> listKey = new List<CharNum>(chars.Length);

            for (int i = 0; i < chars.Length; i++)
            {
                CharNum charNum = new CharNum()
                {
                    Ch = chars[i],
                    NumberInWord = 1
                };

                listKey.Add(charNum);
            }
            return listKey;
        }
        /// <summary>
        /// Отображение ключа.
        /// </summary>
        /// <param name="listCharNum">Список в котором содержатся символы с порядковыми номерами.</param>
        public static void ShowKey(List<CharNum> listCharNum, string message)
        {
            Console.WriteLine(message);

            foreach (var i in listCharNum)
            {
                Console.Write(i.Ch + " ");
            }
            Console.WriteLine();

            foreach (var i in listCharNum)
            {
                Console.Write(i.NumberInWord + " ");
            }
            Console.WriteLine();
            Console.WriteLine();
        }
        /// <summary>
        /// Заполнение символов ключей, порядковыми номерами.
        /// </summary>
        /// <param name="listCharNum"></param>
        /// <returns></returns>
        public static List<CharNum> FillingSerialsNumber(
            List<CharNum> listCharNum)
        {
            int count = 0;

            var result = listCharNum.OrderBy(a => a.Ch);

            foreach (var i in result)
            {
                i.NumberInWord = count++;
            }

            return listCharNum;
        }

        #endregion Methods
    }

}

