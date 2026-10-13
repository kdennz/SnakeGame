using System;
using System.Drawing;
using System.Threading;

namespace JuegoSnake
{
    internal class Program
    {
        private static Ventana ventana;
        private static Snake snake;
        private static Comida comida;

        private static bool ejecucion = true;
        private static bool jugar = false;

        static void Main(string[] args)
        {
            Iniciar();
            Game();
        }

        private static void Iniciar()
        {
            Point limiteSuperior = new Point(5, 3);
            Point limiteInferior = new Point(59, 18);

            ventana = new Ventana("Snake", 65, 20, ConsoleColor.Black, ConsoleColor.White, limiteSuperior, limiteInferior);
            ventana.DibujarMarco();

            comida = new Comida(ConsoleColor.Green, ventana);
            snake = new Snake(new Point(8, 4), ConsoleColor.Red, ConsoleColor.White, ventana, comida);
        }

        private static void Game()
        {
            while (ejecucion)
            {
                ventana.Menu();
                ventana.Teclado(ref ejecucion, ref jugar, snake);

                while (jugar)
                {
                    snake.Informacion(0, 34);
                    snake.Mover();

                    if (!snake.Vivo)
                    {
                        jugar = false;
                        snake.Puntaje = 0;
                    }

                    Thread.Sleep(100);
                }

                Thread.Sleep(100);
            }
        }
    }
}