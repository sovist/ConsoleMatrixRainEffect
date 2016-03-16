using System;
using System.Threading;
using System.Threading.Tasks;

namespace Matrix
{
    internal static class ConsoleMatrixRainEffect
    {
        private static readonly Random Random = new Random();
        private static readonly object WriteCharSync = new object();
        private static readonly object TaskCountSync = new object();
        private static int _currentTaskCount;
        private static bool _isRunning;

        private const int RefreshInterval = 65;
        private const int MaxTaskCount = 15;

        private static readonly char[] Chars =
        {
            'A', 'B', 'C', 'D',
            'E', 'F', 'G', 'H',
            'I', 'J', 'K', 'L',
            'M', 'N', 'O', 'P',
            'Q', 'R', 'S', 'T',
            'U', 'V', 'W', 'X',
            'Y', 'Z', '@', '$',
            '%', '#', '0', '1',
            '2', '3', '4', '5',
            '6', '7', '8', '9'
        };

        public static void Run()
        {
            //ThreadPool.SetMinThreads(MaxTaskCount, MaxTaskCount);
            Console.CursorVisible = false;
            _isRunning = true;
            _currentTaskCount = 0;

            while (true)
            {
                Thread.Sleep(200);

                if (!_isRunning)
                    return;

                if (_currentTaskCount >= MaxTaskCount)
                    continue;

                Task.Factory.StartNew(runSequence);
            }
        }

        private static void writeChar(int left, int top, ConsoleColor charColor, char c)
        {
            if (left < 0 || top < 0 || top >= Console.WindowHeight)
                return;

            lock (WriteCharSync)
            {
                Console.SetCursorPosition(left, top);
                Console.ForegroundColor = charColor;
                Console.Write(c);
            }
        }
        private static void writeRandChar(int left, int top, ConsoleColor charColor)
        {
            var randChar = Chars[Random.Next(0, Chars.Length - 1)];
            writeChar(left, top, charColor, randChar);
        }
        private static void clearChar(int left, int top)
        {
            writeChar(left, top, ConsoleColor.Black, ' ');
        }

        private static void runSequence()
        {
            lock (TaskCountSync)
                _currentTaskCount++;

            var windowHeight = Console.WindowHeight - 1;
            var sequenceLength = Random.Next(4, windowHeight);
            var randColumn = Random.Next(Console.WindowWidth);
            var headPosition = 0;

            //create and move next
            for (; headPosition < windowHeight; headPosition++)
            {
                if (headPosition - sequenceLength >= 0)
                    clearChar(randColumn, headPosition - sequenceLength);
                else if (headPosition < sequenceLength)
                    clearChar(randColumn, windowHeight - sequenceLength + headPosition);

                for (var i = 0; i < sequenceLength && headPosition - i >= 0; i++)
                    writeRandChar(randColumn, headPosition - i, ConsoleColor.DarkGreen);

                writeRandChar(randColumn, headPosition, ConsoleColor.Green);
                writeRandChar(randColumn, headPosition + 1, ConsoleColor.White);

                Thread.Sleep(RefreshInterval);
                if (!_isRunning)
                    return;
            }

            //end
            for (var i = 0; i <= sequenceLength; i++)
            {
                var color = i == 0 ? ConsoleColor.Green : ConsoleColor.DarkGreen;
                writeRandChar(randColumn, headPosition, color);

                for (var j = 1; j < sequenceLength - i && headPosition - j >= 0; j++)
                    writeRandChar(randColumn, headPosition - j, ConsoleColor.DarkGreen);

                clearChar(randColumn, windowHeight - sequenceLength + i);

                Thread.Sleep(RefreshInterval);
                if (!_isRunning)
                    return;
            }

            lock (TaskCountSync)
                _currentTaskCount--;
        }

        public static void Stop()
        {
            _isRunning = false;
            Thread.Sleep(200);
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.White;
            Console.CursorVisible = true;
        }

        /*private static char getRandChar()
        {
            return Chars[Random.Next(0, Chars.Length - 1)];
        }

        private static void v2()
        {
            var len = 15;
            var random = new Random();
            var firstLine = new int[Console.WindowWidth];
            while (true)
            {
                for (var i = 0; i < 2; i++)
                {
                    var x = random.Next(Console.WindowWidth);
                    firstLine[x] = len;
                }

                for (var i = 0; i < Console.WindowWidth; i++)
                {
                    if (firstLine[i] == 0)
                        continue;

                    if (firstLine[i] == len)
                        Console.ForegroundColor = ConsoleColor.White;
                    else if (firstLine[i] == len - 1)
                        Console.ForegroundColor = ConsoleColor.Green;
                    else
                        Console.ForegroundColor = ConsoleColor.DarkGreen;

                    firstLine[i]--;
                    Console.SetCursorPosition(i, 0);
                    Console.Write(getRandChar());
                }

                Thread.Sleep(80);
                Console.MoveBufferArea(0, 0, Console.WindowWidth, Console.WindowHeight, 0, 1);
            }
        }

        private static void v1()
        {
            var random = new Random();
            while (true)
            {
                var x = random.Next(Console.WindowWidth);
                var y = 6;

                Console.ForegroundColor = ConsoleColor.White;
                Console.SetCursorPosition(x, y);
                Console.Write(getRandChar());

                if (y - 1 >= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.SetCursorPosition(x, y - 1);
                    Console.Write(getRandChar());
                    Thread.Sleep(10);
                }

                for (var i = 0; i < 5; i++)
                {
                    if (y - 2 - i < 0)
                        break;

                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    Console.SetCursorPosition(x, y - 2 - i);
                    Console.Write(getRandChar());
                    Thread.Sleep(10);
                }

                Console.MoveBufferArea(0, 0, Console.WindowWidth, Console.WindowHeight, 0, 1);
            }
        }*/
    }
}
