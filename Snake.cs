using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace JuegoSnake
{
    public enum Direccion
    {
        Derecha,
        Izquierda,
        Arriba,
        Abajo
    }

    public class Snake
    {
        public bool Vivo { get; set; } = true;
        public ConsoleColor ColorCabeza { get; set; }
        public ConsoleColor ColorCuerpo { get; set; }
        public Ventana VentanaC { get; set; }
        public Comida ComidaC { get; set; }

        public Point Cabeza { get; set; }
        public Point PosicionInicial { get; set; }
        public List<Point> Cuerpo { get; set; }

        public Direccion DireccionActual { get; set; } = Direccion.Derecha;

        private bool comiendo = false;

        public int Puntaje { get; set; } = 0;
        public int PuntajeMaximo { get; set; } = 0;

        public Snake(Point posicionInicial, ConsoleColor colorCabeza, ConsoleColor colorCuerpo, Ventana ventana, Comida comida)
        {
            PosicionInicial = posicionInicial;
            Cabeza = posicionInicial;
            ColorCabeza = colorCabeza;
            ColorCuerpo = colorCuerpo;
            VentanaC = ventana;
            ComidaC = comida;

            Cuerpo = new List<Point>();
        }

        public void Init()
        {
            Console.Clear();
            Cabeza = PosicionInicial;
            Cuerpo.Clear();
            IniciarCuerpo(2);
            Vivo = true;
            DireccionActual = Direccion.Derecha;

            if (ComidaC != null)
            {
                ComidaC.GenerarComida(this);
            }
        }

        public void IniciarCuerpo(int numPartes)
        {
            int x = Cabeza.X - 1;

            for (int i = 0; i < numPartes; i++)
            {
                Console.SetCursorPosition(x, Cabeza.Y);
                Console.WriteLine("▓");

                Cuerpo.Add(new Point(x, Cabeza.Y));
                x--;
            }
        }

        public void Mover()
        {
            Teclado();

            Point posCabezaAnterior = Cabeza;

            MoverCabeza();
            MoverCuerpo(posCabezaAnterior);

            ColisionesComida();

            if (ColisionesCuerpo())
            {
                Muerte();
                VentanaC.GameOver("G A M E   O V E R");
            }
        }

        public void MoverMenu()
        {
            Point posCabezaAnterior = Cabeza;
            DireccionActual = Direccion.Derecha;

            MoverCabeza();
            MoverCuerpo(posCabezaAnterior);
        }

        private void MoverCabeza()
        {
            Console.ForegroundColor = ColorCabeza;

            Console.SetCursorPosition(Cabeza.X, Cabeza.Y);
            Console.WriteLine(" ");

            Point nuevaCabeza = Cabeza;

            switch (DireccionActual)
            {
                case Direccion.Derecha:
                    nuevaCabeza.X++;
                    break;
                case Direccion.Izquierda:
                    nuevaCabeza.X--;
                    break;
                case Direccion.Abajo:
                    nuevaCabeza.Y++;
                    break;
                case Direccion.Arriba:
                    nuevaCabeza.Y--;
                    break;
            }

            Cabeza = nuevaCabeza;

            ColisionesMarco();

            Console.SetCursorPosition(Cabeza.X, Cabeza.Y);
            Console.WriteLine("█");
        }

        private void MoverCuerpo(Point posCabezaAnterior)
        {
            Console.ForegroundColor = ColorCuerpo;

            Console.SetCursorPosition(posCabezaAnterior.X, posCabezaAnterior.Y);
            Console.WriteLine("▓");

            Cuerpo.Insert(0, posCabezaAnterior);

            if (comiendo)
            {
                comiendo = false;
                return;
            }

            Point ultimaPos = Cuerpo[Cuerpo.Count - 1];
            Console.SetCursorPosition(ultimaPos.X, ultimaPos.Y);
            Console.WriteLine(" ");

            Cuerpo.RemoveAt(Cuerpo.Count - 1);
        }

        private void Teclado()
        {
            if (Console.KeyAvailable)
            {
                ConsoleKeyInfo tecla = Console.ReadKey(true);

                if (tecla.Key == ConsoleKey.RightArrow && DireccionActual != Direccion.Izquierda)
                    DireccionActual = Direccion.Derecha;

                if (tecla.Key == ConsoleKey.LeftArrow && DireccionActual != Direccion.Derecha)
                    DireccionActual = Direccion.Izquierda;

                if (tecla.Key == ConsoleKey.UpArrow && DireccionActual != Direccion.Abajo)
                    DireccionActual = Direccion.Arriba;

                if (tecla.Key == ConsoleKey.DownArrow && DireccionActual != Direccion.Arriba)
                    DireccionActual = Direccion.Abajo;
            }
        }

        private void ColisionesMarco()
        {
            if (Cabeza.X <= VentanaC.LimiteSuperior.X)
                Cabeza = new Point(VentanaC.LimiteInferior.X - 1, Cabeza.Y);

            if (Cabeza.X >= VentanaC.LimiteInferior.X)
                Cabeza = new Point(VentanaC.LimiteSuperior.X + 1, Cabeza.Y);

            if (Cabeza.Y <= VentanaC.LimiteSuperior.Y)
                Cabeza = new Point(Cabeza.X, VentanaC.LimiteInferior.Y - 1);

            if (Cabeza.Y >= VentanaC.LimiteInferior.Y)
                Cabeza = new Point(Cabeza.X, VentanaC.LimiteSuperior.Y + 1);
        }

        private void ColisionesComida()
        {
            if (ComidaC != null && Cabeza == ComidaC.Posicion)
            {
                comiendo = true;
                Puntaje++;

                if (Puntaje > PuntajeMaximo)
                    PuntajeMaximo = Puntaje;

                if (!ComidaC.GenerarComida(this))
                {
                    Vivo = false;
                    VentanaC.GameOver("J U E G O   C O M P L E T A D O");
                }
            }
        }

        private bool ColisionesCuerpo()
        {
            foreach (Point parte in Cuerpo)
            {
                if (Cabeza == parte)
                {
                    Vivo = false;
                    return true;
                }
            }
            return false;
        }

        private void Muerte()
        {
            Console.ForegroundColor = ColorCuerpo;

            foreach (Point parte in Cuerpo)
            {
                if (parte != Cabeza)
                {
                    Console.SetCursorPosition(parte.X, parte.Y);
                    Console.WriteLine("░");
                    Thread.Sleep(120);
                }
            }
        }

        public void Informacion(int dist1, int dist2)
        {
            Console.ForegroundColor = ConsoleColor.White;

            Console.SetCursorPosition(VentanaC.LimiteSuperior.X + dist1, VentanaC.LimiteSuperior.Y - 1);
            Console.Write($"Puntaje: {Puntaje}  ");

            Console.SetCursorPosition(VentanaC.LimiteSuperior.X + dist2, VentanaC.LimiteSuperior.Y - 1);
            Console.Write($"Puntaje Máximo: {PuntajeMaximo}  ");
        }
    }
}