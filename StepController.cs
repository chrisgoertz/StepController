using System;

namespace StateMachineExample
{
    // --- Interface für alle States ---
    public interface IState
    {
        void Enter();
        void Execute(StepController controller);
        void Exit();
    }

    // --- Controller, der die States verwaltet ---
    public class StepController
    {
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState.Enter();
        }

        public void Update()
        {
            _currentState?.Execute(this);
        }
    }

    // --- Konkrete States ---
    public class InitState : IState
    {
        public void Enter() => Console.WriteLine("Init gestartet...");
        public void Execute(StepController controller)
        {
            Console.WriteLine("Initialisierung abgeschlossen.");
            controller.ChangeState(new WaitForInputState());
        }
        public void Exit() => Console.WriteLine("Init beendet.\n");
    }

    public class WaitForInputState : IState
    {
        private string? _input;

        public void Enter() => Console.WriteLine("Bitte eine Eingabe machen (z. B. deinen Namen):");

        public void Execute(StepController controller)
        {
            _input = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(_input))
            {
                controller.ChangeState(new ProcessDataState(_input));
            }
            else
            {
                Console.WriteLine("Leere Eingabe, bitte erneut eingeben:");
            }
        }

        public void Exit() => Console.WriteLine("Eingabe erhalten.\n");
    }

    public class ProcessDataState : IState
    {
        private readonly string _data;

        public ProcessDataState(string data)
        {
            _data = data;
        }

        public void Enter() => Console.WriteLine("Verarbeite Daten...");

        public void Execute(StepController controller)
        {
            Console.WriteLine($"Hallo {_data}, schön dich kennenzulernen!");
            controller.ChangeState(new FinishState());
        }

        public void Exit() => Console.WriteLine("Datenverarbeitung abgeschlossen.\n");
    }

    public class FinishState : IState
    {
        public void Enter() => Console.WriteLine("Programm beendet. Auf Wiedersehen!");
        public void Execute(StepController controller) { }
        public void Exit() { }
    }

    // --- Main Program ---
    class Program
    {
        static void Main(string[] args)
        {
            var controller = new StepController();
            controller.ChangeState(new InitState());

            // Main Loop
            while (true)
            {
                controller.Update();
                if (Console.KeyAvailable) // Abbruch mit ESC
                {
                    if (Console.ReadKey(true).Key == ConsoleKey.Escape)
                        break;
                }
            }
        }
    }
}
